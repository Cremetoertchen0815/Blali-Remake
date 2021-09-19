Imports System
Imports Blali.Framework.UI

' <summary>
' The main class.
' </summary>
Public Module Program

    'Shared variables
    Public Property ScaleMatrix As Matrix
    Friend Property MsgBoxer As MessageBoxer


    ' <summary>
    ' The main entry point for the application.
    ' </summary>
    <STAThread>
    Friend Sub Main()
        Using game As New GameCore
            game.Run()
        End Using
    End Sub
End Module