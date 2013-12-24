var notSupportedBrowsers = [{
    'os': 'Any',
    'browser': 'MSIE',
    'version': 8
}, {
    'os': 'Any',
    'browser': 'Firefox',
    'version': 7
}, {
    'os': 'Any',
    'browser': 'Safari',
    'version': 5
}, {
    'os': 'Any',
    'browser': 'Opera',
    'version': 12
}, {
    'os': 'Any',
    'browser': 'Chrome',
    'version': 15
}];
var BrowserDetection = {
    init: function () {
        if (notSupportedBrowsers == null || notSupportedBrowsers.length < 1) {
            notSupportedBrowsers = this.defaultNotSupportedBrowsers;
        }
        this.detectBrowser();
        this.detectOS();
        if (this.browser == '' || this.browser == 'Unknown' || this.os == '' || this.os == 'Unknown' || this.browserVersion == '' || this.browserVersion == 0) {
            return;
        }
        var oldBrowser = false;
        for (var i = 0; i < notSupportedBrowsers.length; i++) {
            if (notSupportedBrowsers[i].os == 'Any' || notSupportedBrowsers[i].os == this.os) {
                if (notSupportedBrowsers[i].browser == 'Any' || notSupportedBrowsers[i].browser == this.browser) {
                    if (notSupportedBrowsers[i].version == "Any" || this.browserVersion <= parseFloat(notSupportedBrowsers[i].version)) {
                        oldBrowser = true;
                        break;
                    }
                }
            }
        }
        if (oldBrowser) {
            this.displayNotice();
        }
    },
    getEl: function (id) {
        return window.document.getElementById(id);
    },
    getElSize: function (id) {
        var el = this.getEl(id);
        if (el == null) {
            return null;
        }
        return {
            'width': parseInt(el.offsetWidth),
            'height': parseInt(el.offsetHeight)
        };
    },
    getWindowSize: function () {
        if (typeof window.innerWidth != 'undefined') {
            return {
                'width': parseInt(window.innerWidth),
                'height': parseInt(window.innerHeight)
            };
        } else {
            if (window.document.documentElement.clientWidth != 0) {
                return {
                    'width': parseInt(window.document.documentElement.clientWidth),
                    'height': parseInt(window.document.documentElement.clientHeight)
                };
            } else {
                return {
                    'width': parseInt(window.document.body.clientWidth),
                    'height': parseInt(window.document.body.clientHeight)
                };
            }
        }
    },
    positionNotice: function () {
        var noticeSize = this.getElSize('browser-detection');
        var windowSize = this.getWindowSize();
        var noticeEl = this.getEl('browser-detection');
        if (noticeEl == null || noticeSize == null || windowSize == null || !windowSize.width || !windowSize.height) {
            return;
        }
        noticeEl.style.left = (windowSize.width - noticeSize.width) / 2 + "px";
        var offset = (this.browser == "MSIE" && this.browserVersion < 7) ? (window.document.documentElement.scrollTop != 0 ? window.document.documentElement.scrollTop : window.document.body.scrollTop) : 0;
        this.noticeHeight = noticeSize.height;
    },
    displayNotice: function () {
        if (this.readCookie('outdatedBrower') == 1) {
            return;
        }
        this.writeNoticeCode();
        this.positionNotice();
        var el = this;
        window.onresize = function () {
            el.positionNotice();
        };
        if (this.browser == "MSIE" && this.browserVersion < 7) {
            window.onscroll = function () {
                el.positionNotice();
            };
        }
        this.getEl('browser-detection-close').onclick = function () {
            el.remindMe(false);
        };
        this.getEl('remindLater').onclick = function () {
            el.remindMe(false);
        };
        this.getEl('neverRemind').onclick = function () {
            el.remindMe(true);
        };
    },
    remindMe: function (never) {
        this.writeCookie('outdatedBrower', 1, never == true ? 30 : 1);
        this.getEl('browser-detection').style.display = 'none';
        this.getEl('black_overlay').style.display = 'none';
    },
    writeCookie: function (name, value, days) {
        var expiration = "";
        if (parseInt(days) > 0) {
            var date = new Date();
            date.setTime(date.getTime() + parseInt(days) * 24 * 60 * 60 * 1000);
            expiration = '; expires=' + date.toGMTString();
        }
        document.cookie = name + '=' + value + expiration + '; path=/';
    },
    readCookie: function (name) {
        if (!document.cookie) {
            return '';
        }
        var searchName = name + '=';
        var data = document.cookie.split(';');
        for (var i = 0; i < data.length; i++) {
            while (data[i].charAt(0) == ' ') {
                data[i] = data[i].substring(1, data[i].length);
            }
            if (data[i].indexOf(searchName) == 0) {
                return data[i].substring(searchName.length, data[i].length);
            }
        }
        return '';
    },
    writeNoticeCode: function () {
        var arrRemindMeLater = new Array("Remind me later", "Remind me later", "Remind me later", "Remind me later", "Remind me later", "Remind me later");
        var arrNeverRemindAgain = new Array("No, don't remind me again", "No, don't remind me again", "No, don't remind me again", "No, don't remind me again", "No, don't remind me again", "No, don't remind me again");
        var arrTitle = new Array("Outdated Browser Detected", "Outdated Browser Detected", "Outdated Browser Detected", "Outdated Browser Detected", "Outdated Browser Detected", "Outdated Browser Detected");
        var arrNotice = new Array("Our website has detected that you are using an outdated browser. Using your current browser will prevent you from accessing features on our website. An upgrade is not required, but is strongly recommended to improve your browsing experience on our website. <a href='{browser}' class='linkUpgrade' title='Please, upgrade your browser.' target='_blank'>Please, upgrade your browser.</a>", "Our website has detected that you are using an outdated browser. Using your current browser will prevent you from accessing features on our website. An upgrade is not required, but is strongly recommended to improve your browsing experience on our website. <a href='{browser}' class='linkUpgrade' title='Please, upgrade your browser.' target='_blank'>Please, upgrade your browser.</a>", "Our website has detected that you are using an outdated browser. Using your current browser will prevent you from accessing features on our website. An upgrade is not required, but is strongly recommended to improve your browsing experience on our website. <a href='{browser}' class='linkUpgrade' title='Please, upgrade your browser.' target='_blank'>Please, upgrade your browser.</a>", "Our website has detected that you are using an outdated browser. Using your current browser will prevent you from accessing features on our website. An upgrade is not required, but is strongly recommended to improve your browsing experience on our website. <a href='{browser}' class='linkUpgrade' title='Please, upgrade your browser.' target='_blank'>Please, upgrade your browser.</a>", "Our website has detected that you are using an outdated browser. Using your current browser will prevent you from accessing features on our website. An upgrade is not required, but is strongly recommended to improve your browsing experience on our website. <a href='{browser}' class='linkUpgrade' title='Please, upgrade your browser.' target='_blank'>Please, upgrade your browser.</a>", "Our website has detected that you are using an outdated browser. Using your current browser will prevent you from accessing features on our website. An upgrade is not required, but is strongly recommended to improve your browsing experience on our website. <a href='{browser}' class='linkUpgrade' title='Please, upgrade your browser.' target='_blank'>Please, upgrade your browser.</a>");
        var language = getCookie('LANGUAGE');
        if (!language) language = _page.language;
        var remindMeLater = arrRemindMeLater[language];
        var neverRemindAgain = arrNeverRemindAgain[language];
        var title = arrTitle[language];
        var notice = arrNotice[language];
        var urlUpgrade = null;
        switch (this.browser.toLowerCase()) {
        case "opera":
            urlUpgrade = "http://cloakedlink.com/www.opera.com/";
            break;
        case "msie":
            urlUpgrade = "http://cloakedlink.com/www.getie.com/";
            break;
        case "chrome":
            urlUpgrade = "http://cloakedlink.com/www.google.com/chrome/";
            break;
        case "safari":
            urlUpgrade = "http://cloakedlink.com/www.apple.com/safari/";
            break;
        case "firefox":
            urlUpgrade = "http://cloakedlink.com/www.getfirefox.com/";
            break;
        }
        notice = notice.replace("{browser}", urlUpgrade);
        var code = '<div id="black_overlay"></div><div id="browser-detection"><div class="bgtop"></div><div class="bg"><a href="javascript:;" id="browser-detection-close"></a>';
        code += '<div class="float_l" style="width:58px; padding-top:42px"><div class="iconWarning"></div></div>';
        code += '<div class="float_l" style="width:470px; padding-left:12px">';
        code += '<p class="bd-title">' + title + '</p><p class="bd-notice">' + notice + '</p>';
        code += '<ul class="buttons">';
        code += '<li><div id="remindLater" class="buttonRemind"><a class="remind" href="javascript:;">' + remindMeLater + '</a></div></li>';
        code += '<li><div id="neverRemind" class="buttonNotRemind"><a class="remind" href="javascript:;">' + neverRemindAgain + '</a></div></li>';
        code += '</ul>';
        code += '</div>';
        code += '<div class="clear"></div>';
        code += '</div>';
        code += '<div class="bgbottom"></div>';
        code += '</div>';
        window.document.body.innerHTML += code;
    },
    detectBrowser: function () {
        this.browser = '';
        this.browserVersion = 0;
        if (/Opera[\/\s](\d+\.\d+)/.test(navigator.userAgent)) {
            this.browser = 'Opera';
        } else if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
            this.browser = 'MSIE';
        } else if (/Navigator[\/\s](\d+\.\d+)/.test(navigator.userAgent)) {
            this.browser = 'Netscape';
        } else if (/Chrome[\/\s](\d+\.\d+)/.test(navigator.userAgent)) {
            this.browser = 'Chrome';
        } else if (/Safari[\/\s](\d+\.\d+)/.test(navigator.userAgent)) {
            this.browser = 'Safari';
            /Version[\/\s](\d+\.\d+)/.test(navigator.userAgent);
            this.browserVersion = new Number(RegExp.$1);
        } else if (/Firefox[\/\s](\d+\.\d+)/.test(navigator.userAgent)) {
            this.browser = 'Firefox';
        }
        if (this.browser == '') {
            this.browser = 'Unknown';
        } else if (this.browserVersion == 0) {
            this.browserVersion = parseFloat(new Number(RegExp.$1));
        }
    },
    detectOS: function () {
        for (var i = 0; i < this.operatingSystems.length; i++) {
            if (this.operatingSystems[i].searchString.indexOf(this.operatingSystems[i].subStr) != -1) {
                this.os = this.operatingSystems[i].name;
                return;
            }
        }
        this.os = "Unknown";
    },
    noticeHeight: 0,
    browser: '',
    os: '',
    browserVersion: '',
    operatingSystems: [{
        'searchString': navigator.platform,
        'name': 'Windows',
        'subStr': 'Win'
    }, {
        'searchString': navigator.platform,
        'name': 'Mac',
        'subStr': 'Mac'
    }, {
        'searchString': navigator.platform,
        'name': 'Linux',
        'subStr': 'Linux'
    }, {
        'searchString': navigator.userAgent,
        'name': 'iPhone',
        'subStr': 'iPhone/iPod'
    }]
};
/*window.onload = function () {
    BrowserDetection.init();
};*/