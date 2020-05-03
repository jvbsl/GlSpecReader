using System.Collections.Generic;

namespace NewGLSpec.CodeGenerator
{
    public class NameMangler
    {
        private static HashSet<string> _keywords = new HashSet<string> {"ref", "params"};
        public static string MangleParameter(string name)
        {
            if (_keywords.Contains(name))
                return "@" + name;
            return name;
        }
    }
}