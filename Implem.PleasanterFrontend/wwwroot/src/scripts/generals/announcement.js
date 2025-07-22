$p.closeAnnouncement = function ($control) {
    let recordId = $control.attr('data-id');
    let data = {
        'AnnouncementId': recordId
    };
    $p.ajax(
        $('#ApplicationPath').val() + 'Users/CloseAnnouncement',
        'POST',
        data,
        undefined);
};