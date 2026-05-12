<script lang="ts">
    export let model: string | number | boolean | null | undefined = '';
    export let align: number | undefined = undefined;
    export let unit: string | undefined = undefined;
    export let fieldSize: string | undefined = undefined;
    let text: string | number | boolean | null | undefined = undefined;

    text = model;
    $: if (unit) {
        text = String(model || 0) + String(unit);
    } else {
        text = model || undefined;
    }
</script>

<div
    class="readField"
    class:is-center={align === 15}
    class:is-right={align === 20}
    class:is-wide={fieldSize === 'field-wide'}
    class:is-markdown={fieldSize === 'field-markdown'}
>
    {#if fieldSize !== 'field-markdown'}
        <div class="notes">{text || ''}</div>
    {:else}
        <pre class="notes">{text || ''}</pre>
    {/if}
</div>

<style lang="scss">
    @use '../shared';

    .readField {
        text-align: left;
        .notes {
            width: 100%;
            height: 32px;
            padding: 4.5px 8px;
            margin: 0;
            overflow: hidden;
            text-overflow: ellipsis;
            color: var(--control-text-read);
            white-space: nowrap;
            outline: none;
            background: var(--control-bg-read);
            border: 1px solid var(--control-border);
            border-radius: 4px;
        }
        &.is-wide {
            .notes {
                height: auto;
                min-height: 32px;
                white-space: normal;
            }
        }
        &.is-markdown {
            .notes {
                height: auto;
                min-height: 100px;
                font-family:
                    '游ゴシック体', YuGothic, '游ゴシック Medium', 'Yu Gothic Medium', '游ゴシック', 'Yu Gothic',
                    sans-serif;
                white-space: pre-wrap;
            }
        }
        &.is-center {
            text-align: center;
        }
        &.is-right {
            text-align: right;
        }
    }
</style>
