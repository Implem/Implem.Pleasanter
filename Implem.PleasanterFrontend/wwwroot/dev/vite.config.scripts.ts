import { defineConfig } from 'vite';
import path from 'path';
import { inputDir, configParams, getEntries } from '../vite.config.shared';

const rootDir = path.resolve(__dirname, '..');

export default defineConfig(({ mode }) => ({
    ...configParams,
    build: {
        ...configParams.build,
        sourcemap: mode === 'development' ? 'inline' : false,
        minify: false,
        reportCompressedSize: false,
        manifest: 'manifest.json',
        watch: {
            // buildDelay: 200  // Temporarily disabled for vite v7 compatibility testing
        },
        rollupOptions: {
            input: {
                ...getEntries(path.resolve(rootDir, `${inputDir}/scripts`), '.ts')
            },
            output: {
                manualChunks(id: string) {
                    if (id.includes('node_modules')) {
                        return 'vendor';
                    }
                },
                chunkFileNames: 'js/vendor_[hash].js',
                entryFileNames: 'js/[name]_[hash].js'
            }
        }
    }
}));
