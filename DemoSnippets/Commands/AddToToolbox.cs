// <copyright file="AddToToolbox.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets.Commands
{
	internal sealed class AddToToolbox : BaseCommand
	{
		public const int CommandId = 0x0100;

		private AddToToolbox(AsyncPackage package, OleMenuCommandService commandService)
		: base(package, commandService)
		{
			var menuCommandId = new CommandID(CommandSet, CommandId);
			var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
			menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
			commandService.AddCommand(menuItem);
		}

		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in AddToToolbox's constructor requires the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new AddToToolbox(package, commandService);
		}

#pragma warning disable VSTHRD100 // Avoid async void methods
		private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
		{
			try
			{
				await OutputPane.Instance.WriteAsync($"Adding snippets from '{this.SelectedFileName}' to the Toolbox.");
				var itemCount = await ToolboxInteractionLogic.LoadToolboxItemsAsync(this.SelectedFileName);

				var plural = itemCount == 1 ? string.Empty : "s";

				await OutputPane.Instance.WriteAsync($"Added {itemCount} snippet{plural} to the Toolbox.");
			}
			catch (Exception exc)
			{
				await OutputPane.Instance.WriteAsync("Error adding to toolbox: " + exc.Message);
			}
		}
	}
}
