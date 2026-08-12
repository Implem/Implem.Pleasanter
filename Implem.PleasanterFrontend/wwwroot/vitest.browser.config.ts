import { defineConfig } from 'vitest/config';
import { playwright } from '@vitest/browser-playwright';

/**
 * ブラウザモード用の Vitest 設定 (Playwright/Chromium)。
 * *.browser.test.ts のみを対象とし、SunEditor をモックせず実ブラウザで動かす。
 * (happy-dom 単体では検証できない「文字装飾など実プラグイン挙動」を検証する)
 *
 * 実行: npm run test:browser
 * 単体テスト (happy-dom) とは別プロセス/別 include で分離している。
 * ※ Vitest 4 で provider は @vitest/browser-playwright の playwright() 関数指定に変更。
 */
export default defineConfig({
    test: {
        name: 'browser',
        include: ['src/**/*.browser.test.ts'],
        browser: {
            enabled: true,
            provider: playwright(),
            headless: true,
            screenshotFailures: false, // 失敗時の PNG 自動生成を無効化
            instances: [{ browser: 'chromium' }]
        }
    }
});
