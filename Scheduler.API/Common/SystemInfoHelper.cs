using System.Text.RegularExpressions;

namespace Scheduler.API.Common
{
    public static class SystemInfoHelper
    {
        public static string GetIPAddress(HttpContext context)
        {
            try
            {
                // Check for forwarded headers (for proxy/load balancer scenarios)
                var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    // Get the first IP from the forwarded header
                    var firstIP = forwardedHeader.Split(',')[0].Trim();
                    if (IsValidIPAddress(firstIP))
                        return firstIP;
                }

                // Check for real IP header
                var realIPHeader = context.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIPHeader) && IsValidIPAddress(realIPHeader))
                    return realIPHeader;

                // Get the remote IP address
                var remoteIP = context.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(remoteIP) && remoteIP != "::1")
                    return remoteIP;

                // Fallback to localhost
                return "127.0.0.1";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetBrowserInfo(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            try
            {
                // Chrome
                if (userAgent.Contains("Chrome"))
                {
                    var match = Regex.Match(userAgent, @"Chrome/(\d+\.\d+)");
                    return match.Success ? $"Chrome {match.Groups[1].Value}" : "Chrome";
                }
                // Firefox
                else if (userAgent.Contains("Firefox"))
                {
                    var match = Regex.Match(userAgent, @"Firefox/(\d+\.\d+)");
                    return match.Success ? $"Firefox {match.Groups[1].Value}" : "Firefox";
                }
                // Safari
                else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                {
                    var match = Regex.Match(userAgent, @"Version/(\d+\.\d+)");
                    return match.Success ? $"Safari {match.Groups[1].Value}" : "Safari";
                }
                // Edge
                else if (userAgent.Contains("Edge"))
                {
                    var match = Regex.Match(userAgent, @"Edge/(\d+\.\d+)");
                    return match.Success ? $"Edge {match.Groups[1].Value}" : "Edge";
                }
                // Internet Explorer
                else if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
                {
                    var match = Regex.Match(userAgent, @"MSIE (\d+\.\d+)");
                    return match.Success ? $"Internet Explorer {match.Groups[1].Value}" : "Internet Explorer";
                }

                return "Unknown Browser";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetOperatingSystem(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            try
            {
                // Windows
                if (userAgent.Contains("Windows"))
                {
                    if (userAgent.Contains("Windows NT 10.0"))
                        return "Windows 10/11";
                    else if (userAgent.Contains("Windows NT 6.3"))
                        return "Windows 8.1";
                    else if (userAgent.Contains("Windows NT 6.2"))
                        return "Windows 8";
                    else if (userAgent.Contains("Windows NT 6.1"))
                        return "Windows 7";
                    else if (userAgent.Contains("Windows NT 6.0"))
                        return "Windows Vista";
                    else if (userAgent.Contains("Windows NT 5.2"))
                        return "Windows Server 2003";
                    else if (userAgent.Contains("Windows NT 5.1"))
                        return "Windows XP";
                    else
                        return "Windows";
                }
                // macOS
                else if (userAgent.Contains("Mac OS X"))
                {
                    var match = Regex.Match(userAgent, @"Mac OS X (\d+[._]\d+)");
                    if (match.Success)
                    {
                        var version = match.Groups[1].Value.Replace('_', '.');
                        return $"macOS {version}";
                    }
                    return "macOS";
                }
                // Linux
                else if (userAgent.Contains("Linux"))
                {
                    if (userAgent.Contains("Ubuntu"))
                        return "Ubuntu Linux";
                    else if (userAgent.Contains("Fedora"))
                        return "Fedora Linux";
                    else if (userAgent.Contains("CentOS"))
                        return "CentOS Linux";
                    else if (userAgent.Contains("Debian"))
                        return "Debian Linux";
                    else
                        return "Linux";
                }
                // iOS
                else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
                {
                    var match = Regex.Match(userAgent, @"OS (\d+[._]\d+)");
                    if (match.Success)
                    {
                        var version = match.Groups[1].Value.Replace('_', '.');
                        return $"iOS {version}";
                    }
                    return "iOS";
                }
                // Android
                else if (userAgent.Contains("Android"))
                {
                    var match = Regex.Match(userAgent, @"Android (\d+\.\d+)");
                    if (match.Success)
                        return $"Android {match.Groups[1].Value}";
                    return "Android";
                }

                return "Unknown OS";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetDeviceType(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            try
            {
                if (userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
                    return "Mobile";
                else if (userAgent.Contains("Tablet"))
                    return "Tablet";
                else
                    return "Desktop";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static bool IsValidIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            // Basic IP validation
            var parts = ip.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int num) || num < 0 || num > 255)
                    return false;
            }

            return true;
        }
    }
}
