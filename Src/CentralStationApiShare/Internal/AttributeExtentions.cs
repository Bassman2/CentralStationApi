using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CentralStationApi.Internal;

internal static class AttributeExtentions
{
    public static string? GetFileName<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicFields |
            DynamicallyAccessedMemberTypes.PublicConstructors |
            DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicNestedTypes |
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicEvents
        )] T>(this T enu) where T : Enum
    {
        var memberInfo = typeof(T).GetMember(enu.ToString()).FirstOrDefault();
        if (memberInfo != null)
        {
            var attr = memberInfo.GetCustomAttributes(typeof(FileNameAttribute), false).OfType<FileNameAttribute>().FirstOrDefault();
            if (attr != null)
            {
                return attr.FileName;
            }
        }
        return null;
    }

    public static Uri? GetFileNamePath<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicFields |
            DynamicallyAccessedMemberTypes.PublicConstructors |
            DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicNestedTypes |
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicEvents
        )] T>(this T enu, Uri uri) where T : Enum
    {
        string? fileName = GetFileName(enu);
        return fileName is null ? null : new Uri(uri, fileName);
    }

    public static string GetDescription<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicFields |
            DynamicallyAccessedMemberTypes.PublicConstructors |
            DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicNestedTypes |
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicEvents
        )] T>(this T enu) where T : Enum
    {
        return typeof(T).GetField(enu.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? enu.ToString();
    }
}
