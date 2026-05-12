<svelte:options
    customElement={{
        tag: 'smart-design-link',
        extend: customElementConstructor => {
            return class extends customElementConstructor {
                constructor() {
                    super();
                    // @ts-expect-error Custom Elementのインスタンスを Svelte props (rootElem) に渡すための回避策
                    this.rootElem = this;
                }
            };
        }
    }}
/>

<script lang="ts">
    import { appState, supportUrl } from './store';
    export let rootElem: HTMLElement;
    const mainTagName = 'smart-design';
    $supportUrl = rootElem.dataset.supportUrl;
    const onOpen = () => {
        $appState = true;
    };
    if (!document.querySelector(mainTagName)) {
        const mainElem = document.createElement(mainTagName);
        document.body.append(mainElem);
    }
</script>

<div
    role="button"
    tabindex="0"
    on:click={onOpen}
    on:keydown={e => (e.key === 'Enter' || e.key === ' ') && onOpen()}
    aria-label="Trigger action"
>
    <slot />
</div>
