$(document).ready(function () {
    $('#contact_form').bootstrapValidator({
        // To use feedback icons, ensure that you use Bootstrap v3.1.0 or later
        feedbackIcons: {
            //valid: 'glyphicon glyphicon-ok',
            //invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            FirstName: {
                validators: {
                    stringLength: {
                        min: 2,
                    },
                    notEmpty: {
                        message: 'Please enter your First Name'
                    },
                    regexp: {
                        regexp: /^[A-Z][a-z]+$/, 
                        message: 'The full name can consist of alphabetical characters only and start form upper letter'
                    }
                }
            },
            LastName: {
                validators: {
                    stringLength: {
                        min: 2,
                    },
                    notEmpty: {
                        message: 'Please enter your Last Name'
                    },
                    regexp: {
                        regexp: /^[A-Z][a-z]+$/,
                        message: 'The last name can consist of alphabetical characters only and start form upper letter'
                    }
                }
            },
            Username: {
                validators: {
                    stringLength: {
                        min: 4,
                    },
                    notEmpty: {
                        message: 'Please enter your Username'
                    }
                }
            },
            Password: {
                validators: {
                    stringLength: {
                        min: 6,
                    },
                    notEmpty: {
                        message: 'Please enter your Password'
                    }
                }
            },
            ConfirmPassword: {
                validators: {
                    stringLength: {
                        min: 6,
                    },
                    notEmpty: {
                        message: 'Please confirm your Password'
                    },
                    identical: {
                        field: 'Password',
                        message: 'The password and its confirm are not the same'
                    }
                }
            },
            Email: {
                validators: {
                    notEmpty: {
                        message: 'Please enter your Email Address'
                    },
                    emailAddress: {
                        message: 'Please enter a valid Email Address'
                    }
                }
            },
            PhoneNumber: {
                validators: {
                    stringLength: {
                        min: 9,
                        max: 9,
                        notEmpty: {
                            message: 'Please enter your Phone number.'
                        }
                    },
                    regexp: {
                        regexp: /^[0-9]*$/,
                        message: 'The phone number can consist of numerical characters only'
                    }
                },
            }
        }
    })
        //.on('success.form.bv', function (e) {
        //    //$('#success_message').slideDown({ opacity: "show" }, "slow") // Do something ...
        //    $('#contact_form').data('bootstrapValidator').resetForm();

        //    // Prevent form submission
        //    //e.preventDefault();

        //    // Get the form instance
        //    var $form = $(e.target);

        //    // Get the BootstrapValidator instance
        //    var bv = $form.data('bootstrapValidator');

        //    // Use Ajax to submit form data
        //    $.post($form.attr('action'), $form.serialize(), function (result) {
        //        console.log(result);
        //    }, 'json');
        //});
});