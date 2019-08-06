using InDoOut_Desktop.UI.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace InDoOut_Desktop.Actions
{
    internal abstract class AbstractWireDragAction : Action
    {
        private IBlockView _view = null;
        private IUIConnectionStart _start = null;
        private IUIConnection _uiConnection = null;

        private AbstractWireDragAction()
        {
        }

        public AbstractWireDragAction(IUIConnectionStart start, IBlockView view) : this()
        {
            if (start != null && view != null)
            {
                _start = start;
                _view = view;

                _uiConnection = _view.Create(start, _view.GetMousePosition());

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
            if (_view != null && _start != null)
            {
                var elementsUnderMouse = _view.GetElementsUnderMouse();
                var viableElement = _view.GetFirstElementOfType<IUIConnectionEnd>(elementsUnderMouse);

                if (elementsUnderMouse != null && viableElement != null && elementsUnderMouse.Count > 0 && ViableEnd(viableElement) && FinishConnection(_start, viableElement))
                {
                    _uiConnection.AssociatedEnd = viableElement;
                    _uiConnection.UpdatePositionFromInputOutput(_view);

                    Finish(null);
                    return true;
                }
            }

            AbortSafely();
            return false;
        }

        protected abstract bool ViableEnd(IUIConnectionEnd endConnection);
        protected abstract bool FinishConnection(IUIConnectionStart start, IUIConnectionEnd end);

        private void UpdateWireForMousePos(Point _)
        {
            if (_uiConnection != null && _start != null && _start is FrameworkElement element)
            {
                var viewMousePosition = _view.GetMousePosition();
                _uiConnection.Start = _view.GetBestSide(element, viewMousePosition);
                _uiConnection.End = viewMousePosition;
            }
            else
            {
                AbortSafely();
            }
        }

        protected void AbortSafely()
        {
            if (_uiConnection != null)
            {
                _view.Remove(_uiConnection);
            }

            Abort();
        }
    }
}
