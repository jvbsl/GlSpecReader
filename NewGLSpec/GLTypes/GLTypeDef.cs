namespace NewGLSpec
{
    public class GLTypeDef : GLTypeBase
    {
        public GLTypeDef(string name, GLTypeReference originalType) : base(name)
        {
            OriginalType = originalType;
        }
        
        public GLTypeReference OriginalType { get; }

        public override bool IsTypeDef => true;

        public override string ToString()
        {
            return Name;
        }
    }
}