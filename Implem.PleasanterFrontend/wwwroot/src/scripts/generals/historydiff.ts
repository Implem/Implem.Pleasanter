import { diffChars, diffLines, type Change } from 'diff';

// image-viewer-modal はモジュール側のバンドルで定義済み。
// ここで実体を取り込むとカスタム要素の登録が二重になるため、型だけを借りる。
import type { ImageViewerModal } from './modal/imageViewerModal';

const panelSelector = '#FieldSetHistoryDiff';

type DiffMode = 'rendered' | 'source';

// 項目ごとの表示の状態。バージョンを選び直すと中身が入れ替わるため、
// 画面が差し替わっても選んだ状態が残るようにカラム名で覚えておく。
const sourceModeColumns = new Set<string>();
const pinnedColumns = new Set<string>();

function escapeHtml(value: string): string {
    return value
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/\n/g, '<br>');
}

// 差分の計算に掛けてよい時間。長い文章同士でほぼ全面的に違う場合は計算量が膨らむため、
// 超えたら色分けを諦めて「全部消えて全部増えた」として見せる。
const diffTimeout = 1000;

type DiffSide = 'old' | 'new';

// 片側 (旧 / 新) から見た差分の塊。marked は削除 (旧) / 追加 (新) の印を付けるか。
type DiffPart = { value: string; marked: boolean };

type DiffParts = Record<DiffSide, DiffPart[]>;

// 差分の塊を片側の並びへ分ける。変更の無い塊は左右で同じものを共有しているため、
// 後で片側だけ手を加えられるよう、ここで別のものとして作り直す。
function toSideParts(changes: Change[], side: DiffSide): DiffPart[] {
    return changes
        .filter(change => (side === 'old' ? !change.added : !change.removed))
        .map(change => ({
            value: change.value,
            marked: side === 'old' ? change.removed : change.added
        }));
}

// 比較のために末尾へ足した改行を、片側の並びから取り除く。
// 空欄との比較では足した改行だけの塊が空になって残るため、それも除く。
function trimAlignedEol(parts: DiffPart[]): DiffPart[] {
    const last = parts[parts.length - 1];
    if (last?.value.endsWith('\n')) {
        last.value = last.value.slice(0, -1);
    }
    return parts.filter(part => part.value !== '');
}

// 行単位の比較。末尾の行だけは改行を持たないため、そのままでは後ろに行が足されただけで
// 「・A」と「・A\n」が別の行と見なされ、変わっていない行にも印が付いてしまう。
// 両方の末尾に改行を足して揃えてから比較し、比較のあとで足した改行を取り除く。
function diffLinesAlignedEol(oldText: string, newText: string): DiffParts | undefined {
    const changes = diffLines(`${oldText}\n`, `${newText}\n`, { timeout: diffTimeout });
    if (!changes) {
        return undefined;
    }
    return {
        old: trimAlignedEol(toSideParts(changes, 'old')),
        new: trimAlignedEol(toSideParts(changes, 'new'))
    };
}

function diffCharsParts(oldText: string, newText: string): DiffParts | undefined {
    const changes = diffChars(oldText, newText, { timeout: diffTimeout });
    if (!changes) {
        return undefined;
    }
    return {
        old: toSideParts(changes, 'old'),
        new: toSideParts(changes, 'new')
    };
}

function toDiffParts(oldText: string, newText: string): DiffParts {
    // 複数行の項目は行単位、それ以外は文字単位で比較する。
    // 日本語は単語境界が無いため、文字単位のほうが差分が読み取りやすい。
    const parts =
        oldText.indexOf('\n') !== -1 || newText.indexOf('\n') !== -1
            ? diffLinesAlignedEol(oldText, newText)
            : diffCharsParts(oldText, newText);
    return (
        parts ?? {
            old: [{ value: oldText, marked: true }],
            new: [{ value: newText, marked: true }]
        }
    );
}

function toDiffHtml(parts: DiffPart[], side: DiffSide): string {
    const css = side === 'old' ? 'history-diff-removed' : 'history-diff-added';
    return parts
        .map(part => {
            const text = escapeHtml(part.value);
            return part.marked ? `<span class="${css}">${text}</span>` : text;
        })
        .join('');
}

// コメントなどに含まれる画像は ![代替文字](URL) の記法で入っている。
// そのままだと文字列として出てしまうため、画像として描画する。
// URL はレコードの値なので、相対パスと http(s) のみ通す。
const imageMarkdown = /!\[([^\]]*)\]\((\/[^)\s]+|https?:\/\/[^)\s]+)\)/g;

