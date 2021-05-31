Imports System
Imports System.Collections.Generic
Imports Nez.Tiled

Namespace Games.Blali_1
    Public Class PlayerMover
        Inherits Component
        Implements IUpdatable

        'Controls
        Private BtnMove As VirtualJoystick
        Private BtnJump As VirtualButton
        Private BtnBläh As VirtualButton

        'Constants
        Public Shared HorizontalTerminalVelocity As Single = 550 'Horizontal terminal velocity
        Public Shared AirAcceleration As Single = 0.35 'Acceleration speed in mid-air
        Public Shared AirDecceleration As Single = 0.35 'Decceleration speed in mid-air
        Public Shared Acceleration As Single = 0.7 'Acceleration speed on the ground
        Public Shared Decceleration As Single = 2.5 'Decceleration speed on the ground
        Public Shared Gravity As Single = 4800
        Public Shared SpringVelocity As Single = 2200
        Public Shared BlähVelocity As Single = 160
        Public Shared JumpHeight As Single = 1600

        'Spieler Flags
        Private Velocity As Vector2
        Private AccFlip As Boolean
        Private PlayerState As PlayerStatus = PlayerStatus.Idle
        Private JumpState As Boolean = False
        Private DeathPlain As Integer

        'Objects
        Private SpringCollider As New List(Of RectangleF)

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
            _boxCollider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 80, 160)))
            _mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Entity.AddComponent(New PrototypeSpriteRenderer(80, 160)).LocalOffset = New Vector2(40, 80)

            DeathPlain = CInt(Map.Properties("death_plain"))

            'Load object data
            For Each element In Map.GetObjectGroup("Objects").Objects
                If element.Type = "spring" Then SpringCollider.Add(New RectangleF(element.X, element.Y, element.Width, element.Height))
            Next

            BtnMove = New VirtualJoystick(True, New VirtualJoystick.GamePadLeftStick, New VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D, Keys.W, Keys.S))
            BtnJump = New VirtualButton(500, New VirtualButton.GamePadButton(0, Buttons.A), New VirtualButton.KeyboardKey(Keys.Space)) With {.BufferTime = 0}
            BtnBläh = New VirtualButton(500, New VirtualButton.GamePadButton(0, Buttons.X), New VirtualButton.KeyboardKey(Keys.LeftShift)) With {.BufferTime = 0}
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

            If JumpState And _collisionState.Below Then
                'Stop the extended jump
                JumpState = False
            ElseIf BtnJump.IsPressed And NoDrop And _collisionState.Below Then 'Initiate the jump
                'Start launching phase and initialize flags
                JumpState = True
                PlayerState = PlayerStatus.Jumping
                Velocity.Y = -JumpHeight
                'DisableJumpA = True
            End If

            'Preparing Horizontal Controls
            Dim mp As Single = Time.DeltaTime * 6000
            Dim acc As Single = 0
            Dim dec As Single = 0
            If _collisionState.Below Then 'Movin' on the ground, movin' on the ground...
                acc = Acceleration : dec = Decceleration
            ElseIf Not _collisionState.Below Then 'Movement in the air
                acc = AirAcceleration : dec = AirDecceleration
            End If

            'Horizontal Controls
            If movvec < 0 And Not _collisionState.Left Then 'Move the character to the left
                AccFlip = True
                If Not Velocity.X < HorizontalTerminalVelocity * movvec Then Velocity.X += acc * movvec * mp
            ElseIf movvec > 0 And Not _collisionState.Right Then 'Move the character to the right
                AccFlip = False
                If Not Velocity.X > HorizontalTerminalVelocity * movvec Then Velocity.X += acc * movvec * mp
            Else 'Deccelerate & stop(if a certain minimal threshold is reached) the charcter
                If Velocity.X > 0 Then
                    If Velocity.X > dec And Velocity.X - dec * mp > 0 Then Velocity.X -= dec * mp Else Velocity.X = 0
                ElseIf Velocity.X < 0 Then
                    If Velocity.X < -dec And Velocity.X + dec * mp < 0 Then Velocity.X += dec * mp Else Velocity.X = 0
                End If
            End If

            Velocity.Y += Gravity * Time.DeltaTime

            'Blähing
            If BtnBläh.IsDown Then Velocity.Y = BlähVelocity : PlayerState = PlayerStatus.Bläh

            'Move player
            _mover.Move(Velocity * Time.DeltaTime, _boxCollider, _collisionState)


            'Collide with spring
            For Each element In SpringCollider
                Dim overlapX As Single
                Dim overlapY As Single

                If _boxCollider.Bounds.CollisionCheck(element, overlapX, overlapY) AndAlso (overlapX <> 0 Xor overlapY <> 0) AndAlso Velocity.Y > 0 Then
                    Velocity.Y = -SpringVelocity
                    Exit For
                End If
            Next


            'Implement death plain
            If _boxCollider.Bounds.Bottom > DeathPlain Then Die()

            'Renderer.FlipX = AccFlip

            If _collisionState.Below Then Velocity.Y = Math.Min(0, Velocity.Y)
            If _collisionState.Above Then Velocity.Y = Math.Max(0, Velocity.Y)
        End Sub

        Public Sub Die()
            Entity.LocalPosition = Vector2.Zero
            Velocity = Vector2.Zero
        End Sub

        Public Enum PlayerStatus
            Idle = 0 'Static
            Jumping = 1 'Animation(2 Frames, Oneshot)
            Bläh = 2
        End Enum
    End Class
End Namespace