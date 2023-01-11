Imports Mathf = Nez.Mathf

Namespace Framework.Graphics
    Public Class BackgroundSprite
        Inherits RenderableComponent

        Sub New(texture As Texture2D, destination As Rectangle, Parallax As Vector2)
            Me.Texture = texture
            Position = destination.Location
            Size = destination.Size
            Me.Parallax = Parallax
        End Sub


        Public Property Position As Point
        Public Property Size As Point
        Public Property Origin As Vector2
        Public Property Texture As Texture2D
        Public Property Parallax As Vector2
        Public Property LoopHorizontal As LoopMode
        Public Property LoopVertical As LoopMode

        'A batcher not influenced by the camera
        Private rectangleOriginal As New Rectangle(0, 0, 1, 1)
        Protected Blend As BlendState = BlendState.AlphaBlend
        Private Shared NeutralBatcher As New Batcher(Core.GraphicsDevice)

        Public Overrides Sub Render(batcher As Batcher, camera As Camera)
            Dim renderSize As Vector2 = Entity.Scene.SceneRenderTargetSize.ToVector2
            rectangleOriginal = New Rectangle(Position + ((camera.TransformPosition + Origin) * Parallax).ToPoint, Size)
            Dim minX As Integer = System.Math.Min(System.Math.Floor(-rectangleOriginal.Left / rectangleOriginal.Width), 0)
            Dim minY As Integer = System.Math.Min(System.Math.Floor(-rectangleOriginal.Top / rectangleOriginal.Height), 0)
            Dim maxX As Integer = System.Math.Max(System.Math.Ceiling((renderSize.X - rectangleOriginal.Right) / rectangleOriginal.Width), 0)
            Dim maxY As Integer = System.Math.Max(System.Math.Ceiling((renderSize.Y - rectangleOriginal.Bottom) / rectangleOriginal.Height), 0)

            'Calculate screen wrap
            If LoopHorizontal = LoopMode.ScreenWrap Then rectangleOriginal.X = Nez.Mathf.Repeat(rectangleOriginal.X + Size.X, renderSize.X) - Size.X
            If LoopVertical = LoopMode.ScreenWrap Then rectangleOriginal.Y = Nez.Mathf.Repeat(rectangleOriginal.Y + Size.Y, renderSize.Y) - Size.Y

            NeutralBatcher.Begin(Blend)

            'Draw Base
            NeutralBatcher.Draw(Texture, rectangleOriginal)

            'Draw fill loops
            If LoopHorizontal = LoopMode.FillJump Or LoopHorizontal = LoopMode.FillReverse Then
                For x As Integer = minX To maxX
                    Dim fxX As SpriteEffects = If(LoopHorizontal = LoopMode.FillReverse And Nez.Mathf.IsOdd(x), SpriteEffects.FlipHorizontally, SpriteEffects.None)

                    'Draw vertical loop
                    If LoopVertical = LoopMode.FillJump Or LoopVertical = LoopMode.FillReverse Then
                        For y As Integer = minY To maxY
                            If x = 0 And y = 0 Then Continue For
                            Dim fxY As SpriteEffects = If(LoopVertical = LoopMode.FillReverse And Mathf.IsOdd(y), SpriteEffects.FlipVertically, SpriteEffects.None)
                            NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + New Point(x * rectangleOriginal.Width, y * rectangleOriginal.Height), Size), Nothing, Color.White, effects:=fxX Or fxY)
                        Next
                    Else
                        If x = 0 Then Continue For
                        NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + New Point(x * rectangleOriginal.Width, 0), Size), Nothing, Color.White, effects:=fxX)
                    End If
                Next
            ElseIf (LoopVertical = LoopMode.FillJump Or LoopVertical = LoopMode.FillReverse) Then
                For y As Integer = minY To maxY
                    If y = 0 Then Continue For
                    Dim fxY As SpriteEffects = If(LoopVertical = LoopMode.FillReverse And Mathf.IsOdd(y), SpriteEffects.FlipVertically, SpriteEffects.None)
                    NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + New Point(0, y * rectangleOriginal.Height), Size), Nothing, Color.White, effects:=fxY)
                Next
            End If

            'Draw screen repeat
            If LoopHorizontal = LoopMode.ScreenWrap Then NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + New Point(renderSize.X, 0), Size))
            If LoopVertical = LoopMode.ScreenWrap Then NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + New Point(0, renderSize.Y), Size))
            If LoopHorizontal = LoopMode.ScreenWrap And LoopVertical = LoopMode.ScreenWrap Then NeutralBatcher.Draw(Texture, New Rectangle(rectangleOriginal.Location + renderSize.ToPoint, Size))

            NeutralBatcher.End()
        End Sub

        Public Overrides Function IsVisibleFromCamera(camera As Camera) As Boolean
            Return True
            'Dim TransformPosition As Vector2 = camera.TransformPosition()
            'Return LoopVertical <> LoopMode.None Or LoopHorizontal <> LoopMode.None OrElse camera.Bounds.Intersects(New RectangleF(Position.ToVector2 - camera.Position + (camera.TransformPosition + Origin) * Parallax, Size.ToVector2))
        End Function

        Public Overrides ReadOnly Property Bounds As RectangleF
            Get
                Return New RectangleF(Position.ToVector2, Size.ToVector2)
            End Get
        End Property

        Public Enum LoopMode
            None
            FillJump
            FillReverse
            ScreenWrap
        End Enum

    End Class
End Namespace
