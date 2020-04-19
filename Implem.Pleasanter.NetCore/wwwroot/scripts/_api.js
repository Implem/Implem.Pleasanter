$p.apiUrl = function (id, action) {
    return $('#ApplicationPath').val() + 'api/items/' + id + '/' + action;
}

$p.apiGet = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'get'), args);
}

$p.apiCreate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'create'), args);
}

$p.apiUpdate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'update'), args);
}

$p.apiDelete = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'delete'), args);
}

$p.apiExec = function (url, args) {
    return $.ajax({
        type: 'Post',
        url: url,
        data: args.data !== undefined
            ? JSON.stringify(args.data)
            : "",
        dataType: 'json',
        async: args.async !== undefined
            ? args.async
            : true
    })
        .done(args.done)
        .fail(args.fail)
        .always(args.always);
}

$p.apiUsersUrl = function (action, id) {
    switch (action) {
        case 'get':
        case 'create':
            return $('#ApplicationPath').val() + 'api/users/' + action;
            break;
        case 'update':
        case 'delete':
            return $('#ApplicationPath').val() + 'api/users/' + id + '/' + action;
            break;
    }
}

$p.apiUsersGet = function (args) {
    return $p.apiExec($p.apiUsersUrl('get'), args);
}

$p.apiUsersCreate = function (args) {
    return $p.apiExec($p.apiUsersUrl('create'), args);
}

$p.apiUsersUpdate = function (args) {
    return $p.apiExec($p.apiUsersUrl('update', args.id), args);
}

$p.apiUsersDelete = function (args) {
    return $p.apiExec($p.apiUsersUrl('delete', args.id), args);
}
