$p.setMessage = function (target, value) {
    var $control = target !== undefined
        ? $(target)
        : $('.message-dialog:visible');
    var message = JSON.parse(value);
    ($control.length === 0
        ? $('#Message')
        : $control)
            .append($('<div/>')
                .append($('<span/>')
                    .addClass('body')
                    .addClass(message.Css)
                    .text(message.Text))
                .append($('<span/>')
                    .addClass('ui-icon ui-icon-close close')));
}

$p.setErrorMessage = function (error) {
    var data = {};
    data.Css = 'alert-error';
    data.Text = $p.display(error);
    $p.setMessage('#Message', JSON.stringify(data));
}

$p.clearMessage = function () {
    $('[class*="message"]').html('');
}