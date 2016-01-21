using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OneCog.SemanticLogging.Service.Utility.Core
{
    public static class NugetExtensions
    {
        private static System.Runtime.Versioning.FrameworkName Net45 = new System.Runtime.Versioning.FrameworkName(".NETFramework", new Version(4, 5));
        private static System.Runtime.Versioning.FrameworkName Net40 = new System.Runtime.Versioning.FrameworkName(".NETFramework", new Version(4, 0));
        private static System.Runtime.Versioning.FrameworkName Native = new System.Runtime.Versioning.FrameworkName("native", new Version(0, 0));
        private static System.Runtime.Versioning.FrameworkName Portable = new System.Runtime.Versioning.FrameworkName("portable", new Version(0, 0));

        public static IEnumerable<IPackageFile> ForNet45(this IEnumerable<IPackageFile> source)
        {
            return source.Where(pf => pf.TargetFramework == Net45);
        }

        public static IEnumerable<IPackageFile> ForNet40(this IEnumerable<IPackageFile> source)
        {
            return source.Where(pf => pf.TargetFramework == Net40);
        }

        public static IEnumerable<IPackageFile> ForNative(this IEnumerable<IPackageFile> source)
        {
            return source.Where(pf => pf.TargetFramework == Native);
        }

        public static IEnumerable<IPackageFile> ForPortableIncluding(this IEnumerable<IPackageFile> source, string profile)
        {
            return source.Where(pf => pf.TargetFramework.IsPortableFramework() && pf.TargetFramework.Profile.Contains(profile));
        }
    }
}
