$p.display = function (defaultId) {
    var displays = {
        CanNotConnectCamera: 'Can not connect to the camera.',
        CanNotConnectCamera_ja: 'カメラに接続できません。',
        CheckAll: 'Check all',
        CheckAll_ja: '全て選択',
        ConfirmCreateLink: 'Would you like to create a link ?',
        ConfirmCreateLink_ja: 'リンクを作成しますか？',
        ConfirmDelete: 'Are you sure you want to delete ?',
        ConfirmDelete_ja: '本当に削除してもよろしいですか？',
        ConfirmReload: 'Are you sure you want to reload this page?',
        ConfirmReload_ja: 'このページを離れようとしています。',
        ConfirmReset: 'Are you sure you want to reset ?',
        ConfirmReset_ja: '本当にリセットしてもよろしいですか？',
        ConfirmSendMail: 'Are you sure you want to send an email ?',
        ConfirmSendMail_ja: 'メールを送信してもよろしいですか？',
        ConfirmSeparate: 'Are you sure you want to separate ?',
        ConfirmSeparate_ja: '分割してもよろしいですか？',
        ConfirmSynchronize: 'Are you sure you want to synchronize the data ?',
        ConfirmSynchronize_ja: 'データを同期してもよろしいですか？',
        Manager: 'Manager',
        Manager_ja: '管理者',
        OrderAsc: 'Asc',
        OrderAsc_ja: '昇順',
        OrderDesc: 'Desc',
        OrderDesc_ja: '降順',
        OrderRelease: 'Release',
        OrderRelease_ja: '解除',
        ResetOrder: 'Reset',
        ResetOrder_ja: 'リセット',
        UncheckAll: 'Uncheck all',
        UncheckAll_ja: '全て解除',
        ValidateDate: 'This is an invalid date.',
        ValidateDate_ja: '日付または時刻が不正です。',
        ValidateEmail: 'Doesn’t look like a valid email.',
        ValidateEmail_ja: 'メールアドレスの形式が不正です。',
        ValidateEqualTo: 'Please enter the same value again.',
        ValidateEqualTo_ja: '入力した文字列が一致しません。',
        ValidateMaxLength: 'Entered is too long.',
        ValidateMaxLength_ja: '入力した文字が長すぎます。',
        ValidateMaxNumber: 'The number entered is too large.',
        ValidateMaxNumber_ja: '入力された数字が大きすぎます。',
        ValidateMinNumber: 'The number entered is too small.',
        ValidateMinNumber_ja: '入力された数字が小さすぎます。',
        ValidateNumber: 'You can not enter a non-numeric.',
        ValidateNumber_ja: '数値以外入力不可です。',
        ValidateRequired: 'This information is required.',
        ValidateRequired_ja: '入力必須項目です。',
        ValidationError: 'There is an error in the entered content.',
        ValidationError_ja: '入力された内容に誤りがあります。'
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
