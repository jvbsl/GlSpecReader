using System.CodeDom.Compiler;

namespace NewGLSpec.CodeGenerator
{
    public interface IGenerator
    {
        void Generate(IndentedTextWriter indentedTextWriter, IGLSpecification spec, GLFeature feature, GLFeature.GLProfile profile);
    }
}