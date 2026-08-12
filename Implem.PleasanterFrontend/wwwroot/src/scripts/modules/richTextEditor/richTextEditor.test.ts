// @vitest-environment happy-dom
import { describe, it, expect, beforeEach, vi } from 'vitest';

import { installPleasanterGlobals } from '../../../test/setup';

/**
 * richTextEditor (<rt-editor>) のラッパーロジックの単体テスト。
 *
 * SunEditor 本体はモックし、ラッパー (RichTextEditorElement) 自身のロジックだけを検証する。
 * 実ブラウザ挙動 (トグル・画像アップロード・モーダル等) は E2E(TestAutomation) の担当。
 */

// --- suneditor モック --------------------------------------------------------
// モジュール読み込み時に評価される `interfaces.PluginCommand`/`plugins`/`langs` を満たしつつ、
// create() が返す偽インスタンスと生成オプションをテストから参照できるようにする。

/** 画像アップロード時に渡る info (#getInfo() の部分集合) */
interface ImageInfoLike {
    files: File[];
    element: HTMLImageElement | null;
    anchor: HTMLElement | null;
    inputWidth: string;
    inputHeight: string;
    align: string;
    isUpdate: boolean;
    alt: string;
}

/** テストで参照する suneditor.create のオプション (実際の型の部分集合) */
interface CreateOptions {
    lang: unknown;
    placeholder: string;
    events: {
        onChange: (e: { data: string }) => void;
        onPaste: (e: { event: unknown; data: string }) => Promise<string | boolean>;
        onImageUploadBefore: (e: { info: ImageInfoLike }) => Promise<boolean>;
    };
    [key: string]: unknown;
}

const h = vi.hoisted(() => {
    const state = { isEmpty: false, isDisabled: false };
    const refs: {
        instance?: ReturnType<typeof buildInstance>;
        createOptions?: CreateOptions;
        toggleClicks: number;
    } = { toggleClicks: 0 };

    // 言語パックは同一性で判定するためのセンチネル
    const langs = {
        zh_cn: { id: 'zh_cn' },
        en: { id: 'en' },
        ja: { id: 'ja' },
        de: { id: 'de' },
        ko: { id: 'ko' },
        es: { id: 'es' }
    };

    function buildInstance() {
        const wysiwyg = document.createElement('div');
        const toolbar = document.createElement('div');
        const carrier = document.createElement('div');
        const placeholderEl = document.createElement('div');
        // setupModalAutoClose が探すモーダル領域を carrierWrapper 配下に用意する
        const modalArea = document.createElement('div');
        modalArea.className = 'se-modal se-modal-area';
        carrier.appendChild(modalArea);
        // 画像プラグイン: キャプション用チェックボックスと urlUpload(挿入) をモックする
        const captionCheckEl = document.createElement('input');
        captionCheckEl.type = 'checkbox';
        const imagePlugin = { captionCheckEl, uploadService: { urlUpload: vi.fn() } };
        return {
            _els: { wysiwyg, toolbar, carrier, placeholderEl, modalArea },
            isEmpty: vi.fn(() => state.isEmpty),
            destroy: vi.fn(),
            $: {
                html: { set: vi.fn(), get: vi.fn(() => '<p>value</p>'), insert: vi.fn() },
                history: { reset: vi.fn(), resetButtons: vi.fn() },
                frameContext: {
                    get: vi.fn((key: string) => {
                        switch (key) {
                            case 'wysiwyg':
                                return wysiwyg;
                            case 'placeholder':
                                return placeholderEl;
                            case 'isDisabled':
                                return state.isDisabled;
                            case 'key':
                                return 'root';
                            default:
                                return null;
                        }
                    })
                },
                context: { get: vi.fn((key: string) => (key === 'toolbar_main' ? toolbar : null)) },
                ui: { disable: vi.fn(), enable: vi.fn(), offCurrentModal: vi.fn() },
                contextProvider: { carrierWrapper: carrier },
                plugins: { image: imagePlugin },
                component: { get: vi.fn(() => null) },
                focusManager: { focusEdge: vi.fn(), focus: vi.fn() }
            }
        };
    }

    const create = vi.fn((container: HTMLElement, options: CreateOptions) => {
        const instance = buildInstance();
        // setInitContent が読取モード移行のために click する偽トグルボタン
        const toggleBtn = document.createElement('button');
        toggleBtn.className = 'btn-editable-cmd';
        toggleBtn.addEventListener('click', () => {
            refs.toggleClicks++;
        });
        container.appendChild(toggleBtn);
        refs.instance = instance;
        refs.createOptions = options;
        return instance;
    });

    return { state, refs, langs, create };
});

vi.mock('suneditor', () => ({
    default: { create: h.create },
    interfaces: {
        PluginCommand: class {
            $: unknown;
            title = '';
            inner = '';
            constructor(kernel: unknown) {
                this.$ = kernel;
            }
        }
    },
    plugins: {},
    langs: h.langs
}));

// 副作用 (customElements.define) のため読み込む。ToggleReadonlyPlugin は単体検証用に import する
import { ToggleReadonlyPlugin, sanitizeViewerHtml } from './richTextEditor';

// --- ヘルパ ------------------------------------------------------------------

interface MountOptions {
    value?: string;
    language?: string;
    smartdesign?: boolean;
    readonly?: boolean;
    disabled?: boolean;
    enablelightbox?: boolean;
}

