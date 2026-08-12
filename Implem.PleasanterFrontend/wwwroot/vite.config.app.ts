import { defineConfig } from 'vite';
import path from 'path';
import { inputDir, configParams } from './vite.config.shared';

// app.ts のみIIFE でビルドする補助ビルド。
export default defineConfig({
    ...configParams,
    build: {
        ...configParams.build,
        emptyOutDir: false,
        manifest: 'app.manifest.json',
        rolldownOptions: {
            input: {
                app: path.resolve(__dirname, `${inputDir}/scripts/app.ts`)
            },
            output: {
                format: 'iife',
                entryFileNames: 'js/[name]_[hash].js'
            }
        }
    }
});
