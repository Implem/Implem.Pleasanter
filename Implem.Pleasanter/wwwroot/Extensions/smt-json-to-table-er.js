class ERDiagram {
    constructor() {
        document.title = `Pleasanter Site Visualizer`;
        this.currentMermaidText = '';
        this.zoomScale = 1.0;
        this.baseZoomScale = 1.0;
        const container = document.getElementById('erDiagramContainer');
        container.innerHTML = `
            <div style="margin-bottom:8px;">
                <div style="display:inline-flex;gap:4px;vertical-align:middle;">
                    <button id="erZoomInBtn" class="er-tool-btn" title="${t('erd_zoom_in')}">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <line x1="12" y1="5" x2="12" y2="19" />
                            <line x1="5" y1="12" x2="19" y2="12" />
                        </svg>
                    </button>
                    <button id="erZoomOutBtn" class="er-tool-btn" title="${t('erd_zoom_out')}">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <line x1="5" y1="12" x2="19" y2="12" />
                        </svg>
                    </button>
                    <button id="erZoomResetBtn" class="er-tool-btn" title="${t('erd_zoom_reset')}">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M3 12a9 9 0 0 1 9-9 9.75 9.75 0 0 1 6.74 2.74L21 8" />
                            <path d="M21 3v5h-5" />
                            <path d="M21 12a9 9 0 0 1-9 9 9.75 9.75 0 0 1-6.74-2.74L3 16" />
                            <path d="M3 21v-5h5" />
                        </svg>
                    </button>
                    <button id="erPngDownloadBtn" class="er-tool-btn er-tool-save-btn" title="${t('erd_save_png')}">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                            <polyline points="7,10 12,15 17,10" />
                            <line x1="12" y1="15" x2="12" y2="3" />
                        </svg>&nbsp;<span>PNG</span>
                    </button>
                    <button id="erSearchBtn" class="er-tool-btn er-tool-search-btn" title="${t('erd_text_search')}">
                        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <circle cx="11" cy="11" r="8" />
                            <path d="M21 21l-4.35-4.35" />
                            <path d="M7 8h8M11 8v6" stroke-width="2.5" />
                        </svg>
                    </button>
                </div>
            </div>
            <div id="mermaidScrollWrapper" style="width:100%;height:480px;overflow:auto;border:1px solid #ccc;cursor:grab;">
                <div id="mermaidRenderArea"></div>
            </div>
        `;
        this.bind();
        this.erSearch = new ERDiagram.ERSearch(this);
    }
    renderFromData(data) {
        try {
            const erTables = this.extractTables(data);
            const mermaidText = erTables.length ? this.toMermaid(erTables) : t('erd_no_data');
            document.title = `${t('erd_title')} - Pleasanter Site Visualizer`;
            this.currentMermaidText = mermaidText;
            const renderArea = document.getElementById('mermaidRenderArea');
            if (renderArea) renderArea.innerHTML = '';
            this.zoomScale = 1.0;
            this.applyZoom();
            if (erTables.length) {
                mermaid.initialize({ startOnLoad: false, securityLevel: 'strict' });
                mermaid.render('erDiagramSvg', mermaidText)
                    .then(({ svg }) => {
                        if (renderArea) renderArea.innerHTML = svg;
                        this.fitToContainer();
                    })
                    .catch(e => {
                        if (renderArea) renderArea.innerHTML = `<span style="color:red;">${t('erd_mermaid_error', Common.escapeHtml(e.message))}</span>`;
                    });
            }
        } catch (ex) {
            console.error('ERDiagram.renderFromData error', ex);
        }
    }
    bind() {
        const self = this;
        document.getElementById('erZoomInBtn').onclick = () => {
            this.zoomScale = Math.min(this.zoomScale * 1.2, 5);
            this.applyZoom();
        };
        document.getElementById('erZoomOutBtn').onclick = () => {
            this.zoomScale = Math.max(this.zoomScale / 1.2, 0.2);
            this.applyZoom();
        };
        document.getElementById('erZoomResetBtn').onclick = () => {
            this.zoomScale = this.baseZoomScale;
            this.applyZoom();
            this.fitToContainer();
        };
        document.getElementById('erPngDownloadBtn').onclick = () => {
            const svg = document.querySelector('#mermaidRenderArea svg');
            if (!svg) return;
            const svgClone = svg.cloneNode(true);
            svgClone.style.position = 'absolute';
            svgClone.style.left = '-9999px';
            svgClone.style.top = '-9999px';
            svgClone.style.visibility = 'hidden';
            svgClone.style.transform = '';
            svgClone.style.transformOrigin = '';
            document.body.appendChild(svgClone);
            setTimeout(() => {
                if (svgClone.parentNode) svgClone.parentNode.removeChild(svgClone);
            }, 1000);
            let rect = svgClone.getBoundingClientRect();
            let screenW = rect.width;
            let screenH = rect.height;
            let bbox = (typeof svgClone.getBBox === 'function') ? svgClone.getBBox() : { x: 0, y: 0, width: screenW, height: screenH };
            const dlgBg = document.createElement('div');
            dlgBg.className = 'settings-dialog-bg';
            dlgBg.style.zIndex = 9998;
            dlgBg.onclick = null;
            document.body.appendChild(dlgBg);

            const dlg = document.createElement('div');
            dlg.className = 'settings-dialog';
            dlg.style.zIndex = 9999;
            dlg.innerHTML = `
                     <div class="settings-dialog-title">${t('erd_png_export')}</div>
                     <div class="png-dialog-label">
                         <label>${t('erd_png_width')}: <input type="number" id="pngWidthInput" value="${Math.round(screenW)}"></label>
                         <label>${t('erd_png_height')}: <input type="number" id="pngHeightInput" value="${Math.round(screenH)}"></label>
                     </div>
                     <div class="png-dialog-priority">
                         <label><input type="radio" name="pngPriority" value="width" checked> ${t('erd_png_priority_width')}</label>
                         <label><input type="radio" name="pngPriority" value="height"> ${t('erd_png_priority_height')}</label>
                     </div>
                     <div class="png-dialog-actions">
                         <button id="pngExportExecBtn">Download</button>
                         <button id="pngExportCancelBtn">Cancel</button>
                     </div>
                `;
            document.body.appendChild(dlg);
            const widthInput = dlg.querySelector('#pngWidthInput');
            const heightInput = dlg.querySelector('#pngHeightInput');
            const radios = dlg.querySelectorAll('input[name="pngPriority"]');
            function syncSize() {
                let w = parseInt(widthInput.value, 10) || screenW;
                let h = parseInt(heightInput.value, 10) || screenH;
                const priority = Array.from(radios).find(r => r.checked)?.value;
                if (priority === 'width') {
                    h = Math.round(w * screenH / screenW);
                    heightInput.value = h;
                } else {
                    w = Math.round(h * screenW / screenH);
                    widthInput.value = w;
                }
            }
            widthInput.oninput = () => { if (radios[0].checked) syncSize(); };
            heightInput.oninput = () => { if (radios[1].checked) syncSize(); };
            radios.forEach(r => r.onchange = syncSize);
            dlg.querySelector('#pngExportExecBtn').onclick = () => {
                let w = parseInt(widthInput.value, 10) || screenW;
                let h = parseInt(heightInput.value, 10) || screenH;
                const serializer = new XMLSerializer();
                const svgClone = svg.cloneNode(true);
                svgClone.style.transform = '';
                svgClone.style.transformOrigin = '';
                svgClone.setAttribute('width', screenW);
                svgClone.setAttribute('height', screenH);
                svgClone.setAttribute('viewBox', `${bbox.x} ${bbox.y} ${bbox.width} ${bbox.height}`);
                const svgText = serializer.serializeToString(svgClone);
                const svgBase64 = btoa(unescape(encodeURIComponent(svgText)));
                const img = new Image();
                img.onload = () => {
                    const canvas = document.createElement('canvas');
                    canvas.width = w;
                    canvas.height = h;
                    const ctx = canvas.getContext('2d');
                    ctx.fillStyle = '#fff';
                    ctx.fillRect(0, 0, w, h);
                    const svgMargin = 8;
                    ctx.drawImage(
                        img,
                        bbox.x - svgMargin, bbox.y - svgMargin * 2, bbox.width + svgMargin * 2, bbox.height + svgMargin * 4,
                        0, 0, w, h
                    );
                    canvas.toBlob(blob => {
                        const pngUrl = URL.createObjectURL(blob);
                        const a = document.createElement('a');
                        a.href = pngUrl;
                        a.download = vsFilename('ERD', 'PNG');
                        document.body.appendChild(a); a.click();
                        setTimeout(() => {
                            document.body.removeChild(a);
                            URL.revokeObjectURL(pngUrl);
                        }, 100);
                    }, 'image/png');
                };
                img.width = w;
                img.height = h;
                img.src = 'data:image/svg+xml;base64,' + svgBase64;
                dlg.remove();
                dlgBg.remove();
            };
            dlg.querySelector('#pngExportCancelBtn').onclick = () => {
                dlg.remove();
                dlgBg.remove();
            };
        };
        (() => {
            const wrapper = document.getElementById('mermaidScrollWrapper');
            let isDragging = false, startX = 0, startY = 0, scrollLeft = 0, scrollTop = 0;
            wrapper.addEventListener('mousedown', e => {
                if (e.altKey) {
                    wrapper.style.cursor = 'text';
                    try {
                        const cx = e.clientX, cy = e.clientY;
                        let range = null;
                        if (document.caretPositionFromPoint) {
                            const pos = document.caretPositionFromPoint(cx, cy);
                            if (pos && pos.offsetNode) {
                                range = document.createRange();
                                range.setStart(pos.offsetNode, pos.offset);
                                range.collapse(true);
                            }
                        }
                        if (!range && document.caretRangeFromPoint) {
                            range = document.caretRangeFromPoint(cx, cy);
                        }
                        if (range) {
                            const sel = window.getSelection();
                            if (sel) {
                                sel.removeAllRanges();
                                sel.addRange(range);
                            }
                        }
                    } catch (ex) {}
                    return;
                }
                isDragging = true;
                wrapper.style.cursor = 'grabbing';
                startX = e.pageX - wrapper.offsetLeft;
                startY = e.pageY - wrapper.offsetTop;
                scrollLeft = wrapper.scrollLeft;
                scrollTop = wrapper.scrollTop;
                e.preventDefault();
            });
            wrapper.addEventListener('mousemove', e => {
                if (isDragging) return;
                wrapper.style.cursor = e.altKey ? 'text' : 'grab';
            });
            document.addEventListener('mousemove', e => {
                if (!isDragging) return;
                const x = e.pageX - wrapper.offsetLeft;
                const y = e.pageY - wrapper.offsetTop;
                wrapper.scrollLeft = scrollLeft - (x - startX);
                wrapper.scrollTop = scrollTop - (y - startY);
            });
            document.addEventListener('mouseup', () => {
                isDragging = false;
                wrapper.style.cursor = 'grab';
            });
            document.addEventListener('keydown', e => {
                if (e.key === 'Alt' && !isDragging) {
                    try { if (wrapper.matches(':hover')) wrapper.style.cursor = 'text'; } catch (ex) { }
                }
            });
            document.addEventListener('keyup', e => {
                if (e.key === 'Alt' && !isDragging) {
                    try { if (wrapper.matches(':hover')) wrapper.style.cursor = 'grab'; } catch (ex) { }
                }
            });
        })();
        (() => {
            const wrapper = document.getElementById('mermaidScrollWrapper');
            if (!wrapper) return;
            wrapper.addEventListener('wheel', (e) => {
                const svg = document.querySelector('#mermaidRenderArea svg');
                if (!svg) return;
                const scaleStep = 1.15;
                let newScale;
                if (e.deltaY < 0) {
                    newScale = Math.min((self.zoomScale ?? 1) * scaleStep, 5);
                } else {
                    newScale = Math.max((self.zoomScale ?? 1) / scaleStep, 0.2);
                }
                self.zoomScale = newScale;
                self.applyZoom();
                e.preventDefault();
            }, { passive: false });
        })();
    }
    getZoomScale() { return this.zoomScale; }
    applyZoom() {
        const svg = document.querySelector('#mermaidRenderArea svg');
        if (svg) {
            svg.style.transform = `scale(${this.zoomScale})`;
            svg.style.transformOrigin = '0 0';
        }
    }
    fitToContainer() {
        const wrapper = document.getElementById('mermaidScrollWrapper');
        const svg = document.querySelector('#mermaidRenderArea svg');
        if (!wrapper || !svg) return;
        const current = this.zoomScale || 1;
        requestAnimationFrame(() => {
            const rect = svg.getBoundingClientRect();
            const naturalW = rect.width / current || 1;
            const naturalH = rect.height / current || 1;
            const sW = wrapper.clientWidth / naturalW;
            const sH = wrapper.clientHeight / naturalH;
            let fit = Math.min(sW, sH);
            if (!isFinite(fit) || fit <= 0) fit = 1;
            fit = Math.max(0.2, Math.min(fit, 5));
            this.baseZoomScale = fit;
            this.zoomScale = fit;
            this.applyZoom();
            wrapper.scrollLeft = 0;
            wrapper.scrollTop = 0;
        });
    }
    extractTables(data) {
        return (data.ERDiagrams && Array.isArray(data.ERDiagrams.Tables)) ? data.ERDiagrams.Tables : [];
    }
    toMermaid(tables) {
        function escapeMermaid(v) {
            return String(v ?? '').replace(/[ \t]/g, '_').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/[\n\r]/g, ' ').replace(/"/g, "'").replace(/[\[\]{}|`]/g, '');
        }
        let lines = ['erDiagram'];
        lines.push(`
                %%{init:{
                    "theme": "base",
                    "themeVariables": {
                        "primaryColor": "#e0eFF8",
                        "primaryTextColor": "#000",
                        "primaryBorderColor": "#1565c0",
                        "nodeTextColor": "#000",
                        "lineColor": "#000",
                        "secondaryColor": "#e3f2fd",
                        "tertiaryColor": "#bbdefb",
                        "fontFamily": "Courier New, Arial, sans-serif"
                    }
                }}%%
            `);
        tables.forEach(table => {
            let cols = (table.Columns || []).map(col => {
                const k = (col.Type == 'ID') ? 'PK' : (col.RelationSiteId ? 'FK' : '');
                return `  ${escapeMermaid(col.TypeRaw)} ${escapeMermaid(col.Type)} ${k} "${escapeMermaid(col.Label)}"`;
            }).join('\n');
            lines.push(`  TBL_${table.SiteId}["${escapeMermaid(table.Title)}(${table.SiteId})"] {\n${cols}\n}`);
        });
        tables.forEach(table => {
            (table.Columns || []).forEach(col => {
                if (col.RelationSiteId) {
                    const parent = tables.find(t => t.SiteId === col.RelationSiteId);
                    if (parent) {
                        lines.push(`  TBL_${parent.SiteId} |o--o{ TBL_${table.SiteId} : "${escapeMermaid(col.Label)}"`);
                    }
                }
            });
        });
        return lines.join('\n');
    }
}
ERDiagram.ERSearch = class {
    constructor(erDiagram) {
        this.er = erDiagram;
        this.searchDialog = null;
        this.selectedSvgElem = null;
        const btn = document.getElementById('erSearchBtn');
        if (btn) btn.onclick = () => this.show();
    }
    show() {
        this.close();
        const dlg = document.createElement('div');
        dlg.className = 'settings-dialog';
        dlg.id = 'erSearchDialog';
        dlg.style.top = '120px';
        dlg.innerHTML = `
                <div class="settings-dialog-title" style="cursor:move;">${t('erd_search_dialog_title')}</div>
                <span class="settings-dialog-close" title="Close">&times;</span>
                <div id="erSearchInputArea">
                    <input type="text" id="erSearchInput" placeholder="${t('erd_search_placeholder')}">
                    <button id="erSearchExecBtn">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <circle cx="11" cy="11" r="8"/>
                            <line x1="21" y1="21" x2="16.65" y2="16.65"/>
                            <path d="M21 21l-4.35-4.35" />
                            <path d="M7 8h8M11 8v6" stroke-width="2.5" />
                        </svg>
                    </button>
                </div>
                <div id="erSearchResultArea"></div>
            `;
        document.body.appendChild(dlg);
        this.searchDialog = dlg;
        dlg.querySelector('.settings-dialog-close').onclick = () => this.close();
        dlg.querySelector('#erSearchExecBtn').onclick = () => {
            const keyword = dlg.querySelector('#erSearchInput').value.trim();
            this.renderResults(keyword);
            if (parseInt(dlg.style.top) < 176) dlg.style.top = '176px';
        };
        dlg.querySelector('#erSearchInput').onkeydown = (e) => {
            if (e.key === 'Enter') dlg.querySelector('#erSearchExecBtn').click();
        };
        let isDragging = false, off = { x: 0, y: 0 };
        const titleBar = dlg.querySelector('.settings-dialog-title');
        titleBar.onmousedown = (e) => {
            isDragging = true;
            off.x = e.clientX - dlg.offsetLeft;
            off.y = e.clientY - dlg.offsetTop;
            document.body.style.userSelect = 'none';
        };
        const onMove = (e) => {
            if (!isDragging) return;
            dlg.style.left = (e.clientX - off.x) + 'px';
            dlg.style.top = (e.clientY - off.y) + 'px';
        };
        const onUp = () => { isDragging = false; document.body.style.userSelect = ''; };
        document.addEventListener('mousemove', onMove);
        document.addEventListener('mouseup', onUp);
        setTimeout(() => { document.addEventListener('keydown', this._onEsc, { capture: true }); }, 0);
        this._cleanup = () => {
            document.removeEventListener('mousemove', onMove);
            document.removeEventListener('mouseup', onUp);
            document.removeEventListener('keydown', this._onEsc, { capture: true });
        };
    }
    _onEsc = (e) => { if (e.key === 'Escape') this.close(); }
    close() {
        if (this.searchDialog) {
            this._cleanup && this._cleanup();
            this.searchDialog.remove();
            this.searchDialog = null;
        }
        this.clearHighlight();
    }
    renderResults(keyword) {
        const area = this.searchDialog.querySelector('#erSearchResultArea');
        area.innerHTML = '';
        if (!keyword) { area.innerHTML = `<span class="er-search-result-none">${t('erd_search_input')}</span>`; return; }
        const svg = document.querySelector('#mermaidRenderArea svg');
        if (!svg) { area.innerHTML = `<span style="color:red;">${t('erd_not_displayed')}</span>`; return; }
        const keywords = keyword.normalize('NFKC').toLowerCase().split(/\s+/).filter(k => k.length > 0);
        let results = [];
        const rootG = svg.querySelector('g.root');
        if (rootG) {
            const nodeGs = Array.from(rootG.querySelectorAll('g.node'));
            nodeGs.forEach(g => {
                const ps = g.querySelectorAll('span.nodeLabel p');
                if (ps && ps.length > 0) {
                    const tableNameRaw = ps[0].textContent.trim();
                    const tableName = tableNameRaw.normalize('NFKC').toLowerCase();
                    const svgTextElem = ps[0].closest('text');
                    if (keywords.every(k => tableName.includes(k))) {
                        results.push({ type: 'table', name: tableNameRaw, svgElem: ps[0], svgTextElem, parentG: g, idx: results.length });
                    }
                    for (let i = 1; i < ps.length; i++) {
                        const colNameRaw = ps[i].textContent.trim();
                        const colName = colNameRaw.normalize('NFKC').toLowerCase();
                        const svgTextElemCol = ps[i].closest('text');
                        if (keywords.every(k => colName.includes(k))) {
                            results.push({ type: 'column', name: colNameRaw, svgElem: ps[i], svgTextElem: svgTextElemCol, parentG: g, tableName: tableNameRaw, idx: results.length });
                        }
                    }
                }
            });
        }
        if (results.length === 0) { area.innerHTML = `<span class="er-search-result-none">${t('erd_search_none')}</span>`; return; }
        let html = `<div class="er-search-result-count">${t('erd_search_hit',results.length)}</div><ul style="list-style:none;padding:0;margin:0;">`;
        results.forEach((r, i) => {
            html += `<li>
                    <button class="er-search-result-btn" data-idx="${i}">
                        <span class="${r.type === 'table' ? 'er-search-result-type-table' : 'er-search-result-type-column'}">
                            ${r.type === 'table' ? t('erd_table') : t('erd_column')} ${Common.escapeHtml(r.name)}
                            ${r.tableName ? `<span class="er-search-result-table-name">（${Common.escapeHtml(r.tableName)}）</span>` : ''}
                        </span>
                    </button>
                </li>`;
        });
        html += '</ul>';
        area.innerHTML = html;
        Array.from(area.querySelectorAll('.er-search-result-btn')).forEach(btn => {
            btn.onclick = () => {
                const idx = parseInt(btn.getAttribute('data-idx'), 10);
                this.select(results[idx]);
            };
        });
    }
    select(result) {
        this.clearHighlight();
        if (this.selectedSvgElem) this.selectedSvgElem.classList.remove('er-search-highlight');
        const area = this.searchDialog?.querySelector('#erSearchResultArea');
        if (area) {
            Array.from(area.querySelectorAll('.er-search-result-btn')).forEach(btn => {
                btn.classList.remove('selected');
            });
        }
        if (result?.svgElem) {
            result.svgElem.classList.add('er-search-highlight');
            if (result.svgTextElem) {
                result.svgTextElem.setAttribute('data-er-search-highlight', '1');
                result.svgTextElem.classList.add('er-search-svg-highlight');
            }
            this.selectedSvgElem = result.svgElem;
            if (area && typeof result.idx === 'number') {
                const btn = area.querySelector(`.er-search-result-btn[data-idx="${result.idx}"]`);
                if (btn) btn.classList.add('selected');
            }
            const svg = document.querySelector('#mermaidRenderArea svg');
            const wrapper = document.getElementById('mermaidScrollWrapper');
            if (svg && this.er && result.parentG && typeof result.parentG.getBBox === 'function') {
                const bbox = result.parentG.getBBox();
                const margin = 32;
                const scaleX = (wrapper.clientWidth - margin) / bbox.width;
                const scaleY = (wrapper.clientHeight - margin) / bbox.height;
                let fitScale = Math.min(scaleX, scaleY);
                fitScale = Math.max(0.2, Math.min(fitScale, 5));
                if (this.er.zoomScale === this.er.baseZoomScale) {
                    this.er.zoomScale = fitScale;
                    this.er.applyZoom();
                }
            }
        }
        this.scrollTo(result);
    }
    scrollTo(result) {
        const svg = document.querySelector('#mermaidRenderArea svg');
        const wrapper = document.getElementById('mermaidScrollWrapper');
        if (!svg || !result) return;
        const targetEl = (result.svgTextElem && typeof result.svgTextElem.getBBox === 'function')
            ? result.svgTextElem
            : (result.parentG && typeof result.parentG.getBBox === 'function')
                ? result.parentG
                : null;
        if (!targetEl) return;
        const bbox = targetEl.getBBox();
        const ctm = (typeof targetEl.getCTM === 'function') ? targetEl.getCTM() : null;
        const cxLocal = bbox.x + bbox.width / 2;
        const cyLocal = bbox.y + bbox.height / 2;
        const cxUser = ctm ? (ctm.a * cxLocal + ctm.c * cyLocal + ctm.e) : cxLocal;
        const cyUser = ctm ? (ctm.b * cxLocal + ctm.d * cyLocal + ctm.f) : cyLocal;

        let vbX = 0, vbY = 0, vbW = null, vbH = null;
        const viewBoxAttr = svg.getAttribute('viewBox');
        if (viewBoxAttr) {
            const vb = viewBoxAttr.trim().split(/\s+/).map(parseFloat);
            if (vb.length === 4 && vb.every(v => Number.isFinite(v))) { vbX = vb[0]; vbY = vb[1]; vbW = vb[2]; vbH = vb[3]; }
        }
        let unitToPxX, unitToPxY;
        if (vbW && vbH) {
            unitToPxX = svg.clientWidth / vbW;
            unitToPxY = svg.clientHeight / vbH;
        } else {
            const rect = svg.getBoundingClientRect();
            const wAttr = (svg.width?.baseVal?.value) ?? rect.width;
            const hAttr = (svg.height?.baseVal?.value) ?? rect.height;
            unitToPxX = rect.width / (wAttr || 1);
            unitToPxY = rect.height / (hAttr || 1);
        }
        const cssScale = this.er.getZoomScale() || 1;
        const centerXpx = (cxUser - vbX) * unitToPxX * cssScale;
        const centerYpx = (cyUser - vbY) * unitToPxY * cssScale;
        const desiredLeft = centerXpx - wrapper.clientWidth / 2;
        const desiredTop = centerYpx - wrapper.clientHeight / 2;
        const maxLeft = Math.max(0, wrapper.scrollWidth - wrapper.clientWidth);
        const maxTop = Math.max(0, wrapper.scrollHeight - wrapper.clientHeight);
        wrapper.scrollLeft = Math.min(maxLeft, Math.max(0, desiredLeft));
        wrapper.scrollTop = Math.min(maxTop, Math.max(0, desiredTop));
    }
    clearHighlight() {
        const svg = document.querySelector('#mermaidRenderArea svg');
        if (!svg) return;
        Array.from(svg.querySelectorAll('text[data-er-search-highlight="1"]')).forEach(t => {
            t.removeAttribute('data-er-search-highlight');
            t.classList.remove('er-search-svg-highlight');
        });
        if (this.selectedSvgElem) this.selectedSvgElem.classList.remove('er-search-highlight');
        this.selectedSvgElem = null;
        const area = this.searchDialog?.querySelector('#erSearchResultArea');
        if (area) {
            Array.from(area.querySelectorAll('.er-search-result-btn')).forEach(btn => {
                btn.classList.remove('selected');
            });
        }
    }
}
