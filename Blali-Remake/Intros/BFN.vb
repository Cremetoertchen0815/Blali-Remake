Imports Microsoft.Xna.Framework.Audio

Namespace Intros
    Public Class BFN
        Inherits Scene

        Dim end_trigger As Boolean = False
        Dim timer As Single
        Dim skipper As VirtualButton

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)
            ClearColor = Color.Black
            skipper = New VirtualButton(New VirtualButton.KeyboardKey(Keys.Enter))

            'Add renderables
            Dim bg = CreateEntity("background").SetPosition(New Vector2(1920 / 2, 1080 / 2)).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("intro/bfm/bfm_bg"))).SetColor(Color.Transparent)
            Dim cover = CreateEntity("white_blend").SetPosition(New Vector2(1920 / 2, 1080 / 2)).AddComponent(New PrototypeSpriteRenderer(1920, 1080)).SetColor(Color.Transparent)
            Dim fr = CreateEntity("txt").SetPosition(New Vector2(1920 / 2, 1080 / 2)).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("intro/bfm/bfm_fnt"))).SetColor(Color.Transparent)
            Dim cpy = CreateEntity("copyright").AddComponent(New CopyrightLabel).SetColor(Color.Transparent)
            Dim snd = Content.Load(Of SoundEffect)("intro/bfm/bfm")

            'Animate intro
            bg.Entity.TweenScaleTo(4, 6).SetEaseType(Tweens.EaseType.CubicInOut).Start()
            bg.TweenColorTo(Color.White, 2).Start()
            bg.Entity.TweenRotationDegreesTo(15, 10).Start()
            fr.TweenColorTo(Color.White, 4).Start()
            Core.Schedule(1, Sub() cover.TweenColorTo(Color.White * 0.8, 2).SetEaseType(Tweens.EaseType.CubicInOut).SetLoops(Tweens.LoopType.PingPong, 1).Start())
            Core.Schedule(5, Sub() bg.TweenColorTo(Color.Transparent, 4).SetEaseType(Tweens.EaseType.CubicInOut).Start())
            Core.Schedule(5, Sub() cpy.TweenColorTo(Color.White, 2.5).SetEaseType(Tweens.EaseType.CubicInOut).Start())
            'Core.Schedule(3, Sub() snd.Play())

        End Sub

        Public Overrides Sub Update()

            'Transition to new scene
            timer += Time.DeltaTime
            If timer > 9 And Not end_trigger Then
                end_trigger = True
                Core.StartSceneTransition(New FadeTransition(Function() New LF) With {.FadeInDuration = 1, .FadeOutDuration = 2, .FadeToColor = Color.Black, .FadeEaseType = Tweens.EaseType.QuadInOut})
            ElseIf skipper.IsPressed And Not end_trigger Then
                end_trigger = True
                Core.StartSceneTransition(New FadeTransition(Function() New LF) With {.FadeInDuration = 1, .FadeOutDuration = 0.5, .FadeToColor = Color.Black, .FadeEaseType = Tweens.EaseType.QuadInOut})
            End If

            MyBase.Update()
        End Sub


        Private Class CopyrightLabel
            Inherits RenderableComponent

            Dim fnt As NezSpriteFont
            Friend Const Text As String = "Copyright (c) Viktorius Productions"

            Public Overrides Sub Initialize()
                MyBase.Initialize()

                fnt = New NezSpriteFont(Core.Content.Load(Of SpriteFont)("intro/bfm/fnt_HKG_17_M"))
            End Sub

            Public Overrides Sub Render(batcher As Batcher, camera As Camera)
                batcher.DrawString(fnt, Text, New Vector2((1920 - fnt.MeasureString(Text).X) / 2, 900), Me.Color)
            End Sub

            Public Overrides ReadOnly Property Bounds As RectangleF
                Get
                    Return New RectangleF(0, 0, 1920, 1080)
                End Get
            End Property
        End Class

    End Class
End Namespace