namespace InDoOut_UI_Common.Removal
{
    public abstract class AbstractElementRemover<Element> : IElementRemover<Element> where Element : class
    {
        public bool CanRemove(object @object) => (@object as Element) != null;

        public bool TryRemove(object @object) => CanRemove(@object) && @object is Element element ? Remove(element) : false;

        public abstract bool Remove(Element element);
    }
}
