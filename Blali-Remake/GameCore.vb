Imports Nez_template.Games.Blali_1

Public Class GameCore
    Inherits Core
    Sub New()
        MyBase.New(1920, 1080, False, "Blali Collection", "assets")
    End Sub

    Protected Overrides Sub Initialize()
        MyBase.Initialize()

        Screen.SetSize(1920, 1080)
        Scene.SetDefaultDesignResolution(1920, 1080, Scene.SceneResolutionPolicy.BestFit)
        Window.AllowUserResizing = True

        Scene = New GameScene

    End Sub
End Class