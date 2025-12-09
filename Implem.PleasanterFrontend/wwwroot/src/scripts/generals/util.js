/** * グローバル一意識別子（GUID）を生成して返します。
 * UUID v4 を生成し、ハイフンを除去して大文字化した32文字の文字列を返します。
 * * 例: "F47AC10B58CC4372A5670E02B2C3D479"
 * * @returns {string} 生成されたGUIDを返します。
 * */
$p.createGuid = function () {
    return crypto.randomUUID().replace(/-/g, '').toUpperCase();
};