/** マイクロタスク/タイマーを1周させる (MutationObserver や setTimeout(0) の反映用) */
const flush = () => new Promise(resolve => setTimeout(resolve, 0));

/** #Language を用意し、<rt-editor><textarea/></rt-editor> を document に接続する */
function mountEditor(opts: MountOptions = {}) {
    if (opts.language !== undefined) {
        const langInput = document.createElement('input');
        langInput.id = 'Language';
        langInput.value = opts.language;
        document.body.appendChild(langInput);
    }
    const hostAttrs = opts.smartdesign ? ' data-smartdesign="1"' : '';
    const taAttrs =
        (opts.readonly ? ' data-readonly="1"' : '') +
        (opts.disabled ? ' disabled' : '') +
        (opts.enablelightbox ? ' data-enablelightbox="1"' : '');
    // innerHTML 経由でパース/upgrade してから接続する (本番のパーサ経路に近い)
    const host = document.createElement('div');
    host.innerHTML = `<rt-editor${hostAttrs}><textarea id="TestRte"${taAttrs}></textarea></rt-editor>`;
    const el = host.querySelector('rt-editor') as HTMLElement;
    const textarea = host.querySelector('textarea') as HTMLTextAreaElement;
    // value は innerHTML のテキストではなくプロパティで設定する
    // (textarea 内の HTML を RCDATA として保持しない DOM 実装があるため)
    if (opts.value !== undefined) textarea.value = opts.value;
    document.body.appendChild(host); // 接続時に connectedCallback が発火する
    return { el, textarea };
}

beforeEach(() => {
    vi.clearAllMocks();
    // 実行経路/タイミングに依存せず、毎回 current window に $p を用意する
    installPleasanterGlobals();
    h.state.isEmpty = false;
    h.state.isDisabled = false;
    h.refs.instance = undefined;
    h.refs.createOptions = undefined;
    h.refs.toggleClicks = 0;
    // 静的キャッシュがテスト間で漏れないようリセットする
    const Ctor = customElements.get('rt-editor') as unknown as Record<string, unknown>;
    if (Ctor) {
        Ctor.defaultfont = undefined;
        Ctor.fontList = undefined;
        Ctor.fontSize = undefined;
        Ctor.isSafari = undefined;
        Ctor.isResponsive = undefined;
        Ctor.imageViewerModal = undefined;
    }
    document.body.innerHTML = '';
    document.head.querySelector('#rteCustomCss')?.remove();
});

// --- テスト ------------------------------------------------------------------

describe('言語マッピング', () => {
    it('#Language の値を suneditor の言語パックへ対応付ける', () => {
        mountEditor({ language: 'ja', value: '' });
        expect(h.refs.createOptions?.lang).toBe(h.langs.ja);
    });

    it('未知の言語は en にフォールバックする', () => {
        mountEditor({ language: 'xx', value: '' });
        expect(h.refs.createOptions?.lang).toBe(h.langs.en);
    });

    // 全言語マッピング (zh は zh_cn へ、他は同名パックへ)
    it.each([
        ['zh', 'zh_cn'],
        ['de', 'de'],
        ['ko', 'ko'],
        ['es', 'es'],
        ['en', 'en']
    ])('#Language=%s を言語パック %s へマッピングする', (lang, pack) => {
        mountEditor({ language: lang, value: '' });
        expect(h.refs.createOptions?.lang).toBe(h.langs[pack as keyof typeof h.langs]);
    });
});

describe('setInitContent の空判定', () => {
    it('実質空(<p><br></p>)の初期値は空文字へ正規化し、編集モードのまま開く', () => {
        h.state.isEmpty = true;
        const { textarea } = mountEditor({ value: '<p><br></p>' });

        expect(h.refs.instance?.$.html.set).toHaveBeenCalledWith('<p><br></p>');
        expect(textarea.value).toBe('');
        expect(h.refs.toggleClicks).toBe(0); // 読取トグルは押されない
        expect(h.refs.instance?.$.history.reset).toHaveBeenCalled();
    });

    it('内容がある初期値は読取モード(トグル押下)で開き、値は保持する', () => {
        h.state.isEmpty = false;
        const { textarea } = mountEditor({ value: '<p>hello</p>' });

        expect(textarea.value).toBe('<p>hello</p>');
        expect(h.refs.toggleClicks).toBe(1); // 読取トグルが押される
    });
});

describe('onChange: 値の反映と変更通知 (ページ離脱警告 issue #2953 の回帰含む)', () => {
    // onChange は (1) 編集値を textarea/$p へ反映し、(2) bubbles:true の change を発火する。
    // change は document までバブリングする必要がある: Pleasanter のフォーム変更検知は上位の
    // 委譲リスナで change を拾うため、これが届かないと「入力後に更新せずページ移動しても警告が
    // 出ない」不具合 (#2953) に退行する。textarea 自身ではなく document で受けて退行を捕捉する。
    it('内容があれば値を保持し、change が document までバブリングする (未保存として検知)', () => {
        h.state.isEmpty = false;
        const { textarea } = mountEditor({ value: '' });
        const onChange = h.refs.createOptions!.events.onChange;
        const docSpy = vi.fn();
        document.addEventListener('change', docSpy);
        try {
            onChange({ data: '<p>hi</p>' });
            expect(textarea.value).toBe('<p>hi</p>');
            expect(docSpy).toHaveBeenCalledTimes(1); // 上位まで届く = 離脱時に警告が出る
        } finally {
            document.removeEventListener('change', docSpy);
        }
    });

    it('実質空(<p><br></p>)なら空文字へ正規化し、change を発火しない (未保存の誤検知を防ぐ)', () => {
        h.state.isEmpty = true;
        const { textarea } = mountEditor({ value: '' });
        const onChange = h.refs.createOptions!.events.onChange;
        const docSpy = vi.fn();
        document.addEventListener('change', docSpy);
        try {
            onChange({ data: '<p><br></p>' });
            expect(textarea.value).toBe('');
            expect($p.set).toHaveBeenCalledWith(expect.anything(), '');
            expect(docSpy).not.toHaveBeenCalled();
        } finally {
            document.removeEventListener('change', docSpy);
        }
    });
});

