$p.openSeparateSettingsDialog = function ($control) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $('#SeparateSettingsDialog').dialog({
            modal: true,
            width: '700px',
            appendTo: '.main-form'
        });
    }
}

$p.separateSettings = function () {
    $(function () {
        $("#SeparateSettings").on('spin', '#SeparateNumber', function (event, ui) {
            setSeparateNumber($(this), ui.value);
        });
        $("#SeparateSettings").on('change', '#SeparateNumber', function () {
            setSeparateNumber($(this), $(this).val());
        });
        $("#SeparateSettings").on('spin', '[id*="SeparateWorkValue_"]', function (event, ui) {
            return setSource($(this), ui.value);
        });
        $("#SeparateSettings").on('change', '[id*="SeparateWorkValue_"]', function () {
            setSource($(this), $(this).val());
        });
    });

    function setSeparateNumber($sender, number) {
        $('#SeparateSettings .item').each(function (i) {
            if (number >= i + 1) {
                if ($(this).hasClass('hidden')) {
                    $(this).removeClass('hidden');
                }
            } else {
                if (!$(this).hasClass('hidden')) {
                    $(this).addClass('hidden');
                }
            }
        });
        setSource($sender, 0);
    }

    function setSource($sender, value) {
        var ret = true;
        var sum = getSum($sender, value);
        var $source = $('#SourceWorkValue');
        var source = parseFloat($source.attr('data-value'));
        if (source >= sum) {
            source = (Math.round((source - sum) * 100) / 100);
        } else {
            $sender.val(Math.round((source - getSum($sender, 0)) * 100) / 100);
            ret = false;
            source = 0;
        }
        $source.text(source + $('#WorkValueUnit').val());
        $p.getData($sender).SourceWorkValue = source;
        return ret;
    }

    function getSum($sender, value) {
        var controls = $('[id*="SeparateWorkValue_"]:visible');
        return $.map(controls, function (control) {
            return $sender.attr('id') !== control.id
                ? parseFloat($(control).val())
                : parseFloat(value);
        }).reduce(function (x, y) { return x + y; });
    }
}