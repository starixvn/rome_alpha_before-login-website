using System;
using System.Web;

namespace Fanex.Razorbill.Sphinx.Core.Extensions
{
    public class ResourceExtension
    {
      
        /// <summary>
        /// Get Resource Value
        /// </summary>
        /// <param name="resourceClass">Resource class, example: Lang</param>
        /// <param name="key">Key to get value</param>
        /// <returns></returns>
        public static string GetResource(string resourceClass, string key)
        {
            try
            {
                var globalResourceObject = HttpContext.GetGlobalResourceObject(resourceClass, key);
                if (globalResourceObject != null)
                    return globalResourceObject.ToString();
            }
            catch (Exception ex)
            {
                
                throw new Exception("Key: " + key + " does not exist in resource: " + resourceClass);
            }

            return string.Empty;
        }
    }
}
