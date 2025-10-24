import { applyFilters, activeFilters } from './filter.ts';

const initializeEditorColumnsFilterButtons = (root?: HTMLElement) => {
    const base = root || document;
    const selector = [
        "[id^='left-editor-columns-filter-button-']:not([id$='-alt'])",
        "[id^='right-editor-columns-filter-button-']:not([id$='-alt'])"
    ].join(', ');

    (qsaFromBase(selector, base) as HTMLElement[]).forEach(buttonElement => {
        if (buttonElement.dataset.filterInit === '1') {
            return;
        }
        buttonElement.addEventListener('click', () => {
            toggleFilter(buttonElement);
        });
        buttonElement.dataset.filterInit = '1';
    });
};

const qsaFromBase = <T extends Element = Element>(selector: string, base: ParentNode): T[] => {
    return Array.from(base.querySelectorAll<T>(selector));
};

const toggleFilter = (buttonElement: HTMLElement) => {
    if (!buttonElement) {
        return;
    }
    const idParts = buttonElement.id.split('-');
    const filterType = idParts[idParts.length - 1];
    const container = buttonElement.closest('.container-selectable') as HTMLElement;
    const orderedList = container?.querySelector('.wrapper ol');
    if (!orderedList) {
        return;
    }

    const olId = orderedList.id;
    activeFilters[olId] ??= new Set();
    const activeSet = activeFilters[olId];
    if (activeSet.has(filterType)) {
        activeSet.delete(filterType);
        buttonElement.classList.remove('is-active');
    } else {
        activeSet.add(filterType);
        buttonElement.classList.add('is-active');
    }

    applyFilters(olId, container);
};

export const initialize = (): void => {
    initializeEditorColumnsFilterButtons();
};
