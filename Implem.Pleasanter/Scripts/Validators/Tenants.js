$p.validateTenants = function () {
    $('#TenantForm').validate({
        ignore: '',
        rules: {
            Tenants_TenantName: { required:true },
            Tenants_Title: { required:true }
        },
        messages: {
            Tenants_TenantName: { required:$p.display('ValidateRequired') },
            Tenants_Title: { required:$p.display('ValidateRequired') }
        }
    });
}
$p.validateTenants();