describe('読み取り専用ビューア', () => {
    it('readonly: エディタを生成せず、app-readonly のビューア div に内容を描画する', () => {
        const { el } = mountEditor({ value: '<p>read only</p>', readonly: true });

        // SunEditor 未生成 = ツールバー無し (閲覧モード)。閲覧(読取)は無効(disabled)と区別する
        expect(h.create).not.toHaveBeenCalled();
        const viewer = el.querySelector('.sun-editor-editable');
        expect(viewer).not.toBeNull();
        expect(viewer?.innerHTML).toContain('read only');
        expect(el.querySelector('.app-readonly')).not.toBeNull();
        expect(el.querySelector('.app-disabled')).toBeNull();
        // (ビューアの DOMPurify サニタイズは実DOMで検証する: browser テスト側)
    });

    it('disabled: エディタを生成せず app-disabled を付ける', () => {
        const { el } = mountEditor({ value: '<p>x</p>', disabled: true });

        expect(h.create).not.toHaveBeenCalled();
        expect(el.querySelector('.app-disabled')).not.toBeNull();
        expect(el.querySelector('.app-readonly')).toBeNull();
    });
});

describe('複数 RTE インスタンスの共存', () => {
    // 静的キャッシュ (fontList/imageViewerModal 等) を複数エディタで共有する
    it('2つの RTE を接続でき、フォント設定(静的キャッシュ)を共有する', () => {
        const fontList = document.createElement('input');
        fontList.id = 'RteFontList';
        fontList.value = 'Arial,游ゴシック';
        document.body.appendChild(fontList);
        h.state.isEmpty = true;

        mountEditor({ value: '' });
        mountEditor({ value: '' });

        expect(h.create).toHaveBeenCalledTimes(2);
        const opts1 = h.create.mock.calls[0][1];
        const opts2 = h.create.mock.calls[1][1];
        expect(opts1.font).toBeDefined();
        expect(opts1.font).toEqual(opts2.font); // 同じ静的キャッシュを共有
    });

    it('複数 RTE でも imageViewerModal は1つだけ生成される', () => {
        h.state.isEmpty = true;
        mountEditor({ value: '', enablelightbox: true });
        mountEditor({ value: '', enablelightbox: true });

        expect(document.querySelectorAll('image-viewer-modal').length).toBe(1);
    });
});

describe('disconnectedCallback のリソース解放', () => {
    it('切断時に editor を destroy し MutationObserver を disconnect する', () => {
        const disconnectSpy = vi.spyOn(MutationObserver.prototype, 'disconnect');
        h.state.isEmpty = true;
        const { el } = mountEditor({ value: '', smartdesign: true });
        const instance = h.refs.instance;

        el.remove();

        expect(instance?.destroy).toHaveBeenCalled();
        expect(disconnectSpy).toHaveBeenCalled(); // smartDesignObserver が解放される
        disconnectSpy.mockRestore();
    });
});

describe('スマートデザインの placeholder 連携', () => {
    it('textarea の placeholder 変更をエディタの placeholder 表示へ反映する', async () => {
        h.state.isEmpty = true;
        const { textarea } = mountEditor({ value: '', smartdesign: true });
        const placeholderEl = h.refs.instance?._els.placeholderEl as HTMLElement;

        textarea.setAttribute('placeholder', 'new placeholder');
        await new Promise(resolve => setTimeout(resolve, 0)); // MutationObserver は非同期

        expect(placeholderEl.textContent).toBe('new placeholder');
    });
});

describe('SmartDesign の値バインド (smartDesignValueBind)', () => {
    it('smartdesign では onChange で demochange イベントを発火する', () => {
        h.state.isEmpty = false;
        const { el } = mountEditor({ value: '', smartdesign: true });
        const onChange = h.refs.createOptions!.events.onChange;
        const demoSpy = vi.fn();
        el.addEventListener('demochange', demoSpy as EventListener);

        onChange({ data: '<p>hi</p>' });

        expect(demoSpy).toHaveBeenCalledTimes(1);
        expect((demoSpy.mock.calls[0][0] as CustomEvent).detail.value).toBe('<p>hi</p>');
    });

    it('smartdesign でなければ demochange を発火しない', () => {
        h.state.isEmpty = false;
        const { el } = mountEditor({ value: '' });
        const onChange = h.refs.createOptions!.events.onChange;
        const demoSpy = vi.fn();
        el.addEventListener('demochange', demoSpy as EventListener);

        onChange({ data: '<p>hi</p>' });

        expect(demoSpy).not.toHaveBeenCalled();
    });
});

