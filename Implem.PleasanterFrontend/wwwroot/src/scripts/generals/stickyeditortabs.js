$p.applyStickyEditorTabs = function () {
    const html = document.documentElement;
    const nav = document.querySelector(
        'body.sticky-editor-tabs #Editor #EditorTabsContainer > #EditorTabs'
    );
    if ($p.stickyEditorTabsObserver) {
        $p.stickyEditorTabsObserver.disconnect();
        $p.stickyEditorTabsObserver = null;
    }
    $(window).off('resize.stickyEditorTabs');
    if (!nav || getComputedStyle(nav).position !== 'sticky') {
        html.classList.remove('sticky-editor-tabs-active');
        html.style.removeProperty('--sticky-editor-tabs-bottom');
        return;
    }
    const update = function () {
        const top = parseFloat(getComputedStyle(nav).top) || 0;
        html.style.setProperty('--sticky-editor-tabs-bottom', top + nav.offsetHeight + 'px');
    };
    update();
    html.classList.add('sticky-editor-tabs-active');
    $p.stickyEditorTabsObserver = new ResizeObserver(update);
    $p.stickyEditorTabsObserver.observe(nav);
    $(window).on('resize.stickyEditorTabs', update);
};

$p.scrollToStickyEditorTabs = function ($container) {
    if (
        !$container.is('#Editor #EditorTabsContainer') ||
        !document.documentElement.classList.contains('sticky-editor-tabs-active')
    ) {
        return;
    }
    const container = $container.get(0);
    const nav = $container.children('#EditorTabs').get(0);
    const top = parseFloat(getComputedStyle(nav).top) || 0;
    const containerTop = container.getBoundingClientRect().top;
    if (containerTop < top) {
        window.scrollTo(window.scrollX, window.scrollY + containerTop - top);
    }
};
