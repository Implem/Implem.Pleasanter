$p.apiUrl = function (id, action) {
    return $('#ApplicationPath').val() + 'api_items/' + id + '/' + action;
}

$p.apiGet = function (args) {
    $p.apiExec($p.apiUrl(args.id, 'get'), args);
}

$p.apiCreate = function (args) {
    $p.apiExec($p.apiUrl(args.id, 'create'), args);
}

$p.apiUpdate = function (args) {
    $p.apiExec($p.apiUrl(args.id, 'update'), args);
}

$p.apiDelete = function (args) {
    $p.apiExec($p.apiUrl(args.id, 'delete'), args);
}

$p.apiExec = function (url, args) {
    $.post(url, JSON.stringify(args.data), "json")
        .done(args.done)
        .fail(args.fail)
        .always(args.always);
}