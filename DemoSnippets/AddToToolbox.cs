using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace DemoSnippets
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddToToolbox
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("edc0c9c2-6d4c-4c5c-855f-6d4e670f519d");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private string SelectedFileName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToToolbox"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AddToToolbox(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandId);
            menuItem.BeforeQueryStatus += this.MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (sender is OleMenuCommand menuCmd)
                {
                    menuCmd.Visible = menuCmd.Enabled = false;

                    if (!this.IsSingleProjectItemSelection(out var hierarchy, out var itemid))
                    {
                        this.SelectedFileName = null;
                        return;
                    }

                    ((IVsProject)hierarchy).GetMkDocument(itemid, out var itemFullPath);
                    var transformFileInfo = new FileInfo(itemFullPath);

                    // Save the name of the selected file so we have it when the command is executed
                    this.SelectedFileName = transformFileInfo.FullName;

                    if (transformFileInfo.Name.ToLowerInvariant().EndsWith(".demosnippets"))
                    {
                        menuCmd.Visible = menuCmd.Enabled = true;
                    }
                }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AddToToolbox Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in AddToToolbox's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new AddToToolbox(package, commandService);
        }

        private bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;

            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
            if (!(Package.GetGlobalService(typeof(SVsShellMonitorSelection)) is IVsMonitorSelection monitorSelection) || solution == null)
            {
                return false;
            }

            var hierarchyPtr = IntPtr.Zero;
            var selectionContainerPtr = IntPtr.Zero;

            try
            {
                var hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out var multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                if (multiItemSelect != null)
                {
                    // multiple items are selected
                    return false;
                }

                if (itemid == VSConstants.VSITEMID_ROOT)
                {
                    // there is a hierarchy root node selected, thus it is not a single item inside a project
                    return false;
                }

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null)
                {
                    return false;
                }

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out var _)))
                {
                    // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
                    return false;
                }

                // if we got this far then there is a single project item selected
                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }


        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            var lines = File.ReadAllLines(this.SelectedFileName);

            var dsp = new DemoSnippetsParser();
            var toAdd = dsp.GetItemsToAdd(lines);

            foreach (var item in toAdd)
            {
                await AddToToolboxAsync(item.Tab, item.Label, item.Snippet);
            }
        }

        private static async Task AddToToolboxAsync(string tab, string label, string actualText)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(CancellationToken.None);

            var tbs = await Instance.ServiceProvider.GetServiceAsync(typeof(IVsToolbox)) as IVsToolbox;

            var itemInfo = new TBXITEMINFO[1];
            var tbItem = new OleDataObject();

            var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            itemInfo[0].bstrText = label;
            itemInfo[0].dwFlags = (uint)__TBXITEMINFOFLAGS.TBXIF_DONTPERSIST;

            tbItem.SetText(actualText, TextDataFormat.Text);

            var tabLabel = "Demo"; // Default value

            if (!string.IsNullOrWhiteSpace(tab))
            {
                tabLabel = tab.Trim();
            }

            tbs?.AddItem(tbItem, itemInfo, tabLabel);
        }

    }
}
