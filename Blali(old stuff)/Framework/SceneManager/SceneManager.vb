Imports System.Collections.Generic
Imports System.Threading

Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Media

Namespace Framework.SceneManager

    <TestState(TestState.NearCompletion)>
    Public Class SceneManager
        Public SceneStack As Dictionary(Of UShort, Scene) 'Collection of all base scenes
        Public CurrentScene As Scene 'Current base scene

        'Loading screen flags
        Public LoadingThread As Thread
        Public LoadingScreen As UI.General.LoadingScreen
        Public NoUpdates As Boolean = False
        Public NoDraw As Boolean = False
        Public NoInput As Boolean = False
        Public RunFixedDeltaTThread As Boolean = False
        Dim bypass As Boolean = False
        Dim temppara As Integer

        Sub New()
            'Initialize basic variables
            SceneStack = New Dictionary(Of UShort, Scene)
            LoadingScreen = New UI.General.LoadingScreen
            LoadingScreen.LoadContent()
        End Sub

        Sub Update(gameTime As GameTime)
            'Update base scene
            Try
                If Not NoUpdates And CurrentScene IsNot Nothing Then CurrentScene.Update(gameTime)
            Catch ex As Exception
                NoteError(ex, False)
                NoUpdates = False
                NoDraw = False
                SceneMan.NoInput = False
                If CurrentScene IsNot Nothing Then
                    ErrorHandlerMessages = {"Error while updating scene(" & CurrentScene.Config.Descriptor & ")", ex.Message, ex.StackTrace}
                Else
                    ErrorHandlerMessages = {"Error while updating scene", ex.Message, ex.StackTrace}
                End If
                CurrentScene = SceneStack.Item(9)
                LoadingScreen.Disable = True
            End Try

            'Update Loading Screen
            LoadingScreen.Update(gameTime)
        End Sub

        Private Sub LoadSceneNbr(gen As Scene)
            Try
                Do Until LoadingScreen.Sleeper Or bypass
                    Thread.Sleep(50)
                Loop

                NoDraw = True

                'Thread.Sleep(50)

                GC.Collect()
                GC.WaitForFullGCComplete()


                If gen.Config.AutoLoadSoundBank Then gen.SoundBank = New Audio.SoundBank(AudioEng, "Content\sound\" & gen.Config.Descriptor & ".xsb")
                gen.Initialize()
                gen.LoadContent(temppara)
                gen.Loaded = True

                bypass = False
                AddScene(gen)
                CurrentScene = gen
                LoadingScreen.Done = True

                NoUpdates = False
                NoDraw = False
            Catch ex As Exception
                NoteError(ex, False)
                NoUpdates = False
                NoDraw = False
                SceneMan.NoInput = False
                ErrorHandlerMessages = {"Error while initializing scene(Loading thread)", ex.Message, ex.StackTrace}
                CurrentScene = SceneStack.Item(9)
                LoadingScreen.Disable = True
            End Try
        End Sub

        Public Function CanChange() As Boolean
            Return Not (LoadingScreen.Sleeper And Not LoadingScreen.SleeperC And ((CurrentScene IsNot Nothing AndAlso CurrentScene.Config.ID <> 9) Or CurrentScene Is Nothing))
        End Function

        Public Function ChangeToScene(switch As SceneSwitch) As Boolean
            If switch.ID >= 0 Then Return ChangeToScene(switch.ID, switch.Argument)
            Return False
        End Function

        Public Function ChangeToScene(NewScene As Short, Optional ByVal parameter As Integer = 0, Optional ByVal ForceReload As Boolean = False) As Boolean
            If LoadingScreen.Sleeper And Not LoadingScreen.SleeperC And ((CurrentScene IsNot Nothing AndAlso CurrentScene.Config.ID <> 9) Or CurrentScene Is Nothing) Then Return False

            Dim gen As Scene = Nothing
            Try
                If NewScene > 0 Then 'Changes the scene(if NewSceneA is 0, the current base scene is set to nothing)
                    If SceneStack.ContainsKey(NewScene) AndAlso Not SceneStack(NewScene).Config.ReloadOnSelection And Not ForceReload Then
                        'Scene already is in buffer, initalize
                        If CurrentScene IsNot Nothing AndAlso CurrentScene.Config.ID <> 9 And NewScene <> 10 Then CurrentScene.UnloadContent() : CurrentScene.Loaded = False
                        CurrentScene = SceneStack.Item(NewScene)
                        CurrentScene.Initialize()
                        If Not CurrentScene.Loaded Or CurrentScene.Config.ReloadOnSelection Then CurrentScene.LoadContent(parameter) : CurrentScene.Loaded = True
                        'Load sound data & stop bg music
                        MediaPlayer.Stop()
                        MediaPlayer.Volume = SettingsMan.Value(31) / 100
                        AudioEng.GetCategory("Default").SetVolume(SettingsMan.Value(32) / 100)
                        If CurrentScene.Config.AutoLoadSoundBank Then CurrentScene.SoundBank = New Audio.SoundBank(AudioEng, "Content\sound\" & CurrentScene.Config.Descriptor & ".xsb")
                        If NewScene = 9 Then CType(CurrentScene, UI.General.ErrorHandler).firsttick = False
                    ElseIf NewScene = 51 Then
                        CurrentScene = LoadingScreen
                        LoadingScreen.ShowDirectScreen = True
                        LoadingScreen.Initialize()
                    Else

                        'If scene is already contained in the stack, unload and recycle
                        If SceneStack.ContainsKey(NewScene) AndAlso NewScene <> 9 Then
                            gen = SceneStack(NewScene)
                            gen.UnloadContent()
                            gen.Loaded = False
                            SceneStack.Remove(NewScene)
                        Else 'In case the scene stack doesn't contain this selected scene, generate a new one
                            Select Case NewScene
                                Case 1
                                    gen = New IG.Cutscenes.LFIntro
                                Case 2
                                    gen = New UI.Menu.MainMenu
                                Case 6
                                    gen = New UI.Menu.ControllerMenu
                                Case 7
                                    gen = New UI.Menu.PlayRoom
                                Case 8
                                    gen = New UI.General.GeneralHandler
                                Case 9
                                    gen = New UI.General.ErrorHandler
                                Case 10
                                    gen = New Debug.LvlEditor
                                Case 11
                                    gen = New Debug.CutsceneEditor
                                Case 14
                                    gen = New IG.Levels.Lvl00
                                Case 52
                                    gen = New UI.Ingame.PauseMenu With {.Active = True}
                                Case 123
                                    gen = New IG.Cutscenes.BFM
                                Case 666
                                    gen = New UI.General.GeneralHandler
                                Case 3141
                                    gen = New Debug.Yeet.Pie
                            End Select
                        End If



                        NoUpdates = True

                        If gen.Config.ShowLoadingScreen Then
                            NoInput = True
                            temppara = parameter
                            LoadingScreen.EnableExtendedDisplay = gen.Config.ShowLSCustom
                            LoadingScreen.PlayerColor = gen.Config.LSCustomColor
                            Dim texts As String() = gen.LoadLSInformation(parameter)
                            LoadingScreen.LoadTitle = texts(0)
                            LoadingScreen.LoadDescription = texts(1)
                            LoadingScreen.ShowDirectScreen = False
                            LoadingScreen.changevolume = True
                            LoadingScreen.Initialize()
                            LoadingScreen.Disable = False
                            LoadingThread = New Thread(AddressOf LoadSceneNbr) With {.Name = "Loading Thread"}
                            LoadingThread.Start(gen)
                        Else
                            bypass = True
                            temppara = parameter
                            LoadSceneNbr(gen)
                        End If


                    End If

                Else
                    CurrentScene = Nothing
                End If

                Return True
            Catch ex As Exception
                NoteError(ex, False)
                NoUpdates = False
                NoDraw = False
                SceneMan.NoInput = False
                If gen IsNot Nothing Then
                    ErrorHandlerMessages = {"Error while initializing scene(" & gen.Config.Descriptor & ")", ex.Message, ex.StackTrace}
                Else
                    ErrorHandlerMessages = {"Error while initializing scene", ex.Message, ex.StackTrace}
                End If
                CurrentScene = SceneStack.Item(9)
                LoadingScreen.Disable = True
                Return False
            End Try
        End Function

        Public Sub AddScene(Scn As Scene)
            SceneStack.Add(Scn.Config.ID, Scn) '...a scene to the scene stack.
        End Sub

        Public Sub RemoveScene(ID As UShort)
            If ID <> 9 Then SceneStack.Remove(ID) 'Remove scene from base scene stack.
        End Sub

        Public Function GetScene(ID As UShort) As Scene
            Return SceneStack.Item(ID)
        End Function

        Public ReadOnly Property CurrentID As Integer
            Get
                If CurrentScene IsNot Nothing Then Return CurrentScene.Config.ID Else Return 0
            End Get
        End Property
    End Class
End Namespace