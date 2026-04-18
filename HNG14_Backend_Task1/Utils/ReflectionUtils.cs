using System.Reflection;

namespace HNG14_Backend_Task1.Utils
{
    public class ReflectionUtils
    {
        public static object? GetPropertyValue(object? obj, string propertyName)
        {
            if (obj == null) return null;

            PropertyInfo? prop = obj.GetType().GetProperty(propertyName);
            return prop?.GetValue(obj);
        }
    }
}
