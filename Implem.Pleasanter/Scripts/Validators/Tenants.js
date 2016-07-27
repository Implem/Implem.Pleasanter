$p.form.validators.tenants = function () {
    $('#TenantForm').validate({
        rules: {
            Tenants_TenantName: { required:true },
            Tenants_Title: { required:true }
        },
        messages: {
            Tenants_TenantName: { required: $('#Tenants_TenantName').attr('data-validate-required') },
            Tenants_Title: { required: $('#Tenants_Title').attr('data-validate-required') }
        }
    });
}
$p.form.validators.tenants();
