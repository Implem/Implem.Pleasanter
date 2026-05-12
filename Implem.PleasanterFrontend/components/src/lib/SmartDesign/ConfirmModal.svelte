<script lang="ts">
    import { confirmDisplay } from './store';
    import UiModal from './Utility/UiModal.svelte';
    import Button from './Utility/Button.svelte';
    import { pDisplay } from './Utility/$p';

    let isState: boolean;
    $: {
        if ($confirmDisplay && $confirmDisplay.message) {
            isState = true;
        } else {
            isState = false;
        }
    }

    const onAction = () => {
        isState = false;
        setTimeout(() => {
            $confirmDisplay?.onExecute();
        });
    };
    const onClose = () => {
        isState = false;
        $confirmDisplay = null;
    };
</script>

<UiModal bind:isState {onClose}>
    <div class="confirm-modal">
        <div class="modal-body">
            {$confirmDisplay?.message}
        </div>
        <footer class="modal-footer">
            <p><Button onClick={onClose} type="normal">{pDisplay('Cancel')}</Button></p>
            <p><Button onClick={onAction} icon={'check'}>OK</Button></p>
        </footer>
    </div>
</UiModal>

<style lang="scss">
    @use 'css/shared';

    .confirm-modal {
        position: relative;
        font-size: 13px;
        color: var(--base-text);
        background-color: var(--sd-base-bg);
        border-radius: 5px;
        box-shadow: 0 0 10px var(--sd-base-shadow);

        .modal-body {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 480px;
            min-height: 100px;
            padding: 24px;
            font-size: 14px;
            line-height: 1.8;
        }

        .modal-footer {
            display: flex;
            gap: 8px;
            justify-content: center;
            width: 100%;
            padding: 16px 0;
            background: var(--sd-footer-command);
            border-radius: 0 0 4px 4px;
            :global(button) {
                justify-content: center;
                min-width: 140px;
            }
        }
    }
</style>