function renderImages(cell: HTMLElement): void {
    // 文字列で HTML を組み立てず、DOM API で属性を設定する。
    // 代替テキストに引用符などが含まれていても、属性値の一部として扱われる。
    const walker = document.createTreeWalker(cell, NodeFilter.SHOW_TEXT);
    const targets: Text[] = [];
    for (let node = walker.nextNode(); node; node = walker.nextNode()) {
        targets.push(node as Text);
    }
    for (const node of targets) {
        const text = node.data;
        const fragment = document.createDocumentFragment();
        let lastIndex = 0;
        for (const match of text.matchAll(imageMarkdown)) {
            const index = match.index ?? 0;
            fragment.appendChild(document.createTextNode(text.slice(lastIndex, index)));
            const image = document.createElement('img');
            image.className = 'history-diff-image';
            image.setAttribute('src', match[2]);
            image.setAttribute('alt', match[1]);
            fragment.appendChild(image);
            lastIndex = index + match[0].length;
        }
        if (lastIndex > 0) {
            fragment.appendChild(document.createTextNode(text.slice(lastIndex)));
            node.replaceWith(fragment);
        }
    }
}

function valueCell($row: JQuery<HTMLElement>, mode: DiffMode): JQuery<HTMLElement> {
    return $row.find(`.history-diff-value-inner[data-mode="${mode}"]`);
}

// 指定した表示の色分けをまだ入れていなければ入れる。
// レンダリング前の値は文字数が多くなりやすいため、実際に開いたときにだけ計算する。
function renderInlineDiffs($diff: JQuery<HTMLElement>, mode: DiffMode): void {
    const $newPane = $diff.find('.history-diff-pane[data-side="new"]');
    $diff.find('.history-diff-pane[data-side="old"] .history-diff-row.changed').each((_, row) => {
        const $oldRow = $(row);
        const columnName = $oldRow.attr('data-column');
        if (!columnName) {
            return;
        }
        const $newRow = $newPane.find(`.history-diff-row[data-column="${columnName}"]`);
        if ($newRow.length === 0) {
            return;
        }
        const $oldValue = valueCell($oldRow, mode);
        const $newValue = valueCell($newRow, mode);
        if ($oldValue.length === 0 || $oldValue.attr('data-diffed') === '1') {
            return;
        }
        const parts = toDiffParts($oldValue.text(), $newValue.text());
        $oldValue.html(toDiffHtml(parts.old, 'old')).attr('data-diffed', '1');
        $newValue.html(toDiffHtml(parts.new, 'new')).attr('data-diffed', '1');
    });
    if (mode === 'rendered') {
        // レンダリング前は記法をそのまま見せるため、画像へ置き換えるのは後の表示だけ。
        // 差異が無い項目にも画像は入るため、色分けの対象とは別にすべて回す。
        $diff.find('.history-diff-value-inner[data-mode="rendered"]').each((_, cell) => {
            const $cell = $(cell);
            if ($cell.attr('data-imaged') === '1') {
                return;
            }
            renderImages(cell);
            $cell.attr('data-imaged', '1');
        });
    }
}

// 左右の行の高さを揃え、同じ項目が同じ行位置に並ぶようにする。
// バージョン表記の折り返しで見出しの高さも変わるため、見出し行も対象に含める。
function visibleRows($diff: JQuery<HTMLElement>, side: string): JQuery<HTMLElement> {
    return $diff.find(`.history-diff-pane[data-side="${side}"] .history-diff-fields tr`).filter(':visible');
}

function syncRowHeights($diff: JQuery<HTMLElement>): void {
    const $oldRows = visibleRows($diff, 'old');
    const $newRows = visibleRows($diff, 'new');
    $oldRows.css('height', '');
    $newRows.css('height', '');
    $oldRows.each((index, row) => {
        const $newRow = $newRows.eq(index);
        if ($newRow.length === 0) {
            return;
        }
        const height = Math.max($(row).outerHeight() ?? 0, $newRow.outerHeight() ?? 0);
        $(row).css('height', `${height}px`);
        $newRow.css('height', `${height}px`);
    });
}

