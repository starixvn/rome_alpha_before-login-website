var detecasAnalysis = document.getElementById("detecas-analysis");
detecasAnalysis ? (detecasAnalysis.value = '{"startTime":1446686616296,"version":"0.0.0","enable":true}') : false;

var _page = {
    errCode: 0,
    errMsg: '',
    language: 0,
    ip: '',
    __utms: '',
    domain: '',
    captcha: '',
    fpsActivator: ''
};

_page.errCode = document.getElementById("ModelErrCode").value;
_page.errMsg = document.getElementById("ModelErrMsg").value;
_page.language = document.getElementById("ModelLanguage").value;
_page.ip = document.getElementById("ModelIp").value;
_page.__utms = document.getElementById("ModelUTMS").value;
_page.domain = document.getElementById("ModelDomain").value;
_page.captcha = document.getElementById("ModelCaptcha").value;
_page.fpsActivator = document.getElementById("CommonAppSettingFpsActivator").value;

window.rootUrl = document.getElementById("RootUrl").value;

if(parseInt(_page.errCode) === -1) {
    $('#mainholder').css('display', 'none');
    $('#divLoading').css('display', 'block');
    $("body").css("background", "none");
    window.location.href = 'Flow';
}

!function(e, t, a, c, n) {
    var s = t.createElement(a),
    o = t.getElementsByTagName(c)[0];
    s.async = true, s.defer = true, s.src = n, o.appendChild(s)
}(window, document, "script", "body", document.getElementById("CommonAppSettingFpsActivator").value);