namespace NewGLSpec
{
    public class GLParameter
    {
        public GLParameter(GLTypeBase type, string name)
        {
            Type = type;
            Name = name;
        }
        public GLTypeBase Type { get; }
        public string Name { get; }

        public override string ToString()
        {
            return $"{Type.Name} {Name}";
        }
    }
}