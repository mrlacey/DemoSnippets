// <copyright file="DemoSnippetsTabFormatDefinition.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace DemoSnippets.Classifier
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = DemoSnippetsClassificationTypes.DemoSnippetsTab)]
    [Name(DemoSnippetsClassificationTypes.DemoSnippetsTab)]
    [UserVisible(true)]
    internal sealed class DemoSnippetsTabFormatDefinition : ClassificationFormatDefinition
    {
        public DemoSnippetsTabFormatDefinition()
        {
            this.BackgroundColor = Colors.LightGray;
            this.BackgroundOpacity = .4;
            this.DisplayName = "DemoSnippets Tab";
        }
    }
}
