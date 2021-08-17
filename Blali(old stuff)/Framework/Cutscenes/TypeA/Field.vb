
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Cutscenes.TypeA

    <TestState(TestState.WorkInProgress)>
    Public Class Field
        Implements IComparable(Of Field)

        'Lists
        Public Property Vertices As VertexPositionColorTexture()
        Public Property Indices As Integer()

        'Buffers
        Friend Property Disable As Boolean
        Friend Property VertexBuffer As VertexBuffer
        Friend Property IndexBuffer As IndexBuffer

        Public Property Texture As Texture2D
        Public Property TextureNr As Integer
        Public Property Depth As Single
        Public Property Alpha As Single = 1
        Public Property Description As String = "None"

        Sub New(quadcount As Integer, texture As Texture2D)
            Vertices = New VertexPositionColorTexture(quadcount * 4) {}
            Indices = New Integer(Vertices.Length * 3 / 2 - 1) {}
            Me.Texture = texture
        End Sub

        Sub New()

        End Sub

        Friend Sub UpdateBuffers(dev As GraphicsDevice)
            If Indices.Length = 0 Or Vertices.Length = 0 Then
                Disable = True
                Exit Sub
            Else
                Disable = False
            End If
            VertexBuffer = New VertexBuffer(dev, GetType(VertexPositionColorTexture), Math.Max(Vertices.Length, 1), BufferUsage.WriteOnly)
            VertexBuffer.SetData(Vertices)

            IndexBuffer = New IndexBuffer(dev, GetType(Integer), Math.Max(Indices.Length, 1), BufferUsage.WriteOnly)
            IndexBuffer.SetData(Indices)
        End Sub

        Public Function Clone() As Field
            Dim ret As New Field
            ret.Depth = Depth
            ret.Description = Description
            ret.TextureNr = TextureNr
            ret.Texture = Texture
            ret.Vertices = New VertexPositionColorTexture(Vertices.Length - 1) {}
            ret.Indices = New Integer(Indices.Length - 1) {}
            For i As Integer = 0 To Vertices.Length - 1
                Dim cp As VertexPositionColorTexture = Vertices(i)
                ret.Vertices(i) = New VertexPositionColorTexture(New Vector3(cp.Position.X, cp.Position.Y, cp.Position.Z), New Color(cp.Color.R, cp.Color.G, cp.Color.B, cp.Color.A), New Vector2(cp.TextureCoordinate.X, cp.TextureCoordinate.Y))
            Next
            For i As Integer = 0 To Indices.Length - 1
                ret.Indices(i) = Indices(i)
            Next
            Return ret
        End Function

        Public Function CompareTo(other As Field) As Integer Implements IComparable(Of Field).CompareTo
            Return Depth.CompareTo(other.Depth)
        End Function
    End Class

End Namespace