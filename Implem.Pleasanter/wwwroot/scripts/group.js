$p.setGroup = function ($control) {
    $('#CurrentMembers').find('.ui-selected').each(function () {
        var $this = $(this);
        var data = $this.attr('data-value').split(',');
        var type = $control.attr('id');
        $this.attr('data-value', data[0] + ',' + data[1] + ',' + (type === 'Manager'));
        $this.text($this.text().replace(/\(.*\)/, ''));
        $this.text($this.text() + (type === 'GeneralUser'
            ? ''
            : '(' + $p.display(type) + ')'));
    });
    $p.setData($('#CurrentMembers'));
}

$p.moveGroupMembers = function ($control, additionArray, deletionArray) {
    let $selected = $control
        .closest('.container-selectable')
        .find('.ui-selected');
    $selected.each(function (index, element) {
        let value = element
            .getAttribute("data-value");
        let key = value.substr(0, value.lastIndexOf(','));
        if (deletionArray.findIndex(item => item.startsWith(key)) !== -1) {
            deletionArray = deletionArray.filter(item => !item.startsWith(key));
        } else {
            additionArray.push(value);
        }
    });
}

$p.addToCurrentMembers = function ($control) {
    $p.moveGroupMembers(
        $control,
        JSON.parse($("#AddedGroupMembers").val()),
        JSON.parse($("#DeletedGroupMembers").val()));
    $p.setData($("#AddedGroupMembers"));
    $p.setData($("#DeletedGroupMembers"));
    $p.send($control);
}

$p.deleteFromCurrentMembers = function ($control) {
    $p.moveGroupMembers(
        $control,
        JSON.parse($("#DeletedGroupMembers").val()),
        JSON.parse($("#AddedGroupMembers").val()));
    $p.setData($("#AddedGroupMembers"));
    $p.setData($("#DeletedGroupMembers"));
    $p.send($control);
}