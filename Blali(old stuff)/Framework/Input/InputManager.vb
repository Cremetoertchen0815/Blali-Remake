Imports System.Collections.Generic
Imports Emmond.UI.Menu
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input
Imports SharpDX.DirectInput
Imports Joystick = SharpDX.DirectInput.Joystick
Imports JoystickState = SharpDX.DirectInput.JoystickState
Imports Keyboard = Microsoft.Xna.Framework.Input.Keyboard
Imports KeyboardState = Microsoft.Xna.Framework.Input.KeyboardState
Imports Mouse = Microsoft.Xna.Framework.Input.Mouse
Imports MouseState = Microsoft.Xna.Framework.Input.MouseState

Namespace Framework.Input

    <TestState(TestState.NearCompletion)>
    Public Class InputManager
        'DirectInput stuff
        Public Property ConnectionMethod As GamePadType
            Get
                Return SettingsMan.Value(40)
            End Get
            Set(value As GamePadType)
                SettingsMan.Value(40, True) = value
            End Set
        End Property
        Public DxIM As Integer() = {0, 1, 2, -1, 9, 8, 2, 1, 3, 0, 6, 7, 4, 5} 'Input Mapping{Mov X-Axis 0, Mov Y-Axis 1, Aim Axis 2, - 3, Start 4/0, Back 5/1, A 6/2, B 7/3, X 8/4, Y 9/5, LB 10/6, RB 11/7, LT 12/8, RT 13/9}
        Public joystick As Joystick 'The current DirectInput gamepad
        Private cap As Capabilities 'Represents the capabilities of said DirectInput gamepad
        Private HasForceFeedback As Boolean
        Public Multip As Integer() = {1, 1, 1} 'Axis inversion{LS, RS}
        Private initval As Boolean() 'Contains all of the initialisation values of all analog axes
        Public CheckFlag As Boolean = False 'Helper flag for identifying pressed buttons


        'Keyboard & General stuff
        Public KeyIM As Integer() = {Keys.Enter, Keys.F, Keys.Space, -3, Keys.E, Keys.Q, -6, -7, -1, Keys.LeftShift} ' {-1 = Left MB, -2 = Middle MB, -3 = Right MB, -4 = XButton 1, -5 = XButton 2, -6 = Wheel Up, -7 = Wheel Down}
        Private lastwheel As Integer = 0
        Private keysa As New List(Of Keys)
        Private ButtonStack As New List(Of Keys)
        Private oldpress As New List(Of Keys)
        Private distanceS As Double
        Private initialized As Boolean = False

        'Old states
        Private OldCheckMenu As New MenuInputState 'The controller state of the last frame
        Private OldCheckGame As New GameInputState 'The controller state of the last frame

        'Current device states
        Private conret As GamePadState
        Private keyret As KeyboardState
        Private mosret As MouseState
        Private joyret As JoystickState
        Private scrollret As Integer

        'Last device states
        Private lastkeyret As KeyboardState
        Private lastconret As GamePadState

#Region "Properties"
        'Properties
        Dim _AxisMultip As New Vector2(1, 1)
        Public Property AxisMultip As Vector2 'Axis inversion{LS, RS}
            Get
                Dim xs As Integer
                Math.DivRem(CInt(SettingsMan.Value(25)), 2, xs)
                If xs = 1 Then _AxisMultip.X = -1 Else _AxisMultip.X = 1
                If Math.Floor(SettingsMan.Value(25) / 2) = 1 Then _AxisMultip.Y = -1 Else _AxisMultip.Y = 1
                Return _AxisMultip
            End Get
            Set(value As Vector2)
                SettingsMan.Value(25) = 0
                If value.X = -1 Then SettingsMan.Value(25) += 1
                If value.Y = -1 Then SettingsMan.Value(25) += 2
                SettingsMan.Save()
            End Set
        End Property
        Public Property DpadMode As DpadMode
            Get
                If Not SettingsMan.ValueBool(27) Then Return DpadMode.Camera Else Return DpadMode.Movement
            End Get
            Set(value As DpadMode)
                SettingsMan.ValueBool(27) = value = DpadMode.Movement
            End Set
        End Property

        Public Property DisplayHints As HintMode
            Get
                Return SettingsMan.Value(26)
            End Get
            Set(value As HintMode)
                SettingsMan.Value(26, True) = value
            End Set
        End Property

        Public Property MouseSense As Integer
            Get
                Return SettingsMan.Value(28)
            End Get
            Set(value As Integer)
                SettingsMan.Value(28, True) = value
            End Set
        End Property
