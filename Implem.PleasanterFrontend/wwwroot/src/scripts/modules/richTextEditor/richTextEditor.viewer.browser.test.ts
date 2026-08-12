import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

import { installPleasanterGlobals } from '../../../test/setup';
import { ImageViewerModal } from '../../generals/modal/imageViewerModal';
import {
    addHiddenInput,
    figures,
    installRteErrorSuppression,
    resetRteDom,
    teardownEditors
} from './richTextEditor.browserHelpers';
import './richTextEditor'; // 実モジュール (モックしない)

/**
 * 読み取り専用ビューア (data-readonly) の閲覧モード挙動を実ブラウザで検証する。
 *
 * 編集モードのテスト (richTextEditor.browser.test.ts) とは別ファイルにする。
 * imageViewerModal は静的プロパティで一度生成すると保持されるため、モジュール状態が
 * 汚染されていない本ファイルで「EnableLightBox:false(=window.open)」を先に検証する。
 */

installRteErrorSuppression(['contains', 'controlActive']);

/** 読み取り専用 (data-readonly) でビューアを接続する */
function mountViewer(value: string, { enablelightbox = false } = {}) {
    installPleasanterGlobals();
    addHiddenInput('Language', 'en');
    addHiddenInput('RteDefaultFont', 'Arial');

    const host = document.createElement('div');
    const lb = enablelightbox ? ' data-enablelightbox="1"' : '';
    host.innerHTML = `<rt-editor><textarea id="TestRte" data-readonly="1"${lb}></textarea></rt-editor>`;
    const textarea = host.querySelector('textarea') as HTMLTextAreaElement;
    textarea.value = value;
    document.body.appendChild(host); // connectedCallback → viewerInit
    const rt = host.querySelector('rt-editor') as HTMLElement;
    const viewer = rt.querySelector('.sun-editor-editable') as HTMLElement;
    return { rt, viewer };
}

beforeEach(() => {
    resetRteDom();
});

afterEach(async () => {
    vi.restoreAllMocks();
    await teardownEditors();
});

describe('閲覧モード: 読み取り専用ビューア (実 / ブラウザ)', () => {
    it('データ入力不可: ビューアは編集不可でツールバーも無い', () => {
        const { rt, viewer } = mountViewer('<p>readonly content</p>');
        expect(viewer.isContentEditable).toBe(false); // 入力できない
        expect(rt.querySelector('.se-toolbar')).toBeNull(); // ツールバー非表示
        expect(viewer.innerHTML).toContain('readonly content');
    });

    it('EnableLightBox:false: 画像クリックで別タブ (window.open)', () => {
        // imageViewerModal 未生成のうちに検証する (このファイル先頭のライトボックス系)
        const { viewer } = mountViewer(figures('https://host/only.png'), { enablelightbox: false });
        const openSpy = vi.spyOn(window, 'open').mockReturnValue(null);

        (viewer.querySelector('figure img') as HTMLElement).click();

        expect(openSpy).toHaveBeenCalledTimes(1);
        expect(String(openSpy.mock.calls[0][0])).toContain('only.png');
    });

    it('EnableLightBox:true: 画像クリックで拡大モーダル (ImageViewerModal.show)', () => {
        const { viewer } = mountViewer(figures('https://host/a.png'), { enablelightbox: true });
        const showSpy = vi.spyOn(ImageViewerModal.prototype, 'show').mockImplementation(() => {});

        const img = viewer.querySelector('figure img') as HTMLImageElement;
        img.click();

        expect(showSpy).toHaveBeenCalledTimes(1);
        expect(showSpy.mock.calls[0][0]).toBe(img);
    });

    it('EnableLightBox:true(複数): クリック画像とビューア内の全 figure 画像を show へ渡す', () => {
        const { viewer } = mountViewer(figures('https://host/a.png', 'https://host/b.png', 'https://host/c.png'), {
            enablelightbox: true
        });
        const showSpy = vi.spyOn(ImageViewerModal.prototype, 'show').mockImplementation(() => {});

        const imgs = viewer.querySelectorAll('figure img');
        (imgs[1] as HTMLElement).click();

        expect(showSpy).toHaveBeenCalledTimes(1);
        expect(showSpy.mock.calls[0][0]).toBe(imgs[1]); // クリックした画像
        const passedList = showSpy.mock.calls[0][1] as NodeListOf<HTMLImageElement>;
        expect(passedList.length).toBe(3); // ビューア内の全画像コレクション
        // (カレント/最大数表示・左右ボタン・矢印キー・カレント更新・循環は
        //  imageViewerModal.test.ts の unit で網羅済み)
    });

    it('URLクリックでページ遷移する: リンクはライトボックスに横取りされず別タブ設定で開ける', () => {
        const { viewer } = mountViewer('<p><a href="https://example.com" target="_blank">link</a></p>', {
            enablelightbox: true
        });
        const showSpy = vi.spyOn(ImageViewerModal.prototype, 'show').mockImplementation(() => {});
        const openSpy = vi.spyOn(window, 'open').mockReturnValue(null);
        const a = viewer.querySelector('a') as HTMLAnchorElement;
        a.addEventListener('click', event => event.preventDefault()); // テスト内での実遷移を抑止

        a.click();

        // リンククリックは画像ライトボックス処理に横取りされない (imageViewerHandle は IMG のみ対象)
        expect(showSpy).not.toHaveBeenCalled();
        expect(openSpy).not.toHaveBeenCalled();
        // 別タブ遷移する設定が保持されている (=クリックで遷移できる)
        expect(a.getAttribute('href')).toContain('example.com');
        expect(a.getAttribute('target')).toBe('_blank');
    });
});

