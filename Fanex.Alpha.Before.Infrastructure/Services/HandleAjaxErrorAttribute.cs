// --------------------------------------------------------------------------
// <copyright file="HandleAjaxErrorAttribute.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Services
{
    using System;
    using System.Web.Mvc;
    using Fanex.Alpha.Before.Infrastructure.Constants;
    using Fanex.Alpha.Before.Infrastructure.Resources;

    /// <summary>
    /// Handle Ajax Error Attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class HandleAjaxErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            // if the request is AJAX return JSON else view.
            var requestHeaderType = filterContext.HttpContext.Request.Headers["X-Requested-With"];
            switch (requestHeaderType)
            {
                case RequestHeaderTypeConstants.XMLHttpRequest:
                    filterContext.Result = new JsonResult
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            error = ErrorCodeConstants.ServerError,
                            message = Labels.Error_Server,
                            innerException = filterContext.Exception.ToString()
                        }
                    };

                    break;
                default:
                    var controllerName = (string)filterContext.RouteData.Values["controller"];
                    var actionName = (string)filterContext.RouteData.Values["action"];
                    var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                    filterContext.Result = new ViewResult
                    {
                        ViewName = View,
                        MasterName = Master,
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = filterContext.Controller.TempData
                    };

                    break;
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = ErrorCodeConstants.ServerError;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}