namespace NewGLSpec
{
    public class GLPointerType : GLTypeBase
    {
        public GLPointerType(GLTypeBase elementType) : base(elementType.Name + "*")
        {
            ElementType = elementType;
        }
        
        public GLTypeBase ElementType { get; }

        public override bool IsPointer => true;

        public override string ToString()
        {
            return $"{ElementType}*";
        }
    }
}