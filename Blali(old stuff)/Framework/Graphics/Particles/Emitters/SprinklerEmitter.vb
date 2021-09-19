
Imports Microsoft.Xna.Framework

Namespace Framework.Graphics.Effects.Particles.Emmiters

    <TestState(TestState.WorkInProgress)>
    Public Class SprinklerEmitter
        Inherits Emitter

        Public spawnPoint As Vector2
        Public Mode As Integer = 0
        Public angle As Double = 90
        Public angleoffet As Double
        Dim _gravity As Double = 0.1
        Dim _maxvector As Double = 10
        Dim _spawnenergy As Double = 7
        Dim rnd As New Random

        Sub New(baseparticle As Particle, spawn As Vector2, angleDeg As Integer)
            MyBase.New(baseparticle, 10000)
            spawnPoint = spawn
            angle = angleDeg
            GenerateSpeed = 12
            SpawnAmplifier = 100
        End Sub

        Protected Overrides Sub ApplyParticleRefresh(gameTime As GameTime)
            For Each particle In _particles
                particle.Velocity.Y += _gravity * gameTime.ElapsedGameTime.TotalMilliseconds / 20
                particle.Color = Color.Lerp(particle.Color, particle.DualColor, 0.1)
            Next
        End Sub

        Protected Overrides Function GenerateParticle(instance As Particle) As Particle
            Dim newparticle As Particle = instance
            Dim ang As Double = MathHelper.ToRadians(rnd.Next(0, angle * 2) - angle - angleoffet)

            newparticle.Texture = _particlePrefab.Texture
            newparticle.Origin = _particlePrefab.Origin
            newparticle.DespawnHeight = _particlePrefab.DespawnHeight
            newparticle.Position = spawnPoint
            newparticle.Layer = Math.Min(rnd.NextDouble, 0.94)
            newparticle.Opacity = 1
            newparticle.Scale.X = 1
            newparticle.Flex = False
            newparticle.Velocity.X = Math.Cos(ang) * _spawnenergy
            newparticle.Velocity.Y = Math.Sin(ang) * _spawnenergy
            newparticle.Color = Color.Red
            Select Case Mode
                Case 0
                    newparticle.DualColor = New Color(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256))
                Case 1
                    newparticle.DualColor = Color.Red
                Case 2
                    newparticle.DualColor = Color.Lime
                Case 3
                    newparticle.DualColor = Color.Cyan
                Case 4
                    newparticle.DualColor = Color.White
                Case 5
                    If rnd.Next(0, 2) = 1 Then
                        newparticle.DualColor = Color.Cyan
                    Else
                        newparticle.DualColor = Color.Red
                    End If
                Case 6
                    If rnd.Next(0, 2) = 1 Then
                        newparticle.DualColor = Color.Green
                    Else
                        newparticle.DualColor = Color.Purple
                    End If
            End Select

            Return newparticle
        End Function
    End Class
End Namespace