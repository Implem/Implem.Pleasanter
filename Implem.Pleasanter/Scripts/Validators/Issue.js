$(function () {
    $('#IssueForm').validate({
        rules: {
            Issues_CompletionTime: { required:true,date:true },
            Issues_DateA: { date:true },
            Issues_DateB: { date:true },
            Issues_DateC: { date:true },
            Issues_DateD: { date:true },
            Issues_DateE: { date:true },
            Issues_DateF: { date:true },
            Issues_DateG: { date:true },
            Issues_DateH: { date:true },
            Issues_DateI: { date:true },
            Issues_DateJ: { date:true },
            Issues_DateK: { date:true },
            Issues_DateL: { date:true },
            Issues_DateM: { date:true },
            Issues_DateN: { date:true },
            Issues_DateO: { date:true },
            Issues_DateP: { date:true },
            Issues_Title: { required:true }
        },
        messages: {
            Issues_CompletionTime: { required: $('#Issues_CompletionTime').attr('data-validate-required'),date: $('#Issues_CompletionTime').attr('data-validate-date') },
            Issues_DateA: { date: $('#Issues_DateA').attr('data-validate-date') },
            Issues_DateB: { date: $('#Issues_DateB').attr('data-validate-date') },
            Issues_DateC: { date: $('#Issues_DateC').attr('data-validate-date') },
            Issues_DateD: { date: $('#Issues_DateD').attr('data-validate-date') },
            Issues_DateE: { date: $('#Issues_DateE').attr('data-validate-date') },
            Issues_DateF: { date: $('#Issues_DateF').attr('data-validate-date') },
            Issues_DateG: { date: $('#Issues_DateG').attr('data-validate-date') },
            Issues_DateH: { date: $('#Issues_DateH').attr('data-validate-date') },
            Issues_DateI: { date: $('#Issues_DateI').attr('data-validate-date') },
            Issues_DateJ: { date: $('#Issues_DateJ').attr('data-validate-date') },
            Issues_DateK: { date: $('#Issues_DateK').attr('data-validate-date') },
            Issues_DateL: { date: $('#Issues_DateL').attr('data-validate-date') },
            Issues_DateM: { date: $('#Issues_DateM').attr('data-validate-date') },
            Issues_DateN: { date: $('#Issues_DateN').attr('data-validate-date') },
            Issues_DateO: { date: $('#Issues_DateO').attr('data-validate-date') },
            Issues_DateP: { date: $('#Issues_DateP').attr('data-validate-date') },
            Issues_Title: { required: $('#Issues_Title').attr('data-validate-required') }
        }
    });
});
