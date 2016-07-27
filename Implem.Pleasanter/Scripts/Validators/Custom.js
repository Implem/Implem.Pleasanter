$.validator.addMethod(
    'c_num',
    function (value, element) {
        return this.optional(element) || /^(-)?(¥|\\|\$)?[\d,.]+$/.test(value);
    }
);