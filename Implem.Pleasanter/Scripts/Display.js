$p.display = function (defaultId) {
    var displays = {
        CheckAll: 'Check all',
        CheckAll: '全て選択',
        ConfirmCreateLink: 'Would you like to create a link ?',
        ConfirmCreateLink: 'リンクを作成しますか？',
        ConfirmDelete: 'Are you sure you want to delete ?',
        ConfirmDelete: '本当に削除してもよろしいですか？',
        ConfirmReload: 'Are you sure you want to reload this page?',
        ConfirmReload: 'このページを離れようとしています。',
        ConfirmSendMail: 'Are you sure you want to send an email ?',
        ConfirmSendMail: 'メールを送信してもよろしいですか？',
        ConfirmSeparate: 'Are you sure you want to separate ?',
        ConfirmSeparate: '分割してもよろしいですか？',
        ConfirmSynchronize: 'Are you sure you want to synchronize the data ?',
        ConfirmSynchronize: 'データを同期してもよろしいですか？',
        Manager: 'Manager',
        Manager: '管理者',
        OrderAsc: 'Asc',
        OrderAsc: '昇順',
        OrderDesc: 'Desc',
        OrderDesc: '降順',
        OrderRelease: 'Release',
        OrderRelease: '解除',
        ResetOrder: 'Reset',
        ResetOrder: 'リセット',
        UncheckAll: 'Uncheck all',
        UncheckAll: '全て解除',
        ValidateDate: 'This is an invalid date.',
        ValidateDate: '日付または時刻が不正です。',
        ValidateEmail: 'Doesn’t look like a valid email.',
        ValidateEmail: 'メールアドレスの形式が不正です。',
        ValidateEqualTo: 'Please enter the same value again.',
        ValidateEqualTo: '入力した文字列が一致しません。',
        ValidateMaxLength: 'Entered is too long.',
        ValidateMaxLength: '入力した文字が長すぎます。',
        ValidateMaxNumber: 'The number entered is too large.',
        ValidateMaxNumber: '入力された数字が大きすぎます。',
        ValidateMinNumber: 'The number entered is too small.',
        ValidateMinNumber: '入力された数字が小さすぎます。',
        ValidateNumber: 'You can not enter a non-numeric.',
        ValidateNumber: '数値以外入力不可です。',
        ValidateRequired: 'This information is required.',
        ValidateRequired: '入力必須項目です。'
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
