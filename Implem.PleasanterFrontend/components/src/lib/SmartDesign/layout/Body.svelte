<script lang="ts">
    import { appState, apiLoaded, onAppClose, hasAppEdited, confirmDisplay } from '../store';
    import { fade, slide } from 'svelte/transition';
    import { quintOut } from 'svelte/easing';
    import Header from './Header.svelte';
    import Footer from './Footer.svelte';
    import { pDisplay } from '../Utility/$p';
    export let onSubmit: () => void;
</script>

<div class="app-panel">
    <div class="app-panel-wrap" class:is-active={$appState} out:slide={{ duration: 800, easing: quintOut, axis: 'x' }}>
        {#if $apiLoaded}
            <div class="app-panel-inner">
                <Header />
                <div class="app-stage-body">
                    <div class="stage-body-inner">
                        <slot />
                    </div>
                </div>
                <Footer {onSubmit} />
            </div>
        {/if}
    </div>
    <div
        class="app-panel-bg"
        role="button"
        tabindex="0"
        on:click={() => {
            if ($hasAppEdited) {
                $confirmDisplay = {
                    message: pDisplay('SmartDesignExit'),
                    onExecute: onAppClose
                };
            } else {
                onAppClose();
            }
        }}
        on:keydown={e => {
            if (e.key === 'Enter' || e.key === ' ') {
                if ($hasAppEdited) {
                    $confirmDisplay = {
                        message: pDisplay('SmartDesignExit'),
                        onExecute: onAppClose
                    };
                } else {
                    onAppClose();
                }
            }
        }}
        transition:fade={{ duration: 800, easing: quintOut }}
    >
        {#if !$apiLoaded}
            <div class="loader"></div>
        {/if}
    </div>
</div>

<style lang="scss">
    @use '../css/shared';

    /* stage */
    .app-panel {
        position: fixed;
        top: 0;
        left: 0;
        z-index: 1000;
        width: 100%;
        height: 100%;
        font-family:
            'Noto Sans JP',
            'Noto Sans',
            system-ui,
            -apple-system,
            'Segoe UI',
            Roboto,
            'Hiragino Sans',
            'Hiragino Kaku Gothic ProN',
            'Yu Gothic',
            Meiryo,
            Arial,
            sans-serif,
            'Apple Color Emoji',
            'Segoe UI Emoji';
        color: var(--sd-base-text);
        .app-panel-wrap {
            position: absolute;
            top: 0;
            right: 0;
            z-index: 2;
            height: 100%;
            padding: 16px 0 16px 16px;
            overflow: hidden;
            background: var(--sd-base-stage);
            &:not(:empty) {
                width: calc(73vw - 38px + 300px);
                box-shadow: 0 0 8px var(--sd-base-shadow);
                &.is-active {
                    animation: slide 800ms cubic-bezier(0.23, 1, 0.32, 1);
                }
            }
        }
        .app-panel-inner {
            position: relative;
            width: calc(73vw - 38px + 300px - 16px);
            height: 100%;
            overflow: hidden;
            border-right: 0;
            border-radius: 8px 0 0 8px;
            box-shadow: 0 0 8px var(--sd-base-shadow);
        }
        .app-panel-bg {
            position: absolute;
            top: 0;
            left: 0;
            z-index: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            height: 100%;
            background: var(--sd-base-outer);
            backdrop-filter: blur(4px);
            will-change: backdrop-filter;
        }
        .app-stage-body {
            width: 100%;
            height: calc(100% - 50px);
            overflow: hidden;
            .stage-body-inner {
                display: flex;
                height: 100%;
                background: var(--sd-body-inner);
            }
        }
    }

    .loader {
        width: 50px;
        aspect-ratio: 1;
        border: 8px solid var(--sd-base-loader);
        border-radius: 50%;
        animation:
            l20-1 0.8s infinite linear alternate,
            l20-2 1.6s infinite linear;
    }

    @keyframes l20-1 {
        0% {
            clip-path: polygon(50% 50%, 0 0, 50% 0%, 50% 0%, 50% 0%, 50% 0%, 50% 0%);
        }
        12.5% {
            clip-path: polygon(50% 50%, 0 0, 50% 0%, 100% 0%, 100% 0%, 100% 0%, 100% 0%);
        }
        25% {
            clip-path: polygon(50% 50%, 0 0, 50% 0%, 100% 0%, 100% 100%, 100% 100%, 100% 100%);
        }
        50% {
            clip-path: polygon(50% 50%, 0 0, 50% 0%, 100% 0%, 100% 100%, 50% 100%, 0% 100%);
        }
        62.5% {
            clip-path: polygon(50% 50%, 100% 0, 100% 0%, 100% 0%, 100% 100%, 50% 100%, 0% 100%);
        }
        75% {
            clip-path: polygon(50% 50%, 100% 100%, 100% 100%, 100% 100%, 100% 100%, 50% 100%, 0% 100%);
        }
        100% {
            clip-path: polygon(50% 50%, 50% 100%, 50% 100%, 50% 100%, 50% 100%, 50% 100%, 0% 100%);
        }
    }

    @keyframes l20-2 {
        0% {
            transform: scaleY(1) rotate(0deg);
        }
        49.99% {
            transform: scaleY(1) rotate(135deg);
        }
        50% {
            transform: scaleY(-1) rotate(0deg);
        }
        100% {
            transform: scaleY(-1) rotate(-135deg);
        }
    }

    @keyframes slide {
        0% {
            right: calc((73vw - 38px + 300px) * -1);
        }
        100% {
            right: 0;
        }
    }
</style>
