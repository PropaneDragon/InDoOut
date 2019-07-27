using InDoOut_Core.Entities.Functions;
using System;
using System.Threading;

namespace InDoOut_Testing
{
    /// <summary>
    /// Provides extension methods for testing <see cref="IFunction"/> classes.
    /// </summary>
    public static class FunctionTestingExtensions
    {
        /// <summary>
        /// Waits indefinitely (1 day) for <paramref name="function"/> to complete, then continues. This can be used to ensure the running
        /// of a particular function is complete before testing outputs.
        /// </summary>
        /// <param name="function">The function to wait for.</param>
        /// <param name="waitForStart">Whether to wait for the function to start first.</param>
        public static void WaitForCompletion(this IFunction function, bool waitForStart = false)
        {
            WaitForCompletion(function, TimeSpan.FromDays(1), waitForStart);
        }

        /// <summary>
        /// Waits for <paramref name="function"/> to complete within the specified time given by <paramref name="timeout"/> and
        /// returns whether it completed in time or not.
        /// </summary>
        /// <param name="function">The function to wait for.</param>
        /// <param name="timeout">The amount of time to wait for the function to complete.</param>
        /// <param name="waitForStart">Whether to wait for the function to start first.</param>
        /// <returns>Whether the function completed in time or not.</returns>
        public static bool WaitForCompletion(this IFunction function, TimeSpan timeout, bool waitForStart = false)
        {
            if (function != null)
            {
                var endTime = DateTime.UtcNow + timeout;

                if (waitForStart && !function.Running)
                {
                    while (!function.Running && DateTime.UtcNow < endTime)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(1));
                    }
                }

                while (function.Running && DateTime.UtcNow < endTime)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1));
                }

                return !function.Running;
            }

            return false;
        }
    }
}
