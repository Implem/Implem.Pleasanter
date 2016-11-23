$(function () {
    $(document).on('selectableselected', '#GridColumns, #GridSourceColumns', function () {
        $p.clearData('GridColumnProperty,', $p.getData($(this)), 'startsWith');
    });
    $(document).on('change', '[id="GridColumnProperty,UseGridDesign"]', function () {
        $('[id="GridColumnProperty,GridDesignField"]').toggle($(this).prop('checked'));
    });
    $(document).on('selectableselected', '#EditorColumns, #EditorSourceColumns', function () {
        $p.clearData('EditorColumnProperty,', $p.getData($(this)), 'startsWith');
    });
    $(document).on('change', '[id="EditorColumnProperty,ControlType"]', function () {
        var visibility = $(this).val() === 'Spinner';
        $('[id="EditorColumnPropertyField,Min"]').toggle(visibility);
        $('[id="EditorColumnPropertyField,Max"]').toggle(visibility);
        $('[id="EditorColumnPropertyField,Step"]').toggle(visibility);
    });
    $(document).on('change', '[id="EditorColumnProperty,FormatSelector"]', function () {
        var $control = $(this);
        switch ($control.val()) {
            case '\t':
                $('[id="EditorColumnPropertyField,Format"]').toggle(true);
                $('[id="EditorColumnProperty,Format"]').val('');
                break;
            default:
                $('[id="EditorColumnPropertyField,Format"]').toggle(false);
                $('[id="EditorColumnProperty,Format"]').val($control.val());
                break;
        }
        $p.setData($('[id="EditorColumnProperty,Format"]'));
    });
    $(document).on('click', '#SummarySettings .grid-row button', function () {
        var $control = $($(this).attr('data-selector'))
        $p.getData($control)[$control.attr('id') + "Id"] = $(this).attr('data-id');
        $p.send($control);
    });
    $(document).on('click', '#AddDataViewSorter', function () {
        var $dataViewSorter = $('#DataViewSorterSelector option:selected');
        var $dataViewSorterOrderType = $('#DataViewSorterOrderTypes option:selected');
        var orderType = $dataViewSorterOrderType.val();
        $p.addBasket(
            $('#DataViewSorters'),
            $dataViewSorter.text() + '(' + $p.display('Order' + orderType) + ')',
            $dataViewSorter.val() + ',' + orderType);
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