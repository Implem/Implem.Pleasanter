import { clearEditorColumnsFilters } from './filter.ts';

const registerDropdownClear = (dropdownSelector: string) => {
    const prefix = dropdownSelector === '#EditorColumnsTabs' ? 'left' : 'right';
    const collectOlIdsForPrefix = () => {
        const olIdSet = new Set();
        const selector = `button[id^='${prefix}-editor-columns-filter-button-']`;
        (qsa(selector) as HTMLElement[]).forEach(buttonElement => {
            if (buttonElement.id.endsWith('-alt')) {
                return;
            }
            const orderedList = buttonElement.closest('.container-selectable')?.querySelector('.wrapper ol');
            if (orderedList) {
                olIdSet.add(orderedList.id);
            }
        });
        return Array.from(olIdSet);
    };
    const handleDropdownChange = () => {
        const targetOlIds = collectOlIdsForPrefix();
        if (targetOlIds.length > 0) {
            clearEditorColumnsFilters(targetOlIds);
        }
    };
    qsa(dropdownSelector).forEach(dropdownElement => {
        dropdownElement.addEventListener('change', handleDropdownChange);
    });
};

const qsa = <T extends Element = Element>(selector: string): T[] => {
    return Array.from(document.querySelectorAll<T>(selector));
};

export const initialize = (): void => {
    registerDropdownClear('#EditorColumnsTabs');
    registerDropdownClear('#EditorSourceColumnsType');
};
