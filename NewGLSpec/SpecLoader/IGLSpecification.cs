using System.Collections.Generic;

namespace NewGLSpec
{
    public interface IGLSpecification
    {
        Dictionary<string, GLEnum> Enums { get; }
        
        Dictionary<string, GLMethod> Methods { get; }
        
        Dictionary<string, GLTypeBase> BasicTypes { get; }
        
        List<GLFeature> Features { get; }
        
        
        IReadOnlyDictionary<string, GLTypeBase> Types { get; }

        GLTypeBase GetType(string name);
    }
}