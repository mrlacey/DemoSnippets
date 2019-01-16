// <copyright file="AddAllDemoSnippets.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets.Commands
{
    internal sealed class AddAllDemoSnippets : BaseCommand
    {
        public const int CommandId = 0x0300;

        private AddAllDemoSnippets(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService)
        {
            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in the constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AddAllDemoSnippets(package, commandService);
        }

        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(this.Package.DisposalToken);

            if (await this.Package.GetServiceAsync(typeof(DTE)) is DTE dte)
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
