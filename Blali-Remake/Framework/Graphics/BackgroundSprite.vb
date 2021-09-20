Namespace Framework.Graphics
    Public Class BackgroundSprite
        Inherits RenderableComponent

        Public Property Position As Vector2
        Public Property Size As Point
        Public Property Texture As Texture2D
        Public Property Parallax As Vector2
        Public Property LoopHorizontal As LoopMode
        Public Property LoopVertical As LoopMode

        Private Shared NeutralBatcher As New Batcher(Core.GraphicsDevice)

        Public Overrides Sub Render(batcher As Batcher, camera As Camera)
            Dim rectangleOriginal As New Rectangle((Position + camera.Position * Parallax).ToPoint, Size)
            Dim minX As Integer = System.Math.Min(System.Math.Floor(-rectangleOriginal.Left / rectangleOriginal.Width), 0)
            Dim minY As Integer = System.Math.Min(System.Math.Floor(-rectangleOriginal.Top / rectangleOriginal.Height), 0)
            Dim maxX As Integer = System.Math.Max(System.Math.Ceiling((Entity.Scene.SceneRenderTargetSize.X - rectangleOriginal.Right) / rectangleOriginal.Width), 0)
            Dim maxY As Integer = System.Math.Max(System.Math.Ceiling((Entity.Scene.SceneRenderTargetSize.Y - rectangleOriginal.Bottom) / rectangleOriginal.Height), 0)

            NeutralBatcher.Begin(ScaleMatrix)
            'Draw Base
            NeutralBatcher.Draw(Texture, rectangleOriginal)

            'Draw horizontal loop
            If LoopHorizontal <> LoopMode.None Then
                For i As Integer = minX To maxX
                    If i = 0 Then Continue For
                    NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + New Point(i * rectangleOriginal.Width, 0), Size), Nothing, Color.White, effects:=If(LoopHorizontal = LoopMode.Reverse And Mathf.IsOdd(i), SpriteEffects.FlipHorizontally, SpriteEffects.None))
                Next
            End If

            'Draw vertical loop
            If LoopVertical <> LoopMode.None Then
                For i As Integer = minX To maxX
                    If i = 0 Then Continue For
                    NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + New Point(0, i * rectangleOriginal.Height), Size), Nothing, Color.White, effects:=If(LoopVertical = LoopMode.Reverse And Mathf.IsOdd(i), SpriteEffects.FlipVertically, SpriteEffects.None))
                Next
            End If

            NeutralBatcher.End()
        End Sub

        Public Enum LoopMode
            None
            Jump
            Reverse
        End Enum

    End Class
End Namespace
