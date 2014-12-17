using System.Reflection;
using System.Text;
using Domain.RuleExperiments.Attributes;

namespace Application.RuleExperiments.Cachers
{
    public class CacheManager
    {
        /// <summary>
        /// Gets a key string for cache, uses <see cref="CacheKeyAttribute"/> attributes to generate the key. Looks for the inhereted properties
        /// </summary>
        /// <param name="object">Object to generate key from</param>
        /// <returns>string key</returns>
        public static string GetCacheStringFromObject(object @object)
        {
            var properties = @object.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo propertyInfo in properties)
            {
                var cackeKey = propertyInfo.GetCustomAttributes(typeof(CacheKeyAttribute), true)[0];
                if (cackeKey != null)
                {
                    sb.Append(propertyInfo.GetValue(@object, null));
                }
            }

            return sb.ToString();
        }
    }
}