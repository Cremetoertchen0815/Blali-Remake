Imports Blali.Framework.Graphics

Namespace Games.Blali_1.Remastered
    Public Class Cloud
        Inherits BackgroundSprite

        Private Shared Clouds As Texture2D() = {Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_A", True), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_B", True), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_C", True), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_D", True), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_E", True)}

        Sub New(origin As Vector2)
            MyBase.New(Clouds(Random.Range(0, Clouds.Length)), Nothing, New Vector2(Random.NextFloat))
            Position = New Point(Random.Range(0, 1920), Random.Range(-200, 200))
            Size = New Point(Texture.Width, Texture.Height)
            Me.Origin = origin
            LoopVertical = LoopMode.None
            LoopHorizontal = LoopMode.ScreenWrap
            SetLayerDepth(Parallax.X)
        End Sub

    End Class
End Namespace