using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace DemoSnippets.Classifier
{
    internal class DemoSnippetsClassifier : IClassifier
    {
        private readonly IClassificationType _tab;
        private readonly IClassificationType _label;
        private readonly IClassificationType _comment;
        private readonly IClassificationType _naturalLanguage;
        private readonly ITextBuffer _buffer;
        private DemoSnippetsDocument _doc;
        private bool _isProcessing;
        private string _file;

        internal DemoSnippetsClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry, string file)
        {
            _buffer = buffer;
            _file = file;
            _tab = registry.GetClassificationType(DemoSnippetsClassificationTypes.DemoSnippetsTab);
            _label = registry.GetClassificationType(DemoSnippetsClassificationTypes.DemoSnippetsLabel);

            ParseDocument();

            _buffer.Changed += this.BufferChanged;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        private async void BufferChanged(object sender, TextContentChangedEventArgs e)
        {
            await ParseDocument();
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var list = new List<ClassificationSpan>();

            if (_doc == null || this._isProcessing || span.IsEmpty)
            {
                return list;
            }

            var dsLines = _doc.Descendants().Where(b => b.Span.Start <= span.End && b.Span.Length > 0);

            foreach (var dsLine in dsLines)
            {
                var blockSpan = new Span(dsLine.Span.Start, dsLine.Span.Length);

                try
                {
                    if (span.IntersectsWith(blockSpan))
                    {
                        var all = this.GetClassificationTypes(dsLine, span);

                        foreach (var range in all.Keys)
                        {
                            var snapSpan = new SnapshotSpan(span.Snapshot, range);

                            list.Add(new ClassificationSpan(snapSpan, all[range]));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // For some reason span.IntersectsWith throws in some cases.
                    System.Diagnostics.Debug.Write(ex);
                }
            }

            return list;
        }

        private Dictionary<Span, IClassificationType> GetClassificationTypes(DocumentSnippetLine dsLine, SnapshotSpan span)
        {
            var spans = new Dictionary<Span, IClassificationType>();

            if (dsLine is CommentLine)
            {
                spans.Add(dsLine.ToSimpleSpan(), _comment);
            }
            else if (dsLine is LabelLine)
            {
                spans.Add(dsLine.ToSimpleSpan(), _label);
            }
            else if (dsLine is LabelLine)
            {
                spans.Add(dsLine.ToSimpleSpan(), _tab);
            }
            else
            {
                spans.Add(dsLine.ToSimpleSpan(), _naturalLanguage);
            }

            return spans;
        }

        private async Task ParseDocument()
        {
            if (this._isProcessing)
            {
                return;
            }

            this._isProcessing = true;

            await Task.Run(() =>
            {
                this._doc = this._buffer.CurrentSnapshot.ParseToDemoSnippets(this._file);

                var span = new SnapshotSpan(this._buffer.CurrentSnapshot, 0, this._buffer.CurrentSnapshot.Length);

                this.ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(span));

                this._isProcessing = false;
            });
        }
    }

    internal class DocumentSnippetLine
    {
        protected DocumentSnippetLine()
        {
            this.Span = SourceSpan.Empty;
        }

        public SourceSpan Span { get; set; }

        public Span ToSimpleSpan()
        {
            return new Span(this.Span.Start, this.Span.Length);
        }
    }

    internal class CommentLine : DocumentSnippetLine
    {
    }
    internal class TabLine : DocumentSnippetLine
    {
    }
    internal class LabelLine : DocumentSnippetLine
    {
    }

    internal class DemoSnippetsDocument
    {
        private readonly Dictionary<string, object> dataObjects = new Dictionary<string, object>();

        public SourceSpan Span { get; set; }

        public IEnumerable<DocumentSnippetLine> Descendants()
        {
            throw new NotImplementedException();
        }

        public void SetData(string key, object obj)
        {
            if (dataObjects.ContainsKey(key))
            {
                dataObjects[key] = obj;
            }
            else
            {
                dataObjects.Add(key, obj);
            }
        }

        public object GetData(string key)
        {
            return dataObjects[key];
        }
    }

    internal static class DemoSnippetFactory
    {
        private const string AttachedExceptionKey = "attached-exception";
        public static object _syncRoot = new object();
        private static readonly ConditionalWeakTable<ITextSnapshot, DemoSnippetsDocument> CachedDocuments = new ConditionalWeakTable<ITextSnapshot, DemoSnippetsDocument>();

        public static DemoSnippetsDocument ParseToDemoSnippets(this ITextSnapshot snapshot, string file = null)
        {
            lock (_syncRoot)
            {
                return CachedDocuments.GetValue(snapshot, key =>
                {
                    var text = key.GetText();
                    var markdownDocument = ParseToDemoSnippets(text);
                    return markdownDocument;
                });
            }
        }

        public static Exception GetAttachedException(this DemoSnippetsDocument markdownDocument)
        {
            return markdownDocument.GetData(AttachedExceptionKey) as Exception;
        }

        public static DemoSnippetsDocument ParseToDemoSnippets(string text)
        {
            // Safe version that will always return a MarkdownDocument even if there is an exception while parsing
            DemoSnippetsDocument dsDocument;

            try
            {
                dsDocument = DemoSnippetsLogic.Parse(text);
            }
            catch (Exception ex)
            {
                dsDocument = new DemoSnippetsDocument
                {
                    Span = new SourceSpan(0, text.Length - 1)
                };

                // attach the exception to the document that can be later displayed to the user
                dsDocument.SetData(AttachedExceptionKey, ex);
            }

            return dsDocument;
        }

        internal class DemoSnippetsLogic
        {
            public static DemoSnippetsDocument Parse(string text)
            {
                var dsp = new DemoSnippetsParser();
                var lineItems = dsp.GetItemsToAdd(text.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            }
        }
    }

    /// <summary>A span of text.</summary>
    public struct SourceSpan : IEquatable<SourceSpan>
    {
        public static readonly SourceSpan Empty = new SourceSpan(0, -1);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Markdig.Syntax.SourceSpan" /> struct.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public SourceSpan(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Gets or sets the starting character position from the original text source.
        /// Note that for inline elements, this is only valid if <see cref="M:Markdig.MarkdownExtensions.UsePreciseSourceLocation(Markdig.MarkdownPipelineBuilder)" /> is setup on the pipeline.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the ending character position from the original text source.
        /// Note that for inline elements, this is only valid if <see cref="M:Markdig.MarkdownExtensions.UsePreciseSourceLocation(Markdig.MarkdownPipelineBuilder)" /> is setup on the pipeline.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets the character length of this element within the original source code.
        /// </summary>
        public int Length
        {
            get
            {
                return this.End - this.Start + 1;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.Start > this.End;
            }
        }

        public bool Equals(SourceSpan other)
        {
            if (this.Start == other.Start)
                return this.End == other.End;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SourceSpan))
                return false;
            return this.Equals((SourceSpan)obj);
        }

        public override int GetHashCode()
        {
            return this.Start * 397 ^ this.End;
        }

        public static bool operator ==(SourceSpan left, SourceSpan right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SourceSpan left, SourceSpan right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", (object)this.Start, (object)this.End);
        }
    }
}
