/**
 * ブラウザモード (Playwright) の RTE テストで共有するヘルパ群。
 *
 * richTextEditor.browser.test.ts (編集モード) と richTextEditor.viewer.browser.test.ts
 * (読み取り専用ビューア) の双方が使う定型処理を一箇所に集約する。
 * ※ `*.test.ts` にマッチしないファイル名にしてテストとして収集されないようにしている。
 */

/** C# が払い出す hidden input (#Language / #RteFontList 等) を追加する */
export function addHiddenInput(id: string, value: string) {
    const el = document.createElement('input');
    el.type = 'hidden';
    el.id = id;
    el.value = value;
    document.body.appendChild(el);
}

/**
 * 合成マウント環境 (フォーカス/モーダル/ドロップ) で SunEditor 内部が投げる、検証対象と
 * 無関係な既知の非同期例外を握りつぶす。base 定義に加え、ファイル固有のパターンを extra で渡す。
 * 一致しない例外はそのまま伝播させる。
 *
 * ※ "reading 'get'" のような汎用メッセージも対象にしているため、将来の実バグを黙って
 *   飲み込まないよう「抑制したこと」は残す: 抑制した distinct メッセージを初回だけ warn する
 *   (同一メッセージの大量再発ではノイズ化しない)。想定外の一致もここに 1 行現れる。
 */
export function installRteErrorSuppression(extra: string[] = []) {
    const patterns = ['scrollparents', '_preventBlur', "reading 'get'", ...extra];
    const warned = new Set<string>();
    window.addEventListener(
        'error',
        event => {
            const message = String(event.message);
            if (patterns.some(pattern => message.includes(pattern))) {
                if (!warned.has(message)) {
                    warned.add(message);
                    console.warn(`[rte-test] 既知の非同期例外を抑制しました (初回のみ通知): ${message}`);
                }
                event.stopImmediatePropagation();
                event.preventDefault();
            }
        },
        true
    );
}

/** 各テスト前の DOM リセット (前テストのエディタ/注入スタイルを除去する) */
export function resetRteDom() {
    document.body.innerHTML = '';
    document.head.querySelectorAll('#rteCustomCss').forEach(e => e.remove());
}

/**
 * 保留中の focus/sticky 由来の非同期処理を「エディタ生存中に」消化してから rt-editor を
 * 確実に破棄する (disconnectedCallback→destroy)。破棄後に非同期が null 文脈を触って投げる
 * stderr ノイズを抑える。afterEach から呼ぶ。
 */
export async function teardownEditors() {
    await new Promise(resolve => setTimeout(resolve, 0));
    document.querySelectorAll('rt-editor').forEach(el => el.remove());
    await new Promise(resolve => setTimeout(resolve, 0));
}

/** figure>img を並べた HTML 文字列を作る (画像ライトボックス系テスト用) */
export function figures(...srcs: string[]) {
    return srcs.map(src => `<figure><img src="${src}"></figure>`).join('');
}