describe('URL 自動リンク化 (onPaste)', () => {
    it('素の URL をリンク化しつつ既存の <a> は保持する', async () => {
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const onPaste = h.refs.createOptions?.events.onPaste as (e: {
            event: unknown;
            data: string;
        }) => Promise<string | boolean>;

        const result = await onPaste({
            event: { clipboardData: { items: { length: 0 } } },
            data: 'see https://example.com and <a href="http://keep.me">x</a>'
        });

        expect(result).toContain('<a href="https://example.com"');
        expect(result).toContain('href="http://keep.me"'); // 既存 <a> を保持
        expect(result).not.toContain('href="http://keep.me""'); // 二重リンク化しない
    });

    it('細工URLでも href から属性ブレイクアウトしない', async () => {
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const onPaste = h.refs.createOptions!.events.onPaste;

        // URL 正規表現は " ' < > を含まないため、href 値は細工URLで途切れる
        const result = (await onPaste({
            event: { clipboardData: { items: { length: 0 } } },
            data: 'https://evil.com/"><script>alert(1)</script>'
        })) as string;

        const a = new DOMParser().parseFromString(result, 'text/html').querySelector('a')!;
        expect(a.getAttribute('href')).toBe('https://evil.com/'); // " で途切れブレイクアウトしない
        expect(a.querySelector('script')).toBeNull(); // アンカー内に script は混入しない
        // (末尾に残る "><script> はアンカー外のテキストで、SunEditor/DOMPurify で無害化される)
    });

    it('img の src 属性内 URL はリンク化しない (画像コピー&ペーストの回帰)', async () => {
        // 画像をコピー&ペーストすると <figure><img src="http://…/show"> が data に来る。
        // 属性値内の URL をリンク化すると src が <a> で壊れる不具合の回帰テスト。
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const onPaste = h.refs.createOptions!.events.onPaste;

        const result = (await onPaste({
            event: { clipboardData: { items: { length: 0 } } },
            data: '<figure><img src="http://localhost:59803/binaries/abc/show"></figure>'
        })) as string;

        const doc = new DOMParser().parseFromString(result, 'text/html');
        expect(doc.querySelector('img')!.getAttribute('src')).toBe('http://localhost:59803/binaries/abc/show');
        expect(doc.querySelectorAll('a').length).toBe(0); // 属性内 URL はリンク化されない
    });

    it('表(figure>table)を貼り付けても構造・属性は壊れず、セル内の URL のみリンク化する', async () => {
        // 表コントローラの「コピー」も component.copy → clipboard(text/html) 経由で貼付される。
        // 属性(colspan 等)は壊さず、セル本文の URL だけをリンク化することを確認する。
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const onPaste = h.refs.createOptions!.events.onPaste;

        const result = (await onPaste({
            event: { clipboardData: { items: { length: 0 } } },
            data: '<figure><table><tbody><tr><td colspan="2">see https://example.com</td></tr></tbody></table></figure>'
        })) as string;

        const doc = new DOMParser().parseFromString(result, 'text/html');
        expect(doc.querySelector('table')).not.toBeNull(); // 構造は保持
        expect(doc.querySelector('td')!.getAttribute('colspan')).toBe('2'); // 属性は保持
        expect(doc.querySelector('td a')?.getAttribute('href')).toBe('https://example.com'); // セル内 URL はリンク化
    });
});

// --- 画像アップロード系のヘルパ ----------------------------------------------

/** buildUploadUrl 用の DOM を用意する (main-form の action に _action_ プレースホルダを含める) */
function setupUploadDom({ form = false }: { form?: boolean } = {}) {
    const mainForm = document.createElement('form');
    mainForm.className = 'main-form';
    mainForm.setAttribute('action', '/items/1/_action_');
    document.body.appendChild(mainForm);
    if (form) vi.mocked(window.$p.isForm).mockReturnValue(true);
}

function makeFile(name = 'p.png', type = 'image/png') {
    return new File(['dummy'], name, { type });
}

function makeImageInfo(files: File[], overrides: Partial<ImageInfoLike> = {}): ImageInfoLike {
    return {
        files,
        element: null,
        anchor: null,
        inputWidth: '300px',
        inputHeight: '200px',
        align: 'center',
        isUpdate: false,
        alt: 'my alt',
        ...overrides
    };
}

/** multiUpload のコールバックへ渡す、アップロード成功レスポンス JSON */
const UPLOAD_OK_JSON = JSON.stringify([
    { Method: 'InsertText', Target: '#TestRte', Value: '![alt](https://host/img.png)' }
]);

/** multiUpload に記録された成功コールバックを取り出して実行する */
function runUploadCallback(json = UPLOAD_OK_JSON) {
    const callback = vi.mocked(window.$p.multiUpload).mock.calls[0]?.[4] as ((json: string) => void) | undefined;
    callback?.(json);
}

