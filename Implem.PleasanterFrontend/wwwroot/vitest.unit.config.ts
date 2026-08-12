import { defineConfig, configDefaults, type Plugin } from 'vitest/config';

/**
 * ユニットテスト (happy-dom) プロジェクト。
 * SunEditor をモックし、ラッパーのロジックを高速・決定論的に検証する。
 *
 * `?inline` で取り込む CSS/SCSS (suneditor 本体 CSS・自前 scss) を空文字へスタブし、
 * sass/suneditor-css の実コンパイルを避けて高速化する。
 */
const stubInlineStyles = (): Plugin => ({
    name: 'stub-inline-styles',
    enforce: 'pre',
    resolveId(id) {
        if (id.endsWith('?inline')) return `\0stub-style:${id}`;
    },
    load(id) {
        if (id.startsWith('\0stub-style:')) return 'export default "";';
    }
});

export default defineConfig({
    plugins: [stubInlineStyles()],
    test: {
        name: 'unit',
        environment: 'happy-dom',
        globals: true,
        setupFiles: ['./src/test/setup.ts'],
        include: ['src/**/*.{test,spec}.ts'],
        // ブラウザモード専用テストは browser プロジェクトで実行する
        exclude: [...configDefaults.exclude, '**/*.browser.test.ts']
    }
});
