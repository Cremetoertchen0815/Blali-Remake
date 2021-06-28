Imports Blali.Games.Blali_1

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

        Scene = New GameScene(4)

    End Sub
End Class