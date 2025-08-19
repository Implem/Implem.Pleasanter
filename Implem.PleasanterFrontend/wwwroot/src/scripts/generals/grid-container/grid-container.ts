import shadowCss from './grid-container.scss?inline';

export class GridContainerElement extends HTMLElement {
    private hash: string;
    private shadow?: ShadowRoot;
    private isScrollable = false;
    private isEntered = false;
    private isMouseHeld = false;
    public isKeyHeld = false;
    private hasXScroll = false;
    private gridEl: HTMLElement | null = null;
    private stageEl: HTMLElement | null = null;
    private flameEl: HTMLElement | null = null;
    private mutationObserver?: MutationObserver;
    private resizeObserver?: ResizeObserver;

    constructor() {
        super();
        this.isScrollable = Boolean(this.dataset.scrollable);
        this.hash = this.randomString(10);
        if (this.isScrollable) {
            this.classList.add('app-scrollable');
            this.shadow = this.attachShadow({ mode: 'open' });
            this.shadow.append(this.htmlRender());
            this.gridEl = this.querySelector('.grid');
            this.stageEl = this.shadow.querySelector('.app-grid-inner')!;
            this.flameEl = this.shadow.querySelector('.app-grid-frame')!;
            this.gridEl?.classList.add(this.hash);
            this.initShadowStyle();
            setTimeout(() => {
                if (this.gridEl?.id === 'Grid') {
                    this.classList.add('app-is-index');
                }
                this.checkCanScroll();
            }, 10);
        }
    }

    connectedCallback() {
        if (!this.isScrollable || !this.gridEl) return;
        this.addObserver();
        this.addScrollEvents();
    }

    private checkCanScroll() {
        if (!this.flameEl) return;
        if (this.flameEl.scrollWidth > this.flameEl.clientWidth) {
            this.hasXScroll = true;
            this.classList.add('app-x-scroll');
        } else {
            this.hasXScroll = false;
            this.classList.remove('app-x-scroll');
        }

        if (this.flameEl.scrollHeight > this.flameEl.clientHeight) {
            this.classList.add('app-y-scroll');
            this.setAttribute('app-y-scroll', '');
        } else {
            this.classList.remove('app-y-scroll');
        }

        this.setScrollGridStyle();
    }

    private addObserver() {
        this.mutationObserver = new MutationObserver(mutations => {
            const hasChildListMutation = mutations.some(mutation => mutation.type === 'childList');
            if (hasChildListMutation) {
                this.checkCanScroll();
            }
        });
        this.mutationObserver.observe(this.gridEl!, { childList: true });

        let previousWidth: number = this.offsetWidth;
        let resizeTimeID: ReturnType<typeof setTimeout>;
        this.resizeObserver = new ResizeObserver(entries => {
            for (const entry of entries) {
                const newWidth = entry.contentRect.width;
                if (newWidth !== previousWidth) {
                    previousWidth = newWidth;
                    clearTimeout(resizeTimeID);
                    resizeTimeID = setTimeout(() => {
                        this.checkCanScroll();
                    }, 100);
                }
            }
        });
        this.resizeObserver.observe(this);
    }

    private addScrollEvents() {
        // マウスIN|OUT
        this.addEventListener('mouseenter', this.handleAreaEnter);
        this.addEventListener('mouseleave', this.handleAreaLeave);

        // Controlキー操作
        document.addEventListener('keydown', this.handleKeyPress);
        document.addEventListener('keyup', this.handleKeyRelease);

        // スクロール操作
        document.addEventListener('mousedown', this.handleScrollStart);
        document.addEventListener('mouseup', this.handleScrollEnd);
        document.addEventListener('mousemove', this.handleScrollAction);
    }

    private handleAreaEnter = () => {
        this.isEntered = true;
    };
    private handleAreaLeave = () => {
        this.isEntered = false;
    };

    private handleKeyPress = (e: KeyboardEvent) => {
        if (e.key === 'Alt' && !e.repeat) {
            if (this.isEntered && !this.isMouseHeld) {
                this.isKeyHeld = true;
                this.classList.add('app-held-key');
            }
        }
    };
    private handleKeyRelease = (e: KeyboardEvent) => {
        if (e.key === 'Alt') {
            this.isKeyHeld = false;
            this.classList.remove('app-held-key');
        }
    };

