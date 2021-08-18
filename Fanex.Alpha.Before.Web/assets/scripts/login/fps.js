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
        const request = urls.map(url => fetch(url + '/hc.html', { cache: "no-cache", timeout: 1000 }));
        Promise.any(request).then(res => {
            if (res.status == 200) {
                let str = res.url;
                let index = str.lastIndexOf('/');
                let url = str.substring(0, index);;
                __fpsImporter.url(url).then(() => {
                    Detecas.Config.host = url;
                });
            }
        });
    }
};

__fpsImporter.urls(document.getElementById("CommonAppSettingFpsActivator").value);