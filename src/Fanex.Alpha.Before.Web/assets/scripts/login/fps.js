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
            var requests = _urls.map(function (url) {
                return fetch(url + '/hc.html', {
                    cache: "no-cache",
                    timeout: 1000
                });
            });

            Promise.race(requests).then(function (res) {
                if (res.status == 200) {
                    var str = res.url;
                    var index = str.lastIndexOf('/');
                    var url = str.substring(0, index);

                    __fpsImporter.url(url).then(function () {
                        Detecas.Config.host = url;
                    });
                }
            });
        } catch (e) {
            var requests = [];

            var results = _urls.map(function (_url) {
                return __fpsImporter.makeRequest(_url);
            });

            for (var i = 0; i < results.length; i++) {
                var result = results[i];

                if (result.status == 200) {
                    __fpsImporter.loadJS(result.url, function () {
                        Detecas.Config.host = result.url;
                    }, function () { });

                    break;
                }
            }
        }
    }
};

var fpsDomains = document.getElementById("CommonAppSettingFpsActivator").value;

if (fpsDomains != null) {
    var domains = fpsDomains.split(';');

    __fpsImporter.urls(domains);
}