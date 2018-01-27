$p.uploadAttachments = function ($upload, files) {
    var columnName = $upload.attr('data-name');
    var controlId = $upload.parent().find('.control-attachments').attr('id');
    var url = $('.main-form').attr('action').replace('_action_', $upload.attr('data-action'));
    var data = new FormData();
    for (var i = 0; i < files.length; i++) {
        data.append('file', files[i]);
    }
    data.append('ControlId', controlId);
    data.append('ColumnName', columnName);
    data.append('AttachmentsData', $('#' + controlId).val());
    var $status = $('[id="' + columnName + '.status"]');
    var statusBar = new createStatusbar(
        $status,
        $('[id="' + columnName + '.progress"]'),
        $('[id="' + columnName + '.abort"]'));
    $status.show();
    $p.multiUpload(url, data, statusBar);

    function createStatusbar(status, progressBar, abort) {
        this.progressBar = progressBar;
        this.abort = abort;
        this.status = status;
        this.setProgress = function (progress) {
            var progressBarWidth = progress * this.progressBar.width() / 100;
            this.progressBar.find('div').animate({ width: progressBarWidth }, 10);
        }
        this.setAbort = function (uploader) {
            var sb = this.status;
            this.abort.click(function () {
                uploader.abort();
                sb.hide();
            });
        }
    }
}

$p.deleteAttachment = function ($control, $data) {
    var json = JSON.parse($control.val());
    json.filter(function (item, index, array) {
        if (item.Added === true) {
            if (item.Guid.toString() === $data.attr('data-id')) {
                var data = {};
                data.Guid = item.Guid;
                url = $('.main-form')
                    .attr('action')
                    .replace('_action_', $data.attr('data-action'));
                $p.ajax(url, 'post', data);
                $('#' + item.Guid).remove();
                array.splice(index, 1);
            }
        } else {
            if (item.Guid === $data.attr('data-id')) {
                if (!item.Deleted) {
                    item.Deleted = true;
                    $data.parent().addClass('preparation-delete');
                    $data.removeClass('ui-icon-circle-close');
                    $data.addClass('ui-icon-trash');

                } else {
                    item.Deleted = false;
                    $data.parent().removeClass('preparation-delete');
                    $data.removeClass('ui-icon-trash');
                    $data.addClass('ui-icon-circle-close');
                }
            }
        }
    });
    json = JSON.stringify(json);
    $control.val(json);
    $p.setData($control);
}