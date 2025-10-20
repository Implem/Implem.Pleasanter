type CaptionsLanguage = {
    ja: string[];
    en: string[];
};

type PopupImageConfig = {
    captions: CaptionsLanguage;
};

const PopupController = class PopupController {
    private root: typeof document | HTMLElement;
    private currentButton?: HTMLButtonElement;
    private showTimer?: NodeJS.Timeout;
    private hideTimer?: NodeJS.Timeout;
    private isVisible: boolean;
    private popups = new WeakMap();

    constructor(root: typeof document | HTMLElement | undefined) {
        this.root = root || document;
        this.isVisible = false;
        this.popups = new WeakMap();
        this.init();
    }

    init() {
        const groups: HTMLElement[] = Array.from(this.root.querySelectorAll(POPUP_CONFIG.selectors.group));
        groups.forEach(groupElement => {
            const popupElement = this.ensurePopup(groupElement);
            this.attachGroupEvents(groupElement, popupElement);
        });
        document.addEventListener('mousedown', event => this.onDocumentMouseDown(event));
        document.addEventListener(
            'scroll',
            event => {
                if (this.isScrollOrWheelExcluded(event.target as HTMLElement)) {
                    return;
                }
                this.onScrollOrWheel();
            },
            true
        );
    }

    isScrollOrWheelExcluded(target: HTMLElement) {
        if (target instanceof Element) {
            return target.closest('.popup-body');
        }
        return false;
    }

    onScrollOrWheel() {
        if (this.isVisible || this.showTimer) {
            this.hideAll();
        }
    }

    ensurePopup(groupElement: Element) {
        let popupElement = groupElement.querySelector('.popup') as HTMLElement;
        if (!popupElement) {
            popupElement = document.createElement('div');
            popupElement.className = 'popup';
            popupElement.setAttribute('aria-hidden', 'true');
            popupElement.style.display = 'none';
            groupElement.appendChild(popupElement);
        }
        this.popups.set(groupElement, popupElement);
        return popupElement;
    }

    attachGroupEvents(groupElement: HTMLElement, popupElement: HTMLElement) {
        const buttons: HTMLButtonElement[] = Array.from(groupElement.querySelectorAll(POPUP_CONFIG.selectors.iconBtn));
        buttons.forEach(buttonElement => {
            buttonElement.addEventListener('mouseenter', () =>
                this.scheduleShow(buttonElement, groupElement, popupElement)
            );
            buttonElement.addEventListener('mouseleave', event => this.onButtonMouseLeave(event, popupElement));
        });
        popupElement.addEventListener('mouseenter', () => this.clearHideTimer());
        popupElement.addEventListener('mouseleave', event => {
            const related = event.relatedTarget as HTMLElement;
            if (!related || !groupElement.contains(related)) {
                this.scheduleHide();
            }
        });
    }

    scheduleShow(buttonElement: HTMLButtonElement, groupElement: HTMLElement, popupElement: HTMLElement) {
        this.clearTimers();
        this.showTimer = setTimeout(() => {
            this.show(buttonElement, groupElement, popupElement);
        }, POPUP_CONFIG.popup.showDelay);
    }

    scheduleHide() {
        this.clearHideTimer();
        this.hideTimer = setTimeout(() => {
            this.hideAll();
        }, POPUP_CONFIG.popup.hideDelay);
    }

    clearTimers() {
        this.clearShowTimer();
        this.clearHideTimer();
    }

    clearShowTimer() {
        if (this.showTimer) {
            clearTimeout(this.showTimer);
            this.showTimer = undefined;
        }
    }

    clearHideTimer() {
        if (this.hideTimer) {
            clearTimeout(this.hideTimer);
            this.hideTimer = undefined;
        }
    }

    onButtonMouseLeave(event: MouseEvent, popupElement: HTMLElement) {
        this.clearShowTimer();
        if (!popupElement || !popupElement.contains(event.relatedTarget as Node)) {
            this.scheduleHide();
        }
    }

    onDocumentMouseDown(event: MouseEvent) {
        if (!this.isVisible) {
            return;
        }
        const anyPopup = Array.from(document.querySelectorAll('.popup'));
        const clickedInsidePopup = anyPopup.some(popupElement => popupElement.contains(event.target as Node));
        if (clickedInsidePopup) {
            return;
        }
        if (this.currentButton && this.currentButton.contains(event.target as Node)) {
            return;
        }
        this.hideAll();
    }

    show(buttonElement: HTMLButtonElement, groupElement: HTMLElement, popupElement: HTMLElement) {
        this.hideAll();
        this.currentButton = buttonElement;

        const dataType = getTypeFromControl(buttonElement) || '';
        if (!dataType) {
            return;
        }

        popupElement.innerHTML = buildPopupContents(dataType);
        popupElement.setAttribute('role', POPUP_CONFIG.accessibility.role);
        popupElement.setAttribute('aria-hidden', 'false');
        popupElement.style.position = 'fixed';
        popupElement.style.display = 'block';
        popupElement.classList.add(POPUP_CONFIG.classes.popupVisible);

        const groupRect = groupElement.getBoundingClientRect();
        const buttonRect = buttonElement.getBoundingClientRect();
        const popupLeft = Math.round(groupRect.left);
        const popupTop = Math.round(buttonRect.bottom + POPUP_CONFIG.popup.yOffset);
        popupElement.style.left = `${popupLeft}px`;
        popupElement.style.top = `${popupTop}px`;

        const popupRect = popupElement.getBoundingClientRect();
        const arrowCenterX = buttonRect.left + buttonRect.width / 2 - popupLeft;
        const MIN_LEFT = 4;
        const maxLeft = Math.max(MIN_LEFT, Math.floor(popupRect.width - 24));
        const arrowLeft = Math.max(MIN_LEFT, Math.min(Math.round(arrowCenterX - 10), maxLeft));
        popupElement.style.setProperty('--arrow-left', `${arrowLeft}px`);
        this.isVisible = true;
    }

    hideAll() {
        (Array.from(document.querySelectorAll('.popup')) as HTMLElement[]).forEach(popupElement => {
            popupElement.classList.remove(POPUP_CONFIG.classes.popupVisible);
            popupElement.style.display = 'none';
            popupElement.setAttribute('aria-hidden', 'true');
        });
        this.isVisible = false;
        this.currentButton = undefined;
        this.clearTimers();
    }
};

