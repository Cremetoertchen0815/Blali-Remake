Imports Emmond.Framework.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.ObjectManager

    <TestState(TestState.NearCompletion)>
    Public MustInherit Class LevelObject
        Friend obsolete As Boolean
        Public Property CenterLocation As Vector2
        Public Property Alpha As Integer = 255
        Public Property Size As Vector2
        Public Property NoLight As Boolean = False
        Public Property Active As Boolean
        Public Property ExecutionType As ExecType
        Public Property ActivationMode As ObjectActivationType
        Public Property ReleasePoints As Integer = 0
        Protected Friend LastActivation As Boolean
        Protected Friend CulledIn As Boolean
        Public MustOverride ReadOnly Property Type As Integer


        Public MustOverride Sub Draw(gameTime As GameTime)
        Public MustOverride Sub Update(gameTime As GameTime, pl As Entity)
        Public MustOverride Sub Init(lvlS As Level)
        Public MustOverride Sub Activate(shader As Effect)
    End Class
End Namespace