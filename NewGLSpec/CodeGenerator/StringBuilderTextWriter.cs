using System.IO;
using System.Text;

namespace NewGLSpec.CodeGenerator
{
    internal class StringBuilderTextWriter : TextWriter
    {
        public StringBuilder StringBuilder { get; }

        public StringBuilderTextWriter(StringBuilder stringBuilder)
        {
            StringBuilder = stringBuilder;
        }
        public override Encoding Encoding { get; }

        public override void Write(char value)
        {
            StringBuilder.Append(value);
        }
    }
}