$p.uploadAttachments = function (control, filesList) {
    var createUuid = function () {
        var uuid = "", i, random;
        for (i = 0; i < 32; i++) {
            random = Math.random() * 16 | 0;
            if (i === 8 || i === 12 || i === 16 || i === 20) {
                uuid += "";
            }
            uuid += (i === 12 ? 4 : (i === 16 ? (random & 3 | 8) : random)).toString(16);
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
        $p.ajax(deleteUrl, 'post', deleteData, null, false);
    };

    function createStatusbar(status, progressBar, abort) {
        this.progressBar = progressBar;
        this.abort = abort;
        this.status = status;
        this.resetProgress = function () {
            this.progressBar.find('div').width(0);
        };
        this.setProgress = function (progresses) {
            var totalSize = 0;
            var progressSize = 0;
            for (var fileIndex = 0; fileIndex < filesList.length; ++fileIndex) {
                totalSize += filesList[fileIndex].size;
                progressSize += progresses[fileIndex];
            }
            var progress = progressSize / totalSize;
            var progressBarWidth = progress * this.progressBar.width();
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
        this.setAbort = function (uploader) {
            var sb = this.status;
            this.abort.on('click.n2', function () {
                uploader.abort();
                for (var index = 0; index < uuids.length; ++index) {
                    deleteTemp(uuids[index]);
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
        let time = new Date().getTime() - 1000;
        const readContent = function (evt) {
            if (isAborted) {
                fileClear();
                return;
            }
            const result = new Uint8Array(evt.target.result);
            offset += result.length;
            if ((time + 250) < new Date().getTime()) {
                time = new Date().getTime();
                var progress = parseInt(offset / 2, 10);
                progresses[fileIndex] = progress;
                controls.statusBar.setProgress(progresses);
            }
            hash.update(result);
            if (offset >= file.size) {
                func(fileIndex, file, uuid, hash.hex());
            } else {
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
        if ($("#IsNew").length) {
            sendData.formData.IsNew = $("#IsNew").val();
        }
        input.fileupload({
            dropZone: $(".control-attachments-upload")
        })
            .on('fileuploadprogressall', function (e, data) {
                var progress = 0;
                if (filesList.length < 2) {
                    progress = parseInt((data.loaded / 2) + (file.size / 2), 10);
                    if (dones[fileIndex]) progress = file.size;
                    progresses[fileIndex] = progress;
                    controls.statusBar.setProgress(progresses);
                } else {
                    progress = progresses[fileIndex];
                    if (!progress) progress = parseInt((file.size / 2), 10);
                    else progress += parseInt((maxChunkSize / 2), 10);
                    if (progress > file.size) progress = file.size;
                    progresses[fileIndex] = progress;
                    controls.statusBar.setProgress(progresses);
                }
            })
            .on('fileuploaddone', function (e, data) {
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
                    if (!isAborted) $p.setByJsonElement(data.result[0]);
                }
                if (!success) {
                    fileClear();
                    return false;
                }
                dones[fileIndex] = true;
                if (dones.filter(function (value) { return !value; }).length > 0) return;
                var json = data.result.ResponseJson;
                var url = data.url;
                var methodType = 'POST';
                var $control = $(this).parent();
                var fdata = data.formData;
                $p.setByJson(url, methodType, fdata, $control, true, JSON.parse(json));
                isSending = false;
                unbindUnloadHandler();
            })
            .on('fileuploadchunkdone', function (e, data) {
                if (!data.result.files || isAborted) {
                    controls.statusBar.status.hide();
                    success = false;
                    for (var index = 0; index < uuids.length; ++index) {
                        deleteTemp(uuids[index]);
                    }
                    if (!isAborted) $p.setByJsonElement(data.result[0]);
                }
            })
            .on('fileuploadchunksend', function (e, data) {
                isSending = true;
                if (success) return true;
                fileClear();
                return false;
            });
        var ldata = jQuery.extend(true, {}, sendData);
        ldata.formData.uuid = uuid;
        ldata.files = [file];
        ldata.formData.FileHash = fileHash;
        var jqXHR = input.fileupload('send', ldata);
        controls.statusBar.setAbort(jqXHR);
    }

    var dones = new Array();
    var fileIndex = 0;
    for (fileIndex = 0; fileIndex < filesList.length; fileIndex++) {
        dones.push(false);
    }
    var success = true;
    var isAborted = false;
    var isSending = false;
    var progresses = new Array();
    var controls = new Object();
    var columnName = control.attr('data-name');
    var dataName = control.attr('data-name');
    var uuids = new Array();
    for (fileIndex = 0; fileIndex < filesList.length; fileIndex++) {
        uuids.push(createUuid());
    }
    var fieldControl = control.closest('.field-control');
    var input = fieldControl.find("#" + columnName + "\\.input");
    var siteId = fieldControl.closest("#EditorTabsContainer").attr("site-id");
    var url = $('.main-form').attr('action').replace('_action_', 'upload').replace('items', 'binaries');
    var deleteUrl = $('.main-form').attr('action').replace('_action_', 'binaries/deletetemp');
    if (siteId) {
        url = url.replace(/\/binaries\/[0-9]+\/upload/g, "/binaries/" + siteId + "/upload");
        deleteUrl = deleteUrl.replace(/\/items\/[0-9]+\/binaries\/deletetemp/g, "/items/" + siteId + "/binaries/deletetemp");
    }
    var controlId = control.parent().find('.control-attachments').attr('id');
    var attachmentsData = $('#' + controlId).val();
    var formData = { Uuids: uuids, ColumnName: dataName, ControlId: controlId, AttachmentsData: attachmentsData };
    var dataType = 'json';
    var maxChunkSize = 1047552 * 10;
    var sendData = new Object();
    sendData.formData = formData;
    sendData.url = url;
    sendData.dataType = dataType;
    sendData.maxChunkSize = maxChunkSize;
    sendData.files = filesList;
    var fileNameArray = new Array();
    var fileSizeArray = new Array();
    var fileTypeArray = new Array();
    for (fileIndex = 0; fileIndex < filesList.length; fileIndex++) {
        fileNameArray.push(filesList[fileIndex].name);
        fileSizeArray.push(filesList[fileIndex].size);
        fileTypeArray.push(filesList[fileIndex].type);
        progresses.push(0);
    }
    sendData.formData.fileNames = JSON.stringify(fileNameArray);
    sendData.formData.fileSizes = JSON.stringify(fileSizeArray);
    sendData.formData.fileTypes = JSON.stringify(fileTypeArray);
    var $status = control.parent().find('[id="' + columnName + '.status"]');
    var statusBar = new createStatusbar(
        $status,
        control.parent().find('[id="' + columnName + '.progress"]'),
        control.parent().find('[id="' + columnName + '.abort"]'));
    controls.statusBar = statusBar;
    controls.statusBar.abort.click();
    controls.statusBar.abort.off('click.n1');
    controls.statusBar.abort.off('click.n2');
    controls.statusBar.setAbortCalcHash();
    statusBar.resetProgress();
    $status.show();
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
    $p.setFormChanged($(this));
};