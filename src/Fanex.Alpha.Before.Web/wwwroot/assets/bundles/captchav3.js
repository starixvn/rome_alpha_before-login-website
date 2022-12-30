$(document).ready(function () {
    var reCaptchaReadyCount = 0;
    var reCaptchaReadyTimeout = null;
    var isStarted = false;

    function checReCaptchaReady() {
        if (typeof (grecaptcha) !== 'undefined') {
            if (reCaptchaReadyTimeout) {
                clearTimeout(reCaptchaReadyTimeout);
                reCaptchaReadyTimeout = null;
            }

            if (!isStarted) {
                isStarted = true;
                grecaptcha.ready(function () {
                    console.log("grecaptcha.ready is executed");

                    grecaptcha.execute(googleV3key, { action: 'login' }).then(function (token) {                        
                        $.ajax({
                            url: Sphinx.rootUrl + "Auth/ValidateGoogleCaptcha",
                            type: "POST",
                            data: 'token=' + token,
                            success: function (result) {
                                $('#captcha_result').val(result);
                            },
                            error: function (request, status, error) {
                            }
                        });

                    });;
                });
            }
        } else {
            reCaptchaReadyCount++;
            if (reCaptchaReadyCount < 100) {
                reCaptchaReadyTimeout = setTimeout(checReCaptchaReady, 100);
            } else {
                console.log("reCaptchaReadyCount = " + reCaptchaReadyCount);
            }
        }
    }

    try {
        checReCaptchaReady();
    } catch (exception) {
        console.log("exception --> checReCaptchaReady" + exception.toString());
    }
});