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
        resource.Flag = 'flag flag-en';
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
        resource.browserWarning = "Your browser is out of date and might not be working properly with our website. For a better support, we strongly recommend that you upgrade to a newer version";
        resource.updateYourBrowser = "Click here to update your browser";
        resource.messageAlwayShowOnLoginPage = "The site encountered a technical error, we are working to resolve it. The estimated completion time is {0} (GMT +8). Please try again after that time. We are sorry for the inconvenience.";

        return resource;
    };
    addLanguage(english());

    var taiwan = function () {
        var resource = new Language();
        resource.Id = 'zh-TW';
        resource.Username = '用户名';
        resource.Flag = 'flag flag-tw';
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
        resource.browserWarning = "您的瀏覽器已過期，可能無法與我們的網站正常操作。為了提供更好的支援，我們強烈建議您升級至新的版本";
        resource.updateYourBrowser = "請點擊這里以升級您的瀏覽器";
        resource.messageAlwayShowOnLoginPage = "目前網站面對技術有誤，我們正在處理當中。估計完成時間為{0}（GMT +8）。請您稍後再試，給您帶來的不便請見諒";

        return resource;
    };
    addLanguage(taiwan());

    var chinese = function () {
        var resource = new Language();
        resource.Id = 'zh-CN';
        resource.Username = '用户名';
        resource.Flag = 'flag flag-cn';
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
        resource.browserWarning = "您的浏览器已过期，可能无法与我们的网站正常操作。为了提供更好的支援，我们强烈建议您升级至新的版本";
        resource.updateYourBrowser = "请点击这里以升级您的浏览器";
        resource.messageAlwayShowOnLoginPage = "目前网站面对技术有误，我们正在处理当中。估计完成时间为{0}（GMT +8）。请您稍后再试，给您带来的不便请见谅";

        return resource;
    };
    addLanguage(chinese());

    var japanese = function () {
        var resource = new Language();
        resource.Id = 'ja-JP';
        resource.Username = 'ユーザー名';
        resource.Flag = 'flag flag-jp';
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
        resource.browserWarning = "Your browser is out of date and might not be working properly with our website. For a better support, we strongly recommend that you upgrade to a newer version";
        resource.updateYourBrowser = "Click here to update your browser";
        resource.messageAlwayShowOnLoginPage = "The site encountered a technical error, we are working to resolve it. The estimated completion time is {0} (GMT +8). Please try again after that time. We are sorry for the inconvenience.";

        return resource;
    };
    addLanguage(japanese());

    var thailand = function () {
        var resource = new Language();
        resource.Id = 'th-TH';
        resource.Username = 'ชื่อผู้ใช้ระบบ';
        resource.Flag = 'flag flag-th';
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
        resource.browserWarning = "เบราว์เซอร์ของท่านตกรุ่น ซึ่งอาจทำงานได้อย่างไม่เหมาะสมกับเวปไซค์ของทางบริษัท. เพื่อตอบสนองการใช้งานที่ดียิ่งขึ้น ทางเราใคร่ขอแนะนำให้ท่าน ทำการอัพเกรดให้เป็นเวอร์ชั่นที่ใหม่กว่าเดิม";
        resource.updateYourBrowser = "คลิ๊กตรงนี้เพื่อทำการอัพเกรด เบราว์เซอร์ของท่าน";
        resource.messageAlwayShowOnLoginPage = "เว็บไซต์ตรวจพบข้อผิดพลาดทางเทคนิค เรากำลังดำเนินการแก้ไข โดยจะสิ้นสุดเวลาประมาณ {0} (GMT +8). โปรดลองใหม่อีกครั้งภายหลัง ขออภัยในความไม่สะดวก";

        return resource;
    };
    addLanguage(thailand());

    var korean = function () {
        var resource = new Language();
        resource.Id = 'ko-KR';
        resource.Username = '사용자 이름';
        resource.Flag = 'flag flag-kr';
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
        resource.browserWarning = "Your browser is out of date and might not be working properly with our website. For a better support, we strongly recommend that you upgrade to a newer version";
        resource.updateYourBrowser = "Click here to update your browser";
        resource.messageAlwayShowOnLoginPage = "The site encountered a technical error, we are working to resolve it. The estimated completion time is {0} (GMT +8). Please try again after that time. We are sorry for the inconvenience.";

        return resource;
    };
    addLanguage(korean());

    var vietnamese = function () {
        var resource = new Language();
        resource.Id = 'vi-VN';
        resource.Username = 'Tên đăng nhập';
        resource.Flag = 'flag flag-vn';
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
        resource.browserWarning = "Trình duyệt của bạn đã cũ và có thể không tương thích với trang web của chúng tôi. Để hỗ trợ tốt hơn, chúng tôi đề nghị bạn nâng cấp lên một phiên bản mới hơn";
        resource.updateYourBrowser = "Nhấn vào đây để cập nhật trình duyệt của bạn";
        resource.messageAlwayShowOnLoginPage = "Hệ thống đang gặp sự cố và chúng tôi đang cố gắng khắc phục trong thời gian sớm nhất có thể. Dự kiến sẽ hoàn tất lúc {0} (GMT +8). Xin vui lòng thử lại sau thời gian trên. Chúng tôi rất tiếc về sự bất tiện này.";

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