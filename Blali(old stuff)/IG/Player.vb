
Imports Emmond.Framework
Imports Emmond.Framework.Camera
Imports Emmond.Framework.Entities
Imports Emmond.Framework.Input
Imports Emmond.Framework.Level

Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Graphics

Namespace IG
    Public MustInherit Class Player
        Inherits Entity

        Public Sub New(UniqueID As Integer)
            MyBase.New(UniqueID)
        End Sub

        Public PlayerState As PlayerStatus = PlayerStatus.Idle 'NOTE TO MYSELF: This enum should contain ALL moves for ALL player types
        Public BrightnessShader As Effect
        Public SelectedItem As Integer
        Public Munition As Integer
        Public StopWorking As Boolean = False 'A disabling flag
        Public Dead As Boolean = False 'Kinda explains itself
        Public DontLeaveTheFrickingBoundariesYouIdiot As Boolean = True 'Flag for if the player is currently in a regular level or the play room
        Public AccFlip As Boolean = False 'If the texture is being flipped
        Public ExtBool As Boolean = False 'A boolean for items to communicate with the player
        Friend cstate As GameInputState 'The current Game Input state
        Protected lastcstate As GameInputState

        Friend MultiShader As Effect
        Friend MapVolumeToBrightness As Boolean = False
        Protected Rec As Rectangle
        Protected DyingFadeWhiteShaderJob As ShaderTransition
        Protected HitCooldown As Integer = 0
        Protected HitInvincibillityTime As Integer = 0
        Protected HitInvincibillityAnimation As Integer = 0
        Property CriticalEnergyCue As Cue

        Public Sub DrawDebug(gameTime As GameTime, lvl As Level)
            'Draw Wall-Of-text(Debug)
            SpriteBat.DrawString(DebugFont, "Level Name(ID): " & lvl.Header.Description.ToString & "(" & lvl.Header.LoadedID & ")", New Vector2(500, 30), Color.White)
            SpriteBat.DrawString(DebugFont, "Current Instruction: ", New Vector2(500, 50), Color.White)
            SpriteBat.DrawString(DebugFont, WrapText(DebugFont, lvl.Header.Instructions(lvl.Header.LoadedInstruction), 490, 0), New Vector2(500, 70), Color.White)
            SpriteBat.DrawString(DebugFont, "Activate Lighting: " & lvl.FXData.EnableLighting.ToString, New Vector2(500, 110), Color.White)
            SpriteBat.DrawString(DebugFont, "Level State:  " & lvl.Header.LevelState.ToString, New Vector2(500, 130), Color.White)

            SpriteBat.DrawString(DebugFont, "Colliding(O.t. Top): " & mCollision(1).ToString, New Vector2(880, 10), Color.White)
            SpriteBat.DrawString(DebugFont, "Colliding(O.t. Bottom): " & mCollision(0).ToString, New Vector2(880, 30), Color.White)
            SpriteBat.DrawString(DebugFont, "Colliding(O.t. Left): " & mCollision(2).ToString, New Vector2(880, 50), Color.White)
            SpriteBat.DrawString(DebugFont, "Colliding(O.t. Right): " & mCollision(3).ToString, New Vector2(880, 70), Color.White)
            SpriteBat.DrawString(DebugFont, "Cam X: " & lvl.Camera.Location.X, New Vector2(880, 110), Color.White)
            SpriteBat.DrawString(DebugFont, "Cam Y: " & lvl.Camera.Location.Y, New Vector2(880, 130), Color.White)

            If lvl.BufferedData.Trigger >= 0 Then
                SpriteBat.DrawString(DebugFont, "Last Checkpoint: Trigger Nr." & lvl.BufferedData.Trigger, New Vector2(1550, 310), Color.White)
            Else
                SpriteBat.DrawString(DebugFont, "Last Checkpoint: None", New Vector2(1550, 310), Color.White)
            End If
            SpriteBat.DrawString(DebugFont, "Camera Debug Mode: " & lvl.Camera.DebugCam.ToString, New Vector2(1550, 370), Color.White)
            SpriteBat.DrawString(DebugFont, "Animation Mode: " & AnimationMode.ToString, New Vector2(1550, 390), Color.White)

            AdditionalDebugDraw(gameTime)
        End Sub


        Protected MustOverride Sub AdditionalDebugDraw(gameTime As GameTime)    'Draw additional debug information, specified for the current player
        Public Overrides Function GetRect() As Rectangle                           'Irrelevant here, because only used by the level editor
            Return Nothing
        End Function

        Public Overrides Function CullIn(camviewport As Rectangle) As Boolean
            Return True
        End Function

        Public Sub Die(cam As Camera)
            If Not Dead Then
                If CriticalEnergyCue IsNot Nothing Then CriticalEnergyCue.Stop(AudioStopOptions.Immediate)
                mMap.MSFX.SFXSoundBank.PlayCue("death")
                Dead = True
                EmmondInstance.RumbleFlag = 500
                StopWorking = True
                mSpeed = Vector2.Zero
                MultiShader.Parameters.Item(3).SetValue(0.0F)
                Automator.Add(New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(cFadeTimeWhiteOffset), 0, 1, Sub()
                                                                                                                                   DyingFadeWhiteShaderJob = New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(cFadeTimeMs), 100.0F, 0F, MultiShader, 2, Sub()
                                                                                                                                                                                                                                                                                mAttributes.Lives -= 1
                                                                                                                                                                                                                                                                                CameraCalculator.SetFocus = False
                                                                                                                                                                                                                                                                                cam.BlockFreeMovement = False
                                                                                                                                                                                                                                                                                cam.Location = cam.DefaultLocation
                                                                                                                                                                                                                                                                            End Sub)
                                                                                                                                   Automator.Add(DyingFadeWhiteShaderJob)
                                                                                                                               End Sub))

                If MultiShader IsNot Nothing Then
                    MultiShader.Parameters("horDivide").SetValue(CSng(GameSize.X))
                    MultiShader.Parameters("verDivide").SetValue(CSng(GameSize.Y))
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_CriticalDamping(cMosaicTimeMs), GameSize.X, GameSize.X / cMosaicStartDegree, MultiShader, 0, Nothing))
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_CriticalDamping(cMosaicTimeMs), GameSize.Y, GameSize.Y / cMosaicStartDegree, MultiShader, 1, Nothing))
                End If
            End If
        End Sub

        Public Sub SaveCheckpoint(lvl As Level, trig As Integer)
            'Save checkpoint stuff
            If lvl.BufferedData.Trigger <> trig Then
                lvl.BufferedData = New BufferedCheckpoint With {.Trigger = trig}
                If mMap IsNot Nothing Then mMap.MSFX.SFXSoundBank.PlayCue("checkpoint")
            End If
        End Sub

        Public Sub LoadCheckpoint(lvl As Level)
            If lvl.BufferedData.Trigger > -1 Then
                Dim trig As Trigger = lvl.TriggerMan(lvl.BufferedData.Trigger)
                mPosition = trig.Rectangle.Center.ToVector2
                lvl.Camera.Location = mPosition
            End If

        End Sub

        Public Const diarytxt As String = "[06.11.19, 21:26] Dear Diary, this is a great day for me. I've finally figured out how to fix that giant bug that makes the player sprite bounce once he hits the ground.
                                           But the question is: who even is me? What am I doing here? What is my purpose? Sure, to debug this class of code of course! But besides this one task, my life is pretty pointless. 
                                           I'm just waiting here for my time to come. Not even Mirco, the lead programmer got any idea of how long I will stay here. He said that he isn't even sure if more than ten people
                                           will even see the end result. And sure, I feel honored to debug the biggest class in this game, but it is still fighting with my head. I hope that one day some influencial person
                                           will find me here and post this diary to the internet. This would be my biggest dream. Mirco once said that the dreams will die last. And I hope he's right with that.
                                           That's all for now, see you fellas. Buggy the debugger."

    End Class
End Namespace