#End Region

        Public Sub Init()
            LoadControls()
            initialized = False
            Select Case ConnectionMethod
                Case GamePadType.Undefined
                    Dim directInput = New DirectInput()
                    Dim devices As List(Of DeviceInstance) = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                    If GamePad.GetState(PlayerIndex.One).IsConnected Then
                        ConnectionMethod = GamePadType.Xbox
                    ElseIf devices.Count > 0 Then
                        ConnectionMethod = GamePadType.Generic
                        joystick = New Joystick(directInput, devices(0).InstanceGuid)
                        joystick.Properties.BufferSize = 128
                        joystick.Acquire()
                        cap = joystick.Capabilities
                        HasForceFeedback = cap.ForceFeedbackMinimumTimeResolution > 0 And cap.ForceFeedbackSamplePeriod > 0
                        joyret = joystick.GetCurrentState
                        initval = SetDirectInputInits()
                    End If
                Case GamePadType.Generic
                    Dim directInput = New DirectInput()
                    Dim devices As List(Of DeviceInstance) = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                    If devices.Count = 0 Then
                        ConnectionMethod = GamePadType.Undefined
                        Init()
                    Else
                        joystick = New Joystick(directInput, devices(0).InstanceGuid)
                        joystick.Properties.BufferSize = 128
                        joystick.Properties.AxisMode = DeviceAxisMode.Absolute
                        joystick.Acquire()
                        cap = joystick.Capabilities
                        HasForceFeedback = cap.ForceFeedbackMinimumTimeResolution > 0 And cap.ForceFeedbackSamplePeriod > 0
                        joyret = joystick.GetCurrentState
                        initval = SetDirectInputInits()
                    End If
            End Select
        End Sub

        Public Sub GeneralInputUpdate()
            'Record key-strokes(ignore keystrokes if window is not focused, in order to protect privacy[in case game window is minimized and user types in sth confidential]
            keysa.Clear()
            If EmmondInstance.IsActive Then
                For Each A In Keyboard.GetState.GetPressedKeys
                    keysa.Add(A)
                Next
            End If


            For Each element In keysa
                If Not oldpress.Contains(element) Then ButtonStack.Add(element)
            Next

            oldpress.Clear()
            For Each A In keysa
                oldpress.Add(A)
            Next

            If ButtonStack.Count > 20 Then ButtonStack.RemoveAt(0)

            'Activate easter egg
            If SceneMan.CurrentID = 2 AndAlso GetStackKeystroke({Keys.F, Keys.I, Keys.A, Keys.Space, Keys.T, Keys.H, Keys.E, Keys.Space, Keys.W, Keys.A, Keys.T, Keys.E, Keys.R, Keys.F, Keys.A, Keys.L, Keys.L}) Then
                If SceneMan.SceneStack.ContainsKey(2) Then
                    Dim scene As MainMenu = CType(SceneMan.GetScene(2), MainMenu)
                    scene.BGFlag = Not scene.BGFlag
                End If
            End If

            'Delete savefile
            If DebugMode AndAlso GetStackKeystroke({Keys.R, Keys.E, Keys.S, Keys.E, Keys.T}) Then
                MessageBox.Show("x3 *nuzzles* OwO, you so warm!", "Your savefile has been reset.", {"K", "Alrighty", "I don't care, just close this darn window"})
                SettingsMan.ResetSavefile()
            End If

            'Open save file & log directory
            If DebugMode AndAlso GetStackKeystroke({Keys.D, Keys.I, Keys.R}) Then
                Diagnostics.Process.Start(New Diagnostics.ProcessStartInfo("C:\Windows\explorer.EXE", SettingsMan.folder))
            End If

            If Not initialized Then
                distanceS = 0
                Mouse.SetPosition(EmmondInstance.Window.ClientBounds.Center.X, EmmondInstance.Window.ClientBounds.Center.Y)
                initialized = True
            End If


            keyret = Keyboard.GetState
            mosret = Mouse.GetState
            Select Case ConnectionMethod
                Case GamePadType.Xbox
                    conret = GamePad.GetState(PlayerIndex.One)
                Case GamePadType.Generic
                    If joystick Is Nothing Then
                        ConnectionMethod = GamePadType.Undefined
                        Init()
                    Else
                        Try
                            joystick.Poll()
                            joyret = joystick.GetCurrentState
                        Catch ex As Exception
                            Init()
                        End Try
                    End If
            End Select

            If ((keyret.IsKeyDown(Keys.F4) And lastkeyret.IsKeyUp(Keys.F4)) Or (conret.Buttons.LeftShoulder And conret.Buttons.RightShoulder And conret.DPad.Left And Not lastconret.DPad.Left)) And SceneMan.CurrentID <> 10 Then
                Constants.Randm(0.1)
            End If

            Dim wheel As Integer = mosret.ScrollWheelValue
            scrollret = (wheel - lastwheel) / 120
            lastwheel = wheel

            lastkeyret = keyret
            lastconret = conret
        End Sub
