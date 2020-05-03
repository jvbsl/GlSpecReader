using System;
using System.CodeDom.Compiler;

namespace NewGLSpec.Extensions
{
    public static class IndentationExtension
    {
        public struct Indenter : IDisposable
        {
            private readonly IndentedTextWriter _textWriter;

            public Indenter(IndentedTextWriter textWriter)
            {
                _textWriter = textWriter;
                _textWriter.Indent++;
            }


            public void Dispose()
            {
                _textWriter.Indent--;
            }
        }

        public static Indenter Indentation(this IndentedTextWriter writer)
        {
            return new Indenter(writer);
        }
    }
}