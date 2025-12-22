namespace CentralStationWebApi.Serializer
{
    public static class CsSerializer
    {
        public static T Deserialize<T>(Stream stream) where T : ICsSerialize, new()
        {
            var main = new T();

            StreamReader reader = new StreamReader(stream);

            main.Deserialize(reader);
            return main;
        }
    }
}
