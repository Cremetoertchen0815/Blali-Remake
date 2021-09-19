Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics

    <TestState(TestState.NearCompletion)>
    Public Class ClusterData

        Shared Function GetRendertarget(dev As GraphicsDevice) As RenderTarget2D
            Return New RenderTarget2D(
            dev,
            BufferSize.X,
            BufferSize.Y,
            False,
            dev.PresentationParameters.BackBufferFormat,
            DepthFormat.None)
        End Function

        Public InBounds As Boolean
        Public LayerA As RenderTarget2D
        Public LayerB As RenderTarget2D
        Public LayerC As RenderTarget2D
        Public EnableA As Boolean
        Public EnableB As Boolean
        Public EnableC As Boolean
        Public GeneratedA As Boolean
        Public GeneratedB As Boolean
        Public GeneratedC As Boolean
        Public Screenbounds As Rectangle
        Public Tilebounds As Rectangle
        Public TranslationMatrix As Matrix
        Public ClusterKey As Vector2
    End Class
End Namespace
