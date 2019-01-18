// DEMOSNIPPETS-TAB My C# Demo

// DEMOSNIPPETS-LABEL Step 1

// Add necessary namespace
usings MyCoolNamespace;

// DEMOSNIPPETS-LABEL Step 2

public struct SuperOptions
{
    public int Id { get; set; }
    public string Label { get; set; }
    public bool IsEnabled { get; set; }
}


// DEMOSNIPPETS-LABEL Step 3

if (UserOptions.TryGetOptions(out SuperOptions[] options))
{
    Debug.WriteLine(options.Length);
}
