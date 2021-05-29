Imports System
Imports Nez.Tiled

Namespace Games.Blali_1
    Public Class PlayerMover
        Inherits Component
        Implements IUpdatable


        Dim BtnMove As VirtualJoystick
        Dim BtnJump As VirtualButton
        Dim Velocity As Vector2
        Dim OldVelocity As Vector2
        Dim WallJumpLeft As Boolean
        Dim WallJumpRight As Boolean
        Dim AccFlip As Boolean
        Dim PlayerState As PlayerStatus = PlayerStatus.Idle 'NOTE TO MYSELF: This enum should contain ALL moves for ALL player types
        Dim JumpState As Integer = 0 '0 = No jumping, 1 = in jump, 2 = extended jump
        Dim DisableJumpA As Boolean = False 'You can't jump(Stage A)
        Dim DisableJumpB As Boolean = False 'You can't jump(Stage B)
        Dim ShootDownwards As Boolean = False
        Dim DoubleJumpCounter As Integer = 0
        Dim DoubleJump As Boolean = False
        Dim DisableWallJump As Boolean = False 'You can't wall-jump
        Dim ActiveWallJump As Boolean = False 'You can wall-jump
        Dim WallJumpCounter As Integer = 0 'The time since the last wall cling(in ms)
        Dim landcounter As Integer 'Counter flag for landing freeze
        Dim landfreeze As Boolean = False
        Dim CanDoubleJump As Boolean = True
        Dim CanWallJump As Boolean = False

        Public Shared cHorizontalTerminalVelocity As Single = 850 'Horizontal terminal velocity °
        Public Shared cPlayerDecceleration As Single = 5 'Decceleration speed on the ground °
        Public Shared cPlayerAirAcceleration As Single = 1.5 'Acceleration speed in mid-air °
        Public Shared cPlayerAirDecceleration As Single = 0.8 'Decceleration speed in mid-air °
        Public Shared cPlayerAcceleration As Single = 0.75 'Acceleration speed on the ground °
        Public Shared cPlayerWallJumpSlidingSpeed As Single = 50 'Acceleration speed on the ground °
        Public Shared cPlayerJumpDoubleRatio As Single = 0.64 'The ratio between basic jump height and additional jump height °

        Public MoveSpeed As Single = 3500
        Public Gravity As Single = 5500
        Public JumpHeight As Single = 2200
        Public Map As TmxMap
        Private _mover As TiledMapMover
        Private _boxCollider As BoxCollider
        Private _collisionState As New TiledMapMover.CollisionState()
        Public Sub New(map As TmxMap)
            Me.Map = map
        End Sub

        Public Overrides Sub OnRemovedFromEntity()
            BtnMove.Deregister()
            BtnJump.Deregister()
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            'Add components
            _boxCollider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 64, 256)))
            _mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Entity.AddComponent(New PrototypeSpriteRenderer(64, 256)).LocalOffset = New Vector2(32, 128)

            BtnMove = New VirtualJoystick(True, New VirtualJoystick.GamePadLeftStick, New VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.A, Keys.D, Keys.W, Keys.S))
            BtnJump = New VirtualButton(500, New VirtualButton.GamePadButton(0, Buttons.A), New VirtualButton.KeyboardKey(Keys.Space)) With {.BufferTime = 0}
        End Sub

        Public Property Enable As Boolean = True Implements IUpdatable.Enabled
        Private ReadOnly Property IUpdatable_UpdateOrder As Integer Implements IUpdatable.UpdateOrder
            Get
                Return 0
            End Get
        End Property
        Public Sub Update() Implements IUpdatable.Update

            '1st Jumping
            Dim NoDrop As Boolean = BtnMove.Value.Y >= -0.8
            Dim movvec As Single = BtnMove.Value.X

            If PlayerState <> PlayerStatus.WallClinging Then
                Select Case JumpState
                    Case 0 'In case no jumping is happening
                        If BtnJump.IsPressed And NoDrop And Not DisableJumpA And Not landfreeze And _collisionState.Below Then 'Initiate the jump
                            'Start launching phase and initialize flags
                            JumpState = 1
                            PlayerState = PlayerStatus.Jumping
                            Velocity.Y = -JumpHeight
                            'DisableJumpA = True
                        ElseIf BtnJump.IsPressed And NoDrop And CanDoubleJump And Not DisableJumpB And Not _collisionState.Below Then 'Do Double Jump from floor
                            DoubleJump = True
                        End If
                        If _collisionState.Left Or _collisionState.Right Or _collisionState.Below Then DisableWallJump = True 'Disable WallClinging
                    Case 1
                        'Cling to wall
                        If Not DisableWallJump And CanWallJump And (((WallJumpLeft And movvec > 0) Or (WallJumpRight And movvec < 0)) Or (PlayerState = PlayerStatus.JumpingFromWall And ((WallJumpLeft And OldVelocity.X > 0) Or (WallJumpRight And OldVelocity.X < 0)))) Then 'Check if you can wall jump
                            PlayerState = PlayerStatus.WallClinging
                            ActiveWallJump = True
                            Velocity = New Vector2(0, cPlayerWallJumpSlidingSpeed)
                        ElseIf BtnJump.IsPressed And Not DisableJumpB And CanDoubleJump And NoDrop Then 'Do Double Jump
                            DoubleJump = True
                        ElseIf Velocity.Y <= 0 And _collisionState.Below Then 'End Jump And Not BtnJump.IsDownLastFrame
                            JumpState = 2
                            If WallJumpLeft Or WallJumpRight Then DisableWallJump = True : JumpState = 0
                        End If
                    Case 2
                        If _collisionState.Below Then
                            'Stop the extended jump
                            JumpState = 0
                            DisableJumpB = False
                        End If
                End Select
            End If

            If DoubleJump Then
                JumpState = 2
                Velocity.Y = -JumpHeight * cPlayerJumpDoubleRatio
                DisableJumpB = True
                PlayerState = PlayerStatus.ExJumping
                ShootDownwards = True
                DoubleJump = False
                DoubleJumpCounter = 0
            End If

            'Preparing Horizontal Controls
            Dim mp As Single = Time.DeltaTime * 6000
            Dim acc As Single = 0
            Dim dec As Single = 0
            If _collisionState.Below Then 'Movin' on the ground, movin' on the ground...
                acc = cPlayerAcceleration : dec = cPlayerDecceleration
            ElseIf Not _collisionState.Below Then 'Movement in the air
                acc = cPlayerAirAcceleration : dec = cPlayerAirDecceleration
            End If

            'Horizontal Controls
            If movvec < 0 And Not _collisionState.Left Then 'Move the character to the left
                AccFlip = True
                If Not Velocity.X < cHorizontalTerminalVelocity * movvec Then Velocity.X += If(PlayerState = PlayerStatus.Skid, dec, acc) * movvec * mp
            ElseIf movvec > 0 And Not _collisionState.Right Then 'Move the character to the right
                AccFlip = False
                If Not Velocity.X > cHorizontalTerminalVelocity * movvec Then Velocity.X += If(PlayerState = PlayerStatus.Skid, dec, acc) * movvec * mp
            Else 'Deccelerate & stop(if a certain minimal threshold is reached) the charcter
                If Velocity.X > 0 Then
                    If Velocity.X > dec And Velocity.X - dec * mp > 0 Then Velocity.X -= dec * mp Else Velocity.X = 0
                ElseIf Velocity.X < 0 Then
                    If Velocity.X < -dec And Velocity.X + dec * mp < 0 Then Velocity.X += dec * mp Else Velocity.X = 0
                End If
            End If

            Velocity.Y += Gravity * Time.DeltaTime

            _mover.Move(Velocity * Time.DeltaTime, _boxCollider, _collisionState)

            'Renderer.FlipX = AccFlip

            If _collisionState.Below Then Velocity.Y = Math.Min(0, Velocity.Y)
            If _collisionState.Above Then Velocity.Y = Math.Max(0, Velocity.Y)
        End Sub
        Public Enum PlayerStatus
            Idle = 0 'Static
            Jumping = 1 'Animation(2 Frames, Oneshot)
            ExJumping = 2 'Animation(6 Frames, Loop)
            Falling = 3 'Animation(3 Frames, Loop)
            Celebrating = 4
            Run = 5
            Skid = 6
            WallClinging = 7
            JumpingFromWall = 8
            Hit = 9
            Landed = 10
            Sleeping = 11
            Pushing = 12
        End Enum
    End Class
End Namespace