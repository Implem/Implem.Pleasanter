<script lang="ts">
    import ReadField from './ReadField.svelte';
    import { pDisplay } from '../$p';

    export let model: string | undefined;
    export let required: boolean | undefined = undefined;
    export let readOnly: boolean | undefined = undefined;
    export let fieldSize: string | undefined = undefined;
    export let align: number | undefined = undefined;
    export let maxLength: number | undefined = undefined;
    export let placeholder: string | undefined = undefined;
    export let viewerType: number | undefined;
    export let AllowImage: boolean | undefined = false;

    export let onInit: ((model: string | undefined) => void) | undefined = undefined;
    export let onInput: ((event: InputEvent) => void) | undefined = undefined;

    let textarea: HTMLTextAreaElement;
    let errorMessage: string | undefined = undefined;
    let editSwitch: boolean = false;

    $: if (required !== undefined) onValidation();
    $: if (maxLength !== undefined) onValidation();
    const onValidation = () => {
        if (maxLength !== undefined && maxLength > 0 && model) {
            if (maxLength < String(model).length) {
                errorMessage = pDisplay('ValidateMaxLength', maxLength);
            } else {
                errorMessage = undefined;
            }
        } else if (required && model === '') {
            errorMessage = pDisplay('ValidateRequired');
        } else if (errorMessage) {
            errorMessage = undefined;
        }
    };

    const onEditMode = () => {
        if (viewerType !== 3 && !editSwitch) {
            editSwitch = true;
            setTimeout(() => {
                textarea.focus();
            });
        }
    };

    const onPreviewMode = () => {
        setTimeout(() => {
            if (viewerType === 1 && editSwitch) {
                editSwitch = false;
            }
        }, 100);
    };

    const onEditToggle = () => {
        if (!editSwitch) {
            onEditMode();
        } else {
            editSwitch = false;
        }
    };

    if (onInit) {
        setTimeout(() => {
            onInit(model);
        }, 10);
    }
</script>

{#if !readOnly}
    <div class="textarea" class:is-center={align === 15} class:is-right={align === 20}>
        {#if viewerType !== 3}
            <button type="button" class="btn edit-btn" on:click={onEditToggle}>
                <span class="material-symbols-sharp is-fill">edit</span>
            </button>
            {#if !editSwitch}
                <pre on:dblclick={onEditMode} class:is-placeholder={!model}>{model || placeholder}</pre>
            {/if}
        {/if}
        {#if editSwitch || viewerType === 3}
            <textarea
                class="controller"
                bind:this={textarea}
                bind:value={model}
                {placeholder}
                on:blur={onPreviewMode}
                on:input={onInput}
                on:input={onValidation}
            ></textarea>
        {/if}
        {#if AllowImage}
            <ul class="tools">
                <li>
                    <button type="button" class="btn" on:click={onEditToggle}>
                        <span class="material-symbols-sharp is-fill">imagesmode</span>
                    </button>
                </li>
                <li>
                    <button type="button" class="btn" on:click={onEditToggle}>
                        <span class="material-symbols-sharp is-fill">add_a_photo</span>
                    </button>
                </li>
            </ul>
        {/if}
        {#if errorMessage && model !== undefined}
            <p class="error">{errorMessage}</p>
        {/if}
    </div>
{:else}
    <ReadField {model} {align} {fieldSize} />
{/if}

<style lang="scss">
    @use '../shared';

    .textarea {
        position: relative;
        .btn {
            display: block;
            padding: 0;
            margin: 0;
            cursor: pointer;
            outline: none;
            background: transparent;
            border: 0;
            span {
                display: block;
                font-size: 16px;
                color: var(--nonColor07);
            }
        }

        .edit-btn {
            position: absolute;
            top: 4px;
            right: 4px;
        }

        pre,
        .controller {
            display: block;
            width: 100%;
            min-height: 100px;
            padding: 8px 28px 8px 8px;
            margin: 0;
            font-size: 13px;
            line-height: 1.5em;
            color: var(--control-text);
            resize: none;
            outline: none;
            background-color: var(--control-bg);
            border: 1px solid var(--control-border);
            border-radius: 4px;
            transition:
                color 150ms ease-out,
                background 150ms ease-out,
                border 150ms ease-out;
            field-sizing: content;
            &:focus {
                background-color: var(--control-bg-focus);
                border-color: var(--control-border-focus);
            }
        }
        pre {
            font-family:
                '游ゴシック体', YuGothic, '游ゴシック Medium', 'Yu Gothic Medium', '游ゴシック', 'Yu Gothic', sans-serif;
            white-space: pre-wrap;
            &.is-placeholder {
                color: darkgray;
            }
        }
        .controller {
            max-height: 320px;
        }
        .tools {
            display: flex;
            gap: 4px;
            padding: 4px 0 0;
            margin: 0;
            list-style-type: none;
            .btn {
                cursor: not-allowed;
            }
        }
        .error {
            padding: 4px 0 0;
            color: var(--control-error);
        }
        &.is-center {
            text-align: center;
            textarea {
                text-align: center;
            }
        }
        &.is-right {
            text-align: right;
            textarea {
                text-align: right;
            }
        }
    }
</style>
