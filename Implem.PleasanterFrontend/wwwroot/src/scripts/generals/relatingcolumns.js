$(document).ready(function () {
    var methodType = $('#MethodType').val();
    $p.initRelatingColumnWhenViewChanged = function () {
        initRelatingColumn($('#TriggerRelatingColumns_Filter'), 'ViewFilters_');
    };
    $p.initRelatingColumn = function () {
        initRelatingColumn($('#TriggerRelatingColumns_Dialog'), $('#TableName').val());
    };
    $p.initRelatingColumnEditor = function () {
        initRelatingColumn($('#TriggerRelatingColumns_Editor'), $('#TableName').val());
    };
    // 自動ポストバック時はnoSend:trueを指定しparent-data-classなどの属性のセットのみ行う
    $p.initRelatingColumnEditorNoSend = function () {
        initRelatingColumn($('#TriggerRelatingColumns_Editor'), $('#TableName').val(), true);
    };
    if (methodType === 'edit' || methodType === 'new') {
        $p.initRelatingColumnEditor();
    } else {
        $p.initRelatingColumnWhenViewChanged();
    }

    function initRelatingColumn($trigger, tablename, noSend) {
        var param = $trigger.val();
        if (param === undefined) return;
        if (tablename === undefined) return;
        var rcols = JSON.parse(param);
        for (var k in rcols) {
            var prekey = '';
            for (var k2 in rcols[k].Columns) {
                if (
                    prekey !== '' &&
                    rcols[k].Columns[k2] !== null &&
                    rcols[k].ColumnsLinkedClass[rcols[k].Columns[k2]] !== null
                ) {
                    applyRelatingColumn(
                        prekey,
                        rcols[k].Columns[k2],
                        rcols[k].ColumnsLinkedClass[rcols[k].Columns[k2]],
                        tablename,
                        $trigger,
                        noSend
                    );
                }
                prekey = rcols[k].Columns[k2];
            }
        }
    }

    function applyRelatingColumn(prnt, chld, linkedClass, tablename, $trigger, noSend) {
        if ($p.disableAutPostback) return;
        $(document).ready(function () {
            var debounce = function (fn, interval) {
                let timer;
                return function () {
                    clearTimeout(timer);
                    timer = setTimeout(function () {
                        fn();
                    }, interval);
                };
            };
            if ($childElement.prop('tagName') === 'SELECT') {
                c_change(tablename, true);
                $(document).on(
                    'change',
                    '#' + tablename + '_' + prnt,
                    debounce(function () {
                        c_change(tablename, false);
                    }, 500)
                );
            }
        });
        var $parentElement = $('#' + tablename + '_' + prnt);
        var $childElement = $('#' + tablename + '_' + chld);
        var c_change = function (tablename, isInitDisplay) {
            var parentIds = [];
            if ($parentElement.prop('tagName') === 'SELECT') {
                var $parent = $('#' + tablename + '_' + prnt + ' option:selected');
                $parent.each(function (index, element) {
                    var value = $(element).val();
                    if (value === '\t') {
                        value = '-1';
                    }
                    parentIds.push(value);
                });
            } else {
                parentIds.push($parentElement.attr('data-value'));
            }
            var childIds = [];
            var $child = $('#' + tablename + '_' + chld + ' option:selected');
            $child.each(function (index, element) {
                childIds.push($(element).val());
            });
            $childElement.attr('parent-data-class', linkedClass);
            $childElement.attr('parent-data-id', JSON.stringify(parentIds));
            $childElement.attr('selected-options', JSON.stringify(childIds));
            var formData = $p.getData($trigger.closest('form'));
            formData['RelatingDropDownControlId'] = tablename + '_' + chld;
            formData['RelatingDropDownSelected'] = JSON.stringify(childIds);
            formData['RelatingDropDownParentClass'] = linkedClass;
            formData['RelatingDropDownParentDataId'] = JSON.stringify(parentIds);
            formData['IsInitDisplay'] = isInitDisplay;
            $trigger.attr('parent-data-class', linkedClass);
            $trigger.attr('parent-data-id', JSON.stringify(parentIds));
            $trigger.attr('data-action', 'RelatingDropDown');
            $trigger.attr('data-method', 'post');
            if (!noSend) {
                const formId = undefined;
                const _async = false;
                const clearMessage = false;
                $p.send($trigger, formId, _async, clearMessage);
            }
        };
    }
    $p.callbackRelatingColumn = function (targetId) {
        var $target = $(targetId);
        if ($target.length === 0) {
            return;
        }
        $p.RefreshMultiSelectRelatingColum($target);
        $target.addClass('always-send');
        $target.addClass('not-set-form-changed');
        $target.trigger('change');
        $target.removeClass('not-set-form-changed');
    };
});
