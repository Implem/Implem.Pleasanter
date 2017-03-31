$p.confirmReload = function confirmReload() {
    if ($p.formChanged) {
        return confirm($p.display('ConfirmReload'));
    } else {
        return true;
    }
}