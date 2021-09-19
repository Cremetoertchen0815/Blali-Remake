Imports System.Collections.Generic
Imports Microsoft.Xna.Framework

Namespace Framework.Graphics.Effects.Particles

    <TestState(TestState.NearCompletion)>
    Public MustInherit Class Emitter
        Private _generateTimer As Single
        Protected _particlePrefab As Particle
        Protected _particles As List(Of Particle)
        Protected _particlepool As ObjectPool(Of Particle)

        Public Property SpawnAmplifier As Integer = 1
        Public Property GenerateSpeed As Single = 5.0F
        Public Property MaxParticles As Integer = 1000

        Public Sub New(particle As Particle, MaxParticless As Integer)
            _particlePrefab = particle
            _particles = New List(Of Particle)
            MaxParticles = MaxParticless
            _particlepool = New ObjectPool(Of Particle)(MaxParticles)
        End Sub

        Dim partel As Particle
        Public Sub Update(gameTime As GameTime, cam As Rectangle, ByRef IsParticleDone As Boolean)
            _generateTimer += gameTime.ElapsedGameTime.TotalMilliseconds

            'Generate new particles
            AddParticle()

            'Refresh the existing particles
            ApplyParticleRefresh(gameTime)

            For i As Integer = 0 To _particles.Count - 1
                partel = _particles(i)
                partel.Update(gameTime, cam)
                If partel.PoolIsValid Then IsParticleDone = True
            Next

            RemovedFinishedParticles()
        End Sub




        Private Sub AddParticle()
            If _generateTimer > GenerateSpeed Then
                _generateTimer = 0
                For i As Integer = 0 To SpawnAmplifier - 1
                    If _particles.Count < MaxParticles Then
                        _particles.Add(GenerateParticle(_particlepool.Get))
                    End If
                Next
            End If
        End Sub

        Protected MustOverride Sub ApplyParticleRefresh(gameTime As GameTime)

        Dim despawnlist As New List(Of Particle)
        Private Sub RemovedFinishedParticles()
            despawnlist.Clear()

            For i As Integer = 0 To _particles.Count - 1
                partel = _particles(i)
                If partel.IsRemoved Then
                    despawnlist.Add(partel)
                    _particlepool.Release(partel)
                End If
            Next

            For Each element In despawnlist
                _particles.Remove(element)
            Next
        End Sub

        Public ReadOnly Property GetParticleCount As Integer
            Get
                Return _particles.Count
            End Get
        End Property

        Protected MustOverride Function GenerateParticle(instance As Particle) As Particle

        Public Sub Draw(gameTime As GameTime)

            For i As Integer = 0 To _particles.Count - 1
                _particles(i).Draw(gameTime)
            Next
        End Sub
    End Class

End Namespace