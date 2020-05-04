$(function () {
    var selector = '#' + $p.tableName() + '_Locked';
    $(document).on('change', selector , function () {
        if ($('#LockedRecord').val() === '1' && !$(selector).prop('checked')) {
            if (confirm($p.display('ConfirmUnlockRecord'))) {
                var $form = $('#MainForm');
                var action = 'unlockrecord';
                $p.setMustData($form, action);
                $p.ajax(
                    $form.attr('action').replace('_action_', action),
                    'POST',
                    $p.getData($form));
            } else {
                $p.set($(selector), true);
            }
        }
    });
});