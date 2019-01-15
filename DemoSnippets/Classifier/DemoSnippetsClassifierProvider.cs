// <copyright file="DemoSnippetsClassifierProvider.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace DemoSnippets.Classifier
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(DemoSnippets.ContentType)]
    internal class DemoSnippetsClassifierProvider : IClassifierProvider
    {
        [Import]
        private IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer) =>
            buffer.Properties.GetOrCreateSingletonProperty(() =>
                new DemoSnippetsClassifier(this.ClassificationRegistry));
    }
}
