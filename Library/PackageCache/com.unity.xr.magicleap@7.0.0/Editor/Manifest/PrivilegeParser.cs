using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    internal class PrivilegeDescriptor
    {
        public int? apiLevel { get; set; }
        public Privilege.Category category { get; set; }
        public string description { get; set; }
        public string name { get; set; }
    }

    internal static class PrivilegeParser
    {
        const string kPlatformLevelRegex = @"\#define ML_PLATFORM_API_LEVEL (\d+)";
        const string kPrivilegeIDRegex = @"\s+\/\*!\s\n\s+\<b\>Description:\</b\>(.+)\<br/\>\s+\n.+Type:.+(autogranted|reality|sensitive|trusted).+\n.+\n(?:\s+\\apilevel\s(\d+)\s+\n)?\s+\*/\s+\n(?:\s+MLPrivilegeID_(\w+).+\n)+";

        public const string kPlatformHeaderPath = "include/ml_platform.h";
        public const string kPrivilegeHeaderPath = "include/ml_privilege_ids.h";


        public static int ParsePlatformLevelFromHeader(string header_path)
        {
            if (header_path == null || !File.Exists(header_path))
                throw new ArgumentException(string.Format("File '{0}' not found", header_path));
            using (var reader = new StreamReader(header_path))
            {
                var buffer = reader.ReadToEnd().Replace("\n", "\r\n");
                var regex = new Regex(kPlatformLevelRegex);
                var match = regex.Match(buffer);
                return int.Parse(match.Groups[1].Value);
            }
        }

        public static IEnumerable<PrivilegeDescriptor> ParsePrivilegesFromHeader(string header_path)
        {
            if (header_path == null || !File.Exists(header_path))
                throw new ArgumentException(string.Format("File '{0}' not found", header_path));
            using (var reader = new StreamReader(header_path))
            {
                var buffer = reader.ReadToEnd().Replace("\n", "\r\n");
                var regex = new Regex(kPrivilegeIDRegex);
                var matches = regex.Matches(buffer);
                foreach (Match match in matches)
                {
                    if (match.Groups[4].Captures.Count == 1)
                        yield return new PrivilegeDescriptor {
                            apiLevel = ParseAPILevel(match.Groups[3].Value),
                            category = ParseCategory(match.Groups[2].Value),
                            description = match.Groups[1].Value,
                            name = match.Groups[4].Value
                        };
                    else
                        foreach (Capture capture in match.Groups[4].Captures)
                            yield return new PrivilegeDescriptor {
                                apiLevel = ParseAPILevel(match.Groups[3].Value),
                                category = ParseCategory(match.Groups[2].Value),
                                description = match.Groups[1].Value,
                                name = capture.Value
                            };
                }
            }
        }

        private static int? ParseAPILevel(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;
            int value = 0;
            return (int.TryParse(input, out value)) ? new Nullable<int>(value) : null;
        }

        private static Privilege.Category ParseCategory(string input)
        {
            if (input == "autogranted") return Privilege.Category.Autogranted;
            if (input == "reality") return Privilege.Category.Reality;
            if (input == "sensitive") return Privilege.Category.Sensitive;
            if (input == "trusted") return Privilege.Category.Trusted;
            return Privilege.Category.Invalid;
        }
    }
}
