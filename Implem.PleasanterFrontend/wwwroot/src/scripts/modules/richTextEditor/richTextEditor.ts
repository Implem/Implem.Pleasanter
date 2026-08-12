import suneditor, { interfaces, plugins, langs } from 'suneditor';
import DOMPurify from 'dompurify';
import $ from 'jquery';

import type { _Lang } from 'suneditor/langs/_Lang';

import { ImageViewerModal } from '../../generals/modal/imageViewerModal';

import suneditorEditorCss from 'suneditor/css/editor?inline';
import suneditorContentsCss from 'suneditor/css/contents?inline';
import css from './richTextEditor.scss?inline';

const { PluginCommand } = interfaces;
// buttonList で使用するプラグインのみを利用する (未使用の math/drawing/exportPDF 等はツリーシェイクされる)
const {
    font,
    fontSize,
    blockStyle,
    fontColor,
    backgroundColor,
    textStyle,
    align,
    hr,
    list,
    lineHeight,
    table,
    link,
    image
} = plugins;
const { zh_cn, en, ja, de, ko, es } = langs;

/** カスタム要素名 (C# 側が `<rt-editor>` を出力する) */
const TAG_NAME = 'rt-editor';
/** 注入するカスタムスタイルの id */
const CUSTOM_STYLE_ID = 'rteCustomCss';
/** 編集/読み取りトグルボタンのクラス名 (プラグインの static className と共有) */
const TOGGLE_BUTTON_CLASS = 'btn-editable-cmd';
/** 空の 1 行 (ビューア表示のフォールバック用) */
const EMPTY_LINE = '<p><br></p>';

/**
 * ビューア(読み取り専用 / smartdesign)表示用に HTML をサニタイズする。
 * DOMPurify は既定で `target` を除去するため、別タブ表示(target=_blank)を維持できるよう
 * `target`/`rel` を許可する。target=_blank には reverse-tabnabbing 対策で rel を補完する。
 * (編集モードは SunEditor の attributeWhitelist:{ a:'target|href|rel' } で保持されるため対象外)
 *
 * ※ ビューア出力の期待値をテストで再現できるよう、モジュール関数として公開している。
 */
export function sanitizeViewerHtml(html: string): string {
    // form はスクリプト実行ベクターではないが、閲覧表示にフォームは不要でフィッシングの
    // 余地になるため除去する (iframe/object/embed 等は DOMPurify 既定で除去される)
    const clean = DOMPurify.sanitize(html, { ADD_ATTR: ['target', 'rel'], FORBID_TAGS: ['form'] });
    const doc = new DOMParser().parseFromString(clean, 'text/html');
    // 既存 rel(nofollow 等)を保持しつつ、reverse-tabnabbing 対策の noopener/noreferrer を補完する
    doc.querySelectorAll('a[target="_blank"]').forEach(a => {
        const rel = new Set((a.getAttribute('rel') ?? '').split(/\s+/).filter(Boolean));
        rel.add('noopener');
        rel.add('noreferrer');
        a.setAttribute('rel', [...rel].join(' '));
    });
    return doc.body.innerHTML;
}

/** アップロード可能な画像の MIME タイプ / 拡張子 */
const VALID_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/gif'];
const VALID_IMAGE_EXTENSIONS = ['.jpg', '.jpeg', '.png', '.gif'];

/** `#Language` の値 → suneditor 言語パック (未定義の言語は en にフォールバック) */
const LANG_MAP: Record<string, _Lang> = { zh: zh_cn, ja, de, ko, es, en };

/** ツールバーのボタン構成 */
const TOOLBAR_BUTTON_LIST = [
    ['undo', 'redo'],
    ['font', 'fontSize', 'blockStyle'],
    ['bold', 'underline', 'italic', 'strike', 'subscript', 'superscript'],
    ['fontColor', 'backgroundColor', 'textStyle'],
    ['removeFormat', 'showBlocks'],
    ['outdent', 'indent'],
    ['align', 'hr', 'list', 'lineHeight'],
    ['table', 'link', 'image'],
    ['toggleReadonly']
];

