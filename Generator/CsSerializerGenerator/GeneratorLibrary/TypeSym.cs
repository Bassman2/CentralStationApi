using Microsoft.CodeAnalysis;

namespace CsSerializerGenerator.GeneratorLibrary
{
    public class TypeSym(ITypeSymbol symbol) 
    {
        public string Name => symbol.Name;

        public string FullName => symbol.ToDisplayString();

        public string NameSpace => symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;

        public TypeKind TypeKind => symbol.TypeKind;
    }
}