function applyChangedOnly($diff: JQuery<HTMLElement>): void {
    const changedOnly = $diff.find('#HistoryDiffChangedOnly').prop('checked') === true;
    $diff.toggleClass('changed-only', changedOnly);
    $diff.find('.history-diff-empty-filtered').remove();
    // 差異が無い区画は見出しごと隠すため、案内はどこにも差異が無いときだけ出す
    if (changedOnly && $diff.find('.history-diff-row.changed').length === 0) {
        $diff.append(`<p class="history-diff-empty history-diff-empty-filtered">${$p.display('NoChangedColumns')}</p>`);
    }
    syncRowHeights($diff);
}

// 左右の同じ項目をまとめて扱う。片側だけ変わると見比べられなくなるため。
function pairedRows($diff: JQuery<HTMLElement>, columnName: string): JQuery<HTMLElement> {
    return $diff.find(`.history-diff-row[data-column="${columnName}"]`);
}

// 折りたたまれる項目にだけピン留めのボタンを出す。
// 一行に収まる項目にまでボタンが並ぶと、かえって読みにくくなるため。
function markOverflowingRows($diff: JQuery<HTMLElement>): void {
    const $rows = $diff.find('.history-diff-row');
    // 開いている行はそのままでは折りたたみの要否を測れないため、いったん閉じて測る
    const $hovered = $rows.filter('.hovered');
    const $pinned = $rows.filter('.pinned');
    $hovered.removeClass('hovered');
    $pinned.removeClass('pinned');
    const overflowing = new Set<string>();
    $rows.each((_, row) => {
        const columnName = $(row).attr('data-column');
        const cell = $(row).find('.history-diff-value-inner:visible')[0];
        if (columnName && cell && cell.scrollHeight > cell.clientHeight + 1) {
            overflowing.add(columnName);
        }
    });
    $hovered.addClass('hovered');
    $pinned.addClass('pinned');
    $rows.each((_, row) => {
        const $row = $(row);
        const columnName = $row.attr('data-column');
        // ピン留め中はボタンを消さない。消すと留めたままにできなくなるため。
        $row.toggleClass(
            'overflowing',
            $row.hasClass('pinned') || (columnName !== undefined && overflowing.has(columnName))
        );
    });
}

function applySourceMode($row: JQuery<HTMLElement>, source: boolean): void {
    $row.toggleClass('source-mode', source);
    $row.find('.history-diff-source-toggle')
        .toggleClass('on', source)
        .attr('title', $p.display(source ? 'CompareRendered' : 'CompareSource'));
}

function applyPinned($row: JQuery<HTMLElement>, pinned: boolean): void {
    $row.toggleClass('pinned', pinned);
    $row.find('.history-diff-pin')
        .toggleClass('on', pinned)
        .attr('title', $p.display(pinned ? 'UnpinExpanded' : 'PinExpanded'))
        .find('.ui-icon')
        .toggleClass('ui-icon-pin-s', pinned)
        .toggleClass('ui-icon-pin-w', !pinned);
}

// 「変更差分」タブの中身が差し替わったあとにサーバ (Invoke) から呼ばれる。
const initHistoryDiff = (): void => {
    const $diff = $(panelSelector).find('.history-diff');
    if ($diff.length === 0) {
        return;
    }
    // 前に選んでいた表示を戻してから色分けする
    $diff.find('.history-diff-row[data-column]').each((_, row) => {
        const $row = $(row);
        const columnName = $row.attr('data-column') ?? '';
        applySourceMode($row, sourceModeColumns.has(columnName));
    });
    renderInlineDiffs($diff, 'rendered');
    if (sourceModeColumns.size > 0) {
        renderInlineDiffs($diff, 'source');
    }
    // 折りたたんだ状態で測るため、ピン留めを戻すのは高さを調べたあと
    markOverflowingRows($diff);
    $diff.find('.history-diff-row[data-column]').each((_, row) => {
        const $row = $(row);
        applyPinned($row, pinnedColumns.has($row.attr('data-column') ?? ''));
    });
    applyChangedOnly($diff);
    // 画像は読み込みが終わるまで高さが決まらないため、揃え直す
    $diff.find('.history-diff-image').on('load', () => {
        markOverflowingRows($diff);
        syncRowHeights($diff);
    });
};

// マウスオーバーは左右の同じ項目へまとめて効かせる。
// 塗りつぶしも折りたたみの展開も、片側だけだと見比べられなくなるため。
function toggleHoveredRow($row: JQuery<HTMLElement>, hovered: boolean): void {
    const $diff = $row.closest('.history-diff');
    const columnName = $row.attr('data-column');
    if ($diff.length === 0 || columnName === undefined) {
        return;
    }
    pairedRows($diff, columnName).toggleClass('hovered', hovered);
    syncRowHeights($diff);
}