describe('画像アップロード (ツールバー/D&D: onImageUploadBefore)', () => {
    it('FormData を組み立てて multiUpload を呼び、既定アップロードをキャンセルする', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        const result = await onImageUpload({ info: makeImageInfo([makeFile()]) });

        expect(result).toBe(false); // 既定処理はキャンセルし自前挿入する
        expect(window.$p.multiUpload).toHaveBeenCalledTimes(1);
        const [url, formData] = vi.mocked(window.$p.multiUpload).mock.calls[0];
        expect(url).toBe('/items/1/binaries/uploadimage');
        expect(formData.get('ControlId')).toBe('TestRte');
        expect((formData.get('file') as File).name).toBe('p.png');
    });

    it('フォーム内では formbinaries コントローラの URL を使う', async () => {
        h.state.isEmpty = true;
        setupUploadDom({ form: true });
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile()]) });

        expect(vi.mocked(window.$p.multiUpload).mock.calls[0][0]).toBe('/items/1/formbinaries/uploadimage');
    });

    it('成功時、モーダル設定とキャプションを保持して urlUpload で挿入する', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const image = h.refs.instance!.$.plugins.image;
        image.captionCheckEl.checked = true; // 確定時にキャプション ON
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile()]) });
        image.captionCheckEl.checked = false; // モーダル閉鎖時の modalInit リセットを模倣
        runUploadCallback();

        expect(image.uploadService.urlUpload).toHaveBeenCalledWith(
            expect.objectContaining({
                url: 'https://host/img.png',
                alt: 'my alt',
                align: 'center',
                inputWidth: '300px',
                isUpdate: false
            })
        );
        expect(image.captionCheckEl.checked).toBe(true); // 確定時の値へ復元される
    });

    it('画像以外のファイルはアップロードしない (種別/拡張子ガード)', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile('a.txt', 'text/plain')]) });

        expect(window.$p.multiUpload).not.toHaveBeenCalled();
    });

    it('ファイルサイズ超過時はアップロードしない', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        vi.mocked(window.$p.validateImageUploadFileSize).mockReturnValue(false);
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile()]) });

        expect(window.$p.multiUpload).not.toHaveBeenCalled();
    });

    // 拡張子/種別の偽装 (isValidImage は MIME と拡張子の AND 判定)
    it.each([
        ['MIME は画像だが拡張子が .svg', 'evil.svg', 'image/png'],
        ['MIME は画像だが拡張子が .html', 'evil.html', 'image/png'],
        ['拡張子は .png だが MIME が text/html', 'evil.png', 'text/html'],
        ['二重拡張子 .png.exe', 'evil.png.exe', 'image/png']
    ])('偽装ファイルはアップロードしない (%s)', async (_label, name, type) => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile(name, type)]) });

        expect(window.$p.multiUpload).not.toHaveBeenCalled();
    });

    // アップロード応答の異常系 (挿入しない)
    it('応答がエラー(handleMessageFromJson=true)なら画像を挿入しない', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;
        vi.mocked(window.$p.handleMessageFromJson).mockReturnValue(true);

        await onImageUpload({ info: makeImageInfo([makeFile()]) });
        runUploadCallback(); // エラー応答

        expect(h.refs.instance!.$.plugins.image.uploadService.urlUpload).not.toHaveBeenCalled();
        expect(h.refs.instance!.$.html.insert).not.toHaveBeenCalled();
    });

    it('応答に該当画像URLが無ければ挿入しない', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile()]) });
        // Target 不一致で該当 InsertText が見つからない
        runUploadCallback(JSON.stringify([{ Method: 'InsertText', Target: '#Other', Value: '![a](https://h/x.png)' }]));

        expect(h.refs.instance!.$.plugins.image.uploadService.urlUpload).not.toHaveBeenCalled();
    });

    // アップロード先 URL が作れない (.main-form もダイアログも無い) ならアップロードしない
    it('アップロード先URLを作れない(フォーム/ダイアログ無し)ならアップロードしない', async () => {
        h.state.isEmpty = true;
        // setupUploadDom を呼ばない = .main-form / #EditorInDialogRecordId が無い
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile()]) });

        expect(window.$p.multiUpload).not.toHaveBeenCalled();
    });

    // 複数画像を一括アップロードする
    it('複数画像を一括アップロードする (info.files 複数)', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({
            info: makeImageInfo([makeFile('a.png'), makeFile('b.png'), makeFile('c.png')])
        });

        expect(window.$p.multiUpload).toHaveBeenCalledTimes(3);
    });
});

describe('画像アップロード (ペースト: onPaste の file 分岐)', () => {
    const clipboard = (file: File) => ({
        clipboardData: { items: [{ kind: 'file', getAsFile: () => file }] }
    });
    const clipboardMulti = (...files: File[]) => ({
        clipboardData: { items: files.map(file => ({ kind: 'file', getAsFile: () => file })) }
    });

    it('画像ファイルのペーストで multiUpload を呼び、成功時に html.insert で挿入する', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onPaste = h.refs.createOptions!.events.onPaste;

        await onPaste({ event: clipboard(makeFile()), data: '' });

        expect(h.refs.instance!.$.ui.disable).toHaveBeenCalled();
        expect(h.refs.instance!.$.ui.enable).toHaveBeenCalled();
        expect(window.$p.multiUpload).toHaveBeenCalledTimes(1);

        runUploadCallback();
        expect(h.refs.instance!.$.html.insert).toHaveBeenCalledWith('<p><img src="https://host/img.png"></p>');
    });

    it('画像以外のファイルのペーストはアップロードしない', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onPaste = h.refs.createOptions!.events.onPaste;

        await onPaste({ event: clipboard(makeFile('a.txt', 'text/plain')), data: '' });

        expect(window.$p.multiUpload).not.toHaveBeenCalled();
    });

    // 複数画像のペーストを一括アップロードする
    it('複数画像のペーストを一括アップロードする (items 複数)', async () => {
        h.state.isEmpty = true;
        setupUploadDom();
        mountEditor({ value: '' });
        const onPaste = h.refs.createOptions!.events.onPaste;

        await onPaste({ event: clipboardMulti(makeFile('a.png'), makeFile('b.png')), data: '' });

        expect(window.$p.multiUpload).toHaveBeenCalledTimes(2);
    });
});

