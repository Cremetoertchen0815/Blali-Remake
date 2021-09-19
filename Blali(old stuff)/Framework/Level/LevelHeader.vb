Imports Microsoft.Xna.Framework

Namespace Framework.Level
    Public Class LevelHeader
        Public Property Description As String = "No lvl, only BRAIN MILKSHAKES" 'Contains the level's name
        Public Property Instructions As String() = {"Make sure to debug this", "Go to sleep", "Do the groceries", "Make sure to OwO"}
        Public Property SubLvlCount As Integer = 1
        Public Property StdCamLocations As Vector2()
        Public Property TileSetPath As String
        Public Property LevelState As FinishedMode = FinishedMode.Nottin
        'Flags
        Public Loaded As Boolean = False
        Public LoadedID As String
        Public LoadedInstruction As Integer
        Public LoadedSublvl As Integer

        Friend Function Clone() As LevelHeader
            Return Me.MemberwiseClone()
        End Function
    End Class
End Namespace