using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NewGLSpec
{
    public class GLConditionalTypeDef : GLTypeBase
    {
        public GLConditionalTypeDef(string name)
            : base(name)
        {
            TypeRefs = new List<ConditionalTypeRef>();
        }

        public List<ConditionalTypeRef> TypeRefs { get; }
        public struct ConditionalTypeRef
        {
            public ConditionalTypeRef(GLTypeDef underlyingTypeDef, string condition)
            {
                UnderlyingTypeDef = underlyingTypeDef;
                Condition = condition;
            }
            public GLTypeDef UnderlyingTypeDef { get; }
        
            public string Condition { get; }

            public override string ToString()
            {
                return $"{Condition} => {UnderlyingTypeDef}";
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var r in TypeRefs)
            {
                sb.AppendLine(r.ToString());
            }
            return sb.ToString();
        }
    }
}