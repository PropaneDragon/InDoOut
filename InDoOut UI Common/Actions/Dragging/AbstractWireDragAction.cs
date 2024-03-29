﻿using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Actions.Dragging
{
    public abstract class AbstractWireDragAction : Action
    {
        private readonly ICommonDisplay _display = null;
        private readonly IUIConnectionStart _start = null;
        private readonly IUIConnection _uiConnection = null;

        private AbstractWireDragAction()
        {
        }

        public AbstractWireDragAction(IUIConnectionStart start, ICommonDisplay display) : this()
        {
            if (start != null && display != null && display is ICommonProgramDisplay programDisplay)
            {
                _start = start;
                _display = display;

                _uiConnection = programDisplay?.ConnectionCreator?.Create(start, _display.GetMousePosition());

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
            if (_display != null && _start != null)
            {
                var elementsUnderMouse = _display.GetElementsUnderMouse();
                var viableElement = _display.GetFirstElementOfType<IUIConnectionEnd>(elementsUnderMouse);

                if (elementsUnderMouse != null && viableElement != null && elementsUnderMouse.Count > 0 && ViableEnd(viableElement) && FinishConnection(_start, viableElement))
                {
                    _uiConnection.AssociatedEnd = viableElement;
                    _uiConnection.UpdatePositionFromInputOutput(_display);

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
            if (_uiConnection != null && _start != null && _start is FrameworkElement element && _display is ICommonProgramDisplay programDisplay)
            {
                var viewMousePosition = _display.GetMousePosition();
                _uiConnection.Start = programDisplay.GetBestSide(element, viewMousePosition);
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
                _ = (_display?.DeletableRemover?.Remove(_uiConnection));
            }

            Abort();
        }
    }
}
