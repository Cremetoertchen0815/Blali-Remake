Imports System.Collections.Generic
Imports Emmond.Framework.Camera
Imports Emmond.Framework.Data
Imports Emmond.Framework.Entities
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Graphics.PostProcessing
Imports Emmond.Framework.Input
Imports Emmond.Framework.Level
Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Emmond.UI.Ingame
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Media

Namespace IG

    Public MustInherit Class LevelClass
        Inherits Scene

        'Basic Instances
        Public lvl As Level 'Level instance
        Public pl As Player 'Player instance
        Public menu As PauseMenu 'Pause Menu instance

        'Misc
        Dim obsoleteentities As List(Of Entity)
        Dim triggerinfluence As Boolean() = {False, False, False, False}
        Dim fadeout As Boolean = False
        Dim fndir As Integer = 1 'The direction to run to when run thorugh the finish line
        Dim Showskybox As Boolean
        Dim timer As Integer = 0
        Dim inst As Integer = 0

        'Draw Stuff
        Protected DrawAdditionalLighting As Boolean = True
        Protected WCamMatrix As Matrix
        Protected WoCamMatrix As Matrix
        Friend MapRenderer As MapRenderer

        'Shaders
        Protected MultiShader As Effect
        Protected ColorSatShader As Effect
        Protected ColorSpaceShader As Effect
        Protected AlphaPrem As Effect

        'Bloom
        Protected ffx As BloomFilter
        Protected TempTarget As RenderTarget2D
        Protected TempTargetB As RenderTarget2D
        Protected TempTargetC As RenderTarget2D

        'Level Finish
        Protected FinalAction As SceneSwitch
        Protected AimTime As TimeSpan
        Protected MaxTime As TimeSpan
        Protected LevelAddressOffset As Integer = -1

        Friend Overrides Function LoadLSInformation(instructionnr As Integer) As String()
            Dim templvl As Level = LevelParser.LoadLevel(Config.Descriptor)
            inst = instructionnr
            If instructionnr < 0 Then
                Dim lst As New List(Of String) From {
                    templvl.Header.Description
                }
                For i As Integer = 0 To templvl.Header.Instructions.Length - 1
                    lst.Add(templvl.Header.Instructions(i))
                Next
                Return lst.ToArray
            Else
                Return {templvl.Header.Description, templvl.Header.Instructions(instructionnr)}
            End If
        End Function

        Public Overrides Sub Initialize()
            menu = New PauseMenu With {.PauseMode = PauseMode.Play, .vlass = Me}
            menu.Initialize()

            fadeout = False
            InputMan.Init()
            obsoleteentities = New List(Of Entity)
            timer = 0
            SceneMan.RunFixedDeltaTThread = False

        End Sub

        Protected MustOverride Sub LvlLoad()
        Protected MustOverride Sub LvlConfig()
        Protected MustOverride Function SubLvlConfig(sublvlnr As Integer) As Level
        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)

            'Parse the level
            Dim ali As Boolean = DebugMode And LevelInstance IsNot Nothing AndAlso LevelInstance.Header.LoadedID = Config.Descriptor

            'Try
            If ali Then
                lvl = LevelInstance
                lvl.Header.LoadedInstruction = parameter
                lvl.Background.Components.Clear()

                If pl Is Nothing Then LvlLoad()
            Else
                lvl = LevelParser.LoadLevel(Config.Descriptor)
                LevelParser.LoadSubLevel(0, lvl)
                lvl.Header.LoadedInstruction = parameter

                LvlLoad()
            End If

            'Prepare and play Music & SFX
            lvl.MSFX.SFXSoundBank = New Audio.SoundBank(AudioEng, "Content\sound\lvl_sfx.xsb")
            If lvl.MSFX.BGMusic IsNot Nothing Then
                MediaPlayer.Play(lvl.MSFX.BGMusic)
                MediaPlayer.IsRepeating = True
            End If

            'Load level
            LvlConfig()
            lvl.Entities(0) = pl
            If ali Then lvl.LoadContent(False, True) Else lvl.LoadContent()

            For Each item In lvl.ItemMan
                If item.Value.Type = 3 Then
                    CType(item.Value, ObjectManager.Objects.SublevelDoor).SublvlLoad = AddressOf SubLvlConfig
                End If
            Next

            LevelInstance = lvl
            Showskybox = lvl.Background.skybox IsNot Nothing
            CameraCalculator.Init()
            MapRenderer = New MapRenderer
            MapRenderer.Init(lvl)
            lvl.MapRenderer = MapRenderer
            GunSimple.txt = ContentMan.Load(Of Texture2D)("entity\9\bullet")
            'Catch ex As Exception
            '    'Reload level if ali(already loaded instance) is corrupted
            '    If ali Then
            '        LevelInstance = Nothing
            '        LoadContent(parameter)
            '        GC.Collect()
            '    Else
            '        Throw ex
            '    End If
            'End Try

            'Prepare HUD
            menu.LoadContent()

            'Generate temporary main rendertarget for bloom effect
            TempTarget = New RenderTarget2D(
            Graphx.GraphicsDevice,
            BufferSize.X,
            BufferSize.Y,
            False,
            Graphx.GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents) With {.Name = "TmpA"}

            'Generate pause menu background rendertarget
            TempTargetC = New RenderTarget2D(
            Graphx.GraphicsDevice,
            BufferSize.X,
            BufferSize.Y,
            False,
            Graphx.GraphicsDevice.PresentationParameters.BackBufferFormat,
            DepthFormat.None, 0, RenderTargetUsage.DiscardContents) With {.Name = "TmpC"}

            'Generate Bloom filter
            If lvl.FXData IsNot Nothing Then
                ffx = New BloomFilter
                ffx.Load(Graphx.GraphicsDevice, ContentMan, BufferSize.X, BufferSize.Y)
                ffx.BloomPreset = lvl.FXData.BloomPreset
                ffx.BloomThreshold = lvl.FXData.BloomThreshold
                ffx.BloomStrengthMultiplier = lvl.FXData.BloomStrengthMultiplier
            End If

            'Load Shaders
            MultiShader = ContentMan.Load(Of Effect)("fx\merged\fx_lvl")
            ColorSatShader = ContentMan.Load(Of Effect)("fx\fx_colorsat")
            ColorSpaceShader = ContentMan.Load(Of Effect)("fx\fx_colorspace")
            AlphaPrem = ContentMan.Load(Of Effect)("fx\fx_alphapremult")

            ColorSpaceShader.Parameters("colorbitdepth").SetValue(cSpriteColorDepth)
            MultiShader.Parameters("horDivide").SetValue(GameSize.X)
            pl.MultiShader = MultiShader
            MultiShader.Parameters.Item(2).SetValue(100.0F)
            MultiShader.Parameters.Item(3).SetValue(0.0F)
            MultiShader.Parameters.Item(5).SetValue(lvl.Background.skybox)
        End Sub

        Public Overrides Sub UnloadContent()
            Loaded = False

            If pl IsNot Nothing AndAlso pl.CriticalEnergyCue IsNot Nothing Then pl.CriticalEnergyCue.Stop(Audio.AudioStopOptions.Immediate)

            'Unload the level
            If Not DebugMode And lvl IsNot Nothing Then lvl.UnloadContent() 'Prevent class from doing that in debug mode in order to allow fluid lve/lvl switching
            lvl = Nothing

            'Unload the player
            pl = Nothing

            'Reset camera lock
            CameraCalculator.SetFocus = False

            TempTarget.Dispose()
            TempTargetC.Dispose()
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            If menu.PauseMode <> PauseMode.Fade And Loaded Then

                Dim bloom As Boolean = SettingsMan.ValueBool(23) And lvl.FXData.EnableBloom

                DrawLevel(gameTime)

                If bloom Then
                    'Generate bloom
                    Graphx.GraphicsDevice.SetRenderTargets(Nothing)
                    Dim txbloom As Texture2D = ffx.Draw(TempTarget, BufferSize.X, BufferSize.Y)

                    If menu.PauseMode = PauseMode.Capture Then
                        'Draw contents + added bloom to capture target
                        Graphx.GraphicsDevice.SetRenderTargets(TempTargetC)
                        Graphx.GraphicsDevice.Clear(Color.Transparent)
                        menu.PauseMode = PauseMode.Fade
                    Else
                        'Draw contents + added bloom to main frame buffer
                        Graphx.GraphicsDevice.SetRenderTargets(Nothing)
                        Graphx.GraphicsDevice.Clear(Color.Black)
                    End If

                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, Nothing, Nothing, MultiShader, Nothing)
                    SpriteBat.Draw(TempTarget, New Rectangle(lvl.CamShaker.Offset.X, lvl.CamShaker.Offset.Y, BufferSize.X, BufferSize.Y), Color.White)
                    SpriteBat.Draw(txbloom, New Rectangle(lvl.CamShaker.Offset.X, lvl.CamShaker.Offset.Y, BufferSize.X, BufferSize.Y), Color.White)
                    SpriteBat.End()
                Else
                    If menu.PauseMode = PauseMode.Capture Then
                        Graphx.GraphicsDevice.SetRenderTargets(TempTargetC)
                        Graphx.GraphicsDevice.Clear(Color.Black)
                        menu.PauseMode = PauseMode.Fade
                    Else
                        Graphx.GraphicsDevice.SetRenderTargets(Nothing)
                        Graphx.GraphicsDevice.Clear(Color.Black)
                    End If

                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, MultiShader, MainScalingMatrix)
                    SpriteBat.Draw(TempTarget, New Rectangle(lvl.CamShaker.Offset.X, lvl.CamShaker.Offset.Y, GameSize.X, GameSize.Y), Color.White)
                    SpriteBat.End()
                End If


            End If

            If menu.PauseMode <> PauseMode.Play Then
                Graphx.GraphicsDevice.SetRenderTargets(Nothing)
                Graphx.GraphicsDevice.Clear(Color.Transparent)

                SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, ColorSatShader, MainScalingMatrix)
                SpriteBat.Draw(TempTargetC, New Rectangle(0, 0, GameSize.X, GameSize.Y), Color.White)
                SpriteBat.End()
            End If

            If menu.Active Then menu.Draw(gameTime)
        End Sub

        Protected MustOverride Sub AddDraw(gameTime As GameTime)
        Private Sub DrawLevel(gameTime As GameTime)

            MapRenderer.PreDraw()

            'Prepare Bloom and Lighting
            Dim bloom As Boolean = SettingsMan.ValueBool(23) And lvl.FXData.EnableLighting AndAlso lvl.FXData.EnableBloom
            Graphx.GraphicsDevice.SetRenderTargets(TempTarget)
            If Lighting IsNot Nothing Then Lighting.BeginDraw()
            Graphx.GraphicsDevice.Clear(Color.Transparent)

            '-----Draw backgrounds(back)-----
            SpriteBat.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, WoCamMatrix)
            lvl.Background.DrawAll(gameTime, False)
            SpriteBat.End()

            '-----Draw Sprite Layer(influenced by lighting)-----
            SpriteBat.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, ColorSpaceShader, WCamMatrix)
            '-Triggers
            For Each element In lvl.TriggerMan
                element.Value.Draw()
            Next
            '-Entities
            For Each entity In lvl.Entities
                entity.Draw(gameTime)
            Next
            '-Items(influenced by lighting)
            lvl.ItemMan.DrawA(gameTime)
            '-Map
            MapRenderer.Draw()
            'Additional stuff
            If DrawAdditionalLighting Then AddDraw(gameTime)
            SpriteBat.End()

            If Lighting IsNot Nothing Then Lighting.Draw(gameTime)


            '-----Draw Sprite Layer(not influenced by lighting)-----
            SpriteBat.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, WCamMatrix)
            '-items(not influenced by lighting)
            lvl.ItemMan.DrawB(gameTime)
            '-Entity overlays
            For Each entity In lvl.Entities
                If entity.CulledIn Then entity.DrawOverlay(gameTime, lvl)
            Next
            'Additional stuff
            If Not DrawAdditionalLighting Then AddDraw(gameTime)

            GunSimple.Draw(gameTime)

            SpriteBat.End()


            '-----Draw backgrounds(front)-----
            SpriteBat.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, WoCamMatrix)
            lvl.Background.DrawAll(gameTime, True)
            SpriteBat.End()

        End Sub

        Public Overrides Sub DrawDebug(gameTime As GameTime)
            Try
                If menu.PauseMode = PauseMode.Play Then
                    lvl.DrawDebug()
                    pl.DrawDebug(gameTime, lvl)

                    For Each element In lvl.TriggerMan
                        element.Value.DrawDebug(lvl)
                    Next

                    SpriteBat.End()


                    '-----Draw backgrounds(front)-----
                    SpriteBat.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, AlphaPrem, WCamMatrix)

                    If CameraCalculator.BorderAreas IsNot Nothing Then
                        For i As Integer = 0 To 1
                            Dim orr As Rectangle = CameraCalculator.BorderAreas(i)
                            DrawRectangle(New Rectangle(lvl.Camera.TranslateFromCamera(orr.Location.ToVector2).ToPoint, (orr.Size.ToVector2 / lvl.Camera.Zoom).ToPoint), Color.Purple, 2)
                        Next
                        For i As Integer = 2 To 3
                            Dim orr As Rectangle = CameraCalculator.BorderAreas(i)
                            DrawRectangle(New Rectangle(lvl.Camera.TranslateFromCamera(orr.Location.ToVector2).ToPoint, (orr.Size.ToVector2 / lvl.Camera.Zoom).ToPoint), Color.Gray, 2)
                        Next
                    End If
                Else
                    menu.DrawDebug(gameTime)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Protected MustOverride Sub LvlUpdate(gameTime As GameTime, cstate As GameInputState)
        Public Overrides Sub Update(gameTime As GameTime)
            Dim state As GameInputState = InputMan.GetGameInput(True)

            If menu.PauseMode = PauseMode.Play And Loaded Then

                'Update specific stuff
                LvlUpdate(gameTime, state)

                'Process Triggers
                triggerinfluence = {False, False, False, False}
                For Each element In lvl.TriggerMan
                    element.Value.Update(lvl, pl, fndir, triggerinfluence)
                Next

                'Update all entities
                For Each entity In lvl.Entities
                    entity.Update(gameTime, lvl, triggerinfluence)
                    If entity.mObsolete Then obsoleteentities.Add(entity)
                Next

                For Each obsolete In obsoleteentities
                    lvl.Entities.Remove(obsolete)
                Next
                If obsoleteentities.Count > 0 Then obsoleteentities.Clear()

                'Update items
                Dim caninteract As Boolean = False
                If lvl IsNot Nothing Then lvl.ItemMan.Update(gameTime, lvl, MultiShader, pl, caninteract)

                'Helps with the running after touching the finish line
                If lvl.Header.LevelState = FinishedMode.TouchedFinishLine And Math.Floor(Math.Abs(pl.mSpeed.X)) < cHorizontalTerminalVelocity Then
                    pl.mSpeed.X += cPlayerAcceleration * fndir
                    pl.AccFlip = Math.Sign(fndir) < 0
                    pl.PlayerState = PlayerStatus.Run
                End If
                If lvl.Header.LevelState = FinishedMode.TouchedFinishLine And (pl.mAABB.GetRectangle.Right < 0 Or pl.mAABB.GetRectangle.Left > (lvl.Map.Size.X + 1) * cTileSize) Then
                    lvl.Header.LevelState = FinishedMode.OutOfView
                End If

                'Calculate Camera
                lvl.Camera.AdditionalImpulse = pl.mAdditionalImpulse / 2
                timer += gameTime.ElapsedGameTime.TotalMilliseconds
                CameraCalculator.CalculateTypeB(lvl.Camera, pl, timer > cCameraVerticalTimerMs)
                lvl.Camera.UpdateCamera(gameTime)
                MapRenderer.Update(gameTime)
                lvl.Background.UpdateAll(gameTime, lvl)

                If state.Start Then
                    MediaPlayer.Pause()
                    menu.PauseMode = PauseMode.Capture
                    ColorSatShader.Parameters.Item(0).SetValue(100.0F)
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(250), 100.0F, 0F, ColorSatShader, 0, Sub()
                                                                                                                                                menu.Open()
                                                                                                                                            End Sub)) 'Fade from normal to gray
                End If

                If state.Backpack And SettingsMan.ValueBool(144) Then
                    MediaPlayer.Pause()
                    menu.PauseMode = PauseMode.Capture
                    ColorSatShader.Parameters.Item(0).SetValue(100.0F)
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(250), 100.0F, 0F, ColorSatShader, 0, Sub()
                                                                                                                                                menu.Active = True
                                                                                                                                                menu.OpenBackpack()
                                                                                                                                            End Sub)) 'Fade from normal to gray
                End If

                If lvl.Header.LevelState = FinishedMode.OutOfView Then
                    FinishStage()
                End If


                If lvl IsNot Nothing Then
                    WoCamMatrix = lvl.Camera.GetMatrix(True, True) * MainScalingMatrix
                    WCamMatrix = lvl.Camera.GetMatrix(True) * MainScalingMatrix

                    If Lighting IsNot Nothing AndAlso Lighting.Lights.Count > 0 Then
                        Lighting.Lights(0).Position = New Vector2(pl.mPosition.X, 1080 - pl.mPosition.Y)
                        Lighting.Transform = WCamMatrix
                    End If
                End If


                EmmondInstance.IsMouseVisible = False
            Else
                menu.Update(gameTime)

                If menu.hasntended Then
                    menu.hasntended = False
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(250), 0F, 100.0F, ColorSatShader, 0, Sub()
                                                                                                                                                menu.PauseMode = PauseMode.Play
                                                                                                                                                menu.Active = False
                                                                                                                                                menu.hasntstart = False
                                                                                                                                            End Sub)) 'Fade from black to normal
                End If
            End If

        End Sub

        Public Sub FinishStage()
            If Not fadeout Then
                fadeout = True
                pl.MapVolumeToBrightness = True
                Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Linear(cLevelFadeOutComplete), 100.0F, 0F, pl.MultiShader, 2, Sub()
                                                                                                                                                        LevelInstance = Nothing
                                                                                                                                                        MediaPlayer.Stop()
                                                                                                                                                        SceneMan.ChangeToScene(12)
                                                                                                                                                    End Sub))
            End If
        End Sub

        Public Sub Restart()
            SceneMan.ChangeToScene(Config.ID, inst, True)
        End Sub
    End Class

End Namespace