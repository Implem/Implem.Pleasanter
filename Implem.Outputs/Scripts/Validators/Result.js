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
            Results_DateH: { date:true }
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
            Results_DateH: { date: $('#Results_DateH').attr('data-validate-date') }
        }
    });
});
