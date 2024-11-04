using System.Collections.Generic;

namespace Monaverse.Api.Extensions
{
    public static class ApiFilterExtensions
    {
        public static Dictionary<string, object> ToPropertyDictionary(this object filter)
        {
            //turn every property of the filter into a key-value pair
            var dictionary = new Dictionary<string, object>();
            var properties = filter.GetType().GetProperties();
            foreach (var property in properties)
                dictionary.Add(property.Name, property.GetValue(filter));
            return dictionary;
        }
    }
}