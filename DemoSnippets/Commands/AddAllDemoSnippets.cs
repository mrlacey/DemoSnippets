// <copyright file="AddAllDemoSnippets.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets.Commands
{
    internal sealed class AddAllDemoSnippets
    {
        public const int CommandId = 0x0300;

        public static readonly Guid CommandSet = new Guid("edc0c9c2-6d4c-4c5c-855f-6d4e670f519d");

        private readonly AsyncPackage package;

        private AddAllDemoSnippets(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static AddAllDemoSnippets Instance { get; private set; }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in the constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AddAllDemoSnippets(package, commandService);
        }

        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(this.package.DisposalToken);

            if (await this.package.GetServiceAsync(typeof(DTE)) is DTE dte)
            {
                var fileName = dte.Solution.FileName;

                if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
                {
                    var slnDir = Path.GetDirectoryName(fileName);

#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
                    var (fileCount, snippetCount) = await ToolboxInteractionLogic.ProcessAllSnippetFilesAsync(slnDir);
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly

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
    }
}
