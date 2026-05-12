import { defineConfig } from 'vite';
import { svelte } from '@sveltejs/vite-plugin-svelte';

const isWatchMode = process.argv.includes('--watch');
const outputDir = '../../Implem.Pleasanter/wwwroot/components';
const filePrefix = isWatchMode ? 'watch/' : '';

export default defineConfig({
    plugins: [
        svelte({
            compilerOptions: {
                customElement: true
            }
        })
    ],
    css: {
        preprocessorMaxWorkers: true,
        preprocessorOptions: {
            scss: {
                importers: [
                    {
                        findFileUrl(url: string) {
                            if (!url.startsWith('.') && !url.startsWith('/')) {
                                try {
                                    return new URL(import.meta.resolve(url));
                                } catch {
                                    /* not a package */
                                }
                            }
                            return null;
                        }
                    }
                ]
            }
        }
    },
    build: {
        watch: isWatchMode
            ? {
                  buildDelay: 500
              }
            : null,
        outDir: outputDir,
        emptyOutDir: !isWatchMode,
        manifest: 'manifest.json',
        sourcemap: isWatchMode,
        chunkSizeWarningLimit: 1000,
        rollupOptions: {
            input: 'src/main.ts',
            output: {
                manualChunks: id => (id.includes('node_modules') ? 'vendor' : undefined),
                entryFileNames: `${filePrefix}components_[hash].js`,
                chunkFileNames: `${filePrefix}[name]_[hash].js`,
                assetFileNames: `${filePrefix}[name].[ext]`
            }
        }
    }
});
