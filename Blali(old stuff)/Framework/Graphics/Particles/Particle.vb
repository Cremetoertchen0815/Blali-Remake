

Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Namespace Framework.Graphics.Effects.Particles

    <TestState(TestState.NearCompletion)>
    Public Class Particle
        Implements IPoolable
        Public Texture As Texture2D
        Public DespawnHeight As Integer
        Public Flex As Boolean = False
        Public Opacity As Single
        Public Origin As Vector2
        Public Rotation As Single
        Public Scale As Vector2
        Public Color As Color = Color.White
        Public DualColor As Color = Color.White
        Public Layer As Single
        Public Property PoolIsValid As Boolean Implements IPoolable.PoolIsValid
        Public Property PoolIsFree As Boolean Implements IPoolable.PoolIsFree
        Public Position As Vector2
        Public Velocity As Vector2
        Dim FinalPosition As Vector2

        Public Sub Initialize() Implements IPoolable.Initialize
            Flex = False
            IsRemoved = False
            Opacity = 1
            Scale = Vector2.One
        End Sub

        Public Sub Release() Implements IPoolable.Release

        End Sub

        Public ReadOnly Property Rectangle As Rectangle
            Get
                Return New Rectangle(FinalPosition.X, FinalPosition.Y, Texture.Width * Scale.X, Texture.Height * Scale.Y)
            End Get
        End Property

        Public Property IsRemoved As Boolean
        Public Property ShowDebug As Boolean Implements IPoolable.ShowDebug

        Public Sub New()

        End Sub

        Public Sub New(textureS As Texture2D, height As Integer)
            DespawnHeight = height
            Texture = textureS
        End Sub

        Public Sub Update(gameTime As GameTime, cam As Rectangle)
            Position += Velocity * gameTime.ElapsedGameTime.TotalSeconds * 60
            If Flex Then FinalPosition = New Vector2(Position.X + Math.Ceiling((cam.Left - Position.X) / cam.Width) * cam.Width, Position.Y) Else FinalPosition = Position

            If (Position.Y - Origin.Y > DespawnHeight) Then IsRemoved = True
        End Sub

        Public Sub Draw(gameTime As GameTime)
            SpriteBat.Draw(Texture, FinalPosition, Nothing, Color * Opacity, Rotation, Origin, Scale, SpriteEffects.None, Layer)
        End Sub

        Public Function Clone() As Particle
            Return Me.MemberwiseClone
        End Function
    End Class

End Namespace