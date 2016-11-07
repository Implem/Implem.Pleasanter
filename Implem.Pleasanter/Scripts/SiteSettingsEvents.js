$(function () {
    $(document).on('selectableselected', '#EditorColumns, #EditorSourceColumns', function () {
        $p.clearData('ColumnProperty,', $p.getData($(this)), 'startsWith');
    });
    $(document).on('change', '[id="ColumnProperty,ControlType"]', function () {
        var visibility = $(this).val() === 'Spinner';
        $('[id="ColumnPropertyField,Min"]').toggle(visibility);
        $('[id="ColumnPropertyField,Max"]').toggle(visibility);
        $('[id="ColumnPropertyField,Step"]').toggle(visibility);
    });
    $(document).on('change', '[id="ColumnProperty,FormatSelector"]', function () {
        var $control = $(this);
        switch ($control.val()) {
            case '\t':
                $('[id="ColumnPropertyField,Format"]').toggle(true);
                $('[id="ColumnProperty,Format"]').val('');
                break;
            default:
                $('[id="ColumnPropertyField,Format"]').toggle(false);
                $('[id="ColumnProperty,Format"]').val($control.val());
                break;
        }
        $p.setData($('[id="ColumnProperty,Format"]'));
    });
    $(document).on('click', '#SummarySettings .grid-row button', function () {
        var $control = $($(this).attr('data-selector'))
        $p.getData($control)[$control.attr('id') + "Id"] = $(this).attr('data-id');
        $p.send($control);
    });
    $(document).on('click', '#CreateNotification,#UpdateNotification', function () {
        var $control = $(this);
        $p.getData($control).NotificationType = $('#NotificationType').val();
        $p.send($control);
    });
    $(document).on('click', '#NotificationSettings .grid-row', function () {
        var $control = $(this);
        $p.getData($control).NotificationId = $control.attr('data-id');
        $p.openNotificationDialog($('#EditNotification'), '#NotificationDialog');
    });
    $(document).on('click', '#NotificationSettings .delete', function (e) {
        var $control = $(this);
        e.stopPropagation();
        $p.getData($control).NotificationId = $control.attr('data-id');
        $p.send($('#DeleteNotification'));
    });
});