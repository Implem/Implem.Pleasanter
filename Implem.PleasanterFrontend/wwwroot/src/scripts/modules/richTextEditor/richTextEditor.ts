import SunEditor from 'suneditor';
import DOMPurify from 'dompurify';
import $ from 'jquery';

import SunEditorCore, { Core } from 'suneditor/src/lib/core';
import { Lang } from 'suneditor/src/lang/Lang';
import plugins from 'suneditor/src/plugins';
import zh_cn from 'suneditor/src/lang/zh_cn';
import en from 'suneditor/src/lang/en';
import ja from 'suneditor/src/lang/ja';
import de from 'suneditor/src/lang/de';
import ko from 'suneditor/src/lang/ko';
import es from 'suneditor/src/lang/es';

import suneditorCssRaw from 'suneditor/dist/css/suneditor.min.css?raw';
import css from './richTextEditor.scss?inline';

class RichTextEditorElement extends HTMLElement {
    static defaultfont?: string[];
    static fontList?: string[];
    static fontSize?: number[];
    static isResponsive?: boolean;
    static isSafari?: boolean;
    static imageViewerModal: HTMLElement;
    private isReadOnly?: boolean = false;
    private isSmartdesign?: boolean = false;
    private myLang: string;
    private sunEditor?: SunEditorCore;
    private seLang: Lang;
    private editorContainer: HTMLElement = document.createElement('div');
    private viewerContainer?: HTMLElement;
    private safariObserver?: MutationObserver;

    private controller?: HTMLTextAreaElement | null;

    constructor() {
        super();
        this.appendChild(this.editorContainer);

        this.myLang = (document.querySelector('#Language') as HTMLInputElement)?.value || 'en';
        switch (this.myLang) {
            case 'zh':
                this.seLang = zh_cn;
                break;
            case 'ja':
                this.seLang = ja;
                break;
            case 'de':
                this.seLang = de;
                break;
            case 'ko':
                this.seLang = ko;
                break;
            case 'es':
                this.seLang = es;
                break;
            default:
                this.seLang = en;
                break;
        }
    }

    connectedCallback() {
        this.controller = this.querySelector('textarea');
        if (!this.controller) return;
        if (this.controller.dataset.readonly || this.controller.disabled) this.isReadOnly = true;
        if (this.controller.dataset.enablelightbox === '1' && !RichTextEditorElement.imageViewerModal) {
            RichTextEditorElement.imageViewerModal = document.createElement('image-viewer-modal');
            document.body.appendChild(RichTextEditorElement.imageViewerModal);
        }
        if (this.dataset.smartdesign) this.isSmartdesign = true;
        if (RichTextEditorElement.isSafari === undefined) {
            RichTextEditorElement.isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
        }

        this.initStyle();
        if (!this.isReadOnly) {
            this.editorInit();
            this.setInitContent();
            if (this.sunEditor) {
                this.sunEditor.onChange = this.onChange;
                this.sunEditor.onPaste = this.onPaste;
                this.sunEditor.onImageUploadBefore = this.onImageUpload;
                this.sunEditor.core.context.element.wysiwyg.addEventListener('click', this.imageViewerHandle);
                this.smartDesignSetting();
            }
        } else {
            this.viewerInit();
            this.editorContainer.addEventListener('click', this.imageViewerHandle);
        }
        if (RichTextEditorElement.isResponsive === undefined) {
            RichTextEditorElement.isResponsive =
                window.getComputedStyle(document.querySelector('head')!).fontFamily === 'responsive';
        }
    }

