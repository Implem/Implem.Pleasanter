import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { userEvent } from 'vitest/browser';

import { installPleasanterGlobals } from '../../../test/setup';
import { ImageViewerModal } from '../../generals/modal/imageViewerModal';
import {
    addHiddenInput,
    figures,
    installRteErrorSuppression,
    resetRteDom,
    teardownEditors
} from './richTextEditor.browserHelpers';
import './richTextEditor'; // 実 SunEditor を含む本物のモジュール (モックしない)

/**
 * 実ブラウザ (Playwright/Chromium) で本物の SunEditor を動かす「編集モード」のテスト。
 *
 * happy-dom の単体テストではモックしていた実プラグイン挙動 (文字装飾・リスト・表・画像
 * アップロード・貼付整形など) を、実際のツールバー操作 → DOM 反映で検証する。
 * 読み取り専用ビューアの閲覧挙動は richTextEditor.viewer.browser.test.ts が担当する。
 */

installRteErrorSuppression();

/** C# が用意する環境 (グローバル/hidden input) を再現し、編集モードで RTE を接続する */
async function mountRealEditor(
    options: { enablelightbox?: boolean; placeholder?: string; value?: string; language?: string } = {}
) {
    installPleasanterGlobals();
    addHiddenInput('Language', options.language ?? 'en');
    // C# が払い出す #RteFontList / #RteFontSize の本番初期値 (TextEditorUI.json 由来)
    addHiddenInput(
        'RteFontList',
        '游ゴシック,BIZ UDPゴシック,Noto Sans,游明朝,BIZ UDP明朝,Noto Serif JP,Arial,Segoe UI,Calibri,Helvetica,Impact,Times New Roman,ＭＳ ゴシック,ＭＳ Ｐゴシック,ＭＳ 明朝,ＭＳ Ｐ明朝'
    );
    addHiddenInput('RteFontSize', '8,9,10,11,12,14,16,18,20,22,24,26,28,36,48,72');
    addHiddenInput('RteDefaultFont', 'Arial');

    const host = document.createElement('div');
    // 値なし = 編集モードで開く。値ありだと初期トグルで読取(閲覧)モードになる
    const lightboxAttr = options.enablelightbox ? ' data-enablelightbox="1"' : '';
    const placeholderAttr = options.placeholder ? ` placeholder="${options.placeholder}"` : '';
    host.innerHTML = `<rt-editor><textarea id="TestRte"${lightboxAttr}${placeholderAttr}></textarea></rt-editor>`;
    // connectedCallback(setInitContent) より前に値を設定する
    if (options.value !== undefined) {
        (host.querySelector('textarea') as HTMLTextAreaElement).value = options.value;
    }
    document.body.appendChild(host);
    const rt = host.querySelector('rt-editor') as HTMLElement;

    // suneditor の初期化待ち (DOM 生成)
    await vi.waitFor(() => {
        if (!rt.querySelector('.sun-editor-editable') || !rt.querySelector('.se-toolbar')) {
            throw new Error('editor not ready');
        }
    });
    // 遅延初期化 (#editorInit の setTimeout(0)) 完了を待つ。
    // これを待たずにフォーカスすると内部 sticky 処理が未接続の状態を触り例外になる。
    await new Promise(resolve => setTimeout(resolve, 0));
    const wysiwyg = rt.querySelector('.sun-editor-editable') as HTMLElement;
    const toolbar = rt.querySelector('.se-toolbar') as HTMLElement;
    return { rt, wysiwyg, toolbar };
}

/** wysiwyg 内の本文をすべて選択する */
function selectAllText(wysiwyg: HTMLElement) {
    const target = wysiwyg.querySelector('p') ?? wysiwyg;
    wysiwyg.focus();
    const range = document.createRange();
    range.selectNodeContents(target);
    const sel = document.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);
}

/** 指定要素の末尾へキャレット (折り畳んだ選択) を置く */
function placeCaret(el: HTMLElement) {
    el.focus();
    const range = document.createRange();
    range.selectNodeContents(el);
    range.collapse(false); // 末尾へ折り畳む
    const sel = document.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);
}

/** ツールバーのコマンドボタンを押す (SunEditor は mousedown で選択保持 → click で実行) */
function clickCommand(toolbar: HTMLElement, command: string) {
    const btn = toolbar.querySelector(`button[data-command="${command}"]`);
    if (!btn) throw new Error(`command button not found: ${command}`);
    btn.dispatchEvent(new MouseEvent('mousedown', { bubbles: true, cancelable: true }));
    btn.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));
}

