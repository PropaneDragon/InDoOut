using System;

namespace InDoOut_Core.Time
{
    /// <summary>
    /// Utilities for handling time.
    /// </summary>
    public static class TimeUtilities
    {
        /// <summary>
        /// Checks whether the time has occurred since the given <paramref name="time"/>.
        /// </summary>
        /// <param name="this">This time to compare against.</param>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the time has occurred since the given time.</returns>
        public static bool HasOccurredSince(this DateTime @this, DateTime time) => @this >= time;

        /// <summary>
        /// Checks whether the time has occurred within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the time has occurred within the last 5 seconds.
        /// </summary>
        /// <param name="this">This time to compare against.</param>
        /// <param name="time">The time to check.</param>
        /// <returns>Wether the time has occurred within the given time.</returns>
        public static bool HasOccurredWithin(this DateTime @this, TimeSpan time) => @this >= DateTime.Now - time;

        /// <summary>
        /// Checks whether the time has occurred within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the time has occurred within the last 5 seconds.
        /// </summary>
        /// <param name="this">This time to compare against.</param>
        /// <param name="time">The time to check.</param>
        /// <param name="reference">A reference time based around DateTime.Now but can be adjusted for local timezones.</param>
        /// <returns>Wether the time has occurred within the given time.</returns>
        public static bool HasOccurredWithin(this DateTime @this, TimeSpan time, DateTime reference) => @this >= reference - time;
    }
}
