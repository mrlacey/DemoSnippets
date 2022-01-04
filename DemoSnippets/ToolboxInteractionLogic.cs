// <copyright file="ToolboxInteractionLogic.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private const string ToolboxDataLabel = "DSLabel";
        private const string ToolboxDataFileName = "DSFileName";

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

        public static async Task<int> LoadToolboxItemsAsync(string filePath)
        {
            var lines = File.ReadAllLines(filePath);

            var dsp = new DemoSnippetsParser();
            var toAdd = dsp.GetItemsToAdd(lines);

            var addedCount = 0;

            foreach (var item in toAdd)
            {
                if (string.IsNullOrWhiteSpace(item.Tab))
                {
                    item.Tab = DefaultTabName;
                }

                await AddToToolboxAsync(item.Tab, item.Label, item.Snippet, Path.GetFileName(filePath));
                addedCount += 1;
            }

            return addedCount;
        }

        public static async Task RemoveFromToolboxAsync(ToolboxEntry item, CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                await OutputPane.Instance.WriteAsync($"Removing '{item.Label}' from tab '{item.Tab}'");

                var toolbox = await Instance.ServiceProvider.GetServiceAsync<SVsToolbox, IVsToolbox2>();

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

                            if (name?.ToString() == item.Snippet)
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

            var comparableTabName = tab.Trim().ToLowerInvariant();

            switch (comparableTabName)
            {
                case "general":
                    await OutputPane.Instance.WriteAsync("General tab cannot be removed, even if empty.");
                    return;
                case "clipboard ring":
                    await OutputPane.Instance.WriteAsync("Clipboard Ring tab cannot be removed, even if empty.");
                    return;
                default:
                    try
                    {
                        if (await Instance.ServiceProvider.GetServiceAsync<SVsToolbox, IVsToolbox2>() is IVsToolbox2 toolbox)
                        {
                            IEnumToolboxItems tbItems = null;

                            toolbox?.EnumItems(tab, out tbItems);

                            var dataObjects = new IDataObject[1];
                            uint fetched = 0;

                            var item = tbItems?.Next(1, dataObjects, out fetched); // == VSConstants.S_OK)

                            if (fetched == 0)
                            {
                                await OutputPane.Instance.WriteAsync($"Removing tab '{tab}'");

                                toolbox.RemoveTab(tab);
                            }
                        }
                        else
                        {
                            await OutputPane.Instance.WriteAsync("Failed to access Toolbox.");
                        }
                    }
                    catch (Exception e)
                    {
                        await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
                    }

                    break;
            }
        }

        public static async Task RemoveAllDemoSnippetsAsync(CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                if (await Instance.ServiceProvider.GetServiceAsync<SVsToolbox, IVsToolbox2>() is IVsToolbox2 toolbox)
                {
                    var tabsToRemove = new List<string>();

                    IEnumToolboxTabs tbTabs = null;
                    toolbox?.EnumTabs(out tbTabs);
                    var returnedTabNames = new string[1];

                    // Check each tab
                    while (tbTabs?.Next(1, returnedTabNames, out uint tabsReturned) == VSConstants.S_OK)
                    {
                        var tabName = returnedTabNames[0];

                        // Check entries in the tab
                        IEnumToolboxItems tbItems = null;

                        toolbox?.EnumItems(tabName, out tbItems);

                        var dataObjects = new IDataObject[1];
                        uint fetched = 0;

                        while (tbItems?.Next(1, dataObjects, out fetched) == VSConstants.S_OK)
                        {
                            if (dataObjects[0] != null && fetched == 1)
                            {
                                var itemDataObject = new OleDataObject(dataObjects[0]);

                                if (itemDataObject.ContainsText(TextDataFormat.Text))
                                {
                                    var isDemoSnippet = itemDataObject.GetData(ToolboxDataSourceId);

                                    if (isDemoSnippet != null)
                                    {
                                        var label = itemDataObject.GetData(ToolboxDataLabel).ToString();

                                        await OutputPane.Instance.WriteAsync($"Removing '{label}' from tab '{tabName}'");

                                        toolbox?.RemoveItem(dataObjects[0]);
                                        tabsToRemove.Add(tabName);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var tabToRemove in tabsToRemove.Distinct().ToList())
                    {
                        await RemoveTabIfEmptyAsync(tabToRemove, CancellationToken.None);
                    }
                }
                else
                {
                    await OutputPane.Instance.WriteAsync("Failed to access Toolbox.");
                }
            }
            catch (Exception e)
            {
                await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
            }
        }

        public static async Task<(int files, int snippets)> ProcessAllSnippetFilesAsync(string slnDirectory)
        {
            await OutputPane.Instance.WriteAsync($"Loading *.demosnippets files under: {slnDirectory}");

            var allSnippetFiles = Directory.EnumerateFiles(slnDirectory, "*.demosnippets*", SearchOption.AllDirectories);

            var fileCount = 0;
            var itemsCount = 0;

            foreach (var snippetFile in allSnippetFiles)
            {
                await OutputPane.Instance.WriteAsync($"Loading snippets from: {snippetFile}");

                var added = await LoadToolboxItemsAsync(snippetFile);

                if (added == 0)
                {
                    await OutputPane.Instance.WriteAsync($"Found nothing to add in {snippetFile}");
                }
                else
                {
                    fileCount += 1;
                    itemsCount += added;
                }
            }

            return (fileCount, itemsCount);
        }

        public static async Task<IEnumerable<string>> GetAllTabNamesAsync(CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var result = new List<string>();

            try
            {
                if (await Instance.ServiceProvider.GetServiceAsync<SVsToolbox, IVsToolbox2>() is IVsToolbox2 toolbox)
                {
                    IEnumToolboxTabs tbTabs = null;
                    toolbox?.EnumTabs(out tbTabs);
                    var returnedTabNames = new string[1];

                    while (tbTabs?.Next(1, returnedTabNames, out uint tabsReturned) == VSConstants.S_OK)
                    {
                        var tabName = returnedTabNames[0];

                        result.Add(tabName);
                    }
                }
            }
            catch (Exception e)
            {
                await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
            }

            return result;
        }

        public static async Task RefreshEntriesFromFileAsync(string documentPath)
        {
            await RemoveAllItemsFromFileAsync(documentPath);
            await LoadToolboxItemsAsync(documentPath);
        }

        private static async Task<IDataObject> AddToToolboxAsync(string tab, string label, string actualText, string sourceFileName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            try
            {
                var toolbox = await Instance.ServiceProvider.GetServiceAsync<SVsToolbox, IVsToolbox2>();

                var itemInfo = new TBXITEMINFO[1];
                var tbItem = new OleDataObject();

                // Prefix with a zero-width whitespace so can use any character as first char (@ prefix at start of string indicates a resource reference)
                itemInfo[0].bstrText = "\u200B" + label;
                itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

                tbItem.SetText(actualText, TextDataFormat.Text);

                // Add identifier so can know which entries were added by this tool.
                tbItem.SetData(ToolboxDataSourceId, true);

                // Set label here so easy to read back and use in logging when removing.
                tbItem.SetData(ToolboxDataLabel, label);

                // Store this so can avoid duplicating toolbox entries when repeatedly loading the same file
                tbItem.SetData(ToolboxDataFileName, sourceFileName);

                if (await RemoveExistingItemAsync(tab, label, sourceFileName))
                {
                    await OutputPane.Instance.WriteAsync($"Replacing '{label}' in tab '{tab}'");
                }
                else
                {
                    await OutputPane.Instance.WriteAsync($"Adding '{label}' to tab '{tab}'");
                }

                toolbox?.AddItem(tbItem, itemInfo, tab);

                return tbItem;
            }
            catch (Exception e)
            {
                await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
                return null;
            }
        }

        private static async Task<bool> RemoveExistingItemAsync(string tabName, string label, string originalSourceFile)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            try
            {
                var toolbox = await Instance.ServiceProvider.GetServiceAsync<SVsToolbox, IVsToolbox2>();

                IEnumToolboxTabs tbTabs = null;
                toolbox?.EnumTabs(out tbTabs);
                var returnedTabNames = new string[1];

                while (tbTabs?.Next(1, returnedTabNames, out uint tabsReturned) == VSConstants.S_OK)
                {
                    if (tabName == returnedTabNames[0])
                    {
                        IEnumToolboxItems tbItems = null;

                        toolbox?.EnumItems(tabName, out tbItems);

                        var dataObjects = new IDataObject[1];
                        uint fetched = 0;

                        while (tbItems?.Next(1, dataObjects, out fetched) == VSConstants.S_OK)
                        {
                            if (dataObjects[0] != null && fetched == 1)
                            {
                                var itemDataObject = new OleDataObject(dataObjects[0]);

                                if (itemDataObject.ContainsText(TextDataFormat.Text))
                                {
                                    var isDemoSnippet = itemDataObject.GetData(ToolboxDataSourceId);

                                    if (isDemoSnippet != null)
                                    {
                                        var itemFile = itemDataObject.GetData(ToolboxDataFileName).ToString();

                                        if (itemFile == originalSourceFile)
                                        {
                                            var itemLabel = itemDataObject.GetData(ToolboxDataLabel).ToString();

                                            if (itemLabel == label)
                                            {
                                                toolbox?.RemoveItem(dataObjects[0]);
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
            }

            return false;
        }

        private static async Task RemoveAllItemsFromFileAsync(string filePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            try
            {
                var fileName = Path.GetFileName(filePath);

                if (await Instance.ServiceProvider.GetServiceAsync<SVsToolbox, IVsToolbox2>() is IVsToolbox2 toolbox)
                {
                    IEnumToolboxTabs tbTabs = null;
                    toolbox?.EnumTabs(out tbTabs);
                    var returnedTabNames = new string[1];

                    // Check each tab
                    while (tbTabs?.Next(1, returnedTabNames, out uint tabsReturned) == VSConstants.S_OK)
                    {
                        var tabName = returnedTabNames[0];

                        // Check each item in the tab
                        IEnumToolboxItems tbItems = null;

                        toolbox?.EnumItems(tabName, out tbItems);

                        var dataObjects = new IDataObject[1];
                        uint fetched = 0;

                        while (tbItems?.Next(1, dataObjects, out fetched) == VSConstants.S_OK)
                        {
                            if (dataObjects[0] != null && fetched == 1)
                            {
                                var itemDataObject = new OleDataObject(dataObjects[0]);

                                if (itemDataObject.ContainsText(TextDataFormat.Text))
                                {
                                    var isDemoSnippet = itemDataObject.GetData(ToolboxDataSourceId);

                                    if (isDemoSnippet != null)
                                    {
                                        var dataFileName = itemDataObject.GetData(ToolboxDataFileName).ToString();

                                        if (fileName == dataFileName)
                                        {
                                            toolbox?.RemoveItem(dataObjects[0]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    await OutputPane.Instance.WriteAsync("Failed to access Toolbox.");
                }
            }
            catch (Exception e)
            {
                await OutputPane.Instance.WriteAsync($"Error: {e.Message}{Environment.NewLine}{e.Source}{Environment.NewLine}{e.StackTrace}");
            }
        }
    }
}
