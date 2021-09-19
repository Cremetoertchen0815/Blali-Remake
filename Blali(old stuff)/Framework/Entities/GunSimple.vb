Imports System
Imports System.Collections.Generic
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Misc
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Entities
    Public Class GunSimple
        Public cGunBulletSpeed As Integer = 7 '°
        Public Shared DrawingShit As New List(Of BulletSimple)
        Public Shared TotalBulletDrawingCount As Integer
        Friend Shared txt As Texture2D
        Public Bullets As New List(Of BulletSimple)
        Dim Counter As Integer
        Dim BounceCounter As Integer

        Dim rc As Rectangle
        Dim newr As New Rectangle
        Dim vc As Vector2

        Sub New()

        End Sub

        Sub Update(gameTime As GameTime, lvl As Level.Level)
            Counter += gameTime.ElapsedGameTime.TotalMilliseconds

            For i As Integer = 1 To Bullets.Count
                If i > Bullets.Count Then Exit For
                Dim element = Bullets(i - 1)

                'Check whether Bullet should get deleted
                element.OnScreenTime += gameTime.ElapsedGameTime.TotalMilliseconds

                Dim colls As Boolean() = {False, False, False}
                If lvl.IsObstacle(lvl.GetMapTileAtPoint(element.Location + New Vector2(element.Velocity.X * gameTime.ElapsedGameTime.TotalSeconds * 60, 0))) Then
                    colls(0) = True
                ElseIf lvl.IsObstacle(lvl.GetMapTileAtPoint(element.Location + New Vector2(0, element.Velocity.Y * gameTime.ElapsedGameTime.TotalSeconds * 60))) Then
                    colls(1) = True
                ElseIf lvl.IsObstacle(lvl.GetMapTileAtPoint(element.Location + element.Velocity * gameTime.ElapsedGameTime.TotalSeconds * 60)) Then
                    colls(2) = True
                End If

                If element.OnScreenTime > cGunBulletRange - 200 Then element.Color = Color.Lerp(element.Color, New Color(0, 0, 0, 0), 3 * gameTime.ElapsedGameTime.TotalSeconds)
                BounceCounter += gameTime.ElapsedGameTime.TotalMilliseconds

                'Calculate vector
                If Math.Abs(element.Velocity.X) < cGunBulletVelocityDeadzone Then element.Velocity.X = 0
                If Math.Abs(element.Velocity.Y) < cGunBulletVelocityDeadzone Then element.Velocity.Y = 0

                'Move Bullet
                element.Location += element.Velocity * gameTime.ElapsedGameTime.TotalSeconds * 60


                If element.OnScreenTime > cGunBulletRange Or colls(0) Or colls(1) Or colls(2) Then
                    Bullets.RemoveAt(i - 1)
                    i -= 1
                Else
                    DrawingShit.Add(element)
                End If
            Next
        End Sub

        Shared Sub Draw(gameTime As GameTime)
            For i As Integer = 0 To DrawingShit.Count - 1
                Dim element = DrawingShit(i)
                SpriteBat.Draw(txt, New Rectangle(element.Location.X - 6, 1080 - element.Location.Y - 6, 12, 12), Color.White)
            Next
            DrawingShit.Clear()

            TotalBulletDrawingCount = DrawingShit.Count
        End Sub

        Function SpawnNewBullet(location As Vector2, angle As Integer, mSpeed As Vector2, col As Color, dmg As Single, Optional ByRef Energy As Single = 0) As Boolean
            If Counter > cGunCooldown Then
                Dim nw As New BulletSimple
                nw.Location = location
                nw.OnScreenTime = 0
                nw.Color = col
                nw.DefaultSpeed = cGunBulletSpeed
                nw.Velocity = New Vector2(mSpeed.X, 0) + Rotate(New Vector2(0, cGunBulletSpeed), angle * StaticFunctions.DegToRad)
                nw.Damage = dmg
                Bullets.Add(nw)
                Counter = 0
                Energy -= 1
                Return True
            End If
            Return False
        End Function

        Public Const DegToRad = Math.PI / 180
        Public Function Rotate(vec As Vector2, radians As Double) As Vector2
            Dim ca As Double = Math.Cos(radians)
            Dim sa As Double = Math.Sin(radians)
            Return New Vector2(ca * vec.X - sa * vec.Y, sa * vec.X + ca * vec.Y)
        End Function
    End Class

    Public Class BulletSimple
        Public Location As Vector2
        Public Velocity As Vector2
        Public DefaultSpeed As Single
        Public Damage As Single
        Public Color As Color
        Public OnScreenTime As Single
        Public TimeSinceBounce As Integer
    End Class
End Namespace