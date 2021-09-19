Imports Emmond.Framework.Camera
Imports Emmond.Framework.Entities
Imports Emmond.Framework.Graphics
Imports Emmond.IG
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level

    <TestState(TestState.NearCompletion)>
    Public Class Trigger
        Public Type As TriggerType 'Represents what kind of trigger is used
        Public ID As Integer 'A unique identifier for each trigger
        Public ExecType As ExecType 'Represents what kind of execution type being is used
        Public Location As Vector2 'Shows at which grid(!) location the trigger is located
        Public Orientation As TriggerOrientation
        Public Length As UInteger 'Shows the width/height of the trigger
        Public VectorArgument As Vector2 '[Type 3] Point at which the player is being respawned at a checkpoint after dying ..... *cough* triggered/[Type 5] The point at which the camera is locked to
        Public Rectangle As Rectangle
        Public GridLine As Vector2()
        Public Activated As Boolean
        Public LastTouch As Boolean

        Private CulledIn As Boolean
        Private CullRectangle As Rectangle

        Public Sub Draw()
            If CulledIn Then
                Select Case Type
                    Case TriggerType.FinishLine
                        Dim loc As Vector2 = Level.GetMapTilePosition(Location, Vector2.Zero)
                        If Orientation = TriggerOrientation.Vertical Then
                            SpriteBat.Draw(DebugTexture, New Rectangle(loc.X - 5, 1080 - loc.Y - Length * cTileSize, 10, Length * cTileSize), Nothing, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.89)
                        Else
                            SpriteBat.Draw(DebugTexture, New Rectangle(loc.X, 1080 - loc.Y + 5, Length * cTileSize, 10), Nothing, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.89)
                        End If
                    Case TriggerType.Checkpoint
                        Dim loc As Vector2 = Level.GetMapTilePosition(Location, Vector2.Zero)
                        If Orientation = TriggerOrientation.Vertical Then
                            SpriteBat.Draw(DebugTexture, New Rectangle(loc.X - 5, 1080 - loc.Y - Length * cTileSize, 10, Length * cTileSize), Nothing, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 0.89)
                        Else
                            SpriteBat.Draw(DebugTexture, New Rectangle(loc.X, 1080 - loc.Y + 5, Length * cTileSize, 10), Nothing, Color.Green, 0, Vector2.Zero, SpriteEffects.None, 0.89)
                        End If
                End Select
            End If
        End Sub

        Public Sub DrawDebug(lvl As Level)
            If CulledIn Then
                Dim camoffset As Vector2 = lvl.Camera.DefaultOriginLocation
                If Activated Then
                    DrawLine({GridLine(0) - camoffset, GridLine(1) - camoffset}, Color.Orange, 3)
                Else
                    DrawLine({GridLine(0) - camoffset, GridLine(1) - camoffset}, Color.Crimson, 3)
                End If
                If Orientation = TriggerOrientation.Vertical Then
                    SpriteBat.DrawString(DebugSmolFont, "Trigger #" & ID, New Vector2(CullRectangle.Center.X - camoffset.X, CullRectangle.Top - camoffset.Y) + New Vector2(10, 0), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Position/Length: " & Location.ToString & "/" & Length.ToString, New Vector2(CullRectangle.Center.X - camoffset.X, CullRectangle.Top - camoffset.Y) + New Vector2(10, 15), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Type: " & Type.ToString, New Vector2(CullRectangle.Center.X - camoffset.X, CullRectangle.Top - camoffset.Y) + New Vector2(10, 30), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Execution: " & ExecType.ToString, New Vector2(CullRectangle.Center.X - camoffset.X, CullRectangle.Top - camoffset.Y) + New Vector2(10, 45), Color.White)
                Else
                    SpriteBat.DrawString(DebugSmolFont, "Trigger #" & ID, New Vector2(CullRectangle.Left - camoffset.X, CullRectangle.Top - 70 - camoffset.Y) + New Vector2(10, 0), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Position/Length: " & Location.ToString & "/" & Length.ToString, New Vector2(CullRectangle.Left - camoffset.X, CullRectangle.Top - 70 - camoffset.Y) + New Vector2(10, 15), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Type: " & Type.ToString, New Vector2(CullRectangle.Left - camoffset.X, CullRectangle.Top - 70 - camoffset.Y) + New Vector2(10, 30), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Execution: " & ExecType.ToString, New Vector2(CullRectangle.Left - camoffset.X, CullRectangle.Top - 70 - camoffset.Y) + New Vector2(10, 45), Color.White)
                End If
            End If
        End Sub

        Public Sub Update(lvl As Level, pl As Player, ByRef fndir As Integer, ByRef triggerinfluence As Boolean())
            CulledIn = lvl.Camera.Viewport.Intersects(CullRectangle)
            If CulledIn Then
                Dim res As Boolean = Rectangle.Intersects(pl.mAABB.GetRectangle)
                Activated = False
                Select Case Type
                    Case TriggerType.UserDefined
                        Select Case ExecType
                            Case ExecType.OnceTotal
                                If res And Not LastTouch Then Activated = True : LastTouch = True
                            Case ExecType.OncePerTouch
                                If res And Not LastTouch Then Activated = True : LastTouch = True
                                If Not res Then LastTouch = False
                            Case ExecType.Continuous
                                If res Then Activated = True
                        End Select
                    Case TriggerType.FinishLine
                        If res Then
                            If lvl.Header.LevelState = FinishedMode.Nottin Then If pl.mAABB.GetRectangle.Center.X < Location.X * cTileSize Then fndir = 1 Else fndir = -1
                            lvl.Header.LevelState = FinishedMode.TouchedFinishLine
                            pl.AnimationMode = MovementMode.AnimationWPhysics
                        End If
                    Case TriggerType.PlayerBarrier
                        If res Then
                            Activated = True
                            If Orientation = TriggerOrientation.Vertical Then
                                If pl.mAABB.GetRectangle.Center.X < Location.X * cTileSize Then fndir = 1 Else fndir = -1
                                If fndir = 1 Then triggerinfluence(3) = True
                                If fndir = -1 Then triggerinfluence(2) = True
                            ElseIf Orientation = TriggerOrientation.Horizontal Then
                                If pl.mAABB.GetRectangle.Center.Y < Location.Y * cTileSize Then fndir = 1 Else fndir = -1
                                If fndir = 1 Then triggerinfluence(0) = True : pl.mPosition.Y = (Location.Y * cTileSize) - (cPlayerHeight / 2)
                                If fndir = -1 Then triggerinfluence(1) = True : pl.mPosition.Y = (Location.Y * cTileSize) + (cPlayerHeight / 2)
                            End If
                        End If
                    Case TriggerType.CameraLock
                        Activated = False
                        If res And Not LastTouch Then Activated = True : LastTouch = True
                        If Not res Then LastTouch = False

                        If Activated Then
                            CameraCalculator.SetFocus = True
                            CameraCalculator.FocusPoint = VectorArgument
                        End If
                    Case TriggerType.CameraRelease
                        Activated = False
                        If res And Not LastTouch Then Activated = True : LastTouch = True
                        If Not res Then LastTouch = False

                        If Activated Then
                            CameraCalculator.SetFocus = False
                        End If
                    Case TriggerType.DeathPlain
                        If res Then pl.Die(lvl.Camera)
                    Case TriggerType.Checkpoint
                        If res And Not LastTouch Then Activated = True : LastTouch = True
                        If Activated Then pl.SaveCheckpoint(lvl, ID)
                    Case TriggerType.ScrollLock
                        If res Then
                            lvl.Camera.BlockFreeMovement = True
                        End If
                    Case TriggerType.ScrollRelease
                        If res Then
                            lvl.Camera.BlockFreeMovement = False
                        End If
                End Select
            End If
        End Sub

        Public Sub Init()
            Dim pos As Vector2 = Level.GetMapTilePosition(Location, New Vector2(0, 0))
            Dim pointA As New Vector2(pos.X, 1080 - pos.Y)
            Dim vecAdd As New Vector2
            If Orientation = TriggerOrientation.Horizontal Then
                vecAdd.X += Length
            Else
                vecAdd.Y += Length
            End If
            Dim pointB As Vector2 = pointA + (vecAdd * New Vector2(1, -1) * cTileSize)

            Rectangle = New Rectangle(pos.ToPoint - New Point(0, 5), (vecAdd * cTileSize).ToPoint + New Point(0, 10))

            Dim len As Point = (vecAdd * cTileSize).ToPoint * New Point(1, -1) + New Point(4, 10)
            CullRectangle = New Rectangle(pointA.ToPoint - New Point(2, 5) + New Point(0, len.Y), len * New Point(1, -1))
            GridLine = {pointA, pointB}
        End Sub
    End Class

End Namespace