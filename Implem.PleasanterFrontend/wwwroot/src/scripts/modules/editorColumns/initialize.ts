const onDomReady = (callback: () => void): void => {
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', callback, { once: true });
    } else {
        callback();
    }
};

onDomReady(() => {
    const isThemeVersionOver2 = document.getElementById('ThemeVersionOver2');
    if (isThemeVersionOver2) {
        import('./filter').then(module => {
            module.initialize();
        });
        import('./filterbutton').then(module => {
            module.initialize();
        });
        import('./filterinput').then(module => {
            module.initialize();
        });
        import('./dropdown').then(module => {
            module.initialize();
        });
        import('./enabledisablebuttons').then(module => {
            module.initialize();
        });
        import('./toastmenu').then(module => {
            module.initialize();
        });
        import('./toastmenubuttons').then(module => {
            module.initialize();
        });
        import('./popup').then(module => {
            module.initialize();
        });
    }
});
