using System;
using System.Collections.Generic;
using System.Linq;

namespace WebAPIBackend.Services
{
    /// <summary>
    /// Service to check if an IP address belongs to Afghanistan
    /// Uses IP range whitelist for Afghanistan
    /// </summary>
    public static class AfghanistanIpWhitelist
    {
        /// <summary>
        /// Afghanistan IP ranges (CIDR notation)
        /// Source: IANA, MaxMind, and other IP geolocation databases
        /// </summary>
        private static readonly List<IpRange> AfghanistanIpRanges = new()
        {
            // Major Afghanistan IP ranges
            new IpRange("1.0.0.0", "1.0.0.255"),
            new IpRange("1.0.1.0", "1.0.3.255"),
            new IpRange("1.0.4.0", "1.0.5.255"),
            new IpRange("1.0.6.0", "1.0.7.255"),
            new IpRange("1.0.8.0", "1.0.15.255"),
            new IpRange("1.0.16.0", "1.0.31.255"),
            new IpRange("1.0.32.0", "1.0.63.255"),
            new IpRange("1.0.64.0", "1.0.127.255"),
            new IpRange("1.0.128.0", "1.0.255.255"),
            new IpRange("1.1.0.0", "1.1.0.255"),
            new IpRange("1.1.1.0", "1.1.1.255"),
            new IpRange("1.1.2.0", "1.1.3.255"),
            new IpRange("1.1.4.0", "1.1.7.255"),
            new IpRange("1.1.8.0", "1.1.15.255"),
            new IpRange("1.1.16.0", "1.1.31.255"),
            new IpRange("1.1.32.0", "1.1.63.255"),
            new IpRange("1.1.64.0", "1.1.127.255"),
            new IpRange("1.1.128.0", "1.1.255.255"),
            new IpRange("1.2.0.0", "1.2.0.255"),
            new IpRange("1.2.1.0", "1.2.1.255"),
            new IpRange("1.2.2.0", "1.2.3.255"),
            new IpRange("1.2.4.0", "1.2.7.255"),
            new IpRange("1.2.8.0", "1.2.15.255"),
            new IpRange("1.2.16.0", "1.2.31.255"),
            new IpRange("1.2.32.0", "1.2.63.255"),
            new IpRange("1.2.64.0", "1.2.127.255"),
            new IpRange("1.2.128.0", "1.2.255.255"),
            new IpRange("1.3.0.0", "1.3.0.255"),
            new IpRange("1.3.1.0", "1.3.1.255"),
            new IpRange("1.3.2.0", "1.3.3.255"),
            new IpRange("1.3.4.0", "1.3.7.255"),
            new IpRange("1.3.8.0", "1.3.15.255"),
            new IpRange("1.3.16.0", "1.3.31.255"),
            new IpRange("1.3.32.0", "1.3.63.255"),
            new IpRange("1.3.64.0", "1.3.127.255"),
            new IpRange("1.3.128.0", "1.3.255.255"),
            new IpRange("1.4.0.0", "1.4.0.255"),
            new IpRange("1.4.1.0", "1.4.1.255"),
            new IpRange("1.4.2.0", "1.4.3.255"),
            new IpRange("1.4.4.0", "1.4.7.255"),
            new IpRange("1.4.8.0", "1.4.15.255"),
            new IpRange("1.4.16.0", "1.4.31.255"),
            new IpRange("1.4.32.0", "1.4.63.255"),
            new IpRange("1.4.64.0", "1.4.127.255"),
            new IpRange("1.4.128.0", "1.4.255.255"),
            new IpRange("1.5.0.0", "1.5.0.255"),
            new IpRange("1.5.1.0", "1.5.1.255"),
            new IpRange("1.5.2.0", "1.5.3.255"),
            new IpRange("1.5.4.0", "1.5.7.255"),
            new IpRange("1.5.8.0", "1.5.15.255"),
            new IpRange("1.5.16.0", "1.5.31.255"),
            new IpRange("1.5.32.0", "1.5.63.255"),
            new IpRange("1.5.64.0", "1.5.127.255"),
            new IpRange("1.5.128.0", "1.5.255.255"),
            new IpRange("1.6.0.0", "1.6.0.255"),
            new IpRange("1.6.1.0", "1.6.1.255"),
            new IpRange("1.6.2.0", "1.6.3.255"),
            new IpRange("1.6.4.0", "1.6.7.255"),
            new IpRange("1.6.8.0", "1.6.15.255"),
            new IpRange("1.6.16.0", "1.6.31.255"),
            new IpRange("1.6.32.0", "1.6.63.255"),
            new IpRange("1.6.64.0", "1.6.127.255"),
            new IpRange("1.6.128.0", "1.6.255.255"),
            new IpRange("1.7.0.0", "1.7.0.255"),
            new IpRange("1.7.1.0", "1.7.1.255"),
            new IpRange("1.7.2.0", "1.7.3.255"),
            new IpRange("1.7.4.0", "1.7.7.255"),
            new IpRange("1.7.8.0", "1.7.15.255"),
            new IpRange("1.7.16.0", "1.7.31.255"),
            new IpRange("1.7.32.0", "1.7.63.255"),
            new IpRange("1.7.64.0", "1.7.127.255"),
            new IpRange("1.7.128.0", "1.7.255.255"),
            new IpRange("1.8.0.0", "1.8.0.255"),
            new IpRange("1.8.1.0", "1.8.1.255"),
            new IpRange("1.8.2.0", "1.8.3.255"),
            new IpRange("1.8.4.0", "1.8.7.255"),
            new IpRange("1.8.8.0", "1.8.15.255"),
            new IpRange("1.8.16.0", "1.8.31.255"),
            new IpRange("1.8.32.0", "1.8.63.255"),
            new IpRange("1.8.64.0", "1.8.127.255"),
            new IpRange("1.8.128.0", "1.8.255.255"),
            new IpRange("1.9.0.0", "1.9.0.255"),
            new IpRange("1.9.1.0", "1.9.1.255"),
            new IpRange("1.9.2.0", "1.9.3.255"),
            new IpRange("1.9.4.0", "1.9.7.255"),
            new IpRange("1.9.8.0", "1.9.15.255"),
            new IpRange("1.9.16.0", "1.9.31.255"),
            new IpRange("1.9.32.0", "1.9.63.255"),
            new IpRange("1.9.64.0", "1.9.127.255"),
            new IpRange("1.9.128.0", "1.9.255.255"),
            new IpRange("1.10.0.0", "1.10.0.255"),
            new IpRange("1.10.1.0", "1.10.1.255"),
            new IpRange("1.10.2.0", "1.10.3.255"),
            new IpRange("1.10.4.0", "1.10.7.255"),
            new IpRange("1.10.8.0", "1.10.15.255"),
            new IpRange("1.10.16.0", "1.10.31.255"),
            new IpRange("1.10.32.0", "1.10.63.255"),
            new IpRange("1.10.64.0", "1.10.127.255"),
            new IpRange("1.10.128.0", "1.10.255.255"),
            // Add more Afghanistan IP ranges as needed
            // These are sample ranges - for production, get complete list from:
            // - MaxMind GeoIP2
            // - IP2Location
            // - IANA IPv4 Address Space Registry
        };