describe('ImageViewerModal.loadImageWithMinWait (実画像ロード / ブラウザ)', () => {
    // 1x1 透過 PNG (実ブラウザで確実にデコード成功する)
    const PNG =
        'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==';

    // 本ファイルは image-viewer-modal を既に利用しており (ライトボックステスト)、同一
    // コンテキストで定義済みのため createElement で確実に upgrade される
    function mountModal() {
        installPleasanterGlobals();
        const modal = document.createElement('image-viewer-modal') as ImageViewerModal;
        document.body.appendChild(modal);
        return modal;
    }

    it('ロード成功で読み込んだ img を解決する', async () => {
        const modal = mountModal();
        const img = await modal.loadImageWithMinWait(PNG, 0);
        expect(img).toBeInstanceOf(HTMLImageElement);
        expect(img.src).toBe(PNG);
        expect(img.complete).toBe(true);
    });

    it('minWaitMs 分は最低待ってから解決する', async () => {
        const modal = mountModal();
        const start = performance.now();
        await modal.loadImageWithMinWait(PNG, 150);
        expect(performance.now() - start).toBeGreaterThanOrEqual(140);
    });

    it('ロード失敗 (不正な画像) で reject する', async () => {
        const modal = mountModal();
        await expect(modal.loadImageWithMinWait('data:image/png;base64,@@@notvalid@@@', 0)).rejects.toBeDefined();
    });

    it('?thumbnail クエリを除去して読み込む', async () => {
        const modal = mountModal();
        const img = await modal.loadImageWithMinWait(`${PNG}?thumbnail=1`, 0);
        expect(img.src).toBe(PNG);
    });

    it('読み込み開始時にモーダルへローディング表示フラグを立てる', async () => {
        const modal = mountModal();
        const uiModal = modal.shadowRoot!.querySelector('ui-modal')!;
        const promise = modal.loadImageWithMinWait(PNG, 0);
        expect(uiModal.hasAttribute('is-loading')).toBe(true);
        await promise;
    });
});

