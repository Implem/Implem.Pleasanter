$(function () {
    $(document).on('selectableselected', '#EditorColumns', function () {
        clearFormData('ColumnProperty,', getFormData($('.main-form')), 'startsWith');
    });
    $(document).on('change', '[id="ColumnProperty,ControlType"]', function () {
        var visibility = $(this).val() === 'Spinner';
        $('[id="ColumnPropertyField,Min"]').toggle(visibility);
        $('[id="ColumnPropertyField,Max"]').toggle(visibility);
        $('[id="ColumnPropertyField,Step"]').toggle(visibility);
    });
});