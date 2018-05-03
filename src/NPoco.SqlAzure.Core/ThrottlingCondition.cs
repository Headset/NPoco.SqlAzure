using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NPoco.SqlAzure
{
    public class ThrottlingCondition
    {
        /// <summary>
        /// Provides a compiled regular expression used to extract the reason code from the error message.
        /// 
        /// </summary>
        private static readonly Regex sqlErrorCodeRegEx = new Regex("Code:\\s*(\\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Maintains a collection of key/value pairs where a key is the resource type and a value is the type of throttling applied to the given resource type.
        /// 
        /// </summary>
        private readonly IList<Tuple<ThrottledResourceType, ThrottlingType>> throttledResources = new List<Tuple<ThrottledResourceType, ThrottlingType>>(9);

        /// <summary>
        /// Gets the error number that corresponds to the throttling conditions reported by SQL Database.
        /// 
        /// </summary>
        public const int ThrottlingErrorNumber = 40501;

        /// <summary>
        /// Gets an unknown throttling condition in the event that the actual throttling condition cannot be determined.
        /// 
        /// </summary>
        public static ThrottlingCondition Unknown
        {
            get
            {
                ThrottlingCondition throttlingCondition = new ThrottlingCondition()
                {
                    ThrottlingMode = ThrottlingMode.Unknown
                };
                throttlingCondition.throttledResources.Add(
                    Tuple.Create(ThrottledResourceType.Unknown,
                        ThrottlingType.Unknown));
                return throttlingCondition;
            }
        }

        /// <summary>
        /// Gets the value that reflects the throttling mode in SQL Database.
        /// 
        /// </summary>
        public ThrottlingMode ThrottlingMode { get; private set; }

        /// <summary>
        /// Gets a list of the resources in the SQL Database that were subject to throttling conditions.
        /// 
        /// </summary>
        public IEnumerable<Tuple<ThrottledResourceType, ThrottlingType>> ThrottledResources => throttledResources;

        /// <summary>
        /// Gets a value that indicates whether physical data file space throttling was reported by SQL Database.
        /// 
        /// </summary>
        public bool IsThrottledOnDataSpace => throttledResources.Any(x => x.Item1 == ThrottledResourceType.PhysicalDatabaseSpace);

        /// <summary>
        /// Gets a value that indicates whether physical log space throttling was reported by SQL Database.
        /// 
        /// </summary>
        public bool IsThrottledOnLogSpace => throttledResources.Any(x => x.Item1 == ThrottledResourceType.PhysicalLogSpace);

        /// <summary>
        /// Gets a value that indicates whether transaction activity throttling was reported by SQL Database.
        /// 
        /// </summary>
        public bool IsThrottledOnLogWrite => throttledResources.Any(x => x.Item1 == ThrottledResourceType.LogWriteIoDelay);
        /// <summary>
        /// Gets a value that indicates whether data read activity throttling was reported by SQL Database.
        /// 
        /// </summary>
        public bool IsThrottledOnDataRead => throttledResources.Any(x => x.Item1 == ThrottledResourceType.DataReadIoDelay);

        /// <summary>
        /// Gets a value that indicates whether CPU throttling was reported by SQL Database.
        /// 
        /// </summary>
        public bool IsThrottledOnCpu => throttledResources.Any(x => x.Item1 == ThrottledResourceType.Cpu);

        /// <summary>
        /// Gets a value that indicates whether database size throttling was reported by SQL Database.
        /// 
        /// </summary>
        public bool IsThrottledOnDatabaseSize => throttledResources.Any(x => x.Item1 == ThrottledResourceType.DatabaseSize);

        /// <summary>
        /// Gets a value that indicates whether concurrent requests throttling was reported by SQL Database.
        /// 
        /// </summary>
        public bool IsThrottledOnWorkerThreads => throttledResources.Any(x => x.Item1 == ThrottledResourceType.WorkerThreads);

        /// <summary>
        /// Gets a value that indicates whether throttling conditions were not determined with certainty.
        /// 
        /// </summary>
        public bool IsUnknown => ThrottlingMode == ThrottlingMode.Unknown;

        /// <summary>
        /// Determines throttling conditions from the specified SQL exception.
        /// 
        /// </summary>
        /// <param name="ex">The <see cref="T:System.Data.SqlClient.SqlException"/> object that contains information relevant to an error returned by SQL Server when throttling conditions were encountered.</param>
        /// <returns>
        /// An instance of the object that holds the decoded reason codes returned from SQL Database when throttling conditions were encountered.
        /// </returns>
        public static ThrottlingCondition FromException(SqlException ex)
        {
            if (ex != null)
            {
                foreach (SqlError error in ex.Errors)
                {
                    if (error.Number == 40501)
                        return FromError(error);
                }
            }
            return Unknown;
        }

        /// <summary>
        /// Determines the throttling conditions from the specified SQL error.
        /// 
        /// </summary>
        /// <param name="error">The <see cref="T:System.Data.SqlClient.SqlError"/> object that contains information relevant to a warning or error returned by SQL Server.</param>
        /// <returns>
        /// An instance of the object that holds the decoded reason codes returned from SQL Database when throttling conditions were encountered.
        /// </returns>
        public static ThrottlingCondition FromError(SqlError error)
        {
            if (error != null)
            {
                Match match = sqlErrorCodeRegEx.Match(error.Message);
                if (match.Success && int.TryParse(match.Groups[1].Value, out var result))
                    return FromReasonCode(result);
            }
            return Unknown;
        }

        /// <summary>
        /// Determines the throttling conditions from the specified reason code.
        /// 
        /// </summary>
        /// <param name="reasonCode">The reason code returned by SQL Database that contains the throttling mode and the exceeded resource types.</param>
        /// <returns>
        /// An instance of the object holding the decoded reason codes returned from SQL Database when encountering throttling conditions.
        /// </returns>
        public static ThrottlingCondition FromReasonCode(int reasonCode)
        {
            if (reasonCode <= 0)
                return Unknown;
            ThrottlingMode throttlingMode = (ThrottlingMode) (reasonCode & 3);
            ThrottlingCondition throttlingCondition = new ThrottlingCondition()
            {
                ThrottlingMode = throttlingMode
            };
            int num1 = reasonCode >> 8;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.PhysicalDatabaseSpace,
                    (ThrottlingType) (num1 & 3)));
            int num2;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.PhysicalLogSpace,
                    (ThrottlingType) ((num2 = num1 >> 2) & 3)));
            int num3;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.LogWriteIoDelay,
                    (ThrottlingType) ((num3 = num2 >> 2) & 3)));
            int num4;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.DataReadIoDelay,
                    (ThrottlingType) ((num4 = num3 >> 2) & 3)));
            int num5;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.Cpu,
                    (ThrottlingType) ((num5 = num4 >> 2) & 3)));
            int num6;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.DatabaseSize,
                    (ThrottlingType) ((num6 = num5 >> 2) & 3)));
            int num7;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.Internal,
                    (ThrottlingType) ((num7 = num6 >> 2) & 3)));
            int num8;
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.WorkerThreads,
                    (ThrottlingType) ((num8 = num7 >> 2) & 3)));
            throttlingCondition.throttledResources.Add(
                Tuple.Create(ThrottledResourceType.Internal,
                    (ThrottlingType) (num8 >> 2 & 3)));
            return throttlingCondition;
        }

        /// <summary>
        /// Returns a textual representation of the current ThrottlingCondition object, including the information held with respect to throttled resources.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// A string that represents the current ThrottlingCondition object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "Mode: {0} | ", new object[]
            {
                ThrottlingMode
            });
            string[] strArray =
                throttledResources.Where(x => x.Item1 != ThrottledResourceType.Internal).Select(x =>
                                                                                                         string.Format(CultureInfo.CurrentCulture, "{0}: {1}",
                                                                                                                       new object[]
                                                                                                                       {
                                                                                                                           x.Item1,
                                                                                                                           x.Item2
                                                                                                                       })).OrderBy(x => x).ToArray();
            stringBuilder.Append(string.Join(", ", strArray));
            return stringBuilder.ToString();
        }
    }

    public enum ThrottledResourceType
    {
        Unknown = -1,
        PhysicalDatabaseSpace = 0,
        PhysicalLogSpace = 1,
        LogWriteIoDelay = 2,
        DataReadIoDelay = 3,
        Cpu = 4,
        DatabaseSize = 5,
        Internal = 6,
        WorkerThreads = 7,
    }

    public enum ThrottlingType
    {
        None,
        Soft,
        Hard,
        Unknown,
    }

    public enum ThrottlingMode
    {
        Unknown = -1,
        NoThrottling = 0,
        RejectUpdateInsert = 1,
        RejectAllWrites = 2,
        RejectAll = 3,
    }
}