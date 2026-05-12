<script lang="ts">
    import { onMount } from 'svelte';
    import { fade } from 'svelte/transition';

    /* data */
    let dialog: HTMLDialogElement;
    export let dialogContainer: HTMLElement | undefined = undefined;
    export let isState: boolean = false;
    export let onClose: (() => void) | undefined = undefined;
    export let onExit: (() => void) | undefined = undefined;

    /* watch */
    $: if (isState) {
        if (dialog && !dialog.open) {
            dialog.showModal();
        }
    } else {
        if (dialog && dialog.open) {
            dialog.close();
        }
    }

    /* method */
    const modalClose = () => {
        isState = false;
        onClose && onClose();
    };

    const modalExit = () => {
        if (onExit) {
            onExit();
        } else {
            isState = false;
            onClose && onClose();
        }
    };

    onMount(() => {
        dialog.addEventListener('close', () => {
            if (isState) modalClose();
        });
    });
</script>

<dialog class="ui-modal" bind:this={dialog}>
    {#if isState}
        <div class="modal-container" bind:this={dialogContainer} out:fade={{ duration: 300 }}>
            <div class="modal-wrap">
                <div class="modal-content">
                    <button class="modal-btn-close" on:click={modalClose}>
                        <span class="material-symbols-sharp">close</span>
                    </button>
                    <div class="modal-slot">
                        <slot />
                    </div>
                </div>
                <!-- svelte-ignore a11y_click_events_have_key_events -->
                <div class="modal-bg" role="button" tabindex="-1" on:click={modalExit}></div>
            </div>
        </div>
    {/if}
</dialog>

<style lang="scss">
    @use './shared';

    .ui-modal {
        position: relative;
        width: 100%;
        height: 100vh;
        padding: 0;
        overflow: visible;
        outline: none;
        // backdrop-filter: blur(5px);
        background-color: var(--u-modal-bg);
        border: none;
        opacity: 0;
        transition:
            opacity 200ms ease-out,
            overlay 250ms allow-discrete,
            display 250ms allow-discrete;
        &:-internal-dialog-in-top-layer {
            max-width: 100%;
            max-height: 100%;
        }

        &::backdrop {
            opacity: 0;
        }

        &[open] {
            display: block;
            opacity: 1;
            transform: translateY(0);
            transition: initial;
            animation: fade 200ms ease-out;
        }

        .modal-container {
            position: relative;
            height: 100%;
            padding: 0;
            margin: 0 8px;
            overflow: auto;
            animation: fadeUp 250ms ease-out;
            &::-webkit-scrollbar {
                width: 8px;
                height: 8px;
                margin: 10px 0;
            }
            &::-webkit-scrollbar-track {
                margin: 8px 0;
                background-color: transparent;
            }
            &::-webkit-scrollbar-corner {
                background-color: transparent;
            }
            &::-webkit-scrollbar-thumb {
                cursor: grab;
                background: var(--u-modal-scroll);
                border-radius: 8px;
                &:hover {
                    background-color: var(--u-modal-scroll-hover);
                }
                &:active {
                    cursor: grabbing;
                }
            }
        }
        .modal-wrap {
            position: relative;
            z-index: 1;
            display: flex;
            align-items: center;
            width: 100%;
            min-height: 100vh;
            padding: 0 0 24px;
            margin: auto;
        }
        .modal-content {
            position: relative;
            z-index: 1;
            width: fit-content;
            margin: auto;
        }
        .modal-bg {
            position: absolute;
            top: 0;
            left: 0;
            z-index: 0;
            width: 100%;
            min-height: 100%;
            background: transparent;
        }
        .modal-btn-close {
            position: sticky;
            top: 0;
            left: 100%;
            z-index: 2;
            width: 0;
            height: 0;
            padding: 0;
            margin: 0;
            cursor: pointer;
            outline: none;
            background-color: transparent;
            border: none;
            span {
                position: absolute;
                font-size: 48px;
                font-variation-settings: 'wght' 100;
                color: var(--u-modal-close);
            }
        }
    }

    @keyframes fadeUp {
        0% {
            opacity: 0;
            transform: translateY(30px);
        }
        100% {
            opacity: 1;
            transform: translateY(0);
        }
    }

    @keyframes fade {
        0% {
            opacity: 0;
        }
        100% {
            opacity: 1;
        }
    }
</style>
