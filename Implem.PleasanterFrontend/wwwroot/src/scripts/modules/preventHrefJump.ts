const preventClasses = ['void-zero', 'menulabel', 'calendar-to-monthly'];

document.addEventListener('DOMContentLoaded', () => {
    document.addEventListener('click', (event: MouseEvent) => {
        const target = event.target;
        if (!(target instanceof Element)) {
            return;
        }
        const link = target.closest<HTMLAnchorElement>('a[href="#"]');
        if (!link) {
            return;
        }
        const isPreventClass = preventClasses.some(cls => link.classList.contains(cls));
        if (isPreventClass) {
            event.preventDefault();
        }
    });
});
