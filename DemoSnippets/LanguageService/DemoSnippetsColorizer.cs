// <copyright file="DemoSnippetsColorizer.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DemoSnippets.LanguageService
{
    public class DemoSnippetsColorizer : Colorizer
    {
        public DemoSnippetsColorizer(Microsoft.VisualStudio.Package.LanguageService svc, IVsTextLines buffer, IScanner scanner)
            : base(svc, buffer, scanner)
        {
        }
    }
}
