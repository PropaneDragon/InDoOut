using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Core.Variables
{
    /// <summary>
    /// Storage for multiple <see cref="IVariable"/>s. Allows for searching through and returning values
    /// of variables within.
    /// </summary>
    public class VariableStore : IVariableStore
    {
        /// <summary>
        /// All variables currently in storage.
        /// </summary>
        protected List<IVariable> Variables { get; } = new List<IVariable>();

        /// <summary>
        /// Returns an <see cref="IVariable"/> that matches the name given by <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the variable to find.</param>
        /// <returns>The variable found from <paramref name="name"/>, or null if not found.</returns>
        public IVariable GetVariable(string name)
        {
            return Variables.FirstOrDefault(variable => variable.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// Returns the string value of the variable name <paramref name="name"/>, and returns
        /// <paramref name="defaultValue"/> if it doesn't exist.
        /// </summary>
        /// <param name="name">The name of the variable to get the value for.</param>
        /// <param name="defaultValue">The value to return if the variable doesn't exist.</param>
        /// <returns>The string value of the variable.</returns>
        public string GetVariableValue(string name, string defaultValue = null)
        {
            var foundVariable = GetVariable(name);
            return foundVariable == null ? defaultValue : foundVariable.Value;
        }

        /// <summary>
        /// Returns a converted value of type <typeparamref name="T"/> in the variable found from <paramref name="name"/>.
        /// If that conversion fails it returns <paramref name="defaultValue"/> instead.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="name">The name of the variable to find.</param>
        /// <param name="defaultValue">The value to return on failure.</param>
        /// <returns>The converted value of the variable, or <paramref name="defaultValue"/> on failure.</returns>
        public T GetVariableValueAs<T>(string name, T defaultValue = default)
        {
            var foundVariable = GetVariable(name);
            return foundVariable == null ? defaultValue : foundVariable.ValueAs<T>(defaultValue);
        }

        /// <summary>
        /// Sets or creates a variable from the given name <paramref name="name"/> and sets it to the
        /// value <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the variable to set.</param>
        /// <param name="value">The value to set the variable to.</param>
        /// <returns>Whether the variable was set successfully.</returns>
        public bool SetVariable(string name, string value)
        {
            return SetVariable(new Variable(name, value));
        }

        /// <summary>
        /// Sets or creates a variable from the given <paramref name="variable"/>.
        /// </summary>
        /// <param name="variable">The variable to set.</param>
        /// <returns>Whether the variable was set successfully.</returns>
        public bool SetVariable(IVariable variable)
        {
            if (variable != null && variable.Valid)
            {
                var foundVariable = GetVariable(variable.Name);
                if (foundVariable != null)
                {
                    foundVariable.Value = variable.Value;
                }
                else
                {
                    Variables.Add(variable);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether a variable name <paramref name="name"/> exists as a
        /// variable in this storage.
        /// </summary>
        /// <param name="name">The name of the variable to check.</param>
        /// <returns>Whether the given variable name exists.</returns>
        public bool VariableExists(string name)
        {
            return GetVariable(name) != null;
        }
    }
}
