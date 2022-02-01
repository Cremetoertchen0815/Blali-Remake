

Namespace Menu


    Public Class MainMenu
        Inherits Scene

        Private lastmstate As MouseState

        Public Overrides Sub Initialize()
            AddRenderer(New DefaultRenderer)
            CreateEntity("BlaliHintergrundbildDeckmode").SetLocalPosition(New Vector2(1920 / 2, 1020 / 2)).SetScale(2).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("toadallybestbackgwoundpnguwillevewsee")))
            CreateEntity("Buttons").SetLocalPosition(New Vector2(1920 / 3 - 350, 1020 / 3 - 75)).AddComponent(New PrototypeSpriteRenderer(350, 50 * 3.5)).AddComponent(New TextComponent(New NezSpriteFont(Content.Load(Of SpriteFont)("font/button_mainmenu")), "STart!", New Vector2(-220, -100), Color.Black))


        End Sub
        Public Overrides Sub Update()
            Dim mstate As MouseState = Mouse.GetState
            Dim mpos As Point = Vector2.Transform(mstate.Position.ToVector2, Matrix.Invert(ScreenTransformMatrix)).ToPoint
            Dim OneshotPressed As Boolean = mstate.LeftButton = ButtonState.Pressed And lastmstate.LeftButton = ButtonState.Released

            If mpos.X > 115 And mpos.Y > 177 AndAlso mpos.X < 463 And mpos.Y < 350 AndAlso OneshotPressed = True Then
                Core.Scene = New Games.Blali_1.Vanilla.GameScene(0)
            End If
            'If OneshotPressed = True Then
            '    System.Console.WriteLine("Tach3 because why not xP !1!!")
            'End If
            MyBase.Update()
        End Sub

    End Class

End Namespace