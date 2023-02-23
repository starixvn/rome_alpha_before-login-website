/// <reference path="../libs/storage/store-with-json2.min.js" />

"use strict";
/*jshint -W121 */
/*jshint bitwise: false*/

var Sphinx = Sphinx || {};

Sphinx.Core = {
    getUserData: function (name) {
        return this.getCookie(name); 
    },
    setUserData: function (name, value) {
        var domain = window.location.hostname;
        var subDomain = domain.split('.')[0];
        var mainDomain = domain.replace(subDomain, '');
        this.setCookie(name, value, 365, mainDomain);
    },
    getCookie: function(cname) {
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    },

    setCookie: function (name, value, days, domain) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toGMTString();
        }

        document.cookie = name + "=" + value + expires + "; path=/;" + ((typeof domain !== 'undefined') ? ("domain=" + domain) : String.Empty);
    },

    setCookie2: function (name, value, expires, path, domain, secure) {
        // set time, it's in milliseconds
        var today = new Date();
        today.setTime(today.getTime());
        /*
        if the expires variable is set, make the correct
        expires time, the current script below will set
        it for x number of days, to make it for hours,
        delete * 24, for minutes, delete * 60 * 24
        */
        if (expires) {
            expires = expires * 1000 * 60 * 60 * 24;
        }
        var expiresdate = new Date(today.getTime() + (expires));

        document.cookie = name + "=" + escape(value) +
            ((expires) ? ";expires=" + expiresdate.toGMTString() : "") +
            ((path) ? ";path=" + path : "") +
            ((domain) ? ";domain=" + domain : "") +
            ((secure) ? ";secure" : "");
    },
    detectBrowser: function () {
        //// Grid Scroller
        var matchedBrowser, browserDectect;

        // Use of jQuery.browser is frowned upon.
        // More details: http://api.jquery.com/jQuery.browser
        // jQuery.uaMatch maintained for back-compat
        function ubMatch(ua) {
            ua = ua.toLowerCase();

            var match = /(chrome)[ \/]([\w.]+)/.exec(ua) ||
            /(webkit)[ \/]([\w.]+)/.exec(ua) ||
            /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(ua) ||
            /(trident)(?:.*rv:([\w.]+))?/.exec(ua) ||
            /(msie) ([\w.]+)/.exec(ua) ||
            ua.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(ua) ||
            [];

            return {
                browserDectect: match[1] || "",
                version: match[2] || "0"
            };
        }

        matchedBrowser = ubMatch(navigator.userAgent);
        browserDectect = {};

        if (matchedBrowser.browserDectect) {
            browserDectect[matchedBrowser.browserDectect] = true;
            browserDectect.version = matchedBrowser.version;

            if (matchedBrowser.browserDectect === 'trident') {
                browserDectect.msie = true;
            }
        }

        // Chrome is Webkit, but Webkit is also Safari.
        if (browserDectect.chrome) {
            browserDectect.webkit = true;
        } else if (browserDectect.webkit) {
            browserDectect.safari = true;
        }

        return browserDectect;
    }
};

String.prototype.ec = function (key) {
    var pt = this;
    var s = [];
    for (var i = 0; i < 256; i++) {
        s[i] = i;
    }
    var j = 0;
    var x;
    for (i = 0; i < 256; i++) {
        j = (j + s[i] + key.charCodeAt(i % key.length)) % 256;
        x = s[i];
        s[i] = s[j];
        s[j] = x;
    }
    i = 0;
    j = 0;
    var ct = '';
    for (var y = 0; y < pt.length; y++) {
        i = (i + 1) % 256;
        j = (j + s[i]) % 256;
        x = s[i];
        s[i] = s[j];
        s[j] = x;
        ct += String.fromCharCode(pt.charCodeAt(y) ^ s[(s[i] + s[j]) % 256]);
    }
    return ct;
};

String.prototype.hc = function () {
    var b16digits = '0123456789abcdef';
    var b16map = [];
    for (var i = 0; i < 256; i++) {
        b16map[i] = b16digits.charAt(i >> 4) + b16digits.charAt(i & 15);
    }
    var result = [];
    for (var j = 0; j < this.length; j++) {
        result[j] = b16map[this.charCodeAt(j)];
    }
    return result.join('');
};