#Region "Get States"
        Public Function GetMenuInput() As MenuInputState
            If SceneMan.NoInput Then Return New MenuInputState

            Dim ret As New MenuInputState With {
                        .Start = CompOldNew(GetButton(0) Or keyret.IsKeyDown(Keys.Enter), OldCheckMenu.Start, True),
                        .Back = CompOldNew(GetButton(3) Or keyret.IsKeyDown(Keys.LeftShift) Or keyret.IsKeyDown(Keys.Back) Or keyret.IsKeyDown(Keys.Escape), OldCheckMenu.Back, True),
                        .Confirm = CompOldNew(GetButton(2) Or keyret.IsKeyDown(Keys.Space), OldCheckMenu.Confirm, True),
                        .XtraX = CompOldNew(GetButton(4) Or keyret.IsKeyDown(Keys.E), OldCheckMenu.XtraX, True),
                        .XtraY = CompOldNew(GetButton(5) Or keyret.IsKeyDown(Keys.Q), OldCheckMenu.XtraY, True)
                    }

            If ConnectionMethod = GamePadType.Generic Then
                GetMenuStickDataFromDirectInput(ret)
            Else
                GetMenuStickDataFromXInput(ret)
            End If

            OldCheckMenu.Start = GetButton(0) Or keyret.IsKeyDown(Keys.Enter)
            OldCheckMenu.Back = GetButton(3) Or keyret.IsKeyDown(Keys.LeftShift) Or keyret.IsKeyDown(Keys.Back) Or keyret.IsKeyDown(Keys.Escape)
            OldCheckMenu.Confirm = GetButton(2) Or keyret.IsKeyDown(Keys.Space)
            OldCheckMenu.XtraX = GetButton(4) Or keyret.IsKeyDown(Keys.E)
            OldCheckMenu.XtraY = GetButton(5) Or keyret.IsKeyDown(Keys.Q)


            Return ret

        End Function

        Public Function GetGameInput(disablemouse As Boolean) As GameInputState
            If SceneMan.NoInput Then Return New GameInputState


            Dim ret As New GameInputState With {
                        .Start = CompOldNew(GetButton(0) Or GetKey(KeyIM(0)), OldCheckGame.Start, False), 'Start
                        .Backpack = CompOldNew(GetButton(1) Or GetKey(KeyIM(1)), OldCheckGame.Backpack, False), 'Back
                        .Jump = CompOldNew(GetButton(2) Or GetKey(KeyIM(2)), OldCheckGame.Jump, False), 'A
                        .UseItem = CompOldNew(GetButton(3) Or GetKey(KeyIM(3)), OldCheckGame.UseItem, False), 'B
                        .Interact = CompOldNew(GetButton(4) Or GetKey(KeyIM(4)), OldCheckGame.Interact, False), 'X
                        .DropItem = CompOldNew(GetButton(5) Or GetKey(KeyIM(5)), OldCheckGame.DropItem, False), 'Y
                        .SwitchL = CompOldNew(GetButton(6), OldCheckGame.SwitchL, False) Or GetKey(KeyIM(6)), 'LB
                        .SwitchR = CompOldNew(GetButton(7), OldCheckGame.SwitchR, False) Or GetKey(KeyIM(7)), 'RB
                        .Shoot = CompOldNew(GetButton(8) Or GetKey(KeyIM(8)), OldCheckGame.Shoot, False), 'LT
                        .AimMode = CompOldNew(GetButton(9) Or GetKey(KeyIM(9)), OldCheckGame.AimMode, False) 'RT
                    }

            If ConnectionMethod = GamePadType.Generic Then
                GetGameStickDataFromDirectInput(ret, disablemouse)
            Else
                GetGameStickDataFromXInput(ret, disablemouse)
            End If

            Return ret
        End Function

        Public Function GetCameraInput(IgnoreDpadMode As Boolean) As CameraInputState
            If SceneMan.NoInput Then Return New CameraInputState

            Dim Camera As Vector2 = Vector2.Zero
            Dim Zoom As Single = conret.ThumbSticks.Right.Y

            If keyret.IsKeyDown(Keys.Left) Or (IgnoreDpadMode And conret.DPad.Left = ButtonState.Pressed) Or (Not IgnoreDpadMode And DpadMode = DpadMode.Camera And conret.DPad.Left = ButtonState.Pressed) Then Camera.X = -1
            If keyret.IsKeyDown(Keys.Right) Or (IgnoreDpadMode And conret.DPad.Right = ButtonState.Pressed) Or (Not IgnoreDpadMode And DpadMode = DpadMode.Camera And conret.DPad.Right = ButtonState.Pressed) Then Camera.X = 1
            If keyret.IsKeyDown(Keys.Down) Or (IgnoreDpadMode And conret.DPad.Down = ButtonState.Pressed) Or (Not IgnoreDpadMode And DpadMode = DpadMode.Camera And conret.DPad.Down = ButtonState.Pressed) Then Camera.Y = -1
            If keyret.IsKeyDown(Keys.Up) Or (IgnoreDpadMode And conret.DPad.Up = ButtonState.Pressed) Or (Not IgnoreDpadMode And DpadMode = DpadMode.Camera And conret.DPad.Up = ButtonState.Pressed) Then Camera.Y = 1

            If keyret.IsKeyDown(Keys.S) Then Zoom = -1
            If keyret.IsKeyDown(Keys.W) Then Zoom = 1

            Dim ret As New CameraInputState With {
                        .ShiftFunction = (conret.Buttons.RightStick = ButtonState.Pressed) Or keyret.IsKeyDown(Keys.Q),
                        .Movement = Camera,
                        .Zoom = Zoom
                    }


            Return ret

        End Function
