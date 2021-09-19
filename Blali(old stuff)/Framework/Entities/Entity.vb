
Imports Emmond.Framework.Level
Imports Emmond.IG.Player
Imports Microsoft.Xna.Framework

Namespace Framework.Entities

    <TestState(TestState.WorkInProgress)>
    Public MustInherit Class Entity
        'Represents the current position and the position last frame
        Public mOldPosition As New Vector2()
        Public mPosition As New Vector2()
        Public mSpawn As Vector2

        'Represents the current velocity and the velocity last frame
        Friend mOldSpeed As New Vector2()
        Friend mSpeed As New Vector2()
        Friend mAdditionalImpulse As New Vector2()

        Public mAABB As New AABB(New Vector2(0, 0), New Vector2(0, 0)) 'The rectangle of the player
        Friend mAABBOffset As Vector2 'The offset of mPosition compared to the rectangle

        'Represents the current collisions and the collisions last frame '{Floor, Ceiling, Left, Right, One-way plattform, Wall-jump Left, Wall-jump Right, Slope Left, Slope Right}
        Public mCollision As Boolean() = {False, False, False, False, False, False, False, False, False}
        Friend mOldCollision As Boolean() = {False, False, False, False, False, False, False, False, False}

        'Modes

        Friend AnimationMode As MovementMode = MovementMode.Regular
        Friend CulledIn As Boolean = False

        'Slopes
        Private SlopePositions As Vector2() = {Vector2.Zero, Vector2.Zero}
        Private RailLockMode As Boolean = False
        Public mAngle As Single = 0.01
        Friend DebugTile As Rectangle() = {New Rectangle, New Rectangle, New Rectangle, New Rectangle}

        'Misc
        Friend Gunn As GunSimple
        Public mAttributes As EntityAttributes
        Friend mObsolete As Boolean = False
        Protected mMap As Level.Level

        Public MustOverride Sub Initialize()
        Public MustOverride Sub LoadContent(Optional playerid As Integer = 0)
        Public MustOverride Sub Draw(gameTime As GameTime)
        Public MustOverride Sub DrawOverlay(gameTime As GameTime, lvl As Level.Level) 'Draw overlays that aren't influenced by lighting
        Public MustOverride Sub Update(gameTime As GameTime, lvl As Level.Level, triggerinfluence As Boolean())
        Public MustOverride Function CullIn(camviewport As Rectangle) As Boolean
        Public MustOverride Function GetRect() As Rectangle

        Public Sub New(UniqueID As Integer)
            mAttributes = New EntityAttributes(UniqueID)
        End Sub

        Public Sub UpdatePhysics(gameTime As GameTime, Optional Debug As Boolean = False)
            CulledIn = Debug OrElse CullIn(mMap.Camera.Viewport)

            If CulledIn Then

                'Set and reset flags
                Dim groundY As Single = 0.0F
                Dim ceilingY As Single = 0.0F
                Dim rightWallX As Single = 0.0F
                Dim leftWallX As Single = 0.0F
                mOldPosition = mPosition
                mOldSpeed = mSpeed
                mOldCollision = mCollision
                RailLockMode = False
                mCollision = {False, False, False, False, False, False, False, False, False}

                'Move the object by its velocity
                Dim factor As Double = Math.Cos(mAngle)
                Dim finalspeed As Vector2 = (mSpeed + mAdditionalImpulse) * factor * factor
                mPosition += finalspeed * gameTime.ElapsedGameTime.TotalSeconds * 60

                If Not Debug Then

                    'Check Slope to the right
                    If mSpeed.Y <= 0.0F AndAlso GetSlopeRight(mPosition, groundY, True) Then
                        mPosition.Y = groundY + mAABB.halfSize.Y - mAABBOffset.Y
                        mSpeed.Y = 0.0F
                        mCollision(0) = True
                    End If
                    'Check Slope to the left
                    If mSpeed.Y <= 0.0F AndAlso GetSlopeLeft(mPosition, groundY, True) Then
                        mPosition.Y = groundY + mAABB.halfSize.Y - mAABBOffset.Y
                        mSpeed.Y = 0.0F
                        mCollision(0) = True
                    End If

                    'Check floor
                    If mSpeed.Y <= 0.0F AndAlso HasGround(mOldPosition, mPosition, mSpeed, groundY, mCollision(4)) Then
                        mPosition.Y = groundY + mAABB.halfSize.Y - mAABBOffset.Y
                        mSpeed.Y = 0.0F
                        mCollision(0) = True
                    End If
                    'Check ceiling
                    If mSpeed.Y >= 0.0F AndAlso HasCeiling(mOldPosition, mPosition, ceilingY) Then
                        mPosition.Y = ceilingY - mAABB.halfSize.Y - mAABBOffset.Y - 1.0F
                        mSpeed.Y = 0.0F
                        mCollision(1) = True
                    End If

                    'Check left wall
                    If ((Not mOldCollision(3) And mSpeed.X <= 0.0F) Xor (mOldCollision(3) And mSpeed.X < 0.0F)) AndAlso (Not mCollision(8) And CollidesWithLeftWall(mOldPosition, mPosition, leftWallX)) Then
                        If mOldPosition.X - mAABB.halfSize.X + mAABBOffset.X >= leftWallX Then
                            mPosition.X = leftWallX + mAABB.halfSize.X - mAABBOffset.X
                            mCollision(2) = True
                        End If
                        mSpeed.X = Math.Max(mSpeed.X, 0.0F)
                    Else
                        mCollision(2) = False
                    End If
                    'Check right wall
                    If ((Not mCollision(2) And mSpeed.X >= 0.0F) Xor (mCollision(2) And mSpeed.X > 0.0F)) AndAlso (Not mCollision(7) And CollidesWithRightWall(mOldPosition, mPosition, rightWallX)) Then
                        If mOldPosition.X + mAABB.halfSize.X <= rightWallX Then
                            mPosition.X = rightWallX - mAABB.halfSize.X - mAABBOffset.X
                            mCollision(3) = True
                        End If
                        mSpeed.X = Math.Min(mSpeed.X, 0.0F)
                    Else
                        mCollision(3) = False
                    End If

                    'Ceck downward slopes
                    If RailLockMode And Not mCollision(0) Then
                        If mSpeed.X > 0 Then
                            GetSlopeLeft(mPosition, groundY, False)
                            mPosition.Y = groundY + mAABB.halfSize.Y - mAABBOffset.Y
                            mSpeed.Y = 0.0F
                            mCollision(0) = True
                        ElseIf mSpeed.X < 0 Then
                            GetSlopeRight(mPosition, groundY, False)
                            mPosition.Y = groundY + mAABB.halfSize.Y - mAABBOffset.Y
                            mSpeed.Y = 0.0F
                            mCollision(0) = True
                        End If
                    End If

                    'Check contact points for wall jumping
                    Dim aa As Vector2 = mMap.GetMapTileAtPoint(mPosition + (mAABB.halfSize * New Vector2(1, 0)))
                    Dim ab As Vector2 = mMap.GetMapTileAtPoint(mPosition + (mAABB.halfSize * New Vector2(1, -1)))
                    Dim ac As Vector2 = mMap.GetMapTileAtPoint(mPosition + (mAABB.halfSize * New Vector2(-1, 0)) - New Vector2(cTileSize / 1.5, 0))
                    Dim ad As Vector2 = mMap.GetMapTileAtPoint(mPosition + (mAABB.halfSize * New Vector2(-1, -1)) - New Vector2(cTileSize / 1.5, 0))

                    mCollision(5) = mMap.CanWallJump(aa) And mMap.CanWallJump(ab)
                    mCollision(6) = mMap.CanWallJump(ac) And mMap.CanWallJump(ad)

                    Dim deltaY = SlopePositions(0).Y - SlopePositions(1).Y
                    Dim deltaX = SlopePositions(0).X - SlopePositions(1).X
                    mAngle = Math.Atan(deltaY / deltaX)
                End If



            End If

            mAABB.center = mPosition + mAABBOffset
        End Sub


        Private Function HasGround(ByVal oldPosition As Vector2, ByVal position As Vector2, ByVal speed As Vector2, ByRef groundY As Integer, ByRef onOneWayPlatform As Boolean) As Boolean
            Dim oldCenter As Vector2 = oldPosition + mAABBOffset
            Dim center As Vector2 = position + mAABBOffset

            Dim oldBottomLeft As Vector2 = oldCenter - mAABB.halfSize - New Vector2(0, 1) + New Vector2(1, 0)
            Dim newBottomLeft As Vector2 = center - mAABB.halfSize - New Vector2(0, 1) + New Vector2(1, 0)
            Dim newBottomRight As Vector2 = New Vector2(newBottomLeft.X + mAABB.halfSize.X * 2.0F - 2.0F, newBottomLeft.Y)

            'Set foot positions
            If Not mCollision(7) Then SlopePositions(0) = newBottomRight
            If Not mCollision(8) Then SlopePositions(1) = newBottomLeft


            Dim endY As Integer = mMap.GetMapTileYAtPoint(newBottomLeft.Y)
            Dim begY As Integer = Math.Max(mMap.GetMapTileYAtPoint(oldBottomLeft.Y) - 1, endY)
            Dim dist As Integer = Math.Max(Math.Abs(endY - begY), 1)

            Dim tileIndexX As Integer

            For tileIndexY As Integer = begY To endY Step -1
                Dim bottomLeft As Vector2 = Vector2.Lerp(newBottomLeft, oldBottomLeft, CSng(Math.Abs(endY - tileIndexY)) / dist)
                Dim bottomRight As Vector2 = New Vector2(bottomLeft.X + mAABB.halfSize.X * 2.0F - 2.0F, bottomLeft.Y)

                Dim checkedTile As Vector2 = bottomLeft
                Do
                    checkedTile.X = Math.Min(checkedTile.X, bottomRight.X)

                    tileIndexX = mMap.GetMapTileXAtPoint(checkedTile.X)

                    groundY = tileIndexY * cTileSize + cTileSize / 2.0F

                    If mMap.IsGround(tileIndexX, tileIndexY) Then
                        onOneWayPlatform = False
                        Return True
                    ElseIf mMap.IsOneWayPlatform(tileIndexX, tileIndexY) AndAlso Math.Abs(checkedTile.Y - groundY) <= cOneWayPlatformThreshold + mOldPosition.Y - position.Y Then
                        onOneWayPlatform = True
                    End If

                    If checkedTile.X >= bottomRight.X Then
                        If onOneWayPlatform Then
                            Return True
                        End If
                        Exit Do
                    End If

                    checkedTile.X += cTileSize
                Loop
            Next tileIndexY

            Return False
        End Function

        Private Function HasCeiling(ByVal oldPosition As Vector2, ByVal position As Vector2, ByRef ceilingY As Integer) As Boolean
            Dim center As Vector2 = position + mAABBOffset
            Dim oldCenter As Vector2 = oldPosition + mAABBOffset

            ceilingY = 0.0F

            Dim oldTopRight As Vector2 = oldCenter + mAABB.halfSize + New Vector2(0, 1) - New Vector2(1, 0)

            Dim newTopRight As Vector2 = center + mAABB.halfSize + New Vector2(0, 1) - New Vector2(1, 0)
            Dim newTopLeft As Vector2 = New Vector2(newTopRight.X - mAABB.halfSize.X * 2.0F + 2.0F, newTopRight.Y)

            Dim endY As Integer = mMap.GetMapTileYAtPoint(newTopRight.Y)
            Dim begY As Integer = Math.Min(mMap.GetMapTileYAtPoint(oldTopRight.Y) + 1, endY)
            Dim dist As Integer = Math.Max(Math.Abs(endY - begY), 1)

            Dim tileIndexX As Integer

            For tileIndexY As Integer = begY To endY
                Dim topRight As Vector2 = Vector2.Lerp(newTopRight, oldTopRight, CSng(Math.Abs(endY - tileIndexY)) / dist)
                Dim topLeft As Vector2 = New Vector2(topRight.X - mAABB.halfSize.X * 2.0F + 2.0F, topRight.Y)

                Dim checkedTile As Vector2 = topLeft
                Do
                    checkedTile.X = Math.Min(checkedTile.X, topRight.X)

                    tileIndexX = mMap.GetMapTileXAtPoint(checkedTile.X)

                    If mMap.IsObstacle(tileIndexX, tileIndexY) Then
                        ceilingY = tileIndexY * cTileSize - cTileSize / 2.0F
                        Return True
                    End If

                    If checkedTile.X >= topRight.X Then
                        Exit Do
                    End If
                    checkedTile.X += cTileSize
                Loop
            Next tileIndexY

            Return False
        End Function

        Private Function CollidesWithLeftWall(ByVal oldPosition As Vector2, ByVal position As Vector2, ByRef wallX As Integer) As Boolean
            Dim center As Vector2 = position + mAABBOffset
            Dim oldCenter As Vector2 = oldPosition + mAABBOffset

            wallX = 0.0F

            Dim oldBottomLeft As Vector2 = oldCenter - mAABB.halfSize - New Vector2(1, 0)
            Dim newBottomLeft As Vector2 = center - mAABB.halfSize - New Vector2(1, 0)
            Dim newTopLeft As Vector2 = newBottomLeft + New Vector2(0.0F, mAABB.halfSize.Y * 2.0F)

            Dim tileIndexY As Integer

            Dim endX As Integer = mMap.GetMapTileXAtPoint(newBottomLeft.X)
            Dim begX As Integer = Math.Max(mMap.GetMapTileXAtPoint(oldBottomLeft.X) - 1, endX)
            Dim dist As Integer = Math.Max(Math.Abs(endX - begX), 1)

            For tileIndexX As Integer = begX To endX Step -1
                Dim bottomLeft As Vector2 = Vector2.Lerp(newBottomLeft, oldBottomLeft, CSng(Math.Abs(endX - tileIndexX)) / dist)
                Dim topLeft As Vector2 = bottomLeft + New Vector2(0.0F, mAABB.halfSize.Y * 2.0F)

                Dim checkedTile As Vector2 = bottomLeft
                Do
                    checkedTile.Y = Math.Min(checkedTile.Y, topLeft.Y)

                    tileIndexY = mMap.GetMapTileYAtPoint(checkedTile.Y)

                    If mMap.IsObstacle(tileIndexX, tileIndexY) Then
                        wallX = tileIndexX * cTileSize + cTileSize
                        Return True
                    End If

                    If checkedTile.Y >= topLeft.Y Then
                        Exit Do
                    End If
                    checkedTile.Y += cTileSize
                Loop
            Next tileIndexX

            Return False
        End Function

        Public Shared Function ConvertToString() As String
            Return "Day and night I think about you, sweetie. I love you to eternity and always will. And I wish that I could be with you during the holidays. Sadly I can't. But this won't change the fact that I dearly love you and that we will marry soon." & Environment.NewLine & "Very soon ♥  Greetings, your future husband. Thursday, the 28th November 2019."
        End Function

        Private Function CollidesWithRightWall(ByVal oldPosition As Vector2, ByVal position As Vector2, ByRef wallX As Integer) As Boolean
            Dim center As Vector2 = position + mAABBOffset
            Dim oldCenter As Vector2 = oldPosition + mAABBOffset

            wallX = 0.0F

            Dim oldBottomRight As Vector2 = oldCenter + New Vector2(mAABB.halfSize.X, -mAABB.halfSize.Y) + New Vector2(1, 0)
            Dim newBottomRight As Vector2 = center + New Vector2(mAABB.halfSize.X, -mAABB.halfSize.Y) + New Vector2(1, 0)
            Dim newTopRight As Vector2 = newBottomRight + New Vector2(0.0F, mAABB.halfSize.Y * 2.0F)

            Dim endX As Integer = mMap.GetMapTileXAtPoint(newBottomRight.X)
            Dim begX As Integer = Math.Min(mMap.GetMapTileXAtPoint(oldBottomRight.X) + 1, endX)
            Dim dist As Integer = Math.Max(Math.Abs(endX - begX), 1)

            Dim tileIndexY As Integer

            For tileIndexX As Integer = begX To endX
                Dim bottomRight As Vector2 = Vector2.Lerp(newBottomRight, oldBottomRight, CSng(Math.Abs(endX - tileIndexX)) / dist)
                Dim topRight As Vector2 = bottomRight + New Vector2(0.0F, mAABB.halfSize.Y * 2.0F)

                Dim checkedTile As Vector2 = bottomRight + New Vector2(0, 5)
                Do
                    checkedTile.Y = Math.Min(checkedTile.Y, topRight.Y)

                    tileIndexY = mMap.GetMapTileYAtPoint(checkedTile.Y)

                    If mMap.IsObstacle(tileIndexX, tileIndexY) Then
                        wallX = tileIndexX * cTileSize
                        Return True
                    End If

                    If checkedTile.Y >= topRight.Y Then
                        Exit Do
                    End If
                    checkedTile.Y += cTileSize
                Loop
            Next tileIndexX

            Return False
        End Function

        Private Function GetSlopeRight(ByVal position As Vector2, ByRef groundY As Integer, Optional alt As Boolean = True) As Boolean

            Dim center As Vector2 = position + mAABBOffset
            Dim newBottomLeft As Vector2 = center - mAABB.halfSize - New Vector2(0, 1) + New Vector2(1, 0)
            Dim newBottomRight As Vector2 = New Vector2(newBottomLeft.X + mAABB.halfSize.X * 2.0F - 2.0F, newBottomLeft.Y)
            Dim baseTile As Vector2 = mMap.GetMapTileAtPoint(newBottomRight, True)

            Dim heightmapindex As Integer = 0
            Dim height As Integer = 0
            Math.DivRem(CInt(newBottomRight.X), cTileSize, heightmapindex) 'Identify height map array index

            Dim checktile As Tile
            Dim tilerect As Rectangle

            If alt Then
                'Check if player runs down a slope
                For i As Integer = 0 To cSlopeTileYOffsetDownwards
                    'Calculate tiles with their corresponding height map and bounds rectangle
                    checktile = mMap.GetTile(baseTile - New Vector2(0, i))
                    If checktile IsNot Nothing AndAlso checktile.IsSlope Then
                        If mSpeed.X < 0 Then RailLockMode = True
                    End If
                Next

                'Check if player runs up a slope and check height
                'Iterate through 3 tiles to check whether they are slope tiles
                For i As Integer = 0 To cSlopeTileYOffsetUp + cSlopeTileYOffsetDown
                    'Calculate tiles with their corresponding height map and bounds rectangle
                    Dim currentTile As Vector2 = baseTile + New Vector2(0, cSlopeTileYOffsetUp - i)
                    checktile = mMap.GetTile(currentTile.X, currentTile.Y)
                    tilerect = mMap.GetTileRectangle(currentTile)
                    If checktile IsNot Nothing AndAlso checktile.IsSlope Then
                        height = checktile.HeightMap(heightmapindex)
                        groundY = 1080 - tilerect.Bottom + height + 10
                        SlopePositions(0) = New Vector2(newBottomRight.X, groundY)
                        mCollision(7) = True
                        Return newBottomRight.Y <= groundY
                    End If
                    If i > 0 And mSpeed.X < 0 Then Exit For 'If running in the opposite direction, don't check for any other iterationns than the first
                Next

                Return False
            Else
                'Calculate the right height for running down a slope
                For i As Integer = 0 To cSlopeTileYOffsetUp + cSlopeTileYOffsetDown
                    'Calculate tiles with their corresponding height map and bounds rectangle
                    Dim currentTile As Vector2 = baseTile + New Vector2(0, cSlopeTileYOffsetUp - i)
                    checktile = mMap.GetTile(currentTile.X, currentTile.Y)
                    tilerect = mMap.GetTileRectangle(currentTile)
                    If checktile IsNot Nothing AndAlso checktile.IsSlope Then
                        groundY = 1080 - tilerect.Bottom + checktile.HeightMap(heightmapindex) + 10
                        Return newBottomRight.Y <= groundY
                    End If
                Next

                Return False
            End If
        End Function


        Private Function GetSlopeLeft(ByVal position As Vector2, ByRef groundY As Integer, Optional alt As Boolean = True) As Boolean

            Dim center As Vector2 = position + mAABBOffset
            Dim newBottomLeft As Vector2 = center - mAABB.halfSize - New Vector2(0, 1) + New Vector2(1, 0)
            Dim baseTile As Vector2 = mMap.GetMapTileAtPoint(newBottomLeft, True)

            Dim heightmapindex As Integer = 0
            Dim height As Integer = 0
            Math.DivRem(CInt(Math.Max(newBottomLeft.X - 1, 0)), cTileSize, heightmapindex) 'Identify height map array index

            Dim checktile As Tile
            Dim tilerect As Rectangle

            If alt Then
                'Check if player runs down a slope
                For i As Integer = 0 To cSlopeTileYOffsetDownwards
                    'Calculate tiles with their corresponding height map and bounds rectangle
                    checktile = mMap.GetTile(baseTile - New Vector2(0, i))
                    If checktile IsNot Nothing AndAlso checktile.IsSlope Then
                        If mSpeed.X > 0 Then RailLockMode = True
                    End If
                Next

                'Check if player runs up a slope and check height
                'Iterate through 3 tiles to check whether they are slope tiles
                For i As Integer = 0 To cSlopeTileYOffsetUp + cSlopeTileYOffsetDown
                    'Calculate tiles with their corresponding height map and bounds rectangle
                    Dim currentTile As Vector2 = baseTile + New Vector2(0, cSlopeTileYOffsetUp - i)
                    checktile = mMap.GetTile(currentTile.X, currentTile.Y)
                    tilerect = mMap.GetTileRectangle(currentTile)
                    If checktile IsNot Nothing AndAlso checktile.IsSlope Then
                        height = checktile.HeightMap(heightmapindex)
                        groundY = 1080 - tilerect.Bottom + height + 10
                        SlopePositions(1) = New Vector2(newBottomLeft.X, groundY)
                        mCollision(8) = True
                        Return newBottomLeft.Y <= groundY
                    End If
                    If i > 0 And mSpeed.X > 0 Then Exit For 'If running in the opposite direction, don't check for any other iterationns than the first
                Next

                Return False
            Else
                'Calculate the right height for running down a slope
                For i As Integer = 0 To cSlopeTileYOffsetDown
                    'Calculate tiles with their corresponding height map and bounds rectangle
                    Dim currentTile As Vector2 = baseTile - New Vector2(0, i)
                    checktile = mMap.GetTile(currentTile.X, currentTile.Y)
                    tilerect = mMap.GetTileRectangle(currentTile)
                    If checktile IsNot Nothing AndAlso checktile.IsSlope Then
                        groundY = 1080 - tilerect.Bottom + checktile.HeightMap(heightmapindex) + 10
                        Return newBottomLeft.Y <= groundY
                    End If
                Next

                Return False
            End If
        End Function
    End Class

    Public Enum MovementMode
        Regular = 0
        AnimationWPhysics = 1
        AnimationWoPhysics = 2
    End Enum
End Namespace