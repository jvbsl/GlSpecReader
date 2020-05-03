namespace NewGLSpec
{
    public abstract class GLTypeBase
    {
        protected GLTypeBase(string name)
        {
            Name = name;
        }
        public string Name { get; }
        

        public override string ToString()
        {
            return Name;
        }

        public virtual bool IsEnum => false;
        public virtual bool IsMethod => false;
        public virtual bool IsPointer => false;
        public virtual bool IsConst => false;
        public virtual bool IsTypeDef => false;
    }
}