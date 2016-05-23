$(function () {
    $('#DeptForm').validate({
        rules: {
            Depts_DeptName: { required:true }
        },
        messages: {
            Depts_DeptName: { required: $('#Depts_DeptName').attr('data-validate-required') }
        }
    });
});
