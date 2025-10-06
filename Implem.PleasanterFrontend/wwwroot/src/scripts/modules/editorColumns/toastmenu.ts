type EditorColumnsDownLi = {
    type: 'li';
    li: HTMLLIElement;
    x: number;
    y: number;
    time: number;
    scrollTop: number;
    ctrlOrMeta?: boolean;
    wasSelected?: boolean;
};

type EditorColumnsDownOutside = {
    type: 'outside';
    x: number;
    y: number;
    time: number;
    scrollTop: number;
};

type EditorColumnsState = {
    stableBound: boolean;
    down: EditorColumnsDownLi | EditorColumnsDownOutside | null;
};

type FilterState = {
    perTarget: {
        [id: string]: {
            activeTypes: Set<string>;
        };
    };
};

type TextFilterState = {
    perTarget: {
        [id: string]: {
            hasQuery: boolean;
        };
    };
};

type ToastFollow = {
    update: () => void;
    destroy: () => void;
};

type ToastMenuEl = HTMLElement & { toastFollow?: ToastFollow };

let ecState: EditorColumnsState | null = null;
let filterState: FilterState | null = null;
let lastClickedEditorLi: HTMLLIElement | null = null;
let textFilterState: TextFilterState | null = null;

if (ecState == null) ecState = { stableBound: false, down: null };
if (filterState == null) filterState = { perTarget: {} };
if (lastClickedEditorLi == null) lastClickedEditorLi = null;
if (textFilterState == null) textFilterState = { perTarget: {} };

const bindEditorColumnsClickStable = (): void => {
    if (ecState!.stableBound) return;
    ecState!.stableBound = true;

    const THRESHOLD_MS = 500;
    const THRESHOLD_MOVE = 8;
    const THRESHOLD_SCROLL = 2;

    const isClickLike = (
        down: NonNullable<EditorColumnsState['down']>,
        up: PointerEvent | MouseEvent,
        wrapper: HTMLElement | null
    ): boolean =>
        performance.now() - down.time <= THRESHOLD_MS &&
        Math.hypot(up.clientX - down.x, up.clientY - down.y) <= THRESHOLD_MOVE &&
        Math.abs((wrapper?.scrollTop || 0) - down.scrollTop) <= THRESHOLD_SCROLL;

    const onPointerDown = (e: PointerEvent | MouseEvent): void => {
        if (e.button !== 0 || isInToastMenu(e.target as HTMLElement)) return;
        const li = (e.target as HTMLElement)?.closest?.('#EditorColumns li') as HTMLLIElement | null;
        const wrapper = getWrapper();
        const isInToastButton = (el: HTMLElement | null): boolean =>
            el?.closest(".toast-menu .button-icon, .toast-menu button, .toast-menu [role='button']") !== null;
        if (li && !isInToastButton(e.target as HTMLElement)) {
            const me = e as MouseEvent;
            ecState!.down = {
                type: 'li',
                li,
                x: me.clientX,
                y: me.clientY,
                time: performance.now(),
                scrollTop: wrapper?.scrollTop || 0,
                ctrlOrMeta: me.ctrlKey || me.metaKey,
                wasSelected: isLiSelected(li)
            };
        } else {
            const me = e as MouseEvent;
            ecState!.down = {
                type: 'outside',
                x: me.clientX,
                y: me.clientY,
                time: performance.now(),
                scrollTop: wrapper?.scrollTop || 0
            };
        }
    };

    const onPointerUp = (e: PointerEvent | MouseEvent): void => {
        const down = ecState!.down;
        if (!down || isInToastMenu(e.target as HTMLElement)) return;
        const wrapper = getWrapper();
        if (isClickLike(down, e, wrapper)) {
            if (down.type === 'li') {
                lastClickedEditorLi = down.li;
                if (down.ctrlOrMeta) {
                    setTimeout(() => {
                        if (isLiSelected(down.li) && !down.wasSelected) {
                            showToastMenu();
                        } else {
                            hideToastMenu();
                        }
                    }, 0);
                } else {
                    showToastMenu();
                }
            } else if (isToastMenuOpen()) {
                hideToastMenu();
            }
        }
        ecState!.down = null;
    };

    if ('PointerEvent' in window) {
        document.addEventListener('pointerdown', onPointerDown as EventListener, true);
        document.addEventListener('pointerup', onPointerUp as EventListener, true);
        document.addEventListener(
            'pointercancel',
            () => {
                ecState!.down = null;
            },
            true
        );
    } else {
        document.addEventListener('mousedown', onPointerDown as EventListener, true);
        document.addEventListener('mouseup', onPointerUp as EventListener, true);
    }
};

