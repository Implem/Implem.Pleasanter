$(function () {
    $(document).on('change', '[class^="control-"]:not(select[multiple])', function (e) {
        var $control = $(this);
        if ($control.hasClass('control-spinner')) {
            if ($control.val() === '' && $control.hasClass('allow-blank')) {
                $control.val('');
            } else if ($control.val() === '' ||
                $control.val().match(/[^0-9\.]/g) ||
                parseInt($control.val()) < parseInt($control.attr('data-min'))) {
                $control.val($control.attr('data-min'));
            } else if (parseInt($control.val()) > parseInt($control.attr('data-max'))) {
                $control.val($control.attr('data-max'));
            }
            $p.setFormChanged($control);
        }
        $p.setData($control);
        e.preventDefault();
    });
    $(document).on('spin', '.control-spinner', function (event, ui) {
        var $control = $(this);
        var data = $p.getData($control);
        data[this.id] = ui.value;
        $p.setGridTimestamp($control, data);
        $p.setFormChanged($control);
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