/** 色ドロップダウン (fontColor/backgroundColor) を開き、最初のスウォッチを選ぶ。選んだ色値を返す */
async function pickColor(toolbar: HTMLElement, command: string) {
    clickCommand(toolbar, command); // ドロップダウンを開く
    let swatch: HTMLButtonElement | null = null;
    await vi.waitFor(() => {
        // fontColor/backgroundColor の両パレットが DOM に存在するため、
        // 表示中 (開いている) パレットのスウォッチを選ぶ
        const pallets = Array.from(document.querySelectorAll<HTMLElement>('.se-color-pallet'));
        const open = pallets.find(p => p.offsetParent !== null) ?? null;
        swatch = open?.querySelector<HTMLButtonElement>('button[data-value]') ?? null;
        if (!swatch) throw new Error('color palette not open');
    });
    const value = swatch!.getAttribute('data-value')!;
    // スウォッチは色パレット側の click ハンドラで適用される
    swatch!.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));
    return value;
}

/** CSS 色値をブラウザ正規化 (例: '#ff0000' -> 'rgb(255, 0, 0)') して比較可能にする */
function cssColor(value: string, prop: 'color' | 'backgroundColor') {
    const probe = document.createElement('span');
    probe.style[prop] = value;
    return probe.style[prop];
}

/** 表示中 (開いている) の要素だけを対象に取得する */
function findVisible(selector: string): HTMLElement | null {
    return Array.from(document.querySelectorAll<HTMLElement>(selector)).find(e => e.offsetParent !== null) ?? null;
}

async function waitVisible(selector: string): Promise<HTMLElement> {
    let el: HTMLElement | null = null;
    await vi.waitFor(() => {
        el = findVisible(selector);
        if (!el) throw new Error(`not visible: ${selector}`);
    });
    return el!;
}

/** mousedown → click で要素を押す (SunEditor は mousedown で選択保持 → click で実行) */
function pressButton(el: Element) {
    el.dispatchEvent(new MouseEvent('mousedown', { bubbles: true, cancelable: true }));
    el.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));
}

/** ツールバーボタン(セレクタ)でドロップダウン/モーダルを開き、表示中のメニュー項目を押す */
async function openAndClickItem(toolbar: HTMLElement, buttonSelector: string, itemSelector: string) {
    const btn = toolbar.querySelector(buttonSelector);
    if (!btn) throw new Error(`toolbar button not found: ${buttonSelector}`);
    pressButton(btn);
    const item = await waitVisible(itemSelector);
    pressButton(item);
    return item;
}

beforeEach(() => {
    resetRteDom();
});

afterEach(async () => {
    await teardownEditors();
});

describe('文字装飾 (実 SunEditor / ブラウザ)', () => {
    const cases = [
        { command: 'bold', tag: 'strong' },
        { command: 'italic', tag: 'em' },
        { command: 'underline', tag: 'u' },
        { command: 'strike', tag: 'del' }
    ];

    for (const { command, tag } of cases) {
        it(`${command}: 選択テキストへ <${tag}> が適用される`, async () => {
            const { wysiwyg, toolbar } = await mountRealEditor();
            wysiwyg.innerHTML = '<p>hello</p>';
            selectAllText(wysiwyg);

            clickCommand(toolbar, command);

            await vi.waitFor(() => {
                expect(wysiwyg.innerHTML).toContain(`<${tag}>`);
            });
        });
    }

    it('組み合わせ: 太字+斜体+下線+取消線を同一選択に適用できる', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';

        for (const { command, tag } of cases) {
            selectAllText(wysiwyg); // 直前の適用で DOM が変わるため都度選択し直す
            clickCommand(toolbar, command);
            await vi.waitFor(() => {
                expect(wysiwyg.innerHTML).toContain(`<${tag}>`);
            });
        }

        // 4 種すべてが (入れ子で) 残っている
        const html = wysiwyg.innerHTML;
        for (const { tag } of cases) {
            expect(html).toContain(`<${tag}>`);
        }
    });
});

