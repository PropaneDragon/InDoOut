using InDoOut_UI_Common.Actions.Deleting;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Removal
{
    public class BasicDeletableRemover : AbstractElementRemover<IDeletable>, IDeletableRemover
    {
        protected ICommonProgramDisplay Display { get; private set; } = null;

        private BasicDeletableRemover()
        {
        }

        public BasicDeletableRemover(ICommonProgramDisplay display) : this()
        {
            Display = display;
        }

        public override bool Remove(IDeletable deletable)
        {
            if (Display != null && deletable != null && deletable.CanDelete(Display))
            {
                if (deletable is FrameworkElement frameworkElement)
                {
                    Display.Remove(frameworkElement);
                }

                deletable.Deleted(Display);

                return true;
            }

            return false;
        }
    }
}
