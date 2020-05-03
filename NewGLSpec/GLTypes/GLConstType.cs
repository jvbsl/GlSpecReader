namespace NewGLSpec
{
    public class GLConstType : GLTypeBase
    {
        public GLConstType(GLTypeBase elementType) : base("const " + elementType.Name)
        {
            ElementType = elementType;
        }
        
        public GLTypeBase ElementType { get; }

        public override bool IsConst => true;

        public override string ToString()
        {
            return $"const {ElementType}";
        }
    }
}