describe('文字色 / 背景色 (実 SunEditor / ブラウザ)', () => {
    it('文字色: 選択テキストに color の span が適用される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        const value = await pickColor(toolbar, 'fontColor');

        await vi.waitFor(() => {
            const span = wysiwyg.querySelector('span');
            expect(span?.style.color).toBeTruthy();
        });
        expect(wysiwyg.querySelector('span')!.style.color).toBe(cssColor(value, 'color'));
    });

    it('背景色: 選択テキストに background-color の span が適用される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        const value = await pickColor(toolbar, 'backgroundColor');

        await vi.waitFor(() => {
            const span = wysiwyg.querySelector('span');
            expect(span?.style.backgroundColor).toBeTruthy();
        });
        expect(wysiwyg.querySelector('span')!.style.backgroundColor).toBe(cssColor(value, 'backgroundColor'));
    });

    it('組み合わせ: 太字 + 文字色 + 背景色 を同一選択に適用できる', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';

        selectAllText(wysiwyg);
        clickCommand(toolbar, 'bold');
        await vi.waitFor(() => expect(wysiwyg.innerHTML).toContain('<strong>'));

        selectAllText(wysiwyg);
        await pickColor(toolbar, 'fontColor');
        await vi.waitFor(() => expect(wysiwyg.querySelector('span[style*="color"]')).toBeTruthy());

        selectAllText(wysiwyg);
        await pickColor(toolbar, 'backgroundColor');

        // 太字と、color/background-color を持つ span がそれぞれ存在する
        // (span の入れ子順は不定なので先頭決め打ちではなく some で判定する)
        await vi.waitFor(() => {
            const spans = Array.from(wysiwyg.querySelectorAll('span'));
            expect(wysiwyg.innerHTML).toContain('<strong>');
            expect(spans.some(s => s.style.color)).toBe(true);
            expect(spans.some(s => s.style.backgroundColor)).toBe(true);
        });
    });
});

describe('フォント / サイズ (実 SunEditor / ブラウザ)', () => {
    it('フォント: 選択に font-family の span が適用される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        await openAndClickItem(
            toolbar,
            'button[data-command="font"]',
            '.se-list-font-family button[data-command="Arial"]'
        );

        await vi.waitFor(() => {
            const span = wysiwyg.querySelector('span');
            expect(span?.style.fontFamily.toLowerCase()).toContain('arial');
        });
    });

    it('フォント: 游ゴシック (日本語フォント) を選択に適用できる', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        await openAndClickItem(
            toolbar,
            'button[data-command="font"]',
            '.se-list-font-family button[data-command="游ゴシック"]'
        );

        await vi.waitFor(() => {
            const span = wysiwyg.querySelector('span');
            expect(span?.style.fontFamily).toContain('游ゴシック');
        });
    });

    it('サイズ: 選択に font-size:16px の span が適用される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        await openAndClickItem(
            toolbar,
            'button[data-command="fontSize"][data-type="dropdown"]',
            '.se-list-font-size button[data-command="16px"]'
        );

        await vi.waitFor(() => {
            const span = wysiwyg.querySelector('span');
            expect(span?.style.fontSize).toBe('16px');
        });
    });

    it('サイズ: 既定の基準サイズは 13px で描画される', async () => {
        const { wysiwyg } = await mountRealEditor();
        // richTextEditor.scss の --se-edit-font-size:13px が効き、素のテキストは 13px 基準
        expect(getComputedStyle(wysiwyg).fontSize).toBe('13px');
    });
});

describe('全書式の同時適用 (実 SunEditor / ブラウザ)', () => {
    it('斜体+下線+取消線+文字色+背景色+フォント+サイズ をすべて同一選択に適用できる', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';

        // 装飾 3種 (直前の適用で DOM が変わるため都度選択し直す)
        for (const { command, tag } of [
            { command: 'italic', tag: 'em' },
            { command: 'underline', tag: 'u' },
            { command: 'strike', tag: 'del' }
        ]) {
            selectAllText(wysiwyg);
            clickCommand(toolbar, command);
            await vi.waitFor(() => expect(wysiwyg.innerHTML).toContain(`<${tag}>`));
        }

        // 文字色 → 背景色
        selectAllText(wysiwyg);
        await pickColor(toolbar, 'fontColor');
        await vi.waitFor(() => expect(wysiwyg.querySelector('span[style*="color"]')).toBeTruthy());
        selectAllText(wysiwyg);
        await pickColor(toolbar, 'backgroundColor');

        // フォント → サイズ
        selectAllText(wysiwyg);
        await openAndClickItem(
            toolbar,
            'button[data-command="font"]',
            '.se-list-font-family button[data-command="游ゴシック"]'
        );
        selectAllText(wysiwyg);
        await openAndClickItem(
            toolbar,
            'button[data-command="fontSize"][data-type="dropdown"]',
            '.se-list-font-size button[data-command="16px"]'
        );

        // 7 種すべての書式が共存する
        await vi.waitFor(() => {
            const html = wysiwyg.innerHTML;
            expect(html).toContain('<em>');
            expect(html).toContain('<u>');
            expect(html).toContain('<del>');
            const spans = Array.from(wysiwyg.querySelectorAll('span'));
            expect(spans.some(s => s.style.color)).toBe(true);
            expect(spans.some(s => s.style.backgroundColor)).toBe(true);
            expect(spans.some(s => s.style.fontFamily)).toBe(true);
            expect(spans.some(s => s.style.fontSize)).toBe(true);
        });
    });
});

