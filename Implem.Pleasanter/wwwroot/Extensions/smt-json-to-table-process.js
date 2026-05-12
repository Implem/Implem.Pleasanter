class ProcessView {
    constructor() {
        this.container = document.getElementById('processContainer');
    }

    renderFromData(data) {
        const translateWithFallback = (key, fallbackText, ...args) => {
            const translated = t(key, ...args);
            if (translated === key) {
                return fallbackText;
            }
            return translated;
        };

        const proc = data.Process || {};
        const html = `
            <div>
                <div class="ps-main-areas" role="region" aria-label="${t('ps_process_diagram')} / ${t('ps_processes_diagram')}">
                    <div class="ps-top-nav">
                        <h3 id="ps-topNavTitle" class="ps-top-nav-title">${t('ps_processes_diagram')}</h3>
                        <select id="ps-siteSelect" class="ps-site-select" style="display:none;"></select>
                        <div class="ps-top-area-actions">
                            <button id="ps-openProcessSelectorMobile" type="button" class="ps-open-process-selector">${translateWithFallback('ps_select_processes', t('ps_process_diagram'))}</button>
                        </div>
                    </div>

                    <div class="ps-main-content">
                        <aside id="ps-leftPanel" class="ps-left-panel" role="navigation" aria-label="${translateWithFallback('ps_select_processes', t('ps_process_diagram'))}">
                            <div class="ps-left-panel-head">
                                <h3 class="ps-left-panel-title">${translateWithFallback('ps_select_processes', t('ps_process_diagram'))}</h3>
                                <span id="ps-processSelectedCount" class="ps-process-selected-count"></span>
                            </div>
                            <section class="ps-controls ps-left-controls" aria-label="${translateWithFallback('ps_select_processes', t('ps_process_diagram'))}">
                                <input id="ps-fileInput" type="file" accept="application/json" />
                                <div class="ps-process-search-wrap">
                                    <svg class="ps-process-search-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true" focusable="false"><path d="M4.25 5.61C6.27 8.2 10 13 10 13v6c0 .55.45 1 1 1h2c.55 0 1-.45 1-1v-6s3.72-4.8 5.74-7.39A.998.998 0 0 0 18.95 4H5.04c-.83 0-1.3.95-.79 1.61z"/></svg>
                                    <input id="ps-processSearch" type="search" class="ps-process-search" placeholder="${translateWithFallback('ps_search_placeholder', 'キーワードで絞り込み')}" aria-label="${translateWithFallback('ps_search_placeholder', 'キーワードで絞り込み')}" />
                                </div>
                                <div class="ps-process-selector-actions">
                                    <button id="ps-leftProcessSelectorAll" type="button" class="ps-process-selector-action">${t('ps_select_all')}</button>
                                    <button id="ps-leftProcessSelectorNone" type="button" class="ps-process-selector-action">${t('ps_clear_all')}</button>
                                    <button id="ps-leftProcessSelectorReset" type="button" class="ps-process-selector-action">${t('ps_reset_to_initial')}</button>
                                </div>
                                <input type="checkbox" id="ps-toggleNotifs" checked hidden />
                                <input type="checkbox" id="ps-togglePerms" checked hidden />
                                <input type="checkbox" id="ps-toggleButtons" checked hidden />
                            </section>
                            <div id="ps-leftProcessList" class="ps-left-process-list"></div>
                        </aside>

                        <div class="ps-right-pane">
                            <section class="ps-top-area">
                                <div id="ps-mermaidWrap" class="ps-mermaid-wrap"></div>
                                <button id="ps-mermaidExpandBtn" type="button" class="ps-mermaid-expand-btn" title="${translateWithFallback('ps_full_view', '全体表示')}" aria-label="${translateWithFallback('ps_full_view', '全体表示')}">
                                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="18" height="18" fill="currentColor" aria-hidden="true"><path d="M19 19H5V5h7V3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2v-7h-2v7zM14 3v2h3.59l-9.83 9.83 1.41 1.41L19 6.41V10h2V3h-7z"/></svg>
                                </button>
                                <div class="ps-top-btn-group">
                                    <button id="ps-mermaidMmdBtn" type="button" class="ps-png-download-btn ps-png-btn-top" title="${translateWithFallback('ps_save_mmd_flow', 'フロー図MMD保存')}" aria-label="${translateWithFallback('ps_save_mmd_flow', 'フロー図MMD保存')}">
                                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" /><polyline points="7,10 12,15 17,10" /><line x1="12" y1="15" x2="12" y2="3" /></svg>&nbsp;<span>MMD</span>
                                    </button>
                                    <button id="ps-mermaidPngBtn" type="button" class="ps-png-download-btn ps-png-btn-top" title="${translateWithFallback('ps_save_png_flow', 'フロー図PNG保存')}" aria-label="${translateWithFallback('ps_save_png_flow', 'フロー図PNG保存')}">
                                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" /><polyline points="7,10 12,15 17,10" /><line x1="12" y1="15" x2="12" y2="3" /></svg>&nbsp;<span>PNG</span>
                                    </button>
                                </div>
                            </section>

                            <div id="ps-invalidStatusWarning" style="display:none;"></div>

                            <section class="ps-bottom-area">
                                <button id="ps-gridPngBtn" type="button" class="ps-png-download-btn ps-png-btn-bottom" title="${translateWithFallback('ps_save_png_grid', 'プロセス図PNG保存')}" aria-label="${translateWithFallback('ps_save_png_grid', 'プロセス図PNG保存')}">
                                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" width="16" height="16"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" /><polyline points="7,10 12,15 17,10" /><line x1="12" y1="15" x2="12" y2="3" /></svg>&nbsp;<span>PNG</span>
                                </button>
                                <div class="ps-top-left">
                                    <div id="ps-gridArea" class="ps-grid-wrap">
                                        <svg id="ps-gridSvg"></svg>
                                        <div id="ps-gridArea-msg" style="display:none;"></div>
                                    </div>
                                </div>
                            </section>
                        </div>
                    </div>
                </div>

                <!-- プロセス詳細モーダル -->
                <div id="ps-detailModal" class="ps-detail-modal" aria-hidden="true">
                    <div id="ps-detailModalBackdrop" class="ps-detail-modal-backdrop"></div>
                    <div class="ps-detail-modal-content" role="dialog" aria-labelledby="ps-detailModalTitle">
                        <div class="ps-detail-modal-title" id="ps-detailModalTitle">
                            <span class="ps-detail-modal-title-text">${t('ps_detail')}</span>
                            <button id="ps-detailModalClose" class="ps-detail-modal-close" aria-label="${t('ps_close')}">✕</button>
                        </div>
                        <div id="ps-detailModalBody" class="ps-detail-modal-body">${t('ps_select_a_cell')}</div>
                    </div>
                </div>

                <!-- プロセス選択モーダル -->
                <div id="ps-processSelectorModal" class="ps-process-selector-modal" aria-hidden="true">
                    <div id="ps-processSelectorBackdrop" class="ps-process-selector-backdrop"></div>
                    <div class="ps-process-selector-content" role="dialog" aria-labelledby="ps-processSelectorTitle">
                        <div class="ps-process-selector-title" id="ps-processSelectorTitle">
                            <span class="ps-process-selector-title-text">${translateWithFallback('ps_select_processes', t('ps_process_diagram'))}</span>
                            <button id="ps-processSelectorClose" class="ps-process-selector-close" aria-label="${t('ps_close')}">✕</button>
                        </div>
                        <div class="ps-process-selector-actions">
                            <button id="ps-processSelectorAll" type="button" class="ps-process-selector-action">${t('ps_select_all')}</button>
                            <button id="ps-processSelectorNone" type="button" class="ps-process-selector-action">${t('ps_clear_all')}</button>
                            <button id="ps-processSelectorReset" type="button" class="ps-process-selector-action">${t('ps_reset_to_initial')}</button>
                        </div>
                        <div id="ps-processSelectorBody" class="ps-process-selector-body"></div>
                    </div>
                </div>

                <!-- フロー図全体表示モーダル -->
                <div id="ps-mermaidFullModal" class="ps-mermaid-full-modal" aria-hidden="true">
                    <div id="ps-mermaidFullBackdrop" class="ps-mermaid-full-backdrop"></div>
                    <div class="ps-mermaid-full-content" role="dialog" aria-labelledby="ps-mermaidFullTitle">
                        <div class="ps-mermaid-full-header">
                            <span class="ps-mermaid-full-title-text" id="ps-mermaidFullTitle">${translateWithFallback('ps_full_view', '全体表示')}</span>
                            <div class="ps-mermaid-full-header-actions">
                                <button id="ps-mermaidFullClose" class="ps-detail-modal-close" aria-label="${t('ps_close')}">✕</button>
                            </div>
                        </div>
                        <div id="ps-mermaidFullBody" class="ps-mermaid-full-body"></div>
                    </div>
                </div>
            </div>
        `;
        this.container.innerHTML = html;

        const svg = d3.select('#ps-gridSvg');
        svg.attr('viewBox', '0 0 1000 800');

        const dom = {
            fileInput: document.getElementById('ps-fileInput'),
            gridArea: document.getElementById('ps-gridArea'),
            gridAreaMsg: document.getElementById('ps-gridArea-msg'),
            invalidStatusWarning: document.getElementById('ps-invalidStatusWarning'),
            siteSelect: document.getElementById('ps-siteSelect'),
            toggleButtons: document.getElementById('ps-toggleButtons'),
            togglePerms: document.getElementById('ps-togglePerms'),
            toggleNotifs: document.getElementById('ps-toggleNotifs'),
            toggleMermaid: document.getElementById('ps-toggleMermaid'),
            mermaidWrap: document.getElementById('ps-mermaidWrap'),
            mermaidPngBtn: document.getElementById('ps-mermaidPngBtn'),
            mermaidMmdBtn: document.getElementById('ps-mermaidMmdBtn'),
            detailModal: document.getElementById('ps-detailModal'),
            detailModalBody: document.getElementById('ps-detailModalBody'),
            detailModalClose: document.getElementById('ps-detailModalClose'),
            detailModalBackdrop: document.getElementById('ps-detailModalBackdrop'),
            openProcessSelectorMobile: document.getElementById('ps-openProcessSelectorMobile'),
            processSelectedCount: document.getElementById('ps-processSelectedCount'),
            leftPanel: document.getElementById('ps-leftPanel'),
            leftProcessList: document.getElementById('ps-leftProcessList'),
            processSearch: document.getElementById('ps-processSearch'),
            processSelectorModal: document.getElementById('ps-processSelectorModal'),
            processSelectorClose: document.getElementById('ps-processSelectorClose'),
            processSelectorBackdrop: document.getElementById('ps-processSelectorBackdrop'),
            processSelectorBody: document.getElementById('ps-processSelectorBody'),
            processSelectorAll: document.getElementById('ps-processSelectorAll'),
            processSelectorNone: document.getElementById('ps-processSelectorNone'),
            processSelectorReset: document.getElementById('ps-processSelectorReset'),
            leftProcessSelectorAll: document.getElementById('ps-leftProcessSelectorAll'),
            leftProcessSelectorNone: document.getElementById('ps-leftProcessSelectorNone'),
            leftProcessSelectorReset: document.getElementById('ps-leftProcessSelectorReset'),
            mermaidSection: document.querySelector('.ps-top-area'),
            mermaidHeading: document.getElementById('ps-mermaidHeading'),
            gridPngBtn: document.getElementById('ps-gridPngBtn'),
            mermaidExpandBtn: document.getElementById('ps-mermaidExpandBtn'),
            mermaidFullModal: document.getElementById('ps-mermaidFullModal'),
            mermaidFullBackdrop: document.getElementById('ps-mermaidFullBackdrop'),
            mermaidFullClose: document.getElementById('ps-mermaidFullClose'),
            mermaidFullBody: document.getElementById('ps-mermaidFullBody'),
        };

        const state = {
            raw: null,
            model: null,
            processChecked: Object.create(null),
            defaultProcessChecked: Object.create(null),
            gridBaseWidth: 1000,
            gridBaseHeight: 800,
            filters: {
                keyword: '',
            },
            mermaidEdgeLabels: [],
        };

        init();

        try {
            let autoData = null;
            if (data && Object.keys(data).length) {
                autoData = data;
            } else if (window.Common && typeof Common.parseJsonSafe === 'function') {
                autoData = Common.parseJsonSafe();
            }
            if (autoData && (autoData.Process || autoData.SiteSettings || Array.isArray(autoData.Processes) || (autoData.Sites && autoData.Sites.length))) {
                state.raw = autoData;
                detectAndNormalizeSite();
                buildModel();
                render();
                if (dom.fileInput) dom.fileInput.style.display = 'none';
            }
        } catch (e) { /* ignore auto-load errors */ }

        function init() {
            bindFileInput();
            bindSiteSelector();
            bindToggles();
            bindLeftPanel();
            bindKeyboard();
            bindDetailModal();
            bindProcessSelectorModal();
            bindPngDownload();
            initMermaid();
            bindMermaidFullView();
        }

        function bindLeftPanel() {
            if (dom.processSearch) {
                dom.processSearch.addEventListener('input', () => {
                    state.filters.keyword = String(dom.processSearch.value || '').trim().toLowerCase();
                    rebuildProcessSelectorList();
                });
            }
        }

        function applyDefaultProcessSelection() {
            const defaults = state.defaultProcessChecked || {};
            (state.model?.processes || []).forEach((process) => {
                state.processChecked[process.Id] = !!defaults[process.Id];
            });
        }

        function closeProcessSelectorModal() {
            if (!dom.processSelectorModal) return;
            dom.processSelectorModal.setAttribute('aria-hidden', 'true');
            if (dom.openProcessSelectorMobile) {
                dom.openProcessSelectorMobile.focus();
            }
        }

        function bindProcessSelectorModal() {
            const openButtons = [dom.openProcessSelectorMobile].filter(Boolean);
            openButtons.forEach((button) => {
                button.addEventListener('click', () => {
                    rebuildProcessSelectorList();
                    dom.processSelectorModal.setAttribute('aria-hidden', 'false');
                    if (dom.processSelectorClose) {
                        dom.processSelectorClose.focus();
                    }
                });
            });

            if (dom.processSelectorClose) {
                dom.processSelectorClose.addEventListener('click', () => {
                    closeProcessSelectorModal();
                });
            }

            if (dom.processSelectorBackdrop) {
                dom.processSelectorBackdrop.addEventListener('click', () => {
                    closeProcessSelectorModal();
                });
            }

            [dom.processSelectorAll, dom.leftProcessSelectorAll].filter(Boolean).forEach((button) => {
                button.addEventListener('click', () => {
                    (state.model?.processes || []).forEach((process) => {
                        state.processChecked[process.Id] = true;
                    });
                    updateProcessSelectionUi();
                    render();
                    rebuildProcessSelectorList();
                });
            });

            [dom.processSelectorNone, dom.leftProcessSelectorNone].filter(Boolean).forEach((button) => {
                button.addEventListener('click', () => {
                    (state.model?.processes || []).forEach((process) => {
                        state.processChecked[process.Id] = false;
                    });
                    updateProcessSelectionUi();
                    render();
                    rebuildProcessSelectorList();
                });
            });

            [dom.processSelectorReset, dom.leftProcessSelectorReset].filter(Boolean).forEach((button) => {
                button.addEventListener('click', () => {
                    applyDefaultProcessSelection();
                    updateProcessSelectionUi();
                    render();
                    rebuildProcessSelectorList();
                });
            });
        }

        function rebuildProcessSelectorList() {
            const targets = [dom.leftProcessList, dom.processSelectorBody].filter(Boolean);
            if (!targets.length) return;

            targets.forEach((target) => {
                target.innerHTML = '';
            });

            const processes = getFilteredProcesses();
            if (!processes.length) {
                targets.forEach((target) => {
                    const empty = document.createElement('div');
                    empty.className = 'ps-process-selector-empty';
                    empty.textContent = t('ps_none');
                    target.appendChild(empty);
                });
                return;
            }

            targets.forEach((target) => {
                const list = document.createElement('div');
                list.className = 'ps-process-selector-list';
                processes.forEach((process) => {
                    list.appendChild(createProcessCard(process));
                });
                target.appendChild(list);
            });
        }

        function getFilteredProcesses() {
            const processes = state.model?.processes || [];
            const keyword = state.filters.keyword || '';

            return processes.filter((process) => {
                const processTags = getProcessTags(process);
                if (!keyword) return true;
                const source = [
                    process.Name,
                    process.DisplayName,
                    process.Description,
                    getStatusLabel(process.CurrentStatus),
                    getStatusLabel(process.ChangedStatus),
                    processTags.join(' '),
                ].join(' ').toLowerCase();
                return source.includes(keyword);
            });
        }

        function getProcessTags(process) {
            if (!process || typeof process !== 'object') return [];

            const tagsValue = process.Tags || process.Tag;
            if (Array.isArray(tagsValue)) {
                return tagsValue.map((tag) => String(tag || '').trim()).filter((tag) => !!tag);
            }
            if (typeof tagsValue === 'string') {
                return tagsValue.split(',').map((tag) => tag.trim()).filter((tag) => !!tag);
            }
            return [];
        }

        function createProcessCard(process) {
            const row = document.createElement('div');
            row.className = 'ps-process-selector-item';
            row.setAttribute('role', 'button');
            row.setAttribute('tabindex', '0');
            row.setAttribute('aria-pressed', state.processChecked[process.Id] ? 'true' : 'false');
            if (state.processChecked[process.Id]) {
                row.classList.add('is-selected');
            }

            const name = process.Name || process.DisplayName || t('ps_process_fallback', process.Id);
            const tags = getProcessTags(process);

            const check = document.createElement('span');
            check.className = 'ps-process-selector-check';
            check.setAttribute('aria-hidden', 'true');
            check.innerHTML = state.processChecked[process.Id]
                ? '<svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true"><path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/></svg>'
                : '';

            const body = document.createElement('span');
            body.className = 'ps-process-selector-main';

            const title = document.createElement('span');
            title.className = 'ps-process-selector-name';
            title.textContent = name;
            title.title = name;
            body.appendChild(title);

            const metaWrap = document.createElement('span');
            metaWrap.className = 'ps-process-selector-meta-wrap';

            const metaFrom = document.createElement('span');
            metaFrom.className = 'ps-process-selector-meta';
            metaFrom.textContent = getStatusLabel(process.CurrentStatus);
            metaFrom.title = getStatusLabel(process.CurrentStatus);

            const metaArrow = document.createElement('span');
            metaArrow.className = 'ps-process-selector-meta-arrow';
            metaArrow.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" width="14" height="14" aria-hidden="true"><path d="M12 4l-1.41 1.41L16.17 11H4v2h12.17l-5.58 5.59L12 20l8-8z"/></svg>';

            const metaTo = document.createElement('span');
            metaTo.className = 'ps-process-selector-meta';
            metaTo.textContent = getStatusLabel(process.ChangedStatus);
            metaTo.title = getStatusLabel(process.ChangedStatus);

            metaWrap.appendChild(metaFrom);
            metaWrap.appendChild(metaArrow);
            metaWrap.appendChild(metaTo);
            body.appendChild(metaWrap);

            if (tags.length) {
                const tagText = document.createElement('span');
                tagText.className = 'ps-process-selector-tags';
                tagText.textContent = tags.join(', ');
                body.appendChild(tagText);
            }

            const notifications = state.model.notificationsByProcess[process.Id] || [];
            const permItems = [];
            (process.Depts || []).forEach((id) => { if (state.model.permDepts[id]) permItems.push(state.model.permDepts[id]); });
            (process.Groups || []).forEach((id) => { if (state.model.permGroups[id]) permItems.push(state.model.permGroups[id]); });
            (process.Users || []).forEach((id) => { if (state.model.permUsers[id]) permItems.push(state.model.permUsers[id]); });

            if (notifications.length > 0 || permItems.length > 0) {
                const badges = document.createElement('span');
                badges.className = 'ps-process-selector-badges';

                if (notifications.length > 0) {
                    const nb = document.createElement('span');
                    nb.className = 'ps-card-badge ps-card-badge-notif';
                    nb.title = t('ps_notification_count', notifications.length);
                    nb.innerHTML = `<svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true" width="11" height="11"><path d="M20 4H4c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 4-8 5-8-5V6l8 5 8-5v2z"/></svg>${notifications.length}`;
                    badges.appendChild(nb);
                }
                if (permItems.length > 0) {
                    const pb = document.createElement('span');
                    pb.className = 'ps-card-badge ps-card-badge-perm';
                    pb.title = t('ps_permission_count', permItems.length);
                    pb.innerHTML = `<svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true" width="11" height="11"><path d="M12 1 3 5v6c0 5.55 3.84 10.74 9 12 5.16-1.26 9-6.45 9-12V5l-9-4zm0 10.99h7c-.53 4.12-3.28 7.79-7 8.94V12H5V6.3l7-3.11v8.8z"/></svg>${permItems.length}`;
                    badges.appendChild(pb);
                }
                body.appendChild(badges);
            }

            row.appendChild(check);
            row.appendChild(body);

            const detailBtn = document.createElement('button');
            detailBtn.type = 'button';
            detailBtn.className = 'ps-card-detail-btn';
            const detailViewLabel = translateWithFallback('ps_detail_view', '詳細を見る');
            detailBtn.title = detailViewLabel;
            detailBtn.setAttribute('aria-label', detailViewLabel);
            detailBtn.innerHTML = '<svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true" width="20" height="20"><path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-6h2v6zm0-8h-2V7h2v2z"/></svg>';
            detailBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                showProcessDetail(process);
            });
            row.appendChild(detailBtn);

            const onToggle = () => {
                state.processChecked[process.Id] = !state.processChecked[process.Id];
                updateProcessSelectionUi();
                render();
                rebuildProcessSelectorList();
            };

            row.addEventListener('click', onToggle);
            row.addEventListener('keydown', (event) => {
                if (event.key !== 'Enter' && event.key !== ' ') return;
                event.preventDefault();
                onToggle();
            });

            return row;
        }

        function updateProcessSelectionUi() {
            if (!dom.processSelectedCount) return;
            const total = (state.model?.processes || []).length;
            const selected = (state.model?.processes || []).filter((proc) => !!state.processChecked[proc.Id]).length;
            const translated = t('ps_selected_count', selected, total);
            dom.processSelectedCount.textContent = translated === 'ps_selected_count' ? `${selected}/${total}` : translated;
        }

        function getVisibleProcesses() {
            if (!state.model || !Array.isArray(state.model.processes)) return [];
            return state.model.processes.filter((proc) => !!state.processChecked[proc.Id]);
        }

        function closeDetailModal() {
            if (!dom.detailModal) return;
            dom.detailModal.setAttribute('aria-hidden', 'true');
        }

        function bindDetailModal() {
            if (dom.detailModalClose) {
                dom.detailModalClose.addEventListener('click', () => {
                    closeDetailModal();
                });
            }
            if (dom.detailModalBackdrop) {
                dom.detailModalBackdrop.addEventListener('click', () => {
                    closeDetailModal();
                });
            }
        }

        function applyGridZoom() {
            const baseWidth = Math.max(200, Number(state.gridBaseWidth) || 1000);
            const baseHeight = Math.max(160, Number(state.gridBaseHeight) || 800);

            svg.attr('viewBox', `0 0 ${baseWidth} ${baseHeight}`)
               .attr('preserveAspectRatio', 'xMinYMin meet')
               .attr('width', null)
               .attr('height', null);

            svg.style('width', '100%')
               .style('height', 'auto')
               .style('min-width', `${baseWidth}px`)
               .style('min-height', `${baseHeight}px`);

            if (dom.gridArea) {
                dom.gridArea.style.overflowX = 'auto';
                dom.gridArea.style.overflowY = 'auto';
                dom.gridArea.style.scrollbarGutter = 'stable both-edges';
            }
        }

        function initMermaid() {
            if (!window.mermaid) return;
            try {
                mermaid.initialize({
                    startOnLoad: false,
                    theme: 'base',
                    fontFamily: 'system-ui, -apple-system, "Segoe UI", Roboto, "Noto Sans JP", "Helvetica Neue", Arial',
                    flowchart: {
                        useMaxWidth: false,
                        htmlLabels: true,
                        curve: 'basis',
                        nodeSpacing: 35,
                        rankSpacing: 50,
                        padding: 10,
                    },
                    themeVariables: {
                        background: '#f8fbff',
                        primaryColor: '#e3edf8',
                        primaryTextColor: '#0d2d46',
                        primaryBorderColor: '#2a6fa8',
                        secondaryColor: '#edf3f9',
                        secondaryTextColor: '#22384f',
                        secondaryBorderColor: '#c6d6e6',
                        tertiaryColor: '#f7fbff',
                        tertiaryTextColor: '#274563',
                        tertiaryBorderColor: '#d6e3ef',
                        lineColor: '#2563eb',
                        textColor: '#0d2d46',
                        edgeLabelBackground: '#f0f5ff',
                        nodeBorder: '#2a6fa8',
                        clusterBkg: '#f3f7fb',
                        clusterBorder: '#d6e3ef',
                        mainBkg: '#e3edf8',
                    },
                });
            } catch (err) {
                console.warn('mermaid init failed', err);
            }
        }

        function bindFileInput() {
            if (!dom.fileInput) return;
            dom.fileInput.addEventListener('change', (event) => {
                const file = event.target.files && event.target.files[0];
                if (!file) return;

                const reader = new FileReader();
                reader.onload = () => {
                    try {
                        state.raw = JSON.parse(reader.result);
                        detectAndNormalizeSite();
                        buildModel();
                        render();
                    } catch (err) {
                        alert(err.message);
                    }
                };
                reader.readAsText(file);
            });
        }

        function adjustSiteSelectorWidth() {
            if (!dom.siteSelect) return;
            if (dom.siteSelect.style.display === 'none') return;
            const selected = dom.siteSelect.options[dom.siteSelect.selectedIndex];
            if (!selected) return;
            const styles = window.getComputedStyle(dom.siteSelect);
            const measurer = document.createElement('span');
            measurer.style.position = 'absolute';
            measurer.style.visibility = 'hidden';
            measurer.style.whiteSpace = 'pre';
            measurer.style.fontFamily = styles.fontFamily;
            measurer.style.fontSize = styles.fontSize;
            measurer.style.fontWeight = styles.fontWeight;
            measurer.style.fontStyle = styles.fontStyle;
            measurer.style.letterSpacing = styles.letterSpacing;
            measurer.textContent = selected.textContent;
            document.body.appendChild(measurer);
            const textWidth = measurer.getBoundingClientRect().width;
            document.body.removeChild(measurer);
            dom.siteSelect.style.width = `${Math.ceil(textWidth + 36)}px`;
        }

        function bindSiteSelector() {
            if (!dom.siteSelect) return;
            dom.siteSelect.addEventListener('change', () => {
                const idx = Number.parseInt(dom.siteSelect.value, 10);
                if (!Number.isFinite(idx) || !state.raw || !state.raw.Sites) return;
                if (!state.raw.Sites[idx]) return;

                state.raw._activeSiteIndex = idx;
                state.raw.SiteSettings = state.raw.Sites[idx].SiteSettings;
                if (dom.mermaidWrap) dom.mermaidWrap.innerHTML = '';
                adjustSiteSelectorWidth();
                buildModel();
                render();
            });
        }

        function bindToggles() {
            const listeners = [dom.toggleButtons, dom.togglePerms, dom.toggleNotifs];
            listeners.forEach((toggle) => {
                if (!toggle) return;
                toggle.addEventListener('change', () => render());
            });

            if (dom.toggleMermaid) {
                dom.toggleMermaid.addEventListener('change', () => {
                    if (dom.mermaidWrap) {
                        dom.mermaidWrap.style.display = dom.toggleMermaid.checked ? '' : 'none';
                    }
                    try {
                        const checkboxes = document.querySelectorAll('.ps-proc-link-check');
                        checkboxes.forEach((el) => {
                            el.style.display = dom.toggleMermaid.checked ? '' : 'none';
                        });
                    } catch (err) {
                        console.warn('checkbox refresh failed', err);
                    }
                    render();
                    syncMermaidSectionHeight();
                });
            }
        }

        function syncMermaidSectionHeight() {
            if (!dom.mermaidSection || !dom.mermaidWrap) return;

            const visible = !dom.toggleMermaid || dom.toggleMermaid.checked;
            if (!visible) {
                dom.mermaidSection.style.minHeight = '0';
                dom.mermaidSection.style.height = 'auto';
                dom.mermaidWrap.style.minHeight = '0';
                return;
            }

            const checkedCount = Object.keys(state.processChecked || {}).filter((pid) => !!state.processChecked[pid]).length;
            if (checkedCount === 0) {
                const headingHeight = dom.mermaidHeading ? dom.mermaidHeading.offsetHeight : 0;
                const compactWrapHeight = 96;
                const compactSectionHeight = headingHeight + compactWrapHeight + 48;
                dom.mermaidWrap.style.minHeight = `${compactWrapHeight}px`;
                dom.mermaidSection.style.minHeight = `${compactSectionHeight}px`;
                dom.mermaidSection.style.height = 'auto';
                return;
            }

            dom.mermaidSection.style.height = 'auto';
            dom.mermaidSection.style.minHeight = '';
            dom.mermaidWrap.style.minHeight = '';
        }

        function bindKeyboard() {
            document.addEventListener('keydown', (event) => {
                const selectorOpen = dom.processSelectorModal && dom.processSelectorModal.getAttribute('aria-hidden') === 'false';
                const detailOpen = dom.detailModal && dom.detailModal.getAttribute('aria-hidden') === 'false';
                const fullViewOpen = dom.mermaidFullModal && dom.mermaidFullModal.getAttribute('aria-hidden') === 'false';

                if (event.key === 'Escape') {
                    if (fullViewOpen) {
                        closeMermaidFullView();
                        return;
                    }
                    if (selectorOpen) {
                        closeProcessSelectorModal();
                        return;
                    }
                    if (detailOpen) {
                        closeDetailModal();
                    }
                    return;
                }

                if (event.key !== 'Tab') return;
                if (fullViewOpen) {
                    trapFocusInModal(dom.mermaidFullModal, event);
                    return;
                }
                if (selectorOpen) {
                    trapFocusInModal(dom.processSelectorModal, event);
                    return;
                }
                if (detailOpen) {
                    trapFocusInModal(dom.detailModal, event);
                }
            });
        }

        function downloadSvgAsPng(svgElement, filenamePrefix, options) {
            if (!svgElement) return;
            const opts = options || {};

            let contentW = 0, contentH = 0, vbX = 0, vbY = 0;
            const vb = svgElement.getAttribute('viewBox');
            if (vb) {
                const parts = vb.split(/[\s,]+/).map(Number);
                if (parts.length === 4 && parts[2] > 0 && parts[3] > 0) {
                    vbX = parts[0];
                    vbY = parts[1];
                    contentW = parts[2];
                    contentH = parts[3];
                }
            }
            if (contentW <= 0 || contentH <= 0) {
                const tempClone = svgElement.cloneNode(true);
                tempClone.style.cssText = 'position:absolute;left:-9999px;top:-9999px;visibility:hidden;';
                tempClone.style.transform = '';
                document.body.appendChild(tempClone);
                try {
                    const b = tempClone.getBBox();
                    vbX = b.x; vbY = b.y;
                    contentW = b.width; contentH = b.height;
                } catch (_) {
                    const r = tempClone.getBoundingClientRect();
                    contentW = r.width; contentH = r.height;
                }
                tempClone.remove();
            }
            if (contentW <= 0 || contentH <= 0) return;

            const pad = 16;
            const defaultW = Math.round(contentW + pad * 2);
            const defaultH = Math.round(contentH + pad * 2);

            const dlgBg = document.createElement('div');
            dlgBg.style.cssText = 'position:fixed;inset:0;background:rgba(0,0,0,0.4);z-index:9998;';
            document.body.appendChild(dlgBg);

            const dlg = document.createElement('div');
            dlg.style.cssText = 'position:fixed;top:50%;left:50%;transform:translate(-50%,-50%);background:#fff;border-radius:12px;padding:28px 24px;z-index:9999;box-shadow:0 8px 32px rgba(0,0,0,0.2);width:340px;box-sizing:border-box;overflow:hidden;';
            dlg.innerHTML = `
                <div style="font-weight:700;font-size:15px;margin-bottom:16px;">${translateWithFallback('ps_png_export', 'PNG画像出力')}</div>
                <div style="display:flex;gap:16px;margin-bottom:12px;align-items:baseline;">
                    <label>${translateWithFallback('ps_png_width', '幅(px)')}: <input type="number" id="psPngW" value="${defaultW}" style="width:80px;height:30px;padding:4px 6px;border:1px solid #d1d5db;border-radius:6px;box-sizing:border-box;"></label>
                    <label>${translateWithFallback('ps_png_height', '高さ(px)')}: <input type="number" id="psPngH" value="${defaultH}" style="width:80px;height:30px;padding:4px 6px;border:1px solid #d1d5db;border-radius:6px;box-sizing:border-box;"></label>
                </div>
                <div style="margin-bottom:8px;">
                    <label><input type="checkbox" id="psPngKeepRatio" checked> ${translateWithFallback('ps_png_keep_ratio', 'アスペクト比を維持')}</label>
                </div>
                <div style="margin-bottom:16px;">
                    <label><input type="checkbox" id="psPngFullView"> ${translateWithFallback('ps_png_full_view', '図全体を出力する')}</label>
                </div>
                <div id="psPngError" style="display:none;color:#dc2626;font-size:13px;margin-bottom:8px;word-break:break-word;"></div>
                <div style="display:flex;gap:8px;justify-content:flex-end;">
                    <button id="psPngExec" style="padding:6px 16px;background:#2563eb;color:#fff;border:none;border-radius:6px;cursor:pointer;">Download</button>
                    <button id="psPngCancel" style="padding:6px 16px;background:#e5e7eb;border:none;border-radius:6px;cursor:pointer;">Cancel</button>
                </div>
            `;
            document.body.appendChild(dlg);

            const wInput = dlg.querySelector('#psPngW');
            const hInput = dlg.querySelector('#psPngH');
            const keepRatio = dlg.querySelector('#psPngKeepRatio');
            const fullViewCheck = dlg.querySelector('#psPngFullView');
            if (fullViewCheck && !opts.allowFullView) {
                fullViewCheck.closest('div').style.display = 'none';
            }
            let lastChanged = 'w';

            wInput.oninput = () => {
                lastChanged = 'w';
                if (keepRatio.checked) {
                    const w = parseInt(wInput.value, 10) || defaultW;
                    hInput.value = Math.round(w * defaultH / defaultW);
                }
            };
            hInput.oninput = () => {
                lastChanged = 'h';
                if (keepRatio.checked) {
                    const h = parseInt(hInput.value, 10) || defaultH;
                    wInput.value = Math.round(h * defaultW / defaultH);
                }
            };
            keepRatio.onchange = () => {
                if (keepRatio.checked) {
                    if (lastChanged === 'w') {
                        const w = parseInt(wInput.value, 10) || defaultW;
                        hInput.value = Math.round(w * defaultH / defaultW);
                    } else {
                        const h = parseInt(hInput.value, 10) || defaultH;
                        wInput.value = Math.round(h * defaultW / defaultH);
                    }
                }
            };

            const cleanup = () => { dlg.remove(); dlgBg.remove(); };
            const pngError = dlg.querySelector('#psPngError');
            const showPngError = (msg) => {
                if (pngError) {
                    pngError.textContent = msg;
                    pngError.style.display = '';
                }
            };
            const hidePngError = () => {
                if (pngError) {
                    pngError.textContent = '';
                    pngError.style.display = 'none';
                }
            };

            dlg.querySelector('#psPngCancel').onclick = cleanup;
            dlgBg.onclick = cleanup;

            dlg.querySelector('#psPngExec').onclick = () => {
                hidePngError();
                const w = parseInt(wInput.value, 10) || defaultW;
                const h = parseInt(hInput.value, 10) || defaultH;
                const useFullView = fullViewCheck && fullViewCheck.checked;

                if (useFullView && typeof buildMermaidMarkdown === 'function') {
                    const tempContainer = document.createElement('div');
                    tempContainer.style.cssText = 'position:absolute;left:-9999px;top:-9999px;width:2000px;';
                    document.body.appendChild(tempContainer);
                    renderMermaidTo(tempContainer, buildMermaidMarkdown(false));
                    setTimeout(() => {
                        const fullSvg = tempContainer.querySelector('svg');
                        if (fullSvg) {
                            downloadSvgAsPng(fullSvg, filenamePrefix + '_full');
                        }
                        tempContainer.remove();
                        cleanup();
                    }, 500);
                    return;
                }

                const exportClone = svgElement.cloneNode(true);
                exportClone.removeAttribute('style');

                const styleRules = [];
                try {
                    for (const sheet of document.styleSheets) {
                        try {
                            for (const rule of sheet.cssRules || []) {
                                if (rule.selectorText && /\.ps-/.test(rule.selectorText)) {
                                    const rewritten = rule.selectorText.replace(
                                        /\.ps-mermaid-wrap\s+(?:\.mermaid|svg)\s*/g, ''
                                    ).replace(
                                        /\.ps-mermaid-wrap\s*/g, ''
                                    );
                                    styleRules.push(rule.cssText.replace(rule.selectorText, rewritten));
                                }
                            }
                        } catch (_) { /* cross-origin */ }
                    }
                } catch (_) {}
                if (styleRules.length) {
                    const styleEl = document.createElementNS('http://www.w3.org/2000/svg', 'style');
                    styleEl.textContent = styleRules.join('\n');
                    exportClone.insertBefore(styleEl, exportClone.firstChild);
                }

                const svgW = contentW + pad * 2;
                const svgH = contentH + pad * 2;
                exportClone.setAttribute('width', svgW);
                exportClone.setAttribute('height', svgH);
                exportClone.setAttribute('viewBox', `${vbX - pad} ${vbY - pad} ${svgW} ${svgH}`);

                const serializer = new XMLSerializer();
                const svgText = serializer.serializeToString(exportClone);
                const svgBase64 = btoa(unescape(encodeURIComponent(svgText)));
                const img = new Image();
                img.onload = () => {
                    const canvas = document.createElement('canvas');
                    canvas.width = w;
                    canvas.height = h;
                    const ctx = canvas.getContext('2d');
                    if (!ctx) {
                        showPngError(translateWithFallback('ps_png_too_large', '指定サイズが大きすぎるため画像を生成できません。サイズを小さくしてください。'));
                        return;
                    }
                    ctx.fillStyle = '#fff';
                    ctx.fillRect(0, 0, w, h);
                    ctx.drawImage(img, 0, 0, w, h);
                    canvas.toBlob(blob => {
                        if (!blob) {
                            showPngError(translateWithFallback('ps_png_too_large', '指定サイズが大きすぎるため画像を生成できません。サイズを小さくしてください。'));
                            return;
                        }
                        const pngUrl = URL.createObjectURL(blob);
                        const a = document.createElement('a');
                        a.href = pngUrl;
                        a.download = vsFilename(filenamePrefix, 'png');
                        document.body.appendChild(a);
                        a.click();
                        setTimeout(() => {
                            document.body.removeChild(a);
                            URL.revokeObjectURL(pngUrl);
                        }, 100);
                        cleanup();
                    }, 'image/png');
                };
                img.src = 'data:image/svg+xml;base64,' + svgBase64;
            };
        }

        function vsFilename(feature, ext) {
            const pad = n => n.toString().padStart(2, '0');
            const d = new Date();
            const suffix = `${d.getFullYear()}_${pad(d.getMonth() + 1)}_${pad(d.getDate())} ${pad(d.getHours())}_${pad(d.getMinutes())}_${pad(d.getSeconds())}`;
            return `VisualizeSettings_${feature}_${suffix}.${ext.toLowerCase()}`;
        }

        function updatePngButtons(hasSelection) {
            [dom.mermaidMmdBtn, dom.mermaidPngBtn, dom.gridPngBtn].forEach(btn => {
                if (!btn) return;
                btn.disabled = !hasSelection;
                btn.style.opacity = hasSelection ? '' : '0.4';
                btn.style.cursor = hasSelection ? '' : 'default';
                btn.title = hasSelection
                    ? btn.getAttribute('data-title-orig') || ''
                    : translateWithFallback('ps_png_select_process', 'プロセスを選択してください');
            });
        }

        function bindPngDownload() {
            if (dom.mermaidMmdBtn) {
                dom.mermaidMmdBtn.setAttribute('data-title-orig', dom.mermaidMmdBtn.title);
                dom.mermaidMmdBtn.addEventListener('click', () => {
                    if (dom.mermaidMmdBtn.disabled) return;
                    const dlgBg = document.createElement('div');
                    dlgBg.style.cssText = 'position:fixed;inset:0;background:rgba(0,0,0,0.4);z-index:9998;';
                    document.body.appendChild(dlgBg);
                    const dlg = document.createElement('div');
                    dlg.style.cssText = 'position:fixed;top:50%;left:50%;transform:translate(-50%,-50%);background:#fff;border-radius:12px;padding:28px 24px;z-index:9999;box-shadow:0 8px 32px rgba(0,0,0,0.2);min-width:300px;';
                    dlg.innerHTML = `
                        <div style="font-weight:700;font-size:15px;margin-bottom:16px;">${translateWithFallback('ps_mmd_export', 'MMDテキスト出力')}</div>
                        <div style="margin-bottom:16px;">
                            <label><input type="checkbox" id="psMmdFullView"> ${translateWithFallback('ps_png_full_view', '全体表示（省略なし）で出力')}</label>
                        </div>
                        <div style="display:flex;gap:8px;justify-content:flex-end;">
                            <button id="psMmdExec" style="padding:6px 16px;background:#2563eb;color:#fff;border:none;border-radius:6px;cursor:pointer;">Download</button>
                            <button id="psMmdCancel" style="padding:6px 16px;background:#e5e7eb;border:none;border-radius:6px;cursor:pointer;">Cancel</button>
                        </div>
                    `;
                    document.body.appendChild(dlg);
                    const cleanup = () => { dlg.remove(); dlgBg.remove(); };
                    dlg.querySelector('#psMmdCancel').onclick = cleanup;
                    dlgBg.onclick = cleanup;
                    dlg.querySelector('#psMmdExec').onclick = () => {
                        const useFullView = dlg.querySelector('#psMmdFullView').checked;
                        const markdown = buildMermaidMarkdown(!useFullView);
                        if (!markdown) { cleanup(); return; }
                        const blob = new Blob([markdown], { type: 'text/plain;charset=utf-8' });
                        const blobUrl = URL.createObjectURL(blob);
                        const a = document.createElement('a');
                        a.download = vsFilename('Process', 'mmd');
                        a.href = blobUrl;
                        document.body.appendChild(a);
                        a.click();
                        a.remove();
                        setTimeout(() => URL.revokeObjectURL(blobUrl), 10000);
                        cleanup();
                    };
                });
            }
            if (dom.mermaidPngBtn) {
                dom.mermaidPngBtn.setAttribute('data-title-orig', dom.mermaidPngBtn.title);
                dom.mermaidPngBtn.addEventListener('click', () => {
                    if (dom.mermaidPngBtn.disabled) return;
                    const svgEl = dom.mermaidWrap ? dom.mermaidWrap.querySelector('svg') : null;
                    if (!svgEl) return;
                    downloadSvgAsPng(svgEl, 'ProcessFlow', { allowFullView: true });
                });
            }
            if (dom.gridPngBtn) {
                dom.gridPngBtn.setAttribute('data-title-orig', dom.gridPngBtn.title);
                dom.gridPngBtn.addEventListener('click', () => {
                    if (dom.gridPngBtn.disabled) return;
                    const svgEl = document.getElementById('ps-gridSvg');
                    if (!svgEl) return;
                    downloadSvgAsPng(svgEl, 'ProcessGrid');
                });
            }
        }

        function detectAndNormalizeSite() {
            if (!state.raw) return;
            if (state.raw.SiteSettings) {
                hideSiteSelector();
                return;
            }

            try {
                if (state.raw.Process && Array.isArray(state.raw.Process.Processes) && state.raw.Process.Processes.length) {
                    const allSites = [];
                    for (let i = 0; i < state.raw.Process.Processes.length; i++) {
                        const p = state.raw.Process.Processes[i];
                        const sites = (p && p.SitePackage && Array.isArray(p.SitePackage.Sites)) ? p.SitePackage.Sites : (Array.isArray(p && p.Sites) ? p.Sites : null);
                        if (sites && sites.length) {
                            allSites.push(...sites);
                        }
                    }
                    if (allSites.length) {
                        state.raw.Sites = allSites;
                    }
                    if (Array.isArray(state.raw.Sites)) {
                        const choices = collectSiteChoices(state.raw.Sites);
                        if (!choices.length) {
                            alert(t('ps_no_sites_found'));
                            hideSiteSelector();
                            return;
                        }
                        populateSiteSelector(choices);
                        if (!Number.isFinite(state.raw._activeSiteIndex) || !state.raw.Sites[state.raw._activeSiteIndex]) {
                            state.raw._activeSiteIndex = choices[0].idx;
                        }
                        state.raw.SiteSettings = state.raw.Sites[state.raw._activeSiteIndex].SiteSettings;
                        return;
                    }
                }
            } catch (err) {
                console.warn('process-packages parse failed', err);
            }
            let container = document.getElementById('processContainer');
            if (container) {
                container.innerHTML = `<span style="color:red;">${t('common_site_not_found')}</span>`;
            }
            hideSiteSelector();
        }

        function collectSiteChoices(sites) {
            const choices = [];
            sites.forEach((site, idx) => {
                try {
                    const ss = site.SiteSettings;
                    if (!ss) return;
                    if (ss.ReferenceType === 'Results' || ss.ReferenceType === 'Issues') {
                        choices.push({ idx, title: site.Title || `Site ${site.SiteId}`, siteId: site.SiteId, referenceType: ss.ReferenceType, siteSettings: ss });
                    }
                } catch (err) {
                    console.warn('site parse failed', err);
                }
            });
            return choices;
        }

        function populateSiteSelector(choices) {
            if (!dom.siteSelect) return;
            const refTypeLabel = { Results: t('ss_results'), Issues: t('ss_issues') };
            dom.siteSelect.innerHTML = '';
            choices.forEach((choice) => {
                const option = document.createElement('option');
                option.value = String(choice.idx);
                option.textContent = `${choice.title} - [ ${choice.siteId} / ${refTypeLabel[choice.referenceType] || choice.referenceType}]`;
                dom.siteSelect.appendChild(option);
            });
            dom.siteSelect.style.display = 'inline-block';
            adjustSiteSelectorWidth();
        }

        function hideSiteSelector() {
            if (dom.siteSelect) {
                dom.siteSelect.style.display = 'none';
            }
        }

        function buildModel() {
            const ss = (state.raw && state.raw.SiteSettings) || {};
            const columns = ss.Columns || [];
            const processes = (ss.Processes || []).map((p) => ({ ...p }));

            const statusInfo = extractStatuses(columns);
            const permissions = extractPermissions();
            const notificationsByProcess = extractNotifications(processes);
            const columnLabelMap = buildColumnLabelMap(columns);

            const hasWildcard = processes.some(p => p.CurrentStatus === -1 || p.ChangedStatus === -1);
            if (hasWildcard && !(-1 in statusInfo.statusMap)) {
                statusInfo.statusMap[-1] = translateWithFallback('ps_all_statuses', '＊(全ての状況)');
            }
            const hasWildcardFrom = processes.some(p => p.CurrentStatus === -1 && p.ChangedStatus !== -1);
            if (hasWildcardFrom && !statusInfo.statusList.includes(-1)) {
                statusInfo.statusList.push(-1);
            }

            if (!(0 in statusInfo.statusMap)) {
                const zeroReferenced = processes.some(p => p.CurrentStatus === 0 || p.ChangedStatus === 0);
                if (zeroReferenced) {
                    statusInfo.statusMap[0] = translateWithFallback('ps_unset_status', '未設定');
                    if (!statusInfo.statusList.includes(0)) statusInfo.statusList.push(0);
                }
            }

            const invalidStatuses = new Set();
            processes.forEach(p => {
                if (p.CurrentStatus !== -1 && !(p.CurrentStatus in statusInfo.statusMap)) invalidStatuses.add(p.CurrentStatus);
                if (p.ChangedStatus !== -1 && !(p.ChangedStatus in statusInfo.statusMap)) invalidStatuses.add(p.ChangedStatus);
            });
            invalidStatuses.forEach(s => {
                if (!statusInfo.statusList.includes(s)) statusInfo.statusList.push(s);
                if (!(s in statusInfo.statusMap)) statusInfo.statusMap[s] = String(s);
            });
            statusInfo.statusList.sort((a, b) => a - b);

            state.model = {
                processes,
                statusList: statusInfo.statusList,
                statusMap: statusInfo.statusMap,
                invalidStatuses,
                permDepts: permissions.permDepts,
                permGroups: permissions.permGroups,
                permUsers: permissions.permUsers,
                notificationsByProcess,
                columnLabelMap,
                schema: (state.raw && state.raw.Process && state.raw.Process.Schema) || null,
            };

            initializeProcessChecked();
            rebuildProcessSelectorList();
        }

        function parseChoiceLine(line) {
            const parts = line.split(/(?<!\\),/).map(p => p.replace(/\\,/g, ','));
            const value = parts[0] || '';
            const text = parts[1] || value;
            const textMini = parts[2] || text;
            const cssClass = parts[3] || '';
            return { value, text, textMini, cssClass };
        }
        function extractStatuses(columns) {
            const statusList = [];
            const statusMap = {};
            columns.forEach((col) => {
                if (col.ColumnName !== 'Status' || !col.ChoicesText) return;
                col.ChoicesText.split(/\r?\n/)
                    .map((line) => line.trim())
                    .filter(Boolean)
                    .forEach((line) => {
                        const choice = parseChoiceLine(line);
                        if (!/^-?\d+$/.test(choice.value)) return;
                        const key = Number.parseInt(choice.value, 10);
                        if (key === -1) return; // -1 は「＊(全ての状況)」を意味する予約値
                        if (statusList.includes(key)) return;
                        statusList.push(key);
                        statusMap[key] = choice.text;
                    });
            });
            statusList.sort((a, b) => a - b);
            return { statusList, statusMap };
        }

        function extractPermissions() {
            const result = {
                permDepts: {},
                permGroups: {},
                permUsers: {},
            };

            try {
                let pidList = state.raw.PermissionIdList
                    || (state.raw.Sites && state.raw.Sites[state.raw._activeSiteIndex] && state.raw.Sites[state.raw._activeSiteIndex].PermissionIdList);

                if (!pidList && state.raw.Process && Array.isArray(state.raw.Process.Processes)) {
                    for (let i = 0; i < state.raw.Process.Processes.length; i++) {
                        const proc = state.raw.Process.Processes[i];
                        if (!proc) continue;
                        try {
                            if (proc.SitePackage && proc.SitePackage.PermissionIdList) {
                                pidList = proc.SitePackage.PermissionIdList;
                                break;
                            }
                            if (proc.SitePackage && Array.isArray(proc.SitePackage.Sites)) {
                                const idx = (Number.isFinite(state.raw._activeSiteIndex) ? state.raw._activeSiteIndex : 0);
                                const site = proc.SitePackage.Sites[idx] || proc.SitePackage.Sites[0];
                                if (site && site.PermissionIdList) {
                                    pidList = site.PermissionIdList;
                                    break;
                                }
                            }
                        } catch (errInner) { /* ignore per-process errors */ }
                    }
                }

                if (!pidList) return result;

                (pidList.DeptIdList || []).forEach((dept) => {
                    result.permDepts[dept.DeptId] = dept.DeptName;
                });
                (pidList.GroupIdList || []).forEach((group) => {
                    result.permGroups[group.GroupId] = group.GroupName;
                });
                (pidList.UserIdList || []).forEach((user) => {
                    const display = user.Name || user.LoginId || user.UserName || user.UserLogin || '';
                    result.permUsers[user.UserId] = display;
                });
            } catch (err) {
                console.warn('permission extraction failed', err);
            }

            return result;
        }

        function extractNotifications(processes) {
            const map = {};
            processes.forEach((proc) => {
                if (!Array.isArray(proc.Notifications) || !proc.Notifications.length) return;
                map[proc.Id] = proc.Notifications.map((n) => ({ ...n }));
            });
            return map;
        }

        function buildColumnLabelMap(columns) {
            const map = {};
            columns.forEach((col) => {
                if (!col || !col.ColumnName) return;
                map[col.ColumnName] = col.LabelText || col.ColumnName;
            });
            return map;
        }

        function initializeProcessChecked() {
            if (!state.model) return;
            const order = state.model.statusList || [];
            const indexOfStatus = (status) => order.indexOf(status);
            state.processChecked = Object.create(null);
            state.defaultProcessChecked = Object.create(null);

            state.model.processes.forEach((proc) => {
                const fromIdx = indexOfStatus(proc.CurrentStatus);
                const toIdx = indexOfStatus(proc.ChangedStatus);
                const isDefaultChecked = fromIdx >= 0 && toIdx >= 0 && fromIdx < toIdx;
                state.defaultProcessChecked[proc.Id] = isDefaultChecked;
                if (fromIdx >= 0 && toIdx >= 0 && fromIdx < toIdx) {
                    state.processChecked[proc.Id] = true;
                } else {
                    state.processChecked[proc.Id] = false;
                }
            });
        }

        function render() {
            if (!state.model) return;
            svg.selectAll('*').remove();
            const allRows = state.model.processes || [];
            const visibleRows = getVisibleProcesses();

            if (allRows.length === 0) {
                svg.style('display', 'none');
                dom.gridAreaMsg.style.display = '';
                const logoSrc2 = document.querySelector('.site-visualizer-header img')?.getAttribute('src') || '';
                const basePath2 = logoSrc2.replace(/images\/.*$/, '');
                dom.gridAreaMsg.innerHTML = `<div class="ps-empty-state">
                    <img class="ps-empty-state-logo" src="${basePath2}images/SiteVisualizer-hayato.svg" alt="" aria-hidden="true" />
                    <p class="ps-empty-state-text">${t('ps_process_not_found')}</p>
                </div>`;
                if (dom.invalidStatusWarning) dom.invalidStatusWarning.style.display = 'none';
                if (dom.mermaidSection) dom.mermaidSection.style.display = 'none';
                if (dom.mermaidWrap) dom.mermaidWrap.innerHTML = '';
                if (dom.gridPngBtn) dom.gridPngBtn.style.display = 'none';
                updatePngButtons(false);
            } else if (visibleRows.length === 0) {
                if (dom.mermaidSection) dom.mermaidSection.style.display = 'none';
                if (dom.mermaidWrap) dom.mermaidWrap.innerHTML = '';
                if (dom.gridPngBtn) dom.gridPngBtn.style.display = 'none';
                svg.style('display', 'none');
                dom.gridAreaMsg.style.display = '';
                const logoSrc = document.querySelector('.site-visualizer-header img')?.getAttribute('src') || '';
                const basePath = logoSrc.replace(/images\/.*$/, '');
                dom.gridAreaMsg.innerHTML = `<div class="ps-empty-state">
                    <img class="ps-empty-state-logo" src="${basePath}images/SiteVisualizer-hayato.svg" alt="" aria-hidden="true" />
                    <p class="ps-empty-state-text">${translateWithFallback('ps_select_processes_prompt', 'プロセスを選択してください。')}</p>
                </div>`;
                if (dom.invalidStatusWarning) dom.invalidStatusWarning.style.display = 'none';
                updatePngButtons(false);
            } else {
                if (dom.mermaidSection) dom.mermaidSection.style.display = '';
                if (dom.gridPngBtn) dom.gridPngBtn.style.display = '';
                svg.style('display', '');
                dom.gridAreaMsg.style.display = 'none';
                dom.gridAreaMsg.innerHTML = '';
                const gridRows = visibleRows.filter(proc =>
                    proc.ChangedStatus !== -1 && proc.CurrentStatus !== proc.ChangedStatus
                );
                renderGrid(gridRows);
                showInvalidStatusWarning();
                updatePngButtons(true);
            }
            if (dom.detailModalBody) {
                dom.detailModalBody.innerHTML = t('ps_select_a_cell');
            }
            updateProcessSelectionUi();
        }

        function renderGrid(rows) {
            const cols = state.model.statusList;
            if (!rows || !cols) return;

            const toggles = {
                showButtons: dom.toggleButtons ? dom.toggleButtons.checked : true,
                showPerms: dom.togglePerms ? dom.togglePerms.checked : true,
                showNotifs: dom.toggleNotifs ? dom.toggleNotifs.checked : true,
            };

            const metrics = computeLayoutMetrics(rows, cols, toggles);
            state.gridBaseWidth = metrics.gridWidth;
            state.gridBaseHeight = metrics.gridHeight;
            svg.attr('viewBox', `0 0 ${metrics.gridWidth} ${metrics.gridHeight}`);
            createArrowMarker();
            drawColumnHeaders(cols, metrics, rows);
            drawRows(rows, toggles, metrics);
            drawTransitions(rows, cols, metrics);
            applyGridZoom();
            generateMermaid();
        }

        function showInvalidStatusWarning() {
            if (!dom.invalidStatusWarning) return;
            const invalid = state.model && state.model.invalidStatuses;
            if (!invalid || invalid.size === 0) {
                dom.invalidStatusWarning.style.display = 'none';
                return;
            }
            const statusLabels = [...invalid].map(s => `「${s}」`).join('、');
            const statusColumnLabel = (state.model && state.model.columnLabelMap && state.model.columnLabelMap['Status']) || translateWithFallback('ps_invalid_status_warning', '状況');
            const warningSuffix = translateWithFallback('ps_invalid_status_warning_suffix', 'が未設定です。選択肢一覧に追加してください。');
            const openSiteSettingsLabel = translateWithFallback('ps_open_site_settings', 'サイト設定を開く');
            let siteUrl = '';
            try {
                const site = state.raw.Sites && state.raw.Sites[state.raw._activeSiteIndex];
                if (site && site.SiteId) {
                    const pathname = window.location.pathname || '';
                    const itemsSegment = '/items/';
                    const itemsIndex = pathname.indexOf(itemsSegment);
                    let applicationPath = itemsIndex >= 0
                        ? pathname.substring(0, itemsIndex)
                        : pathname.replace(/\/$/, '');
                    if (applicationPath === '/') {
                        applicationPath = '';
                    }
                    siteUrl = `${window.location.origin}${applicationPath}/items/${encodeURIComponent(site.SiteId)}`;
                }
            } catch (_) {}
            dom.invalidStatusWarning.className = 'ps-invalid-status-warning';
            dom.invalidStatusWarning.replaceChildren();
            const iconSpan = document.createElement('span');
            iconSpan.className = 'ps-invalid-status-warning-icon';
            iconSpan.setAttribute('aria-hidden', 'true');
            iconSpan.innerHTML =
                `<svg viewBox="0 0 24 24" fill="currentColor" focusable="false" aria-hidden="true">
                    <path d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"/>
                </svg>`;
            const textSpan = document.createElement('span');
            textSpan.className = 'ps-invalid-status-warning-text';
            textSpan.textContent = `${statusColumnLabel} ${statusLabels} ${warningSuffix}`;
            if (siteUrl) {
                textSpan.appendChild(document.createTextNode(' '));
                const siteLink = document.createElement('a');
                siteLink.href = siteUrl;
                siteLink.target = '_blank';
                siteLink.rel = 'noopener';
                siteLink.textContent = openSiteSettingsLabel;
                textSpan.appendChild(siteLink);
            }
            dom.invalidStatusWarning.appendChild(iconSpan);
            dom.invalidStatusWarning.appendChild(textSpan);
            dom.invalidStatusWarning.style.display = '';
        }

        function computeLayoutMetrics(rows, cols, toggles) {
            const colWidth = 190;
            const rowHeight = 48;
            const permGap = 6;

            const permsByRow = mapPermissions(rows);
            const notifsByRow = mapNotifications(rows);

            const leftContentEnd = 4;
            const leftMargin = 8;

            return {
                colWidth,
                rowHeight,
                permGap,
                permsByRow,
                notifsByRow,
                leftContentEnd,
                leftMargin,
                gridWidth: leftMargin + cols.length * colWidth + 40,
                gridHeight: 60 + rows.length * rowHeight + 40,
            };
        }

        function normalizeProcessButtonLabel(labelValue) {
            const text = String(labelValue || '').trim();
            if (!text) return '';
            const maxChars = 10;
            if (text.length <= maxChars) return text;
            return text.slice(0, maxChars - 1) + '…';
        }

        function measureProcessButtonWidth(targetSvg, labelText) {
            const minWidth = 80;
            const maxWidth = 130;
            if (!labelText) return minWidth;

            const g = targetSvg.append('g').attr('class', 'measure-button-single').attr('visibility', 'hidden');
            const text = g.append('text').attr('class', 'ps-proc-button-text').text(labelText);
            let width = minWidth;
            try {
                width = Math.max(minWidth, Math.min(maxWidth, text.node().getBBox().width + 24));
            } catch (err) {
                width = minWidth;
            }
            g.remove();
            return width;
        }

        function fitTextToWidth(targetSvg, textValue, maxWidth) {
            const text = String(textValue || '');
            if (!text) return text;

            const g = targetSvg.append('g').attr('class', 'measure-fit-text').attr('visibility', 'hidden');
            const measureText = g.append('text').attr('class', 'ps-proc-button-text');
            const ellipsis = '…';

            const widthOf = (value) => {
                measureText.text(value);
                try {
                    return measureText.node().getBBox().width;
                } catch (err) {
                    return value.length * 8;
                }
            };

            if (widthOf(text) <= maxWidth) {
                g.remove();
                return text;
            }

            let low = 0;
            let high = text.length;
            let best = '';
            while (low <= high) {
                const mid = Math.floor((low + high) / 2);
                const candidate = text.slice(0, mid) + ellipsis;
                if (widthOf(candidate) <= maxWidth) {
                    best = candidate;
                    low = mid + 1;
                } else {
                    high = mid - 1;
                }
            }

            g.remove();
            return best || ellipsis;
        }

        /**
         * SVG <text> 要素にフルテキストを <title> としてツールチップ付与する。
         * 省略の有無にかかわらず付与してよい（冗長でも安全側）。
         * @param {d3.Selection} textSelection - d3 で選択した <text> 要素
         * @param {string} fullText - ツールチップに表示するフルテキスト
         */
        function attachSvgTooltip(textSelection, fullText) {
            if (!fullText) return;
            textSelection.select('title').remove();
            textSelection.append('title').text(fullText);
        }

        function mapPermissions(rows) {
            return rows.map((row) => {
                const items = [];
                (row.Depts || []).forEach((id) => {
                    const name = state.model.permDepts[id];
                    if (name) items.push({ type: 'dept', name });
                });
                (row.Groups || []).forEach((id) => {
                    const name = state.model.permGroups[id];
                    if (name) items.push({ type: 'group', name });
                });
                (row.Users || []).forEach((id) => {
                    const name = state.model.permUsers[id];
                    if (name) items.push({ type: 'user', name });
                });
                return items;
            });
        }

        function mapNotifications(rows) {
            return rows.map((row) => state.model.notificationsByProcess[row.Id] || []);
        }

        function buildPermissionSummaries(perms) {
            const counts = { dept: 0, group: 0, user: 0 };
            perms.forEach((item) => {
                if (counts[item.type] !== undefined) {
                    counts[item.type] += 1;
                }
            });
            const labels = {
                group: t('ps_perm_group'),
                dept: t('ps_perm_dept'),
                user: t('ps_perm_user'),
            };
            return ['group', 'dept', 'user']
                .map((type) => ({
                    type,
                    label: labels[type],
                    count: counts[type],
                }))
                .filter((item) => item.count > 0);
        }

        function createArrowMarker() {
            const defs = svg.append('defs');

            defs.append('marker')
                .attr('id', 'arrow')
                .attr('viewBox', '0 0 10 10')
                .attr('refX', 8)
                .attr('refY', 5)
                .attr('markerWidth', 6)
                .attr('markerHeight', 6)
                .attr('orient', 'auto')
                .append('path')
                .attr('d', 'M 0 0 L 10 5 L 0 10 z')
                .attr('fill', '#5046e5');

            defs.append('marker')
                .attr('id', 'arrow-backward')
                .attr('viewBox', '0 0 10 10')
                .attr('refX', 8)
                .attr('refY', 5)
                .attr('markerWidth', 6)
                .attr('markerHeight', 6)
                .attr('orient', 'auto')
                .append('path')
                .attr('d', 'M 0 0 L 10 5 L 0 10 z')
                .attr('fill', '#e07050');
        }

        function drawColumnHeaders(cols, metrics, rows) {
            const rowCount = rows.length;
            const headerG = svg.append('g');
            const xFor = (index) => metrics.leftMargin + index * metrics.colWidth + (metrics.colWidth - 8) / 2;
            const defaultHeaderWidth = 102;
            const headerPad = 8;

            const maxHeaderWidth = 130;
            const measureG = svg.append('g').attr('class', 'measure-header').attr('visibility', 'hidden');
            const measureText = measureG.append('text').attr('class', 'ps-col-label');
            const headerWidths = cols.map(status => {
                const label = String(state.model.statusMap[status] || status);
                measureText.text(label);
                let textW;
                try { textW = measureText.node().getBBox().width; } catch (e) { textW = label.length * 12; }
                return Math.min(maxHeaderWidth, Math.max(defaultHeaderWidth, textW + headerPad * 2));
            });
            measureG.remove();
            const headerWidth = Math.max(...headerWidths);

            const gridVisibleRows = rows.filter(proc => proc.ChangedStatus !== -1);
            const sourceStatuses = new Set(gridVisibleRows.map(proc => proc.CurrentStatus));
            const targetStatuses = new Set(gridVisibleRows.map(proc => proc.ChangedStatus));
            const sinkStatuses = new Set(
                cols.filter(status => !sourceStatuses.has(status) && targetStatuses.has(status))
            );

            const invalidSet = state.model.invalidStatuses || new Set();
            function colHeadClass(d) {
                if (invalidSet.has(d)) return 'ps-col-head-box ps-col-head-box-invalid';
                if (sinkStatuses.has(d)) return 'ps-col-head-box ps-col-head-box-sink';
                return 'ps-col-head-box';
            }
            function colLabelClass(d) {
                if (invalidSet.has(d)) return 'ps-col-label ps-col-label-invalid';
                if (sinkStatuses.has(d)) return 'ps-col-label ps-col-label-sink';
                return 'ps-col-label';
            }

            headerG.selectAll('rect.colHeadBox')
                .data(cols)
                .enter()
                .append('rect')
                .attr('x', (d, i) => xFor(i) - (headerWidth / 2))
                .attr('y', 6)
                .attr('width', headerWidth)
                .attr('height', 26)
                .attr('rx', 6)
                .attr('ry', 6)
                .attr('class', colHeadClass);

            headerG.selectAll('text.colHeadLabel')
                .data(cols)
                .enter()
                .append('text')
                .attr('x', (d, i) => xFor(i))
                .attr('y', 19)
                .attr('text-anchor', 'middle')
                .attr('dominant-baseline', 'central')
                .attr('class', colLabelClass)
                .each(function(status) {
                    const fullText = String(state.model.statusMap[status] || status);
                    const maxW = headerWidth - headerPad * 2;
                    const fitted = fitTextToWidth(svg, fullText, maxW);
                    const sel = d3.select(this).text(fitted);
                    attachSvgTooltip(sel, fullText);
                });

            const yTop = 33;
            const yBottom = 40 + rowCount * metrics.rowHeight + metrics.rowHeight / 2;
            headerG.selectAll('line.statusLine')
                .data(cols)
                .enter()
                .append('line')
                .attr('x1', (d, i) => xFor(i))
                .attr('x2', (d, i) => xFor(i))
                .attr('y1', yTop)
                .attr('y2', yBottom)
                .attr('class', d => invalidSet.has(d) ? 'ps-status-line ps-status-line-invalid' : 'ps-status-line');

            metrics.xFor = xFor;
        }

        function drawRows(rows, toggles, metrics) {
            rows.forEach((row) => {
                if (typeof state.processChecked[row.Id] === 'undefined') {
                    state.processChecked[row.Id] = false;
                }
            });
        }

        function drawProcessButton(transG, proc, startX, y, buttonWidth, buttonHeight, buttonLabel, isBackward) {
            const halfButtonWidth = buttonWidth / 2;
            const startButton = transG.append('g')
                .attr('class', 'ps-proc-button-wrap'
                    + (isBackward ? ' ps-proc-button-backward' : ''))
                .attr('transform', `translate(${startX - halfButtonWidth}, ${y - (buttonHeight / 2)})`)
                .style('cursor', 'pointer')
                .on('click', () => showProcessDetail(proc));

            const fullName = String(proc.DisplayName || proc.Name || '').trim();
            if (fullName) {
                startButton.append('title').text(fullName);
            }

            startButton.append('rect')
                .attr('width', buttonWidth)
                .attr('height', buttonHeight)
                .attr('rx', 6)
                .attr('ry', 6)
                .attr('class', 'ps-proc-button');

            const btnText = startButton.append('text')
                .attr('x', buttonWidth / 2)
                .attr('y', buttonHeight / 2 + 0.5)
                .attr('text-anchor', 'middle')
                .attr('dominant-baseline', 'central')
                .attr('class', 'ps-proc-button-text')
                .text(buttonLabel);

            const fullNameTooltip = String(proc.DisplayName || proc.Name || '').trim();
            attachSvgTooltip(btnText, fullNameTooltip);
        }

        function drawNotificationIcon(transG, iconX, y, notifications) {
            const notif = transG.append('g')
                .attr('transform', `translate(${iconX}, ${y - 9})`)
                .attr('class', 'ps-notif');

            notif.append('title').text(`${t('ps_show_notifications')} (${notifications.length})`);

            notif.append('rect')
                .attr('x', 0)
                .attr('y', 0)
                .attr('width', 14)
                .attr('height', 14)
                .attr('rx', 3)
                .attr('ry', 3)
                .attr('class', 'ps-notif-bg');

            notif.append('path')
                .attr('d', 'M2 4 L7 8 L12 4 M2 4 L2 11 L12 11 L12 4')
                .attr('stroke', '#ffffff')
                .attr('stroke-width', 1.1)
                .attr('fill', 'none');
        }

        function drawPermissionIcon(transG, iconX, y, permSummaries) {
            const tooltipText = permSummaries
                .map((item) => t('ps_perm_summary', item.label, item.count))
                .join('\n');

            const perm = transG.append('g')
                .attr('transform', `translate(${iconX}, ${y - 10})`)
                .attr('class', 'ps-perm-icon');

            perm.append('title').text(tooltipText);

            perm.append('rect')
                .attr('rx', 10)
                .attr('ry', 10)
                .attr('width', 20)
                .attr('height', 20)
                .attr('class', 'ps-perm-icon-bg');

            perm.append('path')
                .attr('d', 'M10 2 3 5v5c0 4.53 2.98 8.73 7 10 4.02-1.27 7-5.47 7-10V5l-7-3zm0 9.1c-1.27 0-2.3-1.03-2.3-2.3S8.73 6.5 10 6.5s2.3 1.03 2.3 2.3-1.03 2.3-2.3 2.3zm0 1.35c1.95 0 3.85.95 4.6 2.45H5.4c.75-1.5 2.65-2.45 4.6-2.45z')
                .attr('class', 'ps-perm-icon-glyph');
        }

        function drawTransitionLines(transG, lineFromX, lineToX, y, proc, isBackward) {
            transG.append('line')
                .attr('x1', lineFromX)
                .attr('y1', y)
                .attr('x2', lineToX)
                .attr('y2', y)
                .attr('stroke-width', 3)
                .attr('class', 'ps-transition-line'
                    + (isBackward ? ' ps-transition-line-backward' : ''))
                .attr('marker-end', isBackward
                    ? 'url(#arrow-backward)'
                    : 'url(#arrow)')
                .style('pointer-events', 'none');

            transG.append('line')
                .attr('x1', lineFromX)
                .attr('y1', y)
                .attr('x2', lineToX)
                .attr('y2', y)
                .attr('stroke', 'transparent')
                .attr('stroke-width', 14)
                .attr('stroke-linecap', 'round')
                .style('pointer-events', 'none');
        }

        function drawTransitionNameLabel(transG, lineFromX, lineToX, y, isLeftward, processName, isBackward, proc) {
            if (!processName) {
                return;
            }

            const lineMinX = Math.min(lineFromX, lineToX);
            const lineMaxX = Math.max(lineFromX, lineToX);
            const arrowGap = 20;
            const sideGap = 8;
            const safeMinX = lineMinX + (isLeftward ? arrowGap : sideGap);
            const safeMaxX = lineMaxX - (isLeftward ? sideGap : arrowGap);
            const availableWidth = Math.max(56, safeMaxX - safeMinX);
            const fittedName = fitTextToWidth(svg, processName, Math.max(40, availableWidth - 20));
            const labelWidth = Math.max(64, Math.min(280, availableWidth, fittedName.length * 9 + 22));
            const centerMin = safeMinX + (labelWidth / 2);
            const centerMax = safeMaxX - (labelWidth / 2);
            const baseCenter = (safeMinX + safeMaxX) / 2;
            const centerX = Math.min(Math.max(baseCenter, centerMin), centerMax);
            const labelGroup = transG.append('g')
                .attr('class', 'ps-transition-name')
                .style('cursor', proc ? 'pointer' : 'default')
                .on('click', proc ? () => showProcessDetail(proc) : null);

            labelGroup.append('rect')
                .attr('x', centerX - (labelWidth / 2))
                .attr('y', y - 9)
                .attr('width', labelWidth)
                .attr('height', 18)
                .attr('rx', 4)
                .attr('ry', 4)
                .attr('class', 'ps-transition-name-bg'
                    + (isBackward ? ' ps-transition-name-bg-backward' : ''));

            const labelText = labelGroup.append('text')
                .attr('x', centerX)
                .attr('y', y + 0.5)
                .attr('text-anchor', 'middle')
                .attr('dominant-baseline', 'central')
                .attr('class', 'ps-transition-name-text')
                .text(fittedName);

            attachSvgTooltip(labelText, processName);
        }


        function drawTransitions(rows, cols, metrics) {
            const transG = svg.append('g');
            rows.forEach((proc, rowIndex) => {
                if (proc.ChangedStatus === -1) return;
                if (proc.CurrentStatus === proc.ChangedStatus) return;
                const fromIndex = cols.indexOf(proc.CurrentStatus);
                const toIndex = cols.indexOf(proc.ChangedStatus);
                if (fromIndex < 0 || toIndex < 0) return;

                const isBackward = toIndex < fromIndex
                    || proc.CurrentStatus === proc.ChangedStatus;

                const startX = metrics.xFor(fromIndex);
                const endX = metrics.xFor(toIndex);
                const y = 40 + rowIndex * metrics.rowHeight + metrics.rowHeight / 2;
                const buttonLabel = normalizeProcessButtonLabel(proc.DisplayName || proc.Name);
                const buttonWidth = measureProcessButtonWidth(svg, buttonLabel);
                const buttonHeight = 24;
                const halfButtonWidth = buttonWidth / 2;
                drawProcessButton(transG, proc, startX, y, buttonWidth, buttonHeight, buttonLabel, isBackward);

                const isLeftward = endX < startX;

                const lineFromX = isLeftward
                    ? (startX - halfButtonWidth - 1)
                    : (startX + halfButtonWidth + 1);
                const lineToX = isLeftward ? (endX + 2) : (endX - 2);
                drawTransitionLines(transG, lineFromX, lineToX, y, proc, isBackward);

                const processName = String(proc.Name || proc.DisplayName || '').trim();
                drawTransitionNameLabel(transG, lineFromX, lineToX, y, isLeftward, processName, isBackward, proc);
            });
        }

        function readMermaidLabelMode() {
            try {
                const radio = document.querySelector('input[name="mermaidLabelMode"]:checked');
                return radio ? radio.value : 'display';
            } catch (err) {
                return 'display';
            }
        }

        function encodeMermaidEntities(text) {
            return text
                .replace(/#/g,  '#35;')    // # を先にエスケープ（二重エンコード防止）
                .replace(/"/g,  '#quot;')
                .replace(/&/g,  '#amp;')
                .replace(/</g,  '#lt;')
                .replace(/>/g,  '#gt;')
                .replace(/\[/g, '#91;')
                .replace(/\]/g, '#93;')
                .replace(/\|/g, '#124;');
        }

        function buildMermaidMarkdown(compact = true) {
            const rows = state.model.processes || [];
            const edges = [];
            const nodes = {};
            const labelMode = readMermaidLabelMode();

            const nodeKey = (status) => `N${String(status).replace(/-/g, 'n')}`;

            const numId = (status) => Number(status);

            state.mermaidEdgeLabels = [];
            const nodeStatusMap = {}; // nodeId → raw status value
            rows.forEach((proc) => {
                if (!state.processChecked[proc.Id]) return;
                const fromId = nodeKey(proc.CurrentStatus);
                const effectiveToStatus = (proc.ChangedStatus === -1) ? proc.CurrentStatus : proc.ChangedStatus;
                const toId = nodeKey(effectiveToStatus);
                const baseLabel = labelMode === 'name' ? proc.Name : (proc.DisplayName || proc.Name);
                const fullLabel = baseLabel || `P${proc.Id}`;
                const maxEdgeLabelChars = 14;
                const truncated = fullLabel.length > maxEdgeLabelChars
                    ? fullLabel.slice(0, maxEdgeLabelChars - 1) + '\u2026'
                    : fullLabel;
                const label = encodeMermaidEntities(truncated);

                const fromNum = numId(proc.CurrentStatus);
                const toNum = numId(effectiveToStatus);
                const isSelf = fromId === toId;
                const isBackward = isSelf || toNum < fromNum;

                edges.push({ from: fromId, to: toId, label, fullLabel, isBackward, isSelf });

                if (!(fromId in nodes)) {
                    nodes[fromId] = String(state.model.statusMap[proc.CurrentStatus] ?? proc.CurrentStatus);
                    nodeStatusMap[fromId] = proc.CurrentStatus;
                }
                if (!(toId in nodes)) {
                    nodes[toId] = String(state.model.statusMap[effectiveToStatus] ?? effectiveToStatus);
                    nodeStatusMap[toId] = effectiveToStatus;
                }
            });

            const nodeIds = Object.keys(nodes);
            if (nodeIds.length === 0) return 'flowchart LR\n  empty["(表示するプロセスがありません)"]';

            const compactThreshold = 24;
            const shouldCompactEdges = compact && edges.length > compactThreshold;
            const renderEdges = shouldCompactEdges
                ? (() => {
                    const grouped = Object.create(null);
                    edges.forEach((e) => {
                        const key = `${e.from}|${e.to}|${e.isBackward ? 1 : 0}`;
                        if (!grouped[key]) {
                            grouped[key] = {
                                from: e.from,
                                to: e.to,
                                isBackward: e.isBackward,
                                isSelf: e.isSelf,
                                labels: [],
                                sampleLabel: e.label,
                            };
                        }
                        grouped[key].labels.push(e.fullLabel || e.label);
                    });
                    return Object.values(grouped).map((g) => {
                        const count = g.labels.length;
                        const displayLabel = count > 1 ? `${count}件` : g.sampleLabel;
                        return {
                            from: g.from,
                            to: g.to,
                            isBackward: g.isBackward,
                            isSelf: g.isSelf,
                            label: displayLabel,
                            fullLabel: count > 1 ? g.labels.join('\n') : (g.labels[0] || g.sampleLabel),
                        };
                    });
                })()
                : edges;
            state.mermaidEdgeLabels = renderEdges.map((e) => e.fullLabel || e.label);

            const maxNodeLabelChars = 12;
            state.mermaidNodeLabels = {};
            nodeIds.forEach((id) => {
                state.mermaidNodeLabels[id] = nodes[id]; // 元テキスト（ツールチップ用）
                const truncated = nodes[id].length > maxNodeLabelChars
                    ? nodes[id].slice(0, maxNodeLabelChars - 1) + '\u2026'
                    : nodes[id];
                nodes[id] = encodeMermaidEntities(truncated);
            });

            const outDegree = {};
            nodeIds.forEach((id) => { outDegree[id] = 0; });
            renderEdges.forEach((e) => {
                if (!e.isSelf) outDegree[e.from] = (outDegree[e.from] ?? 0) + 1;
            });

            const sinkSet = new Set(nodeIds.filter((id) => outDegree[id] === 0));

            const lines = ['flowchart LR'];

            nodeIds.forEach((id) => {
                const lbl = nodes[id];
                if (sinkSet.has(id)) {
                    lines.push(`${id}(["${lbl}"])`);
                } else {
                    lines.push(`${id}["${lbl}"]`);
                }
            });

            renderEdges.forEach((e) => {
                if (e.isBackward) {
                    lines.push(`${e.from} -.->|"${e.label}"| ${e.to}`);
                } else {
                    lines.push(`${e.from} -->|"${e.label}"| ${e.to}`);
                }
            });

            const mermaidInvalid = state.model.invalidStatuses || new Set();
            nodeIds.forEach((id) => {
                const rawStatus = nodeStatusMap[id];
                if (mermaidInvalid.has(rawStatus)) {
                    lines.push(`style ${id} fill:#fff5f5,stroke:#d32f2f,stroke-width:2.5px,color:#b71c1c,font-weight:700`);
                } else if (sinkSet.has(id)) {
                    lines.push(`style ${id} fill:#e6f4ea,stroke:#2e7d32,stroke-width:2.5px,color:#1b4a1e,font-weight:700`);
                } else {
                    lines.push(`style ${id} fill:#e3edf8,stroke:#2a6fa8,stroke-width:2px,color:#0d2d46,font-weight:700`);
                }
            });

            renderEdges.forEach((e, i) => {
                if (e.isBackward) {
                    lines.push(`linkStyle ${i} stroke:#e53e3e,stroke-width:2px,stroke-dasharray:6 4`);
                } else {
                    lines.push(`linkStyle ${i} stroke:#2563eb,stroke-width:2.5px`);
                }
            });

            return lines.join('\n');
        }

        function renderMermaidTo(container, markdown) {
            if (!container) return;

            if (container._psMermaidResizeObserver) {
                container._psMermaidResizeObserver.disconnect();
                container._psMermaidResizeObserver = null;
            }
            if (container._psMermaidResizeTimer) {
                clearTimeout(container._psMermaidResizeTimer);
                container._psMermaidResizeTimer = null;
            }

            container.innerHTML = '';

            const renderHost = document.createElement('div');
            renderHost.className = 'ps-mermaid-render-host';
            container.appendChild(renderHost);

            const fitMermaidSvg = () => {
                const svgNode = renderHost.querySelector('svg');
                if (!svgNode) return;

                const vb = svgNode.getAttribute('viewBox');
                let naturalW = Number(svgNode.getAttribute('width')) || 0;
                let naturalH = Number(svgNode.getAttribute('height')) || 0;
                if (vb) {
                    const parts = vb.split(/[\s,]+/).map(Number);
                    if (parts.length >= 4) {
                        if (!naturalW) naturalW = parts[2];
                        if (!naturalH) naturalH = parts[3];
                    }
                } else {
                    if (Number.isFinite(naturalW) && Number.isFinite(naturalH) && naturalW > 0 && naturalH > 0) {
                        svgNode.setAttribute('viewBox', `0 0 ${naturalW} ${naturalH}`);
                    }
                }

                svgNode.removeAttribute('width');
                svgNode.removeAttribute('height');
                svgNode.setAttribute('preserveAspectRatio', 'xMidYMid meet');

                const isFullView = !!container.closest('.ps-mermaid-full-body');

                if (isFullView) {
                    svgNode.style.width = '100%';
                    svgNode.style.height = 'auto';
                    svgNode.style.display = 'block';
                    svgNode.style.margin = '0 auto';
                    return;
                }

                const containerW = container && Number.isFinite(container.clientWidth) ? container.clientWidth - 24 : 0;
                const containerH = container && Number.isFinite(container.clientHeight) ? container.clientHeight - 24 : 0;

                if (naturalW <= 0 || naturalH <= 0 || containerW <= 0 || containerH <= 0) {
                    svgNode.style.width = '100%';
                    svgNode.style.height = 'auto';
                    svgNode.style.display = 'block';
                    svgNode.style.margin = '0 auto';
                    return;
                }

                const scaleW = containerW / naturalW;
                const scaleH = containerH / naturalH;
                const scale = Math.min(scaleW, scaleH, 1.5);

                const displayW = Math.round(naturalW * scale);
                const displayH = Math.round(naturalH * scale);

                svgNode.setAttribute('viewBox', `0 0 ${naturalW} ${naturalH}`);
                svgNode.style.width = `${displayW}px`;
                svgNode.style.height = `${displayH}px`;
                svgNode.style.maxWidth = 'none';
                svgNode.style.maxHeight = 'none';
                svgNode.style.display = 'block';
                svgNode.style.margin = '0 auto';
            };

            if (typeof ResizeObserver !== 'undefined') {
                const observer = new ResizeObserver(() => {
                    if (container._psMermaidResizeTimer) {
                        clearTimeout(container._psMermaidResizeTimer);
                    }
                    container._psMermaidResizeTimer = setTimeout(() => {
                        container._psMermaidResizeTimer = null;
                        fitMermaidSvg();
                    }, 120);
                });
                observer.observe(container);
                container._psMermaidResizeObserver = observer;
            }

            if (window.mermaid && typeof mermaid.render === 'function') {
                const renderId = `m${Date.now().toString(36)}${Math.random().toString(36).slice(2, 8)}`;
                mermaid.render(renderId, markdown)
                    .then(({ svg: svgMarkup }) => {
                        renderHost.innerHTML = svgMarkup;
                        fitMermaidSvg();
                        if (state.mermaidNodeLabels) {
                            renderHost.querySelectorAll('g.node').forEach((g) => {
                                const nodeId = (g.id || '').replace(/^flowchart-/, '').replace(/-\d+$/, '');
                                const fullLabel = state.mermaidNodeLabels[nodeId];
                                if (!fullLabel) return;
                                const titleEl = document.createElementNS('http://www.w3.org/2000/svg', 'title');
                                titleEl.textContent = fullLabel;
                                g.insertBefore(titleEl, g.firstChild);
                            });
                        }
                        if (state.mermaidEdgeLabels && state.mermaidEdgeLabels.length > 0) {
                            renderHost.querySelectorAll('g.edgeLabel').forEach((g, i) => {
                                const fullLabel = state.mermaidEdgeLabels[i];
                                if (!fullLabel) return;
                                const titleEl = document.createElementNS('http://www.w3.org/2000/svg', 'title');
                                titleEl.textContent = fullLabel;
                                g.insertBefore(titleEl, g.firstChild);
                            });
                        }
                        if (container === dom.mermaidWrap) {
                            syncMermaidSectionHeight();
                            requestAnimationFrame(syncMermaidSectionHeight);
                        }
                    })
                    .catch((err) => {
                        console.warn('mermaid render failed', err);
                        renderHost.innerHTML = `<pre style="color:#c00">Mermaid render error:\n${String(err)}</pre>`;
                    });
                return;
            }

            renderHost.innerHTML = `<div class="mermaid">${markdown}</div>`;
            try {
                if (window.mermaid && typeof mermaid.init === 'function') {
                    mermaid.init(undefined, renderHost.querySelectorAll('.mermaid'));
                    fitMermaidSvg();
                    if (container === dom.mermaidWrap) {
                        syncMermaidSectionHeight();
                        requestAnimationFrame(syncMermaidSectionHeight);
                    }
                }
            } catch (err) {
                console.warn('mermaid init failed', err);
            }
        }

        function getStatusLabel(status) {
            if (state.model && state.model.statusMap && status in state.model.statusMap) {
                return `${state.model.statusMap[status]} (${status})`;
            }
            return String(status || '');
        }

        function labelForColumn(columnName) {
            if (state.model && state.model.columnLabelMap && state.model.columnLabelMap[columnName]) {
                return `${state.model.columnLabelMap[columnName]}(${columnName})`;
            }
            return columnName || '';
        }

        function resolveFormattedValue(v, field) {
            const fmt = field.Format;
            const enumMaps = (state.model && state.model.schema && state.model.schema.EnumMaps) || {};
            if (!fmt) return v != null ? String(v) : '';
            if (fmt === 'status') return getStatusLabel(v);
            if (fmt === 'bool')   return v === true ? 'true' : v === false ? 'false' : '';
            if (fmt === 'column') return v != null ? labelForColumn(v) : '';
            if (fmt === 'enum') {
                const val = (v !== null && v !== undefined) ? v : field.DefaultVal;
                if (val === null || val === undefined) return '';
                const map = enumMaps[field.EnumMap] || {};
                return map[String(val)] || String(val);
            }
            return v != null ? String(v) : '';
        }

        function bindMermaidFullView() {
            if (dom.mermaidExpandBtn) {
                dom.mermaidExpandBtn.addEventListener('click', (e) => {
                    e.stopPropagation();
                    openMermaidFullView();
                });
            }
            if (dom.mermaidFullClose) {
                dom.mermaidFullClose.addEventListener('click', closeMermaidFullView);
            }
            if (dom.mermaidFullBackdrop) {
                dom.mermaidFullBackdrop.addEventListener('click', closeMermaidFullView);
            }
        }

        function openMermaidFullView() {
            if (!dom.mermaidFullModal || !dom.mermaidFullBody || !state.model) return;
            dom.mermaidFullBody.innerHTML = '';
            dom.mermaidFullModal.setAttribute('aria-hidden', 'false');
            renderMermaidTo(dom.mermaidFullBody, buildMermaidMarkdown(false));
            requestAnimationFrame(() => {
                const focusable = getFocusableElements(dom.mermaidFullModal);
                if (focusable.length) focusable[0].focus();
            });
        }

        function closeMermaidFullView() {
            if (!dom.mermaidFullModal) return;
            dom.mermaidFullModal.setAttribute('aria-hidden', 'true');
            if (dom.mermaidExpandBtn) dom.mermaidExpandBtn.focus();
        }

        function generateMermaid() {
            const checkedCount = Object.keys(state.processChecked || {}).filter((pid) => !!state.processChecked[pid]).length;
            if (checkedCount === 0) {
                dom.mermaidWrap.innerHTML = '';

                const empty = document.createElement('div');
                empty.className = 'ps-mermaid-empty';
                empty.textContent = translateWithFallback('ps_no_selected_processes', t('ps_none'));
                dom.mermaidWrap.appendChild(empty);

                syncMermaidSectionHeight();
                return;
            }
            renderMermaidTo(dom.mermaidWrap, buildMermaidMarkdown());
            syncMermaidSectionHeight();
        }

        function getFocusableElements(container) {
            if (!container) return [];
            const elements = container.querySelectorAll('button, [href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])');
            return Array.from(elements).filter((el) => el.offsetParent !== null);
        }

        function trapFocusInModal(container, event) {
            const focusables = getFocusableElements(container);
            if (!focusables.length) return;

            const first = focusables[0];
            const last = focusables[focusables.length - 1];
            const active = document.activeElement;

            if (!event.shiftKey && active === last) {
                event.preventDefault();
                first.focus();
                return;
            }

            if (event.shiftKey && active === first) {
                event.preventDefault();
                last.focus();
            }
        }

        function showProcessDetail(proc) {
            if (!dom.detailModalBody || !dom.detailModal) return;
            dom.detailModalBody.innerHTML = '';
            const container = document.createElement('div');
            container.className = 'ps-process-detail';

            const title = document.createElement('h3');
            title.textContent = proc.Name || proc.DisplayName || t('ps_process_fallback', proc.Id);
            container.appendChild(title);

            const schema = state.model.schema;
            const generalRows = (schema && schema.GeneralFields || []).map(f => [
                f.Label,
                resolveFormattedValue(proc[f.Key], f)
            ]);
            const generalSection = document.createElement('div');
            generalSection.className = 'ps-validateInputs';
            const generalHeading = document.createElement('strong');
            generalHeading.textContent = (schema && schema.GeneralTitle) || t('ps_col_group_general');
            generalSection.appendChild(generalHeading);
            const table = document.createElement('table');
            table.className = 'ps-pdTable';
            const tbody = document.createElement('tbody');
            generalRows.forEach((row) => {
                const tr = document.createElement('tr');
                const th = document.createElement('th');
                th.textContent = row[0];
                const td = document.createElement('td');
                td.textContent = row[1] != null ? String(row[1]) : '';
                tr.appendChild(th);
                tr.appendChild(td);
                tbody.appendChild(tr);
            });
            table.appendChild(tbody);
            generalSection.appendChild(table);
            container.appendChild(generalSection);

            container.appendChild(renderProcessDetailTwoLineTable(proc));
            dom.detailModalBody.appendChild(container);
            dom.detailModal.setAttribute('aria-hidden', 'false');
            dom.detailModalBody.scrollTop = 0;
            if (dom.detailModalClose) {
                dom.detailModalClose.focus();
            }
        }

        function resolveProcessArray(source, keyCandidates) {
            if (!source || typeof source !== 'object') return [];

            const keyMap = {};
            Object.keys(source).forEach((key) => {
                keyMap[key.toLowerCase()] = key;
            });

            for (let i = 0; i < keyCandidates.length; i++) {
                const requested = keyCandidates[i];
                const actualKey = keyMap[String(requested).toLowerCase()] || requested;
                const value = source[actualKey];
                if (Array.isArray(value)) return value;
                if (!value || typeof value !== 'object') continue;
                if (Array.isArray(value.Items)) return value.Items;
                if (Array.isArray(value.List)) return value.List;
                if (Array.isArray(value.Values)) return value.Values;
                return [value];
            }

            return [];
        }

        function renderPermissionDetails(proc) {
            const wrapper = document.createElement('div');
            wrapper.className = 'ps-permissions-detail';

            const schema = state.model.schema;
            const permGroups = (schema && schema.PermissionGroups) || [];
            const lookupMap = {
                Groups: state.model.permGroups,
                Depts:  state.model.permDepts,
                Users:  state.model.permUsers,
            };

            permGroups.forEach(pg => {
                if (pg.Title) {
                    const heading = document.createElement('strong');
                    heading.textContent = pg.Title;
                    wrapper.appendChild(heading);
                }
                wrapper.appendChild(buildPermissionSection(
                    proc[pg.DataKey] || [],
                    lookupMap[pg.DataKey] || {},
                    pg.IdLabel,
                    pg.NameLabel
                ));
            });

            return wrapper;
        }

        function buildPermissionSection(ids, lookup, idLabel, nameLabel) {
            const section = document.createElement('div');
            section.className = 'ps-permissions-section';

            const table = document.createElement('table');
            table.className = 'ps-permissions-table';
            const thead = document.createElement('thead');
            const headerRow = document.createElement('tr');
            [idLabel, nameLabel].forEach((text) => {
                const th = document.createElement('th');
                th.textContent = text;
                headerRow.appendChild(th);
            });
            thead.appendChild(headerRow);
            table.appendChild(thead);

            const tbody = document.createElement('tbody');
            if (!ids.length) {
                const tr = document.createElement('tr');
                const td = document.createElement('td');
                td.colSpan = 2;
                td.textContent = t('ps_none');
                tr.appendChild(td);
                tbody.appendChild(tr);
            } else {
                ids.forEach((id) => {
                    const tr = document.createElement('tr');
                    const idCell = document.createElement('td');
                    idCell.textContent = id;
                    const nameCell = document.createElement('td');
                    nameCell.textContent = lookup[id] || '';
                    tr.appendChild(idCell);
                    tr.appendChild(nameCell);
                    tbody.appendChild(tr);
                });
            }
            table.appendChild(tbody);
            section.appendChild(table);
            return section;
        }

        function renderProcessDetailTwoLineTable(proc) {
            const TOP  = 'top';
            const NEST = 'nest';

            const schema = state.model.schema || {};
            const arrayCandidates = schema.ArrayCandidates || {};
            const sectionDefs = schema.Sections || [];

            const nestedArrays = {};
            Object.entries(arrayCandidates).forEach(([k, candidates]) => {
                nestedArrays[k] = resolveProcessArray(proc, candidates);
            });

            if (nestedArrays.DataChanges && nestedArrays.DataChanges.length) {
                nestedArrays.DataChanges = nestedArrays.DataChanges.map(raw => {
                    if (!raw || typeof raw !== 'object') return raw;
                    const type = raw.Type || '';
                    const isColumn = type === 'CopyValue' || type === 'CopyDisplayValue';
                    const isValue = type === 'InputValue';
                    const isFormula = type === 'InputValueFormula';
                    const isDate = type === 'InputDate';
                    const isDateTime = type === 'InputDateTime';
                    const isDT = isDate || isDateTime;
                    const val = raw.Value || '';
                    const parts = isDT ? String(val).split(',') : [];
                    return {
                        ...raw,
                        _Type: type,
                        _CopyFrom: isColumn ? val : null,
                        _Value: isValue ? val : null,
                        _Formula: isFormula ? val : null,
                        _NotUseDisplayName: isFormula ? (raw.ValueFormulaNotUseDisplayName === true) : null,
                        _IsDisplayError: isFormula ? (raw.ValueFormulaIsDisplayError === true) : null,
                        _BaseDateTime: isDT ? (raw.BaseDateTime || (isDate ? 'CurrentDate' : 'CurrentTime')) : null,
                        _DateTimeValue: isDT ? (parts[0] || '0') : null,
                        _Period: isDT ? (parts[1] || null) : null,
                    };
                });
            }

            function fmtTop(col) {
                return resolveFormattedValue(proc[col.Key], col);
            }
            function fmtNest(item, col) {
                if (!item) return '';
                let v = item[col.Key];
                if ((v === null || v === undefined) && col.DefaultVal !== undefined) v = col.DefaultVal;
                if (col.Format === 'columnFilterHash') return buildColumnFilterHashNode(v);
                return resolveFormattedValue(v, col);
            }

            function buildColumnFilterHashNode(hashObj) {
                const frag = document.createDocumentFragment();
                if (!hashObj || typeof hashObj !== 'object' || Array.isArray(hashObj)) return frag;
                Object.entries(hashObj).forEach(([colName, filterVal]) => {
                    const colLabel = labelForColumn(colName);
                    const valDisplay = formatRawFilterValue(filterVal);
                    const chip = document.createElement('span');
                    chip.className = 'ps-filter-chip';
                    const labelSpan = document.createElement('span');
                    labelSpan.className = 'ps-filter-chip-label';
                    labelSpan.textContent = colLabel;
                    chip.appendChild(labelSpan);
                    if (valDisplay) {
                        const valSpan = document.createElement('span');
                        valSpan.className = 'ps-filter-chip-val';
                        valSpan.textContent = valDisplay;
                        chip.appendChild(valSpan);
                    }
                    frag.appendChild(chip);
                });
                return frag;
            }

            function formatRawFilterValue(val) {
                if (!val || typeof val !== 'string') return '';
                const trimmed = val.trim();
                if (trimmed.startsWith('[')) {
                    try {
                        const arr = JSON.parse(trimmed);
                        if (Array.isArray(arr) && arr.length === 2) {
                            const s = String(arr[0] ?? '').trim();
                            const e = String(arr[1] ?? '').trim();
                            if (s || e) return `${s} - ${e}`;
                            return '';
                        }
                        if (Array.isArray(arr)) {
                            return arr.filter(v => v !== '' && v != null).join(', ');
                        }
                    } catch (_) {}
                }
                return trimmed;
            }

            function buildGroupTable(sectionDef) {
                const title = sectionDef.Title;
                const cols = sectionDef.Columns || [];

                const nestKeys = [...new Set(
                    cols.filter(c => c.Type === NEST).map(c => c.ArrayKey)
                )];
                const nestLen = nestKeys.length
                    ? Math.max(...nestKeys.map(k => (nestedArrays[k] || []).length))
                    : 0;
                const hasNest  = nestKeys.length > 0;
                const maxRows  = hasNest ? nestLen : 1;

                const wrapper = document.createElement('div');
                wrapper.className = 'ps-validateInputs';

                const heading = document.createElement('strong');
                heading.textContent = title;
                wrapper.appendChild(heading);

                const isEmptyConditions = hasNest && nestLen > 0 && nestKeys.every(k => {
                    const arr = nestedArrays[k] || [];
                    return arr.every(item => {
                        if (!item || typeof item !== 'object') return true;
                        return cols.filter(c => c.Type === NEST && c.ArrayKey === k).every(c => {
                            const v = item[c.Key];
                            return v === null || v === undefined || v === '';
                        });
                    });
                });
                if (hasNest && (nestLen === 0 || isEmptyConditions)) {
                    const emptyTable = document.createElement('table');
                    emptyTable.className = 'ps-viTable';
                    const emptyThead = document.createElement('thead');
                    const emptyHeaderRow = document.createElement('tr');
                    cols.forEach(col => {
                        const th = document.createElement('th');
                        th.textContent = col.Label;
                        emptyHeaderRow.appendChild(th);
                    });
                    emptyThead.appendChild(emptyHeaderRow);
                    emptyTable.appendChild(emptyThead);
                    const emptyTbody = document.createElement('tbody');
                    const emptyTr = document.createElement('tr');
                    const emptyTd = document.createElement('td');
                    emptyTd.colSpan = cols.length;
                    emptyTd.textContent = t('ps_none');
                    emptyTr.appendChild(emptyTd);
                    emptyTbody.appendChild(emptyTr);
                    emptyTable.appendChild(emptyTbody);
                    wrapper.appendChild(emptyTable);
                    return wrapper;
                }

                const table = document.createElement('table');
                table.className = 'ps-viTable';

                const thead = document.createElement('thead');
                const headerRow = document.createElement('tr');
                cols.forEach(col => {
                    const th = document.createElement('th');
                    th.textContent = col.Label;
                    headerRow.appendChild(th);
                });
                thead.appendChild(headerRow);
                table.appendChild(thead);

                const tbody = document.createElement('tbody');
                for (let rowIdx = 0; rowIdx < maxRows; rowIdx++) {
                    const tr = document.createElement('tr');
                    cols.forEach(col => {
                        if (col.Type === TOP) {
                            if (rowIdx === 0) {
                                const td = document.createElement('td');
                                if (maxRows > 1) td.rowSpan = maxRows;
                                td.textContent = fmtTop(col);
                                tr.appendChild(td);
                            }
                        } else {
                            const arr = nestedArrays[col.ArrayKey] || [];
                            const td = document.createElement('td');
                            if (rowIdx < arr.length) {
                                const result = fmtNest(arr[rowIdx], col);
                                if (result instanceof Node) {
                                    td.appendChild(result);
                                } else {
                                    td.textContent = result;
                                }
                            }
                            tr.appendChild(td);
                        }
                    });
                    tbody.appendChild(tr);
                }
                table.appendChild(tbody);

                const scrollDiv = document.createElement('div');
                scrollDiv.appendChild(table);
                wrapper.appendChild(scrollDiv);
                return wrapper;
            }

            const fragment = document.createDocumentFragment();
            sectionDefs.forEach(sectionDef => {
                fragment.appendChild(buildGroupTable(sectionDef));
            });
            fragment.appendChild(renderPermissionDetails(proc));
            return fragment;
        }

        function appendCell(row, textContent) {
            const td = document.createElement('td');
            td.textContent = textContent;
            row.appendChild(td);
        }
    }
}
