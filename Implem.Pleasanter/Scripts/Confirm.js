$p.confirmReload = function confirmReload() {
    if ($p.formChanged && $('#Editor').length === 1) {
        return confirm($p.display('ConfirmReload'));
    } else {
        return true;
    }
}