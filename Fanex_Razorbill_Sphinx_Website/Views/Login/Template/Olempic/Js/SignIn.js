/*
* Created 20100113@Lee - Sign-in javascript functions
* Revision ?@? - ...
*
*/
var CurrentLanguageId = 0;

var LanguagesId = new Array(0, 1, 2, 3, 4, 5, 6);
var Languages = new Array("en-US", "zh-TW", "ja-JP", "th-TH", "zh-CN", "ko-KR", "vi-VN");
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
}, {
    errUserName: "Vui lòng điền Tên đăng nhập và Mật khẩu",
    errValidCode: "Bạn phải nhập mã xác thực."
});

function SelectLanguage(value) {
    $('lanEnUS').style.border = '0px';
    $('lanZhTW').style.border = '0px';
    $('lanZhCN').style.border = '0px';
    $('lanJaJP').style.border = '0px';
    $('lanThTH').style.border = '0px';
    $('lanKoKR').style.border = '0px';
    $('lanViVN').style.border = '0px';

    if (value == LanguagesId[0]) // English
    {
        SetValue(value, 'Username', 'Password', 'Login', 'Recommended Browsers', 'Validation', "Show password");
        $('lanEnUS').style.border = '1px solid #FFF';
    } else if (value == LanguagesId[1]) // Chinese traditional
    {
        SetValue(value, '用户名', '密碼', '登入', 'Recommended Browsers', '驗證碼', "顯示密碼");
        $('lanZhTW').style.border = '1px solid #FFF';
    } else if (value == LanguagesId[4]) // Chinese simplified
    {
        SetValue(value, '用户名', '密码', '注册', 'Recommended Browsers', '验证码', "显示密码");
        $('lanZhCN').style.border = '1px solid #FFF';
    } else if (value == LanguagesId[2]) // Japanese
    {
        SetValue(value, 'ユーザー名', 'パスワード', '入力', 'Recommended Browsers', 'Validation', "Show password");
        $('lanJaJP').style.border = '1px solid #FFF';
    } else if (value == LanguagesId[3]) // Thailand
    {
        SetValue(value, 'ชื่อผู้ใช้ระบบ', 'รหัสผ่าน', 'เข้าระบบ', 'Recommended Browsers', 'รหัสตรวจสอบ', "แสดง");
        $('lanThTH').style.border = '1px solid #FFF';
    } else if (value == LanguagesId[5]) // Korean
    {
        SetValue(value, '사용자 이름', '비밀번호', '로그인', 'Recommended Browsers', 'Validation', "비밀번호 나타내기");
        $('lanKoKR').style.border = '1px solid #FFF';
    }
    else if (value == LanguagesId[6]) // Vietnamese
    {
        SetValue(value, 'Tên đăng nhập', 'Mật khẩu', 'Đăng nhập', 'Recommended Browsers', 'Validation', "Hiển thị mật khẩu");
        $('lanViVN').style.border = '1px solid #FFF';
    }
    $("errmsg").innerHTML = '';
}

function SetValue(language, username, pwd, login, browser, captcha, pwdHint) {
    $('hidLanguage').value = Languages[language];
    $('spnUserName').innerHTML = username;
    $('spnPassWord').innerHTML = pwd;
    $('btnLogin').value = login;
    $('divBrowser').innerHTML = browser;
    $('spnCaptcha').innerHTML = captcha;
    $('showPwdLabel').innerHTML = pwdHint;
    setCookie('LANGUAGE', language, 30);
    CurrentLanguageId = language;
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

    $("divPopupMobileVersion").innerHTML = '';

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

function InitLogin() {
    // Do not allow wrapping sign-in page
    if (window.parent != window) {
        window.parent.location = location.href;
    }

    (function () {
        // Insert show password check box
        var td = $('txtPassWord').parentNode;
        var div = document.createElement("DIV");
        var checkbox = document.createElement("INPUT");
        checkbox.setAttribute('type', 'checkbox');
        checkbox.id = 'showPwdChk';
        var label = document.createElement("LABEL");
        label.setAttribute('for', checkbox.id);
        label.id = 'showPwdLabel';
        label.style.color = '#eee';

        div.style.padding = '4px';
        $('spnPassWord').parentNode.vAlign = 'top';
        $('spnCaptcha').parentNode.style.paddingTop = '0px';
        $('imgCode').parentNode.style.paddingTop = '4px';

        div.appendChild(checkbox);
        div.appendChild(label);

        var txtPassWord = $('txtPassWord');
        var pwdMask = document.createElement("INPUT");
        pwdMask.setAttribute('type', 'text');
        pwdMask.className = 'tb';
        pwdMask.style.display = 'none';
        td.appendChild(pwdMask);
        td.appendChild(div);
        $('divBrowser').style.display = 'none';

        checkbox.onclick = function () {
            txtPassWord.style.display = this.checked ? 'none' : 'block';
            pwdMask.style.display = !this.checked ? 'none' : 'block';
        }

        txtPassWord.onchange = function () {
            pwdMask.value = this.value;
        }
        pwdMask.onchange = function () {
            txtPassWord.value = this.value;
        }

    })();

    var language = getCookie('LANGUAGE');
    if (!language) language = _page.language;

    SelectLanguage(language);

    GetValidateImage();

    if (typeof _page.errCode != 'undefined' && _page.errCode != 0) {
        $("errmsg").innerHTML = _page.errMsg;
        if (_page.errCode == 102) {
            $('txtCaptcha').focus();
        }
    }

    if ($('txtUserName').value == "") {
        $('txtUserName').focus();
    }

    $('txtPassWord').setAttribute('autocomplete', 'off');
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