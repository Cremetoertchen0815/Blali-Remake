Public Class RenderableLabel
    Inherits RenderableComponent

    Dim fnt As NezSpriteFont
    Friend Property Text As String = "lol"

    Public Overrides Sub Initialize()
        MyBase.Initialize()

        fnt = New NezSpriteFont(Core.Content.Load(Of SpriteFont)("intro/bfm/fnt_HKG_17_M"))
    End Sub

    Public Function SetText(text As String) As RenderableLabel
        Me.Text = text
        Return Me
    End Function

    Public Overrides Sub Render(batcher As Batcher, camera As Camera)
        Dim siz As Vector2 = fnt.MeasureString(Text)
        batcher.DrawString(fnt, Text, Entity.Transform.Position + LocalOffset, Color,
        Entity.Transform.Rotation, siz / 2, Entity.Transform.Scale, SpriteEffects.None, _layerDepth)
    End Sub

    Public Overrides ReadOnly Property Bounds As RectangleF
        Get
            Return New RectangleF(0, 0, 1920, 1080)
        End Get
    End Property
End Class