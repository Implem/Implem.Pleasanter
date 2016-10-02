$p.validateDepts = function () {
    $('#DeptForm').validate({
        ignore: '',
        rules: {
            Depts_DeptName: { required:true }
        },
        messages: {
            Depts_DeptName: { required:$p.display('ValidateRequired') }
        }
    });
}
$p.validateDepts();
