// -----------------------------------------------------------------------
// <copyright file="ResourceExtension.cs" company="Nexcel Solution">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.Razorbill.Sphinx.Core.Extensions
{
    using System;
    using System.Web;

    /// <summary>
    /// Resource extension.
    /// </summary>
    public class ResourceExtension
    {      
        /// <summary>
        /// Get Resource Value.
        /// </summary>
        /// <param name="resourceClass">Resource class, example: Lang.</param>
        /// <param name="key">Key to get value.</param>
        /// <returns>Return result.</returns>
        public static string GetResource(string resourceClass, string key)
        {
            try
            {
                var globalResourceObject = HttpContext.GetGlobalResourceObject(resourceClass, key);
                if (globalResourceObject != null)
                {
                    return globalResourceObject.ToString();
                }
            }
            catch (Exception ex)
            {                
                throw new Exception(
                    string.Format(
                    "Key: {0} does not exist in resource: {1}<br/> Detail Error: {2}", 
                                key, 
                                resourceClass,
                                ex.Message));
            }

            return string.Empty;
        }
    }
}