/** 画像アップロード API のレスポンス要素 */
interface UploadResponseEntry {
    Method: string;
    Target: string;
    Value?: string;
    [key: string]: unknown;
}

/**
 * 画像プラグインの内部アップロードサービス。
 * `urlUpload(info)` は URL とモーダル設定(サイズ/alt/配置)から figure 画像コンポーネントを挿入する。
 * (自前アップロードした URL を、モーダルの設定を保持したまま挿入するために利用する)
 */
interface ImagePluginInternal {
    /** モーダルのキャプション有無チェックボックス (create() がここをライブで参照する) */
    captionCheckEl?: HTMLInputElement;
    uploadService?: {
        urlUpload?: (info: {
            url: string;
            files: { name: string; size: number };
            element: HTMLImageElement | null;
            anchor: HTMLElement | null;
            inputWidth: string;
            inputHeight: string;
            align: string;
            isUpdate: boolean;
            alt: string;
        }) => void;
    };
}

/**
 * 編集/読み取りをトグルするカスタムコマンドプラグイン (SunEditor v3 の `PluginCommand`)。
 *
 * 読み取り専用の制御には ui.disable()/enable() を使う。ui.readOnly() は
 * commandDispatcher の isButtonDisabled ゲート (isReadOnly を参照) がトグルボタン自身の
 * クリックまで弾き、読み取り専用から編集へ戻せなくなるため使わない。disable() は
 * isDisabled フラグで制御し、このゲートは isReadOnly しか見ないためトグルは再クリックできる。
 * ツールバーの見た目上の無効化は CSS (richTextEditor.scss の .is-readonly) が担当する。
 */
export class ToggleReadonlyPlugin extends PluginCommand {
    static key = 'toggleReadonly';
    static className = TOGGLE_BUTTON_CLASS;

    private isReadonlyMode = false;

    constructor(kernel: SunEditor.Kernel) {
        super(kernel);
        this.title = $p.display('EditModeToggle');
        this.inner = `${$p.display('Edit')} <div class="toggle">ON</div>`;
    }

    action(target?: HTMLElement) {
        this.isReadonlyMode = !this.isReadonlyMode;
        const topArea = this.$.frameContext.get('topArea');
        const button = (target?.closest(`.${TOGGLE_BUTTON_CLASS}`) ??
            this.$.context.get('toolbar_main')?.querySelector(`.${TOGGLE_BUTTON_CLASS}`)) as HTMLButtonElement | null;
        const toggleBtn = button?.querySelector('.toggle') as HTMLElement | null;

        // ツールバーの無効化表示は CSS 側で行う (richTextEditor.scss の .is-readonly)
        topArea?.classList.toggle('is-readonly', this.isReadonlyMode);

        if (this.isReadonlyMode) {
            // 本文編集を止める (contenteditable=false + isDisabled ガードの二重)
            this.$.ui.disable();
            // disable() はトグル自身も DOM 上 disabled にするため、トグルだけ戻して
            // runFromTarget の disabled 判定を通し、編集へ復帰できるようにする
            if (button) button.disabled = false;
        } else {
            this.$.ui.enable();
            // enable() は全ボタンを一律有効化するので undo/redo を履歴に合わせて再同期する
            // (setInitContent の history.reset() により、初期入力があるときは戻り無しになる)
            this.$.history.resetButtons(this.$.frameContext.get('key'));
        }

        if (toggleBtn) {
            toggleBtn.innerText = this.isReadonlyMode ? 'OFF' : 'ON';
            toggleBtn.toggleAttribute('lock', this.isReadonlyMode);
        }
    }
}

class RichTextEditorElement extends HTMLElement {
    static defaultfont?: string[];
    static fontList?: string[];
    static fontSize?: number[];
    static isResponsive?: boolean;
    static isSafari?: boolean;
    static imageViewerModal: HTMLElement;

