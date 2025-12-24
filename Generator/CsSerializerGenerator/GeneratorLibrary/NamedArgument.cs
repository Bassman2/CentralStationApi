using Microsoft.CodeAnalysis;

namespace CsSerializerGenerator.GeneratorLibrary
{
    public class NamedArgument(string name, TypedConstant arg) : ConstructorArgument(arg)
    {
        public string Name => name;
    }
}
