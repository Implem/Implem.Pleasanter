import path from 'path';
import fs from 'fs';

const inputDir = './src';
const outputDir = '../../Implem.Pleasanter/wwwroot/assets';

const getEntries = (dir: string, extension: string) => {
    const entries: Record<string, string> = {};
    const files = fs.readdirSync(dir);

    for (const file of files) {
        const filePath = path.resolve(dir, file);
        const stats = fs.statSync(filePath);

        if (stats.isFile() && file.endsWith(extension) && !file.startsWith('_')) {
            const entryName = file.replace(extension, '');
            entries[entryName] = filePath;
        }
    }
    return entries;
};

const configParams = {
    root: path.resolve(__dirname),
    base: './',
    build: {
        outDir: path.resolve(__dirname, outputDir),
        emptyOutDir: false
    },
    resolve: {
        alias: {
            '@': path.resolve(__dirname, 'ts')
        }
    }
};

export { inputDir, outputDir, configParams, getEntries };