    private isReadOnly = false;
    private isSmartdesign = false;
    private seLang: _Lang;
    private sunEditor?: SunEditor.Instance;
    private editorContainer: HTMLElement = document.createElement('div');
    private viewerContainer?: HTMLElement;
    private safariObserver?: MutationObserver;
    private smartDesignObserver?: MutationObserver;
    private controller?: HTMLTextAreaElement | null;

    constructor() {
        super();
        this.appendChild(this.editorContainer);
        const lang = (document.querySelector('#Language') as HTMLInputElement)?.value || 'en';
        this.seLang = LANG_MAP[lang] ?? en;
    }

    // --- ライフサイクル ---------------------------------------------------

    connectedCallback() {
        this.controller = this.querySelector('textarea');
        if (!this.controller) return;

        if (this.controller.dataset.readonly || this.controller.disabled) this.isReadOnly = true;
        if (this.dataset.smartdesign) this.isSmartdesign = true;
        this.setupImageViewerModal();
        if (RichTextEditorElement.isSafari === undefined) {
            RichTextEditorElement.isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
        }

        this.initStyle();
        if (this.isReadOnly) {
            this.viewerInit();
            this.editorContainer.addEventListener('click', this.imageViewerHandle);
        } else {
            this.editorInit();
            this.setInitContent();
            if (this.sunEditor) {
                this.sunEditor.$.frameContext.get('wysiwyg').addEventListener('click', this.imageViewerHandle);
                this.smartDesignSetting();
            }
        }

        if (RichTextEditorElement.isResponsive === undefined) {
            RichTextEditorElement.isResponsive =
                window.getComputedStyle(document.querySelector('head')!).fontFamily === 'responsive';
        }
    }

    disconnectedCallback() {
        if (this.sunEditor) {
            this.sunEditor.destroy();
            this.sunEditor = undefined;
        }
        this.safariObserver?.disconnect();
        this.smartDesignObserver?.disconnect();
    }

    // --- 編集モードの初期化 -----------------------------------------------

    private editorInit() {
        this.cacheFontSettings();

        this.sunEditor = suneditor.create(this.editorContainer, {
            lang: this.seLang,
            placeholder: this.controller?.getAttribute('placeholder') || '',
            width: '100%',
            height: 'auto',
            plugins: {
                font,
                fontSize,
                blockStyle,
                fontColor,
                backgroundColor,
                textStyle,
                align,
                hr,
                list,
                lineHeight,
                table,
                link,
                image,
                toggleReadonly: ToggleReadonlyPlugin
            },
            buttonList: TOOLBAR_BUTTON_LIST,
            attributeWhitelist: {
                img: 'src|alt|width|height',
                a: 'target|href|rel'
            },
            font: RichTextEditorElement.fontList ? { items: RichTextEditorElement.fontList } : undefined,
            fontSize: RichTextEditorElement.buildFontSizeOption(),
            toolbar_sticky: this.getStickyToolbarOffset(),
            closeModalOutsideClick: true,
            editableFrameAttributes: { spellcheck: 'true' },
            events: {
                onChange: this.onChange,
                onPaste: this.onPaste,
                onImageUploadBefore: this.onImageUpload
            }
        });

        this.applyDefaultFont();
        this.setupToolbarGuards();
        this.setupModalAutoClose();
    }

