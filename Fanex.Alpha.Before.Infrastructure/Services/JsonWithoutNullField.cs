// --------------------------------------------------------------------------
// <copyright file="JsonWithoutNullField.cs" company="">
// Copyright (c). All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.Alpha.Before.Infrastructure.Services
{
    using System.Web.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON Without Null Field.
    /// </summary>
    public class JsonWithoutNullField : ActionResult
    {
        public JsonWithoutNullField(object value)
        {
            this._value = value;
        }

        private object _value { get; set; }

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult" /> class.
        /// </summary>
        /// <param name="context">The context in which the result is executed. 
        /// The context information includes the controller, HTTP content, request context, and route data.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            var jsonString = JsonConvert.SerializeObject(
                this._value,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            context.HttpContext.Response.AddHeader("content-type", "application/json");
            context.HttpContext.Response.Write(jsonString);
        }
    }
}
