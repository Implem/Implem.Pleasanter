$p.setGroup = function ($control) {
    {
        let modifiedMembers = JSON.parse($('#ModifiedGroupMembers').val());
        $('#CurrentMembers')
            .find('.ui-selected')
            .each(function () {
                let $this = $(this);
                let value = $this.attr('data-value');
                let data = value.split(',');
                let type = $control.attr('id');
                let isManagerButton = type === 'Manager';
                let isManager = data[2].toLowerCase() === 'true';
                //アイテムの状態と変更後の値が一致している場合は何もしない
                if (isManager === isManagerButton) {
                    return;
                }
                //UI要素の変更
                let nextValue = data[0] + ',' + data[1] + ',' + isManagerButton;
                $this.attr('data-value', nextValue);
                $this.text($this.text().replace(/\(.*\)/, ''));
                $this.text($this.text() + (isManagerButton ? '(' + $p.display(type) + ')' : ''));
                //変更リストに登録
                let key = value.substr(0, value.lastIndexOf(',') + 1);
                if (modifiedMembers.findIndex(item => item.startsWith(key)) !== -1) {
                    //変更済みリストに存在する場合リストから削除
                    modifiedMembers = modifiedMembers.filter(item => !item.startsWith(key));
                } else {
                    //変更したアイテムをリストに追加
                    modifiedMembers.push(nextValue);
                }
            });
        $p.set($('#ModifiedGroupMembers'), JSON.stringify(modifiedMembers));
    }
    {
        let modifiedChildren = JSON.parse($('#ModifiedGroupChildren').val());
        $('#CurrentChildren')
            .find('.ui-selected')
            .each(function () {
                let $this = $(this);
                let value = $this.attr('data-value');
                let data = value.split(',');
                let type = $control.attr('id');
                let isManagerButton = type === 'Manager'; // TODO 不要なので削除
                let isManager = data[2].toLowerCase() === 'true';
                //アイテムの状態と変更後の値が一致している場合は何もしない
                if (isManager === isManagerButton) {
                    return;
                }
                //UI要素の変更
                let nextValue = data[0] + ',' + data[1] + ',' + isManagerButton;
                $this.attr('data-value', nextValue);
                $this.text($this.text().replace(/\(.*\)/, ''));
                $this.text($this.text() + (isManagerButton ? '(' + $p.display(type) + ')' : ''));
                //変更リストに登録
                let key = value.substr(0, value.lastIndexOf(',') + 1);
                if (modifiedChildren.findIndex(item => item.startsWith(key)) !== -1) {
                    //変更済みリストに存在する場合リストから削除
                    modifiedChildren = modifiedChildren.filter(item => !item.startsWith(key));
                } else {
                    //変更したアイテムをリストに追加
                    modifiedChildren.push(nextValue);
                }
            });
        $p.set($('#ModifiedGroupChildren'), JSON.stringify(modifiedChildren));
    }
};

$p.moveGroupSelected = function ($selected, additionSelector, deletionSelector, deletion2Selector) {
    let additionArray = JSON.parse($(additionSelector).val());
    let deletionArray = JSON.parse($(deletionSelector).val());
    let deletion2Array =
        deletion2Selector === undefined ? null : JSON.parse($(deletion2Selector).val());

    $selected.each(function (index, element) {
        let value = element.getAttribute('data-value');
        let key = value.substr(0, value.lastIndexOf(',') + 1);
        if (deletionArray.findIndex(item => item.startsWith(key)) !== -1) {
            //deletionArray: 対象の要素が存在していたら削除
            deletionArray = deletionArray.filter(item => !item.startsWith(key));
        } else {
            //additionArray: 対象の要素が存在していない場合追加
            additionArray.push(value);
        }
        if (deletion2Array !== null) {
            //deletion2Array: 対象の要素が存在していたら削除
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
};

$p.addToCurrentMembers = function ($control) {
    let $selected = $control.closest('.container-selectable').find('.ui-selected');
    $selected.prependTo($('#CurrentMembers'));
    $p.moveGroupSelected($selected, '#AddedGroupMembers', '#DeletedGroupMembers');
};

$p.deleteFromCurrentMembers = function ($control) {
    let $selected = $control.closest('.container-selectable').find('.ui-selected');
    $selected.remove();
    $p.moveGroupSelected(
        $selected,
        '#DeletedGroupMembers',
        '#AddedGroupMembers',
        '#ModifiedGroupMembers'
    );
    $p.send($control);
};

$p.addToCurrentChildren = function ($control) {
    let $selected = $control.closest('.container-selectable').find('.ui-selected');
    $selected.prependTo($('#CurrentChildren'));
    $p.moveGroupSelected($selected, '#AddedGroupChildren', '#DeletedGroupChildren');
};

$p.deleteFromCurrentChildren = function ($control) {
    let $selected = $control.closest('.container-selectable').find('.ui-selected');
    $selected.remove();
    $p.moveGroupSelected(
        $selected,
        '#DeletedGroupChildren',
        '#AddedGroupChildren',
        '#ModifiedGroupChildren'
    );
    $p.send($control);
};
