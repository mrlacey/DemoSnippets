﻿// <copyright file="DemoSnippetsClassifier.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace DemoSnippets.Classifier
{
    internal class DemoSnippetsClassifier : IClassifier
    {
        private readonly IClassificationType classificationTab;
        private readonly IClassificationType classificationLabel;
        private readonly IClassificationType classificationComment;
        private readonly IClassificationType classificationOther;

        internal DemoSnippetsClassifier(IClassificationTypeRegistryService registry)
        {
            this.classificationTab = registry.GetClassificationType(DemoSnippetsClassificationTypes.DemoSnippetsTab);
            this.classificationLabel = registry.GetClassificationType(DemoSnippetsClassificationTypes.DemoSnippetsLabel);
            this.classificationComment = registry.GetClassificationType(DemoSnippetsClassificationTypes.DemoSnippetsComment);
            this.classificationOther = registry.GetClassificationType(DemoSnippetsClassificationTypes.DemoSnippetsOther);
        }

#pragma warning disable 67 //CS0067
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var list = new List<ClassificationSpan>();

            var text = span.GetText();

            list.Add(new ClassificationSpan(span, this.GetClassificationType(text)));

            return list;
        }

        private IClassificationType GetClassificationType(string line)
        {
            switch (new DemoSnippetsLineTypeIdentifier().GetLineType(line))
            {
                case DemoSnippetsLineType.Comment:
                    return this.classificationComment;

                case DemoSnippetsLineType.Tab:
                    return this.classificationTab;

                case DemoSnippetsLineType.Label:
                    return this.classificationLabel;

                case DemoSnippetsLineType.Other:
                default:
                    return this.classificationOther;
            }
        }
    }
}
