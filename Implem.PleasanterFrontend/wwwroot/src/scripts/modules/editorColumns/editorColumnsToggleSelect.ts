declare const jQuery: typeof import('jquery');

export const initialize = (): void => {
    setupToggleSelectable('EditorColumns');
    setupToggleSelectable('EditorSourceColumns');
};

const setupToggleSelectable = (listId: string): void => {
    const list = document.getElementById(listId);
    if (!list) {
        return;
    }
    const $list = jQuery(list);
    $list.selectable({
        cancel: 'li'
    });

    list.addEventListener('click', ev => {
        if (ev.button !== 0) {
            return;
        }

        const target = ev.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        const li = target.closest<HTMLLIElement>('li');
        if (!li || !list.contains(li)) {
            return;
        }

        if (!li.classList.contains('ui-selectee')) {
            li.classList.add('ui-selectee');
        }

        if (!ev.ctrlKey && !ev.metaKey) {
            list.querySelectorAll('li.ui-selected').forEach(el => {
                if (el !== li) {
                    el.classList.remove('ui-selected');
                }
            });
        }

        li.classList.toggle('ui-selected');
    });
};
