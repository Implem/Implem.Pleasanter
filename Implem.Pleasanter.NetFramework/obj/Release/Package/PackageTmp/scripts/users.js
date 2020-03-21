$p.returnOriginalUser = function (url) {
    $p.ajax(url, 'post', null, $('#SwitchUserInfo a'));
}