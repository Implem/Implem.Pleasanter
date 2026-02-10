import { Marked } from 'marked';
import DOMPurify from 'dompurify';
import hljs from 'highlight.js';
import $ from 'jquery';

import type { Tokens, Token } from 'marked';

import { ImageViewerModal } from '../../generals/modal/imageViewerModal';
import styleCode from './markdownField.scss?inline';
import highlightStyle from 'highlight.js/styles/github-dark.css?inline';

type ViewerType = 'auto' | 'manual' | 'disabled';

class MarkdownFieldElement extends HTMLElement {
    static ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/gif', 'image/webp', 'image/bmp'];
    static AnchorTargetBlank?: boolean;
    static isStyleAppended: boolean = false;
    static imageViewerModal: HTMLElement;
    private isEditable: boolean = false;
    private isDisabled: boolean = false;
    private isReadonly: boolean = false;
    private isComment: boolean = false;
    private isImageUsable: boolean = false;
    private isCameraDisabled: boolean = false;
    private switchTimeId?: number | NodeJS.Timeout;
    private ColumnId: string;
    private controller: HTMLTextAreaElement;
    private viewerElem: HTMLDivElement | null = null;
    private editorBtnElem: HTMLButtonElement | null = null;
    private viewerBtnElem: HTMLButtonElement | null = null;
    private filesBtnElem: HTMLInputElement | null = null;
    private photoBtnElem: HTMLButtonElement | null = null;
    private viewerType: ViewerType = 'auto';
    private uncPathRegex: RegExp =
        /\B\\\\[^\\/:*?"<>|\r\n\s]+\\[^\\/:*?"<>|\r\n\s]+(?:\\[^\\/:*?"<>|\r\n()\s]*(?:\([^\\/:*?"<>|\r\n()]*\)[^\\/:*?"<>|\r\n()\s]*)?)*[^\\/:*?"<>|\r\n()[\]\s]/gi;
    private encodedUncPathRegex: RegExp =
        /(?<!data-href=")%5C%5C(?:[A-Za-z0-9\-_.!'()]+|%(?:[0-9A-Fa-f]{2}))+(?:%5C(?:[A-Za-z0-9\-_.!'()]+|%(?:[0-9A-Fa-f]{2}))+)+/gi;
    private notesLinkRegex: RegExp = /\bnotes:\/\/[^\s<()>[\]"]+/gi;
    private encodedNotesLinkRegex: RegExp = /notes%3A%2F%2F[\w%\-.!~*'()%]+/gi;

    constructor() {
        super();
        this.initStyle();
        this.initHtml();
        this.controller = this.querySelector('textarea')!;
        this.ColumnId = this.controller.id;
    }

    connectedCallback() {
        this.isDisabled = Boolean(this.controller.disabled);
        this.isReadonly = this.isDisabled || Boolean(this.controller.dataset.readonly);
        this.isComment = this.controller.hasAttribute('data-comment');
        this.isCameraDisabled = this.controller.hasAttribute('data-camera-disabled');
        this.isImageUsable = this.controller.classList.contains('upload-image');
        if (this.controller.dataset.enablelightbox === '1' && !MarkdownFieldElement.imageViewerModal) {
            MarkdownFieldElement.imageViewerModal = document.createElement('image-viewer-modal');
            document.body.appendChild(MarkdownFieldElement.imageViewerModal);
        }
        this.initModule();
        this.addEvents(this.isReadonly);
    }

    private initModule() {
        this.controller.classList.add('md-editor');
        this.viewerType = (this.controller.dataset.viewerType as ViewerType) || 'auto';
        this.viewerElem = this.querySelector('.md-viewer');
        if (MarkdownFieldElement.AnchorTargetBlank === undefined) {
            MarkdownFieldElement.AnchorTargetBlank = document.querySelector('#AnchorTargetBlank') ? true : false;
        }
        if (this.isComment) this.classList.add('is-comment');
        if (!this.isReadonly) {
            this.editorBtnElem = this.querySelector('.md-btn.is-editor');
            this.viewerBtnElem = this.querySelector('.md-btn.is-viewer');
            this.filesBtnElem = this.querySelector('.md-btn.is-files input');
            this.photoBtnElem = this.querySelector('.md-btn.is-photo');
            switch (this.viewerType) {
                case 'auto':
                    this.viewerBtnElem?.remove();
                    break;
                case 'disabled':
                    this.viewerElem?.remove();
                    this.viewerBtnElem?.remove();
                    this.editorBtnElem?.remove();
                    this.showEditor();
                    this.viewerMarked = undefined;
                    break;
            }
            if (!this.isImageUsable || this.viewerType === 'disabled') {
                this.filesBtnElem?.remove();
                this.photoBtnElem?.remove();
                this.querySelector('.bottom-tools')?.remove();
            }
            if (this.isCameraDisabled) {
                this.photoBtnElem?.remove();
            }
            if (this.viewerType === 'disabled') return;
            if (this.controller.value || this.isComment) {
                this.showViewer();
            } else {
                this.showEditor();
            }
        } else {
            this.querySelector('.md-btn.is-editor')?.remove();
            this.querySelector('.md-btn.is-viewer')?.remove();
            this.querySelector('.bottom-tools')?.remove();
            this.viewerElem?.classList.add('control-text', 'not-send');
            this.controller.disabled = true;
            if (this.isDisabled) {
                this.classList.add('is-disabled');
                this.controller.remove();
            }
            this.showViewer();
        }
    }

    private addEvents(isReadonly: boolean = false) {
        if (!isReadonly) {
            switch (this.viewerType) {
                case 'auto':
                    this.controller.addEventListener('focusout', this.onEditorFocusout);
                    break;
                case 'manual':
                    this.viewerBtnElem?.addEventListener('click', this.onViewerBtnClick);
                    break;
            }
            if (this.viewerType !== 'disabled') {
                this.viewerElem?.addEventListener('dblclick', this.onViewerDoubleClick);
                this.editorBtnElem?.addEventListener('click', this.onEditorBtnClick);
            }
            this.controller.addEventListener('input', this.onEditorInput);
            this.controller.addEventListener('change', this.onEditorChange);
        }
        if (this.viewerType !== 'disabled') {
            if (this.isImageUsable) {
                this.controller.addEventListener('paste', this.onEditorPaste);
                this.filesBtnElem?.addEventListener('click', this.onFilesClick);
                this.filesBtnElem?.addEventListener('change', this.onFilesUpdate);
                this.photoBtnElem?.addEventListener('click', this.onPhotoBtnClick);
            }
            this.viewerElem?.addEventListener('click', this.onViewerClick);
        }
    }

    public showEditor(autoFocus?: boolean) {
        this.setAttribute('data-editable', '');
        this.isEditable = true;
        this.setEditorHeight();
        if (autoFocus) this.controller.focus({ preventScroll: true });
    }

    private viewerMarked? = new Marked({
        gfm: true,
        breaks: true,
        renderer: {
            html: token => this.escapeHtml(token.text),
            link: token => this.mdRenderLink(token),
            image: token => this.mdRenderImage(token),
            code: token => this.mdRenderCode(token)
        }
    });

    private mdRenderLink = (token: Tokens.Link) => {
        const blank = MarkdownFieldElement.AnchorTargetBlank ? ' target="_blank"' : undefined;
        this.encodedUncPathRegex.lastIndex = 0;
        this.encodedNotesLinkRegex.lastIndex = 0;
        if (this.encodedUncPathRegex.test(token.href)) {
            return `<a data-href="file://${decodeURIComponent(token.href)}"${blank}>${this.escapeHtml(decodeURIComponent(token.text))}</a>`;
        } else if (this.encodedNotesLinkRegex.test(token.href)) {
            return `<a data-href="${decodeURIComponent(token.href)}"${blank}>${this.escapeHtml(decodeURIComponent(token.text))}</a>`;
        } else {
            return `<a href="${token.href}"${blank}>${this.escapeHtml(decodeURIComponent(token.text))}</a>`;
        }
    };

    private mdRenderImage = (token: Tokens.Image) => {
        return /*html */ `<figure><img src="${encodeURI(token.href)}?thumbnail=1" alt="${this.escapeHtml(token.text)}"${token.title ? ` title="${this.escapeHtml(token.title)}"` : ''}></figure>`;
    };

    private mdRenderCode = (token: Tokens.Code) => {
        const lang = (token.lang || '').trim();
        let highlighted: string;
        if (lang && hljs.getLanguage(lang)) {
            highlighted = hljs.highlight(token.text, { language: lang }).value;
        } else {
            highlighted = hljs.highlightAuto(token.text).value;
        }
        return `<div class="md-code-block">
                    <button class="md-code-copy">
                        <span class="md-btn-icon material-symbols-outlined">content_copy</span>
                        <pre class="md-code-copy-item" name="copy_data">${this.escapeHtml(token.text)}</pre>
                    </button>
                    <div class="md-code-copied">Copied!</div>
                    <pre><code class="hljs ${lang ? `language-${lang}` : ''}">${highlighted}</code></pre>
                </div>`;
    };

    private notesRender = (token: Token): string => {
        switch (token.type) {
            case 'link': {
                return this.mdRenderLink(token as Tokens.Link);
            }
            case 'image': {
                return this.mdRenderImage(token as Tokens.Image);
            }
            case 'text':
            case 'paragraph':
                if ('tokens' in token && token.tokens?.length) {
                    return token.tokens.map(_token => this.notesRender(_token)).join('');
                } else {
                    return token.raw;
                }
            case 'space':
            case 'br':
                return token.raw;
            case 'escape':
                return token.text;
            default:
                return token.raw;
        }
    };

    private escapeHtml(str: string): string {
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&apos;');
    }

    private escapeMarkdown(str: string): string {
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&apos;')
            .replace(/#/g, '&#35;')
            .replace(/\*/g, '&#42;')
            .replace(/_/g, '&#95;')
            .replace(/~/g, '&#126;')
            .replace(/- /g, '-&nbsp;')
            .replace(/\+ /g, '+&nbsp;')
            .replace(/\* /g, '&#42;&nbsp;')
            .replace(/\[ \]/g, '&#91;&nbsp;&#93;')
            .replace(/\[x\]/gi, '&#91;x&#93;')
            .replace(/\d+\. /g, m => m.replace(/\. /, '&#46;&nbsp;'))
            .replace(/\\/g, '\\\\')
            .replace(/`/g, '&#96;')
            .replace(/\|/g, '&#124;')
            .replace(/---/g, '&#45;&#45;&#45;');
    }

    private decodeHtmlEntities(input: string) {
        return input
            .replace(/&#(\d+);/g, (_, dec) => String.fromCharCode(Number(dec)))
            .replace(/&#x([0-9A-Fa-f]+);/g, (_, hex) => String.fromCharCode(parseInt(hex, 16)))
            .replace(/&apos;/g, "'");
    }

    private encodeCustomSchemeLink(md: string) {
        this.uncPathRegex.lastIndex = 0;
        md = md.replace(this.uncPathRegex, url => {
            return `${encodeURIComponent(url)}`;
        });
        this.notesLinkRegex.lastIndex = 0;
        md = md.replace(this.notesLinkRegex, url => {
            return `${encodeURIComponent(url)}`;
        });
        return md;
    }

    private createCustomSchemeLink(md: string) {
        md = this.decodeHtmlEntities(md);
        this.encodedUncPathRegex.lastIndex = 0;
        md = md.replace(this.encodedUncPathRegex, url => {
            return this.mdRenderLink({
                href: url,
                text: url
            } as Tokens.Link);
        });
        this.encodedNotesLinkRegex.lastIndex = 0;
        md = md.replace(this.encodedNotesLinkRegex, url => {
            return this.mdRenderLink({
                href: url,
                text: url
            } as Tokens.Link);
        });
        return md;
    }

    private finalizeViewerDom() {
        this.viewerElem!.querySelectorAll('a[data-href]').forEach(a => {
            const element = a as HTMLAnchorElement;
            element.href = element.dataset.href!;
            element.removeAttribute('data-href');
        });

        this.viewerElem!.querySelectorAll('pre code a').forEach(a => {
            const textNode = document.createTextNode(a.textContent || '');
            a.replaceWith(textNode);
        });
    }

    public showViewer() {
        if (this.controller.value || this.isReadonly || this.isComment) {
            let md = this.controller.value;
            md = this.encodeCustomSchemeLink(md);
            if (md.indexOf('[md]') === 0) {
                md = md.split('\n').slice(1).join('\n');
                md = String(this.viewerMarked!.parse(md));
            } else {
                const tokens = this.viewerMarked?.lexer(this.escapeMarkdown(md));
                md = tokens!.map(token => this.notesRender(token)).join('');
                md = `<div class="notes">${md}<br></div>`;
            }
            md = this.createCustomSchemeLink(md);
            md = md.replace(/&amp;#(\d+);/g, '&#$1;');
            md = DOMPurify.sanitize(md, {
                ADD_ATTR: ['target']
            });
            this.viewerElem!.innerHTML = md;
            this.finalizeViewerDom();
            this.removeAttribute('data-editable');
            this.isEditable = false;
        } else if (!this.controller.value) {
            this.viewerElem!.innerHTML = '';
            if (!this.isReadonly && !this.isComment) {
                this.showEditor();
            }
        }
    }

    private setEditorHeight = () => {
        const BORDER_ADJUSTMENT = 2;
        const dummy = document.createElement('div');
        const ctrlStyle = window.getComputedStyle(this.controller);
        dummy.style.visibility = 'hidden';
        dummy.style.position = 'absolute';
        dummy.style.whiteSpace = 'pre-wrap';
        dummy.style.overflowWrap = 'break-word';
        dummy.style.width = `${this.controller.clientWidth}px`;
        dummy.style.font = ctrlStyle.font;
        dummy.style.lineHeight = ctrlStyle.lineHeight;
        dummy.style.padding = ctrlStyle.padding;
        dummy.style.boxSizing = ctrlStyle.boxSizing;
        dummy.textContent = this.controller.value;
        document.body.appendChild(dummy);
        const heightValue = dummy.scrollHeight + BORDER_ADJUSTMENT;
        document.body.removeChild(dummy);
        if (heightValue) this.style.setProperty('--component-height', `${heightValue + BORDER_ADJUSTMENT}px`);
    };

    private getSelectedFileBinary(event: Event) {
        const input = event.target as HTMLInputElement;
        if (input.files && input.files.length > 0) {
            this.uploadBinary(input.files[0]);
            input.value = '';
        }
    }

    private getClipboardBinary(event: ClipboardEvent) {
        if (event.clipboardData && event.clipboardData.types.indexOf('text/plain') === -1) {
            const items = event.clipboardData.items;
            Array.from(items).forEach(item => {
                const blob = item.getAsFile();
                if (blob && item.type.indexOf('image') !== -1) {
                    this.uploadBinary(blob);
                }
            });
        }
    }

    private uploadBinary(blob: File) {
        if (!blob || !MarkdownFieldElement.ALLOWED_IMAGE_TYPES.includes(blob.type)) {
            return;
        }
        const dir = $p.isForm() ? 'formbinaries' : 'binaries';
        const uploadController = `${dir}/uploadimage`;
        const trElem = document.getElementById(this.ColumnId)?.closest('tr');
        const dialogIdField: HTMLInputElement | null = document.querySelector('#EditorInDialogRecordId');
        let url: string = (document.querySelector('#BaseUrl') as HTMLInputElement).value;
        if (trElem) {
            url += trElem.dataset.id + '/_action_';
        } else if (dialogIdField) {
            url += dialogIdField.value + '/_action_';
        } else {
            url = document.querySelector('.main-form')!.getAttribute('action')!;
        }
        url = url.replace('_action_', `${uploadController}`);

        const formData = new FormData();
        formData.append('ControlId', this.ColumnId);
        if (blob) formData.append('file', blob);

        $p.multiUpload(url, formData, undefined, undefined, (json: string) => {
            const jsonData = JSON.parse(json);
            interface JsonDataEntry {
                Method: string;
                Target: string;
                Value?: string;
                [key: string]: unknown;
            }
            const obj = (jsonData as JsonDataEntry[]).find(
                entry => entry.Method === 'InsertText' && entry.Target === `#${this.ColumnId}`
            );
            const insertValue = obj ? obj.Value : undefined;
            if (insertValue) {
                const start = this.controller.value;
                const caret = this.controller.selectionStart ?? 0;
                const next = caret + (insertValue as string).length;
                this.controller.value = start.slice(0, caret) + insertValue + start.slice(caret);
                this.controller.setSelectionRange(next, next);
                this.onEditorChange();
                if (this.controller.offsetParent !== null) {
                    this.controller.focus();
                }
            }
        });
    }

    private openImageViewer = (event: Event) => {
        const path = event.composedPath();
        if ((path[0] as HTMLElement).tagName === 'IMG') {
            const imgNode = path[0] as HTMLImageElement;
            const wrap = this.closest('tr') ?? document;
            if (wrap !== document) event.stopPropagation();
            if (MarkdownFieldElement.imageViewerModal) {
                event.preventDefault();
                const imgNodes: NodeListOf<HTMLImageElement> = wrap.querySelectorAll('markdown-field figure img');
                (MarkdownFieldElement.imageViewerModal as ImageViewerModal).show(imgNode, imgNodes);
            } else {
                const imgSrc = imgNode.src.split('?thumbnail')[0];
                window.open(imgSrc);
            }
        }
    };

    private copyCodeBlock = (event: Event) => {
        const path = event.composedPath();
        if ((path[0] as HTMLElement).classList.contains('md-code-copy')) {
            event.stopPropagation();
            const buttonNode = path[0] as HTMLImageElement;
            const wrap = buttonNode.closest('.md-code-block');
            const code = decodeURIComponent(buttonNode.querySelector('.md-code-copy-item')?.textContent || '');
            navigator.clipboard.writeText(code);
            wrap?.classList.add('is-copied');
            setTimeout(() => wrap?.classList.remove('is-copied'), 1500);
        }
    };

    private onEditorFocusout = () => {
        this.switchTimeId = setTimeout(() => {
            this.showViewer();
        }, 150);
    };

    private onEditorPaste = (event: ClipboardEvent) => {
        this.getClipboardBinary(event);
    };

    private onEditorBtnClick = () => {
        this.showEditor(true);
    };

    private onViewerBtnClick = () => {
        this.showViewer();
    };

    private onFilesClick = () => {
        clearTimeout(this.switchTimeId);
    };

    private onFilesUpdate = (event: Event) => {
        this.getSelectedFileBinary(event);
    };

    private onPhotoBtnClick = () => {
        clearTimeout(this.switchTimeId);
        $p.openVideo(this.ColumnId);
    };

    private onViewerClick = (event: Event) => {
        this.openImageViewer(event);
        this.copyCodeBlock(event);
    };

    private onViewerDoubleClick = () => {
        this.showEditor(true);
    };

    private onEditorChange = () => {
        $p.set($(this.controller), this.controller.value);
        if (!this.isEditable) this.showViewer();
    };

    private onEditorInput = () => {
        this.setEditorHeight();
    };

    private initHtml() {
        this.innerHTML += /* html*/ `
            <div class="md-viewer md"></div>
            <button type="button" class="md-btn is-editor">
                <span class="md-btn-icon material-symbols-outlined">edit</span>
            </button>
            <button type="button" class="md-btn is-viewer">
                <span class="md-btn-icon material-symbols-outlined">edit_off</span>
            </button>
            <ul class="bottom-tools">
                <li>
                    <label type="button" class="md-btn is-files">
                        <span class="md-btn-icon material-symbols-outlined">imagesmode</span>
                        <input type="file" accept="${MarkdownFieldElement.ALLOWED_IMAGE_TYPES.join(',')}">
                    </label>
                </li>
                <li>
                    <button type="button" class="md-btn is-photo">
                        <span class="md-btn-icon material-symbols-outlined">add_a_photo</span>
                    </button>
                </li>
            </ul>
        `;
    }

    private initStyle() {
        if (MarkdownFieldElement.isStyleAppended) return;
        const style = document.createElement('style');
        style.textContent = styleCode + highlightStyle;
        document.head.appendChild(style);
        MarkdownFieldElement.isStyleAppended = true;
    }

    disconnectedCallback() {
        this.controller.removeEventListener('focusout', this.onEditorFocusout);
        this.controller.removeEventListener('input', this.onEditorInput);
        this.controller.removeEventListener('paste', this.onEditorPaste);
        this.controller.removeEventListener('change', this.onEditorChange);
        this.viewerElem?.removeEventListener('click', this.onViewerClick);
        this.viewerElem?.removeEventListener('dblclick', this.onViewerDoubleClick);
        this.editorBtnElem?.removeEventListener('click', this.onEditorBtnClick);
        this.viewerBtnElem?.removeEventListener('click', this.onViewerBtnClick);
        this.filesBtnElem?.removeEventListener('click', this.onFilesClick);
        this.filesBtnElem?.removeEventListener('change', this.onFilesUpdate);
        this.photoBtnElem?.removeEventListener('click', this.onPhotoBtnClick);
        clearTimeout(this.switchTimeId);
    }
}

customElements.define('markdown-field', MarkdownFieldElement);
