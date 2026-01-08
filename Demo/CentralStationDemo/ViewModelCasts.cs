using System.Collections;

namespace CentralStationDemo;

public static class ViewModelCasts
{
    public static List<T> ToViewModelList<T>(this IEnumerable list) => list.Cast<dynamic>().Select(i => (T)Activator.CreateInstance(typeof(T), i)).ToList();

    public static List<T>? ToNullableViewModelList<T>(this IEnumerable? list) => list?.Cast<dynamic>().Select(i => (T)Activator.CreateInstance(typeof(T), i)).ToList(); 

    public static List<T>? ToViewModelList<T>(this IEnumerable? list, object param) => list?.Cast<dynamic>()?.Select(i => (T)Activator.CreateInstance(typeof(T), i, param))?.ToList();

    public static List<T>? ToNullableViewModelList<T>(this IEnumerable? list, object param) => list?.Cast<dynamic>().Select(i => (T)Activator.CreateInstance(typeof(T), i, param)).ToList();
}
