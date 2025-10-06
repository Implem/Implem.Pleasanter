import { getMenu, hideToastMenu } from './toastmenu.ts';

let loadingHold = 0;

const hideEditorColumnsToast = (): void => {
    const menu = getMenu();
    if (menu) {
        menu.classList.remove('show');
        menu.setAttribute('aria-hidden', 'true');
        menu.style.setProperty('display', 'none', 'important');
    }
};

const holdLoading = (): void => {
    loadingHold++;
    if (typeof $p.loading === 'function') {
        $p.loading();
    }
};

const initializeToastMenuButtons = () => {
    const moveHandler = (event: Event, element: HTMLElement) => {
        $p.moveColumns(event, $(element), 'Editor');
    };

    const openHandler = (_event: Event, element: HTMLElement) => {
        $p.openEditorColumnDialog($(element));
    };

    const resetAndDisableColumn = () => {
        if (!confirm($p.display('ConfirmResetAndDisable'))) {
            return;
        }
        hideToastMenu(false);
        const FAIL_TIMEOUT_MS = 30000;
        const failTimer = setTimeout(() => {
            hideEditorColumnsToast();
            releaseLoading();
        }, FAIL_TIMEOUT_MS);

        $(document).one('dialogopen', '#EditorColumnDialog', function () {
            clearTimeout(failTimer);
            const $dialog = $(this);
            const $frame = $dialog.closest('.ui-dialog');
            const $overlay = $('.ui-widget-overlay').last();
            $frame.css('visibility', 'hidden');
            $overlay.css('display', 'none');
            try {
                const $resetButton = $('#ResetEditorColumn');
                const prevConfirm = $resetButton.attr('data-confirm');
                if (prevConfirm) {
                    $resetButton.removeAttr('data-confirm');
                }
                $p.resetEditorColumn($resetButton);
                if (prevConfirm) {
                    $resetButton.attr('data-confirm', prevConfirm);
                }
                const $apply = $('#SetEditorColumn');
                const error = typeof $p.syncSend === 'function' ? $p.syncSend($apply) : 0;
                if (error === 0) {
                    const $disable = $('#ToDisableEditorColumns');
                    if ($disable.length) {
                        $p.syncSend($disable);
                    }
                }
            } finally {
                $dialog.dialog('close');
                $frame.css('visibility', '');
                $overlay.css('display', '');
                hideEditorColumnsToast();
                releaseLoading();
            }
        });

        $p.loading();
        $p.openEditorColumnDialog($('#OpenEditorColumnDialog'));
        holdLoading();
    };

    const handlers = {
        MoveUpEditorColumns: moveHandler,
        MoveDownEditorColumns: moveHandler,
        OpenEditorColumnDialog: openHandler,
        EditorColumnsResetAndDisable: resetAndDisableColumn
    };

    Object.entries(handlers).forEach(([id, handler]) => {
        const target = document.getElementById(id);
        if (!target) {
            return;
        }
        target.addEventListener('click', event => {
            handler(event, target);
        });
    });
};

const releaseLoading = (): void => {
    loadingHold = Math.max(0, loadingHold - 1);
    if (loadingHold === 0) {
        $p.loaded();
    }
};

export const initialize = (): void => {
    initializeToastMenuButtons();
};
