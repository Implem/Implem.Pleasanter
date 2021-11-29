$(function () {
    $(document).on('click', '.current-time', function () {
        var $control = $(this).prev();
        var date = new Date();
        var dateTime = date.getFullYear()
            + '/' + ('0' + (date.getMonth() + 1)).slice(-2)
            + '/' + ('0' + date.getDate()).slice(-2);
        switch ($control.attr('data-format')) {
            case 'Y/m/d H:i':
                dateTime
                    += ' ' + ('0' + date.getHours()).slice(-2)
                    + ':' + ('0' + date.getMinutes()).slice(-2);
                break;
            case 'Y/m/d H:i:s':
                dateTime
                    += ' ' + ('0' + date.getHours()).slice(-2)
                    + ':' + ('0' + date.getMinutes()).slice(-2)
                    + ':' + ('0' + date.getSeconds()).slice(-2);
                break;
        }
        $p.set($control, dateTime);
        $control.change();
    });
});