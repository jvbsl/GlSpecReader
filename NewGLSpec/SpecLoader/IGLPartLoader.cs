namespace NewGLSpec
{
    public interface IGLPartLoader<T>
    {
        void LoadPart(IGLSpecification specification, T data);
    }
}