﻿// <copyright file="DemoSnippets.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace DemoSnippets.Classifier
{
    internal static class DemoSnippets
    {
        internal const string ContentType = nameof(DemoSnippets);

        internal const string FileExtension = ".demosnippets";

        [Export]
        [Name(ContentType)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition ContentTypeDefinition = null;

        [Export]
        [Name(ContentType + nameof(FileExtensionToContentTypeDefinition))]
        [ContentType(ContentType)]
        [FileExtension(FileExtension)]
        internal static FileExtensionToContentTypeDefinition FileExtensionToContentTypeDefinition = null;
    }
}
