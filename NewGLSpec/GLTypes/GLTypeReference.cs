using System;

namespace NewGLSpec
{
    public class GLTypeReference : IEquatable<GLTypeReference>
    {
        protected GLTypeBase ResolvedType;
        public IGLSpecification Context { get; }
        public string Name { get; }
        
        public GLTypeReference(IGLSpecification specification, string name)
        {
            Context = specification;
            Name = name;
        }

        public virtual GLTypeBase Resolved()
        {
            if (ResolvedType != null)
                return ResolvedType;
            ResolvedType = Context.GetType(Name);
            return ResolvedType;
        }

        public override string ToString()
        {
            return Resolved()?.ToString() ?? "#UNABLE TO RESOLVE#";
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public bool Equals(GLTypeReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GLTypeReference) obj);
        }
    }
}