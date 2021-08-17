Imports System.Collections.Generic
Imports Emmond.Framework.SceneManager
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content

Namespace Framework.Modding

    <TestState(TestState.WorkInProgress)>
    Public MustInherit Class ModIdentifier
        'The basics
        Public MustOverride ReadOnly Property Name As String
        Public MustOverride ReadOnly Property Creator As String
        Public MustOverride ReadOnly Property Description As String

        Public Property ReplaceScenes As New Dictionary(Of Short, Short)

        Public Overridable Function RegisterScene(nr As Integer) As Scene
            Return Nothing
        End Function

        Public MustOverride Sub GeneralInitialize()
        Public MustOverride Sub GeneralLoadContent(Content As ContentManager)
        Public MustOverride Sub GeneralUnloadContent()
        Public MustOverride Sub GeneralUpdate(gameTime As GameTime)
    End Class
End Namespace
