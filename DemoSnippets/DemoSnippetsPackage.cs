// <copyright file="DemoSnippetsPackage.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DemoSnippets.Commands;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets
{
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(DemoSnippetsPackage.PackageGuidString)]
    [ProvideOptionPage(typeof(OptionPageGrid), Vsix.Name, "General", 0, 0, true)]
    [ProvideProfileAttribute(typeof(OptionPageGrid), Vsix.Name, "General", 106, 107, isToolsOptionPage: true, DescriptionResourceID = 108)]
    public sealed class DemoSnippetsPackage : AsyncPackage
    {
        public const string PackageGuidString = "9538932d-8cd5-4512-adb9-4c6b73adf57c";

        public DemoSnippetsPackage()
        {
        }

        public bool IsAutoLoadEnabled
        {
            get
            {
                var options = (OptionPageGrid)this.GetDialogPage(typeof(OptionPageGrid));
                return options.AutoLoadUnload;
            }
        }

        public bool IsFileRefreshEnabled
        {
            get
            {
                var options = (OptionPageGrid)this.GetDialogPage(typeof(OptionPageGrid));
                return options.RefreshOnFileSave;
            }
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await OutputPane.Instance.WriteAsync("The DemoSnippets extension works with .demosnippets files. Learn more at https://github.com/mrlacey/DemoSnippets/formatting.md ");
            await OutputPane.Instance.WriteAsync("If you have problems with this extension, or suggestions for improvement, report them at https://github.com/mrlacey/DemoSnippets/issues/new ");
            await OutputPane.Instance.WriteAsync("If you like this extension please leave a review at https://marketplace.visualstudio.com/items?itemName=MattLaceyLtd.DemoSnippets#review-details ");
            await OutputPane.Instance.WriteAsync(string.Empty);

            await ToolboxInteractionLogic.InitializeAsync(this);
            await AddToToolbox.InitializeAsync(this);
            await RemoveAllDemoSnippets.InitializeAsync(this);
            await AddAllDemoSnippets.InitializeAsync(this);
            await RemoveEmptyTabs.InitializeAsync(this);
            await RefreshThisFileInToolbox.InitializeAsync(this);

            await SponsorRequestHelper.CheckIfNeedToShowAsync();

            await this.SetUpRunningDocumentTableEventsAsync(cancellationToken);

            // Since this package might not be initialized until after a solution has finished loading,
            // we need to check if a solution has already been loaded and then handle it.
            bool isSolutionLoaded = await this.IsSolutionLoadedAsync(cancellationToken);

            if (isSolutionLoaded)
            {
                await this.HandleOpenSolutionAsync(cancellationToken);
            }

            // Listen for subsequent solution events
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenSolution += this.HandleOpenSolution;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterCloseSolution += this.HandleCloseSolution;
        }

        private async Task SetUpRunningDocumentTableEventsAsync(CancellationToken cancellationToken)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var runningDocumentTable = new RunningDocumentTable(this);

            var plugin = new DemoSnippetRunningDocTableEvents(this, runningDocumentTable);

            runningDocumentTable.Advise(plugin);
        }

        private void HandleCloseSolution(object sender, EventArgs e)
        {
            this.JoinableTaskFactory.RunAsync(() => this.HandleCloseSolutionAsync(this.DisposalToken)).Task.LogAndForget("DemoSnippets");
        }

        private async Task HandleCloseSolutionAsync(CancellationToken cancellationToken)
        {
            if (this.IsAutoLoadEnabled)
            {
                await OutputPane.Instance.WriteAsync("Removing DemoSnippets from the Toolbox as solution has been closed.");

                await ToolboxInteractionLogic.RemoveAllDemoSnippetsAsync(cancellationToken);
            }
        }

        private async Task<bool> IsSolutionLoadedAsync(CancellationToken cancellationToken)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            if (!(await this.GetServiceAsync(typeof(SVsSolution)) is IVsSolution solService))
            {
                throw new ArgumentNullException(nameof(solService));
            }

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object value));

            return value is bool isSolOpen && isSolOpen;
        }

        private void HandleOpenSolution(object sender, EventArgs e)
        {
            this.JoinableTaskFactory.RunAsync(() => this.HandleOpenSolutionAsync(this.DisposalToken)).Task.LogAndForget("DemoSnippets");
        }

        private async Task HandleOpenSolutionAsync(CancellationToken cancellationToken)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            if (this.IsAutoLoadEnabled)
            {
                // Get all *.demosnippets files from the solution
                // Do this now for performance and to avoid thread issues
                if (await this.GetServiceAsync(typeof(DTE)) is DTE dte)
                {
                    var fileName = dte.Solution.FileName;

                    if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
                    {
                        var slnDir = Path.GetDirectoryName(fileName);
                        var (fileCount, snippetCount) = await ToolboxInteractionLogic.ProcessAllSnippetFilesAsync(slnDir);

                        var filePlural = fileCount == 1 ? string.Empty : "s";
                        var snippetPlural = snippetCount == 1 ? string.Empty : "s";

                        await OutputPane.Instance.WriteAsync($"Added {snippetCount} snippet{snippetPlural}, from {fileCount} file{filePlural}.");
                    }
                    else
                    {
                        await OutputPane.Instance.WriteAsync("Could not access solution file to use to find .demosnippets files.");
                    }
                }
            }
            else
            {
                await OutputPane.Instance.WriteAsync("Automatic loading of .demosnippets files has been disabled.");
            }
        }
    }
}
