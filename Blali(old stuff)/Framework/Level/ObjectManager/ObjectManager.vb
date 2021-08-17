Imports System.Collections.Generic
Imports Emmond.IG
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.ObjectManager

    <TestState(TestState.NearCompletion)>
    Public Class ObjectManager
        Inherits Dictionary(Of Integer, LevelObject)
        Public Property Textures As New Dictionary(Of Integer, Texture2D)
        Dim obs As New List(Of Integer)

        Sub Init()

        End Sub

        Sub DrawA(gameTime As GameTime)
            For Each element In Me
                If Not element.Value.NoLight And element.Value.CulledIn Then
                    element.Value.Draw(gameTime)
                    SpriteCount += 1
                End If
            Next

        End Sub

        Sub DrawB(gameTime As GameTime)
            For Each element In Me
                If element.Value.NoLight And element.Value.CulledIn Then
                    element.Value.Draw(gameTime)
                    SpriteCount += 1
                End If
            Next

        End Sub

        Sub Update(gameTime As GameTime, lvl As Level, MultiShader As Effect, player As Player, ByRef caninteract As Boolean)
            obs.Clear()
            Dim playerrect As Rectangle = player.mAABB.GetRectangle

            For Each element In Me
                Dim itm As LevelObject = element.Value
                Dim activate As Boolean
                Dim itmrect As Rectangle

                If itm.ActivationMode = ObjectActivationType.PlayerTouch Then
                    itmrect = New Rectangle(itm.CenterLocation.X - (itm.Size.X / 2), GameSize.Y - itm.CenterLocation.Y + (itm.Size.Y / 2), element.Value.Size.X, itm.Size.Y)
                    activate = itmrect.Intersects(playerrect)
                Else
                    itmrect = New Rectangle(itm.CenterLocation.X - (itm.Size.X / 2) - cObjActivationDistanceX, GameSize.Y - itm.CenterLocation.Y + (itm.Size.Y / 2) - cObjActivationDistanceY, itm.Size.X + (cObjActivationDistanceX * 2), itm.Size.Y + (cObjActivationDistanceY * 2))
                    Dim intersects As Boolean = itmrect.Intersects(playerrect)
                    activate = intersects And player.cstate.Interact
                    If intersects Then caninteract = True
                End If

                itm.Active = False
                itm.CulledIn = New Rectangle(itm.CenterLocation.X - (itm.Size.X / 2) - cObjCullingRectPadding, itm.CenterLocation.Y + (itm.Size.Y / 2) - cObjCullingRectPadding, itm.Size.X + 2 * cObjCullingRectPadding, itm.Size.Y + 2 * cObjCullingRectPadding).Intersects(lvl.Camera.Viewport)

                If itm.CulledIn Then
                    Select Case itm.ExecutionType
                        Case ExecType.OnceTotal
                            If activate And Not itm.LastActivation Then itm.LastActivation = True : itm.Activate(MultiShader) : itm.Active = True
                        Case ExecType.OncePerTouch
                            If activate And Not itm.LastActivation Then itm.LastActivation = True : itm.Activate(MultiShader) : itm.Active = True
                            If Not activate Then itm.LastActivation = False
                        Case ExecType.Continuous
                            If activate Then itm.Activate(MultiShader) : itm.Active = True
                    End Select

                    If itm.obsolete Then obs.Add(element.Key) : lvl.Entities(0).mAttributes.Score += itm.ReleasePoints

                    itm.Update(gameTime, player)
                End If
            Next

            For Each obsel In obs
                Remove(obsel)
            Next

        End Sub
    End Class
End Namespace