describe('アップロード先 URL (buildUploadUrl: ダイアログ分岐)', () => {
    it('ダイアログ内 (#EditorInDialogRecordId) では BaseUrl + recordId から URL を作る', async () => {
        h.state.isEmpty = true;
        const baseUrl = document.createElement('input');
        baseUrl.id = 'BaseUrl';
        baseUrl.value = '/base/';
        const recordId = document.createElement('input');
        recordId.id = 'EditorInDialogRecordId';
        recordId.value = '7';
        document.body.append(baseUrl, recordId);
        mountEditor({ value: '' });
        const onImageUpload = h.refs.createOptions!.events.onImageUploadBefore;

        await onImageUpload({ info: makeImageInfo([makeFile()]) });

        expect(vi.mocked(window.$p.multiUpload).mock.calls[0][0]).toBe('/base/7/binaries/uploadimage');
    });
});

describe('フォント/サイズ/placeholder のオプション構築', () => {
    it('#RteFontSize からサイズ候補を作り、既定を 13(px) にする', () => {
        const input = document.createElement('input');
        input.id = 'RteFontSize';
        input.value = '8,9,10,13,16,72';
        document.body.appendChild(input);
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        const fontSize = h.refs.createOptions!.fontSize as {
            unitMap: { px: { default: number; list: number[]; min: number; max: number } };
        };
        expect(fontSize.unitMap.px.default).toBe(13);
        expect(fontSize.unitMap.px.list).toEqual([8, 9, 10, 13, 16, 72]);
        expect(fontSize.unitMap.px.min).toBe(8);
        expect(fontSize.unitMap.px.max).toBe(72);
    });

    it('#RteFontList からフォント候補を渡す', () => {
        const input = document.createElement('input');
        input.id = 'RteFontList';
        input.value = '游ゴシック,Arial';
        document.body.appendChild(input);
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        const font = h.refs.createOptions!.font as { items: string[] };
        expect(font.items).toEqual(['游ゴシック', 'Arial']);
    });

    it('textarea の placeholder を suneditor へ渡す', () => {
        h.state.isEmpty = true;
        const host = document.createElement('div');
        host.innerHTML = '<rt-editor><textarea id="TestRte" placeholder="入力ガイド"></textarea></rt-editor>';
        document.body.appendChild(host);

        expect(h.refs.createOptions!.placeholder).toBe('入力ガイド');
    });
});

describe('ToggleReadonlyPlugin.action (編集/閲覧トグル)', () => {
    function setupPlugin() {
        const toggle = document.createElement('div');
        toggle.className = 'toggle';
        const button = document.createElement('button');
        button.className = 'btn-editable-cmd';
        button.appendChild(toggle);
        const toolbar = document.createElement('div');
        toolbar.appendChild(button);
        const topArea = document.createElement('div');
        const kernel = {
            frameContext: { get: (k: string) => (k === 'topArea' ? topArea : k === 'key' ? 'root' : null) },
            context: { get: (k: string) => (k === 'toolbar_main' ? toolbar : null) },
            ui: { disable: vi.fn(), enable: vi.fn() },
            history: { resetButtons: vi.fn() }
        };
        const plugin = new ToggleReadonlyPlugin(kernel as never);
        return { plugin, kernel, topArea, button, toggle };
    }

    it('編集(ON)→閲覧(OFF): 本文を disable しトグルだけ再有効化する', () => {
        const { plugin, kernel, topArea, button, toggle } = setupPlugin();

        plugin.action();

        expect(topArea.classList.contains('is-readonly')).toBe(true);
        expect(kernel.ui.disable).toHaveBeenCalled();
        expect(button.disabled).toBe(false); // トグルだけは再クリックできる
        expect(toggle.textContent).toBe('OFF');
        expect(toggle.hasAttribute('lock')).toBe(true);
    });

    it('閲覧(OFF)→編集(ON): enable して undo/redo ボタンを再同期する', () => {
        const { plugin, kernel, topArea, toggle } = setupPlugin();

        plugin.action(); // ON→OFF
        plugin.action(); // OFF→ON

        expect(topArea.classList.contains('is-readonly')).toBe(false);
        expect(kernel.ui.enable).toHaveBeenCalled();
        expect(kernel.history.resetButtons).toHaveBeenCalledWith('root');
        expect(toggle.textContent).toBe('ON');
        expect(toggle.hasAttribute('lock')).toBe(false);
    });
});

