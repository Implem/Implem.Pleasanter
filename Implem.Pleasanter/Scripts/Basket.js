$p.addBasket = function ($control, text) {
    $control.append($('<li/>')
        .addClass('ui-widget-content ui-selectee')
        .append($('<span/>').text(text))
        .append($('<span/>')
            .addClass('ui-icon ui-icon-close button-delete-address')));
    $p.setData($control);
}