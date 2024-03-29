"use strict";
var Sphinx = Sphinx || {};

Sphinx.AssetAppPaths = {
  globalAssetAppPaths: null,
  healthCheckPage: "/bundles/common/hc.css?v=",
  initGlobalAssetAppPaths: function () {
    Sphinx.AssetAppPaths.globalAssetAppPaths = [];
    var urlsValue = $("#hiddenAssetAppPaths").val();

    if (urlsValue.length) {
      var urls = urlsValue.split(",");

      for (var i = 0, max = urls.length; i < max; i++) {
        Sphinx.AssetAppPaths.globalAssetAppPaths.push({
          url: urls[i],
          isOK: false,
        });
      }
    }
  },
  doHealthCheck: function (urlObject) {
    $.get(
      urlObject.url +
        Sphinx.AssetAppPaths.healthCheckPage +
        new Date().getTime(),
      function () {
        urlObject.isOK = true;
      }
    );
  },
  init: function () {
    Sphinx.AssetAppPaths.initGlobalAssetAppPaths();

    $.each(Sphinx.AssetAppPaths.globalAssetAppPaths, function (index, value) {
      Sphinx.AssetAppPaths.doHealthCheck(value);
    });
  },
  getAvailableAssetAppPaths: function () {
    var urlObject;

    for (
      var i = 0, max = Sphinx.AssetAppPaths.globalAssetAppPaths.length;
      i < max;
      i++
    ) {
      urlObject = Sphinx.AssetAppPaths.globalAssetAppPaths[i];

      if (urlObject.isOK) {
        return urlObject.url;
      }
    }

    return "";
  },
};

Sphinx.DCSStaticAppPaths = {
  globalDSCStaticAppPaths: null,
  initGlobalDCSStaticAppPaths: function () {
    Sphinx.DCSStaticAppPaths.globalDSCStaticAppPaths = [];
    var urlsValue = $("#hiddenDCSStaticAppPaths").val();

    if (urlsValue.length) {
      var urls = urlsValue.split(",");

      for (var i = 0, max = urls.length; i < max; i++) {
        Sphinx.DCSStaticAppPaths.globalDSCStaticAppPaths.push({
          url: urls[i],
          isOK: false,
        });
      }
    }
  },
  doHealthCheck: function (urlObject) {
    $.get(urlObject.url + "?v=" + new Date().getTime(), function () {
      urlObject.isOK = true;
    });
  },
  init: function () {
    Sphinx.DCSStaticAppPaths.initGlobalDCSStaticAppPaths();

    $.each(
      Sphinx.DCSStaticAppPaths.globalDSCStaticAppPaths,
      function (index, value) {
        Sphinx.DCSStaticAppPaths.doHealthCheck(value);
      }
    );
  },
  getAvailableDCSStaticAppPaths: function () {
    var urlObject;

    for (
      var i = 0, max = Sphinx.DCSStaticAppPaths.globalDSCStaticAppPaths.length;
      i < max;
      i++
    ) {
      urlObject = Sphinx.DCSStaticAppPaths.globalDSCStaticAppPaths[i];

      if (urlObject.isOK) {
        return urlObject.url;
      }
    }

    return "";
  },
};
