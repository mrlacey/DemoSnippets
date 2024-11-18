﻿// <copyright file="RemoveEmptyTabs.cs" company="Matt Lacey Ltd.">
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

#pragma warning disable VSTHRD100 // Avoid async void methods
		private async void Execute(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // Avoid async void methods
		{
			try
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(this.Package.DisposalToken);

				await OutputPane.Instance.WriteAsync("Attempting to remove any empty tabs in Toolbox.");

				var tabs = await ToolboxInteractionLogic.GetAllTabNamesAsync(this.Package.DisposalToken);

				foreach (var tab in tabs)
				{
					await ToolboxInteractionLogic.RemoveTabIfEmptyAsync(tab, this.Package.DisposalToken);
				}
			}
			catch (Exception exc)
			{
				await OutputPane.Instance.WriteAsync("Error removing empty tabs: " + exc.Message);
			}
		}
	}
}
