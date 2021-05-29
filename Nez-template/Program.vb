Imports System

' <summary>
' The main class.
' </summary>
Public NotInheritable Class Program

    ' <summary>
    ' The main entry point for the application.
    ' </summary>
    <STAThread>
    Friend Shared Sub Main()
        Using game As New GameCore
            game.Run()
        End Using
    End Sub
End Class