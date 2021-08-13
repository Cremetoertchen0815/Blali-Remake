Namespace Intros
    Public Class LF
        Inherits Scene

        Dim end_trigger As Boolean = False
        Dim timer As Single
        Dim skipper As VirtualButton

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)
            ClearColor = Color.CornflowerBlue
            skipper = New VirtualButton(New VirtualButton.KeyboardKey(Keys.Enter))

            CreateEntity("placeholder").AddComponent(Of PlaceholderLabel)()
        End Sub

        Public Overrides Sub Update()

            'Transition to new scene
            timer += Time.DeltaTime
            If (timer > 4 Or skipper.IsPressed) And Not end_trigger Then
                end_trigger = True
                Core.StartSceneTransition(New FadeTransition(Function() New MonoNez) With {.FadeInDuration = 0.7, .FadeOutDuration = 1, .FadeToColor = Color.Black, .FadeEaseType = Tweens.EaseType.QuadInOut})
            End If

            MyBase.Update()
        End Sub


        Private Class PlaceholderLabel
            Inherits RenderableComponent

            Dim fnt As NezSpriteFont
            Friend Const Text As String = "Luminous Friend, hell yeah!"

            Public Overrides Sub Initialize()
                MyBase.Initialize()

                fnt = New NezSpriteFont(Core.Content.Load(Of SpriteFont)("intro/bfm/fnt_HKG_17_M"))
            End Sub

            Public Overrides Sub Render(batcher As Batcher, camera As Camera)
                batcher.DrawString(fnt, Text, (New Vector2(1920, 1080) - fnt.MeasureString(Text)) / 2, Me.Color)
            End Sub

            Public Overrides ReadOnly Property Bounds As RectangleF
                Get
                    Return New RectangleF(0, 0, 1920, 1080)
                End Get
            End Property
        End Class
    End Class
End Namespace