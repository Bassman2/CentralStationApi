using System.Diagnostics.CodeAnalysis;

namespace CentralStationWebApi;

[AttributeUsage(AttributeTargets.Field)]
public class FileNameAttribute(string fileName) : Attribute
{
    public string FileName => fileName;

    public static string? GetFilename<
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicFields |
            DynamicallyAccessedMemberTypes.PublicConstructors |
            DynamicallyAccessedMemberTypes.PublicMethods |
            DynamicallyAccessedMemberTypes.PublicNestedTypes |
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.PublicEvents
        )] T>(T enu) where T : Enum
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
}
