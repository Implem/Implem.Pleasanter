$p.templates = function ($control) {
    $p.send($control, 'MainForm');
    if ($p.responsive() && screen.width < 1025) {
        $p.openResponsiveMenu();
    }
}

$p.setTemplate = function () {
    var selector = '.template-selectable .control-selectable';
    $('#TemplateTabsContainer:not(.applied)').tabs({
        beforeActivate: function (event, ui) {
            $p.setTemplateData(ui.newPanel.find(selector));
        }
    }).addClass('applied');
    $(selector).selectable({
        selected: function (event, ui) {
            $(ui.selected)
                .addClass("ui-selected")
                .siblings()
                .removeClass("ui-selected")
                .each(function (key, value) {
                    $(value).find('*').removeClass("ui-selected");
                });
        },
        stop: function () {
            $p.setTemplateData($(this));
        }
    });
}

$p.setTemplateData = function ($control) {
    var selected = $control.find('li.ui-selected');
    var show = selected.length === 1;
    $p.setData($control);
    $p.send($control);
    $('#OpenSiteTitleDialog')
        .removeClass('hidden')
        .toggle(show);
    $('#SiteTitle').val(show
        ? selected[0].innerText
        : '');
    $('#TemplateId').val(show
        ? selected[0].getAttribute('data-value')
        : '');
    $('#Template_Title').val(show
        ? selected[0].innerText
        : '');
    $('#Template_Id').val(show
        ? selected[0].getAttribute('data-value')
        : '');
}

$p.setTemplateViewer = function () {
    $('.template-tab-container').tabs().addClass('applied');
}

$p.createSite = async function ($control) {
    await $p.send($control);
    window.location.reload();
}

$p.openSiteTitleDialog = function ($control) {
    if ($('#FieldSetUserTemplate').attr('aria-hidden') === 'false') {
        $('#CreateUserTemplate_Title').text($('#SiteTitle').val());
        $('#CreateUserTemplateDialog').dialog({
            modal: true,
            width: '370px',
            appendTo: '#Application',
            resizable: false
        });
    } else {
        let selectedValue = $('#StandardTemplates .ui-selected:first').data('value');
        if (selectedValue != 'Template2' && selectedValue != 'Template3') {
            $('#DisableCalendar').closest('.field-auto-thin').hide();
            $('#DisableCrosstab').closest('.field-auto-thin').hide();
            $('#DisableTimeSeries').closest('.field-auto-thin').hide();
            $('#DisableAnaly').closest('.field-auto-thin').hide();
            $('#DisableKamban').closest('.field-auto-thin').hide();
            $('#DisableImageLib').closest('.field-auto-thin').hide();
        } else {
            $('#DisableCalendar').closest('.field-auto-thin').show();
            $('#DisableCrosstab').closest('.field-auto-thin').show();
            $('#DisableTimeSeries').closest('.field-auto-thin').show();
            $('#DisableAnaly').closest('.field-auto-thin').show();
            $('#DisableKamban').closest('.field-auto-thin').show();
            $('#DisableImageLib').closest('.field-auto-thin').show();
        }
        if (selectedValue != 'Template2') {
            $('#DisableGantt').closest('.field-auto-thin').hide();
            $('#DisableBurnDown').closest('.field-auto-thin').hide();
        } else {
            $('#DisableGantt').closest('.field-auto-thin').show();
            $('#DisableBurnDown').closest('.field-auto-thin').show();
        }
        $('#SiteTitleDialog').dialog({
            modal: true,
            width: "520px",
            appendTo: '#Application',
            resizable: false
        });
    }
}

$p.refreshTemplateSelector = function () {
    $('#TemplateTabsContainer').tabs("refresh");
    if ($('#FieldSetUserTemplate').attr('aria-hidden') === 'false') {
        $p.setTemplateData($('#UserTemplateTemplates'));
    }
}

$p.openImportUserTemplateDialog = function ($control) {
    $('#ImportUserTemplateForm').validate().resetForm();
    $p.set($('#ImportUserTemplate_Import'), '');
    $p.set($('#ImportUserTemplate_Title'), '');
    $p.set($('#ImportUserTemplate_Description'), '');
    $p.set($('#ImportUserTemplate_SearchText'), $('#Template_SearchText').val());
    $('#ImportUserTemplateDialog').dialog({
        modal: true,
        width: '520px'
    });
    if ($p.responsive() && screen.width < 1025) {
        $p.openResponsiveMenu();
    }
}

$p.importUserTemplate = function ($control) {
    $form = $('#ImportUserTemplateForm');
    if ($control.hasClass('validate')) {
        $p.formValidate($form, $control);
        if (!$form.valid()) {
            $p.setValidationError($form);
            $p.setErrorMessage('ValidationError');
            if (!$control.closest('.ui-dialog')) {
                $("html,body").animate({
                    scrollTop: $('.error:first').offset().top
                });
            }
            return false;
        }
    }
    var data = new FormData();
    data.append('file', $('#ImportUserTemplate_Import').prop('files')[0]);
    data.append('Title', $('#ImportUserTemplate_Title').val());
    data.append('Description', $('#ImportUserTemplate_Description').val());
    data.append('SearchText', $('#Template_SearchText').val());
    $p.multiUpload(
        $('.main-form').attr('action').replace('_action_', $control.attr('data-action')),
        data,
        $control);
    return true;
}

$p.openEditUserTemplateDialog = function () {
    $('#EditUserTemplateDialog').dialog({
        modal: true,
        width: '520px'
    });
    if ($p.responsive() && screen.width < 1025) {
        $p.openResponsiveMenu();
    }
}

