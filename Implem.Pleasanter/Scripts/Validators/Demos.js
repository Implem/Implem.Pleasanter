$p.validateDemos = function () {
    $('#DemoForm').validate({
        ignore: '',
        rules: {
            Demos_Title: { required:true }
        },
        messages: {
            Demos_Title: { required:$p.display('ValidateRequired') }
        }
    });
}
$p.validateDemos();
