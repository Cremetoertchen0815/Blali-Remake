
Imports Emmond.Framework.Entities
Imports Emmond.IG
Imports Microsoft.Xna.Framework
Namespace Framework.Camera

    <TestState(TestState.WorkInProgress)>
    Public Class CameraCalculator
        Public Shared BorderAreas As Rectangle()
        Public Shared BorderAreasA As Rectangle()
        Public Shared BorderAreasB As Rectangle()
        Public Shared FocusPoint As Vector2
        Public Shared SetFocus As Boolean
        Shared LastFocus As Boolean

        Public Shared Sub CalculateTypeA(cam As Camera, player As Player)
            cam.Location.X = Math.Round(player.mAABB.center.X)
            cam.Location.Y = Math.Round(GameSize.Y - player.mAABB.center.Y)
        End Sub

        Public Shared Sub CalculateTypeB(cam As Camera, player As Player, verticalacc As Boolean)
            If BorderAreas Is Nothing Or Not SetFocus And LastFocus Then
                BorderAreas = BorderAreasA
            ElseIf SetFocus And Not LastFocus Then
                BorderAreas = BorderAreasB
            End If

            'Set the focus point
            Dim FocusAABB As New AABB(player.mAABB.center, player.mAABB.halfSize)
            Dim playerrect As Rectangle = FocusAABB.GetRectangle
            FocusAABB.center.X += cCameraVectorAdjHorizontal * player.mSpeed.X
            If player.PlayerState <> PlayerStatus.Jumping Or player.PlayerState <> PlayerStatus.ExJumping Then FocusAABB.center.Y += cCameraVectorAdjVertical * player.mSpeed.Y

            If SetFocus Then FocusAABB.center = FocusPoint Else FocusPoint = FocusAABB.center

            'The players rectangle relative to the camera
            Dim relativeRectangle As Rectangle = FocusAABB.GetRectangle
            relativeRectangle = New Rectangle(cam.TranslateToCamera(New Vector2(relativeRectangle.X, 1080 - relativeRectangle.Y)).ToPoint, relativeRectangle.Size)

            cam.SpeedMode(0) = Math.Abs((cam.Location - FocusAABB.center).X) > GameSize.X / 3

            Dim acc As Boolean() = {False, False}
            If BorderAreas(2).Left < relativeRectangle.Right Then cam.Acceleration.X = cCameraAccelerationHorizontal : acc(0) = True
            If BorderAreas(3).Right > relativeRectangle.Left Then cam.Acceleration.X = -cCameraAccelerationHorizontal : acc(0) = True

            If relativeRectangle.Top + cCameraFastScrollBuffer >= 1080 And player.mSpeed.Y < 0 And Not SetFocus Then
                cam.RapidModeHeight = 1080 - playerrect.Top - (GameSize.Y / 2) + cCameraFastScrollBuffer
                cam.BaseVelocity.Y = 0
                cam.RapidMode = True
            Else

                'Checks intersection with the scrolling areas and sets the acceleration accordingly
                cam.SpeedMode(1) = Math.Abs((cam.Location - FocusAABB.center).Y) > GameSize.Y / 3

                If BorderAreas(0).Top < relativeRectangle.Bottom And verticalacc Then cam.Acceleration.Y = cCameraAccelerationVectical : acc(1) = True
                If BorderAreas(1).Bottom > relativeRectangle.Top And verticalacc Then cam.Acceleration.Y = -cCameraAccelerationVectical : acc(1) = True
                cam.RapidMode = False
            End If

            If SetFocus Then cam.BlockFreeMovement = True
            LastFocus = SetFocus
        End Sub

        Public Shared Sub Init()
            BorderAreasA = {New Rectangle(0, GameSize.Y - cCameraScrollBarrierVectical, GameSize.X, cCameraScrollBarrierVectical),       'Bottom
                           New Rectangle(0, 0, GameSize.X, cCameraScrollBarrierVectical),                                                'Top
                           New Rectangle(GameSize.X - cCameraScrollBarrierHorizontal, 0, cCameraScrollBarrierHorizontal, GameSize.Y),    'Right
                           New Rectangle(0, 0, cCameraScrollBarrierHorizontal, GameSize.X)}                                              'Left
            BorderAreasB = {New Rectangle(0, GameSize.Y - ((GameSize.Y / 2) - cCameraLockSizeY), GameSize.X, ((GameSize.Y / 2) - cCameraLockSizeY)),  'Bottom
                           New Rectangle(0, 0, GameSize.X, ((GameSize.Y / 2) - cCameraLockSizeY)),                                                   'Top
                           New Rectangle(GameSize.X - ((GameSize.X / 2) - cCameraLockSizeX), 0, ((GameSize.X / 2) - cCameraLockSizeX), GameSize.Y),   'Right
                           New Rectangle(0, 0, ((GameSize.X / 2) - cCameraLockSizeX), GameSize.X)}                                                   'Left
        End Sub
    End Class

End Namespace