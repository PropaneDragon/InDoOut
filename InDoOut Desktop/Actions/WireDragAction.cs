using System.Windows;
using InDoOut_Desktop.UI.Interfaces;

namespace InDoOut_Desktop.Actions
{
    internal class WireDragAction : Action
    {
        private IBlockView _view = null;
        private IUIOutput _uiOutput = null;
        private IUIConnection _uiConnection = null;

        public WireDragAction(IUIOutput output, IBlockView view)
        {
            if (output != null && view != null)
            {
                _uiOutput = output;
                _view = view;

                _uiConnection = _view.Create(output, _view.GetMousePosition());

                if (_uiConnection == null)
                {
                    AbortSafely();
                }
            }
            else
            {
                AbortSafely();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            UpdateWireForMousePos(mousePosition);

            return true;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            if (_view != null)
            {
                var offsetPoint = _view.GetMousePosition();
                offsetPoint.X += 1;

                var elementsUnderMouse = _view.GetElementsAtPoint(mousePosition);
                if (elementsUnderMouse != null && _view.GetFirstElementOfType<IUIInput>(elementsUnderMouse) is IUIInput uiInput && uiInput is FrameworkElement chosenElement && _uiOutput is FrameworkElement uiOutput)
                {
                    _uiConnection.AssociatedInput = uiInput;
                    _uiConnection.End = _view.GetBestSide(chosenElement, _view.GetPosition(uiOutput));

                    Finish(null);
                    return true;
                }
            }

            AbortSafely();
            return false;
        }

        private void UpdateWireForMousePos(Point mousePosition)
        {
            if (_uiConnection != null && _uiOutput != null && _uiOutput is FrameworkElement element)
            {
                var relativeMousePosition = _view.GetMousePosition();
                _uiConnection.Start = _view.GetBestSide(element, relativeMousePosition);
                _uiConnection.End = relativeMousePosition;
            }
            else
            {
                AbortSafely();
            }
        }

        private void AbortSafely()
        {
            if (_uiConnection != null)
            {
                _view.Remove(_uiConnection);
            }

            Abort();
        }
    }
}