describe('リスト (実 SunEditor / ブラウザ)', () => {
    it('番号付きリスト: ol が挿入される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        await openAndClickItem(toolbar, 'button[data-command="list"]', 'button[data-command="ol"]');

        await vi.waitFor(() => expect(wysiwyg.querySelector('ol')).toBeTruthy());
    });

    it('箇条書きリスト: ul が挿入される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        await openAndClickItem(toolbar, 'button[data-command="list"]', 'button[data-command="ul"]');

        await vi.waitFor(() => expect(wysiwyg.querySelector('ul')).toBeTruthy());
    });
});

describe('リンク / 表 (実 SunEditor / ブラウザ)', () => {
    it('リンク: モーダルで URL を入力すると a[href] が挿入される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        // リンクモーダルを開く
        pressButton(toolbar.querySelector('button[data-command="link"]')!);

        // URL 入力 (create() は input イベントで更新される linkValue を参照する)
        const urlInput = (await waitVisible('.se-input-url')) as HTMLInputElement;
        urlInput.value = 'https://example.com';
        urlInput.dispatchEvent(new Event('input', { bubbles: true }));

        // 送信 (submit ボタンでフォーム送信 → modalAction がリンクを挿入)
        (await waitVisible('.se-modal button[type="submit"]')).click();

        await vi.waitFor(() => {
            const a = wysiwyg.querySelector('a[href]');
            expect(a?.getAttribute('href')).toContain('example.com');
        });
    });

    it('表: グリッド選択で figure>table が挿入される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>x</p>';
        selectAllText(wysiwyg);

        // 表ドロップダウンを開く
        pressButton(toolbar.querySelector('button[data-command="table"]')!);

        // グリッドを mousemove でサイズ指定 (セル 18px) → click で挿入
        const picker = await waitVisible('.se-controller-table-picker');
        const rect = picker.getBoundingClientRect();
        picker.dispatchEvent(
            new MouseEvent('mousemove', { bubbles: true, clientX: rect.left + 45, clientY: rect.top + 27 })
        );
        picker.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));

        await vi.waitFor(() => {
            expect(wysiwyg.querySelector('figure table td')).toBeTruthy();
        });
    });
});

describe('表のセル内改行 (実 SunEditor / ブラウザ)', () => {
    /** 1x1 の表を挿入し、最初のセルを返す */
    async function insertTable(wysiwyg: HTMLElement, toolbar: HTMLElement) {
        selectAllText(wysiwyg);
        pressButton(toolbar.querySelector('button[data-command="table"]')!);
        const picker = await waitVisible('.se-controller-table-picker');
        const rect = picker.getBoundingClientRect();
        // 左上 1 セル分だけを指定して挿入する
        picker.dispatchEvent(
            new MouseEvent('mousemove', { bubbles: true, clientX: rect.left + 9, clientY: rect.top + 9 })
        );
        picker.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));
        await vi.waitFor(() => expect(wysiwyg.querySelector('figure table td')).toBeTruthy());
        return wysiwyg.querySelector('figure table td') as HTMLElement;
    }

    it('セル内で Enter を押すと同一セル内で改行し、行(tr)は増えない', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>x</p>';

        const cell = await insertTable(wysiwyg, toolbar);
        const table = wysiwyg.querySelector('figure table') as HTMLTableElement;
        const rowsBefore = table.querySelectorAll('tr').length;

        // セルへキャレットを置き、「a」→ 改行 →「b」を実キー入力する
        placeCaret(cell);
        await userEvent.keyboard('a{Enter}b');

        await vi.waitFor(() => {
            // 行は増えず (Enter がセル内改行として扱われる)、同一セルに a と b が入る
            expect(table.querySelectorAll('tr').length).toBe(rowsBefore);
            expect(cell.textContent).toContain('a');
            expect(cell.textContent).toContain('b');
        });
    });
});

