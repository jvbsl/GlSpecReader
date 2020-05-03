using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewGLSpec.Extensions;

namespace NewGLSpec.CodeGenerator
{
    public class MethodGenerator : IGenerator
    {
        public void Generate(IndentedTextWriter indentedTextWriter, IGLSpecification spec, GLFeature feature, GLFeature.GLProfile profile)
        {
            var collected = feature.CollectFeatures(profile.Name);
            List<(GLMethod m, string name)> delegates = new List<(GLMethod m, string name)>();
            foreach (var t in spec.Types.Values.OfType<GLTypeDef>())
            {
                var resolved = t.OriginalType.Resolved();
                if (resolved.IsMethod)
                {
                    var methodType = (GLMethod) resolved;
                    if (string.IsNullOrEmpty(profile.Name)) // Only add delegates in base profile
                        delegates.Add((methodType, t.Name));
                    continue;
                }

                while (resolved.IsTypeDef)
                    resolved = ((GLTypeDef) resolved).OriginalType.Resolved();
                indentedTextWriter.WriteLine("using " + t.Name + " = " + resolved + ";");
            }
            indentedTextWriter.WriteLine("public static unsafe partial class GL");
            indentedTextWriter.WriteLine("{");
            using (indentedTextWriter.Indentation())
            {
                foreach (var (del, delName) in delegates)
                {
                    indentedTextWriter.Write("public delegate " + del.ReturnTypeBase + " " + delName + "(");
                    bool first = true;
                    foreach (var p in del.Parameters)
                    {
                        if (!first)
                            indentedTextWriter.Write(", ");
                        var pType = p.Type;
                        if (pType.IsConst)
                        {
                            indentedTextWriter.Write("[Const]");
                            pType = ((GLConstType) pType).ElementType;
                        }
                        indentedTextWriter.Write(pType + " " + p.Name);
                        first = false;
                    }
                    indentedTextWriter.WriteLine(");");
                }

                if (!string.IsNullOrEmpty(profile.Name))
                {
                    indentedTextWriter.WriteLine("public static class " + profile.Name);
                    indentedTextWriter.WriteLine("{");
                    indentedTextWriter.Indent++;
                }


                
                foreach (var e in spec.Methods.Values)
                    GenerateMethod(indentedTextWriter, e, feature, collected);
                
                if (!string.IsNullOrEmpty(profile.Name))
                {
                    indentedTextWriter.Indent--;
                    indentedTextWriter.WriteLine("}");
                }
            }
            indentedTextWriter.WriteLine("}");
        }

        private void GenerateMethod(IndentedTextWriter writer, GLMethod glMethod, GLFeature feature, HashSet<GLChangeReference> collected)
        {

            var reference = new GLChangeReference("command", glMethod.Name);

            if (!collected.Contains(reference))
                return;
            
            writer.WriteLine($"[DllImport(\"OpenTK.OpenGL\", EntryPoint = \"{glMethod.Name}\")]");
            writer.Write("public extern static ");
            writer.Write(glMethod.ReturnTypeBase + " " + glMethod.Name);

            writer.Write("(");
            bool isFirst = true;
            foreach (var p in glMethod.Parameters)
            {
                if (!isFirst)
                    writer.Write(", ");
                var pType = p.Type;
                if (pType.IsConst)
                {
                    writer.Write("[Const]");
                    pType = ((GLConstType) pType).ElementType;
                }
                
                writer.Write(pType.Name + " " + NameMangler.MangleParameter(p.Name));

                isFirst = false;
            }
            
            writer.Write(");");
            writer.WriteLine();
        }
    }
}