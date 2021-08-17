''' <summary>
''' Contains all global constants of the game and methods for randomizing them
''' </summary>
Public Class Constants

    'Global
    Public Shared cCurrentVersion As String = "Version preAlpha v0.0.5.8"
    Public Shared cActivateDebugMode As Boolean = False
    Public Shared cDeativateDebugMode As Boolean = False
    Public Shared cIgnoreBadSavefile As Boolean = True '°
    Public Shared cMinMaxResetSequence As Single = 5000
    Public Shared cTileSize As Integer = 16 'Describes the universial size of a tile in pixels
    Public Shared cBGLayerSplit As Single = 0.5

    'Data
    Public Shared cDataBackpackOffset As Integer = 90

    'Physics Engine
    Public Shared cOneWayPlatformThreshold As Integer = 2
    Public Shared cSlopeTileYOffsetUp As Single = 5
    Public Shared cSlopeTileYOffsetDown As Single = 1
    Public Shared cSlopeTileYOffsetDownwards As Single = 1
    Public Shared cHorizontalTerminalVelocity As Single = 7.6 'Horizontal terminal velocity °
    Public Shared cVerticalTerminalVelocity As Integer = -25 'Vertical terminal velocity °
    Public Shared cGravity As Double = 1.2 'Gravitational force °

    'Controlling/Movement
    Public Shared cPlayerDecceleration As Double = 1.35 'Decceleration speed on the ground °
    Public Shared cPlayerAirAcceleration As Double = 0.6 'Acceleration speed in mid-air °
    Public Shared cPlayerAirDecceleration As Double = 0.27 'Decceleration speed in mid-air °
    Public Shared cPlayerAcceleration As Double = 0.4 'Acceleration speed on the ground °
    Public Shared cPlayerSkidVector As Double = 0.5 'Vector at which skidding is enabled °
    Public Shared cPlayerJumpHeight As Integer = 22 'Jump height °
    Public Shared cPlayerJumpDoubleEnergyCost As Single = 1.5 'The maximum hold-down steps for the jump button
    Public Shared cPlayerJumpDoubleRatio As Single = 0.9 'The ratio between basic jump height and additional jump height °

    'Player Misc
    Public Shared cPlayerWidth As Integer = 60 '°
    Public Shared cPlayerHeight As Integer = 176 '°
    Public Shared cPlayerHorizontalTextureOffset As Integer = -14 '°
    Public Shared cPlayerVerticalTextureOffset As Integer = -168 '°
    Public Shared cPlayerTextureWidth As Integer = 88 '°
    Public Shared cPlayerTextureHeight As Integer = 176 '°
    Public Shared cPlayerVerticalSpawnOffset As Integer = 0 'With how much vertical offset the player is spawned
    Public Shared cPlayerJumpRumbleDivider As Integer = 40 ' The divider of the landing-rumble time
    Public Shared cPlayerRumbleFactor As Double = 100 ' The factor(of ms) with which the rumble is played
    Public Shared cPlayerLandFreezeTime As Single = 350
    Public Shared cPlayerDyingDepth As Integer = -300 '°
    Public Shared cPlayerWallJumpSlidingSpeed As Single = 2.5 '°
    Public Shared cPlayerWallJumpHorizontalVelocity As Integer = 6 '°
    Public Shared cPlayerWallJumpVerticalVelocity As Integer = 23 '°
    Public Shared cPlayerWallJumpMaxTime As Integer = 1700 'How long the player can slide on the wall before letting go
    Public Shared cPlayerWallJumpDelay As Integer = 1400 'How long the player flies through the air until the 
    Public Shared cPlayerInvincibillityTime As Double = 1500 'The blinking speed of the invincibillity frames °
    Public Shared cPlayerHitMobTouchBounce As Double = 10 'How far the player is being pushed away after touching a mob °
    Public Shared cPlayerCooldownTime As Double = 200 'How long a player cannot move after touching a mob/projectile °
    Public Shared cPlayerInventorySize As Integer = 8
    Public Shared cPlayerBackpackSize As Integer = 27
    Public Shared cPlayerAutoDrainEnergy As Single = 0.5
    Public Shared cPlayerAutoDrainHealth As Single = 5

    'Gun & Arm Properties
    Public Shared cGunAimingSubdivides As Integer = 10 '°
    Public Shared cGunAimingColorLerpAmount As Single = 0.87 '°
    Public Shared cGunAimingColorLerpAmountB As Single = 0.9 '°
    Public Shared cGunAimEnergyCost As Single = 0.5 '°
    Public Shared cGunEnergyCost As Single = 0.5 '°
    Public Shared cGunCooldown As Integer = 150 '°
    Public Shared cGunBulletLength As Integer = 300 '°
    Public Shared cGunBulletWidth As Integer = 3 '°
    Public Shared cGunMaxAngle As Integer = 45 '°
    Public Shared cGunBulletRange As Integer = 1200 '°
    Public Shared cGunBulletTravelSpeed As Integer = 100 '°
    Public Shared cGunBulletVelocityDeadzone As Single = 0.5
    Public Shared cArmLength As Integer = 80
    Public Shared cArmHeight As Integer = 10

    'Power-Ups
    Public Shared cDashSpeed As Integer = 22
    Public Shared cDashLength As Integer = 250
    Public Shared cDashCooldown As Integer = 600
    Public Shared cTJumpFactor As Single = 1.4
    Public Shared cTJumpCooldown As Integer = 150

    'Animation
    Public Shared cPlayerAnimationSpeed As Integer = 30 'Timing of the character animation '°
    Public Shared cPlayerMaxAnimationSpeed As Integer = 260 'Timing of the character animation '°
    Public Shared cPlayerAnimationSpeedCutoff As Double = 6.5 'The horizontal speed at which the animation is switched
    Public Shared cPlayerAnimationInvincibillityBlinkSpeed As Double = 50 'The blinking speed of the invincibillity frames '°
    Public Shared cLevelFadeOutComplete As Integer = 2000
    Public Shared cLevelFadeOutGameOver As Integer = 5000
    Public Shared cResultScreenHighscoreBlinkerSpeed As Integer = 600

    'Type-B Camera
    Public Shared cCameraScrollBarrierHorizontal As Integer = 500
    Public Shared cCameraScrollBarrierVectical As Integer = 420
    Public Shared cCameraAccelerationHorizontal As Single = 0.15
    Public Shared cCameraAccelerationVectical As Single = 0.4
    Public Shared cCameraDeccelerationHorizontal As Double = 0.6
    Public Shared cCameraDeccelerationVectical As Double = 0.4
    Public Shared cCameraVectorAdjHorizontal As Integer = 130
    Public Shared cCameraVectorAdjVertical As Integer = 0
    Public Shared cCameraLockSizeX As Single = 60
    Public Shared cCameraLockSizeY As Single = 110
    Public Shared cCameraMaxSpeedHorizontal As Single = 4.5
    Public Shared cCameraMaxSpeedHorizontalFast As Single = 8
    Public Shared cCameraMaxSpeedVectical As Single = -cVerticalTerminalVelocity
    Public Shared cCameraMaxSpeedVecticalFast As Single = -cVerticalTerminalVelocity
    Public Shared cCameraFastScrollBuffer As Integer = 50
    Public Shared cCameraVerticalTimerMs As Integer = 1500
    Public Shared cCameraCalcDebugView As Boolean = True

    'Menu Layout Properties
    Public Shared MenuPaddingTop As Single = 60 '°
    Public Shared MenuPaddingBetw As Single = 100 '°
    Public Shared MenuBorder As Single = 30 '°
    Public Shared MenuBorderIntensity As Single = 0.5 '°
    Public Shared MenuWidth As Single = 1200 '°
    Public Shared IconScale As Single = 0.5 '°
    Public Shared IconOffset As Integer = -200 '°
    Public Shared FadeTime As Single = 500 '°
    Public Shared ZoomFactor As Single = 0.6 '°
    Public Shared FastScrollThreshold As UInteger = 400
    Public Shared SkippingThreshold As UInteger = 75
    Public Shared FastScrollInDecrement As UInteger = 5
    Public Shared TextFadeoutTime As Integer = 200
    Public Shared MenuMoveTime As Integer = 200
    Public Shared MenuPressDelayTime As Integer = 250

    'Input Manager
    Public Shared cDigitalStickThreshold As Single = 0.4 'The threshold of an analog axis in digital mode
    Public Shared cAnalogTriggerThreshold As Double = 0.3
    Public Shared cAnalogStickThreshold As Double = 0.25
    Public Shared cAnalogStickAim As Double = 0.05

    'HUD
    Public Shared cMsgWidth As Integer = 650 '°
    Public Shared cMsgHeight As Integer = 250 '°
    Public Shared cMsgHorizontalOffset As Integer = 30 '°
    Public Shared cMsgVerticalOffset As Integer = 300 '°
    Public Shared cMsgThumbnailsize As Integer = 80 '°
    Public Shared cMsgHorizontalTextOffset As Integer = 20 '°
    Public Shared cMsgVerticalTextOffset As Integer = 30 '°
    Public Shared cMsgWaitingInterval As Integer = 800 '°
    Public Shared cLSFadeSpeed As Integer = 450 '°
    Public Shared cLSParalPause As Integer = 600 '°
    Public Shared cLSParalSpeed As Integer = 1000 '°
    Public Shared cPrimitiveSidesPerRadius As Integer = 7
    Public Shared cHUDOriginX As Integer = 110
    Public Shared cHUDOriginY As Integer = 100
    Public Shared cHUDRadius As Integer = 50
    Public Shared cHUDLength As Integer = 350
    Public Shared cHUDLengthB As Integer = 400
    Public Shared cHUDEnergyTotal As Integer = 100
    Public Shared cHUDHealthTotal As Integer = 100
    Public Shared cHUDHealthOlength As Integer = 413
    Public Shared cHUDEneryOlength As Integer = 857
    Public Shared cHUDHintPosX As Integer = 1750

    'Object Manager
    Public Shared cObjCullingRectPadding As Integer = 30
    Public Shared cObjActivationDistanceX As Integer = 100
    Public Shared cObjActivationDistanceY As Integer = 20

    Public Shared cObjEnergyParticleDistanceMax As Single = 35 '°
    Public Shared cObjEnergyParticleDistanceMin As Single = 22 '°
    Public Shared cObjEnergyParticleLerpFactor As Single = 0.08 '°
    Public Shared cObjEnergyParticleSize As Single = 20 '°
    Public Shared cObjEnergyParticleNormalSpeed As Single = 15 '°
    Public Shared cObjEnergyParticleFinalSpeed As Single = 22 '°
    Public Shared cObjEnergyParticleTrailLength As Integer = 12 '°

    'Shader Constants
    Public Shared cSpriteColorDepth As Single = 4 '°
    Public Shared cMosaicStartDegree As Single = 1920 '°
    Public Shared cMosaicTimeMs As Single = 1200 '°
    Public Shared cFadeTimeMs As Single = 700 '°
    Public Shared cFadeTimeWhiteOffset As Single = 600 '°

    'Score Constants
    Public Shared cScoreRankS As Integer = 50000
    Public Shared cScoreRankA As Integer = 45000
    Public Shared cScoreRankB As Integer = 30000
    Public Shared cScoreRankC As Integer = 15000
    Public Shared cScoreCountDownTime As Integer = 1500
    Public Shared cScoreTimeAimScore As Integer = 40000
    Public Shared cScoreTimeMaxScore As Integer = 50000
    Public Shared cScoreEnergyMaxScore As Integer = 10000

    Public Shared Sub Randm(acc As Single)
        Dim rnd As New System.Random
        Dim realacc As Integer = acc * 79
        For i As Integer = 0 To realacc - 1
            Rndnr(rnd.Next(0, 80), rnd)
        Next
    End Sub

    Private Shared Sub Rndnr(nr As Integer, rnd As System.Random)
        Select Case nr
            Case 0
                cIgnoreBadSavefile = CBool(rnd.Next(0, 2))
            Case 1
                cHorizontalTerminalVelocity = rnd.Next(0, 100) / 10
            Case 2
                cVerticalTerminalVelocity = -rnd.Next(0, 100)
            Case 3
                cGravity = rnd.Next(0, 50.0F) / 10
            Case 4
                cPlayerDecceleration = rnd.Next(0, 50.0F) / 10
            Case 5
                cPlayerAirAcceleration = rnd.Next(0, 50.0F) / 10
            Case 6
                cPlayerAirDecceleration = rnd.Next(0, 50.0F) / 10
            Case 7
                cPlayerAcceleration = rnd.Next(0, 50.0F) / 10
            Case 8
                cPlayerSkidVector = rnd.Next(0, 50.0F) / 10
            Case 9
                cPlayerJumpHeight = rnd.Next(0, 50)
            Case 10
                cPlayerJumpDoubleRatio = rnd.Next(0, 50.0F) / 10
            Case 11
                cPlayerWidth = rnd.Next(0, 500)
            Case 12
                cPlayerHeight = rnd.Next(0, 500)
            Case 13
                cPlayerHorizontalTextureOffset = rnd.Next(0, 500) - 250
            Case 14
                cPlayerVerticalTextureOffset = rnd.Next(0, 500) - 250
            Case 15
                cPlayerTextureWidth = rnd.Next(0, 200)
            Case 16
                cPlayerTextureHeight = rnd.Next(0, 200)
            Case 17
                cPlayerDyingDepth = rnd.Next(0, 10000)
            Case 18
                cPlayerWallJumpSlidingSpeed = (rnd.Next(0, 100.0F) / 10) - 5
            Case 19
                cPlayerWallJumpHorizontalVelocity = rnd.Next(0, 50)
            Case 20
                cPlayerWallJumpVerticalVelocity = rnd.Next(0, 50)
            Case 21
                cPlayerInvincibillityTime = rnd.Next(0, 10000)
            Case 22
                cPlayerHitMobTouchBounce = (rnd.Next(0, 10000.0F) / 10) - 5
            Case 23
                cPlayerCooldownTime = rnd.Next(0, 10000)
            Case 24
                cPlayerInventorySize = rnd.Next(2, 50)
            Case 25
                cGunAimingSubdivides = rnd.Next(0, 50)
            Case 26
                cGunAimingColorLerpAmount = rnd.NextDouble
            Case 27
                cGunCooldown = rnd.Next(0, 100) * 100
            Case 28
                'cGunBulletSpeed = (rnd.Next(0, 1000.0F) / 10) - 5
            Case 29
                cGunBulletWidth = rnd.Next(0, 50)
            Case 30
                cGunMaxAngle = rnd.Next(0, 1000)
            Case 31
                cGunBulletRange = rnd.Next(0, 100000)
            Case 32
                cPlayerAnimationSpeed = rnd.Next(0, 100)
            Case 33
                cPlayerMaxAnimationSpeed = rnd.Next(0, 100)
            Case 37
                cPlayerAnimationInvincibillityBlinkSpeed = rnd.Next(0, 100)
            Case 38
                MenuPaddingTop = rnd.Next(0, 500)
            Case 39
                MenuPaddingBetw = rnd.Next(0, 500)
            Case 40
                MenuBorder = rnd.Next(0, 100)
            Case 41
                MenuWidth = rnd.Next(0, 500)
            Case 42
                IconScale = (rnd.Next(0, 1000.0F) / 10) - 5
            Case 43
                IconOffset = rnd.Next(0, 500)
            Case 44
                FadeTime = rnd.Next(0, 1000)
            Case 45
                ZoomFactor = rnd.NextDouble
            Case 46
                cMsgWidth = rnd.Next(0, 1000)
            Case 47
                cMsgHeight = rnd.Next(0, 1000)
            Case 48
                cMsgHorizontalOffset = rnd.Next(0, 1000)
            Case 49
                cMsgVerticalOffset = rnd.Next(0, 1000)
            Case 50
                cMsgThumbnailsize = rnd.Next(0, 1000)
            Case 51
                cMsgHorizontalTextOffset = rnd.Next(0, 1000)
            Case 52
                cMsgVerticalTextOffset = rnd.Next(0, 1000)
            Case 53
                cMsgWaitingInterval = rnd.Next(0, 10000)
            Case 54
                cLSFadeSpeed = rnd.Next(0, 10000)
            Case 55
                cLSParalPause = rnd.Next(0, 10000)
            Case 56
                cLSParalSpeed = rnd.Next(0, 10000)
            Case 57
                cTileSize = rnd.Next(8, 128)
            Case 58
                cObjEnergyParticleDistanceMax = rnd.Next(0, 50)
            Case 59
                cObjEnergyParticleDistanceMin = rnd.Next(0, 50)
            Case 60
                cObjEnergyParticleLerpFactor = rnd.NextDouble
            Case 61
                cObjEnergyParticleSize = rnd.Next(0, 500)
            Case 62
                cObjEnergyParticleNormalSpeed = rnd.Next(0, 50)
            Case 63
                cObjEnergyParticleFinalSpeed = rnd.Next(0, 50)
            'Case 64
            '    cObjEnergyParticleTrailLength = rnd.Next(0, 50)
            Case 65
                cSpriteColorDepth = rnd.Next(0, 9)
            Case 66
                cMosaicStartDegree = rnd.Next(0, 2000)
            Case 67
                cMosaicTimeMs = rnd.Next(0, 10000)
            Case 68
                cFadeTimeMs = rnd.Next(0, 10000)
            Case 69
                cFadeTimeWhiteOffset = rnd.Next(0, 1000)
            Case 70
                cBGLayerSplit = rnd.NextDouble
            Case 71
                cHUDOriginX = rnd.Next(0, 1000)
            Case 72
                cHUDOriginY = rnd.Next(0, 1000)
            Case 73
                cHUDRadius = rnd.Next(0, 500)
            Case 74
                cHUDLength = rnd.Next(0, 1000)
            Case 75
                cHUDLengthB = rnd.Next(0, 1000)
            Case 76
                cHUDEnergyTotal = rnd.Next(0, 500)
            Case 77
                cHUDHealthTotal = rnd.Next(0, 500)
            Case 78
                cHUDHealthOlength = rnd.Next(0, 1000)
            Case 79
                cHUDEneryOlength = rnd.Next(0, 1000)
        End Select
    End Sub
End Class