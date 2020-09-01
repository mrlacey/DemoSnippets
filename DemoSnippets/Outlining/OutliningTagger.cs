// <copyright file="OutliningTagger.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace DemoSnippets.Outlining
{
    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        private const string Ellipsis = "...";
        private readonly ITextBuffer buffer;
        private ITextSnapshot snapshot;
        private List<Region> regions;

        public OutliningTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.snapshot = buffer.CurrentSnapshot;
            this.regions = new List<Region>();
            this.ReParse();
            this.buffer.Changed += this.BufferChanged;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                yield break;
            }

            List<Region> currentRegions = this.regions;
            ITextSnapshot currentSnapshot = this.snapshot;
            SnapshotSpan entire = new SnapshotSpan(spans[0].Start, spans[spans.Count - 1].End).TranslateTo(currentSnapshot, SpanTrackingMode.EdgeExclusive);
            int startLineNumber = entire.Start.GetContainingLine().LineNumber;
            int endLineNumber = entire.End.GetContainingLine().LineNumber;

            foreach (var region in currentRegions)
            {
                if (region.StartLine <= endLineNumber &&
                    region.EndLine >= startLineNumber)
                {
                    var startLine = currentSnapshot.GetLineFromLineNumber(region.StartLine);
                    var endLine = currentSnapshot.GetLineFromLineNumber(region.EndLine);

                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End),
                        new OutliningRegionTag(false, false, Ellipsis, string.Empty));
                }
            }
        }

        private static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = (region.StartLine == region.EndLine)
                 ? startLine
                 : snapshot.GetLineFromLineNumber(region.EndLine);

            return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
        }

        private void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != this.buffer.CurrentSnapshot)
            {
                return;
            }

            this.ReParse();
        }

        private void ReParse()
        {
            ITextSnapshot newSnapshot = this.buffer.CurrentSnapshot;
            List<Region> newRegions = new List<Region>();

            PartialRegion currentTab = null;
            PartialRegion currentLabel = null;

            foreach (var line in newSnapshot.Lines)
            {
                string text = line.GetText();

                if (text.StartsWith(DemoSnippetsLineTypeIdentifier.LineStartTab, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (currentLabel != null)
                    {
                        newRegions.Add(new Region()
                        {
                            Level = 2,
                            StartLine = currentLabel.StartLine,
                            StartOffset = currentLabel.StartOffset,
                            EndLine = line.LineNumber - 1
                        });

                        currentLabel = null;
                    }

                    if (currentTab != null)
                    {
                        newRegions.Add(new Region()
                        {
                            Level = 1,
                            StartLine = currentTab.StartLine,
                            StartOffset = currentTab.StartOffset,
                            EndLine = line.LineNumber - 1
                        });

                        currentTab = null;
                    }

                    currentTab = new PartialRegion()
                    {
                        StartLine = line.LineNumber,
                        StartOffset = text.TrimEnd().Length,
                    };
                }
                else if (text.StartsWith(DemoSnippetsLineTypeIdentifier.LineStartLabel, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (currentLabel != null)
                    {
                        newRegions.Add(new Region()
                        {
                            Level = 2,
                            StartLine = currentLabel.StartLine,
                            StartOffset = currentLabel.StartOffset,
                            EndLine = line.LineNumber - 1
                        });

                        currentLabel = null;
                    }

                    currentLabel = new PartialRegion()
                    {
                        StartLine = line.LineNumber,
                        StartOffset = text.TrimEnd().Length,
                        PartialParent = currentTab
                    };
                }
            }

            // Close any regions still open
            if (currentLabel != null)
            {
                newRegions.Add(new Region()
                {
                    Level = 2,
                    StartLine = currentLabel.StartLine,
                    StartOffset = currentLabel.StartOffset,
                    EndLine = newSnapshot.Lines.Count() - 1
                });
            }

            if (currentTab != null)
            {
                newRegions.Add(new Region()
                {
                    Level = 1,
                    StartLine = currentTab.StartLine,
                    StartOffset = currentTab.StartOffset,
                    EndLine = newSnapshot.Lines.Count() - 1
                });

                currentTab = null;
            }

            // determine the changed span, and send a changed event with the new spans
            List<Span> oldSpans =
                new List<Span>(this.regions.Select(r => AsSnapshotSpan(r, this.snapshot)
                    .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                    .Span));
            List<Span> newSpans =
                    new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            NormalizedSpanCollection oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            NormalizedSpanCollection newSpanCollection = new NormalizedSpanCollection(newSpans);

            var removed = NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

            int changeStart = int.MaxValue;
            int changeEnd = -1;

            if (removed.Count > 0)
            {
                changeStart = removed[0].Start;
                changeEnd = removed[removed.Count - 1].End;
            }

            if (newSpans.Count > 0)
            {
                changeStart = Math.Min(changeStart, newSpans[0].Start);
                changeEnd = Math.Max(changeEnd, newSpans[newSpans.Count - 1].End);
            }

            this.snapshot = newSnapshot;
            this.regions = newRegions;

            if (changeStart <= changeEnd)
            {
                ITextSnapshot snap = this.snapshot;
                this.TagsChanged?.Invoke(
                    this,
                    new SnapshotSpanEventArgs(new SnapshotSpan(this.snapshot, Span.FromBounds(changeStart, changeEnd))));
            }
        }

        private class PartialRegion
        {
            public int StartLine { get; set; }

            public int StartOffset { get; set; }

            public int Level { get; set; }

            public PartialRegion PartialParent { get; set; }
        }

        private class Region : PartialRegion
        {
            public int EndLine { get; set; }
        }
    }
}
