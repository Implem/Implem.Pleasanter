import { defineConfig } from 'vite';
import { viteStaticCopy } from 'vite-plugin-static-copy';
import path from 'path';
import { inputDir, configParams, getEntries } from './vite.config.shared';

// modules.ts と styles、および静的コピー(clone/plugins)をビルドするメインビルド。
// (この後に app ビルドが emptyOutDir:false で上乗せする。)
export default defineConfig({
    ...configParams,
    plugins: [
        viteStaticCopy({
            targets: [
                {
                    src: `${inputDir}/clone/**`,
                    dest: '..',
                    rename: { stripBase: 2 }
                },
                {
                    src: `${inputDir}/plugins/**`,
                    dest: 'plugins',
                    rename: { stripBase: 2 }
                }
            ]
        })
    ],
    build: {
        ...configParams.build,
        emptyOutDir: true,
        manifest: 'manifest.json',
        rolldownOptions: {
            input: {
                modules: path.resolve(__dirname, `${inputDir}/scripts/modules.ts`),
                ...getEntries(path.resolve(__dirname, `${inputDir}/styles`), '.scss')
            },
            output: {
                manualChunks(id: string) {
                    if (id.includes('node_modules')) {
                        return 'vendor';
                    }
                },
                entryFileNames: 'js/[name]_[hash].js',
                chunkFileNames: 'js/chunk_[hash].js',
                assetFileNames: `css/[name].min[extname]`
            }
        }
    }
});
