"use strict";
/*global UAParser */

var Sphinx = Sphinx || {};

Sphinx.BrowserSuggestion = (function ($) {
    var $container;

    var browserName;
    var browserVersion;

    var needToUpdate = false;

    var updateLinkIe = "http://windows.microsoft.com/{0}/internet-explorer/download-ie";
    //var updateLinkIe = "https://www.microsoft.com/{0}/download/internet-explorer.aspx";
    //var updateLinkIe = "http://windows.microsoft.com/{0}/windows/upgrade-your-browser";
    var updateLinkFireFox = "https://www.mozilla.org/{0}/firefox/new/";

    var init = function () {
        var parser = new UAParser();

        //var uastring = "Mozilla/5.0 (Windows NT 5.1; rv:2.0) Gecko/20100101 Firefox/4.0";
        //var uastring = "Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        //parser.setUA(uastring);

        var result = parser.getResult();

        browserName = result.browser.name;
        browserVersion = result.browser.major;

        if (result.os.name.toLowerCase() === "windows") {
            if (browserName.toLowerCase() === "ie" && browserVersion <= 7
                || browserName.toLowerCase() === "firefox" && browserVersion <= 30) {

                needToUpdate = true;
            }
        }

        if (needToUpdate === false) {
            $container.remove();
        }
        else {
            $container.show();
        }
    };

    var format = function () {
        var str = arguments[0];
        for (var i = 0; i < arguments.length - 1; i++) {
            var reg = new RegExp("\\{" + i + "\\}", "gm");
            str = str.replace(reg, arguments[i + 1]);
        }
        return str;
    };

    var updateMessage = function (event, languageCode) {
        try {
            if (needToUpdate === true) {
                var language = Sphinx.Resources.getLanguage(languageCode);

                $container.find(".warning-message").html(language.browserWarning);

                var updateLink = $container.find(".update-link");
                updateLink.text(language.updateYourBrowser);

                if (browserName.toLowerCase() === "ie") {
                    updateLink.attr("href", format(updateLinkIe, "en-US"));
                }
                else if (browserName.toLowerCase() === "firefox") {
                    updateLink.attr("href", format(updateLinkFireFox, language.Id));
                }
            }
        } catch (e) {
            console.log("Browser Suggestion: " + e);
        }
    };

    var suggest = function ($containerElement) {
        try {
            $container = $containerElement;
            init();

            $(document).on("Sphinx.Login.LanguageChanged", updateMessage);
        } catch (e) {
            console.log("Browser Suggestion: Failed to init!");
        }
    };

    return {
        suggest: suggest
    };

})(jQuery);

$(document).ready(function () {
    Sphinx.BrowserSuggestion.suggest($(".browser-warning"));
});