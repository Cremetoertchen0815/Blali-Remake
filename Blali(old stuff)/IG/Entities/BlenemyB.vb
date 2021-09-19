
Imports Emmond.Framework.Entities
Imports Emmond.Framework.Level
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace IG.Entities
    Public Class BlenemyB
        Inherits Entity

        Public Sub New(UniqueID As Integer)
            MyBase.New(UniqueID)
        End Sub

        'Basics
        Public Dead As Boolean = False 'Expresses whether the player has died
        Dim rec As Rectangle

        'Movement Flags & constants
        Private Const cOrangeRange As Integer = 350
        Dim flipcooldown As Integer = 0
        Dim flip As Boolean = False

        'Textures
        Public Property Spritesheet As Texture2D 'The idle sprite
        Public Property Hit As Texture2D 'The idle sprite
        Dim ccA As Color = Color.White
        Dim ccB As Color = Color.White
        Dim v As Integer = 0

        Public Overrides Sub Initialize()
            mPosition = mSpawn
            mAABB.halfSize = New Vector2(39, 36)
            mAABB.center = mPosition + New Vector2(0, 64)
            mAttributes.ID = 12
            mAttributes.MaxHealth = 30
            mAttributes.Health = 30
            mAttributes.Energy = 20
            mAttributes.TouchDamage = 20
            mAttributes.ShootDamage = 0
            mAttributes.Lives = 1
            mAttributes.ReleaseScore = 750
            mAttributes.HitTrigger = Function(impactvectorX As Single, dmg As Single)
                                         mAttributes.Health -= dmg
                                         v = 100
                                         Return True
                                     End Function


            rec = New Rectangle(Math.Round(mAABB.GetRectangle.X) + 64, Math.Round(1080 - mAABB.GetRectangle.Y) + 32, 64, 128)
        End Sub

        Public Overrides Sub LoadContent(Optional playerid As Integer = 0)
            Spritesheet = ContentMan.Load(Of Texture2D)("entity\12\enemy_b")
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            If CulledIn Then
                rec = New Rectangle(Math.Round(mAABB.GetRectangle.X), Math.Round(1080 - mAABB.GetRectangle.Y - mAABB.halfSize.Y * 2), mAABB.halfSize.X * 2, mAABB.halfSize.Y * 2)

                Dim fx As SpriteEffects = SpriteEffects.FlipHorizontally
                If flip Then fx = SpriteEffects.None
                SpriteBat.Draw(Spritesheet, rec, Nothing, ccA, 0, Vector2.Zero, fx, 0.52)
            End If
        End Sub


        Public Overrides Function GetRect() As Rectangle
            Return New Rectangle(Math.Round(mAABB.GetRectangle.X), Math.Round(1080 - mAABB.GetRectangle.Y - mAABB.halfSize.Y * 2), mAABB.halfSize.X * 2, mAABB.halfSize.Y * 2)
        End Function

        Public Overrides Sub Update(gameTime As GameTime, lvl As Level, triggerinfluence As Boolean())

            If Not mObsolete Then

                mMap = lvl

                'Update the Physics
                UpdatePhysics(gameTime)

                If CulledIn Then

                    'Shoot & flip
                    flipcooldown += gameTime.ElapsedGameTime.TotalMilliseconds

                    If (rec.Right >= mSpawn.X + cOrangeRange Or mCollision(3)) And Not Dead Then
                        flip = True
                    ElseIf (rec.Left < mSpawn.X - cOrangeRange Or mCollision(2)) And Not Dead Then
                        flip = False
                    End If

                    If mAttributes.Health <= 0 And Not Dead Then Dead = True : v = 0

                    mSpeed = New Vector2(3.5, 0)
                    If flip Then mSpeed *= -1
                    If mCollision(3) And mSpeed.X > 0 Then mSpeed.X = 0
                    If mCollision(2) And mSpeed.X < 0 Then mSpeed.X = 0

                End If
            End If
        End Sub

        Public Overrides Sub DrawOverlay(gameTime As GameTime, lvl As Level)

        End Sub


        Dim CulledRectA As Rectangle
        Dim CulledRectB As Rectangle
        Public Overrides Function CullIn(camviewport As Rectangle) As Boolean
            CulledRectA = New Rectangle(Math.Round(mAABB.GetRectangle.X), Math.Round(1080 - mAABB.GetRectangle.Y - mAABB.halfSize.Y * 2), mAABB.halfSize.X * 2, mAABB.halfSize.Y * 2)
            Return camviewport.Intersects(CulledRectA) Or True
        End Function
    End Class
End Namespace