    private editorInit() {
        if (!RichTextEditorElement.defaultfont) {
            RichTextEditorElement.defaultfont = (
                document.querySelector('#RteDefaultFont') as HTMLInputElement
            )?.value.split(',');
        }
        if (!RichTextEditorElement.fontList) {
            RichTextEditorElement.fontList = (document.querySelector('#RteFontList') as HTMLInputElement)?.value.split(
                ','
            );
        }
        if (!RichTextEditorElement.fontSize) {
            RichTextEditorElement.fontSize = (document.querySelector('#RteFontSize') as HTMLInputElement)?.value
                .split(',')
                .map(Number);
        }
        let stickyToolbar = 0;
        if (RichTextEditorElement.isResponsive) {
            stickyToolbar = (document.querySelector('#Header') as HTMLElement).offsetHeight;
        }

        this.sunEditor = SunEditor.create(this.editorContainer, {
            lang: this.seLang,
            placeholder: this.controller?.getAttribute('placeholder') || '',
            width: '100%',
            height: 'auto',
            plugins: [...Object.values(plugins), this.createToggleReadonlyPlugin()],
            buttonList: [
                ['undo', 'redo'],
                ['font', 'fontSize', 'formatBlock'],
                ['bold', 'underline', 'italic', 'strike', 'subscript', 'superscript'],
                ['fontColor', 'hiliteColor', 'textStyle'],
                ['removeFormat', 'showBlocks'],
                ['outdent', 'indent'],
                ['align', 'horizontalRule', 'list', 'lineHeight'],
                ['table', 'link', 'image'],
                ['toggleReadonly']
                // ['video', 'audio', 'showBlocks', 'codeView', 'paragraphStyle', 'template', 'preview', 'print'],
                // '/', // 改行
                // ['fullScreen', 'save', 'imageGallery', 'math'],
            ],
            // buttonList: [],
            attributesWhitelist: {
                img: 'src|alt|width|height',
                a: 'target|href|rel'
            },
            defaultStyle: RichTextEditorElement.defaultfont
                ? `font-family: ${RichTextEditorElement.defaultfont};`
                : undefined,
            font: RichTextEditorElement.fontList || undefined,
            fontSize: RichTextEditorElement.fontSize || undefined,
            stickyToolbar
        });
    }

    private viewerInit() {
        if (!this.controller) return;
        this.viewerContainer = this.htmlParser('<div class="sun-editor-editable"></div>');
        const defaultfont = (document.querySelector('#RteDefaultFont') as HTMLInputElement)?.value;
        this.viewerContainer.style.fontFamily = defaultfont;

        if (this.controller.value) {
            this.viewerContainer.innerHTML = DOMPurify.sanitize(this.controller.value);
        } else {
            this.viewerContainer.innerHTML = DOMPurify.sanitize(`<p><br></p>`);
        }
        if (!this.controller.disabled) {
            this.editorContainer.classList.add('app-readonly');
        } else {
            this.editorContainer.classList.add('app-disabled');
        }
        this.editorContainer.append(this.viewerContainer);
    }

    private setInitContent() {
        if (!this.controller) return;
        if (this.sunEditor && this.controller.value) {
            this.sunEditor.setContents(this.controller.value);
            (this.querySelector('.btn-editable-cmd') as HTMLElement).click();
            this.sunEditor.core.history.reset(this.controller.value);
        } else {
            this.controller.innerText = '<p><br></p>';
        }

        this.disableTableEditingOnSafari({ observe: true });
    }

    private disableTableEditingOnSafari({ observe = false } = {}) {
        if (!RichTextEditorElement.isSafari) return;

        const tables = this.sunEditor!.core.context.element.wysiwyg.querySelectorAll('table');
        tables.forEach((table: HTMLTableElement) => {
            table.setAttribute('contenteditable', 'false');
            table.querySelectorAll('td > div, th > div').forEach(div => {
                if (div.innerHTML === '<br>' || div.innerHTML === '') {
                    div.innerHTML = '&#8203;<br>';
                }
            });
        });

        if (observe && !this.safariObserver) {
            this.safariObserver = new MutationObserver(() => {
                this.disableTableEditingOnSafari();
            });
            this.safariObserver.observe(this.sunEditor!.core.context.element.wysiwyg, {
                childList: true,
                subtree: true,
                characterData: true
            });
        }
    }

    private onChange = (contents: string) => {
        if (this.controller) {
            if (contents == '<p>​<br></p>' || contents == '<p><br></p>') {
                this.controller.value = '';
                $p.set($(this.controller), '');
                this.smartDesignValueBind('');
            } else {
                this.controller.value = contents;
                $p.set($(this.controller), contents);
                this.smartDesignValueBind(contents);
            }
        }
    };

