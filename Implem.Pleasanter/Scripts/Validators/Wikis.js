$p.form.validators.wikis = function () {
    $('#WikiForm').validate({
        rules: {
            Wikis_Title: { required:true }
        },
        messages: {
            Wikis_Title: { required: $('#Wikis_Title').attr('data-validate-required') }
        }
    });
}
$p.form.validators.wikis();
