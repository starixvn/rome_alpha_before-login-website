"use strict";

var Sphinx = Sphinx || {};
Sphinx.Resources = (function () {

    var listLanguage = [];

    function Language() {
        this.Id = '';
        this.Username = '';
        this.Flag = '';
        this.Password = '';
        this.Language = '';
        this.Captcha = '';
        this.ErrorMessage = '';
        this.Login = '';
        this.ShowPassword = '';
        this.AutumnTitle = '';
        this.SignIn = '';
        this.errUserName = '';
        this.errValidCode = '';
    }

    var addLanguage = function (lang) {
        listLanguage.push(lang);
    };

    var english = function () {
        var resource = new Language();
        resource.Id = 'en-US';
        resource.Username = 'Username';
        resource.Flag = 'flag-en';
        resource.Password = 'Password';
        resource.Language = 'English';
        resource.Captcha = 'Validation';
        resource.ErrorMessage = '';
        resource.Login = 'Login';
        resource.ShowPassword = 'Show password';
        resource.AutumnTitle = 'textTitle_en';
        resource.SignIn = 'SignIn';
        resource.errUserName = 'Please input username and password';
        resource.errValidCode = 'You must input validation code.';

        return resource;
    };
    addLanguage(english());

    var taiwan = function () {
        var resource = new Language();
        resource.Id = 'zh-TW';
        resource.Username = '用户名';
        resource.Flag = 'flag-tw';
        resource.Password = '密碼';
        resource.Language = '繁體中文';
        resource.Captcha = '驗證碼';
        resource.ErrorMessage = '';
        resource.Login = '登入';
        resource.ShowPassword = '顯示密碼';
        resource.AutumnTitle = 'textTitle-tw';
        resource.SignIn = 'SignIn';
        resource.errUserName = '請輸入賬號及密碼。';
        resource.errValidCode = '您必须输入正确验证码';

        return resource;
    };
    addLanguage(taiwan());

    var chinese = function () {
        var resource = new Language();
        resource.Id = 'zh-CN';
        resource.Username = '用户名';
        resource.Flag = 'flag-cn';
        resource.Password = '密码';
        resource.Language = '简体中文';
        resource.Captcha = '验证码';
        resource.ErrorMessage = '';
        resource.Login = '登入';
        resource.ShowPassword = '顯示密碼';
        resource.AutumnTitle = 'textTitle-cn';
        resource.SignIn = 'SignIn';
        resource.errUserName = '请输入账号及密码';
        resource.errValidCode = '您必須輸入正確驗證碼';

        return resource;
    };
    addLanguage(chinese());

    var japanese = function () {
        var resource = new Language();
        resource.Id = 'ja-JP';
        resource.Username = 'ユーザー名';
        resource.Flag = 'flag-jp';
        resource.Password = 'パスワード';
        resource.Language = '日本語';
        resource.Captcha = 'Validation';
        resource.ErrorMessage = '';
        resource.Login = '入力';
        resource.ShowPassword = 'Show password';
        resource.AutumnTitle = 'textTitle_en';
        resource.SignIn = 'SignIn';
        resource.errUserName = "ユーザー名とパスワードを入力してください";
        resource.errValidCode = "You must input validation code.";

        return resource;
    };
    addLanguage(japanese());

    var thailand = function () {
        var resource = new Language();
        resource.Id = 'th-TH';
        resource.Username = 'ชื่อผู้ใช้ระบบ';
        resource.Flag = 'flag-th';
        resource.Password = 'รหัสผ่าน';
        resource.Language = 'ภาษาไทย';
        resource.Captcha = 'รหัสตรวจสอบ';
        resource.ErrorMessage = '';
        resource.Login = 'เข้าระบบ';
        resource.ShowPassword = 'แสดง';
        resource.AutumnTitle = 'textTitle_en';
        resource.SignIn = 'SignIn';
        resource.errUserName = 'โปรดใส่ชื่อผู้ใช้และรหัสผ่าน';
        resource.errValidCode = 'คุณต้องใส่หมายเลขที่ใช้ในการตรวจสอบ';

        return resource;
    };
    addLanguage(thailand());

    var korean = function () {
        var resource = new Language();
        resource.Id = 'ko-KR';
        resource.Username = '사용자 이름';
        resource.Flag = 'flag-kr';
        resource.Password = '비밀번호';
        resource.Language = '한국어';
        resource.Captcha = 'Validation';
        resource.ErrorMessage = '';
        resource.Login = '로그인';
        resource.ShowPassword = '비밀번호 나타내기';
        resource.AutumnTitle = 'textTitle_en';
        resource.SignIn = 'SignIn';
        resource.errUserName = '사용자명과 비밀번호를 입력해주세요';
        resource.errValidCode = '확인코드를 입력해주세요.';

        return resource;
    };
    addLanguage(korean());

    var vietnamese = function () {
        var resource = new Language();
        resource.Id = 'vi-VN';
        resource.Username = 'Tên đăng nhập';
        resource.Flag = 'flag-vn';
        resource.Password = 'Mật khẩu';
        resource.Language = 'Tiếng Việt';
        resource.Captcha = 'Mã xác nhận';
        resource.ErrorMessage = '';
        resource.Login = 'Đăng nhập';
        resource.ShowPassword = 'Hiển thị mật khẩu';
        resource.AutumnTitle = 'textTitle_en';
        resource.SignIn = 'SignIn';
        resource.errUserName = 'Vui lòng điền Tên đăng nhập và Mật khẩu';
        resource.errValidCode = 'Bạn phải nhập mã xác thực.';

        return resource;
    };
    addLanguage(vietnamese());

    return {
        init: function () {
        },

        languages: function () {
            return listLanguage;
        },

        getLanguage: function (languageCode) {

            switch (languageCode) {
                case 'en-US':
                    return english();
                case 'vi-VN':
                    return vietnamese();
                case 'zh-CN':
                    return chinese();
                case 'zh-TW':
                    return taiwan();
                case 'ja-JP':
                    return japanese();
                case 'ko-KR':
                    return korean();
                case 'th-TH':
                    return thailand();
            }

            return english();
        }
    };
})();