"use strict";
/*jshint -W121 */
/*jshint bitwise: false*/

var Sphinx = Sphinx || {};

Sphinx.Core = {
    getCookie: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === ' ') {
                c = c.substring(1, c.length);
            }
            if (c.indexOf(nameEQ) === 0) {
                return c.substring(nameEQ.length, c.length);
            }
        }
        return null;
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