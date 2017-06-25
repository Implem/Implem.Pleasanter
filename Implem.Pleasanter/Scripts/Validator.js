$(function () {
    $.validator.addMethod(
        'c_num',
        function (value, element) {
            return this.optional(element) || /^(-)?(¥|\\|\$)?[\d,.]+$/.test(value);
        }
    );

    $p.setValidationError = function ($form) {
        $form.find('.ui-tabs li').each(function () {
            $('.control-markdown.error').each(function () {
                $p.toggleEditor($(this), true);
            });
            var $control = $('#' + $(this).attr('aria-controls')).find('.error:first');
            if ($control.length === 1) {
                $(this).closest('.ui-tabs').tabs('option', 'active', $(this).index());
                $control.focus();
            }
        });
    }

    $p.applyValidator = function () {
        $.extend($.validator.messages, {
            required: $p.display('ValidateRequired'),
            c_num: $p.display('ValidateNumber'),
            date: $p.display('ValidateDate'),
            email: $p.display('ValidateEmail'),
            equalTo: $p.display('ValidateEqualTo'),
            maxlength: $p.display('ValidateMaxLength')
        });
        $('form').each(function () {
            $(this).validate({ ignore: '' });
        });
        $('[data-validate-required="1"]').each(function () {
            $(this).rules('add', { required: true });
        });
        $('[data-validate-number="1"]').each(function () {
            $(this).rules('add', { c_num: true });
        });
        $('[data-validate-date="1"]').each(function () {
            $(this).rules('add', { date: true });
        });
        $('[data-validate-email="1"]').each(function () {
            $(this).rules('add', { email: true });
        });
        $('[data-validate-equal-to]').each(function () {
            $(this).rules('add', { equalTo: $(this).attr('data-validate-equal-to') });
        });
        $('[data-maxlength]').each(function () {
            $(this).rules('add', { maxlength: $(this).attr('data-validate-maxlength') });
        });
    }
    $p.applyValidator();
});