    private handleScrollStart = (e: MouseEvent) => {
        if (this.isEntered && !this.isKeyHeld && e.button === 0) {
            this.isMouseHeld = true;
        }
    };
    private handleScrollEnd = () => {
        this.isMouseHeld = false;
        this.stageEl!.classList.remove('app-dragging');
    };
    private handleScrollAction = (e: MouseEvent) => {
        if (!this.isKeyHeld && this.isMouseHeld) {
            this.stageEl!.classList.add('app-dragging');
            e.preventDefault();
            if (this.isEntered) {
                const dx = e.movementX;
                const dy = e.movementY;
                this.flameEl!.scrollLeft -= dx;
                this.flameEl!.scrollTop -= dy;
            } else {
                const selection = window.getSelection();
                if (selection) {
                    selection.removeAllRanges();
                }
            }
        }
    };

    private initShadowStyle() {
        const shadowStyle = document.createElement('style');
        shadowStyle.textContent = shadowCss;
        this.shadow?.appendChild(shadowStyle);
    }

    private setScrollGridStyle() {
        const style = document.querySelector(`#${this.hash}-style`) || document.createElement('style');
        style.setAttribute('id', `${this.hash}-style`);
        document.head.appendChild(style);
        style.textContent = '';
        if (this.hasXScroll) {
            const thead = this.gridEl?.querySelector('thead:last-of-type');
            const cells = thead?.querySelectorAll('th');
            if (cells) {
                const lefts: number[] = [0];
                let width: number = 0;
                const stickyIndexes = Array.from(cells)
                    .map((th, index) => (th.dataset.cellSticky ? index : -1))
                    .filter(index => index !== -1);

                if (thead?.querySelector('th:not([data-cell-sticky]) :is(#GridCheckAll, .select-all)')) {
                    stickyIndexes.unshift(0);
                }

                stickyIndexes.forEach(index => {
                    const fixCell = thead?.querySelector(`tr th:nth-child(${index + 1})`) as HTMLElement;
                    width += fixCell.offsetWidth || 0;
                    lefts.push(width);
                });

                style.textContent = /* css */ `
                    ${stickyIndexes
                        .map((cellIndex, index) => {
                            return /* css */ `
                                .${this.hash} tr:first-child th:nth-child(${cellIndex + 1}) {
                                    position: sticky;
                                    z-index: 2;
                                    left: ${lefts[index]}px;
                                }
                                .${this.hash} tr td:nth-child(${cellIndex + 1}) {
                                    position: sticky;
                                    z-index: 2;
                                    left: ${lefts[index]}px;
                                }
                            `;
                        })
                        .join('')}
                `;
            }
        }
    }

    private htmlRender(): HTMLElement {
        return this.htmlParser(/*html*/ `
            <div class="app-grid-inner">
                <div class="app-grid-frame">
                    <slot></slot>
                </div>
                <div class="app-scroll-layer" />
            </div>
        `);
    }

    private htmlParser(htmlString: string) {
        const parser = new DOMParser();
        const doc = parser.parseFromString(htmlString, 'text/html');
        const element = doc.body.firstChild as HTMLDivElement;
        return element;
    }

    private randomString(length = 8) {
        const chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
        let result = 'grid-';
        for (let i = 0; i < length; i++) {
            result += chars[(Math.random() * chars.length) | 0];
        }
        return result;
    }

    disconnectedCallback() {
        if (this.isScrollable) {
            this.mutationObserver?.disconnect();
            this.resizeObserver?.disconnect();
            this.removeEventListener('mouseenter', this.handleAreaEnter);
            this.removeEventListener('mouseleave', this.handleAreaLeave);
            document.removeEventListener('keydown', this.handleKeyPress);
            document.removeEventListener('keyup', this.handleKeyRelease);
            document.removeEventListener('mousedown', this.handleScrollStart);
            document.removeEventListener('mouseup', this.handleScrollEnd);
            document.removeEventListener('mousemove', this.handleScrollAction);
            this.stageEl = null;
            this.flameEl = null;
            this.shadowRoot!.innerHTML = '';
            this.shadow = undefined;
        }
    }
}

customElements.define('grid-container', GridContainerElement);
