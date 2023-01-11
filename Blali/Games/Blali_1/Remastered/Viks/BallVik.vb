Imports Blali.Games.Blali_1.Remastered.Objects
Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Remastered.Viks
    Public Class BallVik
        Inherits IVik

        'Controls
        Private BtnMoveX As VirtualIntegerAxis
        Private BtnMoveY As VirtualIntegerAxis

        'Constants
        Private Const MovingVelocity As Single = 350 'Horizontal terminal velocity

        'Flags
        Private CoinCondition As Integer
        Private Velocity As Vector2
        Private Spawn As Vector2
        Protected DeathPlain As Integer

        'Textures
        Private texIdle As Sprite

        'Components
        Private _spriteRenderer As Sprites.SpriteRenderer
        Private _mover As TiledMapMover
        Private _collisionState As New TiledMapMover.CollisionState()

        Public Sub New(map As TmxMap)
            Me.Map = map
        End Sub

        Public Overrides Sub OnRemovedFromEntity()
            BtnMoveX.Deregister()
            BtnMoveY.Deregister()
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            'Load textures
            texIdle = New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/ball"))

            'Add components
            Entity.Scale = New Vector2(1.8)
            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 8, 8)))
            _mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            _spriteRenderer = Entity.AddComponent(New Sprites.SpriteRenderer(texIdle) With {.LocalOffset = New Vector2(4) * Entity.Scale})


            'Load object data
            DeathPlain = CInt(Map.Properties("death_plain"))
            For Each element In Map.GetObjectGroup("Objects").Objects
                If element.Type = "player" Then Spawn = New Vector2(element.X - 30, element.Y - 130)
                If element.Type = "finish" Then NextID = CInt(element.Properties("followup_ID")) : CoinCondition = CInt(element.Properties("coin_condition"))
            Next
            Entity.LocalPosition = Spawn
            GameObject.ScoreIncrease = Sub(x) LevelScore += x

            'Map controls
            BtnMoveX = New VirtualIntegerAxis(New VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.A, Keys.D), New VirtualAxis.GamePadLeftStickX, New VirtualAxis.GamePadDpadLeftRight)
            BtnMoveY = New VirtualIntegerAxis(New VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.W, Keys.S), New VirtualAxis.GamePadLeftStickY, New VirtualAxis.GamePadDpadUpDown)

            'Set up camera
            Dim camera As Camera = Entity.Scene.Camera
            camera.Position = New Vector2(CInt(Map.Properties("camX")) * Map.TileWidth, CInt(Map.Properties("camY")) * Map.TileHeight)
            camera.Zoom = 0.1
        End Sub

        Public Overrides Sub OnAddedToEntity()
            'Play start sound
            If Not PlayerStartSFX Then
                GameScene.SFX.PlayCue("start")
                PlayerStartSFX = True
            End If
        End Sub
        Public Overrides Sub Update()

            'Move player
            If BtnMoveX.Value <> 0 Then Velocity = New Vector2(BtnMoveX.Value * MovingVelocity, 0)
            If BtnMoveY.Value <> 0 Then Velocity = New Vector2(0, BtnMoveY.Value * MovingVelocity)
            _mover.Move(Velocity * Time.DeltaTime, Collider, _collisionState)


            'Clamp player position to map
            Entity.LocalPosition = New Vector2(Nez.Mathf.Clamp(Entity.LocalPosition.X, 0, Map.Width * 16 - 60), Nez.Mathf.Clamp(Entity.LocalPosition.Y, 0, Map.Height * 16 - 130))

            'Implement death plain
            If Collider.Bounds.Bottom > DeathPlain Then Die()

            'Implement finishing the stage
            If YellowCoin.CollectedCount >= CoinCondition Then FinishStage()


            If (_collisionState.Left Or _collisionState.Right And Velocity.X <> 0) Or (_collisionState.Above Or _collisionState.Below And Velocity.Y <> 0) Then Velocity = Vector2.Zero
        End Sub

        Friend Overrides Function CheckBulletCollision(co As Collider) As Boolean
            Return False
        End Function

        Public Enum PlayerStatus
            Idle = 0 'Static
            Jumping = 1 'Animation(2 Frames, Oneshot)
            Bläh = 2
        End Enum
    End Class
End Namespace