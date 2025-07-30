import css from './ui-modal.scss?inline';

declare global {
    interface Window {
        $p: {
            modal?: Record<string, UiModal>;
        };
    }
}

export class UiModal extends HTMLElement {
    private shadow: ShadowRoot;
    private isOpen: boolean = false;
    private modalElem: HTMLDialogElement | null = null;
    private outerClickTarget?: Element;
    private docScrollY: number = 0;
    private transitionPromise: Promise<void> | null = null;
    static hasActiveDialogCount: number = 0;
    onOpened?: () => void = undefined;
    onClosed?: () => void = undefined;

    constructor() {
        super();
        this.shadow = this.attachShadow({ mode: 'open' });
        this.render();
        this.initStyle();
    }

    connectedCallback() {
        this.modalElem = this.shadow.querySelector('#modal');
        const headerSlot = this.shadow.querySelector('slot[name=header]') as HTMLSlotElement;
        const footerSlot = this.shadow.querySelector('slot[name=footer]') as HTMLSlotElement;

        if (!headerSlot.assignedNodes({ flatten: true }).length) {
            this.modalElem?.setAttribute('no-header-slot', '');
        }
        if (!footerSlot.assignedNodes({ flatten: true }).length) {
            this.modalElem?.setAttribute('no-footer-slot', '');
        }
        if (this.hasAttribute('data-transparent')) {
            this.modalElem?.setAttribute('is-transparent', '');
        }

        const id = this.id?.trim();
        if (id) {
            window.$p.modal ??= {};
            if (!(id in window.$p.modal)) {
                window.$p.modal[id] = this;
            }
        }

        this.shadow.querySelector('.modal-close-icon')?.addEventListener('click', this.modalClose);
        this.modalElem?.addEventListener('pointerdown', this.outerDownHandler);
        this.modalElem?.addEventListener('scroll', this.outerScrollHandler);
        this.modalElem?.addEventListener('pointerup', this.outerUpHandler);
        this.modalElem?.addEventListener('close', this.modalClose);
    }

    private render() {
        this.shadow.innerHTML = /* html */ `
            <dialog id="modal" class="ui-modal">
                <div class="modal-container">
                    <div class="modal-content">
                        <header class="modal-header">
                            <slot name="header"></slot>
                        </header>
                        <div class="modal-body">
                            <slot></slot>
                        </div>
                        <footer class="modal-footer">
                            <slot name="footer"></slot>
                        </footer>
                        <div class="modal-close-icon">
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

    private modalOpen = () => {
        if (this.isOpen) return;
        this.modalElem?.showModal();
        this.isOpen = true;
        this.modalElem?.classList.add('modal-active');
        if (this.onOpened) this.onOpened();
        this.documentBodyLock();
    };

    private modalClose = async () => {
        if (!this.isOpen) return;
        this.isOpen = false;
        this.modalElem?.classList.remove('modal-active');

        if (this.modalElem?.open) {
            try {
                await this.waitTransitionEnd('background-color');
            } catch (e) {
                console.warn('Transition wait failed', e);
            } finally {
                if (this.onClosed) this.onClosed();
                this.modalElem?.close();
            }
        }

        this.documentBodyUnlock();
    };

    private documentBodyLock() {
        UiModal.hasActiveDialogCount++;
        if (UiModal.hasActiveDialogCount === 1) {
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
        UiModal.hasActiveDialogCount = Math.max(0, UiModal.hasActiveDialogCount - 1);
        if (UiModal.hasActiveDialogCount === 0) {
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
                    this.modalElem?.removeEventListener('transitionend', onEnd);
                    resolve();
                    this.transitionPromise = null;
                }
            };
            this.modalElem?.addEventListener('transitionend', onEnd);

            setTimeout(() => {
                if (!resolved) {
                    resolved = true;
                    this.modalElem?.removeEventListener('transitionend', onEnd);
                    resolve();
                    this.transitionPromise = null;
                }
            }, 400);
        });
        return this.transitionPromise;
    };

    private outerDownHandler = (event: PointerEvent) => {
        if (
            event.target instanceof Element &&
            !event.target.closest('ui-modal') &&
            !event.target.closest('.modal-content')
        ) {
            this.outerClickTarget = event.target as Element;
        }
    };

    private outerUpHandler = (event: PointerEvent) => {
        if (this.outerClickTarget) {
            if (event.target === this.outerClickTarget) this.modalClose();
            this.outerClickTarget = undefined;
        }
    };

    private outerScrollHandler = () => {
        if (this.outerClickTarget) {
            this.outerClickTarget = undefined;
        }
    };

    disconnectedCallback() {
        this.shadow.querySelector('.modal-close-icon')?.removeEventListener('click', this.modalClose);
        this.modalElem?.removeEventListener('pointerdown', this.outerDownHandler);
        this.modalElem?.removeEventListener('scroll', this.outerScrollHandler);
        this.modalElem?.removeEventListener('pointerup', this.outerUpHandler);
        this.modalElem?.removeEventListener('close', this.modalClose);
    }

    get open(): boolean {
        return this.isOpen;
    }

    set open(val: boolean) {
        if (val) {
            this.modalOpen();
        } else {
            this.modalClose();
        }
    }
}

customElements.define('ui-modal', UiModal);
