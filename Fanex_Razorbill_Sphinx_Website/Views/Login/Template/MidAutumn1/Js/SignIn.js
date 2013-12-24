/*
* Created 20100113@Lee - Sign-in javascript functions
* Revision ?@? - ...
*
*/
var CurrentLanguageId = 0;

var LanguagesId = new Array(0, 1, 2, 3, 4, 5);
var Languages = new Array("en-US", "zh-TW", "ja-JP", "th-TH", "zh-CN", "ko-KR");
var Flags = new Array("fEnUS", "fZhTW", "fJaJP", "fThTH", "fZhCN", "fKoKR");
var LangName = new Array("English", "繁體中文", "日本語", "ภาษาไทย", "简体中文", "한국어");
var LanguageMessages = new Array({
    errUserName: "Please input username and password",
    errValidCode: "You must input validation code."
}, {
    errUserName: "請輸入會員名字及密碼",
    errValidCode: "您必须输入正确验证码"
}, {
    errUserName: "ユーザー名とパスワードを入力してください",
    errValidCode: "You must input validation code."
}, {
    errUserName: "โปรดใส่ชื่อผู้ใช้และรหัสผ่าน",
    errValidCode: "คุณต้องใส่หมายเลขที่ใช้ในการตรวจสอบ"
}, {
    errUserName: "请输入账号及密码",
    errValidCode: "您必須輸入正確驗證碼"
}, {
    errUserName: "사용자명과 비밀번호를 입력해주세요",
    errValidCode: "확인코드를 입력해주세요."
});

function SelectLanguage(value) {
    var flagBgX = 0;
    var flagWidth = 24
    if (value == LanguagesId[0]) // English
    {
        SetValue(value, 'User Name', 'Password', 'Login', 'Recommended Browsers', 'Validation');
        $('welcomeText').className ='english';
    } else if (value == LanguagesId[1]) // Chinese traditional
    {
        SetValue(value, '用户名', '密碼', '登入', 'Recommended Browsers', '驗證碼');
        flagBgX = flagWidth * 5;
        $('welcomeText').className = 'tradtionalChinese';       
    } else if (value == LanguagesId[4]) // Chinese simplified
    {
        SetValue(value, '用户名', '密码', '注册', 'Recommended Browsers', '验证码');
        flagBgX = flagWidth * 4;
        $('welcomeText').className = 'simplifiedChinese';
    } else if (value == LanguagesId[2]) // Japanese
    {
        SetValue(value, 'ユーザー名', 'パスワード', '入力', 'Recommended Browsers', 'Validation');
        flagBgX = flagWidth * 3;
        $('welcomeText').className = 'english';
    } else if (value == LanguagesId[3]) // Thailand
    {
        SetValue(value, 'ชื่อผู้ใช้ระบบ', 'รหัสผ่าน', 'เข้าระบบ', 'Recommended Browsers', 'รหัสตรวจสอบ');
        flagBgX = flagWidth * 2;
        $('welcomeText').className = 'english';
    } else if (value == LanguagesId[5]) // Korean
    {
        SetValue(value, '사용자 이름', '비밀번호', '로그인', 'Recommended Browsers', 'Validation');
        flagBgX = flagWidth * 1;
        $('welcomeText').className = 'english';
    }

    $('spnLanguage').style.backgroundPosition = flagBgX + 'px 0px';
    $("errmsg").innerHTML = '';
}

function SetValue(language, username, pwd, login, browser, captcha) {
    $('hidLanguage').value = Languages[language];
    $('spnUserName').innerHTML = username;
    $('spnPassWord').innerHTML = pwd;
    $('btnLogin').value = login;
    $('divBrowser').innerHTML = browser;
    $('spnCaptcha').innerHTML = captcha;

    $('selLanguage').innerHTML = LangName[language];
    $("cboLang").style.display = 'none';
    $('mainFlag').className = Flags[language];

    var domain = window.location.hostname.replace(/([a-zA-Z0-9]+.)/, '').replace('www.', '');
    setCookie('LANGUAGE', language, 30, domain);
    CurrentLanguageId = language;

    $('txtUserName').value = $('spnUserName').innerHTML;
    $('txtPassWord').value = $('spnPassWord').innerHTML;
    $('pass_temp').value = $('spnPassWord').innerHTML;
    $('txtCaptcha').value = $('spnCaptcha').innerHTML;
}

