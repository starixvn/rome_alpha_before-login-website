"use strict";

var __fpsImporter = {
    loadJS: function loadJS(_url, resolve, reject) {
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.src = _url + '/activator.ashx';
        script.async = true;
        script.defer = true;
        script.addEventListener('load', function () {
            return resolve(script);
        }, false);
        script.addEventListener('error', function () {
            return reject(script);
        }, false);
        document.body.appendChild(script);
    },
    makeRequest: function makeRequest(_url) {
        try {
            var xhr = new XMLHttpRequest();
            var hcUrl = _url + '/hc.html?livepage=' + new Date() * 1;
            xhr.open('get', hcUrl, false);            
            xhr.send();

            if (xhr.readyState == 4) {
                return {
                    status: xhr.status,
                    response: xhr.responseText,
                    url: _url
                };
            }
        } catch (e) {
            return {
                status: 404
            };
        }
    },
    url: function url(_url) {
        return new Promise(function (resolve, reject) {
            __fpsImporter.loadJS(_url, resolve, reject);
        });
    },
    urls: function urls(_urls) {

        try {        
            $.each(_urls, function (key, _url) {

                var result = __fpsImporter.makeRequest(_url);

                if (result && result.status == 200) {
                    __fpsImporter.loadJS(result.url, function () {
                        Detecas.Config.host = result.url;
                    }, function () {
                        console.log("Cannot inject script");
                    });

                    return false;
                }
            });
            
        } catch (e) {
            console.log(e);
        }
    }
};

var fpsDomains = document.getElementById("CommonAppSettingFpsActivator").value;

if (fpsDomains != null) {
    var domains = fpsDomains.split(';');

    __fpsImporter.urls(domains);
}