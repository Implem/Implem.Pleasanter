$p.addStatusControlAccessControl = function () {
    $('#SourceStatusControlAccessControl li.ui-selected').appendTo(
        '#CurrentStatusControlAccessControl'
    );
    $p.setData($('#CurrentStatusControlAccessControl'));
};

$p.deleteStatusControlAccessControl = function () {
    $('#CurrentStatusControlAccessControl li.ui-selected').appendTo(
        '#SourceStatusControlAccessControl'
    );
};
