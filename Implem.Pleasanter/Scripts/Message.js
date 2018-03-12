$p.setMessage = function (target, value) {
    var $control = target !== undefined
        ? $(target)
        : $('.message-dialog:visible');
    var message = JSON.parse(value);
    var $body = $('<div/>')
        .append($('<span/>')
            .addClass('body')
            .addClass(message.Css)
            .text(message.Text));
    if ($control.length === 0) {
        if ($('#Message').hasClass('message')) {
            $body.append($('<span/>')
                .addClass('ui-icon ui-icon-close close'));
        }
        $('#Message').append($body);
    } else {
        $control.append($body);
    }
}

$p.setErrorMessage = function (error, target) {
    var data = {};
    data.Css = 'alert-error';
    data.Text = $p.display(error);
    $p.clearMessage();
    $p.setMessage(target, JSON.stringify(data));
}

$p.clearMessage = function () {
    $('[class*="message"]').html('');
}