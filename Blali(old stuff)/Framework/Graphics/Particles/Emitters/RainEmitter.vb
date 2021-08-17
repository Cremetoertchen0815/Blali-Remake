
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics.Effects.Particles.Emmiters

    <TestState(TestState.WorkInProgress)>
    Public Class RainEmitter
        Inherits Emitter

        Public Property GlobalVelocitySpeed As Single = 0.8

        Dim _swayTimer As Single
        Dim _spawnHeight As Integer
        Dim rnd As Random
        Dim _speeddiv As Integer

        Dim txt As Texture2D()

        Public Sub New(particle As Particle, spawnHeight As Integer, Optional speeddiv As Integer = 65)
            MyBase.New(particle, 2200)

            GenerateSpeed = 40
            SpawnAmplifier = 18
            _spawnHeight = spawnHeight
            _speeddiv = speeddiv
            rnd = New Random

            txt = {ContentMan.Load(Of Texture2D)("fx\textures\particles\particle_rain_1"), ContentMan.Load(Of Texture2D)("fx\textures\particles\particle_rain_2"), ContentMan.Load(Of Texture2D)("fx\textures\particles\particle_rain_3")}
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
            Dim ySpeed As Single = rnd.Next(15, 25)

            particle.Texture = txt(rnd.Next(0, 2))
            particle.Origin = _particlePrefab.Origin
            particle.DespawnHeight = _particlePrefab.DespawnHeight
            particle.Position.X = xPosition
            particle.Position.Y = _spawnHeight - particle.Texture.Height * particle.Scale.Y
            particle.Opacity = rnd.Next(50, 100) / 100
            particle.Rotation = 0
            particle.Layer = Math.Min(rnd.NextDouble, 0.94)
            particle.Scale = New Vector2(rnd.Next(1, 4))
            particle.Velocity.X = 0
            particle.Velocity.Y = ySpeed
            particle.Flex = True

            Return particle
        End Function
    End Class

End Namespace