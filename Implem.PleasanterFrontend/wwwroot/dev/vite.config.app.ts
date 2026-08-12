import { defineConfig } from 'vite';
import path from 'path';
import { inputDir, configParams } from '../vite.config.shared';

const rootDir = path.resolve(__dirname, '..');

// app.ts のみIIFE でビルドする補助ビルド。
export default defineConfig(({ mode }) => ({
    ...configParams,
    build: {
        ...configParams.build,
        sourcemap: mode === 'development' ? 'inline' : false,
        minify: false,
        reportCompressedSize: false,
        manifest: 'app.manifest.json',
        watch: {},
        rolldownOptions: {
            input: {
                app: path.resolve(rootDir, `${inputDir}/scripts/app.ts`)
            },
            output: {
                format: 'iife',
                entryFileNames: 'js/[name]_[hash].js'
            }
        }
    }
}));
