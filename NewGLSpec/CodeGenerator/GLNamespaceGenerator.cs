using System;
using System.CodeDom.Compiler;
using System.IO;
using NewGLSpec.Extensions;

namespace NewGLSpec.CodeGenerator
{
    public class GLNamespaceGenerator
    {
        public void Generate(string outputFileName, IGenerator generator, IGLSpecification spec, GLFeature feature, GLFeature.GLProfile profile, string namesapceName)
        {
            string outputDir = feature.Name;
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);
            using(var fs = new FileStream(Path.Combine(outputDir,outputFileName), FileMode.Create))
            using (var streamWriter = new StreamWriter(fs))
            using (var indentedTextWriter = new IndentedTextWriter(streamWriter))
            {
                indentedTextWriter.WriteLine("using System;");
                indentedTextWriter.WriteLine("using System.Runtime.InteropServices;");
                
                
                indentedTextWriter.WriteLine("namespace OpenTK.Graphics" + (string.IsNullOrWhiteSpace(namesapceName) ? string.Empty : "." + namesapceName));
                indentedTextWriter.WriteLine("{");
                using (indentedTextWriter.Indentation())
                {
                    generator.Generate(indentedTextWriter, spec, feature, profile);
                }
                
                indentedTextWriter.WriteLine("}");
            }
        }
    }
}