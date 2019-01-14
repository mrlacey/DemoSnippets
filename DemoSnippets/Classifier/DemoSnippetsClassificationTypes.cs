// <copyright file="DemoSnippetsClassificationTypes.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace DemoSnippets.Classifier
{
    internal static class DemoSnippetsClassificationTypes
    {
        public const string DemoSnippetsTab = "ds_tab";
        public const string DemoSnippetsLabel = "ds_label";
        public const string DemoSnippetsComment = PredefinedClassificationTypeNames.Comment;
        public const string DemoSnippetsNaturalLanguage = PredefinedClassificationTypeNames.NaturalLanguage;

        [Export]
        [Name(DemoSnippetsTab)]
        public static ClassificationTypeDefinition DemoSnippetsClassificationTab { get; set; }

        [Export]
        [Name(DemoSnippetsLabel)]
        public static ClassificationTypeDefinition DemoSnippetsClassificationLabel { get; set; }
    }
}
