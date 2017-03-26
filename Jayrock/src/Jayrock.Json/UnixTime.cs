#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (www.raboof.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock
{
    #region Imports

    using System;

    #endregion

    public sealed class UnixTime
    {
        public static readonly DateTime EpochUtc = new DateTime(1970, 1, 1);

        /// <summary>
        /// Converts Unix time (UTC) into a DateTime instance that 
        /// represents the same time in local time with a maximum
        /// precision of a milliseconds.
        /// </summary>
        /// <remarks>
        /// This method works with time expressed up through 
        /// 23:59:59, December 31, 3000, UTC.
        /// </remarks>
        
        public static DateTime ToDateTime(double time)
        {
            return ToDateTime((long) time, (int) Math.Round(time % 1 * 1000));
        }
        
        /// <summary>
        /// Converts Unix time (UTC) into a DateTime instance that 
        /// represents the same time in local time with a maximum
        /// precision of a second.
        /// </summary>
        /// <remarks>
        /// This method works with time expressed up through 
        /// 23:59:59, December 31, 3000, UTC.
        /// </remarks>

        public static DateTime ToDateTime(long time)
        {
            return ToDateTime(time, 0);
        }
        
        /// <summary>
        /// Converts a 64-bit Unix time (UTC) into a DateTime instance that
        /// represents the same time in local time (precision can be a second
        /// or a millisecond depending on whether the second parameter is 
        /// zero or not).
        /// </summary>
        /// <remarks>
        /// See <a href="http://en.wikipedia.org/wiki/Unix_time">Unix time on Wikipedia</a>
        /// for more information. This method works with time expressed 
        /// up through 23:59:59, December 31, 3000, UTC.
        /// </remarks>

        public static DateTime ToDateTime(long time, int ms)
        {
            if (time > 0)
            {
                if (ms < 0 || ms > 999)
                    throw new ArgumentOutOfRangeException("ms");
            }
            else if (time < 0)
            {
                if (ms < -999 || ms > 0)
                    throw new ArgumentOutOfRangeException("ms");
            }
            else
            {
                if (ms < -999 || ms > 999)
                    throw new ArgumentOutOfRangeException("ms");
            }

            return EpochUtc.AddSeconds(time).AddMilliseconds(ms).ToLocalTime();
        }
        
        /// <summary>
        /// Converts a DateTime instance (assumed to represent local time)
        /// to Unix time (UTC) with a maximum precision of a second.
        /// </summary>

        public static long ToInt64(DateTime time)
        {
            return (long) ToDouble(time);
        }

        /// <summary>
        /// Converts a DateTime instance (assumed to represent local time)
        /// to Unix time (UTC) with a precision of over a second (i.e.
        /// fractional part of the returned float-point value is fractions
        /// of a second).
        /// </summary>

        public static double ToDouble(DateTime time)
        {
            return (time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private UnixTime()
        {
            throw new NotSupportedException();
        }
    }
}
