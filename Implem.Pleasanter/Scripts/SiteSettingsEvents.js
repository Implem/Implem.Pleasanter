$(function () {
    $(document).on('selectableselected', '#EditorColumns', function () {
        $p.clearData('ColumnProperty,', $p.getData(), 'startsWith');
    });
    $(document).on('change', '[id="ColumnProperty,ControlType"]', function () {
        var visibility = $(this).val() === 'Spinner';
        $('[id="ColumnPropertyField,Min"]').toggle(visibility);
        $('[id="ColumnPropertyField,Max"]').toggle(visibility);
        $('[id="ColumnPropertyField,Step"]').toggle(visibility);
    });
    $(document).on('change', '[id="ColumnProperty,FormatSelector"]', function () {
        switch ($(this).val()) {
            case '\t':
                $('[id="ColumnPropertyField,Format"]').toggle(true);
                $('[id="ColumnProperty,Format"]').val('');
                break;
            default:
                $('[id="ColumnPropertyField,Format"]').toggle(false);
                $('[id="ColumnProperty,Format"]').val($(this).val());
                break;
        }
        $p.getData().ColumnProperty, Format = $('[id="ColumnProperty,Format"]').val();
    });
});