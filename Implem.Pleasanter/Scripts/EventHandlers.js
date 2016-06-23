$(function () {
    $(document).on('change', '.control-spinner', function () {
        if ($(this).val() === '' && $(this).hasClass('allow-blank')) {
            $(this).val('');
        } else if ($(this).val() === '' ||
            $(this).val().match(/[^0-9\.]/g) ||
            parseInt($(this).val()) < parseInt($(this).attr('min'))) {
            $(this).val($(this).attr('min'));
        } else if (parseInt($(this).val()) > parseInt($(this).attr('max'))) {
            $(this).val($(this).attr('max'));
        }
    });
    $(document).on('change', '[class^="control-"]', function (e) {
        setFormData($(this));
        e.preventDefault();
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
    $(document).on('change', '.auto-postback:not([type="text"])', function () {
        requestByForm(getForm($(this)), $(this));
        if (!$(this).hasClass('keep-form-data')) {
            delete getFormData($(this))[this.id];
        }
    });
    $(document).on('keyup', '.auto-postback[type="text"]', function (e) {
        setFormData($(this));
        if (e.keyCode === 13) {
            requestByForm(getForm($(this)), $(this));
            delete getFormData($(this))[this.id];
        } else {
            var timer = setTimeout(function (control) {
                setFormData(control);
                requestByForm(control.closest('form'), control);
                delete getFormData(control)[control.attr('id')];
            }, 700, $(this));
            $(document).on('keydown', '.auto-postback', function () {
                clearTimeout(timer);
            });
        }
    });
    $(document).on('submit', function () {
        return false;
    });
    $(document).on('click', 'a', function (e) {
        e.stopPropagation();
    });
    $(window).on('load scroll resize', function () {
        if ($('#Grid').length) {
            if ($(window).scrollTop() + $(window).height() >= $('#Grid').offset().top + $('#Grid').height()) {
                if ($('#GridOffset').val() !== '-1') {
                    setFormData($('#GridOffset'));
                    $('#GridOffset').val(-1);
                    requestByForm(getForm($('#Grid')), $('#Grid'));
                }
            }
        }
    });
});