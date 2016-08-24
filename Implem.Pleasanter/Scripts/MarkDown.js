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
        ? marked(text.substring(4))
        : replaceUrl(getMarkedUpHtml(text));

    function cutBrackets(str, start) {
        return str.substring(start, str.length - 1);
    }

    function getMarkedUpHtml(markdownValue) {
        var paragraphsNumber = [0, 0, 0, 0, 0];
        var paragraphLevel = 0;
        var beforeIndex = 0;
        var header = false;
        var inSection = false;
        var $elements = $('<pre/>')
        markdownValue.split(/\r\n|\r|\n/).forEach(function (e) {
            paragraphLevel = /^\++|^\*+|^\+*/.exec(e)[0].length;
            var index = paragraphLevel - 1;
            var tag = 'h' + (paragraphLevel + 1);
            if (0 < paragraphLevel && paragraphLevel < 6) {
                var paragraphType = e.substring(0, 1);
                if (index < beforeIndex) {
                    paragraphsNumber[beforeIndex] = 0;
                }
                if (paragraphType === '+') {
                    paragraphsNumber[index]++;
                }
                $elements
                    .append($('<section/>').addClass(tag)
                        .append($('<' + tag + '/>').text(
                            joinParagraphNumber(paragraphType, paragraphsNumber, index) +
                            e.substring(paragraphLevel)))
                        .append($('<p/>').addClass('paragraph')));
                beforeIndex = index;
                header = true;
                inSection = true;
            }
            else {
                if (!header && $elements.length > 0) {
                    $elements.find('.paragraph').filter(':last').append($('<br/>'));
                }
                if ($elements.find('.paragraph').length !== 0) {
                    $elements.find('.paragraph').filter(':last').append(e);
                }
                else {
                    $elements.append(e).append($('<br/>'));
                }
                header = false;
            }
        });
        return $elements[0].outerHTML;
    }

    var title_regex = /\[[^\]]+\]/i;

    function replaceUrl(text) {
        var regex_t = /(\[[^\]]+\]\(\b(https?|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+)/gi;
        var regex = /(\b(https?|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        return text
            .replace(regex_t, function ($1) {
                return '<a href="' + cutBrackets($1.match(regex)[0], 0) + '" target="_blank">' +
                    cutBrackets($1.match(title_regex)[0], 1) +
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
                    cutBrackets($1.match(title_regex)[0], 1) +
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

    function joinParagraphNumber(paragraphType, paragraphsNumber, paragraphLevel) {
        if (paragraphType === '+') {
            var sum = [];
            for (var i = 0; i <= paragraphLevel; ++i) {
                sum[i] = paragraphsNumber[i];
            }
            return sum.join('.') + '. ';
        }
        else {
            return '';
        }
    }
}