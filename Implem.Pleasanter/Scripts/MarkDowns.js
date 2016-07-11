$(function () {
    $(document).on('blur', '.control-markdown', function () {
        var $viewer = $('[id="' + this.id + '.viewer"]');
        $viewer.html('' + getMarkedUp($(this).val()) + '');
        resizeEditor($(this), $viewer);
        switchControlMode($viewer, false);
    });
    $(document).on('paste', '.upload-image', function (e) {
        if (e.originalEvent.clipboardData !== undefined) {
            var items = e.originalEvent.clipboardData.items;
            for (var i = 0 ; i < items.length ; i++) {
                var item = items[i];
                if (item.type.indexOf('image') !== -1) {
                    var data = new FormData();
                    data.append('Images_Bin', item.getAsFile());
                    requestFile('/images/create', 'post', data);
                }
            }
        }
    });
});

var title_regex = /\[[^\]]+\]/i;
var url_regex_t = /(\[[^\]]+\]\(\b(https?|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+)/gi;
var url_regex = /(\b(https?|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
var unc_regex_t = /(\[[^\]]+\]\(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\))/gi;
var unc_regex = /(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;

function getMarkedUp(markdownValue, encoded) {
    var text = markdownValue;
    if (!encoded) text = getEncordedHtml(text);
    return getMarkedUpHtml(text)
        .replace(url_regex_t, function ($1) {
            return '<a href="' + cutBrackets($1.match(url_regex)[0], 0) + '" target="_blank">' +
                cutBrackets($1.match(title_regex)[0], 1) +
                '</a>';
        })
        .replace(url_regex, function ($1) {
            return $1.slice(-1) != '"'
                ? '<a href="' + $1 + '" target="_blank">' + $1 + '</a>'
                : $1;
        })
        .replace(unc_regex_t, function ($1) {
            return '<a href="file://' + cutBrackets($1.match(unc_regex)[0], 0) + '">' +
                cutBrackets($1.match(title_regex)[0], 1) +
                '</a>';
        })
        .replace(unc_regex, function ($1) {
            return $1.slice(-1) != '"'
                ? '<a href="file://' + $1 + '">' + $1 + '</a>'
                : $1;
        });
}

function cutBrackets(str, start)
{
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

function getEncordedHtml(value) {
    return $('<div/>').text(value).html();
}

function getGeneralId($self) {
    var controlId = $self.attr('id');
    return controlId.indexOf('.') === -1
        ? controlId 
        : controlId.substring(0, controlId.indexOf('.'));
}

function switchControlMode($self, toEdit) {
    var id = getGeneralId($self);
    if ($('[id="' + id + '.editor"]').length !== 0)
    {
        $('[id="' + id + '.viewer"]').toggle(!toEdit);
        $('[id="' + id + '.editor"]').toggle(!toEdit);
        $('[id="' + id + '"]').toggle(toEdit);
    }
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

function resizeEditor(editor, $viewer) {
    if ($viewer.height() <= 300) {
        editor.height($viewer.height());
    }
    else {
        editor.height(300);
    }
}