const getElement = (id: string): HTMLElement | null => document.getElementById(id);

const getFilterTargetState = (id: string) => {
    if (!filterState!.perTarget[id]) {
        filterState!.perTarget[id] = { activeTypes: new Set<string>() };
    }
    const state = filterState!.perTarget[id];
    if (!(state.activeTypes instanceof Set)) {
        const prev = state.activeTypes as unknown;
        if (typeof prev === 'string') {
            state.activeTypes = new Set([prev]);
        } else {
            state.activeTypes = new Set();
        }
    }
    return state;
};

const getTextFilterTargetState = (id: string) => {
    if (!textFilterState!.perTarget[id]) {
        textFilterState!.perTarget[id] = { hasQuery: false };
    }
    return textFilterState!.perTarget[id];
};

const getWrapper = (): HTMLElement | null => getElement('EditorColumnsWrapper');

const isHiddenByClassOrAttr = (el: HTMLElement | null): boolean => {
    if (!el) return false;
    if (
        el.hidden ||
        el.getAttribute('aria-hidden') === 'true' ||
        el.classList.contains('is-text-hidden') ||
        el.classList.contains('is-type-hidden') ||
        el.classList.contains('is-hidden') ||
        el.classList.contains('filtered-out')
    ) {
        return true;
    }
    const cs = getComputedStyle(el);
    return cs.display === 'none' || cs.visibility === 'hidden';
};

const isInToastMenu = (el: HTMLElement | null): boolean => !!el?.closest?.('.toast-menu');

const isLiSelected = (li: HTMLElement | null): boolean =>
    !!li &&
    (li.classList.contains('ui-selected') ||
        li.classList.contains('selected') ||
        li.classList.contains('is-selected') ||
        li.getAttribute('aria-selected') === 'true');

const isListFiltered = (listId?: string): boolean => {
    const id = listId || 'EditorColumns';
    const list: HTMLElement | null = id ? getElement(id) : null;
    if (!list) return false;

    const typeFiltered = !!getFilterTargetState(id).activeTypes.size;
    let textFiltered = !!getTextFilterTargetState(id).hasQuery;

    const items = Array.from(list.querySelectorAll('li'));
    const total = items.length;
    const isLiHiddenByFilter = isHiddenByClassOrAttr;
    const visible = items.filter(li => !isLiHiddenByFilter(li as HTMLElement)).length;
    const visibilityFiltered = total > 0 && visible < total;

    if (!textFiltered && !visibilityFiltered) {
        textFiltered = !!list.querySelector('li.is-text-hidden');
    }
    return typeFiltered || textFiltered || visibilityFiltered;
};

const isToastMenuOpen = (): boolean => {
    const menu = getMenu();
    if (!menu) return false;
    const cs = getComputedStyle(menu);
    return menu.classList.contains('show') && cs.display !== 'none' && parseFloat(cs.opacity) > 0;
};

