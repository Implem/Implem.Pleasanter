$p.display = function (defaultId) {
    var displays = {
        Displays_OrderAsc: 'Asc',
        Displays_OrderAsc_ja: '昇順',
        Displays_OrderDesc: 'Desc',
        Displays_OrderDesc_ja: '降順',
        Displays_OrderRelease: 'Release',
        Displays_OrderRelease_ja: '解除',
        Displays_ResetOrder: 'Reset',
        Displays_ResetOrder_ja: 'リセット',
        Displays_CheckAll: 'Check all',
        Displays_CheckAll_ja: '全て選択',
        Displays_UncheckAll: 'Uncheck all',
        Displays_UncheckAll_ja: '全て解除',
        Displays_ConfirmDelete: 'Are you sure you want to delete ?',
        Displays_ConfirmDelete_ja: '本当に削除してもよろしいですか？',
        Displays_ConfirmSeparate: 'Are you sure you want to separate ?',
        Displays_ConfirmSeparate_ja: '分割してもよろしいですか？',
        Displays_ConfirmSendMail: 'Are you sure you want to send an email ?',
        Displays_ConfirmSendMail_ja: 'メールを送信してもよろしいですか？',
        Displays_ConfirmSynchronize: 'Are you sure you want to synchronize the data ?',
        Displays_ConfirmSynchronize_ja: 'データを同期してもよろしいですか？'
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