#End Region
#Region "Stick Polling"

        Private Sub GetGameStickDataFromXInput(ByRef ret As GameInputState, disablemouse As Boolean)
            Dim Movement As Vector2
            Dim Aim As Double
            Dim Camera As New Vector2

            'Controll movement
            Movement = conret.ThumbSticks.Left
            If (Movement.X > 0 And Movement.X < cAnalogStickThreshold) Or (Movement.X < 0 And Movement.X > -cAnalogStickThreshold) Then Movement.X = 0
            If (Movement.Y > 0 And Movement.Y < cAnalogStickThreshold) Or (Movement.Y < 0 And Movement.Y > -cAnalogStickThreshold) Then Movement.Y = 0
            If keyret.IsKeyDown(Keys.A) Or (DpadMode = DpadMode.Movement And conret.DPad.Left = ButtonState.Pressed) Then Movement.X = -1
            If keyret.IsKeyDown(Keys.D) Or (DpadMode = DpadMode.Movement And conret.DPad.Right = ButtonState.Pressed) Then Movement.X = 1
            If keyret.IsKeyDown(Keys.S) Or (DpadMode = DpadMode.Movement And conret.DPad.Down = ButtonState.Pressed) Then Movement.Y = -1
            If keyret.IsKeyDown(Keys.W) Or (DpadMode = DpadMode.Movement And conret.DPad.Up = ButtonState.Pressed) Then Movement.Y = 1

            'Aim with Mouse
            If EmmondInstance.IsActive And Not disablemouse Then

                Dim deltaS As Double = System.Windows.Forms.Cursor.Position.Y - EmmondInstance.Window.ClientBounds.Center.Y

                System.Windows.Forms.Cursor.Position = New System.Drawing.Point(EmmondInstance.Window.ClientBounds.Center.X, EmmondInstance.Window.ClientBounds.Center.Y)
                distanceS -= deltaS * 2 / (MouseSense) / 55
                distanceS = Math.Max(Math.Min(distanceS, 1), -1)
                Aim = distanceS
            End If

            'Aim with controller
            If (conret.ThumbSticks.Right.Y > cAnalogStickAim) Or conret.ThumbSticks.Right.Y < -cAnalogStickAim Then
                Aim = conret.ThumbSticks.Right.Y
                Dim prefac As Integer = Math.Abs(Aim) / Aim
                Aim *= Aim * prefac
                distanceS = 0
            End If

            'Move camera
            If keyret.IsKeyDown(Keys.Left) Or (DpadMode = DpadMode.Camera And conret.DPad.Left = ButtonState.Pressed) Then Camera.X = -1
            If keyret.IsKeyDown(Keys.Right) Or (DpadMode = DpadMode.Camera And conret.DPad.Right = ButtonState.Pressed) Then Camera.X = 1
            If keyret.IsKeyDown(Keys.Down) Or (DpadMode = DpadMode.Camera And conret.DPad.Down = ButtonState.Pressed) Then Camera.Y = -1
            If keyret.IsKeyDown(Keys.Up) Or (DpadMode = DpadMode.Camera And conret.DPad.Up = ButtonState.Pressed) Then Camera.Y = 1

            ret.Movement = Movement
            ret.Aim = Aim
            ret.Camera = Camera
        End Sub


        Private Sub GetGameStickDataFromDirectInput(ByRef ret As GameInputState, disablemouse As Boolean)
            Dim Camera As New Vector2
            Dim POVdata As Boolean() = GetPOV(joyret.PointOfViewControllers(0))
            Dim Movement As Vector2 = New Vector2(GetAxisByNumber(DxIM(0)), GetAxisByNumber(DxIM(1))) * New Vector2(Multip(0), Multip(1))
            Dim Aimval As Double = GetAxisByNumber(DxIM(2)) * Multip(2)
            Dim Aim As Double = 0


            'Controll movement
            If (Movement.X > 0 And Movement.X < cAnalogStickThreshold) Or (Movement.X < 0 And Movement.X > -cAnalogStickThreshold) Then Movement.X = 0
            If (Movement.Y > 0 And Movement.Y < cAnalogStickThreshold) Or (Movement.Y < 0 And Movement.Y > -cAnalogStickThreshold) Then Movement.Y = 0
            If keyret.IsKeyDown(Keys.A) Or (DpadMode = DpadMode.Movement And POVdata(3)) Then Movement.X = -1
            If keyret.IsKeyDown(Keys.D) Or (DpadMode = DpadMode.Movement And POVdata(1)) Then Movement.X = 1
            If keyret.IsKeyDown(Keys.S) Or (DpadMode = DpadMode.Movement And POVdata(2)) Then Movement.Y = -1
            If keyret.IsKeyDown(Keys.W) Or (DpadMode = DpadMode.Movement And POVdata(0)) Then Movement.Y = 1

            'Aim with Mouse
            If EmmondInstance.IsActive And Not disablemouse Then
                Dim deltaS As Double = mosret.Position.Y - EmmondInstance.Window.ClientBounds.Center.Y
                Mouse.SetPosition(EmmondInstance.Window.ClientBounds.Center.X, EmmondInstance.Window.ClientBounds.Center.Y)
                distanceS -= deltaS * 2 / (MouseSense) / 55
                distanceS = Math.Max(Math.Min(distanceS, 1), -1)
                Aim = distanceS
            End If

            'Aim with controller
            If (Aimval > cAnalogStickThreshold) Or Aimval < -cAnalogStickThreshold Then
                Aim = Aimval
                distanceS = 0
            End If

            'Move camera
            If keyret.IsKeyDown(Keys.Left) Or (DpadMode = DpadMode.Camera And POVdata(3)) Then Camera.X = -1
            If keyret.IsKeyDown(Keys.Right) Or (DpadMode = DpadMode.Camera And POVdata(1)) Then Camera.X = 1
            If keyret.IsKeyDown(Keys.Down) Or (DpadMode = DpadMode.Camera And POVdata(2)) Then Camera.Y = -1
            If keyret.IsKeyDown(Keys.Up) Or (DpadMode = DpadMode.Camera And POVdata(0)) Then Camera.Y = 1

            ret.Movement = Movement
            ret.Aim = Aim
            ret.Camera = Camera
        End Sub

        Private Sub GetMenuStickDataFromXInput(ByRef ret As MenuInputState)
            Dim LeftAna As Vector2 = Vector2.Zero
            If keyret.IsKeyDown(Keys.A) Or conret.DPad.Left Or conret.ThumbSticks.Left.X < -cDigitalStickThreshold Then LeftAna.X = -1
            If keyret.IsKeyDown(Keys.D) Or conret.DPad.Right Or conret.ThumbSticks.Left.X > cDigitalStickThreshold Then LeftAna.X = 1
            If keyret.IsKeyDown(Keys.S) Or conret.DPad.Down Or conret.ThumbSticks.Left.Y < -cDigitalStickThreshold Then LeftAna.Y = -1
            If keyret.IsKeyDown(Keys.W) Or conret.DPad.Up Or conret.ThumbSticks.Left.Y > cDigitalStickThreshold Then LeftAna.Y = 1

            ret.Movement = LeftAna
        End Sub

        Private Sub GetMenuStickDataFromDirectInput(ByRef ret As MenuInputState)
            Dim Movement As Vector2 = New Vector2(GetAxisByNumber(DxIM(0)), GetAxisByNumber(DxIM(1))) * New Vector2(Multip(0), Multip(1))
            Dim LeftAna As Vector2 = Vector2.Zero
            Dim POVdata As Boolean() = GetPOV(joyret.PointOfViewControllers(0))

            If keyret.IsKeyDown(Keys.A) Or POVdata(3) Or Movement.X < -cDigitalStickThreshold Then LeftAna.X = -1
            If keyret.IsKeyDown(Keys.D) Or POVdata(1) Or Movement.X > cDigitalStickThreshold Then LeftAna.X = 1
            If keyret.IsKeyDown(Keys.S) Or POVdata(2) Or Movement.Y < -cDigitalStickThreshold Then LeftAna.Y = -1
            If keyret.IsKeyDown(Keys.W) Or POVdata(0) Or Movement.Y > cDigitalStickThreshold Then LeftAna.Y = 1
            ret.Movement = LeftAna
        End Sub
