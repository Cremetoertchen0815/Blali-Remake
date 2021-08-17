Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics

    <TestState(TestState.Finalized)>
    Public Class TextureAtlas
        Inherits Dictionary(Of Integer, Rectangle)
        Public Property Texture As Texture2D

        Sub New(texture2D As Texture)
            Texture = texture2D
        End Sub
    End Class

End Namespace