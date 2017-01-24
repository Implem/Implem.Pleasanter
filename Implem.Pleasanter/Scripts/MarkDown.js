$p.generalId = function ($control) {
    var controlId = $control.attr('id');
    return controlId.indexOf('.') === -1
        ? controlId
        : controlId.substring(0, controlId.indexOf('.'));
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

    function cutBrackets(str, start) {
        return str.substring(start, str.length - 1);
    }

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
        var regex_t = /(\[[^\]]+\]\(\b(https?|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+)/gi;
        var regex = /(\b(https?|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        return text
            .replace(regex_t, function ($1) {
                return '<a href="' + cutBrackets($1.match(regex)[0], 0) + '" target="_blank">' +
                    cutBrackets($1.match(/\[[^\]]+\]/i)[0], 1) +
                    '</a>';
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
                return '<a href="file://' + cutBrackets($1.match(regex)[0], 0) + '">' +
                    cutBrackets($1.match(/\[[^\]]+\]/i)[0], 1) +
                    '</a>';
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
}