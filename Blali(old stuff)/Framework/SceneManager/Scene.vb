
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio

Namespace Framework.SceneManager
    <TestState(TestState.NearCompletion)>
    Public MustInherit Class Scene
        Implements IDisposable

        Public MustOverride Sub Initialize() 'Initialization routine(replaced by the inheriting class)
        Public MustOverride Sub LoadContent(Optional parameter As Integer = 0) 'Content loading routine(replaced by the inheriting class)
        Public MustOverride Sub UnloadContent() 'Content unloading routine(replaced by the inheriting class)
        Public MustOverride Sub Draw(gameTime As GameTime) 'Rendering loop(replaced by the inheriting class)
        Public MustOverride Sub DrawDebug(gameTime As GameTime) 'Debug Rendering loop(replaced by the inheriting class)
        Public MustOverride Sub Update(gameTime As GameTime) 'Game loop(replaced by the inheriting class)
        Public Property SoundBank As SoundBank
        Public Property Config As SceneConfig 'General information about the scene
        Public Property Loaded As Boolean = False 'Set this to true if assets should be unloaded when switching scenes

        Friend Overridable Function LoadLSInformation(instructionnr As Integer) As String()
            If instructionnr < 0 Then
                Return {"[Insert Level Name Here]", "[Insert Level Description A here]", "[Insert Level Description B here]"}
            Else
                Return {"[Insert Level Name Here]", "[Insert Level Description here]"}
            End If
        End Function 'LS content loading routine(may be replaced by the inheriting class)


#Region "IDisposable Support"
        Private disposedValue As Boolean ' Dient zur Erkennung redundanter Aufrufe.

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
                    SoundBank.Dispose()
                    SoundBank = Nothing
                End If

                ' TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() weiter unten überschreiben.
                ' TODO: große Felder auf Null setzen.
            End If
            disposedValue = True
        End Sub

        ' Dieser Code wird von Visual Basic hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(disposing As Boolean) weiter oben ein.
            Dispose(True)
            ' TODO: Auskommentierung der folgenden Zeile aufheben, wenn Finalize() oben überschrieben wird.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace