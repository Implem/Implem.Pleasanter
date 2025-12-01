import { applyFilters, activeTextFilters, textFilterTimers } from './filter.ts';
import { hideToastMenu } from './toastmenu.ts';

const initializeEditorColumnsFilterInputs = (root?: HTMLElement) => {
    const base = root || document;
    const selector = [
        "[id^='left-editor-columns-filter-input-alt']",
        "[id^='right-editor-columns-filter-input-alt']"
    ].join(', ');
    (qsaFromBaseOptional(selector, base) as HTMLInputElement[]).forEach(inputElement => {
        setupTextFilterInput(inputElement);
    });
};

const initializeFilterVisual = (root?: HTMLElement) => {
    const base = root || document;
    const wrap = base.querySelector('.editor-columns-filter') as HTMLInputElement;
    const inputElement = base.querySelector('#left-editor-columns-filter-input-alt') as HTMLInputElement;
    const buttonElement = base.querySelector('#left-editor-columns-filter-button-alt') as HTMLButtonElement;

    if (!wrap || !inputElement) {
        return;
    }
    if (wrap.dataset.filterVisualInit === '1') {
        return;
    }
    if (buttonElement) {
        buttonElement.setAttribute('tabindex', '-1');
        buttonElement.setAttribute('aria-hidden', 'true');
    }

    const updateState = () => {
        const focused = document.activeElement === inputElement;
        const hovered = wrap.matches(':hover');
        const filled = inputElement.value.length > 0;
        wrap.classList.toggle('is-active', focused || hovered || filled);
        wrap.classList.toggle('has-value', filled);
    };

    updateState();
    ['input', 'focus', 'blur'].forEach(type => {
        inputElement.addEventListener(type, updateState);
    });
    ['mouseenter', 'mouseleave'].forEach(type => {
        wrap.addEventListener(type, updateState);
    });

    wrap.dataset.filterVisualInit = '1';
};

const qsaFromBaseOptional = <T extends Element = Element>(selector: string, base?: ParentNode | null): T[] => {
    return Array.from((base ?? document).querySelectorAll<T>(selector));
};

const queueTextFilter = (inputElement: HTMLInputElement, event?: InputEvent) => {
    if (event?.isComposing || inputElement.dataset.composing === 'true') {
        return;
    }
    const container = inputElement.closest('.container-selectable') as HTMLElement;
    const orderedList = container?.querySelector('.wrapper ol');
    if (!orderedList) {
        return;
    }
    const olId = orderedList.id;
    const value = (inputElement.value || '').trim();
    activeTextFilters[olId] = value;

    const altButton = container!.querySelector("button[id$='-filter-button-alt']");
    if (altButton) {
        altButton.classList.toggle('is-active', !!value);
    }
    if (textFilterTimers[olId]) {
        clearTimeout(textFilterTimers[olId]);
    }

    const textFilterDelaySec = 500;
    textFilterTimers[olId] = setTimeout(() => {
        applyFilters(olId, container);
        delete textFilterTimers[olId];
    }, textFilterDelaySec);
};

const setTextFilter = (inputElement: HTMLInputElement, immediate: boolean) => {
    queueTextFilter(inputElement);
    if (immediate) {
        const container: HTMLElement = inputElement.closest('.container-selectable')!;
        const orderedList = container.querySelector('.wrapper ol');
        if (!orderedList) {
            return;
        }
        const olId = orderedList.id;
        if (textFilterTimers[olId]) {
            clearTimeout(textFilterTimers[olId]);
            delete textFilterTimers[olId];
        }
        applyFilters(olId, container);
        const items = Array.from(container.querySelectorAll(`#${olId} li`)) as HTMLElement[];
        const visibleCount = items.filter(li => {
            if (li.style.display === 'none') return false;
            const cs = getComputedStyle(li);
            return cs.display !== 'none' && cs.visibility !== 'hidden';
        }).length;
        if (visibleCount === 0) {
            hideToastMenu(false);
        }
    }
};

const setupTextFilterInput = (inputElement: HTMLInputElement) => {
    if (inputElement.dataset.filterInit === '1') {
        return;
    }
    inputElement.addEventListener('compositionstart', () => {
        inputElement.dataset.composing = 'true';
    });
    inputElement.addEventListener('compositionend', () => {
        inputElement.dataset.composing = 'false';
        queueTextFilter(inputElement);
    });
    inputElement.addEventListener('input', event => {
        queueTextFilter(inputElement, event as InputEvent);
    });
    inputElement.addEventListener('keydown', event => {
        const key = (event as KeyboardEvent).key;
        if (key === 'Escape') {
            inputElement.value = '';
            setTextFilter(inputElement, true);
            event.stopPropagation();
            event.preventDefault();
        } else if (key === 'Enter') {
            setTextFilter(inputElement, true);
            event.stopPropagation();
            event.preventDefault();
        }
    });
    inputElement.dataset.filterInit = '1';
};

export const initialize = (): void => {
    initializeEditorColumnsFilterInputs();
    initializeFilterVisual();
};
