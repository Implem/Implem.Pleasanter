import { defineConfig } from 'vite';
import path from 'path';
import { inputDir, configParams } from '../vite.config.shared';

const rootDir = path.resolve(__dirname, '..');

// 開発用 modules ビルド(本番 vite.config.ts の JS 部分と同仕様: ES + vendor → modules.manifest.json)。
// styles(scss)は dev/sass.watch.styles.mjs が別途ウォッチするためここには含めない。
export default defineConfig(({ mode }) => ({
    ...configParams,
    build: {
        ...configParams.build,
        sourcemap: mode === 'development' ? 'inline' : false,
        minify: false,
        reportCompressedSize: false,
        manifest: 'manifest.json',
        watch: {},
        rolldownOptions: {
            input: {
                modules: path.resolve(rootDir, `${inputDir}/scripts/modules.ts`)
            },
            output: {
                manualChunks(id: string) {
                    if (id.includes('node_modules')) {
                        return 'vendor';
                    }
                },
                entryFileNames: 'js/[name]_[hash].js',
                chunkFileNames: 'js/chunk_[hash].js'
            }
        }
    }
}));
