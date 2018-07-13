$p.eventArgs = function (url, methodType, data, $control, async, ret, json) {
    var args = {};
    args.url = url;
    args.methodType = methodType;
    args.data = data;
    args.$control = $control;
    args.async = async;
    args.ret = ret;
    args.json = json;
    return args;
}

$p.execEvents = function (event, args) {
    exec(event);
    if (args.$control) {
        exec(event + '_' + args.$control.attr('id'));
        exec(event + '_' + args.$control.attr('data-action'));
    }
    function exec(name) {
        if ($p.events[name] !== undefined) $p.events[name](args);
    }
}

$p.before_validate = function (args) {
    $p.execEvents('before_validate', args);
}

$p.after_validate = function (args) {
    $p.execEvents('after_validate', args);
}

$p.before_send = function (args) {
    $p.execEvents('before_send', args);
}

$p.after_send = function (args) {
    $p.execEvents('after_send', args);
}

$p.before_set = function (args) {
    $p.execEvents('before_set', args);
}

$p.after_set = function (args) {
    $p.execEvents('after_set', args);
}