$(function () {
    $.validator.addMethod('c_attachments_required', function (value, element) {
        var $control = $('[id="' + $(element).data('name') + '.items"]');
        return $control.find('.control-attachments-item:not(.preparation-delete)').length > 0;
    });
    $.validator.addMethod('c_num', function (value, element) {
        const lang = $('#Language').val();
        return this.optional(element) || $p.parseCurrencyString(value, lang) !== null;
    });
    $.validator.addMethod('c_min_num', function (value, element, params) {
        if (this.optional(element)) return true;
        const lang = $('#Language').val();
        const num = $p.parseCurrencyString(value, lang);
        if (num === null) return false;
        return num >= parseFloat(params);
    });
    $.validator.addMethod('c_max_num', function (value, element, params) {
        if (this.optional(element)) return true;
        const lang = $('#Language').val();
        const num = $p.parseCurrencyString(value, lang);
        if (num === null) return false;
        return num <= parseFloat(params);
    });
    $.validator.addMethod('c_regex', function (value, element, params) {
        try {
            return this.optional(element) || new RegExp(params).test(value);
        } catch (e) {
            return false;
        }
    });
    $.validator.addMethod('maxlength', function (value, element, params) {
        try {
            if ($('#data-validation-maxlength-type').val() === 'Regex') {
                return (
                    value.length +
                    value.replace(
                        new RegExp(
                            '[' + $('#data-validation-maxlength-regex').val() + ']',
                            'g'
                        ),
                        ''
                    ).length <=
                    parseFloat(params)
                );
            } else {
                return value.length <= parseFloat(params);
            }
        } catch (e) {
            return false;
        }
    });

    $p.setValidationError = function ($form) {
        $form.find('.ui-tabs li').each(function () {
            $('.control-markdown.error').each(function () {
                var markdownField = $(this).get(0).closest('markdown-field');
                if (markdownField && !$(this).is(':visible')) {
                    markdownField.showEditor();
                }
            });
            var $control = $('#' + $(this).attr('aria-controls')).find(
                'input.error:first:not(.search)'
            );
            if ($control.length === 1) {
                $(this).closest('.ui-tabs').tabs('option', 'active', $(this).index());
                $control.focus();
            }
        });
    };

    $p.formValidate = function ($form, $control) {
        $form.find('input, select, textarea').each(function () {
            // SunEditor 関連要素は即スキップ
            if ($(this).is('[class^="se-"]')) return;
            // type 属性のない input もスキップ
            if (this.tagName.toLowerCase() === 'input' && !$(this).attr('type')) return;
            // フォームに属していない or バリデーション未初期化もスキップ
            if (!this.form || !$(this.form).data('validator')) return;

            $(this).rules('remove');
        });
        if (!$control.data('validations') || $control.hasClass('merge-validations')) {
            $p.applyValidator();
        }
        if ($control.data('validations')) {
            $.each($control.data('validations'), function (i, validation) {
                var $target = $p.getControl(validation.ColumnName);
                if (validation.Required) {
                    if ($target.hasClass('control-attachments')) {
                        $target.rules('add', {
                            c_attachments_required: true
                        });
                    } else {
                        $target.rules('add', {
                            required: true
                        });
                    }
                }
                if (validation.ClientRegexValidation) {
                    $target.rules('add', {
                        c_regex: validation.ClientRegexValidation,
                        messages: { c_regex: validation.RegexValidationMessage }
                    });
                }
                if (validation.Min != undefined) {
                    $target.rules('add', {
                        c_min_num: validation.Min
                    });
                }
                if (validation.Max != undefined) {
                    $target.rules('add', {
                        c_max_num: validation.Max
                    });
                }
            });
        }
        $form.validate({
            errorPlacement: function (error, element) {
                // 複数選択の分類項目に対応するためエラー表示の位置をフィールドの最下段に移動する
                element.closest('.container-normal').append(error);
            }
        });
    };

    $p.parseCurrencyString = function (input, lang) {
        if (!input || input.trim() === '') return 0;
        const raw = input.trim();
        let format;
        switch (lang) {
            case 'en':
                format = {
                    currencyRegex: /\$/,
                    mainPattern: /^-?\s?(\$\s?)?[0-9][0-9,]*(\.[0-9]+)?$/,
                    bannedPatterns: [
                        /\$\$/,
                        /,,/,
                        /,$/,
                        /\.[0-9]*,/,
                        /\$-/,
                        /[0-9]\s*\$/,
                        /^[.,]/,
                        /[.,]\s*$/,
                        /\$.*\$/,
                        /[^0-9$,\\.\-\s]/
                    ]
                };
                break;
            case 'zh':
            case 'ja':
                // zh-CN / ja-JP: -¥1,000,000.00 または ¥1,000,000.00
                format = {
                    currencyRegex: /[¥\\￥]/,
                    mainPattern: /^-?\s?([¥\\￥])?[0-9][0-9,]*(\.[0-9]+)?$/,
                    bannedPatterns: [
                        /,,/,
                        /,$/,
                        /\.[0-9]*,/,
                        /^\s*[¥\\￥]-/,
                        /[0-9]\s*[¥\\￥]$/,
                        /^\./
                    ]
                };
                break;
            case 'es':
            case 'de':
                format = {
                    currencyRegex: /[€]/,
                    mainPattern: /^-?\s?[0-9][0-9.,]*(\s?€)?$/,
                    bannedPatterns: [
                        /\.{2,}/,
                        /,{2,}/,
                        /,[0-9]*\./,
                        /[€].*[€]/,
                        /^[,\\.]/,
                        /\s\d/,
                        /[.,]\s*€/,
                        /[.,]\s*$/,
                        /^€/
                    ]
                };
                break;
            case 'ko':
                format = {
                    currencyRegex: /₩/,
                    mainPattern: /^-?\s?(₩\s?)?[0-9][0-9,]*(\.[0-9]+)?$/,
                    bannedPatterns: [
                        /₩₩/,
                        /,,/,
                        /,$/,
                        /\.[0-9]*,/,
                        /₩-/,
                        /[0-9]\s*₩/,
                        /^[.,]/,
                        /[.,]\s*$/,
                        /[₩].*[₩]/,
                        /[^0-9₩,.\-\s]/
                    ]
                };
                break;
            case 'vn':
                format = {
                    currencyRegex: /[₫]/,
                    mainPattern: /^-?\s?[0-9][0-9.,]*(\s?₫)?$/,
                    bannedPatterns: [
                        /\.{2,}/,
                        /,{2,}/,
                        /,[0-9]*\./,
                        /[₫].*[₫]/,
                        /^[,\\.]/,
                        /\s\d/,
                        /[.,]\s*₫/,
                        /[.,]\s*$/,
                        /^₫/
                    ]
                };
                break;
            default:
                return null;
        }
        if (!format.mainPattern.test(raw)) return null;
        for (let i = 0; i < format.bannedPatterns.length; i++) {
            if (format.bannedPatterns[i].test(raw)) {
                return null;
            }
        }
        let normalized = raw.replace(format.currencyRegex, '').replace(/\s/g, '');
        if (lang === 'de' || lang === 'es' || lang === 'vn') {
            // ドットは千区切りなので除去、カンマを小数点に
            normalized = normalized.replace(/\./g, '').replace(/,/g, '.');
        } else {
            // 英語・日本語・中国語・韓国語: カンマ除去、小数点ドットは残す
            normalized = normalized.replace(/,/g, '');
        }
        const num = parseFloat(normalized);
        if (isNaN(num)) return null;
        return num; // 符号は parseFloat が処理するので再適用しない
    };

    $p.applyValidator();
});
$p.applyValidator = function () {
    $.extend($.validator.messages, {
        required: $p.display('ValidateRequired'),
        c_attachments_required: $p.display('ValidateRequired'),
        c_num: $p.display('ValidateNumber'),
        c_min_num: $p.display('ValidateMinNumber'),
        c_max_num: $p.display('ValidateMaxNumber'),
        date: $p.display('ValidateDate'),
        email: $p.display('ValidateEmail'),
        equalTo: $p.display('ValidateEqualTo'),
        maxlength: $p.display('ValidateMaxLength'),
        url: $p.display('ValidationFormat'),
        number: $p.display('ValidationFormat'),
        digits: $p.display('ValidationFormat'),
        creditcard: $p.display('ValidationFormat'),
        minlength: $p.display('ValidationFormat'),
        rangelength: $p.display('ValidationFormat'),
        range: $p.display('ValidationFormat'),
        max: $p.display('ValidationFormat'),
        min: $p.display('ValidationFormat')
    });
    $('form').each(function () {
        $(this).validate({
            ignore: '[class^="se-"]', // SunEditor 関連要素は除外
            errorPlacement: function (error, element) {
                // 複数選択の分類項目に対応するためエラー表示の位置をフィールドの最下段に移動する
                element.closest('.container-normal').append(error);
            }
        });
    });
    $('[data-validate-required="1"]').each(function () {
        $(this).rules('add', { required: true });
    });
    $('[data-validate-attachments-required="1"]').each(function () {
        $(this).rules('add', { c_attachments_required: true });
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
        $(this).rules('add', {
            maxlength: $(this).attr('data-validate-maxlength'),
            messages: {
                maxlength: $p
                    .display('ValidateMaxLength')
                    .replace('{0}', $(this).attr('data-validate-maxlength'))
            }
        });
    });
    $('[data-validate-regex]').each(function () {
        $(this).rules('add', {
            c_regex: $(this).attr('data-validate-regex'),
            messages: { c_regex: $(this).attr('data-validate-regex-errormessage') }
        });
    });
};
