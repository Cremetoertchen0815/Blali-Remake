Imports System
Imports System.Collections.Generic
Imports Microsoft.Xna.Framework.Audio

Namespace Intros
    Public Class LF
        Inherits Scene

        Protected border_rnd As Sprites.SpriteRenderer
        Protected psy_rnd As LFIntroRenderable
        Protected txt_rnd As Sprites.SpriteRenderer

        Dim end_trigger As Boolean = False
        Dim timer As Single
        Dim skipper As VirtualButton

        Public Overrides Sub Initialize()
            MyBase.Initialize()
            skipper = New VirtualButton(New VirtualButton.KeyboardKey(Keys.Enter))

            Dim psy = AddRenderer(New PsygroundRenderer(1) With {.Source = New Rectangle(270, 240, 520, 520), .RenderTexture = New Textures.RenderTexture, .StencilTexture = Content.LoadTexture("intro/lf/stencil")})
            Dim rnd = AddRenderer(New LfIntroRenderer(0, Me))
            AddRenderer(New DefaultRenderer(2))

            'WHY THE HECK DO THE COLORS SWITCH?
            'Is it a broken Tween? Is it my shader? I have absolutely no clue...
            'Oh well, now it's an easteregg I guess x3
            psy_rnd = CreateEntity("psy").AddComponent(New LFIntroRenderable(psy) With {.Alpha = 0})
            CreateEntity("rnd").AddComponent(New LFIntroRenderable(rnd))
            txt_rnd = CreateEntity("text").SetLocalPosition(New Vector2(1920 / 2, 1080 / 2)).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("intro/lf/overlay_text")) With {.Color = Color.Transparent})
            ClearColor = Color.Black

            border_rnd = CreateEntity("border").SetPosition(New Vector2(530, 500)).SetScale(2).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("intro/lf/border")) With {.Color = Color.Transparent})
        End Sub

        Public Overrides Sub Update()

            'Transition to new scene
            timer += Time.DeltaTime
            If timer > 7 And Not end_trigger Then
                end_trigger = True
                Core.StartSceneTransition(New FadeTransition(Function() New MonoNez) With {.FadeInDuration = 1, .FadeOutDuration = 2, .FadeToColor = Color.Black, .FadeEaseType = Tweens.EaseType.QuadInOut})
            ElseIf skipper.IsPressed And Not end_trigger Then
                end_trigger = True
                Core.StartSceneTransition(New FadeTransition(Function() New MonoNez) With {.FadeInDuration = 1, .FadeOutDuration = 0.5, .FadeToColor = Color.Black, .FadeEaseType = Tweens.EaseType.QuadInOut})
            End If

            MyBase.Update()
        End Sub

        Protected Class LFIntroRenderable
            Inherits RenderableComponent

            Dim Rrnd As Renderer
            Public Property Alpha As Single = 1.0F
            Sub New(rnd As Renderer)
                Rrnd = rnd
            End Sub
            Public Overrides Sub Render(batcher As Batcher, camera As Camera)
                batcher.Draw(Rrnd.RenderTexture, New Rectangle(0, 0, 1920, 1080), Color.White * Alpha)
            End Sub

            Public Overrides ReadOnly Property Bounds As RectangleF
                Get
                    Return New RectangleF(0, 0, 1920, 1080)
                End Get
            End Property
        End Class

        Private Class LfIntroRenderer
            Inherits Renderer

            'Rendering & assets
            Private figur_model As Model
            Private batchlor As Batcher
            Private dev As GraphicsDevice

            'Virtaul playing field
            Private EffectA As BasicEffect
            Private MapBuffer As VertexBuffer
            Private SpielfeldTexture As RenderTarget2D

            'Matrices
            Private View As Matrix
            Private Projection As Matrix

            'Animation
            Private figureXZ As Vector2 = New Vector2(-600, 200)
            Private figureY As Single = 0
            Private playfield_alpha As Single = 1.0F
            Private field_offset As Integer = -50
            Private ow As LF

            'Sounds
            Private jmp As SoundEffect
            Private kill As SoundEffect

            Sub New(order As Integer, owner As LF)
                MyBase.New(order)
                ow = owner
            End Sub

            Public Overrides Sub OnAddedToScene(scene As Scene)
                MyBase.OnAddedToScene(scene)

                dev = Core.GraphicsDevice

                figur_model = scene.Content.Load(Of Model)("intro\lf\piece_std")
                batchlor = New Batcher(dev)
                View = Matrix.CreateScale(1, 1, 1 / 1080) * Matrix.CreateLookAt(New Vector3(0, 0, -1), New Vector3(0, 0, 0), Vector3.Up)
                Projection = Matrix.CreateScale(100) * Matrix.CreatePerspective(800, 480, 1, 100000)

                'Set up rest of the vistual playing field
                Dim vertices As New List(Of VertexPositionColorTexture) From {
                New VertexPositionColorTexture(New Vector3(-475, 475, 0), Color.White, Vector2.UnitX),
                New VertexPositionColorTexture(New Vector3(475, 475, 0), Color.White, Vector2.Zero),
                New VertexPositionColorTexture(New Vector3(-475, -475, 0), Color.White, Vector2.One),
                New VertexPositionColorTexture(New Vector3(475, 475, 0), Color.White, Vector2.Zero),
                New VertexPositionColorTexture(New Vector3(475, -475, 0), Color.White, Vector2.UnitY),
                New VertexPositionColorTexture(New Vector3(-475, -475, 0), Color.White, Vector2.One)
            }
                MapBuffer = New VertexBuffer(dev, GetType(VertexPositionColorTexture), vertices.Count, BufferUsage.WriteOnly)
                MapBuffer.SetData(vertices.ToArray)

                EffectA = New BasicEffect(dev) With {.Alpha = 1.0F,
            .VertexColorEnabled = True,
            .LightingEnabled = False,
            .TextureEnabled = True
        }
                SpielfeldTexture = New RenderTarget2D(
            dev,
            950,
            950,
            False,
            dev.PresentationParameters.BackBufferFormat,
            DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents) With {.Name = "TmpA"}

                'Load sound effects
                jmp = scene.Content.LoadSoundEffect("intro/lf/jump")
                kill = scene.Content.LoadSoundEffect("intro/lf/land")

                'Set up figure model
                Dim cc As Color = Color.Magenta
                Dim element As BasicEffect = CType(figur_model.Meshes(0).Effects(0), BasicEffect)
                element.VertexColorEnabled = False
                element.TextureEnabled = False
                element.DiffuseColor = Color.White.ToVector3
                element.LightingEnabled = True '// turn on the lighting subsystem.
                element.PreferPerPixelLighting = True
                element.AmbientLightColor = Vector3.Zero
                element.EmissiveColor = cc.ToVector3 * 0.12
                element.DirectionalLight0.Direction = New Vector3(0, 0.8, 1.5)
                element.DirectionalLight0.DiffuseColor = cc.ToVector3 * 0.6 '// a gray light
                element.DirectionalLight0.SpecularColor = New Vector3(1, 1, 1) '// with white highlights
                element.View = View
                element.Projection = Projection

                RenderTexture = New Textures.RenderTexture()

                'Launch animation
                Core.Schedule(0.5F, Sub()
                                        Core.Schedule(0.6F, Sub() jmp.Play())
                                        Tween("figureY", 130.0F, 0.3F).SetEaseType(Tweens.EaseType.QuadOut).SetLoops(Tweens.LoopType.PingPong, 3).Start()
                                        Tween("figureXZ", New Vector2(-400, 0), 0.58F).SetEaseType(Tweens.EaseType.Linear).SetCompletionHandler(Sub(x)
                                                                                                                                                    x.Stop(True)
                                                                                                                                                    Core.Schedule(0.6F, Sub() jmp.Play())
                                                                                                                                                    Tween("figureXZ", New Vector2(-200, -200), 0.58F).SetEaseType(Tweens.EaseType.Linear).SetCompletionHandler(Sub(y)
                                                                                                                                                                                                                                                                   y.Stop(True)
                                                                                                                                                                                                                                                                   Core.Schedule(0.6F, Sub() jmp.Play())
                                                                                                                                                                                                                                                                   Tween("figureXZ", New Vector2(0, -400), 0.58F).SetEaseType(Tweens.EaseType.Linear).Start()
                                                                                                                                                                                                                                                               End Sub).Start()
                                                                                                                                                End Sub).Start()
                                    End Sub)

                Core.Schedule(2.3F, Sub() Tween("playfield_alpha", 0F, 1.0F).SetEaseType(Tweens.EaseType.Linear).Start())

                Core.Schedule(3.0F, Sub()
                                        Tween("figureXZ", New Vector2(80, -600), 1.0F).SetEaseType(Tweens.EaseType.Linear).Start()
                                        Tween("figureY", 400.0F, 0.5F).SetEaseType(Tweens.EaseType.QuadOut).SetLoops(Tweens.LoopType.PingPong).Start()
                                        ow.border_rnd.TweenColorTo(Color.White, 1.0F).SetEaseType(Tweens.EaseType.QuadIn).Start()
                                        ow.psy_rnd.Tween("Alpha", 1.0F, 1.0F).SetEaseType(Tweens.EaseType.QuadIn).Start()
                                        Core.Schedule(1.0F, Sub() kill.Play())
                                    End Sub)

                Core.Schedule(3.5F, Sub()
                                        Dim elementB As BasicEffect = CType(figur_model.Meshes(0).Effects(0), BasicEffect)
                                        element.LightingEnabled = False '// turn off the lighting subsystem.
                                        field_offset = -30
                                    End Sub)

                Core.Schedule(4.5F, Sub() ow.txt_rnd.TweenColorTo(Color.White, 1.0F).SetEaseType(Tweens.EaseType.QuadInOut).Start())
            End Sub

            Public Overrides Sub Render(scene As Scene)

                dev.SetRenderTarget(SpielfeldTexture)
                dev.Clear(Color.Black)

                batchlor.Begin()
                batchlor.DrawCircle(New Vector2(870, 480), 24, Color.White, 5)
                batchlor.DrawCircle(New Vector2(700, 570), 24, Color.White, 5)
                batchlor.DrawCircle(New Vector2(475, 675), 24, Color.White, 5)
                batchlor.End()

                dev.SetRenderTarget(RenderTexture)
                dev.Clear(Color.Transparent)

                dev.RasterizerState = RasterizerState.CullNone
                dev.DepthStencilState = DepthStencilState.Default

                'Render map
                EffectA.World = Matrix.CreateRotationX(1.5) * Matrix.CreateTranslation(0, -50, 0)
                EffectA.View = View
                EffectA.Projection = Projection
                EffectA.Alpha = playfield_alpha
                EffectA.TextureEnabled = True
                EffectA.Texture = SpielfeldTexture

                For Each pass As EffectPass In EffectA.CurrentTechnique.Passes
                    dev.SetVertexBuffer(MapBuffer)
                    pass.Apply()

                    dev.DrawPrimitives(PrimitiveType.TriangleList, 0, MapBuffer.VertexCount)
                Next

                CType(figur_model.Meshes(0).Effects(0), BasicEffect).World = Matrix.CreateScale(3 * New Vector3(1, 1, 1)) * Matrix.CreateRotationY(Math.PI * 0.5) * Matrix.CreateRotationZ(Math.PI * 0.5) * Matrix.CreateTranslation(New Vector3(figureXZ.X, figureY + field_offset, figureXZ.Y))
                figur_model.Meshes(0).Draw()

            End Sub
        End Class

        Public Class PsygroundRenderer
            Inherits Renderer

            'Vertex & index buffers
            Private vertexlist As VertexPositionColorTexture()
            Private indexlist As Integer()
            Private vertexbuffer As DynamicVertexBuffer
            Private indexbuffer As IndexBuffer
            Private Effect As Effect
            Private Dev As GraphicsDevice

            'Matrices
            Private World As Matrix
            Private View As Matrix
            Private Projection As Matrix

            'Color faders
            Private faderA As Color
            Private faderB As Color
            Private faderC As Color
            Private faderD As Color

            'WAT
            Private pixl As Texture2D

            'Other junk
            Private loops As Boolean = True

            'Properties
            Public Property Size As Vector2 = New Vector2(1920, 1080)
            Public Property Source As New Rectangle(Vector2.Zero.ToPoint, Size.ToPoint)
            Public Property Colors As Color() = {Color.Blue, Color.DarkCyan, Color.Purple, Color.Black, Color.Green}
            Public Property Speed As Integer = 3
            Public Property StencilTexture As Texture2D = Graphics.Instance.PixelTexture
            Public Property Alpha As Single = 1.0F

            Sub New(order As Integer)
                MyBase.New(order)
            End Sub

            Public Overrides Sub OnAddedToScene(scene As Scene)
                MyBase.OnAddedToScene(scene)
                World = Matrix.Identity

                Dev = Core.GraphicsDevice

                Effect = New BasicEffect(Dev)
                pixl = New Texture2D(Dev, 1, 1)
                pixl.SetData({Color.White})

                View = Matrix.CreateLookAt(New Vector3(0, 0, -1), New Vector3(0, 0, 0), New Vector3(0, -10, 0))
                Projection = Matrix.CreateScale(1, -1, 1) * Matrix.CreatePerspectiveOffCenter(0, Size.X, Size.Y, 0, 1, 999)

                'Generate index buffer
                vertexbuffer = New DynamicVertexBuffer(Dev, GetType(VertexPositionColorTexture), 4, BufferUsage.WriteOnly)
                indexlist = {0, 1, 2, 0, 2, 3}
                indexbuffer = New IndexBuffer(Dev, IndexElementSize.ThirtyTwoBits, indexlist.Length, BufferUsage.WriteOnly)
                indexbuffer.SetData(indexlist)

                Effect = scene.Content.LoadEffect("intro/lf/stencil_fx.mgfxo")

                faderA = RndColor()
                faderB = RndColor()
                faderC = RndColor()
                faderD = RndColor()
                loops = True
                LaunchColorFaders()
            End Sub

            Public Overrides Sub Render(scene As Scene)
                Effect.Parameters("World").SetValue(World)
                Effect.Parameters("View").SetValue(View)
                Effect.Parameters("Projection").SetValue(Projection)
                Effect.Parameters("SourceTexture").SetValue(StencilTexture)
                Effect.Parameters("SpriteTexture").SetValue(pixl)
                Effect.Parameters("alpha").SetValue(Alpha)

                vertexlist = {New VertexPositionColorTexture(New Vector3(Source.Left, Source.Top, 0), faderA, New Vector2(0, 0)),
                          New VertexPositionColorTexture(New Vector3(Source.Right, Source.Top, 0), faderB, New Vector2(1, 0)),
                          New VertexPositionColorTexture(New Vector3(Source.Right, Source.Bottom, 0), faderC, New Vector2(1, 1)),
                          New VertexPositionColorTexture(New Vector3(Source.Left, Source.Bottom, 0), faderD, New Vector2(0, 1))}

                vertexbuffer.SetData(vertexlist)

                Dev.SetRenderTarget(RenderTexture)
                Dev.Clear(Color.Transparent)
                Dev.BlendState = BlendState.AlphaBlend

                For Each element In Effect.CurrentTechnique.Passes
                    Dev.SetVertexBuffer(vertexbuffer)
                    Dev.Indices = indexbuffer

                    element.Apply()

                    Dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4)
                Next
            End Sub

            Public Overrides Sub Unload()
                MyBase.Unload()
                loops = False
            End Sub


            Private Sub LaunchColorFaders()
                If loops Then
                    Tween("faderA", RndColor, Speed).SetEaseType(Tweens.EaseType.Linear).SetCompletionHandler(AddressOf LaunchColorFaders).Start()
                    Tween("faderB", RndColor, Speed).SetEaseType(Tweens.EaseType.Linear).Start()
                    Tween("faderC", RndColor, Speed).SetEaseType(Tweens.EaseType.Linear).Start()
                    Tween("faderD", RndColor, Speed).SetEaseType(Tweens.EaseType.Linear).Start()
                End If
            End Sub

            Private Function RndColor() As Color
                Return Colors(Nez.Random.Range(0, Colors.Length))
            End Function
        End Class
    End Class
End Namespace