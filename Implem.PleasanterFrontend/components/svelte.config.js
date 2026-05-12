import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

export default {
    preprocess: [vitePreprocess()],
    compilerOptions: {
        customElement: true
    },
    onwarn: (warning, handler) => {
        if (warning.code === 'css_unused_selector') return;
        handler(warning);
    }
};
