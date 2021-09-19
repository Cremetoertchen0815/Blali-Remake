Imports System.Collections.Generic
Imports Emmond.Framework.Level.Background.BGs

Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.Background.Components

    <TestState(TestState.WorkInProgress)>
    Public Class Clouds
        Inherits BackgroundComponent
        Public Property SpawnTime As Integer = 3200
        Public Property MoveSpeed As Single = 0.1
        Public Property DepthLimit As Single = 0.25
        Public Property UpperBounds As Integer = 850
        Public Property LowerBounds As Integer = 400
        Public Property Transparency As Byte = 120

        Public Property Skip As Boolean = True

        Private Textures As Texture2D()
        Private BGpool As ObjectPool(Of BackgroundStatic)
        Private ReturnList As List(Of BackgroundStatic)

        Private counter As Integer

        Friend Overrides Sub Init(lvl As Level)
            Textures = {ContentMan.Load(Of Texture2D)("fx\textures\components\bg_cloud_A"), ContentMan.Load(Of Texture2D)("fx\textures\components\bg_cloud_B"),
                        ContentMan.Load(Of Texture2D)("fx\textures\components\bg_cloud_C"), ContentMan.Load(Of Texture2D)("fx\textures\components\bg_cloud_D"),
                        ContentMan.Load(Of Texture2D)("fx\textures\components\bg_cloud_E")}

            BGpool = New ObjectPool(Of BackgroundStatic)(100)
            ReturnList = New List(Of BackgroundStatic)

            Flush(lvl)
        End Sub

        Friend Overrides Sub Update(gameTime As GameTime, lvl As Level)
            counter += gameTime.ElapsedGameTime.TotalMilliseconds

            Dim mov As Single = MoveSpeed
            Dim limit As Integer = 0
            If Skip Then mov = MoveSpeed * SpawnTime / gameTime.ElapsedGameTime.TotalMilliseconds

            Transparency = 255

            If counter >= SpawnTime Or Skip Then
                Dim newbg As BackgroundStatic = BGpool.Get
                newbg.Layer = Rand.Next(5, DepthLimit * 100) / 100
                Do While ContainsLayer(newbg.Layer)
                    If limit > 5 Then Exit Do
                    newbg.Layer = Rand.Next(500, DepthLimit * 10000) / 10000
                    limit += 1
                Loop
                newbg.VectorScale = newbg.Layer
                newbg.Texture = Textures(Rand.Next(0, 5))
                newbg.Size = DebugTexture.Bounds.Size.ToVector2 * ((newbg.Layer + 0.7) * 2)
                newbg.Color = New Color(255, 255, 255, 120)
                newbg.Location = New Vector2(-GameSize.X / 2 - newbg.Size.X, -Rand.Next(LowerBounds, UpperBounds))

                If limit <= 4 Then Elements.Add(newbg)

                counter = 0
            End If

            ReturnList.Clear()
            Dim skipper As Boolean = True

            For Each element In Elements
                element.Location += New Vector2(mov * gameTime.ElapsedGameTime.TotalMilliseconds * element.VectorScale, 0)
                element.Update(gameTime, lvl.Camera.Location)

                If skipper AndAlso element.GetDrawRectangle.Left > lvl.Map.Size.X * cTileSize Then
                    ReturnList.Add(element)
                Else
                    skipper = False
                End If
            Next

            For Each retel In ReturnList
                Elements.Remove(retel)
                BGpool.Release(retel)
            Next
        End Sub

        Public Sub Flush(lvl As Level)
            'Activate fast filling
            Skip = True

            Do Until IsScreenFilled(lvl.Map.Size.X)
                Update(New GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(60), False), lvl)
            Loop

            Skip = False
        End Sub

        Private Function IsScreenFilled(size As Integer) As Boolean
            If Elements.Count > 0 Then
                If Elements(0).GetDrawRectangle.Right > size * cTileSize Then Return True
            End If
            Return False
        End Function

        Private Function ContainsLayer(layer As Single) As Boolean
            For Each element In Elements
                If element.Layer = layer Then Return True
            Next
            Return False
        End Function
    End Class

End Namespace