Imports Blali.Framework.Graphics

Namespace Games.Blali_1.Remastered
    Public Class Cloud
        Inherits BackgroundSprite
        Implements IUpdatable

        Private Const SPEEEEED As Single = 25.0F
        Private Shared Clouds As Texture2D() = {Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_A"), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_B"), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_C",), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_D"), Core.Content.LoadTexture("game/Blali_1/bg/bg_cloud_E")}

        Sub New(origin As Vector2)
            MyBase.New(Clouds(Random.Range(0, Clouds.Length)), Nothing, New Vector2(Random.NextFloat))
            Position = New Point(Random.Range(0, 1920), Random.Range(-200, 200))
            Size = New Point(Texture.Width, Texture.Height)
            local_x = Position.X

            Me.Origin = origin
            Blend = BlendState.NonPremultiplied
            LoopVertical = LoopMode.None
            LoopHorizontal = LoopMode.ScreenWrap
            SetLayerDepth(1 - Parallax.X)
        End Sub

        Private ReadOnly Property IUpdatable_Enabled As Boolean = True Implements IUpdatable.Enabled
        Private ReadOnly Property IUpdatable_UpdateOrder As Integer = 0 Implements IUpdatable.UpdateOrder

        Dim local_x As Single
        Public Sub Update() Implements IUpdatable.Update
            local_x += Parallax.X * Time.DeltaTime * SPEEEEED
            Position = New Point(System.Math.Floor(local_x), Position.Y)
        End Sub
    End Class
End Namespace