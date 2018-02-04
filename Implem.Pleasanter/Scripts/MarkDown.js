$p.generalId = function ($control) {
    var controlId = $control.attr('id');
    return controlId.indexOf('.') === -1
        ? controlId
        : controlId.substring(0, controlId.indexOf('.'));
}

$p.showMarkDownViewer = function ($control) {
    var $viewer = $('[id="' + $control.attr('id') + '.viewer"]');
    if ($viewer.length === 1) {
        $viewer.html($p.markup($control.val()));
        $p.resizeEditor($control, $viewer);
        $p.toggleEditor($viewer, false);
        $p.formChanged = true;
    }
}

$p.editMarkdown = function ($control) {
    $p.toggleEditor($control, true);
    $('[id=\'' + $p.generalId($control) + '\']').focus();
}

$p.toggleEditor = function ($control, edit) {
    var id = $p.generalId($control);
    if ($('[id="' + id + '.editor"]').length !== 0) {
        if (edit) {
            $p.resizeEditor($('[id="' + id + '"]'), $('[id="' + id + '.viewer"]'));
        }
        $('[id="' + id + '.viewer"]').toggle(!edit);
        $('[id="' + id + '.editor"]').toggle(!edit);
        $('[id="' + id + '"]').toggle(edit);
    }
}

$p.resizeEditor = function ($control, $viewer) {
    if ($viewer.height() <= 300) {
        $control.height($viewer.height());
    }
    else {
        $control.height(300);
    }
}

$p.markup = function (markdownValue, encoded) {
    var text = markdownValue;
    if (!encoded) text = getEncordedHtml(text);
    text = replaceUnc(text);
    return text.indexOf('[md]') === 0
        ? '<div class="md">' + marked(text.substring(4)) + '</div>'
        : replaceUrl(markedUp(text));

    function markedUp(text) {
        var $html = $('<pre/>')
        text.split(/\r\n|\r|\n/).forEach(function (line) {
            if (line !== '') {
                $html.append($('<span/>').append(line));
            }
            $html.append($('<br/>'));
        });
        return $html[0].outerHTML;
    }

    function replaceUrl(text) {
        var regex_i = /(!\[[^\]]+\]\(.+?\))/gi;
        var regex_t = /(\[[^\]]+\]\(.+?\))/gi;
        var regex = /(\b(https?|notes|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        return text
            .replace(regex_i, function ($1) {
                return '<a href="' + address($1) + '" target="_blank">' +
                    '<img src="' + address($1) + '" alt="' + title($1) + '" /></a>';
            })
            .replace(regex_t, function ($1) {
                return '<a href="' + address($1) + '" target="_blank">' + title($1) + '</a>';
            })
            .replace(regex, function ($1) {
                return $1.slice(-1) != '"'
                    ? '<a href="' + $1 + '" target="_blank">' + $1 + '</a>'
                    : $1;
            });
    }

    function replaceUnc(text) {
        var regex_t = /(\[[^\]]+\]\(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\))/gi;
        var regex = /(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        return text
            .replace(regex_t, function ($1) {
                return '<a href="file://' + address($1) + '">' + title($1) + '</a>';
            })
            .replace(regex, function ($1) {
                return $1.slice(-1) != '"'
                    ? '<a href="file://' + $1 + '">' + $1 + '</a>'
                    : $1;
            });
    }

    function getEncordedHtml(value) {
        return $('<div/>').text(value).html();
    }

    function address($1) {
        var m = $1.match(/\(.+?\)/gi)[0];
        return m.substring(1, m.length - 1);
    }

    function title($1) {
        var m = $1.match(/\[[^\]]+\]/i)[0];
        return m.substring(1, m.length - 1);
    }
}

$p.insertText = function ($control, value) {
    var body = $control.get(0);
    body.focus();
    var start = body.value;
    var caret = body.selectionStart;
    var next = caret + value.length;
    body.value = start.substr(0, caret) + value + start.substr(caret);
    body.setSelectionRange(next, next);
    $p.setData($control);
    if (!$control.is(':visible')) {
        $p.showMarkDownViewer($control);
    }
}

$p.selectImage = function (controlId) {
    $('[id="' + controlId + '.upload-image-file"]').click();
}

$p.uploadImage = function (controlId, file) {
    var url = $('.main-form')
        .attr('action')
        .replace('_action_', 'binaries/uploadimage');
    var data = new FormData();
    data.append('ControlId', controlId);
    data.append('file', file);
    $p.multiUpload(url, data);
}