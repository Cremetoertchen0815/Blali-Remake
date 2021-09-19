Imports System.Collections.Generic
Imports Emmond.Framework
Imports Emmond.Framework.Data
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Input
Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Emmond.IG
Imports Emmond.UI.General
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Media

''' <summary>
''' This is the main type for your game
''' </summary>

Public Class Emmond
    Inherits Game

    'FRM(Frame rate measurement)
    Public FrameCounter As FrameCounter

    'Hotkeys & debug
    Dim lastkstate As KeyboardState
    Dim nbrstring As String
    Dim nbractive As Boolean = False

    'Other flags
    Public Property RumbleFlag As Double 'Flag for the rumble(duh)
    Public LastRumble As Boolean = False

    Public Sub New()
        Graphx = New GraphicsDeviceManager(Me) With {.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8}
        Content.RootDirectory = "Content"
        ContentMan = Content
    End Sub

    Dim retried As Boolean = False

    Friend Sub UpdateGraphics()
        Try
            Graphx.PreferredBackBufferWidth = ResolutionList(SettingsMan.Value(21)).Width 'Set the appearant width
            Graphx.PreferredBackBufferHeight = ResolutionList(SettingsMan.Value(21)).Height 'Set the appearant height
            Graphx.SynchronizeWithVerticalRetrace = SettingsMan.ValueBool(24)
            Graphx.PreferMultiSampling = True
            Graphx.IsFullScreen = SettingsMan.Value(20) = 0

            Graphx.ApplyChanges()


            IsFixedTimeStep = False

            Backbuffer = New RenderTarget2D(Graphx.GraphicsDevice, Graphx.PreferredBackBufferWidth, Graphx.PreferredBackBufferHeight, False, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents)
            MainScalingMatrix = Matrix.CreateScale(Graphx.PreferredBackBufferWidth / GameSize.X, Graphx.PreferredBackBufferHeight / GameSize.Y, 1)
            BufferSize = New Vector2(Graphx.PreferredBackBufferWidth, Graphx.PreferredBackBufferHeight)
        Catch ex As Exception
            If Not retried Then
                SettingsMan.Value(21) = ResolutionList.Length - 1
                retried = True
            Else
                Throw ex
            End If
        End Try
    End Sub


    Protected Overrides Sub Initialize()
        GraphicsDevice.Clear(Color.Black)

        Dim ass(2) As Byte
        ReDim ass(800)
        ass(700) = 2

        'Load list of supported resolutions
        Dim modelist As New List(Of DisplayMode)
        For Each mode As DisplayMode In GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
            Dim exists As Boolean = False
            For Each element In modelist
                If element.Width = mode.Width And element.Height = mode.Height Then exists = True
            Next
            If Not exists Then modelist.Add(mode)
        Next
        ResolutionList = modelist.ToArray

        Me.IsMouseVisible = False 'Disables the visibillity of the cursor
        Window.AllowUserResizing = False 'Disables the Resizing of the window

        'Load SettingsMan
        SettingsMan = New SettingsManager
        'Create Graphics
        GameSize = New Vector2(1920, 1080) 'Set the basic backbuffer size
        Graphx.GraphicsProfile = GraphicsProfile.HiDef
        Graphx.GraphicsDevice.PresentationParameters.MultiSampleCount = 8
        Graphx.HardwareModeSwitch = True
        Graphx.ApplyChanges()

        UpdateGraphics()

        'FRM
        FrameCounter = New FrameCounter

        SceneMan = New SceneManager 'Initialize SceneManager
        SpriteBat = New SpriteBatch(GraphicsDevice) ' Create a new SpriteBatch, which can be used to draw textures.
        Automator = New TweenManager 'Initializes the effect Manager


        DebugMode = cActivateDebugMode

        GraphicsDevice.Clear(Color.Black)

        MyBase.Initialize() 'Initialize the rest
    End Sub

    ''' <summary>
    ''' LoadContent will be called once per game and is the place to load
    ''' all of your content.
    ''' </summary>
    Protected Overrides Sub LoadContent()
        Dim DontExit As Boolean = DebugMode 'Debug mode check flag

        'Load Debug Stuff
        DebugFont = Content.Load(Of SpriteFont)("font\fnt_debug")
        DebugSmolFont = Content.Load(Of SpriteFont)("font\fnt_debugsmol")
        RoundedCorner = Content.Load(Of Texture2D)("ge_rounded_edge")
        ReferencePixel = Content.Load(Of Texture2D)("ge_refpx")
        DebugTexture = Content.Load(Of Texture2D)("tst_dbg1")
        DebugTextureB = ContentMan.Load(Of Texture2D)("tst_dbg2")

        'Load InputManager
        InputMan = New InputManager
        InputMan.Init()

        'Load Localisation
        Texts = Localisation.GetLocalisationData([Enum].GetName(GetType(Localisation.Languages), SettingsMan.Value(19)))

        'Load Sounds
        AudioEng = New AudioEngine("Content\sound\xactlybeacon.xgs")
        AudioEng.GetCategory("Default").SetVolume(SettingsMan.Value(32) / 100)
        SFXWaveBank = New WaveBank(AudioEng, "Content\sound\sfx.xwb")
        MediaPlayer.Volume = SettingsMan.Value(31) / 100

        'Load Lighting engine
        Lighting = New Penumbra.PenumbraComponent(Me)
        Lighting.Initialize()
        Lighting.Visible = True
        Lighting.SpriteBatchTransformEnabled = True

        'Initialize error handler
        Dim errhnd As New ErrorHandler
        errhnd.Initialize()
        SceneMan.AddScene(errhnd)

        'CurrentEffect.Parameters.Item(0).SetValue(0F)
        Dim st As String() = Environment.GetCommandLineArgs()
        If st.Length >= 3 AndAlso st(1) = "-password" AndAlso st(2) = "I made a toothpick" Then
            If Not cDeativateDebugMode Then DebugMode = True
            If Lighting IsNot Nothing Then Lighting.Debug = SettingsMan.ValueBool(29)
            If DontExit Then DebugMode = False
        End If

        If DebugMode And Not SettingsMan.EndMyLife Then
            SceneMan.ChangeToScene(2) 'Load Main Menu
        ElseIf Not DebugMode And Not SettingsMan.EndMyLife Then
            DebugMode = False
            SettingsMan.ValueBool(29) = False 'Deactivate Debug mode
            If Lighting IsNot Nothing Then Lighting.Debug = False
            SceneMan.ChangeToScene(1)
        Else
            SceneMan.ChangeToScene(7)
        End If

    End Sub

    ''' <summary>
    ''' UnloadContent will be called once per game and is the place to unload
    ''' all content.
    ''' </summary>
    Protected Overrides Sub UnloadContent()
        ' TODO: Unload any non ContentManager content here
    End Sub

    ''' <summary>
    ''' Allows the game to run logic such as updating the world,
    ''' checking for collisions, gathering input, and playing audio.
    ''' </summary>
    ''' <param name="gameTime">Provides a snapshot of timing values.</param>
    Protected Overrides Sub Update(ByVal gameTime As GameTime)

        'Get Keyboard state
        Dim kstate As KeyboardState = Keyboard.GetState

        ' Allows the game to exit
        If kstate.IsKeyDown(Keys.F12) Then
            Me.Exit()
        End If

        'Implements gamepad rumble
        If RumbleFlag > 0 Then
            RumbleFlag -= gameTime.ElapsedGameTime.TotalMilliseconds
            If LastRumble = False Then InputMan.SetRumble(1, 1) : LastRumble = True
        Else
            If LastRumble = True Then InputMan.SetRumble(0, 0) : LastRumble = False
        End If

        'Hotkeys
        If kstate.IsKeyDown(Keys.F9) And (kstate.IsKeyDown(Keys.F9) <> lastkstate.IsKeyDown(Keys.F9)) And DebugMode Then
            SettingsMan.ValueBool(29, True) = Not SettingsMan.ValueBool(29)
            If Lighting IsNot Nothing Then Lighting.Debug = SettingsMan.ValueBool(29)
        End If


        'Debug options
        If DebugMode Then
            'Toggle free camera
            If SceneMan.CurrentID >= 14 Then
                If kstate.IsKeyDown(Keys.F10) And (kstate.IsKeyDown(Keys.F10) <> lastkstate.IsKeyDown(Keys.F10)) And SceneMan.CurrentID >= 10 Then
                    SettingsMan.ValueBool(30, True) = Not SettingsMan.ValueBool(30)
                End If
            End If

            'Level Editor Activation
            If kstate.IsKeyDown(Keys.Tab) And (kstate.IsKeyDown(Keys.Tab) <> lastkstate.IsKeyDown(Keys.Tab)) Then
                Dim lvl As Debug.LvlEditor = Nothing
                Try
                    lvl = CType(SceneMan.GetScene(10), Debug.LvlEditor)
                Catch
                End Try

                If lvl IsNot Nothing AndAlso SceneMan.CurrentID = 10 Then
                    Try
                        lvl = CType(SceneMan.GetScene(10), Debug.LvlEditor)
                    Catch
                    End Try
                    SceneMan.ChangeToScene(lvl.lastsceneID)
                Else
                    Dim sc As Integer = SceneMan.CurrentID
                    SceneMan.ChangeToScene(10)
                    Try
                        lvl = CType(SceneMan.GetScene(10), Debug.LvlEditor)
                        lvl.lastsceneID = sc
                    Catch
                    End Try
                End If
            End If

            If kstate.IsKeyDown(Keys.PageUp) And Not lastkstate.IsKeyDown(Keys.PageUp) Then
                nbrstring = ""
                nbractive = True
            ElseIf kstate.IsKeyDown(Keys.PageDown) And Not lastkstate.IsKeyDown(Keys.PageDown) And nbrstring <> "" Then
                SceneMan.ChangeToScene(CInt(nbrstring))
                LevelInstance = Nothing
                nbractive = False
            ElseIf kstate.IsKeyDown(Keys.NumPad0) And Not lastkstate.IsKeyDown(Keys.NumPad0) Then
                nbrstring = nbrstring & "0"
            ElseIf kstate.IsKeyDown(Keys.NumPad1) And Not lastkstate.IsKeyDown(Keys.NumPad1) Then
                nbrstring = nbrstring & "1"
            ElseIf kstate.IsKeyDown(Keys.NumPad2) And Not lastkstate.IsKeyDown(Keys.NumPad2) Then
                nbrstring = nbrstring & "2"
            ElseIf kstate.IsKeyDown(Keys.NumPad3) And Not lastkstate.IsKeyDown(Keys.NumPad3) Then
                nbrstring = nbrstring & "3"
            ElseIf kstate.IsKeyDown(Keys.NumPad4) And Not lastkstate.IsKeyDown(Keys.NumPad4) Then
                nbrstring = nbrstring & "4"
            ElseIf kstate.IsKeyDown(Keys.NumPad5) And Not lastkstate.IsKeyDown(Keys.NumPad5) Then
                nbrstring = nbrstring & "5"
            ElseIf kstate.IsKeyDown(Keys.NumPad6) And Not lastkstate.IsKeyDown(Keys.NumPad6) Then
                nbrstring = nbrstring & "6"
            ElseIf kstate.IsKeyDown(Keys.NumPad7) And Not lastkstate.IsKeyDown(Keys.NumPad7) Then
                nbrstring = nbrstring & "7"
            ElseIf kstate.IsKeyDown(Keys.NumPad8) And Not lastkstate.IsKeyDown(Keys.NumPad8) Then
                nbrstring = nbrstring & "8"
            ElseIf kstate.IsKeyDown(Keys.NumPad9) And Not lastkstate.IsKeyDown(Keys.NumPad9) Then
                nbrstring = nbrstring & "9"
            End If

        End If



        If kstate.IsKeyDown(Keys.F11) And (kstate.IsKeyDown(Keys.F11) <> lastkstate.IsKeyDown(Keys.F11)) Then
            If SettingsMan.ValueBool(20) Then
                SettingsMan.ValueBool(20, True) = False
                Graphx.IsFullScreen = False
                Graphx.ApplyChanges()
            ElseIf Not SettingsMan.ValueBool(20) Then
                SettingsMan.ValueBool(20, True) = True
                Graphx.IsFullScreen = True
                Graphx.ApplyChanges()
            End If
        End If


        lastkstate = kstate

        'Update common instances
        InputMan.GeneralInputUpdate() 'Update the Input Manager generally
        Automator.Update(gameTime)
        AudioEng.Update()

        'Update the Scene Manager & Scenes(so basically everything else :P)
        SceneMan.Update(gameTime)

        MyBase.Update(gameTime)
    End Sub

    ''' <summary>
    ''' This is called when the game should draw itself.
    ''' </summary>
    ''' <param name="gameTime">Provides a snapshot of timing values.</param>
    Protected Overrides Sub Draw(ByVal gameTime As GameTime)
        SpriteCount = 0
        GraphicsDevice.Clear(Color.Black)
        If SceneMan.CurrentScene IsNot Nothing And Not SceneMan.NoDraw Then SceneMan.CurrentScene.Draw(gameTime) 'Draw base scene

        SceneMan.LoadingScreen.Draw(gameTime)
        'Draw debug stuff
        Graphx.GraphicsDevice.SetRenderTarget(Nothing)
        SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, Nothing, MainScalingMatrix)

        If SettingsMan.ValueBool(29) And SceneMan.CurrentID > 13 And SceneMan.CurrentID < 60 Then
            'Draw backend
            Primitives2D.FillRectangle(New Rectangle(490, 10, 925, 140), New Color(15, 15, 15, 220))
            Primitives2D.FillRectangle(New Rectangle(GameSize.X - 380, 300, 370, 300), New Color(15, 15, 15, 220))
            SpriteBat.DrawString(DebugFont, Math.Round(FrameCounter.AverageFramesPerSecond, 2).ToString & " FPS(" & SpriteCount.ToString & " Sprites)", New Vector2(500, 10), Color.White)
            'Performance information

            'Other stuff
            If SceneMan.CurrentScene IsNot Nothing Then SceneMan.CurrentScene.DrawDebug(gameTime)
        ElseIf SettingsMan.ValueBool(22) And Not DebugMode Then
            Primitives2D.FillRectangle(New Rectangle(10, 1080 - 60, 120, 30), New Color(18, 18, 18, 210))
            SpriteBat.DrawString(DebugFont, Math.Round(FrameCounter.AverageFramesPerSecond, 1).ToString & " FPS", New Vector2(20, 1080 - 55), Color.White)
        End If
        SpriteBat.End()

        SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, Nothing, MainScalingMatrix)
        If SettingsMan.ValueBool(22) And DebugMode Then
            Primitives2D.FillRectangle(New Rectangle(10, 1080 - 190, 230, 160), New Color(18, 18, 18, 210))
            SpriteBat.DrawString(DebugFont, "FPS(Minimal): " & Math.Round(FrameCounter.MinimalFramesPerSecond, 1).ToString, New Vector2(20, 1080 - 180), Color.White)
            SpriteBat.DrawString(DebugFont, "FPS(Average): " & Math.Round(FrameCounter.AverageFramesPerSecond, 1).ToString, New Vector2(20, 1080 - 150), Color.White)
            SpriteBat.DrawString(DebugFont, "FPS(Maximal): " & Math.Round(FrameCounter.MaximalFramesPerSecond, 1).ToString, New Vector2(20, 1080 - 120), Color.White)
            If SceneMan.CurrentScene IsNot Nothing Then
                SpriteBat.DrawString(DebugFont, "Current Scene: " & SceneMan.CurrentID & "(" & SceneMan.CurrentScene.Config.Descriptor & ")", New Vector2(20, 1080 - 90), Color.White)
            Else
                SpriteBat.DrawString(DebugFont, "Current Scene: " & SceneMan.CurrentID, New Vector2(20, 1080 - 90), Color.White)
            End If
            SpriteBat.DrawString(DebugFont, "Transition Count: " & Automator.GetCount, New Vector2(20, 1080 - 60), Color.White)
        End If

        SpriteBat.DrawString(DebugFont, cCurrentVersion, New Vector2(GameSize.X - 230, GameSize.Y - 50), Color.White)
        SpriteBat.End()

        'Update FRM
        FrameCounter.Update(gameTime.ElapsedGameTime.TotalMilliseconds)

        MyBase.Draw(gameTime)
    End Sub

End Class