#End Region
#Region "Common Functions"
        Public Sub SetRumble(motorA As Single, motorB As Single)
            Select Case ConnectionMethod
                Case GamePadType.Xbox
                    GamePad.SetVibration(0, motorA, motorB)
                Case GamePadType.Generic
                    If HasForceFeedback Then
                        If motorA > 0 Or motorB > 0 Then
                            joystick.SendForceFeedbackCommand(ForceFeedbackCommand.Continue)
                        Else
                            joystick.SendForceFeedbackCommand(ForceFeedbackCommand.Pause)
                        End If
                    End If
            End Select
        End Sub
        Private Function CompOldNew(news As Boolean, old As Boolean, DoubleCheck As Boolean) As Boolean
            If DoubleCheck Then
                Return news And Not old
            Else
                Return news
            End If
        End Function

        Private Function GetStackKeystroke(keysa As Keys()) As Boolean
            If keysa.Length > ButtonStack.Count Then Return False
            For i As Integer = 0 To keysa.Length - 1
                If ButtonStack(ButtonStack.Count - i - 1) <> keysa(keysa.Length - i - 1) Then Return False
            Next
            ButtonStack.Add(Keys.BrowserBack)
            Return True
        End Function

        'Get Buttons from GamePad
        Public Function GetButton(nr As Integer) As Boolean
            If ConnectionMethod = GamePadType.Xbox Then
                Select Case nr
                    Case 0
                        Return (conret.Buttons.Start = ButtonState.Pressed)
                    Case 1
                        Return (conret.Buttons.Back = ButtonState.Pressed)
                    Case 2
                        Return (conret.Buttons.A = ButtonState.Pressed)
                    Case 3
                        Return (conret.Buttons.B = ButtonState.Pressed)
                    Case 4
                        Return (conret.Buttons.X = ButtonState.Pressed)
                    Case 5
                        Return (conret.Buttons.Y = ButtonState.Pressed)
                    Case 6
                        Return (conret.Buttons.LeftShoulder = ButtonState.Pressed)
                    Case 7
                        Return (conret.Buttons.RightShoulder = ButtonState.Pressed)
                    Case 8
                        Return (conret.Triggers.Left > cAnalogTriggerThreshold)
                    Case 9
                        Return (conret.Triggers.Right > cAnalogTriggerThreshold)
                    Case Else
                        Return False
                End Select
            ElseIf ConnectionMethod = GamePadType.Generic Then
                If DxIM(4 + nr) < 0 Then Return False
                Return joyret.Buttons(DxIM(4 + nr))
            Else
                Return False
            End If
        End Function

        'Get Buttons from Keyboard & Mouse
        Private Function GetKey(int As Integer) As Boolean
            If int < -7 Then Return False
            Select Case int
                Case -1
                    Return (mosret.LeftButton And EmmondInstance.IsActive)
                Case -2
                    Return mosret.MiddleButton
                Case -3
                    Return (mosret.RightButton And EmmondInstance.IsActive)
                Case -4
                    Return mosret.XButton1
                Case -5
                    Return mosret.XButton2
                Case -6
                    Return scrollret > 0
                Case -7
                    Return scrollret < 0
                Case Else
                    Return (keyret.IsKeyDown(int) And EmmondInstance.IsActive)
            End Select
        End Function
        Public Function GetPressedKey(pressok As Boolean) As Integer
            If pressok Then Return -8
            For i As Integer = 1 To 7
                If GetKey(-i) Then Return -i
            Next
            If keyret.GetPressedKeys.Length > 0 Then
                Return keyret.GetPressedKeys(0)
            End If
            Return -8
        End Function
        Public Function DoesContainKey(n As Integer) As Boolean
            Dim ret As Boolean = False
            For i As Integer = 0 To KeyIM.Length - 1
                If KeyIM(i) = n Then ret = True
            Next

            Return ret
        End Function
        Public Function IsXInputComp() As Boolean
            Return GamePad.GetState(PlayerIndex.One).IsConnected
        End Function
        Public Function IsDirectInputComp() As Boolean
            Dim directInput = New DirectInput()
            Dim devices As List(Of DeviceInstance) = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
            Return devices.Count > 0
        End Function
        Sub SaveControls()
            'Generic Data
            For i As Integer = 0 To 13
                SettingsMan.Value(41 + i) = SbyteToByte(DxIM(i))
            Next
            'Keyboard Data
            For i As Integer = 0 To 9
                Dim tmp As Byte() = IntToByteArray(KeyIM(i))
                SettingsMan.Value(55 + (2 * i)) = tmp(0)
                SettingsMan.Value(56 + (2 * i)) = tmp(1)
            Next

            If Multip(0) = 1 Then SettingsMan.Value(75) = 0 Else SettingsMan.Value(75) = 1
            If Multip(1) = 1 Then SettingsMan.Value(76) = 0 Else SettingsMan.Value(76) = 1
            If Multip(2) = 1 Then SettingsMan.Value(77) = 0 Else SettingsMan.Value(77) = 1

            SettingsMan.Save()
        End Sub

        Sub LoadControls()
            'Generic Data
            For i As Integer = 0 To 13
                DxIM(i) = ByteToSByte(SettingsMan.Value(41 + i))
            Next
            'Keyboard Data
            For i As Integer = 0 To 9
                Dim tmp As Byte() = {SettingsMan.Value(55 + (2 * i)), SettingsMan.Value(56 + (2 * i))}
                KeyIM(i) = IntFromByteArray(tmp)
            Next

            If SettingsMan.Value(75) = 0 Then Multip(0) = 1 Else Multip(0) = -1
            If SettingsMan.Value(76) = 0 Then Multip(1) = 1 Else Multip(1) = -1
            If SettingsMan.Value(77) = 0 Then Multip(2) = 1 Else Multip(2) = -1
        End Sub

        Private Function IntToByteArray(input As Integer) As Byte()
            Dim ret As Integer() = {0, 0}
            ret(0) = Math.DivRem(Math.Abs(input), 256, ret(1))
            If input < 0 Then ret(0) = Byte.MaxValue
            Return {ret(0), ret(1)}
        End Function
        Private Function IntFromByteArray(input As Byte()) As Integer
            If input(0) = Byte.MaxValue Then Return input(1) * -1
            Return (input(0) * 256) + input(1)
        End Function
        Private Function SbyteToByte(SByteS As SByte) As Byte
            If SByteS < 0 Then Return Byte.MaxValue Else Return SByteS
        End Function
        Private Function ByteToSByte(ByteS As Byte) As SByte
            If ByteS = Byte.MaxValue Then Return -1 Else Return ByteS
        End Function
