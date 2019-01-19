' DEMOSNIPPETS-TAB VB Demos
'DEMOSNIPPETS-LABEL a) imports
Imports System.Runtime.CompilerServices

'DEMOSNIPPETS-ENDSNIPPET
Namespace Helpers
    Public Module TaskExtensions
'DEMOSNIPPETS-LABEL b) method
        <Extension>
        Public Sub FireAndForget(task As Task)
            ' This method allows you to call an async method without awaiting it.
            ' Use it when you don't want or need to wait for the task to complete.
        End Sub
        ''' DEMOSNIPPETS-ENDSNIPPET
    End Module
End Namespace
