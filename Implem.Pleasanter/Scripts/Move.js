$p.moveTargets = function ($control) {
    $p.send($control);
    $p.openDialog($control)
}

$p.move = function ($control) {
    $p.getData().MoveTargets = $('#MoveTargets').val();
    $('.ui-dialog-content').dialog('close');
    $p.send($control);
}