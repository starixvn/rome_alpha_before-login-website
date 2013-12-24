var Sphinx = Sphinx || {};

Sphinx.rootUrl = '';

Sphinx.Error = {
    InvalidUserAndPass: '1',
    InvalidCaptcha: '2',

    clientError: function (error) {

        if (error == Sphinx.Error.InvalidUserAndPass || error == Sphinx.Error.InvalidCaptcha) return true;

        return false;

    }

};

Sphinx.Helper = new function () {

    this.browserStandardMode = function () {
        return document.compatMode == 'CSS1Compat';
    };


};

Sphinx.Login = {
    currentLanguageCode: 'en-US',
    init: function () {
        Sphinx.rootUrl = window.rootUrl;
        var language = Sphinx.Core.getCookie('LANGUAGE');
        if (!language) language = _page.language;

        if (language == 0) language = Sphinx.Login.currentLanguageCode;
        Sphinx.Login.setLanguague(language, true);
        //Create dropdownbox
        Sphinx.Login.createDropdownLanguage(language);

        Sphinx.Login.initComponent();
    },

    initComponent: function () {
        $('#txtPassWord').focus(function () {
            if ($('#chbShowPass').is(':checked')) return;
            Sphinx.Login.setPassword($(this));

        });

        $('#txtPassWord').blur(function () {
            Sphinx.Login.resetPassword($(this));
        });

        $('#txtCaptcha').keydown(function (evt) {
            Sphinx.Login.doEnter(evt, 'Sphinx.Login.doLogin()');
        });


        //Adjust component for Quirk mode
        if (!Sphinx.Helper.browserStandardMode()) {
            $('#txtCaptcha').height('22.5px');
            $('#lbLanguage').css('color', '#B96643');
            $('#lbLanguage').css('font-size', '13px');
            $('.showPass').css('top', '1px');
        }

        if ($.browser.msie) {
            $('#chbShowPass').css('top', '-2px');
        }


    },

    getValidateImage: function () {
        var objDate = new Date();
        $("#imgCode").attr('src', Sphinx.rootUrl + 'Login/GetNewCaptcha?code=' + objDate.getMilliseconds());
        setTimeout(Sphinx.Login.getValidateImage, 120000);
    },

    setLanguague: function (languageCode, firstLoad) {

        if (!firstLoad) {
            $('#errmsg').html('');
            $('#divPopupMobileVersion').html('');
        }

        var flag = $("#flag");
        var language = Sphinx.Resources;
        var resource = language.getLanguage(languageCode);

        var error = $('#errmsg').attr('errCode');
        if (Sphinx.Error.clientError(error)) {
            $('#errmsg').html(Sphinx.Login.getErrorMessage(error, resource));
        }

        var ctrUsername = $('#txtUserName');
        var currentUsername = ctrUsername.attr('placeholder');
        ctrUsername.attr('placeholder', resource.Username);
        if ($.browser.msie && ctrUsername.val() == currentUsername) {
            ctrUsername.val(resource.Username);
        }

        var strPassword = $('#txtPassWord');
        var currentPlaceholder = strPassword.attr('placeholder');
        strPassword.attr('placeholder', resource.Password);
        if ($.browser.msie && strPassword.val() == currentPlaceholder) {
            strPassword.val(resource.Password);
        }

        flag.removeClass();
        flag.addClass(resource.Flag);
        $('#lbLanguage').html(resource.Language);

        $('#hidLanguage').val(languageCode);

        $('#lbShowPass').html(resource.ShowPassword);

        $('#textTitle').removeClass();
        $('#textTitle').addClass("span6 " + resource.AutumnTitle); 
        $('.btnLogin').val(resource.Login);

        Sphinx.Core.setCookie('LANGUAGE', languageCode, 30);
        Sphinx.Login.currentLanguageCode = languageCode;
    },

    showPassword: function (chkShowPass) {
        var currentInputPass = $('#txtPassWord');
        if (currentInputPass.val() == currentInputPass.attr('placeholder')) {
            return;
        }
        if (!chkShowPass.checked) {
            Sphinx.Login.setPassword(currentInputPass);
        }
        else {
            Sphinx.Login.resetPassword(currentInputPass);
        }
    },

    setPassword: function ($input) {
        var valPlaceholder = $input.attr('placeholder');
        var inputPassword = $('<input type="password" id="txtPassWord" name="txtPassWord" tabindex="2" placeholder="' + valPlaceholder + '"  class="custom-textbox password"  value="' + $input.val() + '" />');
        $input.replaceWith(inputPassword);
        inputPassword.focus();
        if ($.browser.msie) {
            inputPassword.blur(function () {
                if (inputPassword.val() == '') {
                    Sphinx.Login.resetPassword(inputPassword);
                }
            });
        }
    },


    resetPassword: function ($input) {
        var valPlaceholder = $input.attr('placeholder');
        var inputPassword = $('<input type="text" id="txtPassWord" name="txtPassWord" tabindex="2" placeholder="' + valPlaceholder + '" class="custom-textbox password" value="' + $input.val() + '"  />');
        $input.replaceWith(inputPassword);
        if ($.browser.msie) {
            if (inputPassword.val() == '') {
                inputPassword.val(inputPassword.attr('placeholder')).addClass('placeholder');
            }

            inputPassword.focus(function () {
                if (inputPassword.val() == inputPassword.attr('placeholder')) {
                    inputPassword.val('').removeClass('placeholder');
                    if ($('#chbShowPass').is(':checked')) return;
                    Sphinx.Login.setPassword(inputPassword);
                }
            });

            inputPassword.blur(function () {
                if (inputPassword.val() == '') {
                    inputPassword.val(inputPassword.attr('placeholder')).addClass('placeholder');
                }
            });
        }

    },

    validateInput: function () {
        var languageCode = Sphinx.Login.currentLanguageCode;
        var language = Sphinx.Resources.getLanguage(languageCode);
        if ($('#txtUserName').val() == "" || $('#txtUserName').val() == language.Username) {
            $("#errmsg").html(language.errUserName);
            $('#txtUserName').focus();
            $('#errmsg').attr('errCode', Sphinx.Error.InvalidUserAndPass);
            return false;
        }
        if ($('#txtPassWord').val() == "" || $('#txtPassWord').val() == language.Password) {
            $("#errmsg").html(language.errUserName);
            $('#txtPassWord').focus();
            $('#errmsg').attr('errCode', Sphinx.Error.InvalidUserAndPass);
            return false;
        }
        if ($('#txtCaptcha').val() == "") {
            $("#errmsg").html(language.errValidCode);
            $('#txtCaptcha').focus();
            $('#errmsg').attr('errCode', Sphinx.Error.InvalidCaptcha);
            return false;
        }
        return true;
    },

    doEnter: function (evt, action) {
        if ($.browser.msie) {
            if (window.event.keyCode == 13) eval(action);
        } else {
            if (evt.keyCode == 13) eval(action);
        }
    },

    doLogin: function () {
        if (!Sphinx.Login.validateInput()) return false;
        Sphinx.Login.blockComponent();

        $('#txtPassWord').val($('#txtPassWord').val().ec(_page.captcha).hc());

        try {
            Sphinx.Core.setCookie2("__utms", _page.__utms, 1, "/", "." + _page.domain, "");
        } catch (exception) {
        }

        Sphinx.Core.setCookie("ssl", location.href.indexOf('https://') == 0, 30);

        document.forms[0].submit();

        return true;
    },

    blockComponent: function () {
        // $('#txtCaptcha').attr('disabled', true);
        $('#btnLogin').attr('disabled', true);
        var btnLogin = $('#btnLogin');
        var loading = $('#imgLoading');
        btnLogin.removeClass();
        btnLogin.addClass('btnLoginDisable');
        loading.show();
    },

    resetComponent: function () {
        $('#txtPassWord').val('');
        Sphinx.Login.getValidateImage();
        var captcha = $('#txtCaptcha');
        captcha.val('');
        captcha.attr('disabled', false);
        $('#imgLoading').hide();
        var btnLogin = $('#btnLogin');
        btnLogin.attr('disabled', false);
        btnLogin.removeClass();
        btnLogin.addClass('btnLogin');

    },

    createDropdownLanguage: function (selectedValue) {
        var listItem = new Array();
        var listLanguage = Sphinx.Resources.languages();
        var totalItem = listLanguage.length;
        function dropDownItem(value, img, text) {
            this.value = value;
            this.img = img;
            this.text = text;
        }

        var selectedItem = null;

        for (var i = 0; i < totalItem; i++) {
            var item = listLanguage[i];
            if (item.Id == selectedValue)
                selectedItem = new dropDownItem(item.Id, item.Flag, item.Language);
            listItem.push(new dropDownItem(item.Id, item.Flag, item.Language));
        }

        Sphinx.Component.DropDownbox.create('ddLanguage', listItem, selectedItem, Sphinx.Login.setLanguague);
    },

    getErrorMessage: function (errorCode, resource) {
        switch (errorCode) {
            case Sphinx.Error.InvalidUserAndPass:
                return resource.errUserName;
            case Sphinx.Error.InvalidCaptcha:
                return resource.errValidCode;
        }
        return '';
    }
};


$(document).ready(function () {
    Sphinx.Login.init();
});