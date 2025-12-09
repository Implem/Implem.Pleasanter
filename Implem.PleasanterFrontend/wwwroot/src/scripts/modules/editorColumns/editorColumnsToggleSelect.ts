declare const jQuery: typeof import('jquery');

export const initialize = (): void => {
    setupToggleSelectable('EditorColumns');
    setupToggleSelectable('EditorSourceColumns');
};

const setupToggleSelectable = (listId: string): void => {
    const list = document.getElementById(listId);
    if (!list) return;

    const $list = jQuery(list);

    let ctrlOrMetaOnStart = false;
    let isDragging = false;
    let dragStartTime = 0;
    let mouseDownTarget: HTMLLIElement | null = null;

    const clearOtherSelections = (except?: HTMLElement) => {
        list.querySelectorAll('li.ui-selected').forEach(el => {
            if (el !== except) {
                el.classList.remove('ui-selected');
            }
        });
    };

    const isDragOperation = (li: HTMLElement | null): boolean => {
        const clickTime = Date.now();
        return isDragging || (clickTime - dragStartTime > 200 && mouseDownTarget !== li);
    };

    const ensureSelecteeClass = (li: HTMLElement) => {
        if (!li.classList.contains('ui-selectee')) {
            li.classList.add('ui-selectee');
        }
    };

    $list.on('mousedown', (ev: JQuery.MouseDownEvent) => {
        if (ev.button !== 0) return;

        isDragging = false;
        dragStartTime = Date.now();
        ctrlOrMetaOnStart = !!(ev.ctrlKey || ev.metaKey);

        const li = (ev.target as HTMLElement)?.closest?.('li') || null;
        mouseDownTarget = li;

        if (!ctrlOrMetaOnStart && li) {
            clearOtherSelections(li);
        }
    });

    $list.on('mousemove', (ev: JQuery.MouseMoveEvent) => {
        if (ev.buttons === 1) {
            isDragging = true;
        }
    });

    $list.on('mouseup', () => {
        setTimeout(() => (isDragging = false), 50);
    });

    $list.on('click', (ev: JQuery.ClickEvent) => {
        if (ev.button !== 0) return;

        const li = (ev.target as HTMLElement)?.closest?.('li');
        if (!li || !list.contains(li)) return;

        if (isDragOperation(li)) {
            mouseDownTarget = null;
            return;
        }

        ev.stopImmediatePropagation();
        ev.preventDefault();

        ensureSelecteeClass(li);

        if (!ev.ctrlKey && !ev.metaKey) {
            clearOtherSelections(li);
        }

        li.classList.toggle('ui-selected');
        mouseDownTarget = null;
    });

    // ---- jQuery UI selectable ----
    $list.selectable({
        filter: 'li',
        cancel: "button, input, a, [role='button'], .button-icon",
        tolerance: 'touch',
        delay: 150,
        distance: 5,

        selecting: (_ev, ui) => {
            ensureSelecteeClass(ui.selecting as HTMLElement);
        },
        selected: (_ev, ui) => {
            (ui.selected as HTMLElement).classList.add('ui-selected');
        },
        unselected: (_ev, ui) => {
            (ui.unselected as HTMLElement).classList.remove('ui-selected');
        },
        stop: () => {
            mouseDownTarget = null;
        }
    });
};
