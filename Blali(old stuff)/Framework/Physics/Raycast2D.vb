
Imports System.Collections.Generic
Imports Emmond.Framework.Graphics
Imports Microsoft.Xna.Framework

Namespace Framework.Physics

    <TestState(TestState.NearCompletion)>
    Public Class Raycast2D
        Public Property Origin As Vector2
        Public Property Direction As Vector2
        Public Property Length As Double

        'Gets Point A of the Raycast line
        Public ReadOnly Property P1 As Vector2
            Get
                Return Origin
            End Get
        End Property

        'Gets Point B of the Raycast line
        Public ReadOnly Property P2 As Vector2
            Get
                Return Origin + (Direction * Length)
            End Get
        End Property
        Public Property SubSegments As List(Of Segment)

#Region "Constructors"
        Sub New()
            SubSegments = New List(Of Segment)
        End Sub

        Sub New(origin As Vector2, direction As Vector2, length As Double)
            Me.Origin = origin
            Me.Direction = direction
            Me.Length = length
            SubSegments = New List(Of Segment)
        End Sub

        Sub New(origin As Vector2, direction As Vector2)
            Me.Origin = origin
            Length = Math.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y))
            Me.Direction = direction / Length
            SubSegments = New List(Of Segment)
        End Sub
#End Region

        Sub SetFromRay(origin As Vector2, direction As Vector2)
            Me.Origin = origin
            Length = Math.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y))
            Me.Direction = direction / Length
        End Sub

        Sub SetFromRaycast(origin As Vector2, direction As Vector2, length As Single)
            Me.Origin = origin
            Me.Direction = direction
            Me.Length = length
        End Sub

        Public Function ToLine() As Line
            Dim ret As New Line
            ret.P1 = Origin
            ret.P2 = Origin + (Direction * Length)
            Return ret
        End Function

        Public Function GetAngle() As Double
            Return Math.Atan2(Direction.Y, Direction.X)
        End Function

        Public Sub Draw(color As Color, thicc As Single)
            DrawLine(Origin, Origin + (Direction * Length), color, thicc)
        End Sub

        Public Sub DrawSegments(color As Color, thicc As Single)
            For Each element In SubSegments
                DrawLine(element.P1, element.P2, color, thicc)
            Next
        End Sub

        Public Shared Function GetAngle(vector As Vector2) As Double
            Return Math.Atan2(vector.Y, vector.X)
        End Function

        Public Shared Function GetPointDifference(P1 As PreciseVector2, P2 As PreciseVector2) As Double
            Return Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2))
        End Function
        Public Shared Function GetPointDifference(P1 As Vector2, P2 As PreciseVector2) As Double
            Return Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2))
        End Function

        Public Function GetPointAtLength(len As Single) As Vector2
            Return Origin + Direction * len
        End Function

        Dim lasto As Vector2
        Dim lastl As Single
        Dim lastd As Vector2
        Dim curray As Raycast2D

        'Makes the Raycast bounce of the A layer
        Public Sub SubdivideByMap(lvl As Level.Level)
            If Origin = lasto AndAlso lastl = Length AndAlso lastd = Direction Then Exit Sub 'If the properties didn't change, don't recalculate and leave
            If curray Is Nothing Then curray = New Raycast2D
            SubSegments.Clear()

            Dim curorigin As Vector2 = Origin
            Dim restlen As Single = Length
            Dim curdir As Vector2 = Direction

            'Loop until the entire length of the raycast is processed
            Do While restlen > 0
                curray.SetFromRaycast(curorigin, curdir, restlen) 'Set data for the test ray
                Dim angTile As Single
                Dim curlen As Single = GetTilLength(curray, lvl, angTile) 'Calculate length until ray hits wall
                curray.Length = curlen
                SubSegments.Add(New Segment With {.Offset = Length - restlen, .P1 = curray.P1, .P2 = curray.P2}) 'Add segment to list
                restlen -= curlen 'Subtract length of last segment from total length

                'If there still is some length left(-> if the end isn't reached yet), calculate change of angle
                If restlen > 0 Then
                    Dim angFinal As Single = 2 * angTile - curray.GetAngle 'Calculate new angle
                    curdir = New Vector2(Math.Cos(angFinal), Math.Sin(angFinal)) 'Calculate new directional vector from angle
                    curorigin = curray.P2 + curdir 'Move origin out of the wall
                End If
            Loop

            'Set last parameters
            lasto = Origin
            lastd = Direction
            lastl = Length
        End Sub

        Public Sub CorrectByMap(lvl As Level.Level)
            'Generate temporary ray
            Dim curray As New Raycast2D(Origin, Direction, Length)
            Dim angTile As Single
            Dim curlen As Single = GetTilLength(curray, lvl, angTile) 'Calculate length until ray hits wall
            curray.Length = curlen

            'If ray is longer than collision length
            If curlen < Length Then
                Dim angFinal As Single = 2 * angTile - curray.GetAngle 'Calculate new angle
                Direction = New Vector2(Math.Cos(angFinal), Math.Sin(angFinal)) 'Calculate new directional vector from angle
                Origin = curray.P2 + Direction 'Move origin out of the wall
            End If
        End Sub

        Public Function IsCollidingWithMap(lvl As Level.Level, ByRef length As Single, Optional offset As Boolean = True) As Boolean
            'Generate temporary ray
            Dim curray As New Raycast2D(If(offset, Origin + Direction * 2, Origin), Direction, Me.Length)
            Dim angTile As Single
            Dim curlen As Single = GetTilLength(curray, lvl, angTile) 'Calculate length until ray hits wall
            curray.Length = curlen
            length = curlen

            'Return if the corrected raycast is shorter than the original one(collided with a wall)
            Return curlen < Me.Length
        End Function

        'Extend size of tile A
        Private Function GetExtA(a As Vector2, b As Vector2) As Vector2
            Dim res As Vector2 = a
            If a.X < b.X Then res.X -= 1 Else res.X += 1
            If a.Y < b.Y Then res.Y -= 1 Else res.Y += 1
            Return res
        End Function

        'Extend size of tile B
        Private Function GetExtB(a As Vector2, b As Vector2) As Vector2
            Dim res As Vector2 = b
            If a.X < b.X Then res.X += 1 Else res.X -= 1
            If a.Y < b.Y Then res.Y += 1 Else res.Y -= 1
            Return res
        End Function

        'Inverse Y-Position
        Private Function InvDir(vec As Vector2) As Vector2
            Return New Vector2(vec.X, 1080 - vec.Y)
        End Function

        Private Function GetTilLength(raycast As Raycast2D, lvl As Level.Level, ByRef lineangle As Single) As Single
            Dim lin As Line = raycast.ToLine 'Convert ray to line
            'Get outer bounds tiles of ray and extend them by one tile to every side
            Dim tileA As Vector2 = lvl.GetMapTileAtPoint(InvDir(raycast.P1))
            Dim tileB As Vector2 = lvl.GetMapTileAtPoint(InvDir(raycast.P2))
            tileA = GetExtA(tileA, tileB) : tileB = GetExtB(tileA, tileB)
            'Calculate the direction of line(for loop step)
            Dim dirX As Integer = Math.Sign(tileB.X - tileA.X) : If dirX = 0 Then dirX = 1
            Dim dirY As Integer = Math.Sign(tileB.Y - tileA.Y) : If dirY = 0 Then dirY = 1
            Dim mindistance As Single = raycast.Length
            Dim none As Boolean = True
            'Loop through every tile tile within the ray boundaries
            For x As Integer = tileA.X To tileB.X Step dirX
                For y As Integer = tileA.Y To tileB.Y Step dirY
                    Dim rect As Rectangle = lvl.GetTileRectangle(New Vector2(x, y)) 'Generate the rectangle of the current tile
                    'If the current tile is solid and intersects the ray(line), continue processing
                    If lvl.IsObstacle(x, y) And lin.IntersectRectangle(rect) Then
                        Dim lins As Line() = RectangleToLines(rect) 'Get the surrounding lines of the tile rectangle
                        For i As Integer = 0 To lins.Length - 1 'Loop through surrounding lines
                            Dim colls As Boolean 'Flag the indicates whether ray cuts the current side line of the tile rectangle
                            Dim pos As PreciseVector2 = lins(i).Intersect(lin, colls)

                            If colls And lins(i).OnSegment(pos) Then 'If point of collision actually lays on the side line of the tile rectangle, continue
                                Dim dist As Double = GetPointDifference(raycast.Origin, pos) 'Get distance between the ray origin and the point of intersection
                                'If distance to origin is smaller than before, use as new point and calculate angle
                                If mindistance >= dist Then
                                    mindistance = dist
                                    none = False
                                    lineangle = lins(i).GetAngle
                                End If
                            End If
                        Next
                    End If
                Next
            Next

            'If raycast collides with geometry, return length(capped to 8 in order to prevent the generation of a huge number of very small rays), else return the length of the raycast
            If Not none Then Return Math.Max(mindistance, 8) Else Return raycast.Length
        End Function

        'Return if rectangle is colliding with any segment and the distance length <= len
        Public Function GetRectangleCollisionWithDistance(rect As Rectangle, ByRef doescollide As Boolean) As Single
            doescollide = False

            For i As Integer = 0 To SubSegments.Count - 1 'Loop through all segments
                Dim lin As New Line(SubSegments(i).P1, SubSegments(i).P2) 'Generate line of segment
                If lin.IntersectRectangle(rect) Then
                    For Each subline In RectangleToLines(rect) 'Generate lines from rectangle
                        'DrawRectangle(rect, Color.Red)
                        'DrawLine(lin.P1, lin.P2, Color.Beige)
                        Dim col As Boolean 'If rectangle and segment is colliding
                        Dim colpoint As PreciseVector2 = lin.Intersect(subline, col) 'The point of collision
                        'Return true if point of collision is on the segment and length is smaller than len
                        If col AndAlso lin.OnSegment(colpoint) Then doescollide = True : Return SubSegments(i).Offset + GetPointDifference(lin.P1, colpoint)
                    Next
                End If
            Next
            Return -1
        End Function
    End Class
End Namespace