describe('value の getter / setter', () => {
    const asValue = (el: HTMLElement) => el as unknown as { value: string };

    it('get: エディタがあれば html.get() の結果を返す', () => {
        h.state.isEmpty = true;
        const { el } = mountEditor({ value: '' });
        expect(asValue(el).value).toBe('<p>value</p>'); // フェイク html.get() の戻り
    });

    it('get: エディタが無い(読取ビューア)場合は空文字を返す', () => {
        const { el } = mountEditor({ value: '<p>x</p>', readonly: true });
        expect(asValue(el).value).toBe('');
    });

    it('set: エディタがあれば html.set() へ流す', () => {
        h.state.isEmpty = true;
        const { el } = mountEditor({ value: '' });

        asValue(el).value = '<p>NEW</p>';

        expect(h.refs.instance!.$.html.set).toHaveBeenCalledWith('<p>NEW</p>');
    });

    it('set: 読取+smartdesign ではビューアへサニタイズして反映し、空は空段落にする', () => {
        const { el } = mountEditor({ value: '<p>init</p>', readonly: true, smartdesign: true });
        const viewer = el.querySelector('.sun-editor-editable')!;

        asValue(el).value = '<p>updated</p>';
        expect(viewer.innerHTML).toContain('updated');

        asValue(el).value = ''; // 空は空段落フォールバック (val || EMPTY_LINE) が適用される
        // 実装と同一経路 (sanitizeViewerHtml: DOMPurify → DOMParser 再シリアライズ) の出力と比較する。
        // 素の DOMPurify.sanitize と比較すると入力次第でシリアライズ差により脆くなるため。
        expect(viewer.innerHTML).toBe(sanitizeViewerHtml('<p><br></p>'));
        expect(viewer.innerHTML).not.toBe(''); // 空文字にはならない
    });

    it('set: 読取でも smartdesign でなければビューアを書き換えない', () => {
        const { el } = mountEditor({ value: '<p>keep</p>', readonly: true });
        const viewer = el.querySelector('.sun-editor-editable')!;
        const before = viewer.innerHTML;

        asValue(el).value = '<p>ignored</p>';

        expect(viewer.innerHTML).toBe(before);
    });
});

describe('画像クリック (imageViewerHandle / setupImageViewerModal)', () => {
    /** wysiwyg 配下に figure>img を作り、その img をクリックする */
    function clickImageInEditor() {
        const wysiwyg = h.refs.instance!.$.frameContext.get('wysiwyg') as HTMLElement;
        const figure = document.createElement('figure');
        const image = document.createElement('img');
        image.src = 'https://host/x.png';
        figure.appendChild(image);
        wysiwyg.appendChild(figure);
        image.dispatchEvent(new MouseEvent('click', { bubbles: true, composed: true }));
        return image;
    }

    it('enablelightbox=1 でライトボックス用モーダルを生成する', () => {
        const Ctor = customElements.get('rt-editor') as unknown as { imageViewerModal?: HTMLElement };
        h.state.isEmpty = true;
        mountEditor({ value: '', enablelightbox: true });

        expect(Ctor.imageViewerModal).toBeInstanceOf(HTMLElement);
        expect(document.querySelector('image-viewer-modal')).not.toBeNull();
    });

    it('モーダルがある場合は show() を呼ぶ', () => {
        const Ctor = customElements.get('rt-editor') as unknown as {
            imageViewerModal: { show: ReturnType<typeof vi.fn> };
        };
        Ctor.imageViewerModal = { show: vi.fn() };
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        const image = clickImageInEditor();

        expect(Ctor.imageViewerModal.show).toHaveBeenCalledWith(image, expect.anything());
    });

    it('モーダルが無い場合は別タブ (window.open) で開く', () => {
        const openSpy = vi.spyOn(window, 'open').mockReturnValue(null);
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        clickImageInEditor();

        // reverse-tabnabbing 対策で opener を無効化して開く
        expect(openSpy).toHaveBeenCalledWith('https://host/x.png', '_blank', 'noopener,noreferrer');
    });

    it('IMG 以外のクリックは無視する', () => {
        const openSpy = vi.spyOn(window, 'open').mockReturnValue(null);
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const wysiwyg = h.refs.instance!.$.frameContext.get('wysiwyg') as HTMLElement;
        const span = document.createElement('span');
        wysiwyg.appendChild(span);

        span.dispatchEvent(new MouseEvent('click', { bubbles: true, composed: true }));

        expect(openSpy).not.toHaveBeenCalled();
    });
});

describe('確認ボタンでモーダルを閉じる (setupModalAutoClose)', () => {
    it('submit 後、背景表示中 (se-backdrop-show) なら offCurrentModal を呼ぶ', async () => {
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const modalArea = h.refs.instance!._els.modalArea;
        modalArea.classList.add('se-backdrop-show'); // まだ開いている

        modalArea.dispatchEvent(new Event('submit', { bubbles: true }));
        await flush();

        expect(h.refs.instance!.$.ui.offCurrentModal).toHaveBeenCalled();
    });

    it('既に閉じている (se-backdrop-show 無し) 場合は二重で閉じない', async () => {
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const modalArea = h.refs.instance!._els.modalArea;

        modalArea.dispatchEvent(new Event('submit', { bubbles: true }));
        await flush();

        expect(h.refs.instance!.$.ui.offCurrentModal).not.toHaveBeenCalled();
    });
});

describe('Safari のテーブル編集無効化 (disableTableEditingOnSafari)', () => {
    it('Safari では追加された table を contenteditable=false にし空セルを補正する', async () => {
        const Ctor = customElements.get('rt-editor') as unknown as { isSafari?: boolean };
        Ctor.isSafari = true; // Safari とみなす
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        // 初期化後の table 追加 → MutationObserver 経由で再処理される
        const wysiwyg = h.refs.instance!.$.frameContext.get('wysiwyg') as HTMLElement;
        const table = document.createElement('table');
        table.innerHTML = '<tr><td><div><br></div></td></tr>';
        wysiwyg.appendChild(table);
        await flush();

        expect(table.getAttribute('contenteditable')).toBe('false');
        const cellDiv = table.querySelector('td > div')!;
        expect(cellDiv.innerHTML).not.toBe('<br>'); // 空セルはゼロ幅文字で補正される
        expect(cellDiv.innerHTML).toContain('<br>');
    });
});

