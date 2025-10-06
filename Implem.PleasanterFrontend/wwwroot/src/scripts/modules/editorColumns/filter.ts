const debounce = <T extends (...args: unknown[]) => void>(
    fn: T,
    delayMs: number
): ((...args: Parameters<T>) => void) => {
    let timerId: ReturnType<typeof setTimeout> | undefined;
    return (...args: Parameters<T>) => {
        if (timerId !== undefined) {
            clearTimeout(timerId);
        }
        timerId = setTimeout(() => fn(...args), delayMs);
    };
};

const getFilterType = (dataValue: string) => {
    const PREFIXES = ['class', 'num', 'date', 'description', 'check', 'attachments', '_links', '_section'];
    const valueLower = (dataValue || '').toLowerCase();
    const matched = PREFIXES.find(prefix => {
        return valueLower.startsWith(prefix);
    });
    return matched || 'basic';
};

const getVisibleLabel = (listItem: HTMLElement) => {
    const rawText = Array.from(listItem.childNodes)
        .filter(node => {
            return node.nodeType === Node.TEXT_NODE;
        })
        .map(node => {
            return node.textContent || '';
        })
        .join(' ')
        .replace(/\s+/g, ' ')
        .trim();

    let resultText = rawText;
    if (!resultText) {
        const clone = listItem.cloneNode(true) as HTMLElement;
        clone.querySelectorAll('.material-symbols-sharp').forEach(iconElement => {
            iconElement.remove();
        });
        resultText = (clone.textContent || '').replace(/\s+/g, ' ').trim();
    }
    return resultText.replace(/^\s*\[[^\]]*]\s*/, '').trim();
};

const observeEditorColumns = (): void => {
    const target = document.getElementById('EditorColumns');
    if (!target) return;
    const observer = new MutationObserver(debounce(() => reapplyAllEditorColumnFilters(), 50));
    observer.observe(target, { childList: true });
};

const reapplyAllEditorColumnFilters = (): void => {
    const allIds = new Set([...Object.keys(activeFilters || {}), ...Object.keys(activeTextFilters || {})]);
    allIds.forEach(olId => {
        applyFilters(olId);
    });
};

export const activeFilters: Record<string, Set<string>> = {};
export const activeTextFilters: Record<string, string> = {};
export const applyFilters = (olId: string, container?: HTMLElement): void => {
    const itemNodeList = container
        ? container.querySelectorAll(`#${olId} li`)
        : document.querySelectorAll(`#${olId} li`);
    const activeTypeSet = activeFilters[olId] ?? new Set();
    const textFilterValue = activeTextFilters[olId] || '';

    (itemNodeList as NodeListOf<HTMLElement>).forEach(listItem => {
        const typeToken = getFilterType(listItem.dataset.value!);
        const label = getVisibleLabel(listItem);
        const matchesType = activeTypeSet.size === 0 || activeTypeSet.has('ALL') || activeTypeSet.has(typeToken);
        const matchesText = String(textFilterValue) === '' || label.includes(String(textFilterValue));
        listItem.style.display = matchesType && matchesText ? '' : 'none';
    });
};

export const clearEditorColumnsFilters = (target: unknown) => {
    let olIds;
    if (typeof target === 'string') {
        olIds = [target];
    } else if (Array.isArray(target)) {
        olIds = target.slice();
    } else {
        olIds = Array.from(new Set([...Object.keys(activeFilters || {}), ...Object.keys(activeTextFilters || {})]));
    }
    const clearedOlIds: string[] = [];
    olIds.forEach(olId => {
        if (!olId) {
            return;
        }
        const orderedList = document.getElementById(olId);
        if (!orderedList) {
            delete activeFilters[olId];
            delete activeTextFilters[olId];
            if (textFilterTimers[olId]) {
                clearTimeout(textFilterTimers[olId]);
                delete textFilterTimers[olId];
            }
            clearedOlIds.push(olId);
            return;
        }
        const container = orderedList.closest('.container-selectable');
        delete activeFilters[olId];
        delete activeTextFilters[olId];
        if (textFilterTimers[olId]) {
            clearTimeout(textFilterTimers[olId]);
            delete textFilterTimers[olId];
        }

        if (container) {
            container
                .querySelectorAll(
                    "[id^='left-editor-columns-filter-button-']:not([id$='-alt']), " +
                        "[id^='right-editor-columns-filter-button-']:not([id$='-alt'])"
                )
                .forEach(buttonElement => {
                    buttonElement.classList.remove('is-active');
                });

            const inputElement = container.querySelector(
                "[id^='left-editor-columns-filter-input-alt'], " + "[id^='right-editor-columns-filter-input-alt']"
            ) as HTMLInputElement;
            if (inputElement) {
                inputElement.value = '';
                inputElement.dataset.composing = 'false';
            }

            const altButton = container.querySelector("button[id$='-filter-button-alt']");
            if (altButton) {
                altButton.classList.remove('is-active');
            }

            const filterWrap = container.querySelector('.editor-columns-filter');
            if (filterWrap) {
                filterWrap.classList.remove('is-active', 'has-value');
            }
        }

        clearedOlIds.push(olId);
    });
    return { clearedOlIds: clearedOlIds };
};

export const initialize = (): void => {
    observeEditorColumns();
};

export const textFilterTimers: Record<string, ReturnType<typeof setTimeout>> = {};
