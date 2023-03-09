/// <reference path="core.js" />

"use strict";
/* global _page */
/* jshint unused: vars */

var Sphinx = Sphinx || {};

Sphinx.rootUrl = '';

Sphinx.Error = {
    InvalidUserAndPass: '1',
    InvalidCaptcha: '2',

    clientError: function (error) {
        if (error === Sphinx.Error.InvalidUserAndPass ||
            error === Sphinx.Error.InvalidCaptcha) {
            return true;
        }

        return false;
    }
};

Sphinx.Helper = (function () {
    return {
        browserStandardMode: function () {
            return document.compatMode === 'CSS1Compat';
        }
    };
})();

Sphinx.Login = {
    currentLanguageCode: 'en-US',
    is789y: "0",

    init: function () {
        this.is789y = $("#is789y").val();

        Sphinx.Login.browser = Sphinx.Core.detectBrowser();
        Sphinx.Login.cachedDomElements();
        Sphinx.rootUrl = window.rootUrl;
        var language = Sphinx.Core.getUserData('LANGUAGE');
        if (!language) {
            language = _page.language;
        }

        if (parseInt(language) === 0) {
            language = Sphinx.Login.currentLanguageCode;
        }

        if (this.is789y === "0" && language === "vi-VN") {
            language = this.currentLanguageCode;
            Sphinx.Core.setUserData("LANGUAGE", language);
        }

        Sphinx.Login.setLanguague(language, true);
        Sphinx.Login.initComponent();
        Sphinx.Login.clearBrowserBackHistory();
    },
    domElements: {
        ddlLanguages: null,
        formTitle: null,
        txtUsername: null,
        txtPassword: null,
        chbShowPassword: null,
        btnLogin: null,
        txtCaptcha: null,
        imgCode: null,
        errorMsg: null,
        lblLanguage: null,
        flag: null,
        hidLanguage: null,
        languageItems: null,
        lblShowPass: null,
        formLogin: null,
        divTechnicalErrorMsg: null,
        technicalErrorTime: null,
        iconSwitchMode: null
    },
    cachedDomElements: function () {
        Sphinx.Login.domElements.txtUsername = $('#txtUserName');
        Sphinx.Login.domElements.txtPassword = $('#txtPassWord');
        Sphinx.Login.domElements.chbShowPassword = $('#chbShowPass');
        Sphinx.Login.domElements.txtCaptcha = $('#txtCaptcha');
        Sphinx.Login.domElements.imgCode = $('#imgCode');
        Sphinx.Login.domElements.errorMsg = $('#errmsg');
        Sphinx.Login.domElements.lblLanguage = $('#lblLanguage');
        Sphinx.Login.domElements.flag = $("#flag");
        Sphinx.Login.domElements.hidLanguage = $('#hidLanguage');
        Sphinx.Login.domElements.languageItems = $('#language-ddl a');
        Sphinx.Login.domElements.btnLogin = $('#btnLogin');
        Sphinx.Login.domElements.formTitle = $('.login-text');
        Sphinx.Login.domElements.lblShowPass = $('#lbShowPass');
        Sphinx.Login.domElements.formLogin = $('#formLogin');
        Sphinx.Login.domElements.divTechnicalErrorMsg = $('#divTechnicalErrorMsg');
        Sphinx.Login.domElements.technicalErrorTime = $('#hiddenTechnicalErrorTime').val();
        Sphinx.Login.domElements.iconSwitchMode = $('#icon-switch-mode');
    },
    initComponent: function () {
        var domElements = Sphinx.Login.domElements;

        $(document).keypress(function (evt) {
            if ((!Sphinx.Login.browser.msie && evt.keyCode === 13)
                || (Sphinx.Login.browser.msie && window.event.keyCode === 13)) {
                Sphinx.Login.doLogin();
            }
        });

        Sphinx.Login.getValidateImage();

        domElements.languageItems.click(function () {
            Sphinx.Login.setLanguague($(this).find('span').data('language-code'));
        });
        domElements.btnLogin.click(Sphinx.Login.doLogin);
        domElements.chbShowPassword.click(Sphinx.Login.showPassword);
        domElements.iconSwitchMode.click(Sphinx.Login.changeTheme);
    },

    getValidateImage: function () {
        var objDate = new Date();
        $("#imgCode").attr('src', Sphinx.rootUrl + 'NewCaptcha?code=' + objDate.getMilliseconds());
    },

    setLanguague: function (languageCode, firstLoad) {
        Sphinx.Core.setUserData("hidLanguage", languageCode);
        var domElements = Sphinx.Login.domElements;

        if (!firstLoad) {
            domElements.errorMsg.html('');
            $('#divPopupMobileVersion').html('');
        }

        var flag = domElements.flag;
        var language = Sphinx.Resources;
        var resource = language.getLanguage(languageCode);
        if (window.scInstance) {
            window.scInstance.changeLanguage(languageCode);
        }

        domElements.divTechnicalErrorMsg.html(resource.messageAlwayShowOnLoginPage.replace("{0}", domElements.technicalErrorTime));

        var error = domElements.errorMsg.attr('errCode');
        if (Sphinx.Error.clientError(error)) {
            domElements.errorMsg.html(Sphinx.Login.getErrorMessage(error, resource));
        }

        var ctrUsername = domElements.txtUsername;
        var currentUsername = ctrUsername.attr('placeholder');
        ctrUsername.attr('placeholder', resource.Username);
        if (Sphinx.Login.browser.msie && ctrUsername.val() === currentUsername) {
            ctrUsername.val(resource.Username);
        }

        var strPassword = domElements.txtPassword;
        var currentPlaceholder = strPassword.attr('placeholder');
        strPassword.attr('placeholder', resource.Password);
        if (Sphinx.Login.browser.msie && strPassword.val() === currentPlaceholder) {
            strPassword.val(resource.Password);
        }

        var strCaptcha = domElements.txtCaptcha;
        var currentCaptcha = strCaptcha.attr('placeholder');
        strCaptcha.attr('placeholder', resource.Captcha);
        if (Sphinx.Login.browser.msie && strCaptcha.val() === currentCaptcha) {
            strCaptcha.val(resource.Captcha);
        }

        flag.removeClass().addClass(resource.Flag);
        domElements.lblLanguage.html(resource.Language);

        domElements.hidLanguage.val(languageCode);
        domElements.lblShowPass.html(resource.ShowPassword);

        domElements.btnLogin.val(resource.Login);
        domElements.formTitle.text(resource.Login);
        document.title = resource.Login;

        Sphinx.Core.setUserData('LANGUAGE', languageCode);
        Sphinx.Login.currentLanguageCode = languageCode;

        $(document).trigger("Sphinx.Login.LanguageChanged", [languageCode]);
    },

    showPassword: function () {
        var currentInputPass = Sphinx.Login.domElements.txtPassword;
        if (currentInputPass.val() === currentInputPass.attr('placeholder')) {
            return;
        }

        if ($(this).is(':checked')) {
            Sphinx.Login.resetPassword(currentInputPass);
        }
        else {
            Sphinx.Login.setPassword(currentInputPass);
        }
    },

    setPassword: function ($input) {
        var valPlaceholder = $input.attr('placeholder');
        var inputPassword = $('<input type="password" id="txtPassWord" name="txtPassWord" tabindex="2" placeholder="' +
            valPlaceholder +
            '"  class="txt"  value="' +
            $('<div>').text($input.val()).html() + '" autocomplete="off"  />');
        $input.replaceWith(inputPassword);
        Sphinx.Login.domElements.txtPassword = inputPassword;

        inputPassword.focus();
        if (Sphinx.Login.browser.msie) {
            inputPassword.blur(function () {
                if (inputPassword.val() === '') {
                    Sphinx.Login.resetPassword(inputPassword);
                }
            });
        }
    },

    resetPassword: function ($input) {
        var valPlaceholder = $input.attr('placeholder');
        var inputPassword = $('<input type="text" id="txtPassWord" name="txtPassWord" tabindex="2" placeholder="' +
            valPlaceholder +
            '" class="txt" value="' +
            $('<div>').text($input.val()).html() +
            '" autocomplete="off"  />');

        $input.replaceWith(inputPassword);
        Sphinx.Login.domElements.txtPassword = inputPassword;

        if (Sphinx.Login.browser.msie) {
            if (inputPassword.val() === '') {
                inputPassword.val(inputPassword.attr('placeholder')).addClass('placeholder');
            }

            inputPassword.focus(function () {
                if (inputPassword.val() === inputPassword.attr('placeholder')) {
                    inputPassword.val('').removeClass('placeholder');
                    if (Sphinx.Login.domElements.chbShowPassword.is(':checked')) {
                        return;
                    }
                    Sphinx.Login.setPassword(inputPassword);
                }
            }).blur(function () {
                if (inputPassword.val() === '') {
                    inputPassword.val(inputPassword.attr('placeholder')).addClass('placeholder');
                }
            });
        }
    },

    validateInput: function () {
        var language = Sphinx.Resources.getLanguage(Sphinx.Login.currentLanguageCode);
        var domeElements = Sphinx.Login.domElements;
        if (domeElements.txtUsername.val() === ""
            || domeElements.txtUsername.val() === language.Username) {
            domeElements.errorMsg
                .attr('errCode', Sphinx.Error.InvalidUserAndPass)
                .html(language.errUserName)
                .show();

            domeElements.txtUsername.focus();

            return false;
        }

        if (domeElements.txtPassword.val() === ""
            || domeElements.txtPassword.val() === language.Password) {
            domeElements.errorMsg
                .html(language.errUserName)
                .attr('errCode', Sphinx.Error.InvalidUserAndPass)
                .show();

            domeElements.txtPassword.focus();

            return false;
        }

        if (domeElements.txtCaptcha.val() === "") {
            domeElements.errorMsg
                .html(language.errValidCode)
                .attr('errCode', Sphinx.Error.InvalidCaptcha)
                .show();

            domeElements.txtCaptcha.focus();

            return false;
        }

        return true;
    },

    doLogin: function () {
        if (!Sphinx.Login.validateInput()) {
            return false;
        }

        Sphinx.Login.blockComponent();
        Sphinx.Login.domElements.txtPassword.val(Sphinx.Login.encrypt(Sphinx.Login.domElements.txtPassword.val(), _page.captcha.toString()));

        var size = Sphinx.Login.getBrowserSize();
        $("#browserSize").val(size);
        $("#assetAppPath").val(Sphinx.AssetAppPaths.getAvailableAssetAppPaths());

        Sphinx.Login.domElements.formLogin.submit();

        return true;
    },

    encrypt: function (plainText, secretKey) {
        var passPhrase = 'a5s8d2e9c172';
        var key = CryptoJS.enc.Utf8.parse(passPhrase + secretKey);
        var iv = key;

        var pwd = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(plainText), key,
            {
                keySize: 128 / 8,
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

        return pwd;
    },

    blockComponent: function () {
        Sphinx.Login.domElements.btnLogin.attr('disabled', 'disabled');
    },

    getErrorMessage: function (errorCode, resource) {
        switch (errorCode) {
            case Sphinx.Error.InvalidUserAndPass:
                return resource.errUserName;

            case Sphinx.Error.InvalidCaptcha:
                return resource.errValidCode;
        }

        return '';
    },

    clearBrowserBackHistory: function () {
        if (window.history.pushState) {
            window.history.pushState(null, null, document.URL);
            window.addEventListener('popstate', function () {
                window.history.pushState(null, null, document.URL);
            });
        }
    },

    getBrowserSize: function () {
        var w = window,
            d = document,
            e = d.documentElement,
            g = d.getElementsByTagName('body')[0],
            x = w.innerWidth || e.clientWidth || g.clientWidth,
            y = w.innerHeight || e.clientHeight || g.clientHeight;

        return x + 'x' + y;
    },

    changeTheme: function () {
        const isDarkMode = $(this).hasClass("dark-mode");
        Sphinx.Core.setCookie("ISDARKMODE", isDarkMode ? "0" : "1", 365 * 100);
        window.location.href = Sphinx.Login.removeBookmarksInUrl(location.href);
    },

    removeBookmarksInUrl: function (url) {
        var urlLength = url.length;

        return url[urlLength - 1] === '#'
            ? url.substr(0, urlLength - 1)
            : url;
    }
};

$(document).ready(function () {
    Sphinx.AssetAppPaths.init();
    Sphinx.Login.init();
});
