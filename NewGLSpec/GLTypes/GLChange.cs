using System.Collections.Generic;

namespace NewGLSpec
{
    public abstract class GLChange
    {
        public string Name { get; }
        protected GLChange(string name)
        {
            Name = name;
                
            Additions = new HashSet<GLChangeReference>();
            Removals = new HashSet<GLChangeReference>();
        }
            
        public HashSet<GLChangeReference>  Additions { get; }
        public HashSet<GLChangeReference>  Removals { get; }

        public override string ToString()
        {
            return (Name ?? string.Empty) + "{ +" +  + Additions.Count + ", -" + Removals.Count + " }";
        }
    }
}