    /**
     * 確認ボタン押下時に必ずモーダルを閉じる。
     * v3 のモーダルは modalAction() が false を返すと閉じない (バリデーション相当)。
     * 特に画像モーダルは onImageUploadBefore で false を返して自前アップロードするため
     * 常に閉じなくなる。どこで弾かれたか UI から分からず操作不能に見えるため、確認後に閉じる。
     *
     * - suneditor の submit ハンドラ(#Action)は stopPropagation するので capture で先に拾い、
     *   閉じ処理自体は setTimeout に載せて値の適用(#Action 内のマイクロタスク)を先に走らせる。
     * - 自己クローズ済みのモーダルを再度閉じると hidePopover が例外を投げるため、
     *   se-backdrop-show クラスが残っている(まだ開いている)場合のみ閉じる。
     */
    private setupModalAutoClose() {
        const modal = this.sunEditor?.$.contextProvider.carrierWrapper?.querySelector('.se-modal.se-modal-area');
        if (!modal) return;
        modal.addEventListener(
            'submit',
            () => {
                setTimeout(() => {
                    if (modal.classList.contains('se-backdrop-show')) {
                        this.sunEditor?.$.ui.offCurrentModal();
                    }
                }, 0);
            },
            true
        );
    }

    /** #Rte* の hidden input からフォント設定を読み込み、静的プロパティにキャッシュする */
    private cacheFontSettings() {
        RichTextEditorElement.defaultfont ??= (
            document.querySelector('#RteDefaultFont') as HTMLInputElement
        )?.value.split(',');
        RichTextEditorElement.fontList ??= (document.querySelector('#RteFontList') as HTMLInputElement)?.value.split(
            ','
        );
        RichTextEditorElement.fontSize ??= (document.querySelector('#RteFontSize') as HTMLInputElement)?.value
            .split(',')
            .map(Number);
    }

    /** fontSize プラグインのオプションを構築する (既定サイズは v2 に合わせ 13px) */
    private static buildFontSizeOption() {
        const sizes = RichTextEditorElement.fontSize;
        if (!sizes || !sizes.length) return undefined;
        return {
            unitMap: {
                px: { default: 13, inc: 1, min: Math.min(...sizes), max: Math.max(...sizes), list: sizes }
            }
        };
    }

    /** レスポンシブ時はヘッダー高さ分ツールバーを sticky させる */
    private getStickyToolbarOffset(): number {
        if (!RichTextEditorElement.isResponsive) return 0;
        // responsive でも #Header が無い画面 (ダイアログ内 RTE 等) では null になり得るため、
        // 参照して TypeError で初期化ごと失敗しないよう null 安全にする
        return (document.querySelector('#Header') as HTMLElement | null)?.offsetHeight ?? 0;
    }

    /** v2 の defaultStyle 相当: 編集領域の既定フォントを設定 */
    private applyDefaultFont() {
        if (!this.sunEditor || !RichTextEditorElement.defaultfont) return;
        this.sunEditor.$.frameContext.get('wysiwyg').style.fontFamily = RichTextEditorElement.defaultfont.join(', ');
    }

    /**
     * ツールバー操作時のガード (capture フェーズで suneditor の処理より先に実行)。
     * エディタはインライン(同一ドキュメント)のため、選択がエディタ外にまたがった状態で
     * indent/list/align 等のブロック系コマンドを実行すると対象行の探索がエディタ外へ及び、
     * エディタ外要素の style(margin 等)まで書き換えてしまう。選択がエディタ外に及ぶ場合は
     * キャレットをエディタ内へ collapse し、コマンドがエディタ外へ作用しないようにする。
     * (読み取り専用中のツールバー無効化は richTextEditor.scss の is-readonly 側で行う)
     */
    private setupToolbarGuards() {
        this.sunEditor?.$.context.get('toolbar_main')?.addEventListener('mousedown', this.keepSelectionInEditor, true);
    }

