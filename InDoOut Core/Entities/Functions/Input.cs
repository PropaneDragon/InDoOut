using System;
using System.Collections.Generic;
using System.Linq;
using InDoOut_Core.Entities.Core;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Inputs are triggered by <see cref="IOutput"/>s. These intputs are simple connections for
    /// <see cref="IFunction"/>s which can trigger the code within them. The <see cref="IFunction"/> may
    /// perform different actions based on the type of input that was triggered.
    /// </summary>
    public class Input : InteractiveEntity<IFunction, IOutput>, IInput
    {
        /// <summary>
        /// The name of this input.
        /// </summary>
        public string Name { get; protected set; } = "Input";

        /// <summary>
        /// The parent this input belongs to.
        /// </summary>
        public IFunction Parent => Connections.FirstOrDefault();

        private Input() { }

        /// <summary>
        /// Creates a generic input with a parent.
        /// </summary>
        /// <param name="parent">The <see cref="IFunction"/> this input belongs to.</param>
        /// <param name="name">The name of this input.</param>
        public Input(IFunction parent, string name = "Input")
        {
            SetConnection(parent);
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return obj is Input input && Name == input.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Begins processing this input. This calls the <see cref="Parent"/> it is connected to.
        /// </summary>
        /// <param name="triggeredBy">The <see cref="IOutput"/> that triggered this input.</param>
        protected override void Process(IOutput triggeredBy)
        {
            if (Parent != null && Parent.CanBeTriggered(this))
            {
                Parent.Trigger(this);
            }
        }
    }
}
