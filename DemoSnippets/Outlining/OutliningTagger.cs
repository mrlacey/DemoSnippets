// <copyright file="OutliningTagger.cs" company="Matt Lacey Ltd.">
// Copyright (c) Matt Lacey Ltd. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace DemoSnippets.Outlining
{
    internal sealed class OutliningTagger : ITagger<IOutliningRegionTag>
    {
        string startHide = "[";     //the characters that start the outlining region
        string endHide = "]";       //the characters that end the outlining region
        string ellipsis = "...";    //the characters that are displayed when the region is collapsed
        string hoverText = "hover text"; //the contents of the tooltip for the collapsed span
        ITextBuffer buffer;
        ITextSnapshot snapshot;
        List<Region> regions;
        bool initialParse = true;

        public OutliningTagger(ITextBuffer buffer)
        {
            this.buffer = buffer;
            this.snapshot = buffer.CurrentSnapshot;
            this.regions = new List<Region>();
            this.ReParse();
            this.buffer.Changed += BufferChanged;
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
                yield break;

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

                    //the region starts at the beginning of the "[", and goes until the *end* of the line that contains the "]".
                    yield return new TagSpan<IOutliningRegionTag>(
                        new SnapshotSpan(startLine.Start + region.StartOffset,
                        endLine.End),
                        new OutliningRegionTag(false, false, ellipsis, hoverText));
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            // If this isn't the most up-to-date version of the buffer, then ignore it for now (we'll eventually get another change event).
            if (e.After != this.buffer.CurrentSnapshot)
                return;
            this.ReParse();
        }

        void ReParse()
        {
            ITextSnapshot newSnapshot = this.buffer.CurrentSnapshot;
            List<Region> newRegions = new List<Region>();

            //keep the current (deepest) partial region, which will have
            // references to any parent partial regions.
            PartialRegion currentRegion = null;
            PartialRegion currentTab = null;
            PartialRegion currentLabel = null;

            foreach (var line in newSnapshot.Lines)
            {
                int regionStart = -1;
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

                    // close any existing label regions
                    // close any open tab regions
                    // start a new tab region

                    currentTab = new PartialRegion()
                    {
                        StartLine = line.LineNumber,
                        StartOffset = text.TrimEnd().Length,
                    };
                }
                else if (text.StartsWith(DemoSnippetsLineTypeIdentifier.LineStartLabel, StringComparison.InvariantCultureIgnoreCase))
                {
                    // close any existing label regions
                    // start a new label region
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

                // if end of file
                // close any open regions

                //////  if ((regionStart = text.IndexOf(startHide, StringComparison.Ordinal)) != -1)
                ////{
                ////    regionStart = 0;
                ////    int currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
                ////    int newLevel;
                ////    if (!TryGetLevel(text, regionStart, out newLevel))
                ////        newLevel = currentLevel + 1;

                ////    //levels are the same and we have an existing region;
                ////    //end the current region and start the next
                ////    if (currentLevel == newLevel && currentRegion != null)
                ////    {
                ////        newRegions.Add(new Region()
                ////        {
                ////            Level = currentRegion.Level,
                ////            StartLine = currentRegion.StartLine,
                ////            StartOffset = currentRegion.StartOffset,
                ////            EndLine = line.LineNumber
                ////        });

                ////        currentRegion = new PartialRegion()
                ////        {
                ////            Level = newLevel,
                ////            StartLine = line.LineNumber,
                ////            StartOffset = regionStart,
                ////            PartialParent = currentRegion.PartialParent
                ////        };
                ////    }
                ////    //this is a new (sub)region
                ////    else
                ////    {
                ////        currentRegion = new PartialRegion()
                ////        {
                ////            Level = newLevel,
                ////            StartLine = line.LineNumber,
                ////            StartOffset = regionStart,
                ////            PartialParent = currentRegion
                ////        };
                ////    }
                ////}
                //////lines that contain "]" denote the end of a region
                ////else if ((regionStart = text.IndexOf(endHide, StringComparison.Ordinal)) != -1)
                ////{
                ////    int currentLevel = (currentRegion != null) ? currentRegion.Level : 1;
                ////    int closingLevel;
                ////    if (!TryGetLevel(text, regionStart, out closingLevel))
                ////        closingLevel = currentLevel;

                ////    //the regions match
                ////    if (currentRegion != null &&
                ////        currentLevel == closingLevel)
                ////    {
                ////        newRegions.Add(new Region()
                ////        {
                ////            Level = currentLevel,
                ////            StartLine = currentRegion.StartLine,
                ////            StartOffset = currentRegion.StartOffset,
                ////            EndLine = line.LineNumber
                ////        });

                ////        currentRegion = currentRegion.PartialParent;
                ////    }
                ////}
            }

            //if (buffer.CurrentSnapshot)
            if (initialParse)
            {
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

                initialParse = false;
            }

            //determine the changed span, and send a changed event with the new spans
            List<Span> oldSpans =
                new List<Span>(this.regions.Select(r => AsSnapshotSpan(r, this.snapshot)
                    .TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive)
                    .Span));
            List<Span> newSpans =
                    new List<Span>(newRegions.Select(r => AsSnapshotSpan(r, newSnapshot).Span));

            NormalizedSpanCollection oldSpanCollection = new NormalizedSpanCollection(oldSpans);
            NormalizedSpanCollection newSpanCollection = new NormalizedSpanCollection(newSpans);

            //the changed regions are regions that appear in one set or the other, but not both.
            NormalizedSpanCollection removed =
            NormalizedSpanCollection.Difference(oldSpanCollection, newSpanCollection);

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

        static bool TryGetLevel(string text, int startIndex, out int level)
        {
            level = -1;
            if (text.Length > startIndex + 3)
            {
                if (int.TryParse(text.Substring(startIndex + 1), out level))
                    return true;
            }

            return false;
        }

        static SnapshotSpan AsSnapshotSpan(Region region, ITextSnapshot snapshot)
        {
            var startLine = snapshot.GetLineFromLineNumber(region.StartLine);
            var endLine = (region.StartLine == region.EndLine) ? startLine
                 : snapshot.GetLineFromLineNumber(region.EndLine);
            return new SnapshotSpan(startLine.Start + region.StartOffset, endLine.End);
        }

        class PartialRegion
        {
            public int StartLine { get; set; }
            public int StartOffset { get; set; }
            public int Level { get; set; }
            public PartialRegion PartialParent { get; set; }
        }

        class Region : PartialRegion
        {
            public int EndLine { get; set; }
        }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("text")]
    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            //create a single tagger for each buffer.
            Func<ITagger<T>> sc = delegate () { return new OutliningTagger(buffer) as ITagger<T>; };
            return buffer.Properties.GetOrCreateSingletonProperty<ITagger<T>>(sc);
        }
    }
}
