using NuGet;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneCog.SemanticLogging.Service.Utility.Core
{
    public class Installer
    {
        private readonly Uri _nugetUri;

        public Installer(Uri nugetUri)
        {
            _nugetUri = nugetUri;
        }

        private async Task CopyPackageFileToPath(string installPath, IPackageFile packageFile)
        {
            using (var fileStream = File.Create(Path.Combine(installPath, Path.GetFileName(packageFile.Path))))
            {
                await packageFile.GetStream().CopyToAsync(fileStream);
            }
        }

        private async Task InstallSemanticLoggingService(string installPath, IPackageRepository repository, IProgress<string> progress)
        {
            IPackage package = repository.FindPackage("EnterpriseLibrary.SemanticLogging.Service", new SemanticVersion(2, 0, 1406, 1));
            foreach (IPackageFile packageFile in package.GetToolFiles().Where(pf => string.IsNullOrWhiteSpace(Path.GetDirectoryName(pf.EffectivePath))))
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
        }

        private async Task InstallSemanticLoggingCore(string installPath, IPackageRepository repository, IProgress<string> progress)
        {
            IPackage package = repository.FindPackage("EnterpriseLibrary.SemanticLogging", new SemanticVersion(2, 0, 1406, 1));
            foreach (IPackageFile packageFile in package.GetLibFiles().ForNet45())
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
        }

        private async Task InstallSemanticLoggingTextFile(string installPath, IPackageRepository repository, IProgress<string> progress)
        {
            IPackage package = repository.FindPackage("EnterpriseLibrary.SemanticLogging.TextFile", new SemanticVersion(2, 0, 1406, 1));
            foreach (IPackageFile packageFile in package.GetLibFiles().ForNet45())
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
        }

        private async Task InstallTransientFaultHandling(string installPath, IPackageRepository repository, IProgress<string> progress)
        {
            IPackage package = repository.FindPackage("EnterpriseLibrary.TransientFaultHandling");

            progress.Report(string.Format("Installing '{0}'", package.GetFullName()));

            foreach (IPackageFile packageFile in package.GetLibFiles().ForNet45())
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
        }
        private async Task InstallTransientFaultHandlingData(string installPath, IPackageRepository repository, IProgress<string> progress)
        {
            IPackage package = repository.FindPackage("EnterpriseLibrary.TransientFaultHandling.Data");

            progress.Report(string.Format("Installing '{0}'", package.GetFullName()));

            foreach (IPackageFile packageFile in package.GetLibFiles().ForNet45())
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
        }

        private async Task InstallNewtonsoftJson(string installPath, IPackageRepository repository, IProgress<string> progress)
        {
            IPackage package = repository.FindPackage("Newtonsoft.Json", SemanticVersion.ParseOptionalVersion("5.0.8"));

            progress.Report(string.Format("Installing '{0}'", package.GetFullName()));

            foreach (IPackageFile packageFile in package.GetLibFiles().ForNet45())
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
        }

        private async Task InstallMicrosoftTraceEvent(string installPath, IPackageRepository repository, IProgress<string> progress)
        {
            IPackage package = repository.FindPackage("Microsoft.Diagnostics.Tracing.TraceEvent", SemanticVersion.ParseOptionalVersion("1.0.15"));

            progress.Report(string.Format("Installing '{0}'", package.GetFullName()));

            foreach (IPackageFile packageFile in package.GetLibFiles().ForNet40())
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
            foreach (IPackageFile packageFile in package.GetLibFiles().ForNative())
            {
                await CopyPackageFileToPath(installPath, packageFile);
            }
        }

        public async Task InstallSemanticLogging(string installPath, IProgress<string> progress)
        {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(_nugetUri.ToString());

            await InstallSemanticLoggingService(installPath, repo, progress);
            await InstallSemanticLoggingCore(installPath, repo, progress);
            await InstallSemanticLoggingTextFile(installPath, repo, progress);
            await InstallTransientFaultHandling(installPath, repo, progress);
            await InstallTransientFaultHandlingData(installPath, repo, progress);
            await InstallNewtonsoftJson(installPath, repo, progress);
            await InstallMicrosoftTraceEvent(installPath, repo, progress);
        }
    }
}
