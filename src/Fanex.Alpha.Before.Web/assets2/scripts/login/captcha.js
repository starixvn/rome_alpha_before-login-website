var captchaId = '#' + document.getElementById("CaptchaId").value;

$(function () {
    $(captchaId).show();
    
    $('#captcha-refresh-icon, #' + $('#ModelImageElementId').val()).click(function () {
        var tokenParams = {};
        tokenParams[$("#ModelTokenParameterName").val()] = $('#' + $("#ModelTokenElementId").val()).val();

        $.post($("#ModelRefreshUrl").val(), tokenParams)
          .always(function (result) {
              eval(result.responseText);
              $(captchaId).show();
          })

        return false;
    }); 
});

var noInteractionTimeInMinutes = parseInt(document.getElementById("NoInteractionTimeInMinutes").value);

if (noInteractionTimeInMinutes > 0) {
    setTimeout(
        function () {
            window.location.href = window.location.href;
        }, noInteractionTimeInMinutes);
}