function DoLogin() {
    var errObj = LanguageMessages[CurrentLanguageId];
    if ($('txtUserName').value == "") {
        $("errmsg").innerHTML = errObj.errUserName;
        $('txtUserName').focus();
        return false;
    }
    if ($('txtPassWord').value == "") {
        $("errmsg").innerHTML = errObj.errUserName;
        $('txtPassWord').focus();
        return false;
    }
    if ($('txtCaptcha').value == "") {
        $("errmsg").innerHTML = errObj.errValidCode;
        $('txtCaptcha').focus();
        return false;
    }

    $('btnLogin').disabled = true;
    $('btnLogin').className = 'buttondisable';

    $('txtPassWord').value = $('txtPassWord').value.ec(_page.captcha).hc();

    try {
        setCookie2("__utms", _page.__utms, 1, "/", "." + _page.domain, "");
    } catch (exception) {
    }

    setCookie("ssl", location.href.indexOf('https://') == 0, 30);
    document.forms[0].submit();

    return true;
}

function GetValidateImage() {
    var objDate = new Date();
    $("imgCode").src = "Login/GetNewCaptcha?code=" + objDate.getMilliseconds();

    setTimeout(GetValidateImage, 120000);
}

function InitUserNamePwd() {
    $('txtUserName').value = $('spnUserName').innerHTML;
    $('txtPassWord').value = $('spnPassWord').innerHTML;

    $('txtUserName').onfocus = function () {
        if ($('txtUserName').value == $('spnUserName').innerHTML) {
            $('txtUserName').value = '';
        }
    }

    $('txtUserName').onblur = function () {
        if ($('txtUserName').value == '') {
            $('txtUserName').value = $('spnUserName').innerHTML;
        }
    }

    $('txtPassWord').onblur = function () {
        if ($('txtPassWord').value == '') {
            $('div1').style.display = '';
            $('div2').style.display = 'none';

            $('txtPassWord').value = $('spnPassWord').innerHTML;
        }
    }

    $('pass_temp').onfocus = function () {
        $('div1').style.display = 'none';
        $('div2').style.display = '';
        $('txtPassWord').focus();
        $('txtPassWord').value = '';
    }

    $('txtCaptcha').onfocus = function () {
        if ($('txtCaptcha').value == $('spnCaptcha').innerHTML) {
            $('txtCaptcha').value = '';
        }
    }

    $('txtCaptcha').onblur = function () {
        if ($('txtCaptcha').value == '') {
            $('txtCaptcha').value = $('spnCaptcha').innerHTML;
        }
    }

}

function InitLogin() {
    // Do not allow wrapping sign-in page
    if (window.parent != window) {
        window.parent.location = location.href;
    }

    var language = getCookie('LANGUAGE');
    if (!language) language = _page.language;

    // Map language code to language ID
    switch (language) {
        case 'en-US': // English
            language = LanguagesId[0];
            break;
        case 'ja-JP': // Japanese
            language = LanguagesId[2];
            break;
        case 'th-TH': // Thailand
            language = LanguagesId[3];
            break;
        case 'ko-KR': // Korean
            language = LanguagesId[5];
            break;
        case 'zh-TW': // Chinese traditional
            language = LanguagesId[1];
            break;
        case 'zh-CN': // Chinese simplified
            language = LanguagesId[4];
            break;
    }

    SelectLanguage(language);

    GetValidateImage();

    if (typeof _page.errCode != 'undefined' && _page.errCode != 0) {
        $("errmsg").innerHTML = _page.errMsg;
        if (_page.errCode == 101) {
            $('txtCaptcha').focus();
        }
    }
    InitUserNamePwd();

}

function showHideBox() {
    if ($("cboLang").style.display != 'none') {
        $("cboLang").style.display = 'none';
    }
    else {
        $("cboLang").style.display = 'block';
    }
}

//Script check HTML 5 for devices mobile
if ($("mobileId") != undefined) {
    var mydomain = document.domain.replace("ag.", "");
    if (navigator.geolocation) {
        $("mobileId").href = "http://m." + mydomain;
    } else {
        $("mobileId").href = "http://wap." + mydomain;
    }
}

var _DoLogin = DoLogin;

DoLogin = function () {

    if ($('txtUserName').value == $('spnUserName').innerHTML) {
        $('txtUserName').value = '';
    } else if ($('txtPassWord').value == $('spnPassWord').innerHTML) {
        $('txtPassWord').value = '';
    } else if ($('txtCaptcha').value == $('spnCaptcha').innerHTML) {
        $('txtCaptcha').value = '';
    }
    _DoLogin();
}

// Register start up
_addEvent(window, "load", InitLogin);