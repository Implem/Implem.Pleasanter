import { defineConfig } from 'vite';
import copy from 'rollup-plugin-copy';
import path from 'path';
import { inputDir, outputDir, configParams, getEntries } from './vita.config.shared';

export default defineConfig({
    ...configParams,
    build: {
        ...configParams.build,
        emptyOutDir: true,
        manifest: 'manifest.json',
        rollupOptions: {
            input: {
                ...getEntries(path.resolve(__dirname, `${inputDir}/scripts`), '.ts'),
                ...getEntries(path.resolve(__dirname, `${inputDir}/styles`), '.scss')
            },
            output: {
                manualChunks(id: string) {
                    if (id.includes('node_modules')) {
                        return 'vendor';
                    }
                },
                chunkFileNames: 'js/vendor_[hash].js',
                entryFileNames: 'js/[name]_[hash].js',
                assetFileNames: `css/[name].min[extname]`
            },
            plugins: [
                copy({
                    targets: [
                        {
                            src: `${inputDir}/clone/*`,
                            dest: `${outputDir}/../`
                        },
                        {
                            src: `${inputDir}/plugins/*`,
                            dest: `${outputDir}/plugins/`
                        }
                    ],
                    hook: 'writeBundle',
                    copyOnce: true,
                    verbose: true
                })
            ]
        }
    }
});