    /** 選択がエディタ外に及ぶ場合、キャレットをエディタ内へ collapse する */
    private keepSelectionInEditor = () => {
        if (!this.sunEditor) return;
        const wysiwyg = this.sunEditor.$.frameContext.get('wysiwyg');
        // 読み取り専用中(本文が編集不可)は不要
        if (this.sunEditor.$.frameContext.get('isDisabled')) return;
        const selection = window.getSelection();
        if (!selection || selection.rangeCount === 0) return;
        const inEditor = (node: Node | null) => !!node && wysiwyg.contains(node);
        if (!inEditor(selection.anchorNode) || !inEditor(selection.focusNode)) {
            const last = wysiwyg.lastElementChild;
            // 末尾が画像等のコンポーネントだと focusEdge がそれを「選択」してしまい
            // (focusManager.focusEdge → component.select)、ツールバー操作(編集トグル等)が
            // 画像選択＋操作メニュー表示に化ける。その場合は focus() で安全にキャレットを戻す。
            if (last && this.sunEditor.$.component.get(last)) {
                this.sunEditor.$.focusManager.focus();
            } else {
                this.sunEditor.$.focusManager.focusEdge(last);
            }
        }
    };

    /**
     * 初期コンテンツを設定する。
     * 内容がある場合はトグルをクリックして読み取りモードで開き、直後に history.reset() で
     * 履歴を破棄して初期入力に対する「元に戻す」を無効化する。
     */
    private setInitContent() {
        if (!this.controller || !this.sunEditor) return;
        // 値を流し込んでから editor.isEmpty() で空判定する。
        // 文字列の truthy 判定だと <p><br></p> 等の実質空データも「内容あり」扱いになり、
        // 再読込時に読み取りモードで開いてしまう。isEmpty() は実内容(テキスト/画像・表等)で判定する。
        this.sunEditor.$.html.set(this.controller.value);
        if (this.sunEditor.isEmpty()) {
            // 実質空なら空文字へ正規化し、編集モードのまま開く
            this.controller.value = '';
        } else {
            // 内容がある場合は読み取りモードで開く
            (this.querySelector(`.${TOGGLE_BUTTON_CLASS}`) as HTMLElement)?.click();
        }
        // 初期入力に対する「元に戻す」を無効化 (html.set の履歴を破棄)
        this.sunEditor.$.history.reset();
        this.disableTableEditingOnSafari({ observe: true });
    }

    // --- 読み取り専用ビューア ---------------------------------------------

    private viewerInit() {
        if (!this.controller) return;
        this.viewerContainer = this.htmlParser('<div class="sun-editor-editable"></div>');
        this.viewerContainer.style.fontFamily = (document.querySelector('#RteDefaultFont') as HTMLInputElement)?.value;
        // ビューア(プレーン div)表示用フォールバック: 空だと div が高さ0で潰れるため空段落を出す
        this.viewerContainer.innerHTML = sanitizeViewerHtml(this.controller.value || EMPTY_LINE);
        this.editorContainer.classList.add(this.controller.disabled ? 'app-disabled' : 'app-readonly');
        this.editorContainer.append(this.viewerContainer);
    }

    // --- エディタイベント -------------------------------------------------

    private onChange = ({ data }: { data: string }) => {
        if (!this.controller) return;
        // 実質空(空段落/空白/ゼロ幅等)は空文字に正規化する。
        // 文字列一致より editor.isEmpty() の方が網羅的 (data と現在の wysiwyg 内容は一致する)。
        const value = this.sunEditor?.isEmpty() ? '' : data;
        this.controller.value = value;
        $p.set($(this.controller), value);
        if (value) this.controller.dispatchEvent(new Event('change', { bubbles: true }));
        this.smartDesignValueBind(value);
    };

    private onPaste = async ({ event, data }: SunEditor.EventParams.ClipboardEvent): Promise<string | boolean> => {
        // 画像ファイルの貼り付けはアップロードして自前で挿入する
        const items = (event as ClipboardEvent).clipboardData!.items;
        if (items.length && items[0].kind === 'file') {
            for (let i = 0; i < items.length; i++) {
                if (this.sunEditor && items[i].kind === 'file') {
                    // 貼り付けはモーダル設定が無いため素の <img> 挿入
                    this.sunEditor.$.ui.disable();
                    this.uploadBinary(items[i].getAsFile());
                    this.sunEditor.$.ui.enable();
                }
            }
        }
        return this.autoLinkUrls(data);
    };

