Imports System.Collections.Generic
Imports Microsoft.Xna.Framework

Namespace Framework.Level.Background

    <TestState(TestState.NearCompletion)>
    Public MustInherit Class BackgroundComponent
        Friend MustOverride Sub Init(lvl As Level)
        Friend MustOverride Sub Update(gameTime As GameTime, lvl As Level)
        Friend Property Elements As New List(Of IBackground)
    End Class
End Namespace
