﻿using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIInput : UserControl, IUIInput
    {
        private IInput _input = null;

        public IInput AssociatedInput { get => _input; set => SetInput(value); }

        public UIInput() : base()
        {
            InitializeComponent();
        }

        public UIInput(IInput input) : this()
        {
            AssociatedInput = input;
        }

        private void SetInput(IInput input)
        {
            if (_input != null)
            {
                //Todo: Teardown old input
            }

            _input = input;

            if (_input != null)
            {
                //Warning: Potential bug? INamed has no safe version.
                IO_Main.Text = _input.Name;
            }
        }
    }
}