$(document).on('click', '.grid-row.history', function () {
    var formData = getFormData($(this));
    formData['Ver'] = $(this).attr('data-ver');
    formData['Latest'] = $(this).attr('data-latest');
    formData['SwitchTargets'] = $('#SwitchTargets').val();
    requestByForm(getForm($(this)), $(this));
});