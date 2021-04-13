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

$p.apiBulkDelete = function (args) {
    return $p.apiBulkDeleteExec($p.apiUrl(args.id, 'bulkdelete'), args);
}

$p.apiBulkDeleteExec = function (url, args) {
    if (args.async !== undefined) {
        $.ajaxSetup({ async: args.async });
    }
    if (args.data.GridCheck) {
        args.data.All = $p.data.MainForm.GridCheckAll;
        args.data.Selected = args.data.All
            ? $p.data.MainForm.GridUnCheckedItems === ''
                ? []
                : $p.data.MainForm.GridUnCheckedItems.split(',')
            : $p.data.MainForm.GridCheckedItems.split(',');
    }
    var data = args.data !== undefined
        ? args.data
        : {};
    if ($('#Token').length === 1) {
        data.Token = $('#Token').val();
    }
    return $.ajax({
        type: 'post',
        url: url,
        data: JSON.stringify(data),
        dataType: 'json'
    })
        .done(args.done)
        .fail(args.fail)
        .always(args.always);
}

$p.apiExec = function (url, args) {
    if (args === undefined) {
        args = {};
    }
    if (args.async !== undefined) {
        $.ajaxSetup({ async: args.async });
    }
    var data = args.data !== undefined
        ? args.data
        : {};
    if ($('#Token').length === 1) {
        data.Token = $('#Token').val();
    }
    var ajaxSetings = {
        type: 'post',
        url: url,
        cache: false,
        data: JSON.stringify(data),
        dataType: 'json'
    };
    if (/(?:^|\/)api\//.test(url)) {
        ajaxSetings.contentType = 'application/json';
    }
    return $.ajax(ajaxSetings)
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

$p.apiDeptsUrl = function (action) {
    return $('#ApplicationPath').val() + 'api/depts/' + action;
}

$p.apiDeptsGet = function (args) {
    return $p.apiExec($p.apiDeptsUrl('get'), args);
}

$p.apiGroupsUrl = function (action) {
    return $('#ApplicationPath').val() + 'api/groups/' + action;
}

$p.apiGroupsGet = function (args) {
    return $p.apiExec($p.apiGroupsUrl('get'), args);
}

$p.apiSendMailUrl = function (id) {
    return $('#ApplicationPath').val() + 'api/items/' + id + '/OutgoingMails/Send';
}

$p.apiSendMail = function (args) {
    return $p.apiExec($p.apiSendMailUrl(args.id), args);
}