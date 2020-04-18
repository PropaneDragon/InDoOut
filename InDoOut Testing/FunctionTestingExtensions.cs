using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace InDoOut_Testing
{
    /// <summary>
    /// Provides extension methods for testing <see cref="IFunction"/> classes.
    /// </summary>
    public static class TriggerableTestingExtensions
    {
        /// <summary>
        /// Waits  for a maximum of 1 minute for <paramref name="triggerable"/> to complete, then continues. This can be used to ensure the running
        /// of a particular triggerable is complete before testing outputs.
        /// </summary>
        /// <param name="triggerable">The triggerable to wait for.</param>
        /// <param name="waitForStart">Whether to wait for the function to start first.</param>
        public static void WaitForCompletion(this ITriggerable triggerable, bool waitForStart = false, int sensitivity = 5)
        {
            _ = WaitForCompletion(triggerable, TimeSpan.FromMinutes(1), waitForStart, sensitivity);
        }

        /// <summary>
        /// Waits for <paramref name="triggerable"/> to complete within the specified time given by <paramref name="timeout"/> and
        /// returns whether it completed in time or not.
        /// </summary>
        /// <param name="triggerable">The triggerable to wait for.</param>
        /// <param name="timeout">The amount of time to wait for the triggerable to complete.</param>
        /// <param name="waitForStart">Whether to wait for the triggerable to start first.</param>
        /// <param name="sensitivity">The sensitivity of the wait timer. This is the amount of milliseconds to wait and check if the <paramref name="triggerable"/> is reporting it's not running anymore (due to it switching functions internally for example).</param>
        /// <returns>Whether the triggerable completed in time or not.</returns>
        public static bool WaitForCompletion(this ITriggerable triggerable, TimeSpan timeout, bool waitForStart = false, int sensitivity = 5)
        {
            if (triggerable != null)
            {
                var stopwatch = Stopwatch.StartNew();

                if (waitForStart && !triggerable.Running)
                {
                    while (!triggerable.Running && stopwatch.Elapsed < timeout)
                    {
                    }

                    if (stopwatch.Elapsed >= timeout)
                    {
                        return false;
                    }
                }

                var notRunningCount = 0;

                while ((triggerable.Running || notRunningCount++ < sensitivity) && stopwatch.Elapsed < timeout)
                {
                    if (triggerable.Running)
                    {
                        notRunningCount = 0;
                    }

                    Thread.Sleep(1);
                }

                return stopwatch.Elapsed >= timeout ? false : !triggerable.Running;
            }

            return false;
        }

        /// <summary>
        /// Gets an <see cref="IInput"/> attached to <paramref name="function"/> from the name <paramref name="inputName"/>.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="inputName">The name of the <see cref="IInput"/> to find.</param>
        /// <returns>The found input from the given name. Returns null if nothing has been found.</returns>
        public static IInput GetInputByName(this IFunction function, string inputName) => FindByName(function.Inputs, inputName);

        /// <summary>
        /// Gets an <see cref="IOutput"/> attached to <paramref name="function"/> from the name <paramref name="outputName"/>.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="outputName">The name of the <see cref="IOutput"/> to find.</param>
        /// <returns>The found output from the given name. Returns null if nothing has been found.</returns>
        public static IOutput GetOutputByName(this IFunction function, string outputName) => FindByName(function.Outputs, outputName);

        /// <summary>
        /// Gets an <see cref="IProperty"/> attached to <paramref name="function"/> from the name <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="propertyName">The name of the <see cref="IProperty"/> to find.</param>
        /// <returns>The found property from the given name. Returns null if nothing has been found.</returns>
        public static IProperty GetPropertyByName(this IFunction function, string propertyName) => FindByName(function.Properties, propertyName);

        /// <summary>
        /// Gets an <see cref="IResult"/> attached to <paramref name="function"/> from the name <paramref name="resultName"/>.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="resultName">The name of the <see cref="IResult"/> to find.</param>
        /// <returns>The found result from the given name. Returns null if nothing has been found.</returns>
        public static IResult GetResultByName(this IFunction function, string resultName) => FindByName(function.Results, resultName);

        /// <summary>
        /// Gets whether a function has an <see cref="IInput"/> attached to it.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="inputName">The name of the <see cref="IInput"/> to find.</param>
        /// <returns>Whether the function has the requested name.</returns>
        public static bool HasInput(this IFunction function, string inputName) => GetInputByName(function, inputName) != null;

        /// <summary>
        /// Gets whether a function has an <see cref="IOutput"/> attached to it.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="outputName">The name of the <see cref="IOutput"/> to find.</param>
        /// <returns>Whether the function has the requested name.</returns>
        public static bool HasOutput(this IFunction function, string outputName) => GetOutputByName(function, outputName) != null;

        /// <summary>
        /// Gets whether a function has an <see cref="IProperty"/> attached to it.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="propertyName">The name of the <see cref="IProperty"/> to find.</param>
        /// <returns>Whether the function has the requested name.</returns>
        public static bool HasProperty(this IFunction function, string propertyName) => GetPropertyByName(function, propertyName) != null;

        /// <summary>
        /// Gets whether a function has an <see cref="IResult"/> attached to it.
        /// </summary>
        /// <param name="function">The function to search within.</param>
        /// <param name="resultName">The name of the <see cref="IResult"/> to find.</param>
        /// <returns>Whether the function has the requested name.</returns>
        public static bool HasResult(this IFunction function, string resultName) => GetResultByName(function, resultName) != null;

        /// <summary>
        /// Sets the value of a property on the <paramref name="function"/> with the <paramref name="propertyName"/> to <paramref name="propertyValue"/>.
        /// </summary>
        /// <param name="function">The function to set the property on.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="propertyValue">The value to set the property to.</param>
        /// <returns>Whether the value was successfully set.</returns>
        public static bool SetPropertyValue(this IFunction function, string propertyName, string propertyValue)
        {
            var foundProperty = function?.GetPropertyByName(propertyName);
            if (foundProperty != null)
            {
                foundProperty.RawValue = propertyValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the value of a result from the result name.
        /// </summary>
        /// <param name="function">The function to get the result from.</param>
        /// <param name="resultName">The name of the result to return the value of.</param>
        /// <returns>The value of the found result. Returns null if no result is found.</returns>
        public static string GetResultValue(this IFunction function, string resultName)
        {
            var foundResult = function?.GetResultByName(resultName);
            return foundResult?.RawValue;
        }

        private static T FindByName<T>(List<T> names, string name) where T : class, INamed
        {
            return names?.FirstOrDefault(named => named.Name == name);
        }
    }
}
