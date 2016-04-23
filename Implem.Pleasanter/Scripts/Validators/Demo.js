$(function () {
    $('#DemoForm').validate({
        rules: {
            Demos_Title: { required:true }
        },
        messages: {
            Demos_Title: { required: $('#Demos_Title').attr('data-validate-required') }
        }
    });
});
