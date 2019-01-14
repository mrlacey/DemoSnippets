// <copyright file="OptionPageGrid.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace DemoSnippets
{
    public class OptionPageGrid : DialogPage
    {
        [Category("DemoSnippets")]
        [DisplayName("Auto-load when solution opens")]
        [Description("Automatically load all .demosnippets files in the solution when opened. Also removes them from the Toolbox when the solution is closed.")]
        public bool AutoLoadUnload { get; set; } = true;

        [Category("DemoSnippets")]
        [DisplayName("Refresh Toolbox on file save")]
        [Description("Automatically refresh Toolbox entries from open .demosnippets files when saved.")]
        public bool RefreshOnFileSave { get; set; } = true;
    }
}