const POPUP_CONFIG = {
    popup: {
        showDelay: 700,
        hideDelay: 1500,
        yOffset: 10
    },
    classes: {
        popupVisible: 'show'
    },
    selectors: {
        group: '.command-left, .command-right',
        iconBtn: '.button-icon'
    },
    accessibility: {
        role: 'popup'
    }
};

const POPUP_IMAGE_MAP: Record<string, PopupImageConfig> = {
    basic: {
        captions: {
            ja: [
                `ID:レコードのID (読取専用)`,
                `バージョン:レコードのバージョン (読取専用)`,
                `タイトル:レコードのタイトル`,
                `内容:レコードの内容`,
                `状況:レコードのステータス`,
                `管理者:レコードの管理者`,
                `担当者:レコードの担当者`,
                `コメント:レコードのコメント`,
                `ロック:レコードのロック`,
                `開始:レコードの開始日時
(期限付きテーブルのみ)`,
                `完了:レコードの完了日時
(期限付きテーブルのみ)`,
                `作業量:レコードの作業量
(期限付きテーブルのみ)`,
                `進捗率:レコードの進捗率
(期限付きテーブルのみ)`,
                `残作業量:「作業量」と「進捗率」から計算して表示
(期限付きテーブルのみ、読取専用)`
            ],
            en: [
                `ID:ID of Record (Read-only)`,
                `Version:Version of Record (Read-only)`,
                `Title:Title of Record`,
                `Body:Body of Record`,
                `Status:Status of Record`,
                `Manager:Manager of Record`,
                `Owner:Owner of Record`,
                `Comment:Comment of Record`,
                `Lock:Lock of Record`,
                `Start:Start Date and Time of Record
(time-limited tables only)`,
                `Complete:completion date and time of Record
(time-limited tables only)`,
                `Work volume:Work volume of Record
(time-limited tables only)`,
                `Progression Rate:Progression Rate of Record
(time-limited tables only)`,
                `Remaining Work Volume:「Work volume」and「Progression Rate」Calculate and display
(time-limited tables only,Read-only)`
            ]
        }
    },
    class: {
        captions: {
            ja: [`テキスト入力`, `ラジオボタン`, `ドロップダウンリスト`],
            en: [`Text Input`, `Radio Button`, `Dropdown List`]
        }
    },
    num: {
        captions: {
            ja: [`標準`, `通貨`, `桁区切り`, `スピナー`, `単位表示`],
            en: [`Standard`, `Currency`, `Digit Separator`, `Spinner`, `Unit Display`]
        }
    },
    date: {
        captions: {
            ja: [`年月日`, `日付と時刻(分)`, `日付と時刻(秒)`],
            en: [`Year-Month-Day`, `Date And Time (Minutes)`, `Date And Time (Seconds)`]
        }
    },
    description: {
        captions: {
            ja: [`複数行入力①`, `複数行入力②`, `マークダウン入力①`, `マークダウン入力②`, `リッチテキストエディタ`],
            en: [`Multi-Line Input 1`, `Multi-Line Input 2`, `Markdown Input 1`, `Markdown Input 2`, `Rich Text Editor`]
        }
    },
    check: {
        captions: {
            ja: [`チェックボックス`],
            en: [`Check Box`]
        }
    },
    attachments: {
        captions: {
            ja: [`添付ファイル`, `添付ファイル`],
            en: [`Attachment`, `Attachment`]
        }
    }
};

