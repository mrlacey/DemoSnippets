// <copyright file="DemoSnippetsLabelFormatDefinition.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace DemoSnippets.Classifier
{
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = DemoSnippetsClassificationTypes.DemoSnippetsLabel)]
    [Name(DemoSnippetsClassificationTypes.DemoSnippetsLabel)]
    internal sealed class DemoSnippetsLabelFormatDefinition : ClassificationFormatDefinition
    {
        public DemoSnippetsLabelFormatDefinition()
        {
            this.IsBold = true;
            this.DisplayName = "DemoSnippets Label";
        }
    }
}
