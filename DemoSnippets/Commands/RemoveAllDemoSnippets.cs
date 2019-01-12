// <copyright file="RemoveAllDemoSnippets.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets.Commands
{
    internal sealed class RemoveAllDemoSnippets
    {
        public const int CommandId = 0x0200;

        public static readonly Guid CommandSet = new Guid("edc0c9c2-6d4c-4c5c-855f-6d4e670f519d");

        private readonly AsyncPackage package;

        private RemoveAllDemoSnippets(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static RemoveAllDemoSnippets Instance { get; private set; }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in RemoveAllDemoSnippets's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new RemoveAllDemoSnippets(package, commandService);
        }

        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            await OutputPane.Instance.WriteAsync($"Attempting to remove all demosnippets from the toolbox.");

            await ToolboxInteractionLogic.RemoveAllDemoSnippetsAsync();
        }
    }
}
