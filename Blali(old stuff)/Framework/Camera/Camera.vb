Imports System.Collections.Generic
Imports Emmond.Framework.Input
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Namespace Framework.Camera
    ''' <summary>
    ''' Provides methods for moving the camera, collision with camera barriers and calculating the transform matrix
    ''' </summary>
    <TestState(TestState.WorkInProgress)>
    Public Class Camera

#Region "Variables"
        'Movement flags
        Public RapidMode As Boolean = False
        Public RapidModeHeight As Integer
        Public BlockFreeMovement As Boolean = False
        Public BaseVelocity As Vector2
        Public AdditionalImpulse As Vector2
        Public Acceleration As Vector2

        'Camera properties from last frame
        Dim lastZoom As Single
        Dim lastLocation As Vector2
        Dim lastLocationB As Vector2
        Dim lastRotation As Double

        Dim Bounds As Vector2 'The boundaries of the screen
        Dim CurrentMatrix As Matrix 'Represents the current matrix
        Dim CurrentInvertedMatrix As Matrix 'Represents the current matrix
        Dim WOLocationMatrix As Matrix 'Represents the current matrix, but with the location excluded
        Dim NewMA As Rectangle
        Dim NewMAX As Rectangle 'Represents the future horizontal vision rectangle
        Dim NewMAY As Rectangle 'Represents the future vertical vision rectangle
        Dim IsLevel As Boolean
        Dim state As CameraInputState
        Dim tstmatX As Matrix
        Dim tstmatY As Matrix

        'Camera properties
        Public Zoom As Single = 1
        Public Location As Vector2
        Public Rotation As Double
#End Region

#Region "Properties"
        'Default camera properties
        Public Property DefaultLocation As Vector2

        'Other flags
        ''' <summary>
        ''' A list of points that the camera is not allowed to hit when scrolling.
        ''' </summary>
        Public Property NoCameraPoints As List(Of Vector2)
        ''' <summary>
        ''' The boundaries of the environment the camera is in at the moment.
        ''' </summary>
        Public Property LevelBounds As Rectangle
        ''' <summary>
        ''' Indicates whether Manual camera movement in X-direction is enabled
        ''' </summary>
        Public Property AllowHorizontal As Boolean
        ''' <summary>
        ''' Indicates whether Manual camera movement in X-direction is enabled
        ''' </summary>
        Public Property AllowVertical As Boolean
        ''' <summary>
        ''' Indicates whether Manual camera movement in Y-direction is enabled
        ''' </summary>
        Public Property DebugCam As Boolean
        ''' <summary>
        ''' Enables the camera to move faster than default speed, in order to catch up with the player
        ''' </summary>
        Public Property SpeedMode As Boolean() = {False, False}
        ''' <summary>
        ''' Represents the camera location with (0,0) as origin
        ''' </summary>
        Public Property DefaultOriginLocation As Vector2
            Get
                Return Location - (Bounds / 2)
            End Get
            Set(value As Vector2)
                Location = value + (Bounds / 2)
            End Set
        End Property
        ''' <summary>
        ''' Indicates whether Manual camera movement in X-direction is enabled
        ''' </summary>
        Public ReadOnly Property Viewport As Rectangle
            Get
                Return NewMA
            End Get
        End Property
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Creates a new instance of the Camera class.
        ''' </summary>
        ''' <param name="IsALevel">Indicates whether the user should have boundless control over the camera.</param>
        Sub New(IsALevel As Boolean)
            Bounds = GameSize
            NoCameraPoints = New List(Of Vector2)
            Location = Bounds / 2
            Me.IsLevel = IsALevel
        End Sub
#End Region

#Region "Standard Methods"

        ''' <summary>
        ''' Recalculates camera position, bounding boxes, matrices, etc.
        ''' </summary>
        ''' <param name="gameTime">>Provides a snapshot of timing values.</param>
        Public Overridable Sub UpdateCamera(gameTime As GameTime)

            'Manually move camera
            state = InputMan.GetCameraInput(Not IsLevel)
            Dim ManualCameraX As Double = state.Movement.X
            Dim ManualCameraY As Double = state.Movement.Y
            Dim ManualCameraZ As Double = state.Zoom
            If state.ShiftFunction And DebugMode Then
                If Not BlockFreeMovement Then Zoom += ManualCameraY / 100
            Else
                If Not BlockFreeMovement And AllowHorizontal And Not Keyboard.GetState.IsKeyDown(Keys.LeftControl) And ManualCameraY <> 0 Then Acceleration.Y = -ManualCameraY * 5
                If Not BlockFreeMovement And AllowVertical And Not Keyboard.GetState.IsKeyDown(Keys.LeftControl) And ManualCameraX <> 0 Then Acceleration.X = ManualCameraX * 5
            End If

            'Translate the acceleration to velocity
            Dim accX As Single = Acceleration.X * Math.Pow(gameTime.ElapsedGameTime.TotalSeconds * 60, 2)
            Dim accY As Single = Acceleration.Y * Math.Pow(gameTime.ElapsedGameTime.TotalSeconds * 60, 2)
            If accX > 0 Then accX = Math.Max(accX, cCameraAccelerationHorizontal)
            If accX < 0 Then accX = Math.Min(accX, -cCameraAccelerationHorizontal)
            If accY > 0 Then accY = Math.Max(accY, cCameraAccelerationVectical)
            If accY < 0 Then accY = Math.Min(accY, -cCameraAccelerationVectical)
            BaseVelocity += New Vector2(accX, Math.Min(Math.Max(accY, -0.6), 0.6))

            'Braking
            If Acceleration.X = 0 And BaseVelocity.X > 0 Then BaseVelocity.X *= cCameraDeccelerationHorizontal
            If Acceleration.X = 0 And BaseVelocity.X < 0 Then BaseVelocity.X *= cCameraDeccelerationHorizontal
            If Acceleration.Y = 0 And BaseVelocity.Y > 0 Then BaseVelocity.Y *= cCameraDeccelerationVectical
            If Acceleration.Y = 0 And BaseVelocity.Y < 0 Then BaseVelocity.Y *= cCameraDeccelerationVectical

            'Minimal velocity
            If (BaseVelocity.X < cCameraAccelerationHorizontal And BaseVelocity.X > 0) Or (BaseVelocity.X > -cCameraAccelerationHorizontal And BaseVelocity.X < 0) Then BaseVelocity.X = 0
            If (BaseVelocity.Y < cCameraAccelerationVectical And BaseVelocity.Y > 0) Or (BaseVelocity.Y > -cCameraAccelerationVectical And BaseVelocity.Y < 0) Then BaseVelocity.Y = 0

            'Limit the maximum Camera Speed
            If BaseVelocity.X > 0 And SpeedMode(0) Then BaseVelocity.X = Math.Min(BaseVelocity.X, cCameraMaxSpeedHorizontalFast)
            If BaseVelocity.X < 0 And SpeedMode(0) Then BaseVelocity.X = Math.Max(BaseVelocity.X, -cCameraMaxSpeedHorizontalFast)
            If BaseVelocity.Y > 0 And SpeedMode(1) Then BaseVelocity.Y = Math.Min(BaseVelocity.Y, cCameraMaxSpeedVecticalFast)
            If BaseVelocity.Y < 0 And SpeedMode(1) Then BaseVelocity.Y = Math.Max(BaseVelocity.Y, -cCameraMaxSpeedVecticalFast)
            If BaseVelocity.X > 0 And Not SpeedMode(0) Then BaseVelocity.X = Math.Min(BaseVelocity.X, cCameraMaxSpeedHorizontal)
            If BaseVelocity.X < 0 And Not SpeedMode(0) Then BaseVelocity.X = Math.Max(BaseVelocity.X, -cCameraMaxSpeedHorizontal)
            If BaseVelocity.Y > 0 And Not SpeedMode(1) Then BaseVelocity.Y = Math.Min(BaseVelocity.Y, cCameraMaxSpeedVectical)
            If BaseVelocity.Y < 0 And Not SpeedMode(1) Then BaseVelocity.Y = Math.Max(BaseVelocity.Y, -cCameraMaxSpeedVectical)


            Dim movflagX As Boolean = True
            Dim movflagY As Boolean = True

            If RapidMode Then
                'Create test matrices
                tstmatX = Matrix.CreateTranslation(New Vector3(-Location.X - (BaseVelocity.X + AdditionalImpulse.X) * gameTime.ElapsedGameTime.TotalSeconds * 60 + Bounds.X * 0.5F, -Location.Y + Bounds.Y * 0.5F, 0))
                tstmatY = Matrix.CreateTranslation(New Vector3(-Location.X + Bounds.X * 0.5F, RapidModeHeight + Bounds.Y * 0.5F, 0))

                'Get viewport of test matrix
                NewMAX = GetVisibleArea(tstmatX)
                NewMAY = GetVisibleArea(tstmatY)
                'Check whether the camera operates within the level boundaries
                If NewMAX.Left < 0 Or NewMAX.Right > LevelBounds.Width Then movflagX = False
                If NewMAY.Bottom < 0 Then movflagY = False

                'Check whether the camera is outside a No-Go-Area
                For Each element In NoCameraPoints
                    If NewMAY.Contains(element.X, element.Y) Then movflagY = False
                Next

                'If one of the statements above was False, stop the camera
                If Not movflagX Or Not AllowHorizontal Then BaseVelocity.X = 0
                If movflagY Then Location.Y = RapidModeHeight
            Else
                'If DebugCam is disabled and the level boundaries are smaller than the viewport, calculate camera stops
                If Not DebugCam AndAlso (LevelBounds.Size.X >= Bounds.X Or LevelBounds.Size.Y >= Bounds.Y) Then
                    'Create test matrices
                    tstmatX = Matrix.CreateTranslation(New Vector3(-Location.X - (BaseVelocity.X + AdditionalImpulse.X) * gameTime.ElapsedGameTime.TotalSeconds * 60 + Bounds.X * 0.5F, -Location.Y + Bounds.Y * 0.5F, 0))
                    tstmatY = Matrix.CreateTranslation(New Vector3(-Location.X + Bounds.X * 0.5F, -Location.Y - (BaseVelocity.Y + AdditionalImpulse.Y) * gameTime.ElapsedGameTime.TotalSeconds * 60 + Bounds.Y * 0.5F, 0))

                    'Get viewport of test matrix
                    NewMAX = GetVisibleArea(tstmatX)
                    NewMAY = GetVisibleArea(tstmatY)

                    'Check whether the camera operates within the level boundaries
                    If NewMAX.Left < 0 Or NewMAX.Right > LevelBounds.Width Then movflagX = False
                    If NewMAY.Top < 1080 - LevelBounds.Height Or NewMAY.Bottom > Bounds.Y Then movflagY = False

                    'Check whether the camera is outside a No-Go-Area
                    For Each element In NoCameraPoints
                        'If GetSideIntersection(NewMA, element)(0) Then movflagX = False
                        'If GetSideIntersection(NewMA, element)(1) Then movflagY = False
                        If NewMAX.Contains(element.X, element.Y) Then movflagX = False
                        If NewMAY.Contains(element.X, element.Y) Then movflagY = False
                    Next

                    'If one of the statements above was False, stop the camera
                    If Not movflagX Or Not AllowHorizontal Then BaseVelocity.X = 0 : AdditionalImpulse.X = 0
                    If Not movflagY Or Not AllowVertical Then BaseVelocity.Y = 0 : AdditionalImpulse.Y = 0
                End If
            End If
            Location += (BaseVelocity + AdditionalImpulse) * gameTime.ElapsedGameTime.TotalSeconds * 60

            If Location <> lastLocation And Not DebugMode Then
                CurrentMatrix = Matrix.CreateTranslation(New Vector3(Math.Floor(-Location.X + Bounds.X * 0.5F), Math.Floor(-Location.Y + Bounds.Y * 0.5F), 0))
                CurrentInvertedMatrix = Matrix.Invert(CurrentMatrix)
            ElseIf DebugMode And cCameraCalcDebugView And IsLevel Then
                CurrentMatrix = Matrix.CreateTranslation(New Vector3(Math.Floor(-Location.X + Bounds.X * 0.5F), Math.Floor(-Location.Y + Bounds.Y * 0.5F), 0))
                CurrentInvertedMatrix = Matrix.Invert(CurrentMatrix)
                WOLocationMatrix = Matrix.Identity
            ElseIf DebugMode And (Not cCameraCalcDebugView Or Not IsLevel) Then
                CurrentMatrix =
                        Matrix.CreateTranslation(New Vector3(Math.Floor(-Location.X), Math.Floor(-Location.Y), 0)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(Zoom) *
                        Matrix.CreateTranslation(New Vector3(Bounds.X * 0.5F, Bounds.Y * 0.5F, 0))
                CurrentInvertedMatrix = Matrix.Invert(CurrentMatrix)
                WOLocationMatrix =
                            Matrix.CreateRotationZ(Rotation) *
                            Matrix.CreateScale(Zoom)
            End If

            NewMA = GetVisibleArea()

            'Move camera along vector

            lastLocation = Location
            lastLocationB = Location
            lastRotation = Rotation
            lastZoom = Zoom

            Acceleration = Vector2.Zero
        End Sub
#End Region

#Region "Functions"
        ''' <summary>
        ''' Returns a transformation matrix that corresponds to the camera settings.
        ''' </summary>
        ''' <returns>Transformation matrix that can be used as parameter of your sprite batch.</returns>
        Public Function GetMatrix(Optional view As Boolean = False, Optional dbgexclusive As Boolean = False) As Matrix
            If DebugMode And cCameraCalcDebugView And view Then
                If dbgexclusive Then
                    Return Matrix.CreateTranslation(New Vector3(Bounds.X * -0.5F, Bounds.Y * -0.5F, 0)) *
                            Matrix.CreateRotationZ(Rotation) *
                            Matrix.CreateScale(Zoom) *
                            Matrix.CreateTranslation(New Vector3(Bounds.X * 0.5F, Bounds.Y * 0.5F, 0))
                Else

                    Return Matrix.CreateTranslation(New Vector3(Math.Floor(-Location.X), Math.Floor(-Location.Y), 0)) *
                            Matrix.CreateRotationZ(Rotation) *
                            Matrix.CreateScale(Zoom) *
                            Matrix.CreateTranslation(New Vector3(Bounds.X * 0.5F, Bounds.Y * 0.5F, 0))
                End If
            Else
                Return CurrentMatrix
            End If
        End Function

        ''' <summary>
        ''' Returns an inverted transformation matrix that corresponds to the camera settings.
        ''' </summary>
        ''' <returns>Transformation matrix that can be used as parameter of your sprite batch.</returns>
        Public Function GetInvertedMatrix() As Matrix
            Return CurrentInvertedMatrix
        End Function

        ''' <summary>
        ''' Converts a point in world space into cam space.
        ''' </summary>
        ''' <param name="vec">The point to be converted.</param>
        ''' <param name="LocationalExclusiveMode">Indicates whether point (0,0) should be used as origin.</param>
        ''' <returns></returns>
        Public Function TranslateToCamera(vec As Vector2, Optional LocationalExclusiveMode As Boolean = False) As Vector2
            If Not LocationalExclusiveMode Then
                Return Vector2.Transform(vec, CurrentMatrix)
            Else
                Return Vector2.Transform(vec, WOLocationMatrix)
            End If
        End Function

        ''' <summary>
        ''' Converts a point in cam space into world space.
        ''' </summary>
        ''' <param name="vec">The point to be converted.</param>
        ''' <param name="LocationalExclusiveMode">Indicates whether point (0,0) should be used as origin.</param>
        ''' <returns></returns>
        Public Function TranslateFromCamera(vec As Vector2, Optional LocationalExclusiveMode As Boolean = False) As Vector2
            If Not LocationalExclusiveMode Then
                Return Vector2.Transform(vec, CurrentInvertedMatrix)
            Else
                Return Vector2.Transform(vec, Matrix.Invert(WOLocationMatrix))
            End If
        End Function

        Private Function GetVisibleArea() As Rectangle
            Dim inverseViewMatrix As Matrix = Matrix.Invert(CurrentMatrix)
            Dim tl As Vector2 = Vector2.Transform(Vector2.Zero, inverseViewMatrix)
            Dim tr As Vector2 = Vector2.Transform(New Vector2(Bounds.X, 0), inverseViewMatrix)
            Dim bl As Vector2 = Vector2.Transform(New Vector2(0, Bounds.Y), inverseViewMatrix)
            Dim br As Vector2 = Vector2.Transform(Bounds, inverseViewMatrix)
            Dim min As Vector2 = New Vector2(MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))), MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))))
            Dim max As Vector2 = New Vector2(MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))), MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))))
            Return New Rectangle(CInt(Math.Truncate(min.X)), CInt(Math.Truncate(min.Y)), CInt(Math.Truncate(max.X - min.X)), CInt(Math.Truncate(max.Y - min.Y)))
        End Function
        Private Function GetVisibleArea(inv As Matrix) As Rectangle
            Dim inverseViewMatrix As Matrix = Matrix.Invert(inv)
            Dim tl As Vector2 = Vector2.Transform(Vector2.Zero, inverseViewMatrix)
            Dim tr As Vector2 = Vector2.Transform(New Vector2(Bounds.X, 0), inverseViewMatrix)
            Dim bl As Vector2 = Vector2.Transform(New Vector2(0, Bounds.Y), inverseViewMatrix)
            Dim br As Vector2 = Vector2.Transform(Bounds, inverseViewMatrix)
            Dim min As Vector2 = New Vector2(MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))), MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))))
            Dim max As Vector2 = New Vector2(MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))), MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))))
            Return New Rectangle(CInt(Math.Truncate(min.X)), CInt(Math.Truncate(min.Y)), CInt(Math.Truncate(max.X - min.X)), CInt(Math.Truncate(max.Y - min.Y)))
        End Function
#End Region
    End Class
End Namespace
