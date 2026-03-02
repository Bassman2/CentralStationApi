//using System.Collections;
//using System.Diagnostics.CodeAnalysis;
//using System.Reflection;
//using System.Runtime.Serialization;

//namespace CentralStationDemo.Misc;


//public static class M2VMExtentions
//{
//    private const BindingFlags ConstructorDefault = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance;

//    public static T? CastModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this object? value) where T : class
//    {
//        return value is null ? null : (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [value], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T).Name}!"));
//    }

//    public static T? CastModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this object? value, object service) where T : class
//    {
//        return value is null ? null : (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [service, value], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T).Name}!"));
//    }

//    public static T? CastModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this object? value, object service, object param) where T : class
//    {
//        return value is null ? null : (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [service, value, param], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T).Name}!"));
//    }

//    public static List<T>? CastModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IEnumerable? value) where T : class
//    {
//        return value?.Cast<object>().Select(i => (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [i], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T).Name}!"))).ToList<T>();
//    }

//    public static List<T>? CastModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IEnumerable? value, object service) where T : class
//    {
//        return value?.Cast<object>().Select(i => (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [service, i], null) ?? throw new ArgumentException($"CastModel with jira failed for {typeof(T).Name}!"))).ToList<T>();
//    }

//    public static Dictionary<string, T>? CastModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this Dictionary<string, object> value) where T : class
//    {
//        return value?.Select(i => new KeyValuePair<string, T>(i.Key, (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [i.Value], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T).Name}!")))).ToDictionary();
//    }

//    public static Dictionary<string, T>? CastModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IDictionary? value) where T : class
//    {
//        //if (value == null) return null;

//        //Dictionary<string, T>? result = new();

//        //foreach (var key in value.Keys)
//        //{
//        //    var item = value[key]!;
//        //    T val = ActivatorCreate<T>(item);
//        //    result.Add((string)key, val);

//        //}

//        //return result;

//        return value?.Keys.Cast<string>().Select(k => new KeyValuePair<string, T>(k, ActivatorCreate<T>(value[k]!))).ToDictionary();
//    }

//    public static async IAsyncEnumerable<T1> CastModelAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T1, T2>(this IAsyncEnumerable<T2> value) where T2 : class where T1 : class
//    {
//        await foreach (var item in value)
//        {
//            yield return (T1)(Activator.CreateInstance(typeof(T1), ConstructorDefault, null, [item], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T1).Name}!"));
//        }
//    }

//    private static T ActivatorCreate<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(object value)
//    {
//        return (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [value], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T).Name}!"));
//    }

//    private static T ActivatorCreate<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicConstructors | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(object service, object value)
//    {
//        return (T)(Activator.CreateInstance(typeof(T), ConstructorDefault, null, [service, value], null) ?? throw new ArgumentException($"CastModel failed for {typeof(T).Name}!"));
//    }

//    public static string? Value<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEnum>(this TEnum enumVal) where TEnum : Enum
//    {
//        return typeof(TEnum).GetMember(enumVal?.ToString()!).FirstOrDefault()?.GetCustomAttributes(typeof(EnumMemberAttribute), false).Cast<EnumMemberAttribute>().FirstOrDefault()?.Value;
//    }
//}