// 拡大表示のライトボックス。開いたときに一度だけ作り、以降は使い回す。
let imageViewerModal: ImageViewerModal | undefined;

// 差分に出ている画像を拡大して見せる。
// ライトボックスが使えない設定・環境では、他の画面と同じく別のタブで開く。
function showImageViewer($diff: JQuery<HTMLElement>, image: HTMLImageElement): void {
    const source = image.src.split('?thumbnail')[0];
    if ($diff.attr('data-enablelightbox') !== '1' || !customElements.get('image-viewer-modal')) {
        window.open(source, '_blank', 'noopener,noreferrer');
        return;
    }
    if (!imageViewerModal) {
        // マークダウンの入力欄などが既に作っていれば、それを使い回す。
        // 表示前に中身は入れ替わるため、画面に一つあれば足りる。
        imageViewerModal = document.querySelector<ImageViewerModal>('body > image-viewer-modal') ?? undefined;
    }
    if (!imageViewerModal) {
        imageViewerModal = document.createElement('image-viewer-modal') as ImageViewerModal;
        document.body.appendChild(imageViewerModal);
    }
    // 送り戻しは同じバージョンの中だけにする。左右をまたぐと今どちらを見ているのか分からなくなるため。
    const $pane = $(image).closest('.history-diff-pane');
    imageViewerModal.show(image, ($pane[0] ?? $diff[0]).querySelectorAll<HTMLImageElement>('.history-diff-image'));
}

// 履歴の一覧で版を開いたあと、続けて差分を見たい場面が多いため変更差分のタブを開く。
function activateHistoryDiffTab(): void {
    const $container = $('#EditorTabsContainer');
    if (!$container.hasClass('ui-tabs')) {
        return;
    }
    const $tabs = $('#EditorTabs > li:not(.ignore-tab)');
    const $target = $(`#EditorTabs > li > a[href="${panelSelector}"]`).parent();
    if ($target.length === 0) {
        return;
    }
    const index = $tabs.index($target);
    if (index < 0) {
        return;
    }
    // タブの切り替えで beforeActivate が動き、差分の中身が読み込まれる。
    $container.tabs('option', 'active', index);
}

$(function () {
    $(document).on('click', '.grid.history .grid-row td', function () {
        // 確認で取り消した場合など、画面が差し替わっていないときは何もしない。
        // 差し替わっていれば、クリックしたセルは文書から外れている。
        if (!document.contains(this)) {
            activateHistoryDiffTab();
        }
    });
    $(document).on('mouseenter', '.history-diff-row', function () {
        toggleHoveredRow($(this), true);
    });
    $(document).on('mouseleave', '.history-diff-row', function () {
        toggleHoveredRow($(this), false);
    });
    $(document).on('click', '.history-diff-image', function (event) {
        event.preventDefault();
        showImageViewer($(this).closest('.history-diff'), this as HTMLImageElement);
    });
    $(document).on('change', '#HistoryDiffChangedOnly', function () {
        applyChangedOnly($(this).closest('.history-diff'));
    });
    $(document).on('click', '.history-diff-source-toggle', function () {
        const $diff = $(this).closest('.history-diff');
        const columnName = $(this).closest('.history-diff-row').attr('data-column');
        if ($diff.length === 0 || columnName === undefined) {
            return;
        }
        const source = !sourceModeColumns.has(columnName);
        if (source) {
            sourceModeColumns.add(columnName);
            // レンダリング前は文字数が多くなりやすいため、開いたときに色分けする
            renderInlineDiffs($diff, 'source');
        } else {
            sourceModeColumns.delete(columnName);
        }
        applySourceMode(pairedRows($diff, columnName), source);
        markOverflowingRows($diff);
        syncRowHeights($diff);
    });
    $(document).on('click', '.history-diff-pin', function () {
        const $diff = $(this).closest('.history-diff');
        const columnName = $(this).closest('.history-diff-row').attr('data-column');
        if ($diff.length === 0 || columnName === undefined) {
            return;
        }
        const pinned = !pinnedColumns.has(columnName);
        if (pinned) {
            pinnedColumns.add(columnName);
        } else {
            pinnedColumns.delete(columnName);
        }
        applyPinned(pairedRows($diff, columnName), pinned);
        syncRowHeights($diff);
    });
});

export const HistoryDiff = {
    initHistoryDiff
};

Object.assign($p, HistoryDiff);
