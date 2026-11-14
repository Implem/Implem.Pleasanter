import './tooltip/ui-tooltip';

type StepPlacement = 'Top' | 'Bottom';

interface Step {
    Target: string;
    Placement: StepPlacement;
    Title: string;
    Body: string;
}

const PADDING = 8;
const GAP = 14;

const createElement = <K extends keyof HTMLElementTagNameMap>(
    tag: K,
    className?: string,
    text?: string
): HTMLElementTagNameMap[K] => {
    const element = document.createElement(tag);

    if (className) {
        element.className = className;
    }

    if (text !== undefined) {
        element.textContent = text;
    }

    return element;
};

const isVisible = (target: Element | null): target is HTMLElement => {
    if (!target) {
        return false;
    }

    const { width, height } = target.getBoundingClientRect();

    return width > 0 && height > 0;
};

const parseSteps = (root: HTMLElement): Step[] => {
    try {
        return JSON.parse(root.dataset.steps ?? '[]') as Step[];
    } catch {
        return [];
    }
};

class StartGuideTour {
    private index = 0;
    private target: HTMLElement | null = null;

    private readonly overlay = createElement('div', 'sg-tour-overlay');
    private readonly spotlight = createElement('div', 'sg-tour-spotlight');
    private readonly tooltip = createElement('ui-tooltip', 'sg-tour-tooltip');
    private readonly title = createElement('span');
    private readonly body = createElement('span');
    private readonly skipButton = createElement('button', 'sg-tour-skip');
    private readonly counter = createElement('span', 'sg-tour-counter');
    private readonly nextButton = createElement('button', 'sg-tour-next');

    constructor(
        private readonly steps: Step[],
        private readonly labels: { next: string; done: string; skip: string },
        private readonly onFinish: () => void
    ) {
        this.tooltip.setAttribute('role', 'dialog');
        this.tooltip.setAttribute('aria-live', 'polite');
        this.tooltip.setAttribute('align', 'start');
        this.tooltip.setAttribute('offset', String(PADDING + GAP));

        this.title.slot = 'header';
        this.skipButton.slot = 'footer';
        this.counter.slot = 'footer';
        this.nextButton.slot = 'footer';

        this.skipButton.type = 'button';
        this.nextButton.type = 'button';
        this.skipButton.textContent = labels.skip;

        this.tooltip.append(this.title, this.body, this.skipButton, this.counter, this.nextButton);
    }

    start() {
        document.body.append(this.overlay, this.spotlight, this.tooltip);

        this.nextButton.addEventListener('click', this.next);
        this.skipButton.addEventListener('click', this.finish);
        this.overlay.addEventListener('click', this.swallowClick);

        window.addEventListener('resize', this.placeSpotlight);
        window.addEventListener('scroll', this.placeSpotlight, true);
        document.addEventListener('keydown', this.onKeyDown, true);

        this.go(0);
    }

    private next = () => {
        this.go(this.index + 1);
    };

    private swallowClick = (event: MouseEvent) => {
        event.stopPropagation();
    };

    private onKeyDown = (event: KeyboardEvent) => {
        const actions: Record<string, (() => void) | undefined> = {
            Escape: this.finish,
            Enter: this.next,
            ArrowRight: this.next,
            ArrowLeft: this.index > 0 ? () => this.go(this.index - 1) : undefined
        };

        const action = actions[event.key];

        if (!action) {
            return;
        }

        event.preventDefault();
        action();
    };

    private go(index: number) {
        if (index >= this.steps.length) {
            this.finish();
            return;
        }

        this.target?.classList.remove('sg-tour-target');

        this.index = index;

        const step = this.steps[index];
        const target = document.querySelector<HTMLElement>(step.Target);

        if (!target) {
            this.go(index + 1);
            return;
        }

        this.target = target;
        this.target.classList.add('sg-tour-target');

        const isLast = index === this.steps.length - 1;

        this.title.textContent = step.Title;
        this.body.textContent = step.Body;
        this.counter.textContent = `${index + 1} / ${this.steps.length}`;
        this.nextButton.textContent = isLast ? this.labels.done : this.labels.next;
        this.nextButton.classList.toggle('sg-tour-next-done', isLast);

        const { top, bottom } = target.getBoundingClientRect();

        if (top < 0 || bottom > window.innerHeight) {
            target.scrollIntoView({ block: 'center' });
        }

        this.placeSpotlight();

        this.tooltip.placement = step.Placement === 'Top' ? 'top' : 'bottom';
        this.tooltip.show(target);

        this.nextButton.focus({ preventScroll: true });
    }

    private placeSpotlight = () => {
        if (!this.target) {
            return;
        }

        const { top, left, width, height } = this.target.getBoundingClientRect();

        Object.assign(this.spotlight.style, {
            top: `${top - PADDING}px`,
            left: `${left - PADDING}px`,
            width: `${width + PADDING * 2}px`,
            height: `${height + PADDING * 2}px`
        });
    };

    private finish = () => {
        this.target?.classList.remove('sg-tour-target');
        this.target = null;

        this.tooltip.hide();
        this.overlay.remove();
        this.spotlight.remove();
        this.tooltip.remove();

        window.removeEventListener('resize', this.placeSpotlight);
        window.removeEventListener('scroll', this.placeSpotlight, true);
        document.removeEventListener('keydown', this.onKeyDown, true);

        this.onFinish();
    };
}

let activeTour: StartGuideTour | null = null;

const startSkystarStartGuide = () => {
    const root = document.getElementById('SkystarStartGuide');

    if (!root || activeTour) {
        return;
    }

    const steps = parseSteps(root).filter(step => isVisible(document.querySelector(step.Target)));

    if (steps.length === 0) {
        return;
    }

    const labels = {
        next: root.dataset.next ?? 'Next',
        done: root.dataset.done ?? 'Done',
        skip: root.dataset.skip ?? 'Skip'
    };

    activeTour = new StartGuideTour(steps, labels, () => {
        activeTour = null;
        $p.setStartGuide(1);
    });

    activeTour.start();
};

export const SkystarStartGuide = {
    startSkystarStartGuide
};

Object.assign($p, SkystarStartGuide);

$(startSkystarStartGuide);
