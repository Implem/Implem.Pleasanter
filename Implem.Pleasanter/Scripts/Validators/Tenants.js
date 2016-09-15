$p.validateTenants = function () {
    $('#TenantForm').validate({
        ignore: '',
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
$p.validateTenants();
