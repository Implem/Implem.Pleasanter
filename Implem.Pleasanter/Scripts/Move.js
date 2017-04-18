$p.moveTargets = function ($control) {
    $p.send($control);
    $p.openDialog($control, '.main-form')
}

$p.move = function ($control) {
    var error = $p.syncSend($control);
    if (error === 0) {
        $('.ui-dialog-content').dialog('close');
    }
}