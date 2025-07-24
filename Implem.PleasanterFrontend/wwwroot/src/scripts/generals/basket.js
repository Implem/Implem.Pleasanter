$p.addBasket = function ($control, text, value) {
    var $li = $('<li/>');
    if (value !== undefined){
        $li.attr('data-value', value);
    }
    $li
        .addClass('ui-widget-content ui-selectee')
        .append($('<span/>').text(text))
        .append($('<span/>')
            .addClass('ui-icon ui-icon-close delete'));
    $control.append($li);
    $p.setData($control);
}