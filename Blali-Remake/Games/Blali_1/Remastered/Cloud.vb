Imports Blali.Framework.Graphics

Namespace Games.Blali_1.Remastered
    Public Class Cloud
        Inherits BackgroundSprite

        Private Shared Clouds As Texture2D() = {Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_A"), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_B"), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_C"), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_D"), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_E")}

        Sub New(origin As Vector2)
            MyBase.New(Clouds(Random.Range(0, Clouds.Length)), Nothing, New Vector2(Random.Range(1, 9) * 0.1))
            Position = New Point(Random.Range(0, 1920), Random.Range(0, 100))
            Size = New Point(Texture.Width, Texture.Height)
            Me.Origin = origin
            LoopVertical = LoopMode.None
            LoopHorizontal = LoopMode.ScreenWrap
        End Sub

    End Class
End Namespace