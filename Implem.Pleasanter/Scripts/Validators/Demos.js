$p.validateDemos = function () {
    $('#DemoForm').validate({
        ignore: '',
        rules: {
            Demos_Title: { required:true }
        },
        messages: {
            Demos_Title: { required: $('#Demos_Title').attr('data-validate-required') }
        }
    });
}
$p.validateDemos();
