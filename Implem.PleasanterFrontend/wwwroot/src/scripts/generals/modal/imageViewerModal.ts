import { UiModal } from './ui-modal.ts';
import css from './imageViewerModal.scss?inline';

export class ImageViewerModal extends HTMLElement {
    private shadow: ShadowRoot;
    private customModalElem!: UiModal;
    private imgCurrent?: number;
    private imgCollection?: string[];
    private imgElem!: HTMLImageElement;
    private collectionCurrentElem!: HTMLElement;
    private collectionMaxNumElem!: HTMLElement;
    private collectionPrevElem!: HTMLButtonElement;
    private collectionNextElem!: HTMLButtonElement;

    constructor() {
        super();
        this.shadow = this.attachShadow({ mode: 'open' });
    }

    connectedCallback() {
        this.render();
        this.initStyle();
        this.customModalElem = this.shadow.querySelector('ui-modal')!;
        this.imgElem = this.shadow.querySelector('#imgViewer')!;
        this.collectionCurrentElem = this.shadow.querySelector('.counter .current')!;
        this.collectionMaxNumElem = this.shadow.querySelector('.counter .max')!;
        this.collectionPrevElem = this.shadow.querySelector('.counter .prev')!;
        this.collectionNextElem = this.shadow.querySelector('.counter .next')!;

        this.collectionPrevElem.addEventListener('click', this.naviHandler);
        this.collectionNextElem.addEventListener('click', this.naviHandler);
        this.shadow.addEventListener('keydown', this.keyHandler as EventListener);
    }

    private render() {
        this.shadow.innerHTML = /* html */ `
            <ui-modal data-transparent="1" style="--modal-min-width: 0;">
                <div id="loading"><span class="loader" /></div>
                <div class="viewer-stage">
                    <div class="stage-body">
                        <figure>
                            <img id="imgViewer" src="" alt="" />
                        </figure>
                    </div>
                    <div class="counter">
                        <div class="counter-inner">
                            <button type="button" class="navi prev">
                                <span class="icon">keyboard_arrow_left</span>
                            </button>
                            <span>Image</span>
                            <span class="current"></span>
                            <span>of</span>
                            <span class="max"></span>
                            <button type="button" class="navi next">
                                <span class="icon">keyboard_arrow_right</span>
                            </button>
                        </div>
                    </div>
                </div>
            </ui-modal>
        `;
    }

    private initStyle() {
        const styleElem = document.createElement('style');
        styleElem.textContent = css;
        this.shadow.prepend(styleElem);
    }

    private naviHandler = (event: MouseEvent) => {
        if (this.customModalElem.hasAttribute('is-loading')) return;
        if ((event.currentTarget as HTMLElement).classList.contains('prev')) {
            this.naviPrev();
        } else if ((event.currentTarget as HTMLElement).classList.contains('next')) {
            this.naviNext();
        }
    };

    private keyHandler = (event: KeyboardEvent) => {
        if (this.customModalElem.hasAttribute('is-loading')) return;
        if (event.key === 'ArrowLeft') {
            this.naviPrev();
        }
        if (event.key === 'ArrowRight') {
            this.naviNext();
        }
    };

    private naviPrev() {
        if (this.imgCurrent === undefined || !this.imgCollection) return;
        this.imgCurrent = this.imgCurrent <= 0 ? this.imgCollection.length - 1 : this.imgCurrent - 1;
        this.imgDisplay(this.imgCollection[this.imgCurrent], 400);
    }

    private naviNext() {
        if (this.imgCurrent === undefined || !this.imgCollection) return;
        this.imgCurrent = this.imgCurrent >= this.imgCollection.length - 1 ? 0 : this.imgCurrent + 1;
        this.imgDisplay(this.imgCollection[this.imgCurrent], 400);
    }

    show(node: HTMLImageElement, imageNodes?: NodeListOf<HTMLImageElement>) {
        let imgSrc;
        this.imgElem.src = '';
        this.imgCurrent = undefined;
        this.imgCollection = undefined;
        this.customModalElem.removeAttribute('has-collection');
        this.collectionCurrentElem.innerHTML = '';
        this.collectionMaxNumElem.innerHTML = '';
        if (imageNodes && imageNodes.length > 1) {
            this.imgCurrent = Array.from(imageNodes).indexOf(node);
            this.imgCollection = Array.from(imageNodes).map(img => (img as HTMLImageElement).src);
            this.customModalElem.setAttribute('has-collection', '');
            this.collectionCurrentElem.textContent = String(this.imgCurrent + 1);
            this.collectionMaxNumElem.textContent = String(this.imgCollection.length);
            imgSrc = this.imgCollection[this.imgCurrent];
        } else {
            imgSrc = node.src;
        }

        if (imgSrc) this.imgDisplay(imgSrc, 1000);
    }

    private imgDisplay(imgSrc: string, minWaitMs: number) {
        this.loadImageWithMinWait(imgSrc, minWaitMs)
            .then(img => {
                this.imgElem.src = img.src;
                this.customModalElem.removeAttribute('is-loading');
                this.customModalElem.removeAttribute('no-display-close');
                if (this.imgCurrent !== undefined) {
                    this.collectionCurrentElem.textContent = String(this.imgCurrent + 1);
                }
            })
            .catch(_ => {
                this.customModalElem.open = false;
            });
    }

    loadImageWithMinWait(src: string, minWaitMs: number = 2000): Promise<HTMLImageElement> {
        return new Promise((resolve, reject) => {
            const start = Date.now();
            const img = document.createElement('img');
            this.customModalElem.setAttribute('is-loading', '');
            this.customModalElem.setAttribute('no-display-close', '');

            img.onload = () => {
                const elapsed = Date.now() - start;
                const waitTime = Math.max(0, minWaitMs - elapsed);
                setTimeout(() => resolve(img), waitTime);
            };

            img.onerror = e => {
                reject(e);
            };
            this.customModalElem.open = true;
            img.src = src;
        });
    }

    disconnectedCallback() {
        this.collectionPrevElem?.removeEventListener('click', this.naviHandler);
        this.collectionNextElem?.removeEventListener('click', this.naviHandler);
        this.shadow.removeEventListener('keydown', this.keyHandler as EventListener);
    }

    set onClosed(method: () => void) {
        this.customModalElem.onClosed = () => {
            if (this.imgCollection?.length) {
                this.imgCurrent = undefined;
                this.imgCollection = undefined;
                this.customModalElem.removeAttribute('has-collection');
                this.collectionCurrentElem.innerHTML = '';
                this.collectionMaxNumElem.innerHTML = '';
            }
            this.imgElem.src = '';
            method();
        };
    }
}

customElements.define('image-viewer-modal', ImageViewerModal);