    /**
     * 既存の <a> タグを保持しつつ、素の URL を <a> リンクへ変換する。
     *
     * 文字列置換ではなく DOM をパースし「テキストノード中の URL」だけを変換する。
     * こうすることで `<img src="http://…">` のような属性値内の URL は (テキストノードに
     * 含まれないため) 対象外になり、画像コピー&ペースト時に src が壊れる不具合を防ぐ。
     * 既存の <a> 内テキストは二重リンク化しないようスキップする。
     */
    private autoLinkUrls(html: string): string {
        // g フラグ付き正規表現は lastIndex を保持するため、判定(test)と抽出(matchAll)で
        // 同一インスタンスを使い回さず、都度生成して副作用を避ける
        const makeUrlRegex = () => /(https?:\/\/[^\s<>"']+)/g;
        const doc = new DOMParser().parseFromString(html, 'text/html');
        const walker = doc.createTreeWalker(doc.body, NodeFilter.SHOW_TEXT);

        // 走査中に置換すると木が変わるため、対象テキストノードを先に集める
        const targets: Text[] = [];
        for (let node = walker.nextNode(); node; node = walker.nextNode()) {
            const text = node as Text;
            if (text.parentElement?.closest('a')) continue; // 既存 <a> 内は対象外
            if (text.nodeValue && makeUrlRegex().test(text.nodeValue)) targets.push(text);
        }

        for (const node of targets) {
            const text = node.nodeValue ?? '';
            const frag = doc.createDocumentFragment();
            let last = 0;
            for (const match of text.matchAll(makeUrlRegex())) {
                const url = match[0];
                const offset = match.index ?? 0;
                if (offset > last) frag.appendChild(doc.createTextNode(text.slice(last, offset)));
                const anchor = doc.createElement('a');
                anchor.setAttribute('href', url);
                anchor.setAttribute('target', '_blank');
                anchor.setAttribute('rel', 'noopener noreferrer');
                anchor.textContent = url;
                frag.appendChild(anchor);
                last = offset + url.length;
            }
            if (last < text.length) frag.appendChild(doc.createTextNode(text.slice(last)));
            node.parentNode?.replaceChild(frag, node);
        }
        return doc.body.innerHTML;
    }

    private onImageUpload = async ({ info }: { info: SunEditor.EventParams.ImageInfo }): Promise<boolean> => {
        // キャプション有無は #getInfo() に含まれず create() がモーダルのチェックボックスをライブ参照する。
        // アップロードは非同期で、その完了前にモーダルが閉じ modalInit() でチェックがリセットされるため、
        // 確定時点の値をここで同期キャプチャして挿入時に復元する。
        const caption = !!(this.sunEditor?.$.plugins.image as ImagePluginInternal | undefined)?.captionCheckEl?.checked;
        for (let i = 0; i < info.files.length; i++) {
            // モーダル/ドロップの設定(サイズ/alt/配置)を保持して挿入するため info を渡す
            this.uploadBinary(info.files[i], info, caption);
        }
        // デフォルトのアップロード処理をキャンセルし、自前で挿入する
        return false;
    };

    /** 画像を Pleasanter の binaries/formbinaries API へアップロードし、結果を本文へ挿入する */
    private uploadBinary = (blob: File | null, imageInfo?: SunEditor.EventParams.ImageInfo, caption = false) => {
        if (!this.isValidImage(blob)) return;
        if (!window.$p.validateImageUploadFileSize(blob)) return;

        const url = this.buildUploadUrl();
        if (!url) return;

        const controllerId = this.controller?.getAttribute('id');
        const formData = new FormData();
        if (controllerId) formData.append('ControlId', controllerId);
        formData.append('file', blob);

        window.$p.multiUpload(url, formData, undefined, undefined, (json: string) => {
            const entries = JSON.parse(json);
            if (window.$p.handleMessageFromJson(entries)) return;
            const inserted = (entries as UploadResponseEntry[]).find(
                entry => entry.Method === 'InsertText' && entry.Target === `#${controllerId}`
            );
            const imageUrl = inserted?.Value?.match(/!\[.*?\]\((.*?)\)/)?.[1];
            if (imageUrl) this.insertUploadedImage(imageUrl, blob, imageInfo, caption);
        });
    };

    /**
     * アップロード済み URL を本文へ挿入する。
     * モーダル/ドロップ経由 (imageInfo あり) は画像プラグインの urlUpload を用い、
     * サイズ/alt/配置を保持した figure コンポーネントとして挿入する。
     * urlUpload が利用できない場合や貼り付け経由 (imageInfo なし) は素の <img> にフォールバックする。
     */
    private insertUploadedImage(
        imageUrl: string,
        blob: File,
        imageInfo?: SunEditor.EventParams.ImageInfo,
        caption = false
    ) {
        if (!this.sunEditor) return;
        const imagePlugin = imageInfo ? (this.sunEditor.$.plugins.image as ImagePluginInternal | undefined) : undefined;
        if (imageInfo && typeof imagePlugin?.uploadService?.urlUpload === 'function') {
            // create() はキャプション有無をチェックボックスからライブで読むが、非同期挿入時には
            // モーダルが閉じ modalInit() でリセット済みのため、確定時の値へ戻してから挿入する
            if (imagePlugin.captionCheckEl) imagePlugin.captionCheckEl.checked = caption;
            imagePlugin.uploadService.urlUpload({
                url: imageUrl,
                files: { name: blob.name, size: blob.size },
                element: imageInfo.element,
                anchor: imageInfo.anchor,
                inputWidth: imageInfo.inputWidth,
                inputHeight: imageInfo.inputHeight,
                align: imageInfo.align,
                isUpdate: imageInfo.isUpdate,
                alt: imageInfo.alt
            });
        } else {
            this.sunEditor.$.html.insert(`<p><img src="${imageUrl}"></p>`);
        }
    }

    /** アップロード可能な画像かどうか (種別・拡張子) を判定する */
    private isValidImage(blob: File | null): blob is File {
        if (!blob) return false;
        const typeIsValid = VALID_IMAGE_TYPES.includes(blob.type);
        const nameIsValid = !!blob.name && VALID_IMAGE_EXTENSIONS.some(ext => blob.name.toLowerCase().endsWith(ext));
        return typeIsValid && nameIsValid;
    }

    /** 画像アップロード先 URL を構築する (ダイアログ内 / フォーム / 通常画面で分岐) */
    private buildUploadUrl(): string | undefined {
        const uploadController = window.$p.isForm() ? 'formbinaries' : 'binaries';
        const dialogRecordId = (document.querySelector('#EditorInDialogRecordId') as HTMLInputElement)?.value;
        const baseUrl = dialogRecordId
            ? (document.querySelector('#BaseUrl') as HTMLInputElement)?.value + dialogRecordId + '/_action_'
            : document.querySelector('.main-form')?.getAttribute('action');
        return baseUrl?.replace('_action_', `${uploadController}/uploadimage`);
    }

    /** 画像クリックでライトボックス (image-viewer-modal) を開く */
    private imageViewerHandle = (event: Event) => {
        const path = event.composedPath();
        if ((path[0] as HTMLElement).tagName !== 'IMG') return;
        const imgNode = path[0] as HTMLImageElement;
        if (!this.sunEditor && !this.isReadOnly) return;

        if (RichTextEditorElement.imageViewerModal) {
            const wrap = this.isReadOnly ? this.editorContainer : this.sunEditor!.$.frameContext.get('wysiwyg');
            const imgNodes: NodeListOf<HTMLImageElement> = wrap.querySelectorAll('figure img');
            (RichTextEditorElement.imageViewerModal as ImageViewerModal).show(imgNode, imgNodes);
        } else {
            window.open(imgNode.src, '_blank', 'noopener,noreferrer');
        }
    };

    // --- スマートデザイン連携 ---------------------------------------------

    private smartDesignSetting() {
        if (!this.isSmartdesign || !this.controller) return;
        // placeholder 属性の変更をエディタの placeholder 表示へ反映する
        this.smartDesignObserver = new MutationObserver(mutations => {
            for (const mutation of mutations) {
                if (mutation.type === 'attributes' && mutation.attributeName === 'placeholder') {
                    const placeholderEl = this.sunEditor?.$.frameContext.get('placeholder');
                    if (placeholderEl) placeholderEl.textContent = this.controller?.placeholder || '';
                }
            }
        });
        this.smartDesignObserver.observe(this.controller, { attributes: true, attributeFilter: ['placeholder'] });
    }

    private smartDesignValueBind(newValue: string) {
        if (!this.isSmartdesign) return;
        this.dispatchEvent(
            new CustomEvent('demochange', { detail: { value: newValue }, bubbles: true, composed: true })
        );
    }

    // --- Safari 対応 ------------------------------------------------------

    /** Safari では table の編集を無効化し、空セルの表示を補正する */
    private disableTableEditingOnSafari({ observe = false } = {}) {
        if (!RichTextEditorElement.isSafari || !this.sunEditor) return;

        const wysiwyg = this.sunEditor.$.frameContext.get('wysiwyg');
        wysiwyg.querySelectorAll('table').forEach((table: HTMLTableElement) => {
            table.setAttribute('contenteditable', 'false');
            table.querySelectorAll('td > div, th > div').forEach(div => {
                if (div.innerHTML === '<br>' || div.innerHTML === '') {
                    div.innerHTML = '&#8203;<br>';
                }
            });
        });

        if (observe && !this.safariObserver) {
            this.safariObserver = new MutationObserver(() => this.disableTableEditingOnSafari());
            this.safariObserver.observe(wysiwyg, { childList: true, subtree: true, characterData: true });
        }
    }

    // --- スタイル / ユーティリティ ----------------------------------------

    /** suneditor 本体 CSS + 独自 CSS を <style> として注入する (smartdesign 時は自要素配下へ) */
    private initStyle() {
        if (document.querySelector(`#${CUSTOM_STYLE_ID}`) && !this.isSmartdesign) return;
        const style = document.createElement('style');
        style.setAttribute('id', CUSTOM_STYLE_ID);
        style.textContent = css + suneditorEditorCss + suneditorContentsCss;
        if (this.isSmartdesign) {
            this.appendChild(style);
        } else {
            document.querySelector('head')!.appendChild(style);
        }
    }

    /** enablelightbox 指定時、ライトボックス用モーダルを一度だけ生成する */
    private setupImageViewerModal() {
        if (this.controller?.dataset.enablelightbox === '1' && !RichTextEditorElement.imageViewerModal) {
            RichTextEditorElement.imageViewerModal = document.createElement('image-viewer-modal');
            document.body.appendChild(RichTextEditorElement.imageViewerModal);
        }
    }

    private htmlParser(htmlString: string) {
        const doc = new DOMParser().parseFromString(htmlString, 'text/html');
        return doc.body.firstChild as HTMLDivElement;
    }

    // --- 値の getter / setter (外部 API) ----------------------------------

    get value(): string {
        return this.sunEditor ? this.sunEditor.$.html.get() : '';
    }

    set value(val: string) {
        if (this.sunEditor) {
            // エディタは空文字でも suneditor 自身が空段落を払い出すためフォールバック不要
            this.sunEditor.$.html.set(val);
        } else if (this.viewerContainer && this.isSmartdesign) {
            // ビューア(プレーン div)表示用フォールバック: 空だと div が高さ0で潰れるため空段落を出す
            this.viewerContainer.innerHTML = sanitizeViewerHtml(val || EMPTY_LINE);
        }
    }
}

customElements.define(TAG_NAME, RichTextEditorElement);
