class SiteSetting {
    constructor() {
        this.allSites = [];
        this.currentSiteIdx = 0;
        this.tabKeys = [];
        this.currentTab = '';
        SiteSetting.setupDragScroll();
    }
    static setupDragScroll() {
        if (SiteSetting._dragScrollSetup) return;
        SiteSetting._dragScrollSetup = true;
        let wrapper = null;
        let table = null;
        let isDown = false;
        let hasMoved = false;
        let startX = 0;
        let startY = 0;
        let scrollLeft = 0;
        let scrollTop = 0;
        document.addEventListener('mousedown', (e) => {
            if (e.button !== 0) return;
            if (e.altKey) return;
            const tbl = e.target.closest('table');
            if (!tbl) return;
            const w = tbl.closest('.scroll-table-wrapper');
            if (!w) return;
            wrapper = w;
            table = tbl;
            isDown = true;
            hasMoved = false;
            startX = e.pageX - wrapper.offsetLeft;
            startY = e.pageY - wrapper.offsetTop;
            scrollLeft = wrapper.scrollLeft;
            scrollTop = wrapper.scrollTop;
            table.classList.add('is-grabbing');
        });
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Alt') document.body.classList.add('alt-selecting');
        });
        document.addEventListener('keyup', (e) => {
            if (e.key === 'Alt') document.body.classList.remove('alt-selecting');
        });
        window.addEventListener('blur', () => {
            document.body.classList.remove('alt-selecting');
        });
        document.addEventListener('mousemove', (e) => {
            if (!isDown || !wrapper) return;
            const x = e.pageX - wrapper.offsetLeft;
            const y = e.pageY - wrapper.offsetTop;
            const walkX = x - startX;
            const walkY = y - startY;
            if (!hasMoved && Math.abs(walkX) < 5 && Math.abs(walkY) < 5) return;
            hasMoved = true;
            e.preventDefault();
            wrapper.scrollLeft = scrollLeft - walkX;
            wrapper.scrollTop = scrollTop - walkY;
        });
        const endDrag = () => {
            if (!isDown) return;
            isDown = false;
            if (table) table.classList.remove('is-grabbing');
            wrapper = null;
            table = null;
        };
        document.addEventListener('mouseup', endDrag);
        document.addEventListener('mouseleave', endDrag);
    }
    renderFromData(data) {
        this.loadSites(data);
        this.renderTable();
    }
    loadSites(json) {
        this.allSites = (json.SiteSetting && Array.isArray(json.SiteSetting.Sites)) ? json.SiteSetting.Sites : [];
        this.currentSiteIdx = 0;
        this.renderSiteSelector(this.allSites);
        this.updateTabKeysAndButtons();
    }
    renderSiteSelector(sites) {
        const sel = document.getElementById('siteSelector');
        const isShowVer = sites.length !== new Set(sites.map(v => v.Info?.SiteId)).size;
        sel.innerHTML = '';
        sites.forEach((site, idx) => {
            const opt = document.createElement('option');
            opt.value = idx;
            opt.textContent = (site.Info && site.Info.Title)
                ? `${site.Info.Title} - [ ${site.Info.SiteId}${isShowVer ? '(Ver=' + site.Info.Ver + ')' : ''} / ${site.Info.ReferenceType}]`
                : `Site${idx + 1}`;
            sel.appendChild(opt);
        });
        sel.selectedIndex = this.currentSiteIdx;
        sel.onchange = () => {
            this.currentSiteIdx = parseInt(sel.value, 10) || 0;
            this.updateTabKeysAndButtons();
            this.renderTable();
            const selectedText = sel.options[sel.selectedIndex]?.textContent || '';
            document.title = `${selectedText} - Pleasanter Site Visualizer`;
        };
        if (sites.length > 0 && sel.selectedIndex >= 0) {
            const selectedText = sel.options[sel.selectedIndex]?.textContent || '';
            document.title = `${selectedText} - Pleasanter Site Visualizer`;
        }
        Common.updateGearErrorIndicator();
        Common.positionGearErrorIndicator();
    }
    renderTabButtons(keys, site) {
        const tabDiv = document.getElementById('tabButtons');
        tabDiv.innerHTML = '';
        keys.forEach(tab => {
            const btn = document.createElement('button');
            btn.className = 'tab-btn' + (tab === this.currentTab ? ' active' : '');
            btn.textContent = site.Tabs?.[tab]?.ButtonLabel ?? tab;
            btn.onclick = () => {
                this.currentTab = tab;
                this.renderTabButtons(this.tabKeys, site);
                this.renderTable();
            };
            tabDiv.appendChild(btn);
        });
    }
    updateTabKeysAndButtons() {
        if (!this.allSites.length) return;
        const site = this.allSites[this.currentSiteIdx] || {};
        this.tabKeys = Object.keys(site.Tabs || {});
        if (!this.tabKeys.includes(this.currentTab)) this.currentTab = this.tabKeys[0] || '';
        this.renderTabButtons(this.tabKeys, site);
    }
    renderTable() {
        const data = Common.parseJsonSafe();
        this.allSites = (data.SiteSetting && Array.isArray(data.SiteSetting.Sites)) ? data.SiteSetting.Sites : [];
        if (!this.allSites.length) {
            document.getElementById('tableContainer').innerHTML = `<span style="color:red;">${t('common_site_not_found')}</span>`;
            this.renderSiteSelector([]);
            this.renderTabButtons([]);
            return;
        }
        if (this.currentSiteIdx >= this.allSites.length) this.currentSiteIdx = 0;
        this.renderSiteSelector(this.allSites);
        const site = this.allSites[this.currentSiteIdx] || {};
        this.tabKeys = Object.keys(site.Tabs || {});
        if (!this.tabKeys.includes(this.currentTab)) this.currentTab = this.tabKeys[0] || '';
        this.renderTabButtons(this.tabKeys, site);

        try {
            const tab = site.Tabs?.[this.currentTab];
            if (!tab || !Array.isArray(tab.Tables) || tab.Tables.length === 0) {
                document.getElementById('tableContainer').innerHTML = `<span style="color:red;">${Common.escapeHtml(this.currentTab)}が見つかりません。</span>`;
                return;
            }
            let html = '';
            tab.Tables.forEach((table, tIdx) => {
                if (SiteSetting.isTableTypeKeyValue(table)) html += SiteSetting.makeTableTypeKeyValue(table, tIdx);
                else if (SiteSetting.isTableTypeList(table)) html += SiteSetting.makeTableTypeList(table, tIdx);
                else if (SiteSetting.isTableTypeOneLineHeaderTable(table)) html += SiteSetting.makeTableTypeOneLineHeaderTable(table, tIdx);
                else if (SiteSetting.isTableTypeTwoLineHeaderTable(table)) html += SiteSetting.makeTableTypeTwoLineHeaderTable(table, tIdx);
                else html += `<span style="color:red;">${Common.escapeHtml(this.currentTab)}のTableTypeが不明です: ${Common.escapeHtml(table.TableType)}</span>`;
            });
            document.getElementById('tableContainer').innerHTML = html;
        } catch {
            document.getElementById('tableContainer').innerHTML = `<span style="color:red;">${Common.escapeHtml(this.currentTab)}の表示に失敗しました。</span>`;
        }
    }
    static isTableTypeKeyValue(table) { return table.TableType === 'KeyValue'; }
    static isTableTypeList(table) { return table.TableType === 'List'; }
    static isTableTypeOneLineHeaderTable(table) { return table.TableType === 'OneLineHeaderTable'; }
    static isTableTypeTwoLineHeaderTable(table) { return table.TableType === 'TwoLineHeaderTable'; }
    static makeTableTypeKeyValue(table, tIdx) {
        if (!SiteSetting.isTableTypeKeyValue(table)) {
            return `<span style="color:red;">TableTypeがKeyValueではありません。TableType: ${Common.escapeHtml(table.TableType)}</span>`;
        }
        const gridName = Common.escapeHtml(table.Label || '');
        let html = '';
        if (gridName) html += `<div class="table-title">${gridName}</div>`;
        html += `<div class="scroll-table-wrapper"><table>`;
        if (Array.isArray(table.Header?.Labels)) {
            html += '<thead><tr>';
            table.Header.Labels.forEach((h, idx) => {
                html += `<th${idx === 0 ? ' class="sticky-col"' : ''}>${Common.escapeHtml(h)}</th>`;
            });
            html += '</tr></thead>';
        }
        html += '<tbody>';
        if (Array.isArray(table.Columns)) {
            table.Columns.forEach(col => {
                let tdClass = '';
                if (col.ReadOnly === true) tdClass = ' class="missing"';
                if (col.Changed === true) tdClass = ' class="changed"';
                if (col.ReadOnly === true && col.Changed === true) tdClass = ' class="changed"';
                let tdStyle = '';
                if (col.Type === 'int' || col.Type === 'long') tdStyle = ' style="text-align:right;"';
                html += `<tr><th class="sticky-col">${Common.escapeHtml(col.Label ?? '')}</th><td${tdClass}${tdStyle}><pre>${Common.escapeHtml(col.Value ?? '')}</pre></td></tr>`;
            });
        }
        html += '</tbody></table></div>';
        return html;
    }
    static makeTableTypeList(table, tIdx) {
        if (!SiteSetting.isTableTypeList(table)) {
            return `<span style="color:red;">TableTypeがListではありません。TableType: ${Common.escapeHtml(table.TableType)}</span>`;
        }
        const gridName = Common.escapeHtml(table.Label || '');
        let html = '';
        if (gridName) html += `<div class="table-title">${gridName}</div>`;
        html += '<div class="scroll-table-wrapper"><table>';
        if (Array.isArray(table.Header?.Labels)) {
            html += '<thead><tr>';
            table.Header.Labels.forEach(h => html += `<th>${Common.escapeHtml(h)}</th>`);
            html += '</tr></thead>';
        }
        if (Array.isArray(table.Columns)) {
            html += '<tbody>';
            table.Columns.forEach(col => {
                html += '<tr>';
                if (Array.isArray(col)) col.forEach(cell => html += `<td><pre>${Common.escapeHtml(cell)}</pre></td>`);
                else html += `<td><pre>${Common.escapeHtml(col)}</pre></td>`;
                html += '</tr>';
            });
            html += '</tbody>';
        }
        html += '</table></div>';
        return html;
    }
    static makeTableTypeOneLineHeaderTable(table, tIdx) {
        table.Columns = table.Columns || [];
        if (table.TableType !== "OneLineHeaderTable" || !Array.isArray(table.Header?.Labels) || !Array.isArray(table.Columns)) {
            return '<span style="color:red;">OneLineHeaderTableテーブル形式に未対応です。</span>';
        }
        let headerRow = '<tr>';
        table.Header.Labels.forEach((h, idx) => {
            const thClass = idx === 0 ? ' class="sticky-col"' : '';
            headerRow += `<th${thClass}>${Common.escapeHtml(h.Text ?? h.Key ?? '')}</th>`;
        });
        headerRow += '</tr>';
        let bodyRows = '';
        table.Columns.forEach(col => {
            const changed = Array.isArray(col.ChangedColumns) ? col.ChangedColumns : [];
            bodyRows += '<tr>';
            table.Header.Labels.forEach((h, idx) => {
                const key = h.Key;
                let tdClass = '';
                let value = (key && col[key] !== undefined) ? col[key] : '';
                let valueIsHtml = false;
                if (typeof value === 'object' && value !== null) {
                    if (Array.isArray(value)) {
                        value = SiteSetting.convJson2Html(value, key, table);
                        valueIsHtml = true;
                    } else {
                        value = Common.escapeHtml(JSON.stringify(value));
                        valueIsHtml = true;
                    }
                }
                if (changed.includes(key)) {
                    tdClass = idx === 0 ? ' class="changed sticky-col"' : ' class="changed"';
                } else if (key && col[key] === undefined) {
                    tdClass = idx === 0 ? ' class="missing sticky-col"' : ' class="missing"';
                } else if (idx === 0) {
                    tdClass = ' class="sticky-col"';
                }
                bodyRows += `<td${tdClass}>${valueIsHtml ? value : `<pre>${Common.escapeHtml(value)}</pre>`}</td>`;
            });
            bodyRows += '</tr>';
        });
        const gridName = Common.escapeHtml(table.Label || '');
        let html = '';
        if (gridName) html += `<div class="table-title">${gridName}</div>`;
        html += `<div class="scroll-table-wrapper"><table><thead>${headerRow}</thead><tbody>${bodyRows}</tbody></table></div>`;
        return html;
    }
    static makeGenericTable(table, tIdx) {
        const gridName = Common.escapeHtml(table.Label || '');
        let html = '';
        if (gridName) html += `<div class="table-title">${gridName}</div>`;
        html += '<div class="scroll-table-wrapper"><table>';
        if (Array.isArray(table.Header?.Labels)) {
            html += '<thead><tr>';
            table.Header.Labels.forEach(h => html += `<th>${Common.escapeHtml(h)}</th>`);
            html += '</tr></thead>';
        }
        if (Array.isArray(table.Columns)) {
            html += '<tbody>';
            table.Columns.forEach(col => {
                html += '<tr>';
                if (Array.isArray(col)) col.forEach(cell => html += `<td><pre>${Common.escapeHtml(cell)}</pre></td>`);
                else html += `<td><pre>${Common.escapeHtml(col)}</pre></td>`;
                html += '</tr>';
            });
            html += '</tbody>';
        }
        html += '</table></div>';
        return html;
    }
    static convJson2Html(value, k, table) {
        if (Array.isArray(value)) {
            if (value.length === 0) return '';
            const item = value[0];
            if (typeof item === 'object') {
                const keys = Object.keys(item);
                let html = '<div class="array">';
                value.forEach(row => {
                    html += '<span class="item-pair">';
                    keys.forEach(kk => html += `<span class="${Common.escapeHtml(kk)}">${Common.escapeHtml(row[kk])}</span>`);
                    html += '</span>';
                });
                html += '</div>';
                return html;
            } else {
                let html = '<div class="array">';
                value.forEach(row => { html += `<span class="item-single">${Common.escapeHtml(row)}</span>`; });
                html += '</div>';
                return html;
            }
        }
        return JSON.stringify(value);
    }
    static makeTableTypeTwoLineHeaderTable(table, tIdx) {
        table.Columns = table.Columns || [];
        if (table.TableType !== 'TwoLineHeaderTable' || !Array.isArray(table.Columns) || !Array.isArray(table.Header?.Labels)) {
            return '<span style="color:red;">TwoLineHeaderTableテーブル形式に未対応です。</span>';
        }
        let groups = [];
        let readonyRow = [];
        table.Header.Labels.forEach(tab => {
            if (tab.TabName && Array.isArray(tab.Labels)) {
                groups.push({
                    groupName: tab.TabName.Value,
                    keys: tab.Labels.map(l => l.Key),
                    headers: tab.Labels.map(l => l.Text)
                });
                readonyRow.push(tab.Labels.filter(v => v.ReadOnly).map(v => v.Key));
            }
        });
        readonyRow = readonyRow.flat();
        const allKeys = groups.flatMap(g => g.keys);
        const columns = table.Columns;
        if (!Array.isArray(columns)) {
            return '<span style="color:red;">TwoLineHeaderTableカラム情報がありません。</span>';
        }
        const firstColIdx = 0;

        let groupHeaderRow = '<tr>';
        let colIdx = 0;
        groups.forEach(g => {
            const groupName = g.groupName !== '' ? Common.escapeHtml(g.groupName) : '&nbsp;';
            const thClass = colIdx === firstColIdx ? ' class="sticky-col"' : '';
            groupHeaderRow += `<th${thClass} colspan="${g.keys.length}">${groupName}</th>`;
            colIdx += g.keys.length;
        });
        groupHeaderRow += '</tr>';

        let itemHeaderRow = '<tr>';
        let headerColIdx = 0;
        groups.forEach(g => {
            g.headers.forEach(h => {
                const thClass = headerColIdx === firstColIdx ? ' class="sticky-col"' : '';
                itemHeaderRow += `<th${thClass}>${Common.escapeHtml(h)}</th>`;
                headerColIdx++;
            });
        });
        itemHeaderRow += '</tr>';

        function splitKeysByNest(keys) {
            let result = [];
            let current = { type: null, keys: [] };
            for (let i = 0; i < keys.length; i++) {
                const k = keys[i];
                const isNest = k && k.includes('.');
                const type = isNest ? k.split('.', 2)[0] : '';
                if (current.type === null) {
                    current.type = isNest ? type : '';
                    current.keys.push(k);
                } else if ((isNest ? type : '') === current.type) {
                    current.keys.push(k);
                } else {
                    result.push(current);
                    current = { type: isNest ? type : '', keys: [k] };
                }
            }
            if (current.keys.length) result.push(current);
            return result;
        }
        let keyBlocks = splitKeysByNest(allKeys);

        let bodyRows = '';
        columns.forEach(col => {
            let nestRowCounts = {};
            keyBlocks.forEach(block => {
                if (block.type && block.type.length > 0) {
                    nestRowCounts[block.type] = Array.isArray(col[block.type]) ? col[block.type].length : 0;
                }
            });
            let maxNestRows = 1;
            keyBlocks.forEach(block => {
                if (block.type && block.type.length > 0) {
                    maxNestRows = Math.max(maxNestRows, nestRowCounts[block.type] || 0);
                }
            });
            for (let rowIdx = 0; rowIdx < maxNestRows; rowIdx++) {
                bodyRows += '<tr>';
                for (let b = 0; b < keyBlocks.length; b++) {
                    const block = keyBlocks[b];
                    const keys = block.keys;
                    const isNest = block.type && block.type.length > 0;
                    if (!isNest) {
                        if (rowIdx === 0) {
                            keys.forEach((k, idx) => {
                                let tdClass = '';
                                let value = col[k];
                                let valueIsHtml = false;
                                const changed = Array.isArray(col.ChangedColumns) ? col.ChangedColumns : [];
                                const readOnly = Array.isArray(col.ReadOnlyColumns) ? col.ReadOnlyColumns : [];
                                if (readonyRow.includes(k) || readOnly.includes(k) || value === undefined || value === null) {
                                    tdClass = ' class="missing' + (idx === firstColIdx ? ' sticky-col' : '') + '"';
                                } else if (changed.includes(k)) {
                                    tdClass = ' class="changed' + (idx === firstColIdx ? ' sticky-col' : '') + '"';
                                } else if (idx === firstColIdx) {
                                    tdClass = ' class="sticky-col"';
                                }
                                let rowspan = maxNestRows > 1 ? ` rowspan="${maxNestRows}"` : '';
                                if (typeof value === 'object' && value !== null) {
                                    if (Array.isArray(value)) { value = SiteSetting.convJson2Html(value, k, table); valueIsHtml = true; }
                                    else { value = Common.escapeHtml(JSON.stringify(value)); valueIsHtml = true; }
                                }
                                bodyRows += `<td${tdClass}${rowspan}>${value !== undefined ? (valueIsHtml ? value : `<pre>${Common.escapeHtml(value)}</pre>`) : ''}</td>`;
                            });
                        }
                    } else {
                        let nestLen = nestRowCounts[block.type] || 0;
                        if (rowIdx < nestLen) {
                            keys.forEach(k => {
                                let tdClass = '';
                                let value = '';
                                let valueIsHtml = false;
                                const [parent, child] = k.split('.', 2);
                                const roCols = Array.isArray(col[parent]?.[rowIdx]?.ReadOnlyColumns) ? col[parent][rowIdx].ReadOnlyColumns : [];
                                const chCols = Array.isArray(col[parent]?.[rowIdx]?.ChangedColumns) ? col[parent][rowIdx].ChangedColumns : [];
                                if (Array.isArray(col[parent]) && col[parent][rowIdx] && col[parent][rowIdx][child] !== undefined) {
                                    value = col[parent][rowIdx][child];
                                } else {
                                    value = undefined;
                                }
                                if (typeof value === 'object' && value !== null) {
                                    if (Array.isArray(value)) { value = SiteSetting.convJson2Html(value, k, table); valueIsHtml = true; }
                                    else { value = Common.escapeHtml(JSON.stringify(value)); valueIsHtml = true; }
                                }
                                if (readonyRow.includes(k) || roCols.includes(child) || value === undefined || value === null) tdClass = ' class="missing"';
                                else if (chCols.includes(child)) tdClass = ' class="changed"';
                                bodyRows += `<td${tdClass}>${value !== undefined ? (valueIsHtml ? value : `<pre>${Common.escapeHtml(value)}</pre>`) : ''}</td>`;
                            });
                        } else if (rowIdx === nestLen && nestLen < maxNestRows) {
                            let tdClass = ' class="missing"';
                            let rowspan = (maxNestRows - nestLen) > 1 ? ` rowspan="${maxNestRows - nestLen}"` : '';
                            keys.forEach(() => { bodyRows += `<td${tdClass}${rowspan}></td>`; });
                        }
                    }
                }
                bodyRows += '</tr>';
            }
        });

        const gridName = Common.escapeHtml(table.Label || '');
        let html = '';
        if (gridName) html += `<div class="table-title">${gridName}</div>`;
        html += `<div class="scroll-table-wrapper"><table><thead>${groupHeaderRow}${itemHeaderRow}</thead><tbody>${bodyRows}</tbody></table></div>`;
        return html;
    }
}
