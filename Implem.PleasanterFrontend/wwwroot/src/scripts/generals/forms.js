/**
 * 現在のページがフォームかどうかを判定します。
 * <meta name="is-form" content="true"> が設定されている場合 true を返します。
 * @returns {boolean} フォームページであれば true、そうでなければ false を返します。
 */
$p.isForm = function () {
    return $('meta[name="is-form"]').attr('content') === 'true';
};
