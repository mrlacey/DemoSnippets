// <copyright file="DemoSnippetRunningDocTableEvents.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DemoSnippets
{
    internal class DemoSnippetRunningDocTableEvents : IVsRunningDocTableEvents
    {
        private readonly DemoSnippetsPackage package;
        private readonly RunningDocumentTable runningDocumentTable;

        public DemoSnippetRunningDocTableEvents(DemoSnippetsPackage package, RunningDocumentTable runningDocumentTable)
        {
            this.package = package;
            this.runningDocumentTable = runningDocumentTable;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            if (this.package.IsFileRefreshEnabled)
            {
                var documentInfo = this.runningDocumentTable.GetDocumentInfo(docCookie);

                var documentPath = documentInfo.Moniker;

                var extension = Path.GetExtension(documentPath);

                if (extension?.ToLowerInvariant() == ".demosnippets")
                {
                    this.package.JoinableTaskFactory.RunAsync(async () =>
                        await ToolboxInteractionLogic.RefreshEntriesFromFileAsync(documentPath));
                }
            }

            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }
    }
}
