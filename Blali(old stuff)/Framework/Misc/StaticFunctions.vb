Imports System.Collections.Generic
Imports System.Text
Imports Emmond.Framework.Physics
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Namespace Framework.Misc

    <TestState(TestState.WorkInProgress)>
    Public Module StaticFunctions
        Public Const DegToRad = Math.PI / 180
        Public Function RotateVector(vec As Vector2, radians As Double) As Vector2
            Dim ca As Double = Math.Cos(radians)
            Dim sa As Double = Math.Sin(radians)
            Return New Vector2(ca * vec.X - sa * vec.Y, sa * vec.X + ca * vec.Y)
        End Function

        Public Function WrapText(spriteFont As SpriteFont, text As String, maxLineWidth As Single, ByRef height As Integer) As String
            height = 0
            Dim words As String() = text.Split(" ")
            Dim sb As New StringBuilder
            Dim linewidth As Single = 0F
            Dim spaceWidth = spriteFont.MeasureString(" ").X

            For Each word In words
                Dim size As Vector2 = spriteFont.MeasureString(word)

                If linewidth + size.X < maxLineWidth Then
                    sb.Append(word & " ")
                    linewidth += size.X + spaceWidth
                Else
                    sb.Append(Environment.NewLine & word & " ")
                    linewidth = size.X + spaceWidth
                End If
            Next

            Return sb.ToString()
        End Function


        'Returns an outline rectangle around a section of tiles
        Public Function GetSectionBoundaries(sec As Vector2()) As Rectangle
            Dim xa As Single = 0
            Dim xb As Single = 0
            Dim ya As Single = 0
            Dim yb As Single = 0
            For Each element In sec
                If element.X > xb Then xb = element.X
                If element.Y > yb Then yb = element.Y
            Next
            xa = xb
            ya = yb
            For Each element In sec
                If element.X < xa Then xa = element.X
                If element.Y < ya Then ya = element.Y
            Next
            Return New Rectangle(xa, ya - 1, xb - xa + 1, ya - yb - 1)
        End Function


        Public Function GetSectionBoundaries(sec As Dictionary(Of Vector2, Integer)) As Rectangle
            Dim xa As Single = 0
            Dim xb As Single = 0
            Dim ya As Single = 0
            Dim yb As Single = 0
            For Each element In sec
                If element.Key.X > xb Then xb = element.Key.X
                If element.Key.Y > yb Then yb = element.Key.Y
            Next
            xa = xb
            ya = yb
            For Each element In sec
                If element.Key.X < xa Then xa = element.Key.X
                If element.Key.Y < ya Then ya = element.Key.Y
            Next
            Return New Rectangle(xa, ya - 1, xb - xa + 1, ya - yb - 1)
        End Function

        Public Function IntersectRectangle(vec As Vector2(), ByVal rect As Rectangle) As Boolean
            Dim minX As Double = Math.Min(vec(0).X, vec(1).X)
            Dim maxX As Double = Math.Max(vec(0).X, vec(1).X)

            If maxX > rect.Right Then
                maxX = rect.Right
            End If

            If minX < rect.Left Then
                minX = rect.Left
            End If

            If minX > maxX Then
                Return False
            End If

            Dim minY As Double = Math.Min(vec(0).Y, vec(1).Y)
            Dim maxY As Double = Math.Max(vec(0).Y, vec(1).Y)

            If maxY > rect.Bottom Then
                maxY = rect.Bottom
            End If

            If minY < rect.Top Then
                minY = rect.Top
            End If

            If minY > maxY Then
                Return False
            End If

            Return True
        End Function

        Public Function InversionToString(i As Integer) As String
            If i = -1 Then
                Return "-"
            Else
                Return ""
            End If
        End Function


        Public Function interpolate(ByVal d1 As Double, ByVal d2 As Double, ByVal dPercentage As Double) As Double
            Dim dDifference As Double = d2 - d1
            Dim dDistance As Double = dDifference * dPercentage
            Dim dResult As Double = d1 + dDistance
            Return dResult
        End Function

        Public Function interpolate(ByVal i1 As Integer, ByVal i2 As Integer, ByVal dPercentage As Double) As Integer
            Return CInt(interpolate(CDbl(i1), CDbl(i2), dPercentage))
        End Function

        Public Function interpolate(ByVal f1 As Single, ByVal f2 As Single, ByVal dPercentage As Double) As Single
            Return CSng(interpolate(CDbl(f1), CDbl(f2), dPercentage))
        End Function

        Public Function convertLinearToEaseInEaseOut(ByVal dElapsed As Double) As Double
            Dim dFirstHalfTime As Double = If((dElapsed > 0.5), 0.5, dElapsed)
            Dim dSecondHalfTime As Double = If((dElapsed > 0.5), dElapsed - 0.5, 0.0)
            Dim dResult As Double = 2 * dFirstHalfTime * dFirstHalfTime + 2 * dSecondHalfTime * (1.0 - dSecondHalfTime)
            Return dResult
        End Function

        Public Function convertLinearToAcceleration(ByVal dElapsed As Double) As Double
            Return dElapsed * dElapsed
        End Function

        Public Function convertLinearToDeceleration(ByVal dElapsed As Double) As Double
            Return dElapsed * (2.0 - dElapsed)
        End Function

        '---Vector functions---

        'Returns the CrossProduct of P1 and P2 with the current Point being the Vertex
        Public Function CrossProduct(vertex As Vector2, P1 As Vector2, P2 As Vector2) As Double
            'Ax * By - Bx * Ay
            Return (P1.X - vertex.X) * (P2.Y - vertex.Y) - (P2.X - vertex.X) * (P1.Y - vertex.Y)
        End Function

        'Returns the DotProduct of P1 and P2 with the current Point being the Vertex
        Public Function DotProduct(vertex As Vector2, P1 As Vector2, P2 As Vector2) As Double
            'Ax * Bx + Ay * Cy
            Return (P1.X - vertex.X) * (P2.X - vertex.X) + (P1.Y - vertex.Y) * (P2.Y - vertex.Y)
        End Function

        'Rotates point around Axis
        Public Function RotatePoint(Point As Vector2, Degrees As Double, Axis As Vector2) As Vector2
            'Rotate around Axis
            Return New Vector2(
                (Point.X - Axis.X) * Math.Cos(Degrees / 180.0 * Math.PI) - (Point.Y - Axis.Y) * Math.Sin(Degrees / 180.0 * Math.PI) + Axis.X,
                (Point.X - Axis.X) * Math.Sin(Degrees / 180.0 * Math.PI) + Math.Cos(Degrees / 180.0 * Math.PI) * (Point.Y - Axis.Y) + Axis.Y)
        End Function

        Public Function RectangleToLines(rect As Rectangle) As Line()
            Dim P1 As New Vector2(rect.X, rect.Y)
            Dim P2 As New Vector2(rect.X + rect.Size.X, rect.Y)
            Dim P3 As New Vector2(rect.X + rect.Size.X, rect.Y + rect.Size.Y)
            Dim P4 As New Vector2(rect.X, rect.Y + rect.Size.Y)

            Return {New Line(P1, P2), New Line(P2, P3), New Line(P3, P4), New Line(P4, P1)}
        End Function
    End Module

End Namespace