describe('画像アップロード (実 D&D / ツールバー / ブラウザ)', () => {
    /** buildUploadUrl 用の .main-form を用意する */
    function addMainForm() {
        const form = document.createElement('form');
        form.className = 'main-form';
        form.setAttribute('action', '/items/1/_action_');
        document.body.appendChild(form);
    }

    /** wysiwyg にファイルをドラッグ&ドロップする */
    function dropFile(wysiwyg: HTMLElement, file: File) {
        const dt = new DataTransfer();
        dt.items.add(file);
        const rect = wysiwyg.getBoundingClientRect();
        const coords = { bubbles: true, cancelable: true, clientX: rect.left + 10, clientY: rect.top + 10 };
        wysiwyg.dispatchEvent(new DragEvent('dragover', { ...coords, dataTransfer: dt }));
        wysiwyg.dispatchEvent(new DragEvent('drop', { ...coords, dataTransfer: dt }));
    }

    it('画像ファイルをドロップすると、アップロード後に figure>img が挿入される', async () => {
        const { wysiwyg } = await mountRealEditor();
        addMainForm();
        wysiwyg.innerHTML = '<p>x</p>';

        // multiUpload を成功レスポンスで即コールバックするスタブに差し替える
        const uploadJson = JSON.stringify([
            { Method: 'InsertText', Target: '#TestRte', Value: '![img](https://host/dropped.png)' }
        ]);
        window.$p.multiUpload = ((_url, _data, _c1, _c2, cb) => cb?.(uploadJson)) as typeof window.$p.multiUpload;

        // 画像ファイルをドロップ (drop → onFilePasteAndDrop → onImageUploadBefore → 自前アップロード)
        dropFile(wysiwyg, new File(['dummy'], 'dropped.png', { type: 'image/png' }));

        // 実 SunEditor の画像プラグイン (urlUpload) が figure>img を挿入する
        await vi.waitFor(() => {
            const img = wysiwyg.querySelector('figure img') as HTMLImageElement | null;
            expect(img?.src).toContain('dropped.png');
        });
    });

    it('画像以外のファイルをドロップしても登録されない (種別/拡張子ガード)', async () => {
        const { wysiwyg } = await mountRealEditor();
        addMainForm();
        wysiwyg.innerHTML = '<p>x</p>';

        // 非画像 (text/plain) をドロップ
        dropFile(wysiwyg, new File(['hello'], 'note.txt', { type: 'text/plain' }));

        // アップロードも画像挿入も発生しない (isValidImage ガードで弾かれる)
        await new Promise(resolve => setTimeout(resolve, 100)); // 非同期処理を待つ
        expect(wysiwyg.querySelector('figure img')).toBeNull();
        expect(window.$p.multiUpload).not.toHaveBeenCalled();
    });

    it('ツールバー「画像」で選択した画像がアップロードされ figure>img が挿入される', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        addMainForm();
        wysiwyg.innerHTML = '<p>x</p>';

        const uploadJson = JSON.stringify([
            { Method: 'InsertText', Target: '#TestRte', Value: '![img](https://host/toolbar.png)' }
        ]);
        window.$p.multiUpload = ((_url, _data, _c1, _c2, cb) => cb?.(uploadJson)) as typeof window.$p.multiUpload;

        // ツールバーの「画像」ボタンでモーダルを開く
        pressButton(toolbar.querySelector('button[data-command="image"]')!);
        await waitVisible('.se-modal-content');

        // モーダルのファイル入力へ画像を設定する
        const fileInput = document.querySelector('.__se__file_input') as HTMLInputElement;
        const dt = new DataTransfer();
        dt.items.add(new File(['dummy'], 'toolbar.png', { type: 'image/png' }));
        fileInput.files = dt.files;

        // 送信 (modalAction が files を submitFile → onImageUploadBefore → 自前アップロード)
        (await waitVisible('.se-modal button[type="submit"]')).click();

        await vi.waitFor(() => {
            const img = wysiwyg.querySelector('figure img') as HTMLImageElement | null;
            expect(img?.src).toContain('toolbar.png');
        });
    });
});

