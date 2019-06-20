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
    var result = exec(event);
    if (args.$control) {
        result = exec(event + '_' + args.$control.attr('id')) && result;
        result = exec(event + '_' + args.$control.attr('data-action')) && result;
    }
    return result;
    function exec(name) {
        if ($p.events[name] !== undefined) {
            return ($p.events[name](args) === false) ? false : true;
        }
    }
}

$p.before_validate = function (args) {
    return $p.execEvents('before_validate', args);
}

$p.after_validate = function (args) {
    return $p.execEvents('after_validate', args);
}

$p.before_send = function (args) {
    return $p.execEvents('before_send', args);
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