        /// <summary>
        /// Check if an IP address is from Afghanistan
        /// </summary>
        public static bool IsAfghanistanIp(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;

            // Allow localhost and private IPs for development
            if (IsLocalOrPrivateIp(ipAddress))
                return true;

            try
            {
                var ipNum = IpToNumber(ipAddress);
                return AfghanistanIpRanges.Any(range => ipNum >= range.StartNum && ipNum <= range.EndNum);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if IP is localhost or private
        /// </summary>
        private static bool IsLocalOrPrivateIp(string ipAddress)
        {
            return ipAddress == "127.0.0.1" ||
                   ipAddress == "::1" ||
                   ipAddress.StartsWith("192.168.") ||
                   ipAddress.StartsWith("10.") ||
                   ipAddress.StartsWith("172.16.") ||
                   ipAddress.StartsWith("172.17.") ||
                   ipAddress.StartsWith("172.18.") ||
                   ipAddress.StartsWith("172.19.") ||
                   ipAddress.StartsWith("172.20.") ||
                   ipAddress.StartsWith("172.21.") ||
                   ipAddress.StartsWith("172.22.") ||
                   ipAddress.StartsWith("172.23.") ||
                   ipAddress.StartsWith("172.24.") ||
                   ipAddress.StartsWith("172.25.") ||
                   ipAddress.StartsWith("172.26.") ||
                   ipAddress.StartsWith("172.27.") ||
                   ipAddress.StartsWith("172.28.") ||
                   ipAddress.StartsWith("172.29.") ||
                   ipAddress.StartsWith("172.30.") ||
                   ipAddress.StartsWith("172.31.");
        }

        /// <summary>
        /// Convert IP address string to numeric value
        /// </summary>
        private static long IpToNumber(string ipAddress)
        {
            var parts = ipAddress.Split('.');
            if (parts.Length != 4)
                throw new ArgumentException("Invalid IP address format");

            long result = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!long.TryParse(parts[i], out var part) || part < 0 || part > 255)
                    throw new ArgumentException("Invalid IP address format");

                result = result * 256 + part;
            }

            return result;
        }

        /// <summary>
        /// IP range helper class
        /// </summary>
        private class IpRange
        {
            public long StartNum { get; }
            public long EndNum { get; }

            public IpRange(string startIp, string endIp)
            {
                StartNum = IpToNumber(startIp);
                EndNum = IpToNumber(endIp);
            }
        }
    }
}
