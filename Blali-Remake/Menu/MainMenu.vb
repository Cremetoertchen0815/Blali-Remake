Namespace Menu
    Public Class MainMenu
        Inherits Scene
        Public Overrides Sub Initialize()
            AddRenderer(New DefaultRenderer)
            CreateEntity("BlaliHintergrundbildDeckmode").SetLocalPosition(New Vector2(1920 / 2, 1020 / 2)).SetScale(2).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("toadallybestbackgwoundpnguwillevewsee")))
            CreateEntity("Buttons").SetLocalPosition(New Vector2(1920 / 3 - 350, 1020 / 3 - 75)).AddComponent(New PrototypeSpriteRenderer(350, 50 * 3.5)) '{With color = color.blanchedalmond}
        End Sub

    End Class
End Namespace