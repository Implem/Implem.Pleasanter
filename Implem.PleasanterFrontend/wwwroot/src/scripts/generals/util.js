/** * グローバル一意識別子（GUID）を生成して返します。
 * UUID v4 を生成し、ハイフンを除去して大文字化した32文字の文字列を返します。
 * * 例: "F47AC10B58CC4372A5670E02B2C3D479"
 * * @returns {string} 生成されたGUIDを返します。
 * */
$p.createGuid = function () {
    // 1. まず標準のcrypto.randomUUIDを試す（HTTPSならこれが動く）
    try {
        if (typeof crypto !== 'undefined' && crypto.randomUUID) {
            return crypto.randomUUID().replace(/-/g, '').toUpperCase();
        }
    } catch (e) {
        // crypto.randomUUID が存在していても、ブラウザのセキュリティ設定によっては呼び出し時に「アクセス拒否エラー」で落ちることがためTry-Catchを用いる。
        // セキュリティエラー等で落ちた場合は無視してフォールバックへ進む
    }

    // 2. HTTP環境などで上記が使えない場合のフォールバック（Math.random使用）
    // UUID v4テンプレート (32文字: 8-4-4-4-12形式のハイフン抜き)
    var template = 'xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx';

    return template
        .replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0;
            var v = c === 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        })
        .toUpperCase();
};

/**
 * 画像アップロード前に、生ファイルサイズが上限（MB）を超えていないか検証します。
 * 上限値は hidden の #ImageUploadFileSizeLimit（サーバーパラメータ BinaryStorage.ImageUploadFileSizeLimit）から取得します。
 * 上限が未設定（0以下や未出力）の場合は制限なしとして true を返します。
 * 超過時はエラーメッセージを表示して false を返します。
 *
 * @param {File|Blob|null} blob 検証対象のファイル/Blob
 * @returns {boolean} 送信可能なら true、上限超過なら false
 */
$p.validateImageUploadFileSize = function (blob) {
    if (!blob) return true;
    var limitElem = document.querySelector('#ImageUploadFileSizeLimit');
    var limitMb = limitElem ? parseFloat(limitElem.value) : NaN;
    if (!(limitMb > 0)) return true;
    if (blob.size > limitMb * 1024 * 1024) {
        $p.setErrorMessage('TooLargeFile');
        return false;
    }
    return true;
};
