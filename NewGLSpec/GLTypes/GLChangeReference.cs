using System;

namespace NewGLSpec
{
    public struct GLChangeReference : IEquatable<GLChangeReference>
    {
        public GLChangeReference(string type, string name)
        {
            Type = type;
            Name = name;
        }
            
        public string Type { get; }
        public string Name { get; }

        public bool Equals(GLChangeReference other)
        {
            return Type == other.Type && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj.GetType() == this.GetType() && Equals((GLChangeReference) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name);
        }

        public override string ToString()
        {
            return Type + " " + Name;
        }
    }
}