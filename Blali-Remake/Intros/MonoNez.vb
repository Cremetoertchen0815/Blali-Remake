Imports Blali.Menu

Namespace Intros
    Public Class MonoNez
        Inherits Scene

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)
            ClearColor = Color.White

            'Add renderables
            CreateEntity("mono").SetPosition(New Vector2(1920 / 2, 350)).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("intro/mononez/monogame")))
            CreateEntity("nez").SetPosition(New Vector2(1920 / 2, 750)).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("intro/mononez/nez")))
            CreateEntity("sep").AddComponent(New Seperator)

            'Transition to new scene
            Core.Schedule(1.5, Sub() Core.StartSceneTransition(New FadeTransition(Function() New Games.Blali_1.Vanilla.GameScene(0)) With {.FadeInDuration = 1, .FadeOutDuration = 0.7, .FadeToColor = Color.Black, .FadeEaseType = Tweens.EaseType.QuadInOut}))
        End Sub

        Private Class Seperator
            Inherits RenderableComponent
            Dim ctr As New Vector2(1920 / 2, 1080 / 2)

            Public Overrides Sub Render(batcher As Batcher, camera As Camera)
                batcher.DrawLine(ctr - New Vector2(400, 0), ctr + New Vector2(400, 0), Color.DarkGray, 5)
            End Sub

            Public Overrides ReadOnly Property Bounds As RectangleF
                Get
                    Return New RectangleF(0, 0, 1920, 1080)
                End Get
            End Property
        End Class
    End Class
End Namespace