describe('モーダルの閉じ挙動 (実 SunEditor / ブラウザ)', () => {
    it('確認ボタンで必ず閉じる: 空URLで送信してもリンクモーダルが閉じる (setupModalAutoClose)', async () => {
        const { wysiwyg, toolbar } = await mountRealEditor();
        wysiwyg.innerHTML = '<p>hello</p>';
        selectAllText(wysiwyg);

        pressButton(toolbar.querySelector('button[data-command="link"]')!);
        await waitVisible('.se-modal-content'); // モーダルが開く

        // 空URLで送信: 本来 modalAction は false を返し閉じないが、自作フックで必ず閉じる
        (await waitVisible('.se-modal button[type="submit"]')).click();

        await vi.waitFor(() => {
            expect(findVisible('.se-modal-content')).toBeNull(); // 閉じている
        });
    });
});

describe('URL 自動リンク (実 SunEditor / ブラウザ)', () => {
    /** プレーンテキストを wysiwyg に貼り付ける */
    function pasteText(wysiwyg: HTMLElement, text: string) {
        const dt = new DataTransfer();
        dt.setData('text/plain', text);
        wysiwyg.dispatchEvent(new ClipboardEvent('paste', { bubbles: true, cancelable: true, clipboardData: dt }));
    }

    it('URL を貼り付けると target="_blank" のリンクへ変換される (別タブで開く)', async () => {
        const { wysiwyg } = await mountRealEditor();
        wysiwyg.innerHTML = '<p><br></p>';
        placeCaret(wysiwyg.querySelector('p') as HTMLElement);

        pasteText(wysiwyg, 'see https://example.com now');

        await vi.waitFor(() => {
            const a = wysiwyg.querySelector('a[href]') as HTMLAnchorElement | null;
            expect(a?.getAttribute('href')).toContain('example.com');
            // autoLinkUrls が付与する target="_blank" で別タブ遷移になる (#47)。
            // (rel="noopener..." も付与するが SunEditor のペースト整形で除去される。
            //  rel 付与自体は autoLinkUrls の単体テストで検証済み)
            expect(a?.getAttribute('target')).toBe('_blank');
        });
    });
});

describe('データ入力 (実 SunEditor / ブラウザ)', () => {
    it('HTMLタグを直接入力しても要素化されずリテラル文字列になる', async () => {
        const { wysiwyg } = await mountRealEditor();
        wysiwyg.innerHTML = '<p><br></p>';

        await userEvent.click(wysiwyg); // エディタにフォーカス
        await userEvent.keyboard('<b>hello</b>'); // タグ文字を実キー入力

        await vi.waitFor(() => {
            // タグとして解釈されず (要素化されない)、テキストとして入る
            expect(wysiwyg.querySelector('b')).toBeNull();
            expect(wysiwyg.textContent).toContain('<b>hello</b>');
        });
    });
});

describe('画像ライトボックス (実 SunEditor / ブラウザ)', () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    /** wysiwyg に figure>img を差し込む */
    function setImages(wysiwyg: HTMLElement, srcs: string[]) {
        wysiwyg.innerHTML = figures(...srcs);
    }

    it('ライトボックス無効: 画像クリックで別タブ (window.open) が開く', async () => {
        // imageViewerModal 静的プロパティは一度生成されると保持されるため、
        // 「無効 (window.open 経路)」はライトボックス有効テストより前に実行する
        const { wysiwyg } = await mountRealEditor();
        setImages(wysiwyg, ['https://host/single.png']);
        const openSpy = vi.spyOn(window, 'open').mockReturnValue(null);

        (wysiwyg.querySelector('figure img') as HTMLElement).click();

        expect(openSpy).toHaveBeenCalledTimes(1);
        expect(String(openSpy.mock.calls[0][0])).toContain('single.png');
    });

    it('ライトボックス有効: 画像クリックで拡大モーダル (ImageViewerModal.show) が開く', async () => {
        const { wysiwyg } = await mountRealEditor({ enablelightbox: true });
        setImages(wysiwyg, ['https://host/a.png']);
        // 実画像ロード (loadImageWithMinWait) は E2E 担当。ここでは配線 (show 呼び出し) のみ検証する
        const showSpy = vi.spyOn(ImageViewerModal.prototype, 'show').mockImplementation(() => {});

        const target = wysiwyg.querySelector('figure img') as HTMLImageElement;
        target.click();

        expect(showSpy).toHaveBeenCalledTimes(1);
        expect(showSpy.mock.calls[0][0]).toBe(target);
    });

    it('ライトボックス有効(複数画像): クリック画像と全 figure 画像のリストを show へ渡す', async () => {
        const { wysiwyg } = await mountRealEditor({ enablelightbox: true });
        setImages(wysiwyg, ['https://host/a.png', 'https://host/b.png', 'https://host/c.png']);
        const showSpy = vi.spyOn(ImageViewerModal.prototype, 'show').mockImplementation(() => {});

        const imgs = wysiwyg.querySelectorAll('figure img');
        (imgs[1] as HTMLElement).click();

        expect(showSpy).toHaveBeenCalledTimes(1);
        expect(showSpy.mock.calls[0][0]).toBe(imgs[1]); // クリックした画像
        const passedList = showSpy.mock.calls[0][1] as NodeListOf<HTMLImageElement>;
        expect(passedList.length).toBe(3); // 本文中の全画像
    });

    it('画像以外 (テキスト) をクリックしてもライトボックスは開かない', async () => {
        const { wysiwyg } = await mountRealEditor({ enablelightbox: true });
        wysiwyg.innerHTML = '<p>plain text</p>';
        const showSpy = vi.spyOn(ImageViewerModal.prototype, 'show').mockImplementation(() => {});

        (wysiwyg.querySelector('p') as HTMLElement).click();

        expect(showSpy).not.toHaveBeenCalled();
    });
});

