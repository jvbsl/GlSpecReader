using System;
using System.Collections.Generic;
using System.Text;

namespace NewGLSpec
{
    public class GLMethod : GLTypeBase
    {
        public GLMethod(GLTypeBase returnTypeBase, string name) : base(name)
        {
            ReturnTypeBase = returnTypeBase;
            Parameters = new List<GLParameter>();
        }
        
        public GLTypeBase ReturnTypeBase { get; }
        
        public List<GLParameter> Parameters { get; }


        public override bool IsMethod => true;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{ReturnTypeBase} {Name}(");

            bool isFirst = true;
            foreach (var p in Parameters)
            {
                if (!isFirst)
                    sb.Append(", ");
                sb.Append(p);
                isFirst = false;
            }
            
            sb.Append(")");

            return sb.ToString();
        }
    }
}