$p.setGroup = function ($control) {
    let modifiedMembers = JSON.parse($("#ModifiedGroupMembers").val());
    $('#CurrentMembers').find('.ui-selected').each(function () {
        let $this = $(this);
        let value = $this.attr('data-value');
        let data = value.split(',');
        let type = $control.attr('id');
        let isManagerButton = type === 'Manager';
        let isManager = data[2].toLowerCase() === 'true';
        if (isManager === isManagerButton) {
            return;
        }
        let nextValue = data[0] + ',' + data[1] + ',' + isManagerButton;
        $this.attr('data-value', nextValue);
        $this.text($this.text().replace(/\(.*\)/, ''));
        $this.text($this.text() + (isManagerButton
            ? '(' + $p.display(type) + ')'
            : ''));
        let key = value.substr(0, value.lastIndexOf(',') + 1);
        if (modifiedMembers.findIndex(item => item.startsWith(key)) !== -1) {
            modifiedMembers = modifiedMembers.filter(item => !item.startsWith(key));
        } else {
            modifiedMembers.push(nextValue);
        }
    });
    $p.set($("#ModifiedGroupMembers"), JSON.stringify(modifiedMembers));
}

$p.moveGroupMembers = function ($selected, additionSelector, deletionSelector, deletion2Selector) {
    let additionArray = JSON.parse($(additionSelector).val());
    let deletionArray = JSON.parse($(deletionSelector).val());
    let deletion2Array = (deletion2Selector === undefined)
        ? null
        : JSON.parse($(deletion2Selector).val());

    $selected.each(function (index, element) {
        let value = element
            .getAttribute("data-value");
        let key = value.substr(0, value.lastIndexOf(',') + 1);
        if (deletionArray.findIndex(item => item.startsWith(key)) !== -1) {
            deletionArray = deletionArray.filter(item => !item.startsWith(key));
        } else {
            additionArray.push(value);
        }
        if (deletion2Array !== null) {
            if (deletion2Array.findIndex(item => item.startsWith(key)) !== -1) {
                deletion2Array = deletion2Array.filter(item => !item.startsWith(key));
            }
        }
    });
    $p.set($(additionSelector), JSON.stringify(additionArray));
    $p.set($(deletionSelector), JSON.stringify(deletionArray));
    if (deletion2Array !== null) {
        $p.set($(deletion2Selector), JSON.stringify(deletion2Array));
    }
}

$p.addToCurrentMembers = function ($control) {
    let $selected = $control
        .closest('.container-selectable')
        .find('.ui-selected');
    $selected.prependTo($('#CurrentMembers'));
    $p.moveGroupMembers(
        $selected,
        '#AddedGroupMembers',
        '#DeletedGroupMembers');
}

$p.deleteFromCurrentMembers = function ($control) {
    let $selected = $control
        .closest('.container-selectable')
        .find('.ui-selected');
    $selected.remove();
    $p.moveGroupMembers(
        $selected,
        '#DeletedGroupMembers',
        '#AddedGroupMembers',
        '#ModifiedGroupMembers');
    $p.send($control);
}