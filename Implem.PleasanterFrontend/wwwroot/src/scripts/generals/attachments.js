$p.uploadAttachments = function (control, filesList) {
    var createUuid = function () {
        var uuid = '',
            i,
            random;
        for (i = 0; i < 32; i++) {
            random = (Math.random() * 16) | 0;
            if (i === 8 || i === 12 || i === 16 || i === 20) {
                uuid += '';
            }
            uuid += (i === 12 ? 4 : i === 16 ? (random & 3) | 8 : random).toString(16);
        }
        return uuid.toUpperCase();
    };

    var fileClear = function () {
        var c = input.clone();
        c.val('');
        input.replaceWith(c);
    };

    var deleteTemp = function (guid) {
        var deleteData = { Guid: guid };
        $p.ajax(deleteUrl, 'post', deleteData, null, true);
    };

    function createStatusbar(status, progressBar, abort) {
        this.progressBar = progressBar;
        this.abort = abort;
        this.status = status;
        this.resetProgress = function () {
            this.progressBar.find('div').width(0);
        };
        this.updateTime = new Date().getTime();
        this.setProgress = function () {
            if (this.updateTime + 250 > new Date().getTime()) return;
            this.updateTime = new Date().getTime();
            var uploaded = 0;
            for (var index = 0; index < progresses.length; ++index) {
                uploaded += progresses[index];
            }
            const progress = uploaded / (allUploadSize * 2);
            const progressBarWidth = progress * this.progressBar.width();
            this.progressBar.find('div').animate({ width: progressBarWidth }, 10);
        };
        this.setAbortCalcHash = function () {
            var sb = this.status;
            this.abort.on('click.n1', function () {
                isAborted = true;
                sb.hide();
                fileClear();
                controls.statusBar.abort.off('click.n1');
                unbindUnloadHandler();
            });
        };
        var aboutUploader = [];
        this.setAbort = function (uploader, uuid) {
            aboutUploader.push({ uploader: uploader, uuid: uuid });
            this.abort.off('click.n2');
            this.abort.on('click.n2', function () {
                for (var index = 0; index < aboutUploader.length; ++index) {
                    const v = aboutUploader[index];
                    v.uploader.abort();
                    deleteTemp(v.uuid);
                }
                controls.statusBar.abort.off('click.n2');
            });
        };
    }

    function getMd5(fileIndex, file, uuid, func) {
        const hash = md5.create();
        const chunkSize = 1000 * 10;
        let offset = 0;
        let blockRead = null;
        const readContent = function (evt) {
            if (isAborted) {
                fileClear();
                return;
            }
            const result = new Uint8Array(evt.target.result);
            offset += result.length;
            hash.update(result);
            controls.statusBar.setProgress();
            if (offset >= file.size) {
                progresses[fileIndex] = file.size;
                func(fileIndex, file, uuid, hash.hex());
            } else {
                progresses[fileIndex] = offset;
                blockRead(offset);
            }
        };
        blockRead = function (_offset) {
            const blob = file.slice(_offset, chunkSize + _offset);
            const fileReader = new FileReader();
            fileReader.onloadend = readContent;
            fileReader.readAsArrayBuffer(blob);
        };
        blockRead(offset);
    }

    function upload(fileIndex, file, uuid, fileHash) {
        sendData.formData.FileHash = fileHash;
        if ($p.data.MainForm) {
            for (const key of Object.keys($p.data.MainForm)) {
                formData[key] = $p.data.MainForm[key];
            }
        }
        if ($('#IsNew').length) {
            sendData.formData.IsNew = $('#IsNew').val();
        }
        var ldata = jQuery.extend(true, {}, sendData);
        ldata.formData.uuid = uuid;
        ldata.files = [file];
        ldata.formData.FileHash = fileHash;
        ldata.formData.FileIndex = fileIndex;
        var jqXHR = input.fileupload('send', ldata);
        controls.statusBar.setAbort(jqXHR, uuid);
    }

    var dones = [];
    var fileIndex = 0;
    for (fileIndex = 0; fileIndex < filesList.length; fileIndex++) {
        dones.push(false);
    }
    var allUploadSize = 0;

    var success = true;
    var isAborted = false;
    var isSending = false;
    var progresses = [];
    var controls = new Object();
    var columnName = control.attr('data-name');
    var dataName = control.attr('data-name');
    var uuids = [];
    for (fileIndex = 0; fileIndex < filesList.length; fileIndex++) {
        uuids.push(createUuid());
    }
    var fieldControl = control.closest('.field-control');
    var siteId = fieldControl.closest('#EditorTabsContainer').attr('site-id');
    var url = $('.main-form')
        .attr('action')
        .replace('_action_', 'upload')
        .replace('items', 'binaries');
    var deleteUrl = $('.main-form').attr('action').replace('_action_', 'binaries/deletetemp');
    if (siteId) {
        url = url.replace(/\/binaries\/[0-9]+\/upload/g, '/binaries/' + siteId + '/upload');
        deleteUrl = deleteUrl.replace(
            /\/items\/[0-9]+\/binaries\/deletetemp/g,
            '/items/' + siteId + '/binaries/deletetemp'
        );
    }
    var controlId = control.parent().find('.control-attachments').attr('id');
    var token = $('#Token').val();
    var attachmentsData = $('#' + controlId).val();
    var formData = {
        Uuids: uuids,
        ColumnName: dataName,
        ControlId: controlId,
        Token: token,
        AttachmentsData: attachmentsData
    };
    var dataType = 'json';
    var maxChunkSize = 1047552 * 10;
    var sendData = new Object();
    sendData.formData = formData;
    sendData.url = url;
    sendData.dataType = dataType;
    sendData.maxChunkSize = maxChunkSize;
    sendData.files = filesList;
    var fileNameArray = [];
    var fileSizeArray = [];
    var fileTypeArray = [];
    for (fileIndex = 0; fileIndex < filesList.length; fileIndex++) {
        fileNameArray.push(filesList[fileIndex].name);
        fileSizeArray.push(filesList[fileIndex].size);
        fileTypeArray.push(filesList[fileIndex].type);
        progresses.push(0);
        allUploadSize += filesList[fileIndex].size;
    }
    sendData.formData.fileNames = JSON.stringify(fileNameArray);
    sendData.formData.fileSizes = JSON.stringify(fileSizeArray);
    sendData.formData.fileTypes = JSON.stringify(fileTypeArray);
    var $status = control.parent().find('[id="' + columnName + '.status"]');
    var statusBar = new createStatusbar(
        $status,
        control.parent().find('[id="' + columnName + '.progress"]'),
        control.parent().find('[id="' + columnName + '.abort"]')
    );
    controls.statusBar = statusBar;
    controls.statusBar.abort.click();
    controls.statusBar.abort.off('click.n1');
    controls.statusBar.abort.off('click.n2');
    controls.statusBar.setAbortCalcHash();
    statusBar.resetProgress();
    $status.show();

    var input = fieldControl.find('#' + columnName + '\\.input');
    input
        .fileupload({
            dropZone: $('.control-attachments-upload')
        })
        .on('fileuploadprogress', function (e, data) {
            progresses[data.formData.FileIndex] = data.total + data.loaded;
            controls.statusBar.setProgress();
        })
        .on('fileuploadstart', function (e, data) {
            isSending = true;
        })
        .on('fileuploaddone', function (e, data) {
            var fileIndex = data.formData.FileIndex;
            if (!success) {
                fileClear();
                return false;
            }
            if (!data.result.files || isAborted) {
                controls.statusBar.status.hide();
                success = false;
                for (var index = 0; index < uuids.length; ++index) {
                    deleteTemp(uuids[index]);
                }
                if (!isAborted) {
                    $.each(data.result, function () {
                        $p.setByJsonElement(this);
                    });
                }
            }
            if (!success) {
                fileClear();
                return false;
            }
            dones[fileIndex] = true;
            if (
                dones.filter(function (value) {
                    return !value;
                }).length > 0
            )
                return;
            var json = data.result.ResponseJson;
            var url = data.url;
            var methodType = 'POST';
            var $control = $(this).parent();
            var fdata = data.formData;
            $p.setByJson(url, methodType, fdata, $control, true, JSON.parse(json));
            isSending = false;
            unbindUnloadHandler();
        });

    for (fileIndex = 0; fileIndex < filesList.length; fileIndex++) {
        getMd5(fileIndex, filesList[fileIndex], uuids[fileIndex], upload);
    }
    var buttons = $('#MainCommands > button').filter(function (index, element) {
        return element.getAttribute('data-action') === 'Update';
    });
    $(window).off('unload.' + columnName);
    buttons.off('click.' + columnName);
    var unload = function () {
        isAborted = true;
        unbindUnloadHandler();
        if (isSending) {
            for (var index = 0; index < uuids.length; ++index) {
                deleteTemp(uuids[index]);
            }
        }
    };
    var updateClick = function () {
        isAborted = true;
        unbindUnloadHandler();
    };
    var unbindUnloadHandler = function () {
        $(window).off('unload.' + columnName);
        buttons.off('click.' + columnName);
    };
    $(window).on('unload.' + columnName, unload);
    buttons.on('click.' + columnName, updateClick);
    var deleteAllAddedTemp = function () {
        var attachments = $('.control-attachments');
        for (var index = 0; index < attachments.length; ++index) {
            var json = JSON.parse(attachments[index].value);
            json.filter(function (item, index, array) {
                if (item.Added === true) {
                    deleteTemp(item.Guid);
                }
            });
        }
    };
    $(window).off('unload.deleteAllAddedTemp');
    $(window).on('unload.deleteAllAddedTemp', deleteAllAddedTemp);
};

$p.deleteAttachment = function ($control, $data) {
    var json = JSON.parse($control.val());
    var target = json.find(
        v => v.Guid == $data.attr('data-id') && v.Overwritten && v.Added != true
    );
    json = json.filter(function (item, index, array) {
        if (item.Added === true) {
            if (
                item.Guid === $data.attr('data-id') ||
                (target != null && item.Name == target.Name)
            ) {
                var data = {};
                data.Guid = item.Guid;
                var url = $('.main-form')
                    .attr('action')
                    .replace('_action_', $data.attr('data-action'));
                $p.ajax(url, 'post', data);
                $('#' + item.Guid).remove();
                return false;
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
        return true;
    });
    json = JSON.stringify(json);
    $control.val(json);
    $p.setData($control);
    $p.setFormChanged($(this));
};