describe('レイアウト: 親の text-align の影響を受けない (実 SunEditor / ブラウザ)', () => {
    // rt-editor は自身に text-align:initial を指定しており (richTextEditor.scss)、
    // 親の text-align (center/right) を継承しない = 配置は RTE ツールバーで行う設計。
    // このリセットが消えると回帰するため、computed 値で担保する。
    for (const align of ['right', 'center'] as const) {
        it(`親が text-align:${align} でも rt-editor は継承しない (initial リセット)`, async () => {
            const { rt } = await mountRealEditor();
            const parent = rt.parentElement as HTMLElement;
            parent.style.textAlign = align;

            // 同じ親配下の素の要素は align を継承する (親指定が有効であることの確認)
            const plain = document.createElement('div');
            parent.appendChild(plain);
            expect(getComputedStyle(plain).textAlign).toBe(align);

            // rt-editor は text-align:initial でリセットされ継承しない (initial = start)
            expect(getComputedStyle(rt).textAlign).toBe('start');
        });
    }
});

describe('編集→保存の同期 (実 SunEditor / ブラウザ)', () => {
    it('実エディタで打鍵すると土台 textarea へ同期され change / $p.set が呼ばれる', async () => {
        const { wysiwyg } = await mountRealEditor();
        const textarea = document.querySelector('#TestRte') as HTMLTextAreaElement;
        const changeSpy = vi.fn();
        textarea.addEventListener('change', changeSpy);

        await userEvent.click(wysiwyg);
        await userEvent.keyboard('hello');

        await vi.waitFor(() => {
            // onChange 経由で保存対象の textarea へ反映される
            expect(textarea.value).toContain('hello');
            // Pleasanter への値バインド (フォーム送信で永続化される値)
            expect(window.$p.set).toHaveBeenCalled();
        });
        // 非空編集では change を発火する (依存フィールド更新等のトリガ)
        expect(changeSpy).toHaveBeenCalled();
    });
    // 実質空(<p><br></p>)の空文字正規化は onChange の空判定 (unit) で網羅済みのため browser では省略
});

describe('言語設定の反映 (実 SunEditor / ブラウザ)', () => {
    // ツールバー(標準ボタン)の説明テキストは言語パックのラベルが入る (bold: ja=太字 / en=Bold)
    const boldTooltip = (toolbar: HTMLElement) =>
        toolbar.querySelector('button[data-command="bold"] .se-tooltip-text')?.textContent;

    // 説明にはショートカット(例: CTRL+B)も併記されるため、ラベル部分を部分一致で判定する
    it('日本語設定: ツールバーの説明が日本語で表示される', async () => {
        const { toolbar } = await mountRealEditor({ language: 'ja' });
        expect(boldTooltip(toolbar)).toContain('太字');
        expect(boldTooltip(toolbar)).not.toContain('Bold'); // 英語ラベルではない
    });

    it('英語設定: ツールバーの説明が英語で表示される', async () => {
        const { toolbar } = await mountRealEditor({ language: 'en' });
        expect(boldTooltip(toolbar)).toContain('Bold');
    });
});

