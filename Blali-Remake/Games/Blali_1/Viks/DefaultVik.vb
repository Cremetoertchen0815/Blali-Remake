Imports System
Imports System.Collections.Generic
Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Viks
    Public Class DefaultVik
        Inherits IVik

        'Controls
        Private BtnMove As VirtualJoystick
        Private BtnJump As VirtualButton
        Private BtnBläh As VirtualButton

        'Constants
        Private Const HorizontalTerminalVelocity As Single = 250 'Horizontal terminal velocity
        Private Const VerticalTerminalVelocity As Single = 900 'Horizontal terminal velocity
        Private Const Gravity As Single = 1000
        Private Const SpringVelocity As Single = 800
        Private Const BlähVelocity As Single = 35
        Private Const JumpHeight As Single = 580

        'Spieler Flags
        Private Velocity As Vector2
        Private Spawn As Vector2
        Private AccFlip As Boolean
        Private PlayerState As PlayerStatus = PlayerStatus.Idle
        Private JumpState As Boolean = False
        Protected DeathPlain As Integer

        'SFX flags
        Private blähSFXCounter As Single
        Private Shared PlayedStart As Boolean = False

        'Objects
        Private SpringCollider As New List(Of RectangleF)

        'Textures
        Private texIdle As Sprite
        Private texJmp As Sprite
        Private texBläh As Sprite

        'Components
        Private FinishCollider As RectangleF
        Private _spriteRenderer As Sprites.SpriteRenderer
        Private _mover As TiledMapMover
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


            'Load textures
            texIdle = New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/spr_1"))
            texJmp = New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/spr_2"))
            texBläh = New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/spr_3"))

            'Add components
            Entity.Scale = New Vector2(0.9)
            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(3, 0, 54, 130)))
            _mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            _spriteRenderer = Entity.AddComponent(New Sprites.SpriteRenderer(texIdle) With {.LocalOffset = New Vector2(30, 66) * Entity.Scale})


            'Load object data
            DeathPlain = CInt(Map.Properties("death_plain"))
            For Each element In Map.GetObjectGroup("Objects").Objects
                If element.Type = "spring" Then SpringCollider.Add(New RectangleF(element.X, element.Y, element.Width, element.Height))
                If element.Type = "player" Then Spawn = New Vector2(element.X - 30, element.Y - 130)
                If element.Type = "finish" Then FinishCollider = New RectangleF(element.X, element.Y, element.Width, element.Height) : NextID = CInt(element.Properties("followup_ID"))
            Next
            Entity.LocalPosition = Spawn
            GameObject.ScoreIncrease = Sub(x) LevelScore += x

            'Map controls
            BtnMove = New VirtualJoystick(True, New VirtualJoystick.GamePadLeftStick, New VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D, Keys.W, Keys.S))
            BtnJump = New VirtualButton(500, New VirtualButton.GamePadButton(0, Buttons.A), New VirtualButton.KeyboardKey(Keys.Space)) With {.BufferTime = 0}
            BtnBläh = New VirtualButton(500, New VirtualButton.GamePadButton(0, Buttons.X), New VirtualButton.KeyboardKey(Keys.LeftShift)) With {.BufferTime = 0}
        End Sub

        Public Overrides Sub OnAddedToEntity()
            'Play start sound
            If Not PlayedStart Then
                GameScene.SFX.PlayCue("start")
                PlayedStart = True
            End If
        End Sub
        Public Overrides Sub Update()

            '1st Jumping
            Dim NoDrop As Boolean = BtnMove.Value.Y >= -0.8
            Dim movvec As Single = BtnMove.Value.X

            If JumpState And _collisionState.Below Then
                'Stop jump
                JumpState = False
                GameScene.SFX.PlayCue("land")
            ElseIf BtnJump.IsPressed And NoDrop And _collisionState.Below Then 'Initiate the jump
                'Start launching phase and initialize flags
                JumpState = True
                PlayerState = PlayerStatus.Jumping
                Velocity.Y = -JumpHeight
                'DisableJumpA = True
            End If

            'Adapt sprite and collider to default value
            Collider.Height = 130
            _spriteRenderer.Sprite = If(JumpState, texJmp, texIdle)

            'Preparing Horizontal Controls
            Dim mp As Single = Time.DeltaTime * 6000
            Velocity.X = BtnMove.Value.X * HorizontalTerminalVelocity

            Velocity.Y += Gravity * Time.DeltaTime

            'Blähing
            If BtnBläh.IsDown Then
                Velocity.Y = BlähVelocity
                PlayerState = PlayerStatus.Bläh
                _spriteRenderer.Sprite = texBläh
                Collider.Height = 100
                blähSFXCounter += Time.DeltaTime * 1000.0F
                If blähSFXCounter > 50 Then GameScene.SFX.PlayCue("blaeh") : blähSFXCounter = 0
            End If

            'Move player
            _mover.Move(Velocity * Time.DeltaTime, Collider, _collisionState)


            'Clamp player position to map
            Entity.LocalPosition = New Vector2(Mathf.Clamp(Entity.LocalPosition.X, 0, Map.Width * 16 - 60), Mathf.Clamp(Entity.LocalPosition.Y, 0, Map.Height * 16 - 130))

            'Clamp vertical player velocity
            Velocity.Y = Mathf.Clamp(Velocity.Y, -VerticalTerminalVelocity, VerticalTerminalVelocity)

            'Collide with spring
            For Each element In SpringCollider
                Dim overlapX As Single
                Dim overlapY As Single

                If Collider.Bounds.CollisionCheck(element, overlapX, overlapY) AndAlso (overlapX <> 0 Xor overlapY <> 0) AndAlso Velocity.Y > 0 Then
                    Velocity.Y = -SpringVelocity
                    GameScene.SFX.PlayCue("spring")
                    Exit For
                End If
            Next

            'Implement death plain
            If Collider.Bounds.Bottom > DeathPlain Then Die()

            'Implement finishing the stage
            If Collider.Bounds.CollisionCheck(FinishCollider, 0, 0) Then FinishStage()

            'Flip sprite
            If Velocity.X <> 0 Then AccFlip = Velocity.X < 0
            _spriteRenderer.FlipX = AccFlip

            If _collisionState.Below Then Velocity.Y = Math.Min(0, Velocity.Y)
            If _collisionState.Above Then Velocity.Y = Math.Max(0, Velocity.Y)
        End Sub

        Public Enum PlayerStatus
            Idle = 0 'Static
            Jumping = 1 'Animation(2 Frames, Oneshot)
            Bläh = 2
        End Enum
    End Class
End Namespace