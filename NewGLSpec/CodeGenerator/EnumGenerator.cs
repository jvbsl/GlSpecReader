using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewGLSpec.Extensions;

namespace NewGLSpec.CodeGenerator
{
    public class EnumGenerator : IGenerator
    {
        public void Generate(IndentedTextWriter indentedTextWriter, IGLSpecification spec, GLFeature feature, GLFeature.GLProfile profile)
        {
            var collected = feature.CollectFeatures(profile.Name);
            foreach (var e in spec.Enums.Values)
            {
                if (e.Name == "SpecialNumbers")
                    GenerateSpecialNumbers(indentedTextWriter, e, feature, collected);
                else
                    GenerateEnum(indentedTextWriter, e, feature, collected);
            }
        }

        private void GenerateSpecialNumbers(IndentedTextWriter writer, GLEnum glEnum, GLFeature feature, HashSet<GLChangeReference> collected)
        {
            bool any = false;
            using(var sb = new StringBuilderTextWriter(new StringBuilder()))
            using (var tempWriter = new IndentedTextWriter(sb))
            {
                tempWriter.Indent = writer.Indent;
                tempWriter.WriteLine("public static class " + glEnum.Name + "");
                tempWriter.WriteLine("{");

                using (tempWriter.Indentation())
                {
                    foreach (var entry in glEnum.Entries)
                    {
                        var reference = new GLChangeReference("enum", entry.Value.Name);
                    
                        if (!collected.Contains(reference))
                            continue;
                        any = true;
                        string dataType = entry.Value.Value <= byte.MaxValue ? "byte" :
                                        entry.Value.Value <= ushort.MaxValue ? "ushort" :
                                        entry.Value.Value <= uint.MaxValue ? "uint" : "ulong";
                        tempWriter.WriteLine($"public static {dataType} {entry.Value.Name} = {entry.Value.Value};");
                    }
                }
            
                tempWriter.WriteLine("}");

                if (any)
                    writer.WriteLine(sb.StringBuilder.ToString());
            }
        }
        private void GenerateEnum(IndentedTextWriter writer, GLEnum glEnum, GLFeature feature, HashSet<GLChangeReference> collected)
        {

            bool any = false;
            using(var sb = new StringBuilderTextWriter(new StringBuilder()))
            using (var tempWriter = new IndentedTextWriter(sb))
            {
                tempWriter.Indent = writer.Indent;
                tempWriter.WriteLine("enum " + glEnum.Name + " : uint");
                tempWriter.WriteLine("{");

                using (tempWriter.Indentation())
                {
                    foreach (var entry in glEnum.Entries)
                    {
                        var reference = new GLChangeReference("enum", entry.Value.Name);
                    
                        if (!collected.Contains(reference))
                            continue;
                        any = true;
                    
                        tempWriter.WriteLine(entry.Value.Name + " = " + entry.Value.Value + ",");
                    }
                }
            
                tempWriter.WriteLine("}");

                if (any)
                    writer.WriteLine(sb.StringBuilder.ToString());
            }

        }
    }
}