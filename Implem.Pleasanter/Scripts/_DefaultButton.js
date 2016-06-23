function cancelDefaultButton(e) {
    if (!e) var e = window.event;
    if (e.keyCode == 13) {
        if (e.srcElement.type != 'submit' && e.srcElement.type != 'textarea') {
            if (e.srcElement.type != 'image') {
                return false;
            }
        }
    }
}