$(function () {
    $.validator.addMethod(
        'c_num',
        function (value, element) {
            return this.optional(element) || /^(-)?(¥|\\|\$)?[\d,.]+$/.test(value);
        }
    );
    $.validator.addMethod(
        'c_min_num',
        function (value, element, params) {
            return this.optional(element) ||
                parseFloat(value.replace(/[¥,]/g, '')) >= parseFloat(params);
        }
    );
    $.validator.addMethod(
        'c_max_num',
        function (value, element, params) {
            return this.optional(element) ||
                parseFloat(value.replace(/[¥,]/g, '')) <= parseFloat(params);
        }
    );

    $p.setValidationError = function ($form) {
        $form.find('.ui-tabs li').each(function () {
            $('.control-markdown.error').each(function () {
                $p.toggleEditor($(this), true);
            });
            var $control = $('#' + $(this)
                .attr('aria-controls'))
                .find('input.error:first:not(.search)');
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
            c_min_num: $p.display('ValidateMinNumber'),
            c_max_num: $p.display('ValidateMaxNumber'),
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
        $('[data-validate-min-number]').each(function () {
            $(this).rules('add', { c_min_num: $(this).attr('data-validate-min-number') });
        });
        $('[data-validate-max-number]').each(function () {
            $(this).rules('add', { c_max_num: $(this).attr('data-validate-max-number') });
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
        $('[data-validate-maxlength]').each(function () {
            $(this).rules('add', { maxlength: $(this).attr('data-validate-maxlength') });
        });
    }
    $p.applyValidator();
});