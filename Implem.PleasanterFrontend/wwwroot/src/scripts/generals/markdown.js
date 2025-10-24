$p.showMarkDownViewer = function ($control) {
    var id = $control.attr('id');
    var enableLightBox = Boolean($control.data('enablelightbox'));
    var $viewer = $('[id="' + id + '.viewer"]');
    if ($viewer.length === 1) {
        var markup = $p.markup($control.val(), enableLightBox);
        $viewer.html($p.setInputGuide(id, $control.val(), markup));
        $p.resizeEditor($control, $viewer);
        $p.toggleEditor($control, false);
        $p.setTargetBlank();
    }
};

$p.editMarkdown = function ($control) {
    if ($control.is(':visible')) {
        if ($control.hasClass('manual')) {
            $p.showMarkDownViewer($control);
        } else {
            $p.toggleEditor($control, false);
        }
    } else {
        $p.toggleEditor($control, true);
        $($control).focus();
    }
};

$p.toggleEditor = function ($control, edit) {
    var id = $control.attr('id');
    if ($('[id="' + id + '.editor"]').length !== 0) {
        if (edit) {
            $p.resizeEditor($('[id="' + id + '"]'), $('[id="' + id + '.viewer"]'));
        }
        $('[id="' + id + '.viewer"]').toggle(!edit);
        $('[id="' + id + '"]').toggle(edit);
    }
};

$p.resizeEditor = function ($control, $viewer) {
    if ($viewer.height() <= 300) {
        $control.height($viewer.height());
    } else {
        $control.height(300);
    }
};

