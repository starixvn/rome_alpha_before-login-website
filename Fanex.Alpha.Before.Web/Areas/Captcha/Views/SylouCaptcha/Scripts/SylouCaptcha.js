"use strict";

var syloucaptcha;

var Syloucaptcha = function () {
    var self = this;
    var _elements =
    {
        maskedWindow: null,
        txtPageName: null
    };

    self.Init = function () {
        _elements.maskedWindow = jQuery('#frCaptcha', window.parent.document);
    };

    self.ShowPopup = function () {
        _elements.maskedWindow.show();
    };

    self.ClosePopup = function () {
        try {
            _elements.maskedWindow.remove();
        } catch (e) {

        }

    };
};

jQuery(document).ready(function () {
    syloucaptcha = new Syloucaptcha();
    syloucaptcha.Init();
    if (jQuery('#ValidateSuccess').val() === "True") {
        syloucaptcha.ClosePopup();
    }
    else {
        syloucaptcha.ShowPopup();
    }
});