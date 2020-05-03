using System.IO;

namespace NewGLSpec.CodeGenerator
{
    public class GLGenerator
    {
        private GLNamespaceGenerator _namespaceGenerator = new GLNamespaceGenerator();
        private EnumGenerator _enumGenerator = new EnumGenerator();
        private MethodGenerator _methodGenerator = new MethodGenerator();
        public void Generate(IGLSpecification spec, GLFeature feature)
        {
            using (var fs = new FileStream(Path.Combine(feature.Name, "OpenGL.csproj"), FileMode.Create))
            using(var str = new StreamWriter(fs))
            {
                str.WriteLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
                str.WriteLine("    <PropertyGroup>");
                str.WriteLine("        <TargetFrameworks>netstandard2.0</TargetFrameworks>");
                str.WriteLine("        <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
                str.WriteLine("    </PropertyGroup>");
                str.WriteLine("</Project>");
            }
            
            foreach (var (profileName, profile) in feature.Profiles)
            {
                const string ext = ".cs";
                const string prefix = "GL.";

                var profileFileAddition = string.IsNullOrEmpty(profileName) ? string.Empty : ("." + profileName);
                
                _namespaceGenerator.Generate(prefix + "Enums" + profileFileAddition + ext, _enumGenerator, spec, feature, profile, profileName);
                _namespaceGenerator.Generate(prefix + "Methods" + profileFileAddition + ext, _methodGenerator, spec, feature, profile, string.Empty);
            }
        }
    }
}