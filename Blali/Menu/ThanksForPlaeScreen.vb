Namespace Menu
    Public Class ThanksForPlaeScreen
        Inherits Scene

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer).Material.SamplerState = SamplerState.LinearClamp
            ClearColor = Color.Black

            CreateEntity("txt").AddComponent(Of RenderableLabel)().SetText("Thanks for plae gaem! <3").Entity.SetLocalPosition(New Vector2(1920, 1080) / 2).SetScale(0.5).TweenScaleTo(2, 10).SetEaseType(Tweens.EaseType.QuadOut).Start()

        End Sub

    End Class
End Namespace