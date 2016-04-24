$(function () {
    $('#ResultForm').validate({
        rules: {
            Results_Title: { required:true },
            Results_DateA: { date:true },
            Results_DateB: { date:true },
            Results_DateC: { date:true },
            Results_DateD: { date:true },
            Results_DateE: { date:true },
            Results_DateF: { date:true },
            Results_DateG: { date:true },
            Results_DateH: { date:true },
            Results_DateI: { date:true },
            Results_DateJ: { date:true },
            Results_DateK: { date:true },
            Results_DateL: { date:true },
            Results_DateM: { date:true },
            Results_DateN: { date:true },
            Results_DateO: { date:true },
            Results_DateP: { date:true }
        },
        messages: {
            Results_Title: { required: $('#Results_Title').attr('data-validate-required') },
            Results_DateA: { date: $('#Results_DateA').attr('data-validate-date') },
            Results_DateB: { date: $('#Results_DateB').attr('data-validate-date') },
            Results_DateC: { date: $('#Results_DateC').attr('data-validate-date') },
            Results_DateD: { date: $('#Results_DateD').attr('data-validate-date') },
            Results_DateE: { date: $('#Results_DateE').attr('data-validate-date') },
            Results_DateF: { date: $('#Results_DateF').attr('data-validate-date') },
            Results_DateG: { date: $('#Results_DateG').attr('data-validate-date') },
            Results_DateH: { date: $('#Results_DateH').attr('data-validate-date') },
            Results_DateI: { date: $('#Results_DateI').attr('data-validate-date') },
            Results_DateJ: { date: $('#Results_DateJ').attr('data-validate-date') },
            Results_DateK: { date: $('#Results_DateK').attr('data-validate-date') },
            Results_DateL: { date: $('#Results_DateL').attr('data-validate-date') },
            Results_DateM: { date: $('#Results_DateM').attr('data-validate-date') },
            Results_DateN: { date: $('#Results_DateN').attr('data-validate-date') },
            Results_DateO: { date: $('#Results_DateO').attr('data-validate-date') },
            Results_DateP: { date: $('#Results_DateP').attr('data-validate-date') }
        }
    });
});
