import css from './ui-dialog.scss?inline';

declare global {
    interface Window {
        dialog?: Record<string, UiDialog>;
    }
}

class UiDialog extends HTMLElement {
    private shadow: ShadowRoot;
    private isOpen: boolean = false;
    private dialogElem: HTMLDialogElement | null = null;
    private outerClickTarget?: Element;
    private docScrollY: number = 0;
    static hasActiveDialogCount: number = 0;
    private transitionPromise: Promise<void> | null = null;

    constructor() {
        super();
        this.shadow = this.attachShadow({ mode: 'open' });
        this.render();
        this.initStyle();
    }

    connectedCallback() {
        this.dialogElem = this.shadow.querySelector('#dialog');
        const headerSlot = this.shadow.querySelector('slot[name=header]') as HTMLSlotElement;
        const footerSlot = this.shadow.querySelector('slot[name=footer]') as HTMLSlotElement;

        if (!headerSlot.assignedNodes({ flatten: true }).length) {
            this.dialogElem?.setAttribute('no-header-slot', '');
        }
        if (!footerSlot.assignedNodes({ flatten: true }).length) {
            this.dialogElem?.setAttribute('no-footer-slot', '');
        }
        if (this.hasAttribute('data-transparent')) {
            this.dialogElem?.setAttribute('is-transparent', '');
        }

        const id = this.id?.trim();
        if (id) {
            window.dialog ??= {};
            if (!(id in window.dialog)) {
                window.dialog[id] = this;
            }
        }

        this.shadow.querySelector('.dialog-close-icon')?.addEventListener('click', this.dialogClose);
        this.dialogElem?.addEventListener('pointerdown', this.outerDownHandler);
        this.dialogElem?.addEventListener('scroll', this.outerScrollHandler);
        this.dialogElem?.addEventListener('pointerup', this.outerUpHandler);
        this.dialogElem?.addEventListener('close', this.dialogClose);
    }

    private render() {
        this.shadow.innerHTML = /* html */ `
            <dialog id="dialog" class="modal-dialog">
                <div class="dialog-container">
                    <div class="dialog-content">
                        <header class="dialog-header">
                            <slot name="header"></slot>
                        </header>
                        <div class="dialog-body">
                            <slot></slot>
                        </div>
                        <footer class="dialog-footer">
                            <slot name="footer"></slot>
                        </footer>
                        <div class="dialog-close-icon">
                            <button type="button">close</button>
                        </div>
                    </div>
                </div>
            </dialog>
        `;
    }

    private initStyle() {
        const styleElem = document.createElement('style');
        styleElem.textContent = css;
        this.shadow.prepend(styleElem);
    }

    private dialogOpen = () => {
        if (this.isOpen) return;
        this.dialogElem?.showModal();
        this.isOpen = true;
        this.dialogElem?.classList.add('dialog-active');
        this.documentBodyLock();
    };

    private dialogClose = async () => {
        if (!this.isOpen) return;
        this.isOpen = false;
        this.dialogElem?.classList.remove('dialog-active');

        if (this.dialogElem?.open) {
            try {
                await this.waitTransitionEnd('background-color');
            } catch (e) {
                console.warn('Transition wait failed', e);
            }
            this.dialogElem?.close();
        }

        this.documentBodyUnlock();
    };

    private documentBodyLock() {
        UiDialog.hasActiveDialogCount++;
        if (UiDialog.hasActiveDialogCount === 1) {
            this.docScrollY = window.scrollY || window.pageYOffset || document.documentElement.scrollTop;
            const scrollWidth = window.innerWidth - document.documentElement.clientWidth;
            const bodyStyle = window.getComputedStyle(document.body);
            document.body.style.overflow = 'hidden';
            document.body.style.position = 'fixed';
            document.body.style.top = `-${this.docScrollY}px`;
            document.body.style.width = `calc(100% - ${bodyStyle.getPropertyValue('margin-left')})`;
            document.body.style.paddingRight = `${scrollWidth}px`;
        }
    }

    private documentBodyUnlock() {
        UiDialog.hasActiveDialogCount = Math.max(0, UiDialog.hasActiveDialogCount - 1);
        if (UiDialog.hasActiveDialogCount === 0) {
            document.body.style.removeProperty('overflow');
            document.body.style.removeProperty('position');
            document.body.style.removeProperty('top');
            document.body.style.removeProperty('width');
            document.body.style.removeProperty('padding-right');
            window.scrollTo(0, this.docScrollY);
        }
    }

    private waitTransitionEnd = (targetProp: string) => {
        if (this.transitionPromise) return this.transitionPromise;
        this.transitionPromise = new Promise(resolve => {
            let resolved = false;
            const onEnd = (event: TransitionEvent) => {
                if (event.propertyName === targetProp && !resolved) {
                    resolved = true;
                    this.dialogElem?.removeEventListener('transitionend', onEnd);
                    this.transitionPromise = null;
                    resolve();
                }
            };
            this.dialogElem?.addEventListener('transitionend', onEnd);

            setTimeout(() => {
                if (!resolved) {
                    resolved = true;
                    this.dialogElem?.removeEventListener('transitionend', onEnd);
                    this.transitionPromise = null;
                    resolve();
                }
            }, 400);
        });
        return this.transitionPromise;
    };

    private outerDownHandler = (event: PointerEvent) => {
        if (
            event.target instanceof Element &&
            !event.target.closest('ui-dialog') &&
            !event.target.closest('.dialog-content')
        ) {
            this.outerClickTarget = event.target;
        }
    };

    private outerUpHandler = (event: PointerEvent) => {
        if (this.outerClickTarget) {
            if (event.target === this.outerClickTarget) this.dialogClose();
            this.outerClickTarget = undefined;
        }
    };

    private outerScrollHandler = () => {
        if (this.outerClickTarget) {
            this.outerClickTarget = undefined;
        }
    };

    disconnectedCallback() {
        this.shadow.querySelector('.dialog-close-icon')?.removeEventListener('click', this.dialogClose);
        this.dialogElem?.removeEventListener('pointerdown', this.outerDownHandler);
        this.dialogElem?.removeEventListener('scroll', this.outerScrollHandler);
        this.dialogElem?.removeEventListener('pointerup', this.outerUpHandler);
        this.dialogElem?.removeEventListener('close', this.dialogClose);
    }

    get open(): boolean {
        return this.isOpen;
    }

    set open(val: boolean) {
        if (val) {
            this.dialogOpen();
        } else {
            this.dialogClose();
        }
    }
}

customElements.define('ui-dialog', UiDialog);
