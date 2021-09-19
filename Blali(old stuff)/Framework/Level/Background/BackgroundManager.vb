Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.Background

    <TestState(TestState.NearCompletion)>
    Public Class BackgroundManager
        Inherits List(Of IBackground)
        Public Property Components As New List(Of BackgroundComponent)

        Public Property skyboxPath As String
        Public Property skybox As Texture2D

        Public Sub Init(lvl As Level)
            For Each element In Components
                element.Init(lvl)
            Next
        End Sub

        'Draws all background objects
        Public Sub DrawAll(gameTime As GameTime, front As Boolean)
            For Each element In Me
                If front AndAlso element.IsFront AndAlso element.Visible Then
                    element.Draw(gameTime) : SpriteCount += CUInt(1)
                ElseIf Not front AndAlso Not element.IsFront AndAlso element.Visible Then
                    element.Draw(gameTime) : SpriteCount += CUInt(1)
                End If
            Next

            For Each elementB In Components
                For Each element In elementB.Elements
                    If front AndAlso element.IsFront AndAlso element.Visible Then
                        element.Draw(gameTime) : SpriteCount += CUInt(1)
                    ElseIf Not front AndAlso Not element.IsFront AndAlso element.Visible Then
                        element.Draw(gameTime) : SpriteCount += CUInt(1)
                    End If
                Next
            Next
        End Sub

        'Updates all background objects
        Public Sub UpdateAll(gameTime As GameTime, lvl As Level)
            For Each element In Me
                element.IsFront = element.Layer > cBGLayerSplit
                element.Update(gameTime, lvl.Camera.Location)
            Next

            For Each element In Components
                element.Update(gameTime, lvl)
                For Each elementB In element.Elements
                    elementB.IsFront = elementB.Layer > cBGLayerSplit
                Next
            Next
        End Sub
    End Class
End Namespace