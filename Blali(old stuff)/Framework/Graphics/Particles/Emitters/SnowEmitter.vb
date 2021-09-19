
Imports Microsoft.Xna.Framework

Namespace Framework.Graphics.Effects.Particles.Emmiters

    <TestState(TestState.WorkInProgress)>
    Public Class SnowEmitter
        Inherits Emitter

        Public Property GlobalVelocitySpeed As Single = 0.8

        Dim _swayTimer As Single
        Dim _spawnHeight As Integer
        Dim rnd As Random
        Dim _speeddiv As Integer

        Public Sub New(particle As Particle, spawnHeight As Integer, Optional speeddiv As Integer = 65)
            MyBase.New(particle, 2200)

            GenerateSpeed = 100
            _spawnHeight = spawnHeight
            _speeddiv = speeddiv
            rnd = New Random
        End Sub

        Protected Overrides Sub ApplyParticleRefresh(gameTime As GameTime)

            _swayTimer += gameTime.ElapsedGameTime.TotalSeconds

            If _swayTimer > GlobalVelocitySpeed Then
                _swayTimer = 0
                Dim xSway As Single = rnd.Next(-2, 2)
                For Each particle In _particles
                    particle.Velocity.X = (xSway * particle.Scale.X) / _speeddiv
                Next
            End If
        End Sub

        Protected Overrides Function GenerateParticle(instance As Particle) As Particle
            Dim particle As Particle = instance

            Dim xPosition As Integer = rnd.Next(0, GameSize.X)
            Dim ySpeed As Single = rnd.Next(10, 100) / 40

            particle.Texture = _particlePrefab.Texture
            particle.Origin = _particlePrefab.Origin
            particle.DespawnHeight = _particlePrefab.DespawnHeight
            particle.Position.X = xPosition
            particle.Position.Y = _spawnHeight - particle.Texture.Height * particle.Scale.Y
            particle.Opacity = rnd.NextDouble
            particle.Rotation = MathHelper.ToRadians(rnd.Next(0, 3))
            particle.Layer = Math.Min(rnd.NextDouble, 0.94)
            particle.Scale = New Vector2(particle.Layer + rnd.Next(0, 3))
            particle.Velocity.X = 0
            particle.Velocity.Y = ySpeed
            particle.Flex = True

            Return particle
        End Function
    End Class

End Namespace