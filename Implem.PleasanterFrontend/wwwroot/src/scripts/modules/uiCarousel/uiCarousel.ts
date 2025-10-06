import Glide from '@glidejs/glide';
import glidejsCssRaw from '@glidejs/glide/dist/css/glide.core.min.css?raw';
import css from './uiCarousel.scss?inline';

export class uiCarousel extends HTMLElement {
    private items?: Element[];
    private glide?: Glide;

    constructor() {
        super();
    }

    connectedCallback() {
        this.items = Array.from(this.children);
        if (this.items.length > 1) {
            this.render();
            this.initStyle();

            const gridEl = this.querySelector('.glide') as HTMLElement;
            if (gridEl) {
                try {
                    const autoPlay = this.getAutoPlay(this.getAttribute('auto-play'));
                    this.glide = new Glide(gridEl, {
                        type: 'slider',
                        perView: 1,
                        gap: 0,
                        autoplay: autoPlay,
                        hoverpause: true
                    });
                    this.glide.mount();
                } catch (e) {
                    console.error('Error initializing Glide:', e);
                }
            }
        }
    }

    private getAutoPlay(attr: string | null): number {
        if (!attr?.trim() || attr.toLowerCase() === 'true') {
            return 3000;
        }
        if (attr.toLowerCase() === 'false') {
            return 0;
        }
        const speed = Number(attr);
        if (Number.isFinite(speed) && speed >= 0) {
            return speed;
        }
        return 0;
    }

    private render() {
        const bullets = this.items!.map(
            (_, i) => `<div class="bullets-item glide__bullet" data-glide-dir="=${i}"><span></span></div>`
        ).join('');
        this.innerHTML = /* html */ `
            <div class="carousel-stage glide">
                <div class="carousel-inner glide__track" data-glide-el="track">
                    <div class="carousel-items glide__slides">
                        ${this.items!.map(item => {
                            return `<li class="carousel-item glide__slide">${item.outerHTML}</li>`;
                        }).join('')}
                    </div>
                </div>
                <div class="carousel-arrows glide__arrows" data-glide-el="controls">
                    <div class="arrows-nav is-prev glide__arrow glide__arrow--left" data-glide-dir="<">
                        <span class="material-symbols-outlined">arrow_back_ios_new</span>
                    </div>
                    <div class="arrows-nav is-next glide__arrow glide__arrow--right" data-glide-dir=">">
                        <span class="material-symbols-outlined">arrow_forward_ios</span>
                    </div>
                </div>
                <div class="carousel-bullets glide__bullets" data-glide-el="controls[nav]">${bullets}</div>
            </div>
        `;
    }

    private initStyle() {
        if (!document.querySelector('#uiCarouselCss')) {
            const style = document.createElement('style');
            style.setAttribute('id', 'uiCarouselCss');
            style.textContent = glidejsCssRaw + css;
            document.head.append(style);
        }
    }

    disconnectedCallback() {
        if (this.glide) this.glide.destroy();
    }
}

customElements.define('ui-carousel', uiCarousel);
