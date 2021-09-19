Imports System.IO
Namespace Framework.Misc

    <TestState(TestState.NearCompletion)>
    Friend Module ErrorLogger
        Friend Folder As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Emmond"
        Friend ErrPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Emmond\errors.log"

        Friend Sub NoteError(ex As Exception, unhandeled As Boolean)
            'Create file stream
            Dim streamw As StreamWriter
            If Not Directory.Exists(Folder) Then Directory.CreateDirectory(Folder)
            If Not File.Exists(ErrPath) Then
                streamw = File.CreateText(ErrPath)
            Else
                Dim oldtxt As String = File.ReadAllText(ErrPath)
                streamw = New StreamWriter(ErrPath)
                streamw.Write(oldtxt)
            End If
            'Write date/time
            streamw.Write("[" & Date.Now.ToString & "] ")
            If Not unhandeled Then
                streamw.Write("Handeled Exception: " & ex.ToString)
            Else
                streamw.Write("Unhandeled Exception: " & ex.ToString)
            End If
            streamw.WriteLine()
            streamw.Close()
            streamw.Dispose()
        End Sub
    End Module

End Namespace