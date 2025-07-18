$p.copyDirectUrlToClipboard = function (url) {
    var div = $('<div>');
    var pre = $('<pre>');
    div.append(pre);
    pre.text(url);
    div.css({ 'position': 'fixed', 'left': '-100%', 'top': '-100%' });
    $(document.body).append(div);
    document.getSelection().selectAllChildren(div.get(0));
    document.execCommand('copy');
    div.remove();
    alert($p.display('DirectUrlCopied'));
}