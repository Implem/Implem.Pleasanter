$p.setMessage = function (target, value) {
    var message = JSON.parse(value);
    var $control = target !== undefined
        ? target.split('_')[0] === 'row'
            ? $('[data-id="' + target.split('_')[1] + '"]')
            : $(target)
        : $('.message-dialog:visible');
    if ($control.prop('tagName') === 'TR') {
        var $body = $($('<tr/>')
            .addClass("message-row")
            .append($('<td/>')
                .attr('colspan', $control.find('td').length)
                .append($('<span/>')
                    .addClass('body')
                    .addClass(message.Css)
                    .text(message.Text))));
        $control.after($body);
        $('html,body').animate({
            scrollTop: $control.offset().top - 50
        });
    }
    else {
        var $body = $($('<div/>')
            .append($('<span/>')
                .addClass('body')
                .addClass(message.Css)
                .text(message.Text)));
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