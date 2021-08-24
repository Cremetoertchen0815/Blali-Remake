Imports Blali.Framework.UI
Imports Blali.Intros

Public Class GameCore
    Inherits Core
    Sub New()
        MyBase.New(1920, 1080, False, "Blali Collection", "assets")
    End Sub

    Protected Overrides Sub Initialize()
        MyBase.Initialize()

        Screen.SetSize(1920, 1080)
        Scene.SetDefaultDesignResolution(1920, 1080, Scene.SceneResolutionPolicy.BestFit)
        Screen.SynchronizeWithVerticalRetrace = True
        Window.AllowUserResizing = True
        DebugRenderEnabled = False

        'Create MessageBoxer
        MsgBoxer = New MessageBoxer
        FinalRenderable = MsgBoxer
        RegisterGlobalManager(MsgBoxer)

        'Update transformation matrix if event is fired
        Emitter.AddObserver(CoreEvents.GraphicsDeviceReset, Sub() ScaleMatrix = Scene.ScreenTransformMatrix)

        Scene = New BFN()
        'Scene = New Games.Blali_1.GameScene(0)

    End Sub
End Class