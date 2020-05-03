namespace NewGLSpec
{
    public class GLMissingType : GLTypeBase
    {
        public GLMissingType(string name) : base(name)
        {
        }

        public override string ToString()
        {
            return "#MISSING#(" + base.ToString() + ")";
        }
    }
}