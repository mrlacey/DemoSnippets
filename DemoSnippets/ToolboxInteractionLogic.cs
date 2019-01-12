// <copyright file="ToolboxInteractionLogic.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IDataObject = Microsoft.VisualStudio.OLE.Interop.IDataObject;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets
{
    public class ToolboxInteractionLogic
    {
        private const string DefaultTabName = "Demo";
        private const string ToolboxDataSourceId = "DemoSnippets";

        private readonly AsyncPackage package;

        public ToolboxInteractionLogic(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }

        public static ToolboxInteractionLogic Instance { get; private set; }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            Instance = new ToolboxInteractionLogic(package);
        }

        public static async Task LoadToolboxItemsAsync(string fileName, Action<ToolboxEntry> alsoWithEachItem = null)
        {
            var lines = File.ReadAllLines(fileName);

            var dsp = new DemoSnippetsParser();
            var toAdd = dsp.GetItemsToAdd(lines);

            foreach (var item in toAdd)
            {
                if (string.IsNullOrWhiteSpace(item.Tab))
                {
                    item.Tab = DefaultTabName;
                }

                var obj = await AddToToolboxAsync(item.Tab, item.Label, item.Snippet);

                alsoWithEachItem?.Invoke(item);
            }
        }

        public static async Task RemoveFromToolboxAsync(ToolboxEntry item, CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                var toolbox = await Instance.ServiceProvider.GetServiceAsync(typeof(IVsToolbox)) as IVsToolbox;

                await OutputPane.Instance.WriteAsync($"Removing '{item.Label}' from tab '{item.Tab}'");

                IEnumToolboxItems tbItems = null;

                toolbox?.EnumItems(item.Tab, out tbItems);

                var dataObjects = new IDataObject[1];
                uint fetched = 0;

                bool found = false;

                while (!found && tbItems?.Next(1, dataObjects, out fetched) == VSConstants.S_OK)
                {
                    if (dataObjects[0] != null && fetched == 1)
                    {
                        var itemDataObject = new OleDataObject(dataObjects[0]);

                        if (itemDataObject.ContainsText(TextDataFormat.Text))
                        {
                            var name = itemDataObject.GetText(TextDataFormat.Text);

                            if (name.ToString() == item.Snippet)
                            {
                                toolbox?.RemoveItem(dataObjects[0]);
                                found = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
            }
        }

        public static async Task RemoveTabIfEmptyAsync(string tab, CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // TODO: Implement
        }

        private static async Task<IDataObject> AddToToolboxAsync(string tab, string label, string actualText)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            try
            {
                var toolbox = await Instance.ServiceProvider.GetServiceAsync(typeof(IVsToolbox)) as IVsToolbox;

                var itemInfo = new TBXITEMINFO[1];
                var tbItem = new OleDataObject();

                var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                itemInfo[0].bstrText = label;
                itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

                tbItem.SetText(actualText, TextDataFormat.Text);

                // Add identifier so can know which entries were added by this tool.
                tbItem.SetData(ToolboxDataSourceId, true);

                await OutputPane.Instance.WriteAsync($"Adding '{label}' to tab '{tab}'");

                // TODO: avoid adding duplicate items
                toolbox?.AddItem(tbItem, itemInfo, tab);

                return tbItem;
            }
            catch (Exception e)
            {
                await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
                return null;
            }
        }
    }
}
