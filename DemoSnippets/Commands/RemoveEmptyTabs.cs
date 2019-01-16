// <copyright file="RemoveEmptyTabs.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets.Commands
{
    internal sealed class RemoveEmptyTabs : BaseCommand
    {
        public const int CommandId = 0x0400;

        private RemoveEmptyTabs(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService)
        {
            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in command's constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new RemoveEmptyTabs(package, commandService);
        }

        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(this.Package.DisposalToken);

            await OutputPane.Instance.WriteAsync("Attempting to remove any empty tabs in Toolbox.");

            var tabs = await ToolboxInteractionLogic.GetAllTabNamesAsync(this.Package.DisposalToken);

            foreach (var tab in tabs)
            {
                await ToolboxInteractionLogic.RemoveTabIfEmptyAsync(tab, this.Package.DisposalToken);
            }
        }
    }
}
