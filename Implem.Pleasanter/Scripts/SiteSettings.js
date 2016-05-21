$(function () {
    $(document).on('selectableselected', '#EditorColumns', function () {
        clearFormData('ColumnProperty,', getFormData($('.main-form')), 'startsWith');
    });
    $(document).on('change', '[id="ColumnProperty,ControlType"]', function () {
        var visibility = $(this).val() === 'Spinner';
        $('[id="Field_ColumnProperty,Min"]').toggle(visibility);
        $('[id="Field_ColumnProperty,Max"]').toggle(visibility);
        $('[id="Field_ColumnProperty,Step"]').toggle(visibility);
    });
});