const setupToastFollow = (wrapper: HTMLElement | null, menu: ToastMenuEl | null): void => {
    if (!wrapper || !menu) return;
    if (menu.toastFollow) {
        return menu.toastFollow.update();
    }

    const update = () => {
        const visibleBottom = wrapper.scrollTop + wrapper.clientHeight;
        const top = visibleBottom - menu.offsetHeight;
        const maxTop = Math.max(0, wrapper.scrollHeight - menu.offsetHeight);
        menu.style.top = `${Math.max(0, Math.min(top, maxTop))}px`;
        menu.style.bottom = 'auto';
    };
    update();

    const onScroll = () => update();
    const onResize = () => update();

    wrapper.addEventListener('scroll', onScroll, { passive: true });
    window.addEventListener('resize', onResize);

    let roMenu: ResizeObserver | undefined;
    let roWrap: ResizeObserver | undefined;
    if ('ResizeObserver' in window) {
        roMenu = new ResizeObserver(update);
        roWrap = new ResizeObserver(update);
        roMenu.observe(menu);
        roWrap.observe(wrapper);
    }

    menu.toastFollow = {
        update,
        destroy: () => {
            wrapper.removeEventListener('scroll', onScroll);
            window.removeEventListener('resize', onResize);
            roMenu?.disconnect();
            roWrap?.disconnect();
            delete menu.toastFollow;
        }
    };
};

const showToastMenu = (): void => {
    const wrapper = getWrapper();
    const menu = getMenu();
    if (!wrapper || !menu) return;

    wrapper.classList.add('toast-host');
    if (menu.parentElement !== wrapper) {
        wrapper.appendChild(menu);
    }

    menu.style.setProperty('display', 'block', 'important');
    requestAnimationFrame(() => {
        menu.classList.add('show');
    });
    menu.removeAttribute('hidden');
    menu.setAttribute('aria-hidden', 'false');

    updateToastMenuControlsVisibility('EditorColumns');
    setupToastFollow(wrapper, menu);
};

const toggle = (el: HTMLElement | null, hide: boolean): void => {
    if (!el) return;
    el.hidden = hide;
    el.setAttribute('aria-hidden', String(hide));
    if (hide) {
        el.style.setProperty('display', 'none', 'important');
    } else {
        el.style.removeProperty('display');
    }
};

const updateToastMenuControlsVisibility = (listId?: string): void => {
    const id = listId || 'EditorColumns';
    const menu = getMenu();
    if (!menu) return;

    const btn = (sel: string): HTMLElement | null => menu.querySelector(sel);

    const moveUp = btn('#MoveUpEditorColumns');
    const moveDown = btn('#MoveDownEditorColumns');
    if (!moveUp || !moveDown) return;

    const hideMove = isListFiltered(id);
    [moveUp, moveDown].forEach(b => toggle(b, hideMove));

    const list = getElement(id);
    const selected = list?.querySelectorAll('li.ui-selected') || [];
    const settingsBtn = btn('#OpenEditorColumnDialog');
    const resetBtn = btn('#EditorColumnsResetAndDisable');

    if (selected.length > 1) {
        [settingsBtn, resetBtn].forEach(b => toggle(b, true));
        return;
    }

    const value = lastClickedEditorLi?.getAttribute('data-value') || '';
    toggle(settingsBtn, /^_Links-/.test(value));
    toggle(resetBtn, /^_Section-|^_Links-/.test(value));
};

export const getMenu = (): ToastMenuEl | null => getElement('editor-columns-toast-menu') as ToastMenuEl | null;

export const hideToastMenu = (useDelay: boolean = true): void => {
    const menu = getMenu();
    if (!menu) return;
    menu.toastFollow?.destroy();
    menu.classList.remove('show');
    const delay = useDelay ? 500 : 0;

    setTimeout(() => {
        if (menu.contains(document.activeElement)) {
            const wrapper = getWrapper();
            let fallback: HTMLElement | null = wrapper;
            if (fallback) {
                if (!fallback.hasAttribute('tabindex')) {
                    fallback.setAttribute('tabindex', '-1');
                }
            } else {
                fallback = document.body;
            }
            try {
                fallback.focus({ preventScroll: true });
            } catch (e) {
                console.error('Error in Hide ToastMenu:', e);
            }
        }
        menu.style.display = 'none';
        menu.style.removeProperty('top');
        menu.style.removeProperty('left');
        menu.style.removeProperty('right');
        menu.style.removeProperty('bottom');
        menu.style.removeProperty('transform');
        menu.setAttribute('aria-hidden', 'true');
    }, delay);
};

export const initialize = (): void => {
    bindEditorColumnsClickStable();
    updateToastMenuControlsVisibility('EditorColumns');
};
