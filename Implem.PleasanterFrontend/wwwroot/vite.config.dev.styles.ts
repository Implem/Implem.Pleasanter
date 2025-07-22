import { defineConfig } from 'vite';
import path from 'path';
import { inputDir, configParams, getEntries } from './vita.config.shared';

export default defineConfig(({ mode }) => ({
    ...configParams,
    build: {
        ...configParams.build,
        sourcemap: mode === 'development',
        rollupOptions: {
            input: {
                ...getEntries(path.resolve(__dirname, `${inputDir}/styles`), '.scss')
            },
            output: {
                assetFileNames: `css/[name].min[extname]`
            }
        }
    }
}));
