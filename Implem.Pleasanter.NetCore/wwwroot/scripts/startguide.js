$p.setStartGuide = function (disable, redirect) {
    var data = {};
    data.DisableStartGuide = disable;
    $p.ajax(
        $('#ApplicationPath').val() + 'Users/SetStartGuide',
        'POST',
        data,
        undefined,
        redirect !==1);
    if (redirect === 1) {
        location.href = $('#ApplicationPath').val();
    }
}