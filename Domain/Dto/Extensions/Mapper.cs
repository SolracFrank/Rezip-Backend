using System.Reflection;

namespace Domain.Dto.Extensions
{
    public static class Mapper
    {
        public static Y Map<T, Y>(T source)
            where T : class, new()
            where Y : class, new()
        {
            var destination = new Y();
            var sourceProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(sp => sp.CanRead);
            var destProperties = typeof(Y).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(dp => dp.CanWrite)
                .ToDictionary(dp => dp.Name);

            foreach (var sourceProp in sourceProperties)
            {
                if (destProperties.TryGetValue(sourceProp.Name, out var destProp))
                {
                    if (destProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                    {
                        var value = sourceProp.GetValue(source, null);
                        destProp.SetValue(destination, value, null);
                    }
                }
            }

            return destination;
        }

        public static IEnumerable<Y> Map<T, Y>(IEnumerable<T> sourceCollection)
            where T : class, new()
            where Y : class, new()
        {
            return sourceCollection.Select(sourceItem => Map<T, Y>(sourceItem)).ToList();
        }
    }
}