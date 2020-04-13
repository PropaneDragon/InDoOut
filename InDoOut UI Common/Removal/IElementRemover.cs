namespace InDoOut_UI_Common.Removal
{
    public interface IElementRemover
    {
        bool CanRemove(object @object);

        bool TryRemove(object @object);
    }

    public interface IElementRemover<Element> : IElementRemover where Element : class
    {
        bool Remove(Element element);
    }
}
