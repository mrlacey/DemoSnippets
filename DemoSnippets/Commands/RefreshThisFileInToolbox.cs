// <copyright file="RefreshThisFileInToolbox.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets.Commands
{
    internal sealed class RefreshThisFileInToolbox : BaseCommand
    {
        public const int CommandId = 0x0500;

        private RefreshThisFileInToolbox(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService)
        {
            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in command's constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new RefreshThisFileInToolbox(package, commandService);
        }

        private async void Execute(object sender, EventArgs e)
        {
            await ToolboxInteractionLogic.RefreshEntriesFromFileAsync(this.SelectedFileName);
        }
    }
}
