import { defineConfig } from 'vite';
import path from 'path';
import { inputDir, configParams, getEntries } from './vita.config.shared';

export default defineConfig(({ mode }) => ({
    ...configParams,
    build: {
        ...configParams.build,
        sourcemap: mode === 'development',
        manifest: 'manifest.json',
        rollupOptions: {
            input: {
                ...getEntries(path.resolve(__dirname, `${inputDir}/scripts`), '.ts')
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
