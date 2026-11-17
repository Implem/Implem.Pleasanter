import css from './ui-tooltip.scss?inline';

export type UiTooltipPlacement = 'top' | 'bottom';

const VIEWPORT_MARGIN = 12;
const ARROW_INSET = 12;
const START_ALIGN_OFFSET = 28;
const DEFAULT_OFFSET = 10;

export class UiTooltip extends HTMLElement {
    private readonly shadow: ShadowRoot;
    private anchorElement: Element | null = null;
    private frame = 0;

    constructor() {
        super();

        this.shadow = this.attachShadow({ mode: 'open' });

        this.shadow.innerHTML = /* html */ `
            <style>
                ${css}
            </style>
            <div class="tooltip" part="tooltip">
                <div class="arrow" part="arrow"></div>
                <div class="header" part="header"><slot name="header"></slot></div>
                <div class="body" part="body"><slot></slot></div>
                <div class="footer" part="footer"><slot name="footer"></slot></div>
            </div>
        `;
    }

    connectedCallback() {
        if (!this.hasAttribute('role')) {
            this.setAttribute('role', 'tooltip');
        }

        const headerSlot = this.shadow.querySelector<HTMLSlotElement>('slot[name=header]');
        const footerSlot = this.shadow.querySelector<HTMLSlotElement>('slot[name=footer]');

        const syncSlots = () => {
            this.toggleAttribute('no-header', !headerSlot?.assignedNodes({ flatten: true }).length);
            this.toggleAttribute('no-footer', !footerSlot?.assignedNodes({ flatten: true }).length);
        };

        headerSlot?.addEventListener('slotchange', syncSlots);
        footerSlot?.addEventListener('slotchange', syncSlots);

        syncSlots();

        if (this.open) {
            this.listen(true);
        }
    }

    disconnectedCallback() {
        this.listen(false);
    }

    get anchor(): Element | null {
        return this.anchorElement;
    }

    set anchor(element: Element | null) {
        this.anchorElement = element;

        if (this.open) {
            this.reposition();
        }
    }

    get placement(): UiTooltipPlacement {
        return this.getAttribute('placement') === 'top' ? 'top' : 'bottom';
    }

    set placement(value: UiTooltipPlacement) {
        this.setAttribute('placement', value);
    }

    get open(): boolean {
        return this.hasAttribute('open');
    }

    set open(value: boolean) {
        if (value) {
            this.show();
        } else {
            this.hide();
        }
    }

    show(anchor?: Element | null) {
        if (anchor !== undefined) {
            this.anchorElement = anchor;
        }

        if (!this.open) {
            this.setAttribute('open', '');
            this.listen(true);
        }

        this.reposition();
        this.replayEnterAnimation();
    }

    hide() {
        if (!this.open) {
            return;
        }

        this.removeAttribute('open');
        this.listen(false);
    }

    reposition = () => {
        if (!this.open || !this.anchorElement) {
            return;
        }

        const rect = this.anchorElement.getBoundingClientRect();
        const offset = Number(this.getAttribute('offset') ?? DEFAULT_OFFSET);
        const viewportWidth = document.documentElement.clientWidth;
        const viewportHeight = window.innerHeight;
        const width = this.offsetWidth;
        const height = this.offsetHeight;

        const below = rect.bottom + offset;
        const above = rect.top - offset - height;
        const fitsBelow = below + height <= viewportHeight;
        const fitsAbove = above >= 0;

        let placement = this.placement;

        if (placement === 'bottom' && !fitsBelow && fitsAbove) {
            placement = 'top';
        } else if (placement === 'top' && !fitsAbove && fitsBelow) {
            placement = 'bottom';
        }

        const alignStart = this.getAttribute('align') === 'start';

        const anchorX = alignStart
            ? Math.min(rect.left + START_ALIGN_OFFSET, rect.left + rect.width / 2)
            : rect.left + rect.width / 2;

        const preferredLeft = alignStart ? anchorX - START_ALIGN_OFFSET : anchorX - width / 2;
        const left = Math.max(VIEWPORT_MARGIN, Math.min(preferredLeft, viewportWidth - width - VIEWPORT_MARGIN));
        const top = Math.max(VIEWPORT_MARGIN, placement === 'bottom' ? below : above);
        const arrowX = Math.max(ARROW_INSET, Math.min(anchorX - left, width - ARROW_INSET));

        this.style.left = `${left}px`;
        this.style.top = `${top}px`;
        this.style.setProperty('--arrow-x', `${arrowX}px`);
        this.dataset.placement = placement;
    };

    private replayEnterAnimation() {
        const tooltip = this.shadow.querySelector<HTMLElement>('.tooltip');

        if (!tooltip) {
            return;
        }

        tooltip.classList.remove('enter');
        void tooltip.offsetWidth;
        tooltip.classList.add('enter');
    }

    private scheduleReposition = () => {
        cancelAnimationFrame(this.frame);
        this.frame = requestAnimationFrame(this.reposition);
    };

    private listen(enabled: boolean) {
        if (enabled) {
            window.addEventListener('resize', this.scheduleReposition);
            window.addEventListener('scroll', this.scheduleReposition, true);
            return;
        }

        window.removeEventListener('resize', this.scheduleReposition);
        window.removeEventListener('scroll', this.scheduleReposition, true);
        cancelAnimationFrame(this.frame);
    }
}

declare global {
    interface HTMLElementTagNameMap {
        'ui-tooltip': UiTooltip;
    }
}

if (!customElements.get('ui-tooltip')) {
    customElements.define('ui-tooltip', UiTooltip);
}
