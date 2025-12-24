using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace CsSerializerGenerator.GeneratorLibrary
{
    public abstract class BaseAttributes
    {
        public abstract IEnumerable<Attribute> Attributes { get; }

        public bool HasAttribute(string nameWithNamespace) => Attributes.Any(a => a.FullName == nameWithNamespace);

        public Attribute? GetAttribute(string nameWithNamespace) => Attributes.FirstOrDefault(a => a.FullName == nameWithNamespace);

    }
}
