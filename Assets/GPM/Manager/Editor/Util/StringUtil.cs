using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Gpm.Manager.Util
{
    internal static class StringUtil
    {
        public static bool VersionGreaterThan(this string version, string compareVersion)
        {
            Version v1 = new Version(version);
            Version v2 = new Version(compareVersion);

            return v1 > v2;
        }

        public static bool IsInstallableUnityVersion(string compareVersion)
        {
            UnityVersion current = new UnityVersion(Application.unityVersion);
            UnityVersion compare = new UnityVersion(compareVersion);

            int r = current.Major.CompareTo(compare.Major);
            if (r != 0)
            {
                return r > 0;
            }

            r = current.Minor.CompareTo(compare.Minor);
            if (r != 0)
            {
                return r > 0;
            }

            r = current.Update.CompareTo(compare.Update);
            return r >= 0;
        }

        private sealed class UnityVersion
        {
            private static readonly Regex parseRegex = new Regex(
                @"^(?<major>\d+)(\.(?<minor>\d+))?(\.(?<update>\d+))?(((?<type>[abfp]+)?(?<patch>\w+))|\+)?$",
                RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

            public int Major { get; private set; }
            public int Minor { get; private set; }
            public int Update { get; private set; }
            public string DistType { get; private set; }
            public string Patch { get; private set; }

            public UnityVersion(string version)
            {
                var match = parseRegex.Match(version);

                Major = int.Parse(match.Groups["major"].Value);
                Minor = int.Parse(match.Groups["minor"].Value);
                Update = int.Parse(match.Groups["update"].Value);

                string value = match.Groups["type"].Value;
                if (string.IsNullOrEmpty(value) == false)
                {
                    DistType = value;
                }

                value = match.Groups["patch"].Value;
                if (string.IsNullOrEmpty(value) == false)
                {
                    Patch = value;
                }
            }
        }
    }
}