$p.markup = function (markdownValue, enableLightBox, encoded, dataId) {
    var text = markdownValue;
    if (!encoded) text = getEncordedHtml(text);
    text = replaceUnc(text);
    return text.indexOf('[md]') === 0
        ? '<div class="md">' + marked(text.substring(4)) + '</div>'
        : markedUp(replaceUrl(text, enableLightBox, dataId));

    function markedUp(text) {
        var $html = $('<pre/>');
        text.split(/\r\n|\r|\n/).forEach(function (line) {
            if (line !== '') {
                $html.append($('<span/>').append(line));
            }
            $html.append($('<br/>'));
        });
        return $html[0].outerHTML;
    }

    function replaceUrl(text, enableLightBox, dataId) {
        // regex_linkはmarkdown.jsと同じ正規表現を使用
        // https://github.com/markedjs/marked/blob/dbb32d473b675913452ec75c05898f8d75b432c1/src/rules.ts#L341
        function edit(regex, opt = '') {
            const other_caret = /(^|[^\[])\^/g;
            let source = typeof regex === 'string' ? regex : regex.source;
            const obj = {
                replace: (name, val) => {
                    let valSource = typeof val === 'string' ? val : val.source;
                    valSource = valSource.replace(other_caret, '$1');
                    source = source.replace(name, valSource);
                    return obj;
                },
                getRegex: () => {
                    return new RegExp(source, opt);
                },
            };
            return obj;
        }
        const _inlineLabel = /(?:\[(?:\\[\s\S]|[^\[\]\\])*\]|\\[\s\S]|`[^`]*`|[^\[\]\\`])*?/;
        const regex_link = edit(/!?\[(label)\]\(\s*(href)(?:(?:[ \t]*(?:\n[ \t]*)?)(title))?\s*\)/, "g")
            .replace('label', _inlineLabel)
            // href の部分で閉じ丸括弧 ')' と空白を含まないようにする（隣接するマークダウンが結合されるのを防ぐ）
            .replace('href', /<(?:\\.|[^\n<>\\])+>|[^ \t\n\x00-\x1f)]+/)
            .replace('title', /"(?:\\"?|[^"\\])*"|'(?:\\'?|[^'\\])*'|\((?:\\\)?|[^)\\])*\)/)
            .getRegex();
        var regex = /(\b(https?|notes|ftp):\/\/((?!\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        var anchorTargetBlank = $('#AnchorTargetBlank').length === 1;
        return text
            .replace(regex_link, function ($1, $2, $3, $4) {
                return $1.startsWith("!")
                    ? getEncordedImgTag(encodeURI($3), $2, enableLightBox, dataId)
                    : getEncordedATag(encodeURI($3), $2, anchorTargetBlank);
            })
            .replace(regex, function ($1) {
                return $1.slice(-1) != '"'
                    ? getEncordedATag(decode($1), decode($1), anchorTargetBlank)
                    : $1;
            });
    }

    function replaceUnc(text) {
        var regex_t =
            /(\[[^\]]+\]\(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\))/gi;
        var regex = /(\B\\\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+\\((?!:|\*|"|<|>|\||&gt;|&lt;).)+"?)/gi;
        return text
            .replace(regex_t, function ($1) {
                return getEncordedATag('file://' + address($1), title($1));
            })
            .replace(regex, function ($1) {
                return $1.slice(-1) != '"'
                    ? getEncordedATag('file://' + decode($1), decode($1))
                    : $1;
            });
    }

    function getEncordedHtml(value) {
        return $('<div/>').text(value).html();
    }

    function getEncordedATag(href, text, anchorTargetBlank) {
        let $tag = $('<a/>').attr('href', href).text(text);
        if (anchorTargetBlank) $tag.attr('target', '_blank');
        return $tag.prop('outerHTML');
    }

    function getEncordedImgTag(url, text, enableLightBox, dataId) {
        let $tag = $('<a/>')
            .attr('href', url)
            .attr('target', '_blank')
            .append(
                $('<img/>')
                    .attr('src', url + '?thumbnail=1')
                    .attr('alt', text)
            );
        if (enableLightBox) {
            if (dataId) {
                $tag.attr('data-lightbox', text + '_' + dataId);
            } else {
                $tag.attr('data-lightbox', text);
            }
        }
        return $tag.prop('outerHTML');
    }

    function address($1) {
        var m = $1.match(/\]\(.+?\)/gi)[0];
        return decode(m.substring(2, m.length - 1));
    }

    function title($1) {
        var m = $1.match(/\[[^\]]+\]\(/i)[0];
        return m.substring(1, m.length - 2);
    }

    function decode($1) {
        return $1.replace(/&amp;/g, '&');
    }
};

$p.setInputGuide = function (id, text, markup) {
    if (text.length === 0) {
        $('[id="' + id + '.viewer"]').css('color', 'darkgray');
        var placeholder = $('[id="' + id + '"]').attr('placeholder');
        return !placeholder ? '' : placeholder;
    } else {
        $('[id="' + id + '.viewer"]').css('color', 'black');
        return markup;
    }
};

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
};

$p.selectImage = function (controlId) {
    $('[id="' + controlId + '.upload-image-file"]').click();
};

$p.uploadImage = function (controlId, file) {
    var $tr = $('[id="' + controlId + '"]').closest('tr');
    var $editorInDialogRecordId = $('#EditorInDialogRecordId');
    var url;
    if ($tr.length) {
        url = $('#BaseUrl').val() + $tr.data('id') + '/binaries/uploadimage';
    } else if ($('#EditorDialog').is(':visible') && $editorInDialogRecordId.length) {
        url = $('#BaseUrl').val() + $editorInDialogRecordId.val() + '/binaries/uploadimage';
    } else {
        url = $('.main-form').attr('action').replace('_action_', 'binaries/uploadimage');
    }
    var data = new FormData();
    data.append('ControlId', controlId);
    data.append('file', file);
    $p.multiUpload(url, data);
};

$p.setTargetBlank = function () {
    if ($('#AnchorTargetBlank').length === 1) {
        var aTags = $('.md a');
        aTags.each(function () {
            $(this).attr('target', '_blank');
        });
    }
};
