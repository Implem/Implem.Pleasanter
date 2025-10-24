document.addEventListener('DOMContentLoaded', () => {
    const buttons = document.querySelectorAll<HTMLButtonElement>('.display-none-btn');

    buttons.forEach(button => {
        button.addEventListener('click', () => {
            const parent = button.closest<HTMLElement>('[id]');
            if (parent) {
                parent.style.display = 'none';
            }
        });
    });
});
