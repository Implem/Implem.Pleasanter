<script lang="ts">
    import { pDisplay } from '../$p';

    export let model: boolean | undefined;
    export let required: boolean | undefined = undefined;
    export let readOnly: boolean | undefined = undefined;
    export let align: number | undefined = undefined;
    export let placeholder: string | undefined = undefined;
    export let fileDelete: boolean | undefined = undefined;
    export let hasError: boolean = false;
</script>

<label class="filefield" class:is-center={align === 15} class:is-right={align === 20}>
    {#if !readOnly}
        <div class="file-inner">
            {#if !placeholder}
                <p>{pDisplay('FileDragDrop')}</p>
            {:else}
                {placeholder}
            {/if}
        </div>
    {/if}
    {#if model}
        <div class="items">
            <div class="unit">
                <div class="icon"><span class="material-symbols-sharp is-fill">zoom_in</span></div>
                <div class="title"><span>{pDisplay('AttachedDataSample')} (12.34 KB)</span></div>
                {#if !readOnly && fileDelete}
                    <div class="delete"><span class="material-symbols-sharp is-fill">cancel</span></div>
                {/if}
            </div>
        </div>
    {/if}

    <input type="text" />
    {#if hasError && !readOnly}
        <p class="error">
            {#if required}
                {pDisplay('ValidateRequired')}
            {:else}
                {pDisplay('ErrorMessageSample')}
            {/if}
        </p>
    {/if}
</label>

<style lang="scss">
    @use '../shared';

    .filefield {
        float: left;
        display: block;
        width: 100%;
        .file-inner {
            padding: 25px 0;
            text-align: center;
            border: dotted 2px var(--primaryColor);
            border-radius: 3px;
            + .items {
                margin-top: 5px;
            }
        }
        input {
            display: none;
        }
        .items {
            .unit {
                display: flex;
                gap: 8px;
                align-items: center;
                padding: 8px;
                font-size: 13px;
                color: var(--base-text);
                background-color: var(--primarySub03);
                border: 1px solid var(--primaryColor);
                border-radius: 5px;
                .icon {
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    width: 16px;
                    height: 16px;
                    color: var(--invert-text);
                    cursor: not-allowed;
                    background-color: var(--base-text);
                    border-radius: 100%;
                    transform: scaleX(-1);
                    .material-symbols-sharp {
                        display: block;
                        font-size: 12px;
                    }
                }
                .title {
                    flex: 1;
                    text-align: left;
                    span {
                        font-size: 13px;
                        text-decoration: underline;
                        cursor: not-allowed;
                    }
                }
                .delete {
                    cursor: not-allowed;
                    .material-symbols-sharp {
                        display: block;
                        font-size: 20px;
                    }
                }
            }
        }
        .error {
            padding: 4px 0 0;
            color: var(--control-error);
        }
        &.is-center {
            text-align: center;
            .file-inner {
                text-align: center;
            }
        }
        &.is-right {
            text-align: right;
            .file-inner {
                text-align: right;
            }
        }
    }
</style>
