
Imports Emmond.Framework
Imports Emmond.Framework.Entities
Imports Emmond.Framework.Level
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace IG.Entities
    Public Class Portal
        Inherits Entity

        Public Sub New(UniqueID As Integer)
            MyBase.New(UniqueID)
        End Sub

        'Basics
        Dim rec As Rectangle

        'Movement Flags
        Dim bouncecooldown As Integer = 0 'The time to the next bullet show
        Dim alttexture As Boolean = False


        'Textures
        Public Property Spritesheet As Texture2D 'The idle sprite
        Public Property Hit As Texture2D 'The idle sprite
        Dim ccA As Color = Color.White
        Dim ccB As Color = Color.White
        Dim v As Integer = 0

        Public Overrides Sub Initialize()
            mPosition = mSpawn
            mAABB.halfSize = New Vector2(85 / 2, 47 / 2)
            mAABB.center = mPosition + mAABBOffset
            mAttributes.ID = 10
            mAttributes.MaxHealth = 50
            mAttributes.Health = 50
            mAttributes.Energy = 20
            mAttributes.TouchDamage = 0
            mAttributes.ShootDamage = 0
            mAttributes.Lives = 1
            mAttributes.ReleaseScore = 500
            mAttributes.HitTrigger = Function(impactvectorX As Single, dmg As Single)
                                         mAttributes.Health -= dmg
                                         v = 100
                                         Return True
                                     End Function
        End Sub

        Public Overrides Sub LoadContent(Optional playerid As Integer = 0)
            Spritesheet = ContentMan.Load(Of Texture2D)("entity\10\spritesheet")
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            If CulledIn Then

                rec = New Rectangle(Math.Round(mAABB.GetRectangle.X), Math.Round(1080 - mAABB.GetRectangle.Y - 35), 64, 35)

                Dim y As Integer = 0
                If alttexture Then y = 35

                SpriteBat.Draw(Spritesheet, rec, New Rectangle(0, y, 64, 35), ccA, 0, New Vector2(0, 0), Nothing, 0.51)
            End If
        End Sub

        Public Overrides Function GetRect() As Rectangle
            Return rec
        End Function

        Public Overrides Sub Update(gameTime As GameTime, lvl As Level, triggerinfluence As Boolean())

            If Not mObsolete Then

                mMap = lvl

                UpdatePhysics(gameTime, True)

                If CulledIn Then

                    'Shoot & flip
                    bouncecooldown += gameTime.ElapsedGameTime.TotalMilliseconds

                    If bouncecooldown >= 30 Then
                        alttexture = Not alttexture
                        bouncecooldown = 0
                    End If

                End If
            End If
        End Sub

        Public Overrides Sub DrawOverlay(gameTime As GameTime, lvl As Level)
        End Sub

        Dim CulledRect As Rectangle
        Public Overrides Function CullIn(camviewport As Rectangle) As Boolean
            CulledRect = mAABB.GetRectangle
            CulledRect = New Rectangle(CulledRect.X, 1080 - CulledRect.Y - CulledRect.Height, CulledRect.Width, CulledRect.Height)
            Return camviewport.Intersects(CulledRect)
        End Function
    End Class
End Namespace