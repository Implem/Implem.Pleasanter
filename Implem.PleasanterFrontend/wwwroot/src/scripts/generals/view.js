$p.addViewAccessControl = function () {
    $('#SourceViewAccessControl li.ui-selected').appendTo('#CurrentViewAccessControl');
    $p.setData($('#CurrentViewAccessControl'));
}

$p.deleteViewAccessControl = function () {
    $('#CurrentViewAccessControl li.ui-selected').appendTo('#SourceViewAccessControl');
}