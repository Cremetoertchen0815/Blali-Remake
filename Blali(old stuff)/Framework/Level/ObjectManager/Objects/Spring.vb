
Imports Emmond.Framework.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.ObjectManager.Objects

    <TestState(TestState.WorkInProgress)>
    Public Class Spring
        Inherits LevelObject

        Dim lvl As Level
        Dim Actived As Boolean
        Dim ang As Single = 0

        Public Overrides ReadOnly Property Type As Integer
            Get
                Return 4
            End Get
        End Property

        Public Overrides Sub Draw(gameTime As GameTime)
            SpriteBat.Draw(lvl.ItemMan.Textures(6), New Rectangle(CenterLocation.X - (Size.X / 2), CenterLocation.Y - (Size.Y / 2), Size.X, Size.Y), Nothing, New Color(255, 255, 255, Alpha), 0, Vector2.Zero, SpriteEffects.None, 0.1)
        End Sub

        Sub New(lvlS As Level, LocationS As Vector2)
            CenterLocation = New Vector2(LocationS.X, LocationS.Y)
            Size = New Vector2(64)
        End Sub

        Dim lastcoll As Boolean = False
        Public Overrides Sub Update(gameTime As GameTime, act As Entity)
            Dim pr As Rectangle = act.mAABB.GetRectangle
            Dim ir As Rectangle = New Rectangle(CenterLocation.X - (Size.X / 2), 1080 - CenterLocation.Y - (Size.Y / 2), Size.X, Size.Y)
            Dim coll As Boolean = pr.Intersects(ir)

            If coll And Not lastcoll Then
                act.mSpeed.Y = 26.5
                CType(act, IG.Player).ExtBool = True
            End If

            lastcoll = coll
        End Sub

        Public Overrides Sub Activate(MultiShader As Effect)
            Actived = Not Actived
            lvl.MSFX.SFXSoundBank.PlayCue("switch")
        End Sub

        Public Overrides Sub Init(lvlS As Level)
            lvl = lvlS
            If Not lvlS.ItemMan.Textures.ContainsKey(6) Then lvlS.ItemMan.Textures.Add(6, ContentMan.Load(Of Texture2D)("obj\4\spring"))

            Actived = False
            ActivationMode = ObjectActivationType.PlayerTouch
            ExecutionType = ExecType.OncePerTouch
            ReleasePoints = 0
        End Sub
    End Class
End Namespace