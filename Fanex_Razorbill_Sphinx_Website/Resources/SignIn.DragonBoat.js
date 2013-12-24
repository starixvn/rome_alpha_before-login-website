
var _isIE = false;
var _isUsingCustomScript = true;
var placeHolderLanguage = {
    txtUserName: '',
    txtPassWord: '',
    txbShowPassword: ''
};
var placeHolder_IsSelect = {
    txtUserName: false,
    txtPassWord: false,
    txbShowPassword: false
};

function CustomInitLoginPage() {
    var pwdMask = $('txbShowPassword');
    //    pwdMask.setAttribute('placeholder', 'Password');
    $('boxShowPassword').style.textAlign = 'right';
    _isIE = isIE();
    if (_isIE) {
        $('boxShowPassword').style.padding = 0;
        $('rootHtml').className = "overflow-hidden";
        var myNav = navigator.userAgent.toLowerCase();
            $('main').className = "ie7";
    }
};

function CustomUpdateLoginPage(language, username, pwd, login, browser, pwdHint) {
    $('btnLogin').innerHTML = login;
    _isIE = isIE();
    if (_isIE) {
        placeHolderLanguage.txtUserName = username;
        placeHolderLanguage.txtPassWord = pwd;
        placeHolderLanguage.txbShowPassword = pwd;
    }
};

function CustomSelectLanguage(value) {
    $("errmsg").innerHTML = '&nbsp;';
};

function isIE() {
    var myNav = navigator.userAgent.toLowerCase();
    return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : false;
}

function MakePlaceHolder() {
    placeHolderLanguage.txtUserName = placeHolderLanguage['txtUserName'] ? placeHolderLanguage['txtUserName'] : 'Username';
    placeHolderLanguage.txtPassWord = placeHolderLanguage['txtPassWord'] ? placeHolderLanguage['txtPassWord'] : 'Password';
    placeHolderLanguage.txbShowPassword = placeHolderLanguage['txbShowPassword'] ? placeHolderLanguage['txbShowPassword'] : 'Password';

    if ($('txtUserName').value != '' && $('txtUserName').value != placeHolderLanguage['txtUserName']) {
        placeHolder_IsSelect['txtUserName'] = true;
    }

    $('txtUserName').setAttribute('onblur', "placeHolder_Blur('txtUserName');");
    $('txtUserName').setAttribute('onkeydown', "placeHolder_OnKeydown('txtUserName');");

    $('txtPassWord').value = placeHolderLanguage.txtPassWord;
    $('txtPassWord').setAttribute('onblur', "placeHolder_Blur('txtPassWord');");
    $('txtPassWord').setAttribute('onkeydown', "placeHolder_OnKeydown('txtPassWord');");

    $('txbShowPassword').value = placeHolderLanguage.txbShowPassword;
    $('txbShowPassword').setAttribute('onblur', "placeHolder_Blur('txbShowPassword');");
    $('txbShowPassword').setAttribute('onkeydown', "placeHolder_OnKeydown('txbShowPassword');");

    var myNav = navigator.userAgent.toLowerCase();
    if (parseInt(myNav.split('msie')[1]) <= 7) {
        $('txtUserName').onkeydown = function (e) {
            placeHolder_OnKeydown('txtUserName');
        };

        $('txtPassWord').onkeydown = function (e) {
            placeHolder_OnKeydown('txtPassWord');
        };

        $('txbShowPassword').onkeydown = function (e) {
            placeHolder_OnKeydown('txbShowPassword');
        };

        $('txtUserName').onblur = function (e) {
            placeHolder_Blur('txtUserName');
        };

        $('txtPassWord').onblur = function (e) {
            placeHolder_Blur('txtPassWord');
        };

        $('txbShowPassword').onblur = function (e) {
            placeHolder_Blur('txbShowPassword');
        };
    }
};

function placeHolder_Blur(id) {
    if (_isIE) {
        var txtValue = $(id).value;
        if (txtValue == null || txtValue == '') {
            $(id).value = placeHolderLanguage[id];
            placeHolder_IsSelect[id] = false;
        }

        if (id == 'txtPassWord') {
            placeHolder_IsSelect['txbShowPassword'] = placeHolder_IsSelect[id];

            if (placeHolder_IsSelect[id]) {
                $('txtPassWord').setAttribute("type", "password");
            } else {
                $('txtPassWord').setAttribute("type", "text");
            }
        }

    }
};

function placeHolder_OnKeydown(id) {
    if (_isIE) {
        if (placeHolder_IsSelect[id] == false) {
            $(id).value = '';
            placeHolder_IsSelect[id] = true;
            if (id == 'txtPassWord') {
                placeHolder_IsSelect['txbShowPassword'] = placeHolder_IsSelect[id];
                $('txtPassWord').setAttribute("type", "password");
            }
        }
    }
};

function CustomValidate() {
    if (_isIE) {
        var errObj = LanguageMessages[CurrentLanguageId];

        if (placeHolder_IsSelect['txtUserName'] == false) {
            $("errmsg").innerHTML = errObj.errUserName;
            $('txtUserName').focus();
            return false;
        }

        if (placeHolder_IsSelect['txtPassWord'] == false &&
            placeHolder_IsSelect['txbShowPassword'] == false) {
            $("errmsg").innerHTML = errObj.errUserName;
            $('txtPassWord').focus();
            return false;
        }
    }

    return true;
};

function ShowHideShowPasswordTextBox() {
    var pwdMask = $('txtPassWord');
    var txtPassWord = $('txbShowPassword');
    var $this = $('showPwdChk');
    pwdMask.style.display = $this.checked ? 'none' : 'inline-block';
    txtPassWord.style.display = !$this.checked ? 'none' : 'inline-block';
};

function ShowHideShowPasswordTextBox() {
    var pwdMask = $('txtPassWord');
    var txtPassWord = $('txbShowPassword');
    var $this = $('showPwdChk');
    pwdMask.style.display = $this.checked ? 'none' : 'inline-block';
    txtPassWord.style.display = !$this.checked ? 'none' : 'inline-block';
};
