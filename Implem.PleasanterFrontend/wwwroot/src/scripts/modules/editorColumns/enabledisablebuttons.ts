import { hideToastMenu, normalizeLiSelecteeOnce } from './toastmenu.ts';

type SetMustDataFn = ($form: JQuery, action: string) => unknown;
type GetFormIdFn = ($control: JQuery) => string;

const handleEnableEditorColumns = (event: Event, target: HTMLElement): void => {
    const sourceList = document.getElementById('EditorSourceColumns');
    if (!sourceList) {
        $p.enableColumns(event, $(target), 'Editor', 'EditorSourceColumnsType');
        normalizeLiSelecteeOnce();
        return;
    }
    const selectedValues: string[] = [];
    sourceList.querySelectorAll('li.ui-selected').forEach(li => {
        const value = (li as HTMLElement).dataset.value || (li as HTMLElement).getAttribute('title');
        if (value) {
            selectedValues.push(value);
        }
    });
    if (selectedValues.length === 0) {
        $p.enableColumns(event, $(target), 'Editor', 'EditorSourceColumnsType');
        normalizeLiSelecteeOnce();
        return;
    }
    const pAny = $p as unknown as Record<string, unknown>;
    const originalSetMustData = pAny.setMustData as SetMustDataFn;
    pAny.setMustData = function ($form: JQuery, action: string): unknown {
        try {
            const result = originalSetMustData.call(pAny, $form, action);
            const getFormId = pAny.getFormId as GetFormIdFn | undefined;
            if (getFormId && pAny.data) {
                const formId = getFormId($form);
                const dataStore = pAny.data as Record<string, Record<string, unknown>>;
                if (dataStore[formId]) {
                    dataStore[formId].EditorSourceColumns = JSON.stringify(selectedValues);
                }
            }
            return result;
        } finally {
            pAny.setMustData = originalSetMustData;
        }
    };
    $p.enableColumns(event, $(target), 'Editor', 'EditorSourceColumnsType');
    normalizeLiSelecteeOnce();
};

export const initialize = (): void => {
    const setupButtonEvent = (buttonId: string, handler: (event: Event, target: HTMLElement) => void) => {
        const target = document.getElementById(buttonId);
        if (!target) {
            return;
        }
        target.addEventListener('click', event => handler(event, target), true);
    };

    setupButtonEvent('ToEnableEditorColumns', handleEnableEditorColumns);

    setupButtonEvent('ToDisableEditorColumns', (_, target) => {
        hideToastMenu();
        $p.send($(target));
    });
};
