export const initialize = (): void => {
    const setupButtonEvent = (buttonId: string, handler: (event: Event, target: HTMLElement) => void) => {
        const target = document.getElementById(buttonId);
        if (!target) {
            return;
        }
        target.addEventListener('click', event => handler(event, target));
    };

    setupButtonEvent('ToEnableEditorColumns', (event, target) => {
        $p.enableColumns(event, $(target), 'Editor', 'EditorSourceColumnsType');
    });

    setupButtonEvent('ToDisableEditorColumns', (_, target) => {
        $p.send($(target));
    });
};