describe('編集/閲覧トグル (実 SunEditor / ブラウザ)', () => {
    const toggleOf = (rt: HTMLElement) => rt.querySelector('.btn-editable-cmd .toggle') as HTMLElement;

    it('値有り: 初期は閲覧(OFF/編集不可)、クリックで編集(ON)⇔閲覧(OFF)が切り替わる', async () => {
        const { rt, wysiwyg } = await mountRealEditor({ value: '<p>content</p>' });
        const toggleBtn = rt.querySelector('.btn-editable-cmd') as HTMLElement;

        // 値有り → 初期は閲覧モード(OFF): 本文は編集不可 (contenteditable=false)
        await vi.waitFor(() => {
            expect(toggleOf(rt).textContent).toBe('OFF');
            expect(wysiwyg.getAttribute('contenteditable')).toBe('false');
        });

        // OFF クリック → 編集(ON): 本文が編集可 (contenteditable=true)
        toggleBtn.click();
        await vi.waitFor(() => {
            expect(toggleOf(rt).textContent).toBe('ON');
            expect(wysiwyg.getAttribute('contenteditable')).toBe('true');
        });

        // ON クリック → 閲覧(OFF) に戻る
        toggleBtn.click();
        await vi.waitFor(() => {
            expect(toggleOf(rt).textContent).toBe('OFF');
            expect(wysiwyg.getAttribute('contenteditable')).toBe('false');
        });
    });

    it('値無し: 初期は編集(ON/編集可)', async () => {
        const { rt, wysiwyg } = await mountRealEditor(); // 空 = 編集モード
        expect(toggleOf(rt).textContent).toBe('ON');
        expect(wysiwyg.getAttribute('contenteditable')).toBe('true');
    });
});

describe('プレースホルダ表示 (実 SunEditor / ブラウザ)', () => {
    it('空エディタでは placeholder が表示され、打鍵すると消える', async () => {
        const { rt, wysiwyg } = await mountRealEditor({ placeholder: '入力してください' });
        const placeholder = rt.querySelector('.se-placeholder') as HTMLElement;

        // 空のとき: textarea の placeholder が表示される (display:block)
        expect(placeholder.textContent).toBe('入力してください');
        await vi.waitFor(() => expect(getComputedStyle(placeholder).display).toBe('block'));

        // 打鍵で内容が入ると非表示になる (display:none)
        await userEvent.click(wysiwyg);
        await userEvent.keyboard('x');
        await vi.waitFor(() => expect(getComputedStyle(placeholder).display).toBe('none'));
    });
});

describe('セキュリティ: 編集モードの属性除去 (実 SunEditor / ブラウザ)', () => {
    it('危険属性(onerror/onclick)は保存値・本文に混入しない (attributeWhitelist)', async () => {
        const { wysiwyg } = await mountRealEditor();
        const textarea = document.querySelector('#TestRte') as HTMLTextAreaElement;
        wysiwyg.innerHTML = '<p><br></p>';
        placeCaret(wysiwyg.querySelector('p') as HTMLElement);

        // 危険属性つき HTML を貼り付ける (SunEditor が attributeWhitelist で整形する)
        const dt = new DataTransfer();
        dt.setData(
            'text/html',
            '<p><img src="x" onerror="window.__xss=1"><a href="https://ok.test" onclick="window.__xss=1">link</a></p>'
        );
        wysiwyg.dispatchEvent(new ClipboardEvent('paste', { bubbles: true, cancelable: true, clipboardData: dt }));

        // 保存値へ反映されるまで待つ (href は許可属性なので残る)
        await vi.waitFor(() => expect(textarea.value).toContain('ok.test'));

        // 危険属性は本文にも保存値にも残らない
        expect(wysiwyg.innerHTML).not.toContain('onerror');
        expect(wysiwyg.innerHTML).not.toContain('onclick');
        expect(textarea.value).not.toContain('onerror');
        expect(textarea.value).not.toContain('onclick');
    });
});

describe('テーマ: CSS カスタムプロパティ (実 / ブラウザ)', () => {
    afterEach(() => {
        document.documentElement.style.removeProperty('--primaryColor');
    });

    it('--primaryColor でツールバー背景色が変わる', async () => {
        // テーマは祖先(root)のカスタムプロパティで色を切り替える。継承で .se-toolbar に届く
        document.documentElement.style.setProperty('--primaryColor', 'rgb(1, 2, 3)');
        const { toolbar } = await mountRealEditor();
        expect(getComputedStyle(toolbar).backgroundColor).toBe('rgb(1, 2, 3)');
    });

    it('未設定時はフォールバック色 (#fafafa) になる', async () => {
        const { toolbar } = await mountRealEditor();
        expect(getComputedStyle(toolbar).backgroundColor).toBe('rgb(250, 250, 250)');
    });
});
