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
    private static readonly PLACEHOLDER_BASE = 'http://ph.invalid/';
    private static readonly PLACEHOLDER_BASE_ESCAPED = MarkdownFieldElement.PLACEHOLDER_BASE.replace(
        /[.*+?^${}()|[\]\\]/g,
        '\\$&'
    );
    private static readonly PLACEHOLDER_REGEX = new RegExp(
        `<a\\s+href="${MarkdownFieldElement.PLACEHOLDER_BASE_ESCAPED}(\\d+)/"([^>]*)>([\\s\\S]*?)</a>`,
        'gi'
    );
    private static readonly URL_REGEX: RegExp = /(?:(?:https?|ftp):\/\/|www\.)(?:[a-zA-Z0-9-]+\.?)+[^\s<]*/gi;
    private static readonly EMAIL_REGEX: RegExp =
        /[A-Za-z0-9._+-]+@[a-zA-Z0-9_-]+(?:\.[a-zA-Z0-9_-]*[a-zA-Z0-9])+(?![_-])/g;
    private customSchemeLinks: Array<{ url: string; displayText?: string }> = [];
    private uncPathRegex: RegExp =
        /\B\\\\[^\\/:*?"<>|\r\n\s]+\\[^\\/:*?"<>|\r\n\s]+(?:\\[^\\/:*?"<>|\r\n()\s]*(?:\([^\\/:*?"<>|\r\n()]*\)[^\\/:*?"<>|\r\n()\s]*)?)*[^\\/:*?"<>|\r\n()[\]\s]/gi;
    private mdLinkCustomSchemeRegex: RegExp =
        /(\[(?:[^\\[\]]|\\.|![^\]]*\]\([^)]*\))*\]\()((?:\\\\|notes:\/\/)[^()\s]*(?:\([^()]*\)[^()\s]*)*)(\s+(?:"[^"]*"|'[^']*'))?(\))/gi;
    private notesLinkRegex: RegExp = /\bnotes:\/\/[^\s<()>[\]"]+/gi;

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
            if (!this.isImageUsable) {
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
            this.viewerElem?.addEventListener('click', this.onViewerClick);
        }
        if (this.isImageUsable) {
            this.controller.addEventListener('paste', this.onEditorPaste);
            this.filesBtnElem?.addEventListener('click', this.onFilesClick);
            this.filesBtnElem?.addEventListener('change', this.onFilesUpdate);
            this.photoBtnElem?.addEventListener('click', this.onPhotoBtnClick);
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

    private renderLinkText = (token: Tokens.Link): string => {
        if ('tokens' in token && token.tokens && token.tokens.length > 0) {
            return token.tokens.map(t => this.renderInlineToken(t)).join('');
        }
        return this.escapeHtml(decodeURIComponent(token.text));
    };

    private renderInlineToken = (token: Token): string => {
        switch (token.type) {
            case 'link': {
                return this.mdRenderLink(token as Tokens.Link);
            }
            case 'image': {
                const imgToken = token as Tokens.Image;
                const href = imgToken.href.toLowerCase();
                if (href.startsWith('javascript:') || href.startsWith('data:')) {
                    return `<span class="invalid-image" title="Invalid image URL">${this.escapeHtml(imgToken.text)}</span>`;
                }
                return `<img src="${this.escapeHtml(encodeURI(imgToken.href))}?thumbnail=1" alt="${this.escapeHtml(imgToken.text)}"${imgToken.title ? ` title="${this.escapeHtml(imgToken.title)}"` : ''}>`;
            }
            case 'strong':
                if ('tokens' in token && token.tokens) {
                    return `<strong>${token.tokens.map(t => this.renderInlineToken(t)).join('')}</strong>`;
                }
                return `<strong>${this.escapeHtml(token.text)}</strong>`;
            case 'em':
                if ('tokens' in token && token.tokens) {
                    return `<em>${token.tokens.map(t => this.renderInlineToken(t)).join('')}</em>`;
                }
                return `<em>${this.escapeHtml(token.text)}</em>`;
            case 'del':
                if ('tokens' in token && token.tokens) {
                    return `<del>${token.tokens.map(t => this.renderInlineToken(t)).join('')}</del>`;
                }
                return `<del>${this.escapeHtml(token.text)}</del>`;
            case 'codespan':
                return `<code>${this.escapeHtml(this.restorePlaceholdersInText(token.text))}</code>`;
            case 'text':
                return this.escapeHtml(token.text);
            case 'escape':
                return this.escapeHtml(token.text);
            case 'br':
                return '<br>';
            case 'html':
                return this.escapeHtml(token.raw);
            default:
                if ('text' in token) {
                    return this.escapeHtml(token.text || '');
                }
                return this.escapeHtml(token.raw || '');
        }
    };

    private mdRenderLink = (token: Tokens.Link) => {
        const title = token.title ? ` title="${this.escapeHtml(token.title)}"` : '';
        const linkText = this.renderLinkText(token);
        return `<a href="${this.escapeHtml(token.href)}"${title}>${linkText}</a>`;
    };

    private mdRenderImage = (token: Tokens.Image) => {
        const href = token.href.toLowerCase();
        if (href.startsWith('javascript:') || href.startsWith('data:')) {
            return `<figure><span class="invalid-image" title="Invalid image URL">${this.escapeHtml(token.text)}</span></figure>`;
        }
        return /*html */ `<figure><img src="${this.escapeHtml(encodeURI(token.href))}?thumbnail=1" alt="${this.escapeHtml(token.text)}"${token.title ? ` title="${this.escapeHtml(token.title)}"` : ''}></figure>`;
    };

    private mdRenderCode = (token: Tokens.Code) => {
        const lang = (token.lang || '').trim();
        const text = this.restorePlaceholdersInText(token.text);
        let highlighted: string;
        if (lang && hljs.getLanguage(lang)) {
            highlighted = hljs.highlight(text, { language: lang }).value;
        } else {
            highlighted = hljs.highlightAuto(text).value;
        }
        return `<div class="md-code-block">
                    <button class="md-code-copy">
                        <span class="md-btn-icon material-symbols-outlined">content_copy</span>
                        <pre class="md-code-copy-item" name="copy_data">${this.escapeHtml(text)}</pre>
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

    private restorePlaceholdersInText(text: string): string {
        return text.replace(
            new RegExp(`${MarkdownFieldElement.PLACEHOLDER_BASE_ESCAPED}(\\d+)/`, 'g'),
            (match, indexStr) => {
                const entry = this.customSchemeLinks[parseInt(indexStr, 10)];
                return entry !== undefined ? entry.url : match;
            }
        );
    }

    private escapeMarkdown(str: string): string {
        const autoLinks: string[] = [];
        str = str.replace(/\uE001/g, '\uE001\uE001');
        str = str.replace(MarkdownFieldElement.URL_REGEX, match => {
            autoLinks.push(match);
            return `\uE001ESC${autoLinks.length - 1}\uE001`;
        });
        str = str.replace(MarkdownFieldElement.EMAIL_REGEX, match => {
            autoLinks.push(match);
            return `\uE001ESC${autoLinks.length - 1}\uE001`;
        });
        str = str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/#/g, '&#35;')
            .replace(/\*/g, '&#42;')
            .replace(/_/g, '&#95;')
            .replace(/~/g, '&#126;')
            .replace(/- /g, '-&nbsp;')
            .replace(/\+ /g, '+&nbsp;')
            .replace(/\* /g, '&#42;&nbsp;')
            .replace(/\[ \](?!\()/g, '&#91;&nbsp;&#93;')
            .replace(/\[x\](?!\()/gi, '&#91;x&#93;')
            .replace(/\d+\. /g, m => m.replace(/\. /, '&#46;&nbsp;'))
            .replace(/\\/g, '\\\\')
            .replace(/`/g, '&#96;')
            .replace(/\|/g, '&#124;')
            .replace(/---/g, '&#45;&#45;&#45;');
        str = str.replace(/\uE001ESC(\d+)\uE001/g, (_, i) => autoLinks[parseInt(i, 10)]);
        return str.replace(/\uE001\uE001/g, '\uE001');
    }

    private replaceWithPlaceholders(md: string): string {
        this.customSchemeLinks = [];
        const codeBlocks: string[] = [];
        md = md.replace(/\uE000/g, '\uE000\uE000');
        const protectCode = (match: string): string => {
            const idx = codeBlocks.length;
            codeBlocks.push(match);
            return `\uE000ESC${idx}\uE000`;
        };
        // インラインコード（複数バックティック・単体バックティック）を保護
        // URL 正規表現がバッククオートを消費してコードスパン構文を壊すのを防ぐ
        // フェンス・インデントコードブロックは mdRenderCode 内の restorePlaceholdersInText で復元する
        md = md.replace(/(`{2,})(?:(?!\n\n)[\s\S])+?\1|`[^`\n]+`/g, protectCode);
        const storePath = (url: string, displayText?: string): string => {
            const index = this.customSchemeLinks.length;
            this.customSchemeLinks.push({ url, displayText });
            return `${MarkdownFieldElement.PLACEHOLDER_BASE}${index}/`;
        };
        this.mdLinkCustomSchemeRegex.lastIndex = 0;
        md = md.replace(this.mdLinkCustomSchemeRegex, (_, prefix, url, title, suffix) => {
            const linkText = prefix.slice(1, -2);
            return `${prefix}${storePath(url, linkText)}${title || ''}${suffix}`;
        });
        this.uncPathRegex.lastIndex = 0;
        md = md.replace(this.uncPathRegex, url => storePath(url));
        this.notesLinkRegex.lastIndex = 0;
        md = md.replace(this.notesLinkRegex, url => storePath(url));
        md = md.replace(/\uE000ESC(\d+)\uE000/g, (match, i) => {
            const idx = parseInt(i, 10);
            return idx < codeBlocks.length ? codeBlocks[idx] : match;
        });
        return md.replace(/\uE000\uE000/g, '\uE000');
    }

    private restoreFromPlaceholders(html: string): string {
        MarkdownFieldElement.PLACEHOLDER_REGEX.lastIndex = 0;
        html = html.replace(MarkdownFieldElement.PLACEHOLDER_REGEX, (_, indexStr, attrs, linkText) => {
            const index = parseInt(indexStr, 10);
            const entry = this.customSchemeLinks[index];
            // entry が見つからない場合はリンクテキストのみを返し、プレースホルダー URL 露出を防ぐ
            if (entry === undefined) return linkText;
            const isUnc = entry.url.startsWith('\\\\');
            const href = isUnc ? `file://${entry.url}` : entry.url;
            // linkText がプレースホルダー URL のまま残っている場合は、平文 UNC/notes URL が
            // GFM autolink 化されたケース。entry.displayText（[text](\\url) 形式で
            // 保存した元テキスト）があればそれを、なければ元 URL を表示する。
            // それ以外は marked が正しくレンダリング済みの HTML をそのまま使用する。
            const displayText = linkText.includes(MarkdownFieldElement.PLACEHOLDER_BASE)
                ? this.escapeHtml(entry.displayText ?? entry.url)
                : linkText;
            return `<a href="${this.escapeHtml(href)}"${attrs}>${displayText}</a>`;
        });
        // HTML 内に残ったプレースホルダー URL を元の URL に復元する
        html = html.replace(
            new RegExp(`${MarkdownFieldElement.PLACEHOLDER_BASE_ESCAPED}(\\d+)/`, 'g'),
            (match, indexStr) => {
                const entry = this.customSchemeLinks[parseInt(indexStr, 10)];
                return entry !== undefined ? this.escapeHtml(entry.url) : match;
            }
        );
        return html;
    }

    private finalizeViewerDom() {
        this.viewerElem!.querySelectorAll('pre code a').forEach(a => {
            const textNode = document.createTextNode(a.textContent || '');
            a.replaceWith(textNode);
        });
        if (MarkdownFieldElement.AnchorTargetBlank) {
            this.viewerElem!.querySelectorAll('a').forEach(a => {
                const anchor = a as HTMLAnchorElement;
                anchor.target = '_blank';
                anchor.rel = 'noopener noreferrer';
            });
        }
    }

    public showViewer() {
        if (!this.viewerMarked || !this.viewerElem) return;
        if (this.controller.value || this.isReadonly || this.isComment) {
            let md = this.controller.value;
            md = this.replaceWithPlaceholders(md);
            if (md.indexOf('[md]') === 0) {
                md = md.slice(4);
                md = String(this.viewerMarked!.parse(md));
            } else {
                const tokens = this.viewerMarked?.lexer(this.escapeMarkdown(md));
                md = tokens!.map(token => this.notesRender(token)).join('');
                md = `<div class="notes">${md}<br></div>`;
            }
            md = DOMPurify.sanitize(md, {
                ADD_ATTR: ['title']
            });
            md = this.restoreFromPlaceholders(md);
            md = md.replace(/&amp;#(\d+);/g, '&#$1;');
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
        if (CSS.supports('field-sizing', 'content') || this.viewerType === 'disabled') return;

        const BORDER_ADJUSTMENT = 2;
        const dummy = document.createElement('textarea');
        const ctrlStyle = window.getComputedStyle(this.controller);
        dummy.style.visibility = 'hidden';
        dummy.style.position = 'absolute';
        dummy.style.width = `${this.controller.clientWidth}px`;
        dummy.style.font = ctrlStyle.font;
        dummy.style.lineHeight = ctrlStyle.lineHeight;
        dummy.style.padding = ctrlStyle.padding;
        dummy.style.boxSizing = ctrlStyle.boxSizing;
        dummy.textContent = this.controller.value;
        document.body.appendChild(dummy);
        const heightValue = dummy.scrollHeight + BORDER_ADJUSTMENT;
        document.body.removeChild(dummy);
        if (heightValue) this.style.setProperty('--component-height', `${heightValue}px`);
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
        if (!$p.validateImageUploadFileSize(blob)) {
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
            if ($p.handleMessageFromJson(jsonData)) {
                return;
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
        if ((path[0] as HTMLElement).tagName === 'IMG' && (path[1] as HTMLElement).tagName === 'FIGURE') {
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
            const rawText = buttonNode.querySelector('.md-code-copy-item')?.textContent || '';
            let code: string;
            try {
                code = decodeURIComponent(rawText);
            } catch {
                code = rawText;
            }
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