const buildPopupContents = (dataType: string) => {
    const dataTypeLower = String(dataType || '').toLowerCase();
    const isBasic = dataTypeLower === 'basic';
    const languageElement = document.getElementById('Language') as HTMLInputElement | null;
    const language = languageElement ? languageElement.value : 'ja';

    const getHeaderTag = (dataTypeLower: string, dataType: string) => {
        const iconMap: { [key: string]: string } = {
            class: 'text_fields',
            num: 'timer_10',
            date: 'calendar_month',
            description: 'edit_note',
            check: 'check',
            attachments: 'attach_file'
        };
        const icon = iconMap[dataTypeLower] || 'apps';
        const title = `${$p.display(dataType)}${$p.display('CanDoWith')}`;
        return `
<header class="popup-header">
    <span class="material-symbols-sharp is-fill">${icon}</span>
    ${title}
</header>`;
    };

    const getDescriptionTag = (dataType: string) => {
        const descriptionContent = $p.display(`EditorColumnsPopup${dataType}`);
        return `
<div class="popup-body-description">
    <div class="popup-body-description-label">
        ${descriptionContent}
    </div>
</div>`;
    };

    const getAnchorTag = (dataTypeLower: string, dataType: string, isBasic: boolean) => {
        const suffix = isBasic ? 'column' : dataTypeLower;
        const userManualUrlBase = `${$p.display('UserManualUrl')}${suffix}`;
        const utmSource = (document.getElementById('utmSource') as HTMLInputElement | null)?.value ?? '';
        const userManualUrlParameter = `?utm_source=${encodeURIComponent(utmSource)}&utm_medium=app&utm_campaign=manual&utm_content=table-management-tooltip`;
        const userManualUrl = `${userManualUrlBase}${userManualUrlParameter}`;
        const userManual = `${$p.display(dataType)}${$p.display('UserManual')}`;
        return `
<div class="popup-body-link-label">
    <a href="${userManualUrl}" target="_blank">
        ${userManual}
    </a>
</div>`;
    };

    const getImageTag = (dataTypeLower: string) => {
        const popupImageConfig = POPUP_IMAGE_MAP[dataTypeLower as keyof typeof POPUP_IMAGE_MAP];
        if (!popupImageConfig) {
            return '';
        }
        const captionLanguage = language === 'ja' ? 'ja' : 'en';
        const figures = popupImageConfig.captions[captionLanguage]
            .map((caption, index) => {
                const image = `/assets/images/editorColumns/popup-${dataTypeLower}-${index}.svg`;
                return `
<figure>
    <span><img src="${image}" alt="${dataTypeLower}"></span>
    <figcaption>${caption}</figcaption>
</figure>`;
            })
            .join('');

        return `
<div class="popup-body-image">
    <ui-carousel>
        ${figures}
    </ui-carousel>
</div>`;
    };

    const getFooterTag = (dataTypeLower: string) => {
        const isShowFooter = (document.getElementById('ShowLinkText') as HTMLInputElement).value === 'True';
        const isBasic = dataTypeLower === 'basic';
        if (language !== 'ja' || !isShowFooter || isBasic) {
            return '';
        }
        const footerContent = $p.display('UseColumnsExtension');
        return `
<footer class="popup-footer">
    <div class="popup-footer-label">
        ${footerContent}
    </div>
</footer>`;
    };

    return `
        <div class="popup-container">
            ${getHeaderTag(dataTypeLower, dataType)}
            <div class="popup-body">
                ${getDescriptionTag(dataType)}
                ${getAnchorTag(dataTypeLower, dataType, isBasic)}
                ${getImageTag(dataTypeLower)}
            </div>
            ${getFooterTag(dataTypeLower)}
        </div>
    `;
};

const getTypeFromControl = (controlElement: HTMLElement) => {
    const el = controlElement;
    if (!el) {
        return '';
    }
    const id = el.id || '';
    const match = id.match(/^(?:left|right)-editor-columns-filter-button-([a-z_]+)$/i);
    if (match && match[1]) {
        const token = match[1].toLowerCase();
        const map: { [key: string]: string } = {
            basic: 'Basic',
            class: 'Class',
            num: 'Num',
            date: 'Date',
            description: 'Description',
            check: 'Check',
            attachments: 'Attachments'
        };
        if (map[token]) {
            return map[token];
        }
    }
    return '';
};

const initPopupController = () => {
    new PopupController(document);
};

export const initialize = (): void => {
    const isPcScreen = window.matchMedia('(min-width: 1025px)').matches;
    if (isPcScreen) {
        initPopupController();
    }
};
