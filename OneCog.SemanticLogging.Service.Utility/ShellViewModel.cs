using System;
using Caliburn.Micro;
using System.IO;
using Caliburn.Micro.Reactive.Extensions;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using NuGet;
using System.Windows.Input;

namespace OneCog.SemanticLogging.Service.Utility
{
    public class ShellViewModel : Screen, IShellViewModel
    {
        private const string NugetApi = "https://packages.nuget.org/api/v2";
        private const string SemanticLoggingServiceName = "SemanticLogging-svc.exe";
        private static readonly string SemanticLoggingServiceInstallationDirectory = Path.Combine(Environment.CurrentDirectory, "Packages");
        private const string SemanticLoggingPackageId = "EnterpriseLibrary.SemanticLogging";
        private const string SemanticLoggingServicePackageId = "EnterpriseLibrary.SemanticLogging.Service";
        private const string SemanticLoggingTextSinksPackageId = "EnterpriseLibrary.SemanticLogging.TextFile";

        private readonly ObservableProperty<bool> _semanticLoggingServiceInstalled;
        private readonly ObservableCommand _installSemanticLoggingService;

        private IDisposable _behaviors;

        public ShellViewModel()
        {
            _semanticLoggingServiceInstalled = new ObservableProperty<bool>(false, this, () => SemanticLoggingServiceInstalled);
            _installSemanticLoggingService = new ObservableCommand();

            _behaviors = new CompositeDisposable(
                InstallSemanticLoggingServiceCommandShouldBeEnabledWhenSemanticLoggingServiceIsNotInstalled(),
                SemanticLoggingServiceShouldBeInstalledWhenInstallSemanticLoggingServiceCommandExecuted()
            );
        }
        
        private IDisposable SemanticLoggingServiceShouldBeInstalledWhenInstallSemanticLoggingServiceCommandExecuted()
        {
            return _installSemanticLoggingService
                .SelectMany(_ => InstallSemanticLoggingService())
                .Subscribe(_ => { }, DetermineWhetherSemanticLoggingServiceIsAvailable);
        }

        //private static IObservable<string> ExecutePowershellCommand(string command)
        //{
        //    return Observable.Create<string>(
        //        observable =>
        //        {
        //            PSDataCollection<string> data = new PSDataCollection<string>();

        //            PowerShell.Create().AddCommand(command).Invoke()
        //        }
        //    );
        //}

        private class FlatPathResolver : IPackagePathResolver
        {
            public string GetInstallPath(IPackage package)
            {
                return SemanticLoggingServiceInstallationDirectory;
            }            

            public string GetPackageDirectory(IPackage package)
            {
                return SemanticLoggingServiceInstallationDirectory;
            }

            public string GetPackageDirectory(string packageId, SemanticVersion version)
            {
                return SemanticLoggingServiceInstallationDirectory;
            }

            public string GetPackageFileName(IPackage package)
            {
                return package.Id;
            }

            public string GetPackageFileName(string packageId, SemanticVersion version)
            {
                return packageId;
            }
        }

        private IObservable<string> InstallSemanticLoggingService()
        {
            return Observable.Create<string>(
                observer =>
                {
                    observer.OnNext(string.Format("Creating package directory", SemanticLoggingServicePackageId, NugetApi));

                    Directory.CreateDirectory(SemanticLoggingServiceInstallationDirectory);

                    IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(NugetApi);
                    PackageManager packageManager = new PackageManager(repo, new FlatPathResolver(), new PhysicalFileSystem(SemanticLoggingServiceInstallationDirectory));

                    observer.OnNext(string.Format("Installing Package '{0}' from Nuget ('{1}')", SemanticLoggingServicePackageId, NugetApi));
                    packageManager.InstallPackage(SemanticLoggingServicePackageId, new SemanticVersion(2, 0, 1406, 1));

                    observer.OnNext(string.Format("Installing Package '{0}' from Nuget ('{1}')", SemanticLoggingPackageId, NugetApi));
                    packageManager.InstallPackage(SemanticLoggingPackageId, new SemanticVersion(2, 0, 1406, 1));

                    observer.OnNext(string.Format("Installing Package '{0}' from Nuget ('{1}')", SemanticLoggingTextSinksPackageId, NugetApi));
                    packageManager.InstallPackage(SemanticLoggingTextSinksPackageId, new SemanticVersion(2, 0, 1406, 1));

                    return Disposable.Empty;
                }
            );
        }

        private IDisposable InstallSemanticLoggingServiceCommandShouldBeEnabledWhenSemanticLoggingServiceIsNotInstalled()
        {
            return _semanticLoggingServiceInstalled
                .Select(installed => !installed)
                .Subscribe(_installSemanticLoggingService);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            DetermineWhetherSemanticLoggingServiceIsAvailable();
        }

        private void DetermineWhetherSemanticLoggingServiceIsAvailable()
        {
            Observable
                .Start(() => File.Exists(Path.Combine(SemanticLoggingServiceInstallationDirectory, SemanticLoggingServiceName)))
                .ObserveOnDispatcher()
                .Subscribe(_semanticLoggingServiceInstalled);
        }
        
        public bool SemanticLoggingServiceInstalled
        {
            get { return _semanticLoggingServiceInstalled.Get(); }
            private set { _semanticLoggingServiceInstalled.Set(value); }
        }

        public ICommand InstallSemanticLoggingServiceCommand
        {
            get { return _installSemanticLoggingService; }
        }
    }
}