describe('スタイル注入 (initStyle)', () => {
    it('非smartdesign: 共有スタイルを head へ一度だけ注入する (2つ目以降はスキップ)', () => {
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        mountEditor({ value: '' }); // 2つ目は #rteCustomCss 既存のため早期 return

        expect(document.head.querySelectorAll('#rteCustomCss').length).toBe(1);
    });

    it('smartdesign: 共有せず自要素配下へ注入する', () => {
        h.state.isEmpty = true;
        const { el } = mountEditor({ value: '', smartdesign: true });

        expect(el.querySelector('#rteCustomCss')).not.toBeNull();
        expect(document.head.querySelector('#rteCustomCss')).toBeNull();
    });
});

describe('既定フォント (applyDefaultFont)', () => {
    it('#RteDefaultFont を編集領域の font-family に設定する', () => {
        const input = document.createElement('input');
        input.id = 'RteDefaultFont';
        input.value = '游ゴシック,Arial';
        document.body.appendChild(input);
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        const wysiwyg = h.refs.instance!.$.frameContext.get('wysiwyg') as HTMLElement;
        expect(wysiwyg.style.fontFamily).toContain('游ゴシック');
    });
});

describe('sticky ツールバー (getStickyToolbarOffset)', () => {
    it('レスポンシブ時は #Header の高さを toolbar_sticky に渡す', () => {
        const Ctor = customElements.get('rt-editor') as unknown as { isResponsive?: boolean };
        Ctor.isResponsive = true;
        const header = document.createElement('div');
        header.id = 'Header';
        Object.defineProperty(header, 'offsetHeight', { value: 48, configurable: true });
        document.body.appendChild(header);
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        expect(h.refs.createOptions!.toolbar_sticky).toBe(48);
    });

    it('非レスポンシブ時は 0 を渡す', () => {
        const Ctor = customElements.get('rt-editor') as unknown as { isResponsive?: boolean };
        Ctor.isResponsive = false;
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        expect(h.refs.createOptions!.toolbar_sticky).toBe(0);
    });

    it('レスポンシブでも #Header が無ければ 0 を返す (ダイアログ内 RTE 等で TypeError にしない)', () => {
        const Ctor = customElements.get('rt-editor') as unknown as { isResponsive?: boolean };
        Ctor.isResponsive = true; // responsive だが #Header は DOM に無い
        h.state.isEmpty = true;
        mountEditor({ value: '' });

        expect(h.refs.createOptions!.toolbar_sticky).toBe(0);
    });
});

describe('選択範囲のエディタ外流出防止 (keepSelectionInEditor)', () => {
    const fireToolbarMousedown = () => {
        const toolbar = h.refs.instance!.$.context.get('toolbar_main') as HTMLElement;
        toolbar.dispatchEvent(new MouseEvent('mousedown', { bubbles: true }));
    };
    const selectContents = (node: Node) => {
        const range = document.createRange();
        range.selectNodeContents(node);
        const sel = window.getSelection()!;
        sel.removeAllRanges();
        sel.addRange(range);
    };

    it('選択がエディタ外なら focusEdge でキャレットを戻す', () => {
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const outside = document.createElement('div');
        outside.textContent = 'outside';
        document.body.appendChild(outside);
        selectContents(outside);

        fireToolbarMousedown();

        expect(h.refs.instance!.$.focusManager.focusEdge).toHaveBeenCalled();
    });

    it('末尾が画像コンポーネントなら focusEdge で選択せず focus() で戻す (画像貼付後のトグル回帰)', () => {
        // focusEdge は末尾コンポーネントを component.select してしまい、ツールバー操作(編集トグル等)が
        // 画像選択＋操作メニュー表示に化ける。末尾がコンポーネントのときは focus() で安全に戻す。
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const wysiwyg = h.refs.instance!.$.frameContext.get('wysiwyg') as HTMLElement;
        const figure = document.createElement('figure');
        figure.appendChild(document.createElement('img'));
        wysiwyg.appendChild(figure);
        // component.get が末尾要素をコンポーネントと判定するようにする
        vi.mocked(h.refs.instance!.$.component.get).mockReturnValue({ target: figure } as never);
        const outside = document.createElement('div');
        outside.textContent = 'outside';
        document.body.appendChild(outside);
        selectContents(outside);

        fireToolbarMousedown();

        expect(h.refs.instance!.$.focusManager.focus).toHaveBeenCalled();
        expect(h.refs.instance!.$.focusManager.focusEdge).not.toHaveBeenCalled();
    });

    it('選択がエディタ内なら何もしない', () => {
        h.state.isEmpty = true;
        mountEditor({ value: '' });
        const wysiwyg = h.refs.instance!.$.frameContext.get('wysiwyg') as HTMLElement;
        document.body.appendChild(wysiwyg); // 選択のため接続する
        const inside = document.createElement('p');
        inside.textContent = 'inside';
        wysiwyg.appendChild(inside);
        selectContents(inside);

        fireToolbarMousedown();

        expect(h.refs.instance!.$.focusManager.focusEdge).not.toHaveBeenCalled();
    });

    it('読み取り専用中 (isDisabled) は何もしない', () => {
        h.state.isEmpty = true;
        h.state.isDisabled = true;
        mountEditor({ value: '' });
        const outside = document.createElement('div');
        outside.textContent = 'outside';
        document.body.appendChild(outside);
        selectContents(outside);

        fireToolbarMousedown();

        expect(h.refs.instance!.$.focusManager.focusEdge).not.toHaveBeenCalled();
    });
});
