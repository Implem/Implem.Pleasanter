import { EditorView, basicSetup } from 'codemirror';
import { keymap } from '@codemirror/view';
import { EditorState } from '@codemirror/state';
import { indentUnit } from '@codemirror/language';
import { autocompletion, acceptCompletion } from '@codemirror/autocomplete';
import { indentWithTab } from '@codemirror/commands';
import { html } from '@codemirror/lang-html';
import { css } from '@codemirror/lang-css';
import { javascript } from '@codemirror/lang-javascript';
import { json } from '@codemirror/lang-json';
import { oneDark } from '@codemirror/theme-one-dark';
import $ from 'jquery';
import type { LanguageSupport } from '@codemirror/language';

import customStyle from './codeEditor.scss?inline';

class CodeEditorElement extends HTMLElement {
    private shadow: ShadowRoot;
    private editorWrapElem: HTMLDivElement | null = null;
    private editor?: EditorView;
    private controller?: HTMLTextAreaElement | null;
    private langs: Record<string, LanguageSupport> = {
        html: html(),
        css: css(),
        javascript: javascript(),
        json: json()
    };

    constructor() {
        super();
        this.shadow = this.attachShadow({ mode: 'open' });
        this.initHtml();
        this.initStyle();
    }

    connectedCallback() {
        this.editorWrapElem = this.shadow.querySelector('.editor-container');
        this.controller = this.querySelector('textarea');
        if (!this.controller || !this.editorWrapElem) return;
        this.optionStyle();
        this.editorInit();
    }

    private editorInit() {
        if (!this.controller || !this.editorWrapElem) return;
        const value = this.controller.value;
        const getLang = this.controller.dataset.lang || 'javascript';
        const Keymap = keymap.of([
            {
                key: 'Tab',
                run: view => {
                    return (acceptCompletion(view) || indentWithTab.run?.(view)) ?? false;
                }
            },
            {
                key: 'Shift-Tab',
                run: view => indentWithTab.shift?.(view) ?? false
            }
        ]);

        this.editor = new EditorView({
            doc: value,
            extensions: [
                basicSetup,
                autocompletion(),
                this.langs[getLang] ?? javascript(),
                oneDark,
                Keymap,
                this.onChange,
                indentUnit.of('  '),
                EditorState.tabSize.of(2)
            ],
            parent: this.editorWrapElem
        });
    }

    private onChange = EditorView.updateListener.of(update => {
        if (!this.controller || !this.editorWrapElem) return;
        if (update.docChanged) {
            this.controller.value = update.state.doc.toString();
            $p.set($(this.controller), this.controller.value);
        }
    });

    private initHtml() {
        this.shadow.innerHTML = /* html*/ `
            <div>
                <div class="editor-container"></div>
                <div class="control-container">
                    <slot></slot>
                </div>
            </div>
        `;
    }

    private initStyle() {
        const style = document.createElement('style');
        style.textContent = customStyle;
        this.shadow.appendChild(style);
    }

    private optionStyle() {
        if (this.controller && this.controller.classList.contains('o-low')) {
            this.classList.add('o-low');
        }
    }

    disconnectedCallback() {
        if (this.editor) {
            this.editor.destroy();
            this.editor = undefined;
        }
    }
}

customElements.define('code-editor', CodeEditorElement);
