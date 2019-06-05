$p.confirmReload = function confirmReload() {
    if ($p.formChanged) {
        return confirm($p.display('ConfirmUnload'));
    } else {
        return true;
    }
}