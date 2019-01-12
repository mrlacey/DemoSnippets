// <copyright file="AddToToolboxPackage.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
    [InstalledProductRegistration("#110", "#112", "1.1", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(AddToToolboxPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class AddToToolboxPackage : AsyncPackage
    {
        public const string PackageGuidString = "9538932d-8cd5-4512-adb9-4c6b73adf57c";

        public AddToToolboxPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        private static List<ToolboxEntry> TrackedSnippets { get; set; } = new List<ToolboxEntry>();

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

            await ToolboxInteractionLogic.InitializeAsync(this);
            await AddToToolbox.InitializeAsync(this);

            // Since this package might not be initialized until after a solution has finished loading,
            // we need to check if a solution has already been loaded and then handle it.
            bool isSolutionLoaded = await this.IsSolutionLoadedAsync(cancellationToken);

            if (isSolutionLoaded)
            {
                await this.HandleOpenSolutionAsync(cancellationToken);
            }

            // TODO: Make auto-load/unload configurable
            // Listen for subsequent solution events
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenSolution += this.HandleOpenSolution;
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterCloseSolution += this.HandleCloseSolution;
            await RemoveAllDemoSnippets.InitializeAsync(this);
        }

        private void HandleCloseSolution(object sender, EventArgs e)
        {
            this.JoinableTaskFactory.RunAsync(() => this.HandleCloseSolutionAsync(this.DisposalToken)).Task.LogAndForget("DemoSnippets");
        }

        private async Task HandleCloseSolutionAsync(CancellationToken cancellationToken)
        {
            var snippetsToTryAndRemove = new List<ToolboxEntry>(TrackedSnippets);

            foreach (var toTryAndRemove in snippetsToTryAndRemove)
            {
                await ToolboxInteractionLogic.RemoveFromToolboxAsync(toTryAndRemove, cancellationToken);
                TrackedSnippets.Remove(toTryAndRemove);
            }

            var tabsToTryAndRemove = snippetsToTryAndRemove.Select(s => s.Tab).Distinct().ToList();

            foreach (var tab in tabsToTryAndRemove)
            {
                await ToolboxInteractionLogic.RemoveTabIfEmptyAsync(tab, cancellationToken);
            }
        }

        private async Task<bool> IsSolutionLoadedAsync(CancellationToken cancellationToken)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

#pragma warning disable SA1119 // Statement must not use unnecessary parenthesis
            if (!(await this.GetServiceAsync(typeof(SVsSolution)) is IVsSolution solService))
#pragma warning restore SA1119 // Statement must not use unnecessary parenthesis
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

            // Get all *.demosnippets files from the solution
            // Do this now for performance and to avoid thread issues
            if (await this.GetServiceAsync(typeof(DTE)) is DTE dte)
            {
                var fileName = dte.Solution.FileName;

                if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
                {
                    var slnDir = Path.GetDirectoryName(fileName);
                    await this.ProcessAllSnippetFilesAsync(slnDir);
                }

                if (TrackedSnippets.Any())
                {
                    var plural = TrackedSnippets.Count > 1 ? "s" : string.Empty;
                    await OutputPane.Instance.WriteAsync($"Added {TrackedSnippets.Count} snippet{plural} to Toolbox.");
                }
            }
        }

        private async Task ProcessAllSnippetFilesAsync(string slnDirectory)
        {
            await OutputPane.Instance.WriteAsync($"Loading *.demosnippets files under: {slnDirectory}");

            var allSnippetFiles = Directory.EnumerateFiles(slnDirectory, "*.demosnippets", SearchOption.AllDirectories);

            foreach (var snippetFile in allSnippetFiles)
            {
                await OutputPane.Instance.WriteAsync($"Loading items from: {snippetFile}");
                await ToolboxInteractionLogic.LoadToolboxItemsAsync(snippetFile, i => TrackedSnippets.Add(i));
            }
        }
    }
}
