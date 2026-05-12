<script lang="ts">
    import { onAppClose, tabState, siteData, getIcon } from '../store';
    import { pDisplay } from '../Utility/$p';
</script>

<header class="app-header">
    <div class="header-inner">
        <div class="site-icon">
            <span class="material-symbols-sharp is-fill">{getIcon($siteData.ReferenceType)}</span>
        </div>
        <h2 class="ttl">{$siteData.Title}</h2>
        <div class="tabs">
            <ul>
                <li>
                    <button type="button" on:click={() => ($tabState = 'edit')} class:is-active={$tabState === 'edit'}>
                        <span class="ico material-symbols-sharp is-fill">edit_square</span>
                        <span class="txt">{pDisplay('Editor')}</span>
                    </button>
                </li>
                <li>
                    <button type="button" on:click={() => ($tabState = 'grid')} class:is-active={$tabState === 'grid'}>
                        <span class="ico material-symbols-sharp is-fill">lists</span>
                        <span class="txt">{pDisplay('Grid')}</span>
                    </button>
                </li>
                <li>
                    <button
                        type="button"
                        on:click={() => ($tabState = 'filter')}
                        class:is-active={$tabState === 'filter'}
                    >
                        <span class="ico material-symbols-sharp is-fill">filter_alt</span>
                        <span class="txt">{pDisplay('Filters')}</span>
                    </button>
                </li>
            </ul>
        </div>
        <button type="button" on:click={onAppClose} class="btn-close">
            <span class="material-symbols-sharp">close</span>
        </button>
    </div>
</header>

<style lang="scss">
    @use '../css/shared';

    .app-header {
        position: relative;
        height: 50px;
        background-color: var(--sd-header-bg);
        border-bottom: 1px solid var(--sd-header-border);
        .header-inner {
            display: flex;
            gap: 8px;
            align-items: center;
            width: 100%;
            height: 100%;
            padding: 0 4px 0 8px;
            .site-icon {
                color: var(--sd-header-icon);
                span {
                    font-size: 30px;
                }
            }
        }
        .ttl {
            flex: 1;
            overflow: hidden;
            text-overflow: ellipsis;
            font-size: 20px;
            font-weight: normal;
            white-space: nowrap;
        }
        .tabs {
            align-self: flex-end;
            ul {
                display: flex;
                gap: 8px;
                align-items: flex-end;
                padding: 0;
                margin: 0;
            }
            li {
                list-style: none;
            }
            button {
                display: flex;
                gap: 4px;
                align-items: center;
                padding: 4px 16px;
                color: var(--sd-tab-label);
                cursor: pointer;
                background-color: var(--sd-tab-bg);
                border: 1px solid var(--sd-tab-border);
                border-bottom: 0;
                border-radius: 4px 4px 0 0;
                transition:
                    background 200ms ease-out,
                    padding 200ms ease-out,
                    color 200ms ease-out;
                .ico {
                    font-size: 14px;
                }
                .txt {
                    font-size: 12px;
                }
                &:hover {
                    background-color: var(--sd-tab-hover);
                }
                &.is-active {
                    padding: 6px 16px;
                    color: var(--sd-tab-active-label);
                    background-color: var(--sd-tab-active-bg);
                    border-color: var(--sd-tab-active-border);
                    &:hover {
                        background-color: var(--sd-tab-active-hover);
                    }
                }
            }
        }
        .btn-close {
            display: block;
            width: 50px;
            height: 50px;
            padding: 0;
            margin: 0;
            cursor: pointer;
            background-color: transparent;
            border: 0;
            span {
                font-size: 36px;
                font-variation-settings: 'wght' 100;
                color: var(--sd-header-close);
            }
        }
    }
</style>
