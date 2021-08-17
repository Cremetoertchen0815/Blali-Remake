Imports System.Collections.Generic
Imports Emmond.Framework
Imports Emmond.Framework.Entities
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Graphics.Animation
Imports Emmond.Framework.Input
Imports Emmond.Framework.Level
Imports Emmond.Framework.Physics
Imports Emmond.Framework.Tweening
Imports Emmond.UI.Ingame
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Namespace IG.Entities
    Public Class Blali
        Inherits Player

        Public Sub New(UniqueID As Integer)
            MyBase.New(UniqueID)
        End Sub

        'Moves & Combat
        Public Aimbot As Raycast2D
        Public Armbot As Raycast2D
        Dim CanArm As Boolean = False
        Dim ArmRect As Rectangle
        Dim FinishLvlNottin As Boolean
        Dim IsShoot As Boolean
        Dim UsesAimmode As Boolean = False
        Dim DisplayAimmode As Boolean = False
        Dim ang As Integer
        Dim guncounter As Integer

        'Debug flags & easter eggs
        Public Const maxgraphnum As Single = 150 'Maximum velocity graph point list count(debug)
        Public lvlcl As LevelClass 'Reference to its containing level calls(for debug calls)
        Dim ddlist As List(Of Vector2) 'Mysterious stuff
        Dim PlayerPlacement As Integer = 0 'Whether the player is in free position editing mode(debug) {0 = None, 1 = Mouse, 2 = Controller}

        'Movement Flags
        Dim cPlayerDecceleration As Double = 1.1 'Decceleration speed on the ground °
        Dim cPlayerAcceleration As Double = 0.65 'Acceleration speed on the ground °
        Dim cPlayerAirAcceleration As Double = cPlayerAcceleration 'Acceleration speed in mid-air °
        Dim cPlayerAirDecceleration As Double = cPlayerDecceleration 'Decceleration speed in mid-air °
        Dim cHorizontalTerminalVelocity As Single = 4.5 'Horizontal terminal velocity °
        Dim cGravity As Double = 0.75 'Gravitational force °
        Dim cPlayerJumpHeight As Integer = 16 'Jump height °
        Dim cPlayerBlaeh As Single = -0.9 'Jump height °

        Dim lastkstate As KeyboardState
        Dim lastccstate As GamePadState
        Dim VecFlip As Boolean = False 'Helping flag for skid detection
        Dim IsJump As Boolean = False
        Dim LastJumpPosition As Vector2 'Describes the player location before jumping
        Dim JumpLength As Double = 0 'Long the current jump button has already been pressed
        Dim DisableLeft As Boolean = False 'You can't go left
        Dim DisableRight As Boolean = False 'You can't go right
        Dim DisableJump As Boolean = False 'You can't jump(Stage A)
        Dim lasten As Single 'Contains the enegy level from last frame

        'Textures
        Dim Sprites As TextureAtlas
        Dim Dd As Texture2D 'SCROLL THE CAMERA DOWN YOU IDIOT!

        Public Overrides Sub Initialize()
            mPosition = Level.GetMapTilePosition(mSpawn, New Vector2(0, 0)) + New Vector2(0, cPlayerVerticalSpawnOffset)
            mAABB.halfSize = New Vector2(28, 70)
            mAABBOffset = New Vector2(0, 7)
            mAABB.center = mPosition + mAABBOffset

            If mAttributes Is Nothing Then mAttributes = New EntityAttributes(0)
            mAttributes.ID = 5
            mAttributes.ShootDamage = 0
            mAttributes.BulletColor = Color.Lime
            mAttributes.Health = 0
            mAttributes.Energy = 100
            lasten = mAttributes.Energy

            'Inventory & Backpack
            SelectedItem = 0
            If Not Dead And DontLeaveTheFrickingBoundariesYouIdiot Then SaveCheckpoint(lvlcl.lvl, -1)
            'Gun
            Aimbot = New Raycast2D
            Armbot = New Raycast2D
            UsesAimmode = SettingsMan.ValueBool(87) Or True

            Gunn = New GunSimple
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)

            'Check for innvincibillity frames
            If Not (HitInvincibillityTime > 0 AndAlso HitInvincibillityAnimation < 0) Then

                'Calculate rectangle
                Dim SpriteEff As SpriteEffects = SpriteEffects.None
                If AccFlip Then SpriteEff = SpriteEffects.FlipHorizontally


                'Draw right sprite
                Select Case PlayerState
                    Case PlayerStatus.Idle
                        SpriteBat.Draw(Sprites.Texture, Rec, Sprites(PlayerStatus.Idle), Color.White, 0, New Vector2(0, 0), SpriteEff, 0.5)
                    Case PlayerStatus.Jumping
                        SpriteBat.Draw(Sprites.Texture, Rec, Sprites(PlayerStatus.Jumping), Color.White, 0, New Vector2(0, 0), SpriteEff, 0.5)
                    Case PlayerStatus.ExJumping
                        SpriteBat.Draw(Sprites.Texture, Rec, Sprites(PlayerStatus.ExJumping), Color.White, 0, New Vector2(0, 0), SpriteEff, 0.5)
                End Select
            End If

            '-aiming arrow
            If CanArm And FinishLvlNottin And Not DisplayAimmode Then
                SpriteBat.Draw(ReferencePixel, ArmRect, New Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(ang - 90), New Vector2(0, 0.5), CInt(AccFlip), 1)
            End If


            'Easter egg
            For Each element In ddlist
                SpriteBat.Draw(Dd, New Rectangle(element.X, 1080 - element.Y, cPlayerWidth, cPlayerHeight), New Rectangle(0, 0, Dd.Width, Dd.Height), Color.White, 0, New Vector2(0, 0), SpriteEffects.None, 0.54)
            Next
        End Sub

        Protected Overrides Sub AdditionalDebugDraw(gameTime As GameTime)
            SpriteBat.DrawString(DebugFont, "Grid Location: " & Level.GetMapTileAtPoint(New Vector2(mPosition.X, mPosition.Y), Vector2.Zero).ToString, New Vector2(1140, 10), Color.White)
            SpriteBat.DrawString(DebugFont, "In Jump: " & IsJump.ToString, New Vector2(1140, 30), Color.White)
            SpriteBat.DrawString(DebugFont, PlayerState.ToString, New Vector2(1140, 50), Color.White)
            SpriteBat.DrawString(DebugFont, "Disable Left: " & DisableLeft.ToString, New Vector2(1140, 70), Color.White)
            SpriteBat.DrawString(DebugFont, "Disable Right: " & DisableRight.ToString, New Vector2(1140, 90), Color.White)
            SpriteBat.DrawString(DebugFont, "Disable Jump: " & DisableJump.ToString, New Vector2(1140, 110), Color.White)
        End Sub

        Public Overrides Sub DrawOverlay(gameTime As GameTime, lvl As Level)

            If DisplayAimmode Then
                Aimbot.DrawSegments(Color.Lerp(Color.Magenta, Color.Transparent, 1 - cGunAimingColorLerpAmountB), 1)
            End If

        End Sub

        Public Overrides Sub LoadContent(Optional playerid As Integer = 0)
            'Load Sprites
            Sprites = New TextureAtlas(ContentMan.Load(Of Texture2D)("entity\3\spritesheet"))
            Sprites.Add(PlayerStatus.Idle, New Rectangle(0, 0, 56, 160))
            Sprites.Add(PlayerStatus.Jumping, New Rectangle(56, 0, 56, 160))
            Sprites.Add(PlayerStatus.ExJumping, New Rectangle(112, 0, 56, 160))
            Dd = ContentMan.Load(Of Texture2D)("ge_dieeast")

            ddlist = New List(Of Vector2)

            BrightnessShader = ContentMan.Load(Of Effect)("fx\fx_bright")
            BrightnessShader.Parameters.Item(1).SetValue(0F)

            'Prepare some flags
            EmmondInstance.LastRumble = True
            EmmondInstance.RumbleFlag = 0
        End Sub

        Dim obs As New List(Of BulletSimple)
        Public Overrides Sub Update(gameTime As GameTime, lvl As Level, triggerinfluence As Boolean())
            Dim kstate As KeyboardState = Keyboard.GetState
            Dim ccstate As GamePadState = GamePad.GetState(PlayerIndex.One)
            cstate = InputMan.GetGameInput(PlayerPlacement)
            mMap = lvl

            If PlayerPlacement = 1 Then 'If free position editing mode is activated
                Dim pos As New Vector2(Mouse.GetState.X, Mouse.GetState.Y)
                pos = lvl.Camera.TranslateFromCamera(pos)
                mPosition = New Vector2(pos.X, 1080 - pos.Y)

                If Mouse.GetState.LeftButton Then
                    PlayerPlacement = 0
                    StopWorking = False
                End If
            ElseIf PlayerPlacement = 2 Then
                mPosition += cstate.Movement * gameTime.ElapsedGameTime.TotalMilliseconds * 0.5

                If cstate.Jump Then
                    PlayerPlacement = 0
                    StopWorking = False
                End If
            ElseIf PlayerPlacement = 0 And DebugMode Then 'Activated free position editing mode
                If kstate.IsKeyDown(Keys.F8) Then
                    PlayerPlacement = 1
                    StopWorking = True
                ElseIf ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.Buttons.Back Then
                    PlayerPlacement = 2
                    StopWorking = True
                ElseIf (kstate.IsKeyDown(Keys.F1) And lastkstate.IsKeyUp(Keys.F1)) Or (ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.DPad.Up And Not lastccstate.DPad.Up) Then
                    If lvlcl IsNot Nothing Then lvlcl.FinishStage()
                ElseIf (kstate.IsKeyDown(Keys.F2) And lastkstate.IsKeyUp(Keys.F2)) Or (ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.DPad.Right And Not lastccstate.DPad.Right) Then
                    Die(lvl.Camera)
                ElseIf (kstate.IsKeyDown(Keys.F3) And lastkstate.IsKeyUp(Keys.F3)) Or (ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.DPad.Down And Not lastccstate.DPad.Down) Then

                ElseIf (kstate.IsKeyDown(Keys.F5) And lastkstate.IsKeyUp(Keys.F5)) Or (ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.Buttons.LeftStick And Not lastccstate.Buttons.LeftStick) Then

                ElseIf (kstate.IsKeyDown(Keys.F6) And lastkstate.IsKeyUp(Keys.F6)) Or (ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.Buttons.RightStick And Not lastccstate.Buttons.RightStick) Then
                    If AnimationMode = MovementMode.Regular Then
                        AnimationMode = MovementMode.AnimationWPhysics
                    Else
                        AnimationMode = MovementMode.Regular
                    End If
                ElseIf (kstate.IsKeyDown(Keys.F7) And lastkstate.IsKeyUp(Keys.F7)) Or (ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.Buttons.Start And Not lastccstate.Buttons.Start) Then

                ElseIf (kstate.IsKeyDown(Keys.F10) And lastkstate.IsKeyUp(Keys.F10)) Or (ccstate.Buttons.LeftShoulder And ccstate.Buttons.RightShoulder And ccstate.Buttons.B And Not lastccstate.Buttons.B) Then
                    lvl.Camera.DebugCam = Not lvl.Camera.DebugCam
                End If
            End If

            Dim acc As Boolean = False

            If Not StopWorking Then

                If AnimationMode <> MovementMode.Regular Then cstate = New GameInputState

                'Get the horizontal input vector
                Dim movvec As Double = cstate.Movement.X

                '1st Jumping
                If cstate.Jump And Not IsJump And Not DisableJump And Not cstate.UseItem Then 'Initiate the jump
                    'Start launching phase and initialize flags
                    IsJump = True
                    PlayerState = PlayerStatus.Jumping
                    mMap.MSFX.SFXSoundBank.PlayCue("jump")
                    JumpLength = 0
                    mSpeed.Y = cPlayerJumpHeight
                End If

                If mCollision(0) Then DisableJump = False : IsJump = False

                'Implement vertical terminal velocity
                If mSpeed.Y < cVerticalTerminalVelocity Then mSpeed.Y = cVerticalTerminalVelocity

                Dim mp As Double = gameTime.ElapsedGameTime.TotalSeconds * 60

                'Horizontal Controlls
                If AnimationMode = MovementMode.Regular Then 'Movin' on the ground, movin' on the ground...

                    If movvec < 0 And Not mCollision(2) And Not DisableLeft Then 'Move the character to the left
                        AccFlip = True
                        acc = True
                        If Not mSpeed.X < -cHorizontalTerminalVelocity Then mSpeed.X += cPlayerAirAcceleration * movvec * mp
                    ElseIf movvec > 0 And Not mCollision(3) And Not DisableRight Then 'Move the character to the right
                        AccFlip = False
                        acc = True
                        If Not mSpeed.X > cHorizontalTerminalVelocity Then mSpeed.X += cPlayerAirAcceleration * movvec * mp
                    ElseIf AnimationMode <> MovementMode.AnimationWoPhysics Then 'Deccelerate & stop(if a certain minimal threshold is reached) the charcter
                        If mSpeed.X > 0 Then
                            If mSpeed.X > cPlayerAirDecceleration Then mSpeed.X -= cPlayerAirDecceleration * gameTime.ElapsedGameTime.TotalSeconds * 60 Else mSpeed.X = 0
                        ElseIf mSpeed.X < 0 Then
                            If mSpeed.X < -cPlayerAirDecceleration Then mSpeed.X += cPlayerAirDecceleration * gameTime.ElapsedGameTime.TotalSeconds * 60 Else mSpeed.X = 0
                        End If
                    End If

                End If

                'Implentation of gravity
                If Not mCollision(0) And Not mCollision(7) And Not mCollision(8) And PlayerState <> PlayerStatus.WallClinging Then mSpeed -= New Vector2(0, cGravity * mp)


                'Respawn if player falls out of the map
                If mPosition.Y <= -5 * cTileSize And AnimationMode = MovementMode.Regular Then
                    ddlist.Add(New Vector2(mPosition.X, mPosition.Y))
                    Die(lvl.Camera)
                End If

            End If

            'Check for hits
            Dim playerrect As Rectangle = mAABB.GetRectangle
            Dim collisionrect As Rectangle
            Dim mov As Integer
            For Each enemy In lvl.Entities
                If enemy.mAttributes.UniqueID > 0 Then
                    'Check enemy collision
                    collisionrect = enemy.mAABB.GetRectangle
                    If playerrect.Intersects(collisionrect) Then
                        'Calculate player bounce when hit
                        mov = cPlayerHitMobTouchBounce
                        If collisionrect.Center.X < playerrect.Center.X Then mov = -cPlayerHitMobTouchBounce
                        'Hit player / shield
                        If enemy.mAttributes.TouchDamage > 0 Then Die(lvl.Camera)
                    End If
                End If
            Next

            If Not DontLeaveTheFrickingBoundariesYouIdiot Then mAttributes.Energy = 100

            'Implement the trigger influence
            If triggerinfluence(0) And mSpeed.Y > 0 Then mSpeed.Y = 0
            If triggerinfluence(1) And mSpeed.Y < 0 Then mSpeed.Y = 0
            If triggerinfluence(2) And mSpeed.X < 0 Then mSpeed.X = 0
            If triggerinfluence(3) And mSpeed.X > 0 Then mSpeed.X = 0

            'Run physics and generate final drawing rectangle
            Dim recoff As Vector2 = Vector2.Zero
            If AnimationMode = MovementMode.Regular And ((mAABB.center.X - mAABB.halfSize.X < 0 And mSpeed.X + mAdditionalImpulse.X < 0) Or (mAABB.center.X + mAABB.halfSize.X > (lvl.Map.Size.X - 1) * cTileSize And mSpeed.X + mAdditionalImpulse.X > 0)) And DontLeaveTheFrickingBoundariesYouIdiot Then mSpeed.X = 0 : mAdditionalImpulse = Vector2.Zero
            If Not StopWorking Or lvl.Header.LevelState <> FinishedMode.Nottin Then
                Select Case PlayerState
                    Case PlayerStatus.Idle
                        mAABB.halfSize = New Vector2(28, 70)
                        mAABBOffset = New Vector2(0, 7)
                    Case PlayerStatus.Jumping
                        mAABB.halfSize = New Vector2(28, 70)
                        mAABBOffset = New Vector2(0, 7)
                    Case PlayerStatus.ExJumping
                        mAABB.halfSize = New Vector2(28, 50)
                        mAABBOffset = New Vector2(0, 0)
                        recoff.Y = -5
                End Select
                UpdatePhysics(gameTime)
            Else
                mAABB.center = mPosition + mAABBOffset
            End If
            Rec = New Rectangle(Math.Round(mPosition.X - 28 + recoff.X), Math.Round(1080 - mPosition.Y - 90 + recoff.Y), 56, 160)
            If Not DontLeaveTheFrickingBoundariesYouIdiot Then Rec.X -= cTileSize

            'Texture adaptation
            PlayerState = PlayerStatus.Idle
            If Not mCollision(0) And IsJump Then PlayerState = PlayerStatus.Jumping

            'Blähen
            If cstate.UseItem And Not ExtBool Then
                mSpeed.Y = cPlayerBlaeh
                PlayerState = PlayerStatus.ExJumping
            End If

            'Disable Blähen when hit a spring
            If Not cstate.UseItem And ExtBool Then ExtBool = False

            'Respawn
            If DyingFadeWhiteShaderJob IsNot Nothing AndAlso DyingFadeWhiteShaderJob.TransitionStater = TransitionState.Done And Dead Then
                'Prepare respawn
                Initialize()
                LoadCheckpoint(lvl)
                Dead = False
                'Fade in
                Automator.Remove(DyingFadeWhiteShaderJob)
                Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_CriticalDamping(cFadeTimeWhiteOffset), 0F, 100.0F, MultiShader, 2, Nothing))
                'Initialize Mosaic effect
                If MultiShader IsNot Nothing Then
                    MultiShader.Parameters("horDivide").SetValue(CSng(Math.Floor(GameSize.X / cMosaicStartDegree)))
                    MultiShader.Parameters("verDivide").SetValue(CSng(Math.Floor(GameSize.Y / cMosaicStartDegree)))
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(cMosaicTimeMs), GameSize.X / cMosaicStartDegree, GameSize.X, MultiShader, 0, Nothing))
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(cMosaicTimeMs), GameSize.Y / cMosaicStartDegree, GameSize.Y, MultiShader, 1, Nothing))
                End If
                'Reset lighting
                Lighting.AmbientColor = lvl.FXData.AmbientColor
                DyingFadeWhiteShaderJob = Nothing
                StopWorking = False
            End If

            If MapVolumeToBrightness Then
                Media.MediaPlayer.Volume = (SettingsMan.Value(31) / 100) * (MultiShader.Parameters(2).GetValueSingle / 100)
                AudioEng.GetCategory("Default").SetVolume((SettingsMan.Value(32) / 100) * (MultiShader.Parameters(2).GetValueSingle / 100))
            End If

            ''Implement rumble if the player lands on the ground
            'If mCollision(0) Then
            '    If Not mOldCollision(0) And PlayerState = PlayerStatus.Falling And Not IgnoreFall And Not mOldCollision(7) And Not mOldCollision(8) Then
            '        If EmmondInstance.RumbleFlag < MaxVerticalSpeed / cPlayerJumpRumbleDivider * cPlayerRumbleFactor Then EmmondInstance.RumbleFlag = MaxVerticalSpeed / cPlayerJumpRumbleDivider * cPlayerRumbleFactor
            '        MaxVerticalSpeed = 0
            '        mMap.MSFX.SFXSoundBank.PlayCue("land")
            '        lvl.CamShaker.TriggerShaker(30)
            '        landcounter = 0
            '    ElseIf Not mOldCollision(0) And MyBase.PlayerState = Global.Emmond.IG.PlayerStatus.Falling And IgnoreFall And EmmondInstance.RumbleFlag < (MaxVerticalSpeed / cPlayerJumpRumbleDivider) * cPlayerRumbleFactor Then
            '        IgnoreFall = False
            '    End If
            'Else
            '    If mSpeed.Y < -MaxVerticalSpeed Then MaxVerticalSpeed = -mSpeed.Y
            'End If

            If cstate.Shoot And Munition > 0 Then
                Gunn.SpawnNewBullet(mPosition + New Vector2(0, 20), If(AccFlip, 90, 270), mSpeed, Color.White, 5, Munition)
            End If


            'Check for hits
            For i As Integer = 0 To lvl.Entities.Count - 1
                Dim enemy As Entity = lvl.Entities(i)
                If i > 0 Then
                    'Check enemy collision
                    collisionrect = enemy.mAABB.GetRectangle

                    'Check enemy bullet collision
                    For Each bullet In Gunn.Bullets
                        If collisionrect.Contains(bullet.Location.ToPoint) Then
                            enemy.mObsolete = True
                            obs.Add(bullet)
                        End If
                    Next
                    For Each obsoletbullet In obs
                        Gunn.Bullets.Remove(obsoletbullet)
                    Next
                    obs.Clear()
                End If
            Next

            If DontLeaveTheFrickingBoundariesYouIdiot Then BrightnessShader.Parameters.Item(0).SetValue(MultiShader.Parameters.Item(2).GetValueSingle)

            FinishLvlNottin = lvl.Header.LevelState = FinishedMode.Nottin

            Gunn.Update(gameTime, lvl)

            lastcstate = cstate
            lastkstate = kstate
            lastccstate = ccstate
        End Sub
    End Class
End Namespace