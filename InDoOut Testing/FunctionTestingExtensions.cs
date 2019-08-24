using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using System;
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
        /// Waits indefinitely (1 day) for <paramref name="triggerable"/> to complete, then continues. This can be used to ensure the running
        /// of a particular triggerable is complete before testing outputs.
        /// </summary>
        /// <param name="triggerable">The triggerable to wait for.</param>
        /// <param name="waitForStart">Whether to wait for the function to start first.</param>
        public static void WaitForCompletion(this ITriggerable triggerable, bool waitForStart = false)
        {
            WaitForCompletion(triggerable, TimeSpan.FromDays(1), waitForStart);
        }

        /// <summary>
        /// Waits for <paramref name="triggerable"/> to complete within the specified time given by <paramref name="timeout"/> and
        /// returns whether it completed in time or not.
        /// </summary>
        /// <param name="triggerable">The triggerable to wait for.</param>
        /// <param name="timeout">The amount of time to wait for the triggerable to complete.</param>
        /// <param name="waitForStart">Whether to wait for the triggerable to start first.</param>
        /// <returns>Whether the triggerable completed in time or not.</returns>
        public static bool WaitForCompletion(this ITriggerable triggerable, TimeSpan timeout, bool waitForStart = false)
        {
            if (triggerable != null)
            {
                var endTime = DateTime.UtcNow + timeout;

                if (waitForStart && !triggerable.Running)
                {
                    while (!triggerable.Running && DateTime.UtcNow < endTime)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(1));
                    }
                }

                while (triggerable.Running && DateTime.UtcNow < endTime)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1));
                }

                return !triggerable.Running;
            }

            return false;
        }

        /// <summary>
        /// Sets the value of a property on the <paramref name="function"/> with the <paramref name="propertyName"/> to <paramref name="propertyValue"/>.
        /// </summary>
        /// <param name="function">The function to set the property on.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="propertyValue">The value to set the property to.</param>
        /// <returns>Whether the value was successfully set.</returns>
        public static bool SetPropertyValue(this IFunction function, string propertyName, string propertyValue)
        {
            if (function != null && !string.IsNullOrEmpty(propertyName))
            {
                var foundProperty = function.Properties.FirstOrDefault(property => property.Name == propertyName);
                if (foundProperty != null)
                {
                    foundProperty.RawValue = propertyValue;
                    return true;
                }
            }

            return false;
        }
    }
}
