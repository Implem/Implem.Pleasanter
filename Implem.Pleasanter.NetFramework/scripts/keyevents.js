$(function () {
    $(document).on('keypress', 'form[data-enter] input:not([type="button"])', function (e) {
        if (e.which === 13) {
            $($(this).closest('form').attr('data-enter')).click();
        }
    });
    $(document).on('keypress', 'input', function (e) {
        return e.which !== 13;
    });
    $(document).on('keydown', '[class^="control-"]', function (e) {
        if (e.keyCode === 9) {
            var controlId = $(this).attr('id');
            var find = false;
            var ret = true;
            var status = 0;
            var $controls = !event.shiftKey
                ? $('[class^="control-"]')
                : $($('[class^="control-"]').get().reverse());
            $controls.each(function () {
                if (find) {
                    status = setFocus($(this));
                    if (status === 1) {
                        return false;
                    }
                    if (status === 2) {
                        ret = false;
                        return false;
                    }
                } else if ($(this).attr('id') === controlId) {
                    find = true;
                }
            });
            return ret;
        }

        function setFocus($control) {
            if ($control.hasClass('control-markdown')) {
                $p.toggleEditor($control, true);
                $('#' + $control.attr('id')).focus();
                return 2;
            } else if ($control.is(':hidden') ||
                $control.hasClass('control-text') ||
                $control.hasClass('control-markup')) {
                return 0;
            }
            return 1;
        }
    });
});