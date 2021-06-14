Imports System
Imports System.Collections.Generic
Imports Nez.Textures
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
        Public Shared HorizontalTerminalVelocity As Single = 250 'Horizontal terminal velocity
        Public Shared VerticalTerminalVelocity As Single = 900 'Horizontal terminal velocity
        Public Shared Gravity As Single = 1000
        Public Shared SpringVelocity As Single = 800
        Public Shared BlähVelocity As Single = 35
        Public Shared JumpHeight As Single = 580

        'Spieler Flags
        Private Velocity As Vector2
        Private Spawn As Vector2
        Private AccFlip As Boolean
        Private PlayerState As PlayerStatus = PlayerStatus.Idle
        Private JumpState As Boolean = False
        Private DeathPlain As Integer
        Private LevelScore As Integer = 0

        'SFX flags
        Private blähSFXCounter As Single
        Private Shared PlayedStart As Boolean = False

        'Objects
        Private SpringCollider As New List(Of RectangleF)
        Private NextID As Integer

        'Textures
        Private texIdle As Sprite
        Private texJmp As Sprite
        Private texBläh As Sprite

        'Components
        Public Map As TmxMap
        Public Collider As BoxCollider
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
                If element.Type = "pl_spawn" Then Spawn = New Vector2(element.X - 30, element.Y - 130)
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

        Private Property Enable As Boolean Implements IUpdatable.Enabled
            Get
                Return Enabled
            End Get
            Set(value As Boolean)
                Enabled = value
            End Set
        End Property
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

        Public Sub Die()
            If Not Enabled Then Return

            Time.TimeScale = 0
            Enabled = False
            LevelScore = 0
            GameScene.SFX.PlayCue("death")

            Dim newsc As New GameScene(GameScene.Current)
            Core.Schedule(0.5, Sub()
                                   Core.Scene = newsc
                                   Time.TimeScale = 1
                               End Sub)
        End Sub

        Public Sub FinishStage()
            If Not Enabled Then Return

            Time.TimeScale = 0
            GameScene.Score += LevelScore
            Enabled = False
            GameScene.SFX.PlayCue("finish")

            Core.Schedule(0.5, Sub() Core.StartSceneTransition(New TransformTransition(TransformTransition.TransformTransitionType.SlideRight) With {.OnScreenObscured = Sub()
                                                                                                                                                                             Core.Scene = New GameScene(NextID)
                                                                                                                                                                             Time.TimeScale = 1
                                                                                                                                                                         End Sub, .TransitionEaseType = Tweens.EaseType.Linear, .Duration = 2}))

        End Sub

        Public Enum PlayerStatus
            Idle = 0 'Static
            Jumping = 1 'Animation(2 Frames, Oneshot)
            Bläh = 2
        End Enum
    End Class
End Namespace