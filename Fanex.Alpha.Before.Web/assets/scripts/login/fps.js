const __fpsImporter = {
    url: (url) => {
        return new Promise((resolve, reject) => {
            let script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = url + '/activator.ashx';
            script.async = true;
            script.defer = true;
            script.addEventListener('load', () => resolve(script), false);
            script.addEventListener('error', () => reject(script), false);
            document.body.appendChild(script);
        });
    },
    urls: (strUrls) => {
        const urls = strUrls.split(';');
        const request = urls.map(url => fetch(url + '/hc.html', { cache: "no-cache" }));
        Promise.any(request).then(res => {
            if (res.status == 200) {
                const requestUrl = res.url;
                const domain = (new URL(requestUrl));
                __fpsImporter.url(domain.origin).then(() => {
                    Detecas.Config.host = domain.origin;
                });
            }
        });
    }
};

__fpsImporter.urls(document.getElementById("CommonAppSettingFpsActivator").value);