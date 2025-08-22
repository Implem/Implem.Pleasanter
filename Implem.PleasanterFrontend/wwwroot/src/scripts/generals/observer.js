$p.pageObserve = function (selector) {
    const observerTarget = document.createElement('div');
    const observerTargetName = `${selector}Observer`;
    const contents = document.getElementById(selector);
    const mutationObserver = new MutationObserver(() => {
        observerTarget.style.width = 'auto';
        observerTarget.style.width = `${contents.scrollWidth}px`;
        if (!document.querySelector(`#${selector}`)) {
            intersectionObserver.disconnect();
            mutationObserver.disconnect();
        }
    });
    const intersectionObserver = new IntersectionObserver(entries => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                $p.paging(`#${selector}`);
            }
        });
    });

    let resizeTimeID;
    const resizeObserver = new ResizeObserver(() => {
        clearTimeout(resizeTimeID);
        resizeTimeID = setTimeout(() => {
            observerTarget.style.width = 'auto';
            observerTarget.style.width = `${contents.scrollWidth}px`;
        }, 200);
    });

    if (contents && contents.parentNode) {
        observerTarget.setAttribute('id', observerTargetName);
        observerTarget.style.width = `${contents.scrollWidth}px`;
        observerTarget.style.height = `1px`;
        contents.parentNode.insertBefore(observerTarget, contents.nextSibling);
        intersectionObserver.observe(observerTarget);
        mutationObserver.observe(document.getElementById('ViewModeContainer'), {
            attributes: false,
            childList: true,
            subtree: true
        });
        resizeObserver.observe(contents);
    }
};
