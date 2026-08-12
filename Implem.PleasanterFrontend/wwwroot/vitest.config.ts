import { defineConfig } from 'vitest/config';

/**
 * ルート設定。2 つのプロジェクトを束ねる。
 * - unit    : happy-dom でラッパーロジックを検証 (SunEditor はモック)
 * - browser : Playwright/Chromium で実 SunEditor を検証 (文字装飾など)
 *
 * VS Code の Vitest 拡張はこのルート設定を読み、両プロジェクトを Test タブに表示する。
 * CLI では `--project unit` / `--project browser` で個別実行できる (package.json 参照)。
 */
export default defineConfig({
    test: {
        projects: ['./vitest.unit.config.ts', './vitest.browser.config.ts']
    }
});
