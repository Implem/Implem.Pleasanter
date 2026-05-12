<script lang="ts">
    import { onAppClose, appMessage, submitState } from '../store';
    import { pDisplay } from '../Utility/$p';
    import Button from '../Utility/Button.svelte';
    export let onSubmit: () => void;
</script>

<div class="app-footer">
    {#if $appMessage}
        <div class="app-message">{$appMessage}</div>
    {/if}
    <div class="footer-inner">
        <div class="buttons">
            <p>
                <Button onClick={onAppClose} icon={'close'} disabled={$submitState}>{pDisplay('Close')}</Button>
            </p>
            <p>
                <Button type="positive" onClick={onSubmit} icon={'save'} loader={$submitState}
                    >{pDisplay('Update')}</Button
                >
            </p>
        </div>
    </div>
</div>

<style lang="scss">
    @use '../css/shared';

    .app-footer {
        position: absolute;
        bottom: 0;
        left: 0;
        z-index: 10;
        width: calc(73vw - 38px + 300px);
        padding: 0 0 0 300px;
        .app-message {
            padding: 8px;
            font-size: 14px;
            color: var(--invert-text);
            text-align: center;
            background: var(--warning-color);
        }
        .footer-inner {
            background: var(--sd-footer-command);
        }
        .buttons {
            display: flex;
            gap: 8px;
            align-items: center;
            justify-content: center;
            height: 72px;
            font-size: 13px;
        }
    }
</style>
