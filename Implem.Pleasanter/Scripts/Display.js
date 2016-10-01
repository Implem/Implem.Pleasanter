$p.display = function (defaultId) {
    var displays = {
        OrderAsc: 'Asc',
        OrderAsc_ja: '昇順',
        OrderDesc: 'Desc',
        OrderDesc_ja: '降順',
        OrderRelease: 'Release',
        OrderRelease_ja: '解除',
        ResetOrder: 'Reset',
        ResetOrder_ja: 'リセット',
        CheckAll: 'Check all',
        CheckAll_ja: '全て選択',
        UncheckAll: 'Uncheck all',
        UncheckAll_ja: '全て解除',
        ConfirmDelete: 'Are you sure you want to delete ?',
        ConfirmDelete_ja: '本当に削除してもよろしいですか？',
        ConfirmSeparate: 'Are you sure you want to separate ?',
        ConfirmSeparate_ja: '分割してもよろしいですか？',
        ConfirmSendMail: 'Are you sure you want to send an email ?',
        ConfirmSendMail_ja: 'メールを送信してもよろしいですか？',
        ConfirmSynchronize: 'Are you sure you want to synchronize the data ?',
        ConfirmSynchronize_ja: 'データを同期してもよろしいですか？',
        ValidateRequired: 'This information is required.',
        ValidateRequired_ja: '入力必須項目です。',
        ValidateNumber: 'You can not enter a non-numeric.',
        ValidateNumber_ja: '数値以外入力不可です。',
        ValidateDate: 'This is an invalid date.',
        ValidateDate_ja: '日付または時刻が不正です。',
        ValidateMail: 'Doesn’t look like a valid email.',
        ValidateMail_ja: 'メールアドレスの形式が不正です。',
        ValidateEqualTo: 'Please enter the same value again.',
        ValidateEqualTo_ja: '入力した文字列が一致しません。'
    };
    var localId = defaultId + '_' + $('#Language').val();
    if (displays[localId]) {
        return displays[localId];
    } else if (displays[defaultId]) {
        return displays[defaultId];
    } else {
        return defaultId;
    }
}
