import { vi } from 'vitest';

/**
 * Pleasanter 本体が実行時に用意するグローバル `$p` (= window.$p) を、
 * テスト用の最小スタブとして current window/globalThis へ設定する。
 *
 * config の setupFiles 経由でも、各テストの beforeEach からでも呼べるようにして、
 * 実行経路 (CLI / IDE 拡張) や実行タイミングに依存せず必ず $p を用意する。
 */
export function installPleasanterGlobals() {
    const $pStub = {
        display: (key: string) => key,
        set: vi.fn(),
        isForm: vi.fn(() => false),
        multiUpload: vi.fn(),
        handleMessageFromJson: vi.fn(() => false),
        validateImageUploadFileSize: vi.fn(() => true)
    };
    (globalThis as unknown as { $p: typeof $pStub }).$p = $pStub;
    if (typeof window !== 'undefined') {
        (window as unknown as { $p: typeof $pStub }).$p = $pStub;
    }
    return $pStub;
}

// setupFiles として読み込まれた場合の即時適用 (window が無い環境では globalThis のみ)
installPleasanterGlobals();
