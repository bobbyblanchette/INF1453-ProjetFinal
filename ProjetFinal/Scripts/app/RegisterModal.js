$(document).ready(function () {
    var registerLink = $("a[id='registerLink']");
    registerLink.attr({ "href": "#", "data-toggle": "modal", "data-target": "#ModalRegister" });

    $("#registerform").submit(function (event) {
        if ($("#registerform").valid()) {
            var username = $("#Email").val();
            var password = $("#Password").val();
            var rememberme = $("#RememberMe").val();
            var antiForgeryToken = Sample.Web.ModalRegister.Views.Common.getAntiForgeryValue();

            Sample.Web.ModalRegister.Identity.RegisterIntoStd(username, password, rememberme, antiForgeryToken, Sample.Web.ModalRegister.Views.RegisterModal.registerSuccess, Sample.Web.ModalRegister.Views.RegisterModal.registerFailure);
        }
        return false;
    });

    $("#ModalRegister").on("hidden.bs.modal", function (e) {
        Sample.Web.ModalRegister.Views.RegisterModal.resetRegisterForm();
    });

    $("#ModalRegister").on("shown.bs.modal", function (e) {
        $("#Email").focus();
    });

});

var Sample = Sample || {};
Sample.Web = Sample.Web || {};
Sample.Web.ModalRegister = Sample.Web.ModalRegister || {};
Sample.Web.ModalRegister.Views = Sample.Web.ModalRegister.Views || {}

Sample.Web.ModalRegister.Views.Common = {
    getAntiForgeryValue: function () {
        return $('input[name="__RequestVerificationToken"]').val();
    }
}

Sample.Web.ModalRegister.Views.RegisterModal = {
    resetRegisterForm: function () {
        $("#registerform").get(0).reset();
        $("#alertBox").css("display", "none");
    },

    registerFailure: function (message) {
        var alertBox = $("#alertBox");
        alertBox.html(message);
        alertBox.css("display", "block");
    },

    registerSuccess: function () {
        window.location.href = window.location.href;
    }
}


Sample.Web.ModalRegister.Identity = {
    RegisterIntoStd: function (username, password, rememberme, antiForgeryToken, successCallback, failureCallback) {
        var data = { "__RequestVerificationToken": antiForgeryToken, "username": username, "password": password, "rememberme": rememberme };

        $.ajax({
            url: "/Account/RegisterJson",
            type: "POST",
            data: data
        })
        .done(function (registerSuccessful) {
            if (registerSuccessful) {
                successCallback();
            }
            else {
                failureCallback("Invalid register attempt.");
            }
        })
        .fail(function (jqxhr, textStatus, errorThrown) {
            failureCallback(errorThrown);
        });
    }
}
