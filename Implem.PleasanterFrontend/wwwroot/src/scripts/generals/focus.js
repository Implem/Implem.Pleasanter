$p.focusMainForm = function () {
    $('#FieldSetGeneral').find('[class^="control-"]').each(function () {
        if (!$(this).is(':hidden') &&
            !$(this).hasClass('control-text') &&
            !$(this).hasClass('control-markup'))
        {
            // 検索ダイアログがいきなり表示される場合は「全般」タブにフォーカスする
            if ($(this).hasClass('control-dropdown search'))
            {
                $('#EditorTabs > li:nth-child(1) > a').focus();
            }
            else
            {
                $(this).focus();
            }
            return false;
        }
    });
}