$(document).on('dragenter', '.control-attachments-upload', function (e) {
    e.stopPropagation();
    e.preventDefault();
});

$(document).on('dragover', '.control-attachments-upload', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(this).css('border', '2px solid #d19405');
});

$(document).on('drop', '.control-attachments-upload', function (e) {
    var $control = $(this);
    $control.css('border', '2px dotted #d19405');
    e.preventDefault();
    var files = e.originalEvent.dataTransfer.files;
    $p.uploadAttachments($control, files);
});

$(document).on('click', '.control-attachments-upload', function (e) {
    var $control = $(this);
    var input = document.getElementById($control.attr('data-name') + '.input');
    input.click();
});

$(document).on('change', '.control-attachments-upload', function (e) {
    var $control = $(this);
    var input = document.getElementById($control.attr('data-name') + '.input');
    if (input.files.length == 0) return;
    $p.uploadAttachments($control, input.files);
});

$(document).on('dragenter', function (e) {
    e.stopPropagation();
    e.preventDefault();
});

$(document).on('dragover', function (e) {
    e.stopPropagation();
    e.preventDefault();
    $(this).css('border', '2px solid #d19405');
});

$(document).on('drop', function (e) {
    e.stopPropagation();
    e.preventDefault();
});