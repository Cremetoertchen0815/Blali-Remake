Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics

    <TestState(TestState.NearCompletion)>
    Module Primitives2D
        Private ReadOnly circleCache As New Dictionary(Of Integer, List(Of Vector2))

        Public Sub DrawPoints(ByVal position As Vector2, ByVal points As List(Of Vector2), ByVal color As Color, ByVal thickness As Single)
            If points.Count < 2 Then Return

            For i As Integer = 1 To points.Count - 1
                DrawLine(points(i - 1) + position, points(i) + position, color, thickness)
            Next
        End Sub

        Public Sub DrawPoints(ByVal position As Vector2, ByVal points As Vector2(), ByVal color As Color, ByVal thickness As Single)
            If points.Length < 2 Then Return

            For i As Integer = 1 To points.Length - 1
                DrawLine(points(i - 1) + position, points(i) + position, color, thickness)
            Next
        End Sub

        Private Function CreateCircle(ByVal radius As Double) As List(Of Vector2)

            If circleCache.ContainsKey(radius) Then
                Return circleCache(radius)
            End If

            Dim vectors As List(Of Vector2) = New List(Of Vector2)()
            Const max As Double = 2.0 * Math.PI
            Dim [step] As Double = max / (radius * cPrimitiveSidesPerRadius)
            Dim theta As Double = 0.0

            While theta < max
                vectors.Add(New Vector2(CSng((radius * Math.Cos(theta))), CSng((radius * Math.Sin(theta)))))
                theta += [step]
            End While

            vectors.Add(New Vector2(CSng((radius * Math.Cos(0))), CSng((radius * Math.Sin(0)))))
            circleCache.Add(radius, vectors)
            Return vectors
        End Function

        Dim points As New List(Of Vector2)
        Dim final As New List(Of Vector2)
        Private Function CreateArc(ByVal radius As Single, ByVal startingAngle As Single, ByVal radians As Single) As List(Of Vector2)
            points.Clear()
            points.AddRange(CreateCircle(radius))
            points.RemoveAt(points.Count - 1)
            Dim curAngle As Double = 0.0
            Dim anglePerSide As Double = MathHelper.TwoPi / (radius * cPrimitiveSidesPerRadius)

            While (curAngle + (anglePerSide / 2.0)) < startingAngle
                curAngle += anglePerSide
                points.Add(points(0))
                points.RemoveAt(0)
            End While

            points.Add(points(0))
            Dim sidesInArc As Integer = CInt(((radians / anglePerSide) + 0.5))
            points.RemoveRange(sidesInArc + 1, points.Count - sidesInArc - 1)
            Return points
        End Function

        Public Function CreateArc(ByVal center As Vector2, ByVal radius As Single, ByVal sides As Integer, ByVal startingAngle As Single, ByVal radians As Single) As Vector2()
            points.Clear()
            points.AddRange(CreateCircle(radius))
            points.RemoveAt(points.Count - 1)
            Dim curAngle As Double = 0.0
            Dim anglePerSide As Double = MathHelper.TwoPi / (radius * cPrimitiveSidesPerRadius)

            While (curAngle + (anglePerSide / 2.0)) < startingAngle
                curAngle += anglePerSide
                points.Add(points(0))
                points.RemoveAt(0)
            End While

            points.Add(points(0))
            Dim sidesInArc As Integer = CInt(((radians / anglePerSide) + 0.5))
            points.RemoveRange(sidesInArc + 1, points.Count - sidesInArc - 1)

            final.Clear()
            For Each element In points
                final.Add(center + element)
            Next
            Return final.ToArray
        End Function
        Public Function CreateArc(ByVal center As Vector2, ByVal radius As Single, ByVal sides As Integer, ByVal startingAngle As Single, ByVal radians As Single, existinglist As List(Of Vector2)) As List(Of Vector2)
            points.Clear()
            points.AddRange(CreateCircle(radius))
            points.RemoveAt(points.Count - 1)
            Dim curAngle As Double = 0.0
            Dim anglePerSide As Double = MathHelper.TwoPi / (radius * cPrimitiveSidesPerRadius)

            While (curAngle + (anglePerSide / 2.0)) < startingAngle
                curAngle += anglePerSide
                points.Add(points(0))
                points.RemoveAt(0)
            End While

            points.Add(points(0))
            Dim sidesInArc As Integer = CInt(((radians / anglePerSide) + 0.5))
            points.RemoveRange(sidesInArc + 1, points.Count - sidesInArc - 1)

            For i As Integer = 1 To points.Count
                Dim itm As Vector2 = points(points.Count - i)
                existinglist.Add(center + itm)
            Next
            Return existinglist
        End Function

        <Extension()>
        Sub FillRectangle(ByVal rect As Rectangle, ByVal color As Color)

            SpriteBat.Draw(ReferencePixel, rect, color)
        End Sub

        <Extension()>
        Sub FillRectangle(ByVal rect As Rectangle, ByVal color As Color, ByVal angle As Single)

            SpriteBat.Draw(ReferencePixel, rect, Nothing, color, angle, Vector2.Zero, SpriteEffects.None, 0)
        End Sub

        <Extension()>
        Sub FillRectangle(ByVal location As Vector2, ByVal size As Vector2, ByVal color As Color)
            FillRectangle(location, size, color, 0.0F)
        End Sub

        <Extension()>
        Sub FillRectangle(ByVal location As Vector2, ByVal size As Vector2, ByVal color As Color, ByVal angle As Single)

            SpriteBat.Draw(ReferencePixel, location, Nothing, color, angle, Vector2.Zero, size, SpriteEffects.None, 0)
        End Sub

        <Extension()>
        Sub FillRectangle(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal color As Color)
            FillRectangle(New Vector2(x, y), New Vector2(w, h), color, 0.0F)
        End Sub

        <Extension()>
        Sub FillRectangle(ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single, ByVal color As Color, ByVal angle As Single)
            FillRectangle(New Vector2(x, y), New Vector2(w, h), color, angle)
        End Sub

        <Extension()>
        Sub DrawRectangle(ByVal rect As Rectangle, ByVal color As Color)
            DrawRectangle(rect, color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawRectangle(ByVal rect As Rectangle, ByVal color As Color, ByVal thickness As Single)
            DrawLine(New Vector2(rect.X, rect.Y), New Vector2(rect.Right, rect.Y), color, thickness)
            DrawLine(New Vector2(rect.X + 1.0F, rect.Y), New Vector2(rect.X + 1.0F, rect.Bottom + thickness), color, thickness)
            DrawLine(New Vector2(rect.X, rect.Bottom), New Vector2(rect.Right, rect.Bottom), color, thickness)
            DrawLine(New Vector2(rect.Right + 1.0F, rect.Y), New Vector2(rect.Right + 1.0F, rect.Bottom + thickness), color, thickness)
        End Sub

        <Extension()>
        Sub DrawRectangle(ByVal location As Vector2, ByVal size As Vector2, ByVal color As Color)
            DrawRectangle(New Rectangle(CInt(location.X), CInt(location.Y), CInt(size.X), CInt(size.Y)), color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawRectangle(ByVal location As Vector2, ByVal size As Vector2, ByVal color As Color, ByVal thickness As Single)
            DrawRectangle(New Rectangle(CInt(location.X), CInt(location.Y), CInt(size.X), CInt(size.Y)), color, thickness)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal color As Color)
            DrawLine(New Vector2(x1, y1), New Vector2(x2, y2), color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal x1 As Single, ByVal y1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal color As Color, ByVal thickness As Single)
            DrawLine(New Vector2(x1, y1), New Vector2(x2, y2), color, thickness)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal line As Vector2(), ByVal color As Color)
            DrawLine(New Vector2(line(0).X, line(0).Y), New Vector2(line(1).X, line(1).Y), color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal line As Vector2(), ByVal color As Color, ByVal thickness As Single)
            DrawLine(New Vector2(line(0).X, line(0).Y), New Vector2(line(1).X, line(1).Y), color, thickness)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal point1 As Vector2, ByVal point2 As Vector2, ByVal color As Color)
            DrawLine(point1, point2, color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal point1 As Vector2, ByVal point2 As Vector2, ByVal color As Color, ByVal thickness As Single)
            Dim distance As Single = Vector2.Distance(point1, point2)
            Dim angle As Single = CSng(Math.Atan2(point2.Y - point1.Y, point2.X - point1.X))
            DrawLine(point1, distance, angle, color, thickness)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal point As Vector2, ByVal length As Single, ByVal angle As Single, ByVal color As Color)
            DrawLine(point, length, angle, color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawLine(ByVal point As Vector2, ByVal length As Single, ByVal angle As Single, ByVal color As Color, ByVal thickness As Single)
            SpriteBat.Draw(ReferencePixel, point, Nothing, color, angle, Vector2.Zero, New Vector2(length, thickness), SpriteEffects.None, 0)
        End Sub

        <Extension()>
        Sub PutPixel(ByVal x As Single, ByVal y As Single, ByVal color As Color)
            PutPixel(New Vector2(x, y), color)
        End Sub

        <Extension()>
        Sub PutPixel(ByVal position As Vector2, ByVal color As Color)
            SpriteBat.Draw(ReferencePixel, position, color)
        End Sub

        <Extension()>
        Sub DrawCircle(ByVal center As Vector2, ByVal radius As Single, ByVal sides As Integer, ByVal color As Color)
            DrawPoints(center, CreateCircle(radius), color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawCircle(ByVal center As Vector2, ByVal radius As Single, ByVal sides As Integer, ByVal color As Color, ByVal thickness As Single)
            DrawPoints(center, CreateCircle(radius), color, thickness)
        End Sub

        <Extension()>
        Sub DrawCircle(ByVal x As Single, ByVal y As Single, ByVal radius As Single, ByVal sides As Integer, ByVal color As Color)
            DrawPoints(New Vector2(x, y), CreateCircle(radius), color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawCircle(ByVal x As Single, ByVal y As Single, ByVal radius As Single, ByVal sides As Integer, ByVal color As Color, ByVal thickness As Single)
            DrawPoints(New Vector2(x, y), CreateCircle(radius), color, thickness)
        End Sub

        <Extension()>
        Sub DrawArc(ByVal center As Vector2, ByVal radius As Single, ByVal sides As Integer, ByVal startingAngle As Single, ByVal radians As Single, ByVal color As Color)
            DrawArc(center, radius, sides, startingAngle, radians, color, 1.0F)
        End Sub

        <Extension()>
        Sub DrawArc(ByVal center As Vector2, ByVal radius As Single, ByVal sides As Integer, ByVal startingAngle As Single, ByVal radians As Single, ByVal color As Color, ByVal thickness As Single)
            Dim arc As List(Of Vector2) = CreateArc(radius, startingAngle, radians)
            DrawPoints(center, arc, color, thickness)
        End Sub

        <Extension()>
        Function GenerateRoundedRectangle(ByVal rect As Rectangle, ByVal radius As Single)
            Dim ret As New RoundedRectangleHeader

            'Generate base
            ret.base = New Rectangle(rect.X + radius, rect.Y + radius, rect.Width - 2 * radius, rect.Height - 2 * radius)
            'Generate sides
            ret.sideA = New Rectangle(rect.Left, rect.Top + radius, radius, rect.Height - 2 * radius) 'Left
            ret.sideB = New Rectangle(rect.Right - radius, rect.Top + radius, radius, rect.Height - 2 * radius) 'Right
            ret.sideC = New Rectangle(rect.Left + radius, rect.Top, rect.Width - 2 * radius, radius) 'Top
            ret.sideD = New Rectangle(rect.Left + radius, rect.Bottom - radius, rect.Width - 2 * radius, radius) 'Bottom
            'Generate corners
            ret.cornerA = New Rectangle(rect.Left, rect.Top, radius, radius)
            ret.cornerB = New Rectangle(rect.Right - radius, rect.Top, radius, radius)
            ret.cornerC = New Rectangle(rect.Left, rect.Bottom - radius, radius, radius)
            ret.cornerD = New Rectangle(rect.Right - radius, rect.Bottom - radius, radius, radius)

            Return ret
        End Function

        Sub DrawRoundedRectangle(ByVal rect As RoundedRectangleHeader, ByVal color As Color)
            'Draw base
            SpriteBat.Draw(ReferencePixel, rect.base, color)
            'Draw sides
            SpriteBat.Draw(ReferencePixel, rect.sideA, color) 'Left
            SpriteBat.Draw(ReferencePixel, rect.sideB, color) 'Right
            SpriteBat.Draw(ReferencePixel, rect.sideC, color) 'Top
            SpriteBat.Draw(ReferencePixel, rect.sideD, color) 'Bottom
            'Draw corners
            SpriteBat.Draw(RoundedCorner, rect.cornerA, Nothing, color, 0, Vector2.Zero, SpriteEffects.None, 0)
            SpriteBat.Draw(RoundedCorner, rect.cornerB, Nothing, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0)
            SpriteBat.Draw(RoundedCorner, rect.cornerC, Nothing, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0)
            SpriteBat.Draw(RoundedCorner, rect.cornerD, Nothing, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally Or SpriteEffects.FlipVertically, 0)
        End Sub

    End Module

    Friend Structure CircleHeader
        Dim radius As Single
        Dim sides As Integer
    End Structure

    Friend Structure RoundedRectangleHeader
        Dim base As Rectangle
        Dim sideA As Rectangle
        Dim sideB As Rectangle
        Dim sideC As Rectangle
        Dim sideD As Rectangle
        Dim cornerA As Rectangle
        Dim cornerB As Rectangle
        Dim cornerC As Rectangle
        Dim cornerD As Rectangle
    End Structure
End Namespace
