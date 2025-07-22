import { defineConfig } from 'vite';
import copy from 'rollup-plugin-copy';
import path from 'path';
import { inputDir, outputDir, configParams, getEntries } from './vita.config.shared';

export default defineConfig({
    ...configParams.build,
    build: {
        ...configParams.build,
        emptyOutDir: true,
        rollupOptions: {
            input: {
                ...getEntries(path.resolve(__dirname, `${inputDir}/scripts`), '.ts'),
                ...getEntries(path.resolve(__dirname, `${inputDir}/styles`), '.scss')
            },
            output: {
                entryFileNames: `js/[name].min.js`,
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