    private onPaste = (event: Event, cleanData: string, _maxCharCount: number, core: Core): string | boolean => {
        const items = (event as ClipboardEvent).clipboardData!.items;
        for (let i = 0; i < items.length; i++) {
            const item = items[i];
            if (this.sunEditor && item.kind === 'file') {
                this.sunEditor.disable();
                this.uploadBinary(item.getAsFile(), core);
                this.sunEditor.enable();
            }
        }

        const urlRegex = /(https?:\/\/[^\s<>"']+)/g;
        const replacedData = cleanData.replace(urlRegex, url => {
            return `<a href="${url}" target="_blank" rel="noopener noreferrer">${url}</a>`;
        });
        return replacedData;
    };

    private onImageUpload = (
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        files: any[],
        _info: object,
        core: Core,
        // eslint-disable-next-line @typescript-eslint/no-unsafe-function-type
        uploadHandler: Function
    ) => {
        for (let i = 0; i < files.length; i++) {
            this.uploadBinary(files[i], core, uploadHandler);
        }
        return false;
    };

    // eslint-disable-next-line @typescript-eslint/no-unsafe-function-type
    private uploadBinary = (blob: File | null, _core: Core, uploadHandler?: Function) => {
        const validTypes = ['image/jpeg', 'image/png', 'image/gif'];
        const validExtensions = ['.jpg', '.jpeg', '.png', '.gif'];

        const typeIsValid = blob && validTypes.includes(blob.type);
        const nameIsValid = blob && blob.name && validExtensions.some(ext => blob.name.toLowerCase().endsWith(ext));

        if (!typeIsValid || !nameIsValid) return;

        // url
        const uploadController = 'binaries';
        const dialogRecordId: string | null = (document.querySelector('#EditorInDialogRecordId') as HTMLInputElement)
            ?.value;
        let url = dialogRecordId
            ? (document.querySelector('#BaseUrl') as HTMLInputElement)?.value + dialogRecordId + '/_action_'
            : document.querySelector('.main-form')?.getAttribute('action');
        url = url?.replace('_action_', `${uploadController}/uploadimage`);

        // formData
        const formData = new FormData();
        const controllerID = this.controller?.getAttribute('id');
        if (controllerID) formData.append('ControlId', controllerID);
        if (blob) formData.append('file', blob);

        if (!url) return;

        // 画像のアップロード
        window.$p.multiUpload(url, formData, undefined, undefined, (json: string) => {
            const jsonData = JSON.parse(json);
            interface JsonDataEntry {
                Method: string;
                Target: string;
                Value?: string;
                [key: string]: unknown;
            }
            const obj = (jsonData as JsonDataEntry[]).find(
                entry => entry.Method === 'InsertText' && entry.Target === `#${controllerID}`
            );
            if (obj && obj.Value && this.sunEditor) {
                const match = obj.Value.match(/!\[.*?\]\((.*?)\)/);
                if (match && match[1]) {
                    if (blob && uploadHandler) {
                        uploadHandler({
                            result: [
                                {
                                    url: match[1],
                                    name: blob.name,
                                    size: blob.size
                                }
                            ]
                        });
                    } else {
                        this.sunEditor.insertHTML(`<p><img src="${match[1]}"></p>`);
                    }
                }
            }
        });
    };

    private createToggleReadonlyPlugin() {
        return {
            name: 'toggleReadonly',
            display: 'command',
            title: $p.display('EditModeToggle'),
            innerHTML: `${$p.display('Edit')} <div class="toggle">ON</div>`,
            isDisabled: false,
            core: undefined as Core | undefined,
            btn: undefined as HTMLButtonElement | undefined,
            buttonClass: 'btn-editable-cmd',
            add(core: Core, targetElement: HTMLButtonElement) {
                this.core = core;
                this.btn = targetElement;
                this.isDisabled = false;
                this.btn.addEventListener('click', this.action.bind(this));
            },
            action() {
                if (!this.core || !this.btn) return;
                const toggleBtn: HTMLElement | null = this.btn.querySelector('.toggle');
                if (this.core.context.element.toolbar && toggleBtn) {
                    const undoBtn = this.core.context.element.toolbar.querySelector('.se-btn[data-command="undo"]');
                    const redoBtn = this.core.context.element.toolbar.querySelector('.se-btn[data-command="redo"]');
                    if (this.isDisabled) {
                        this.core.context.element.toolbar
                            .querySelectorAll('button:not(.btn-editable-cmd)')
                            .forEach(btn => {
                                (btn as HTMLButtonElement).disabled = false;
                            });
                        this.core.context.element.topArea.classList.remove('is-readonly');
                        if (toggleBtn) {
                            toggleBtn.innerText = 'ON';
                            toggleBtn.removeAttribute('lock');
                        }
                        this.core.context.element.editorArea.setAttribute('spellcheck', 'true');
                        this.core.context.element.wysiwyg.setAttribute('contenteditable', 'true');

                        // 履歴確認してボタン状態を再設定
                        if (this.core.history.getCurrentIndex() === 0) {
                            undoBtn?.setAttribute('disabled', 'true');
                        } else {
                            undoBtn?.removeAttribute('disabled');
                        }
                        if (this.core.history.stack.length - 1 === this.core.history.getCurrentIndex()) {
                            redoBtn?.setAttribute('disabled', 'true');
                        } else {
                            redoBtn?.removeAttribute('disabled');
                        }
                    } else {
                        this.core.context.element.toolbar
                            .querySelectorAll('button:not(.btn-editable-cmd)')
                            .forEach(btn => {
                                (btn as HTMLButtonElement).disabled = true;
                            });
                        this.core.context.element.topArea.classList.add('is-readonly');
                        if (toggleBtn) toggleBtn.innerText = 'OFF';
                        toggleBtn.setAttribute('lock', '');
                        this.btn.removeAttribute('disabled');
                        this.core.context.element.editorArea.setAttribute('spellcheck', 'false');
                        this.core.context.element.wysiwyg.setAttribute('contenteditable', 'false');
                    }
                }
                this.isDisabled = !this.isDisabled;
            }
        };
    }

    imageViewerHandle = (event: Event) => {
        const path = event.composedPath();
        if ((path[0] as HTMLElement).tagName === 'IMG') {
            const imgNode = path[0] as HTMLImageElement;
            if (this.sunEditor || this.isReadOnly) {
                if (RichTextEditorElement.imageViewerModal) {
                    const wrap = !this.isReadOnly ? this.sunEditor!.core.context.element.wysiwyg : this.editorContainer;
                    const imgNodes = wrap.querySelectorAll('figure img');
                    // eslint-disable-next-line @typescript-eslint/no-explicit-any
                    (RichTextEditorElement.imageViewerModal as any).show(imgNode, imgNodes);
                } else {
                    window.open(imgNode.src);
                }
            }
        }
    };

    private initStyle() {
        if (!document.querySelector('#rteCustomCss') || this.isSmartdesign) {
            const style = document.createElement('style');
            style.setAttribute('id', 'rteCustomCss');
            style.textContent = css;
            style.textContent += suneditorCssRaw;
            if (!this.isSmartdesign) {
                document.querySelector('head')!.appendChild(style);
            } else {
                this.appendChild(style);
            }
        }
    }

    private htmlParser(htmlString: string) {
        const parser = new DOMParser();
        const doc = parser.parseFromString(htmlString, 'text/html');
        const element = doc.body.firstChild as HTMLDivElement;
        return element;
    }

    disconnectedCallback() {
        if (this.sunEditor) {
            this.sunEditor.destroy();
            this.sunEditor = undefined;
        }
        this.safariObserver?.disconnect();
    }

    private smartDesignSetting() {
        if (!this.isSmartdesign) return;
        if (this.isSmartdesign && this.controller) {
            const observer = new MutationObserver(mutationsList => {
                for (const mutation of mutationsList) {
                    if (mutation.type === 'attributes' && mutation.attributeName === 'placeholder') {
                        // placeholderが変わったときの処理
                        this.sunEditor?.setOptions({
                            placeholder: this.controller?.placeholder
                        });
                    }
                }
            });
            observer.observe(this.controller, {
                attributes: true,
                attributeFilter: ['placeholder']
            });
        }
    }

    private smartDesignValueBind(newValue: string) {
        if (!this.isSmartdesign) return;
        this.dispatchEvent(
            new CustomEvent('demochange', {
                detail: { value: newValue },
                bubbles: true,
                composed: true
            })
        );
    }

    get value(): string {
        return this.sunEditor ? this.sunEditor.getContents(true) : '';
    }
    set value(val: string) {
        if (this.sunEditor) {
            this.sunEditor.setContents(val);
        } else if (this.viewerContainer && this.isSmartdesign) {
            val = val || `<p><br></p>`;
            this.viewerContainer.innerHTML = DOMPurify.sanitize(val);
        }
    }
}

customElements.define('rt-editor', RichTextEditorElement);
