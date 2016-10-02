$p.moveTargets = function ($control) {
    $p.send($control);
    $p.openDialog($control, '.main-form')
}

$p.move = function ($control) {
    $('.ui-dialog-content').dialog('close');
    $p.send($control);
}