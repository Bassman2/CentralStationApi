namespace CentralStationWebApi.Serializer
{
    public static class CsSerializer
    {
        //public static T Deserialize<T>(Stream stream) where T : ICsSerialize, new()
        //{
        //    var main = new T();

        //    StreamReader reader = new StreamReader(stream);

        //    main.Deserialize(reader);
        //    return main;
        //}

        public static T Deserialize<T>(Stream stream, string id) where T : ICsSerialize, new()
        {
            Debug.WriteLine($"Deserialize {typeof(T).Name} {id}");
            Debug.IndentLevel = 2;

            var main = new T();

            Stack<ICsSerialize> stack = new Stack<ICsSerialize>();

            StreamReader reader = new StreamReader(stream);
            ICsSerialize? leave = main;
            int level = 0;

            string? line = reader.ReadLine();
            if (line == null || !line.StartsWith(id))
            {
                throw new InvalidDataException("Invalid lokomotive data");
            }
            while ((line = reader.ReadLine()) != null)
            {
                if (level != 0 && !line.StartsWith(" " + new string('.', level)))
                {
                    
                    // end of section
                    leave = stack.Pop();
                    level--;
                    Debug.Unindent();
                    Debug.WriteLine($"End {leave.GetType().Name}");
                }
                
                // subclasses
                if (!line.Contains('='))
                {
                    stack.Push(leave);
                    level++; 
                    leave = leave.DeserializeLeave(line);
                    Debug.WriteLine($"Begin {leave.GetType().Name}");
                    Debug.Indent();

                }
                else
                {
                    var parts = line.Split('=', 2);
                    leave.DeserializeProperty(parts[0], parts[1]);
                }
            }
            Debug.WriteLine("Deserialize End");

            return main;
        }
    }
}
