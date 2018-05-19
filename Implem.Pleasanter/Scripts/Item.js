$p.get = function ($control, ajax) {
    if (!$p.confirmReload()) return false;
    if ($p.outsideDialog($control)) return false;
    switch ($control.attr('id')) {
        case 'Reload': move(0, ajax); break;
        case 'Previous': move(-1, ajax); break;
        case 'Next': move(1, ajax); break;
    }

    function move(additional, ajax) {
        var array = $p.switchTargets();
        var index = $p.currentIndex(array);
        var target = index + additional;
        if (index !== -1 && target >= 0 && target < array.length) {
            var url = $('#BaseUrl').val() + array[target];
            if (ajax) {
                $p.ajax(url, 'post', null, null, false);
                if (additional !== 0) history.pushState(null, null, url);
            } else {
                location.href = url;
            }
        }
    }
}

$p.new = function ($control) {
    location.href = $('#BaseUrl').val() +
        $control.attr('data-to-site-id') + '/new' +
        '?FromSiteId=' + $control.attr('data-from-site-id') +
        '&LinkId=' + $control.attr('data-id');
}

$p.copy = function ($control) {
    var error = $p.syncSend($control);
    if (error === 0) {
        history.pushState(null, null, $('#BaseUrl').val() + $('#Id').val());
    }
}

$p.search = function (searchWord, redirect, offset) {
    offset = offset !== undefined ? offset : 0;
    if ($p.searchWord !== searchWord + offset) {
        var url = $('#ApplicationPath').val() +
            'items/search?text=' + escape(searchWord)
        if (offset > 0) url += '&offset=' + offset;
        if (redirect) {
            location.href = url;
        } else if (offset !== '-1') {
            $p.ajax(url, 'get', false);
            if (offset === 0) history.pushState(null, null, url);
        }
        $p.searchWord = searchWord + offset;
    }
}