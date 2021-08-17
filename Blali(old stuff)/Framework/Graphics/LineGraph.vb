Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics

    <TestState(TestState.DebugOnly)>
    Public Class Graph
        Public Enum GraphType
            Line
            Fill
        End Enum

        Public Property Type As GraphType
        Public Property Position As Vector2
        Public Property Size As Point
        Public Property MaxValue As Single
        Private _scale As Vector2 = New Vector2(1.0F, 1.0F)
        Private _effect As BasicEffect
        Private lineListIndices As Short()
        Private triangleStripIndices As Short()

        Public Sub New(ByVal graphicsDevice As GraphicsDevice, ByVal size As Point)
            _effect = New BasicEffect(graphicsDevice) With {
                .View = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up),
                .Projection = Matrix.CreateOrthographicOffCenter(0, CSng(graphicsDevice.Viewport.Width), CSng(graphicsDevice.Viewport.Height), 0, 1.0F, 1000.0F),
                .World = Matrix.Identity,
                .VertexColorEnabled = True
            }
            Me.MaxValue = 1
            Me.Size = size
            If size.Y <= 0 Then size = New Point(size.X, 1)
            If size.X <= 0 Then size = New Point(1, size.Y)
            Me.Type = Graph.GraphType.Line
        End Sub

        Private Sub UpdateWorld()
            _effect.World = Matrix.CreateScale(_scale.X, _scale.Y, 1.0F) * Matrix.CreateRotationX(MathHelper.Pi) * Matrix.CreateTranslation(New Vector3(Me.Position, 0))
        End Sub

        Public Sub Draw(ByVal values As List(Of Tuple(Of Single, Color)))
            If values.Count < 2 Then Return
            Dim xScale As Single = Me.Size.X / CSng(values.Count)
            Dim yScale As Single = Me.Size.Y / MaxValue
            _scale = New Vector2(xScale, yScale)
            UpdateWorld()

            If Me.Type = GraphType.Line Then
                Dim pointList As VertexPositionColor() = New VertexPositionColor(values.Count - 1) {}

                For i As Integer = 0 To values.Count - 1
                    pointList(i) = New VertexPositionColor(New Vector3(i, If(values(i).Item1 < Me.MaxValue, values(i).Item1, Me.MaxValue), 0), values(i).Item2)
                Next

                DrawLineList(pointList)
            ElseIf Me.Type = GraphType.Fill Then
                Dim pointList As VertexPositionColor() = New VertexPositionColor(values.Count * 2 - 1) {}

                For i As Integer = 0 To values.Count - 1
                    pointList(i * 2 + 1) = New VertexPositionColor(New Vector3(i, If(values(i).Item1 < Me.MaxValue, values(i).Item1, Me.MaxValue), 0), values(i).Item2)
                    pointList(i * 2) = New VertexPositionColor(New Vector3(i, 0, 0), values(i).Item2)
                Next

                DrawTriangleStrip(pointList)
            End If
        End Sub

        Public Sub Draw(ByVal values As List(Of Single), ByVal color As Color)
            If values.Count < 2 Then Return
            Dim xScale As Single = Me.Size.X / CSng(values.Count)
            Dim yScale As Single = Me.Size.Y / MaxValue
            _scale = New Vector2(xScale, yScale)
            UpdateWorld()

            If Me.Type = GraphType.Line Then
                Dim pointList As VertexPositionColor() = New VertexPositionColor(values.Count - 1) {}

                For i As Integer = 0 To values.Count - 1
                    pointList(i) = New VertexPositionColor(New Vector3(i, If(values(i) < Me.MaxValue, values(i), Me.MaxValue), 0), color)
                Next

                DrawLineList(pointList)
            ElseIf Me.Type = GraphType.Fill Then
                Dim pointList As VertexPositionColor() = New VertexPositionColor(values.Count * 2 - 1) {}

                For i As Integer = 0 To values.Count - 1
                    pointList(i * 2 + 1) = New VertexPositionColor(New Vector3(i, If(values(i) < Me.MaxValue, values(i), Me.MaxValue), 0), color)
                    pointList(i * 2) = New VertexPositionColor(New Vector3(i, 0, 0), color)
                Next

                DrawTriangleStrip(pointList)
            End If
        End Sub

        Private Sub DrawLineList(ByVal pointList As VertexPositionColor())
            If lineListIndices Is Nothing OrElse lineListIndices.Length <> ((pointList.Length * 2) - 2) Then
                lineListIndices = New Short((pointList.Length * 2) - 2 - 1) {}

                For i As Integer = 0 To pointList.Length - 1 - 1
                    lineListIndices(i * 2) = CShort((i))
                    lineListIndices((i * 2) + 1) = CShort((i + 1))
                Next
            End If

            For Each pass As EffectPass In _effect.CurrentTechnique.Passes
                pass.Apply()
                _effect.GraphicsDevice.DrawUserIndexedPrimitives(Of VertexPositionColor)(PrimitiveType.LineList, pointList, 0, pointList.Length, lineListIndices, 0, pointList.Length - 1)
            Next
        End Sub

        Private Sub DrawTriangleStrip(ByVal pointList As VertexPositionColor())
            If triangleStripIndices Is Nothing OrElse triangleStripIndices.Length <> pointList.Length Then
                triangleStripIndices = New Short(pointList.Length - 1) {}

                For i As Integer = 0 To pointList.Length - 1
                    triangleStripIndices(i) = CShort(i)
                Next
            End If

            For Each pass As EffectPass In _effect.CurrentTechnique.Passes
                pass.Apply()
                _effect.GraphicsDevice.DrawUserIndexedPrimitives(Of VertexPositionColor)(PrimitiveType.TriangleStrip, pointList, 0, pointList.Length, triangleStripIndices, 0, pointList.Length - 2)
            Next
        End Sub
    End Class
End Namespace
