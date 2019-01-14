// <copyright file="DemoSnippetsClassifierProvider.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace DemoSnippets.Classifier
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("DemoSnippets")]
    internal class DemoSnippetsClassifierProvider : IClassifierProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        [Import]
        private IClassificationTypeRegistryService classificationRegistry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (!this.TextDocumentFactoryService.TryGetTextDocument(buffer, out ITextDocument document))
            {
                return null;
            }

            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new DemoSnippetsClassifier(buffer, this.classificationRegistry, document.FilePath));
        }
    }
}
