// @vitest-environment happy-dom
import { afterEach, beforeEach, describe, expect, it } from 'vitest';

import { UiTooltip } from './ui-tooltip';

const VIEWPORT_WIDTH = 1000;
const VIEWPORT_HEIGHT = 600;
const TOOLTIP_WIDTH = 240;
const TOOLTIP_HEIGHT = 100;

const createAnchor = (rect: { top: number; left: number; width: number; height: number }) => {
    const anchor = document.createElement('div');

    anchor.getBoundingClientRect = () =>
        ({
            ...rect,
            right: rect.left + rect.width,
            bottom: rect.top + rect.height,
            x: rect.left,
            y: rect.top,
            toJSON: () => ({})
        }) as DOMRect;

    document.body.append(anchor);

    return anchor;
};

const createTooltip = (attributes: Record<string, string> = {}) => {
    const tooltip = document.createElement('ui-tooltip');

    Object.entries(attributes).forEach(([name, value]) => tooltip.setAttribute(name, value));

    Object.defineProperty(tooltip, 'offsetWidth', { value: TOOLTIP_WIDTH });
    Object.defineProperty(tooltip, 'offsetHeight', { value: TOOLTIP_HEIGHT });

    document.body.append(tooltip);

    return tooltip;
};

describe('ui-tooltip', () => {
    beforeEach(() => {
        Object.defineProperty(document.documentElement, 'clientWidth', {
            value: VIEWPORT_WIDTH,
            configurable: true
        });

        Object.defineProperty(window, 'innerHeight', { value: VIEWPORT_HEIGHT, configurable: true });
    });

    afterEach(() => {
        document.body.innerHTML = '';
    });

    it('is registered as a custom element', () => {
        expect(document.createElement('ui-tooltip')).toBeInstanceOf(UiTooltip);
    });

    it('defaults to the tooltip role and flags empty slots', () => {
        const tooltip = createTooltip();

        expect(tooltip.getAttribute('role')).toBe('tooltip');
        expect(tooltip.hasAttribute('no-header')).toBe(true);
        expect(tooltip.hasAttribute('no-footer')).toBe(true);
    });

    it('keeps a role set by the consumer', () => {
        const tooltip = createTooltip({ role: 'dialog' });

        expect(tooltip.getAttribute('role')).toBe('dialog');
    });

    it('opens below and centered on the anchor by default', () => {
        const anchor = createAnchor({ top: 100, left: 400, width: 200, height: 40 });
        const tooltip = createTooltip();

        tooltip.show(anchor);

        expect(tooltip.open).toBe(true);
        expect(tooltip.dataset.placement).toBe('bottom');
        expect(tooltip.style.top).toBe('150px');
        expect(tooltip.style.left).toBe('380px');
        expect(tooltip.style.getPropertyValue('--arrow-x')).toBe('120px');
    });

    it('flips to the top when there is no room below', () => {
        const anchor = createAnchor({ top: 520, left: 400, width: 200, height: 40 });
        const tooltip = createTooltip();

        tooltip.show(anchor);

        expect(tooltip.dataset.placement).toBe('top');
        expect(tooltip.style.top).toBe('410px');
    });

    it('flips to the bottom when there is no room above', () => {
        const anchor = createAnchor({ top: 20, left: 400, width: 200, height: 40 });
        const tooltip = createTooltip({ placement: 'top' });

        tooltip.show(anchor);

        expect(tooltip.dataset.placement).toBe('bottom');
    });

    it('aligns to the start of the anchor and clamps to the viewport', () => {
        const anchor = createAnchor({ top: 100, left: 900, width: 80, height: 20 });
        const tooltip = createTooltip({ align: 'start', offset: '22' });

        tooltip.show(anchor);

        expect(tooltip.style.top).toBe('142px');
        expect(tooltip.style.left).toBe('748px');
        expect(tooltip.style.getPropertyValue('--arrow-x')).toBe('180px');
    });

    it('closes with hide', () => {
        const anchor = createAnchor({ top: 100, left: 400, width: 200, height: 40 });
        const tooltip = createTooltip();

        tooltip.show(anchor);
        tooltip.hide();

        expect(tooltip.open).toBe(false);
    });
});
