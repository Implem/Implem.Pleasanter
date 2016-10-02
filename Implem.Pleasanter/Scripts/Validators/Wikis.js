$p.validateWikis = function () {
    $('#WikiForm').validate({
        ignore: '',
        rules: {
            Wikis_Title: { required:true }
        },
        messages: {
            Wikis_Title: { required:$p.display('ValidateRequired') }
        }
    });
}
$p.validateWikis();
