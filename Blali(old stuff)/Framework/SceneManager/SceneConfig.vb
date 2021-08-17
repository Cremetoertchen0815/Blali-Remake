Imports Microsoft.Xna.Framework

Namespace Framework.SceneManager

    <TestState(TestState.NearCompletion)>
    Public Structure SceneConfig
        Public ID As Integer
        Public Descriptor As String
        Public ReloadOnSelection As Boolean
        Public ShowLoadingScreen As Boolean
        Public AutoLoadSoundBank As Boolean
        Public ShowLSCustom As Boolean
        Public LSCustomColor As Color
    End Structure
End Namespace
