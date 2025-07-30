$p.addProcessAccessControl = function () {
    $('#SourceProcessAccessControl li.ui-selected').appendTo('#CurrentProcessAccessControl');
    $p.setData($('#CurrentProcessAccessControl'));
};

$p.deleteProcessAccessControl = function () {
    $('#CurrentProcessAccessControl li.ui-selected').appendTo('#SourceProcessAccessControl');
};

$p.setProcessValidateInputDialog = function (onChange) {
    var $control = $('#ProcessValidateInputColumnName :selected');
    $('#ProcessValidateInputRequiredField').toggle($control.data('required') === 1);
    $('#ProcessValidateInputClientRegexValidationField').toggle($control.data('string') === 1);
    $('#ProcessValidateInputServerRegexValidationField').toggle($control.data('string') === 1);
    $('#ProcessValidateInputRegexValidationMessageField').toggle($control.data('string') === 1);
    $('#ProcessValidateInputMinField').toggle($control.data('num') === 1);
    $('#ProcessValidateInputMaxField').toggle($control.data('num') === 1);
    if (onChange) {
        $('#ProcessValidateInputMin').val('');
        $('#ProcessValidateInputMax').val('');
    }
};

$p.execProcess = function ($control) {
    $p.send($control);
};