#End Region
#Region "DirectInput Functions"
        Private Function SetDirectInputInits() As Boolean()
            Dim res As New List(Of Boolean)
            For i As Integer = 0 To 5
                res.Add(GetAxisByNumber(i, True) > 0)
            Next
            Return res.ToArray
        End Function

        Private Function GetAxisByNumber(number As Integer, Optional Direct As Boolean = False) As Double
            If Not Direct Then
                Select Case number
                    Case 0
                        If initval(0) Then Return Math.Round(joyret.X / 65535 * 2, 2) - 1 Else Return 0
                    Case 1
                        If initval(1) Then Return Math.Round(joyret.Y / 65535 * 2, 2) - 1 Else Return 0
                    Case 2
                        If initval(2) Then Return Math.Round((joyret.Z / 65535 * 2), 2) - 1 Else Return 0
                    Case 3
                        If initval(3) Then Return Math.Round((joyret.RotationX / 65535 * 2), 2) - 1 Else Return 0
                    Case 4
                        If initval(4) Then Return Math.Round((joyret.RotationY / 65535 * 2), 2) - 1 Else Return 0
                    Case 5
                        If initval(5) Then Return Math.Round((joyret.RotationZ / 65535 * 2), 2) - 1 Else Return 0
                    Case Else
                        Return 0
                End Select
            Else
                Select Case number
                    Case 0
                        Return joyret.X
                    Case 1
                        Return joyret.Y
                    Case 2
                        Return joyret.Z
                    Case 3
                        Return joyret.RotationX
                    Case 4
                        Return joyret.RotationY
                    Case 5
                        Return joyret.RotationZ
                    Case Else
                        Return 0
                End Select
            End If
        End Function

        Private Function GetPOV(number As Integer) As Boolean() 'Up, Right, Down, Left
            Dim res As Boolean() = {False, False, False, False}
            If number > -1 Then
                Dim tempn As Single = number / 9000
                Dim af As Integer = Math.Floor(tempn)
                Dim bf As Integer = Math.Ceiling(tempn)
                If af < 4 Then res(af) = True
                If bf < 4 Then res(bf) = True
            End If
            Return res
        End Function
        Function GetPressedButton(acflag As Boolean) As Integer
            If Not acflag Then CheckFlag = True
            Dim res As Integer = -1
            If CheckFlag Then
                For i As Integer = 0 To joystick.Capabilities.ButtonCount - 1
                    If joystick.GetCurrentState.Buttons(i) Then res = i
                Next
            End If
            Return res
        End Function

        Function GetPressedAxis() As Integer
            Dim res As Integer = -1
            For i As Integer = 0 To 5
                If GetAxisByNumber(i) > cAnalogStickThreshold Or GetAxisByNumber(i) < -cAnalogStickThreshold Then res = i
            Next
            Return res
        End Function


        Public Function DoesContain(AsAxis As Boolean, n As Integer) As Boolean
            Dim ret As Boolean = False
            If AsAxis Then
                For i As Integer = 0 To 3
                    If DxIM(i) = n Then ret = True
                Next
            Else
                For i As Integer = 4 To DxIM.Length - 1
                    If DxIM(i) = n Then ret = True
                Next
            End If

            Return ret
        End Function
        Public Sub LoadGeneric()
            Dim directInput = New DirectInput()
            Dim devices As List(Of DeviceInstance) = directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
            If devices.Count = 0 Then
                ConnectionMethod = GamePadType.Undefined
                Init()
            Else
                joystick = New Joystick(directInput, devices(0).InstanceGuid)
                joystick.Properties.BufferSize = 128
                joystick.Properties.AxisMode = DeviceAxisMode.Absolute
                joystick.Acquire()
                cap = joystick.Capabilities
                joyret = joystick.GetCurrentState
                initval = SetDirectInputInits()
            End If
        End Sub
    End Class
#End Region


End Namespace