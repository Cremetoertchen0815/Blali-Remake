Imports Emmond.Framework.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.ObjectManager.Objects

    <TestState(TestState.WorkInProgress)>
    Public Class AnimeTyp
        Inherits LevelObject

        Dim lvl As Level
        Dim Textures As Integer()
        Dim Actived As Boolean = False
        Dim CapturedItemNbr As Integer = 1

        Public Overrides ReadOnly Property Type As Integer
            Get
                Return 7
            End Get
        End Property

        Public Overrides Sub Draw(gameTime As GameTime)
            SpriteBat.Draw(lvl.ItemMan.Textures(9), New Rectangle(CenterLocation.X - (Size.X / 2), CenterLocation.Y - (Size.Y / 2), Size.X, Size.Y), New Color(255, 255, 255, Alpha))
        End Sub

        Sub New(lvlS As Level, LocationS As Vector2)
            CenterLocation = New Vector2(LocationS.X, LocationS.Y)
            Size = New Vector2(100, 200)
        End Sub

        Public Overrides Sub Update(gameTime As GameTime, act As Entity)

        End Sub

        Public Overrides Sub Activate(MultiShader As Effect)

        End Sub

        Public Overrides Sub Init(lvlS As Level)
            lvl = lvlS
            If Not lvlS.ItemMan.Textures.ContainsKey(9) Then lvlS.ItemMan.Textures.Add(9, ContentMan.Load(Of Texture2D)("obj\6\enemy_d"))

            Actived = False
            ActivationMode = ObjectActivationType.PlayerTouch
            ExecutionType = ExecType.OncePerTouch
            ReleasePoints = 50
        End Sub
    End Class
End Namespace