describe('セキュリティ: 読み取り専用ビューアのサニタイズ (実 DOMPurify / ブラウザ)', () => {
    it('インラインイベントハンドラを除去する', () => {
        const { viewer } = mountViewer('<p onclick="window.__xss=1">t</p><img src="x" onerror="window.__xss=1">');
        expect(viewer.innerHTML).not.toContain('onclick');
        expect(viewer.innerHTML).not.toContain('onerror');
        expect(viewer.querySelector('img')?.hasAttribute('onerror')).toBeFalsy();
    });

    it('javascript: プロトコルのリンクを無害化する', () => {
        const { viewer } = mountViewer('<a href="javascript:window.__xss=1">x</a>');
        const a = viewer.querySelector('a');
        expect(a?.getAttribute('href') ?? '').not.toContain('javascript:');
    });

    it('iframe/object/embed/form を除去する', () => {
        // 埋め込み/スクリプト実行ベクターに加え、フィッシング用の form も除去する
        // (form は FORBID_TAGS で明示除去。iframe/object/embed は DOMPurify 既定で除去)
        const { viewer } = mountViewer(
            '<iframe src="https://evil"></iframe><object data="x"></object><embed src="x">' +
                '<form action="https://evil"><input></form><p>ok</p>'
        );
        expect(viewer.querySelector('iframe')).toBeNull();
        expect(viewer.querySelector('object')).toBeNull();
        expect(viewer.querySelector('embed')).toBeNull();
        expect(viewer.querySelector('form')).toBeNull();
        expect(viewer.textContent).toContain('ok');
    });

    it('SVG ベースのベクター (onload/内部script) を無害化する', () => {
        const { viewer } = mountViewer('<svg onload="window.__xss=1"><script>window.__xss=1</script></svg><p>ok</p>');
        expect(viewer.innerHTML).not.toContain('onload');
        expect(viewer.querySelector('script')).toBeNull();
        expect(viewer.textContent).toContain('ok');
    });

    it('data:text/html の危険な URI を無害化する', () => {
        const { viewer } = mountViewer('<a href="data:text/html,<script>alert(1)</script>">x</a>');
        const a = viewer.querySelector('a');
        expect(a?.getAttribute('href') ?? '').not.toContain('data:text/html');
    });

    it('正常な要素・属性は保持する (過剰サニタイズしない)', () => {
        const { viewer } = mountViewer(
            '<p>para</p><a href="https://ok.test">link</a><figure><img src="https://h/i.png" alt="a"></figure>' +
                '<table><tbody><tr><td>cell</td></tr></tbody></table><ul><li>li</li></ul>' +
                '<strong>b</strong><em>i</em><u>u</u>'
        );
        expect(viewer.querySelector('p')?.textContent).toBe('para');
        expect(viewer.querySelector('a')?.getAttribute('href')).toContain('ok.test');
        expect(viewer.querySelector('figure img')?.getAttribute('src')).toContain('i.png');
        expect(viewer.querySelector('table td')?.textContent).toBe('cell');
        expect(viewer.querySelector('ul li')).not.toBeNull();
        expect(viewer.querySelector('strong')).not.toBeNull();
        expect(viewer.querySelector('em')).not.toBeNull();
        expect(viewer.querySelector('u')).not.toBeNull();
    });

    it('target="_blank" のリンクを保持し rel(noopener) を補完する', () => {
        // DOMPurify 既定では target が除去されるため、sanitizeViewerHtml で target を保持し、
        // reverse-tabnabbing 対策の rel="noopener noreferrer" を補完する (別ウィンドウで開く)
        const { viewer } = mountViewer('<p><a href="https://example.com" target="_blank">link</a></p>');
        const a = viewer.querySelector('a') as HTMLAnchorElement;
        expect(a.getAttribute('href')).toContain('example.com');
        expect(a.getAttribute('target')).toBe('_blank');
        expect(a.getAttribute('rel')).toContain('noopener');
    });

    it('set value(smartdesign ビューア)経由でも危険なタグ/属性を除去する', () => {
        installPleasanterGlobals();
        addHiddenInput('Language', 'en');
        addHiddenInput('RteDefaultFont', 'Arial');
        const host = document.createElement('div');
        // smartdesign(rt-editor) + 読み取り専用(textarea) で viewerContainer 経由の set value を通す
        host.innerHTML =
            '<rt-editor data-smartdesign="1"><textarea id="TestRte" data-readonly="1"></textarea></rt-editor>';
        document.body.appendChild(host);
        const rt = host.querySelector('rt-editor') as HTMLElement & { value: string };

        rt.value = '<p>ok</p><script>window.__b1=1</script><img src="x" onerror="window.__b1=1">';

        const viewer = rt.querySelector('.sun-editor-editable') as HTMLElement;
        expect(viewer.querySelector('script')).toBeNull();
        expect(viewer.innerHTML).not.toContain('onerror');
        expect(viewer.textContent).toContain('ok');
    });
});
