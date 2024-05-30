$p.apiUrl = function (id, action, controller) {
    if (controller) {
        if (id) {
            return $('#ApplicationPath').val() + 'api/' + controller + '/' + id + '/' + action;
        } else {
            return $('#ApplicationPath').val() + 'api/' + controller + '/' + action;
        }
    } else {
        return $('#ApplicationPath').val() + 'api/items/' + id + '/' + action;
    }
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

$p.apiUpsert = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'upsert'), args);
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

$p.apiUsersGet = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'get', 'users'), args);
}

$p.apiUsersCreate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'create', 'users'), args);
}

$p.apiUsersUpdate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'update', 'users'), args);
}

$p.apiUsersDelete = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'delete', 'users'), args);
}

$p.apiDeptsGet = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'get', 'depts'), args);
}

$p.apiGroupsGet = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'get', 'groups'), args);
}

$p.apiGroupsCreate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'create', 'groups'), args);
}

$p.apiGroupsUpdate = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'update', 'groups'), args);
}

$p.apiGroupsDelete = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'delete', 'groups'), args);
}

$p.apiSendMailUrl = function (id) {
    return $('#ApplicationPath').val() + 'api/items/' + id + '/OutgoingMails/Send';
}

$p.apiSendMail = function (args) {
    return $p.apiExec($p.apiSendMailUrl(args.id), args);
}

$p.apiCopySitePackage = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'copysitepackage'), args);
}

$p.apiGetSite = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'getsite'), args);
}

$p.apiCreateSite = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'createsite'), args);
}

$p.apiUpdateSite = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'updatesite'), args);
}

$p.apiDeleteSite = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'deletesite'), args);
}

$p.apiGetClosestSiteId = function (args) {
    return $p.apiExec($p.apiUrl(args.id, 'getclosestsiteid'), args);
}
