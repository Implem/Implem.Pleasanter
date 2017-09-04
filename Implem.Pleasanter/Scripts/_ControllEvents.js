$(function () {
    $(document).on('change', '[class^="control-"]:not(select[multiple])', function (e) {
        if ($(this).hasClass('control-spinner')) {
            if ($(this).val() === '' && $(this).hasClass('allow-blank')) {
                $(this).val('');
            } else if ($(this).val() === '' ||
                $(this).val().match(/[^0-9\.]/g) ||
                parseInt($(this).val()) < parseInt($(this).attr('data-min'))) {
                $(this).val($(this).attr('data-min'));
            } else if (parseInt($(this).val()) > parseInt($(this).attr('data-max'))) {
                $(this).val($(this).attr('data-max'));
            }
            $p.formChanged = true;
        }
        $p.setData($(this));
        e.preventDefault();
    });
    $(document).on('spin', '.control-spinner', function (event, ui) {
        $p.getData($(this))[this.id] = ui.value;
        $p.formChanged = true;
    });
    $(document).on('change', '.control-checkbox.visible', function () {
        show(this.id.substring(7, this.id.length), $(this).prop('checked'));

        function show(selector, value) {
            if (value) {
                $(selector).show();
            }
            else {
                $(selector).hide();
            }
        }
    });
    $(document).on('change', '.auto-postback:not([type="text"],select[multiple])', function () {
        $p.send($(this));
    });
    $(document).on('change', '.datepicker.auto-postback', function () {
        $p.send($(this));
    });
    $(document).on('keyup', '.auto-postback[type="text"]', function (e) {
        var $control = $(this);
        $p.setData($control);
        if (e.keyCode === 13) {
            $p.send($control);
            delete $p.getData($control)[$control.attr('id')];
        }
    });
});