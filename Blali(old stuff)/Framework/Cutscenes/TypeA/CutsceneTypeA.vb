Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Xml
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Media

Namespace Framework.Cutscenes.TypeA

    <TestState(TestState.WorkInProgress)>
    Public Class CutsceneTypeA
        Public Property Loading As LoadType = LoadType.FrameAfterFrame
        Public Property Description As String = "None"
        Public Property MusicPath As String = ""
        Public Property Song As Song
        Public Property TextureBuffer As New List(Of Texture2D)
        Public Property FrameBuffer As New List(Of Frame)
        'Camera
        Public CamPos As CamKeyframe
        Dim currentframe As Integer = -1
        Dim aimframe As Integer

        'Matrices
        Dim camUp As Vector3
        Dim World As Matrix
        Dim View As Matrix
        Dim Projection As Matrix
        Dim camera2Drotation As Single = 0F

        'Rendering
        Dim Dev As GraphicsDevice
        Dim DebugEffect As Effect
        Friend Effect As Effect
        Dim Stencil As DepthStencilState = DepthStencilState.DepthRead

        'Debug Box Buffer
        Dim BoxVertexBuffer As VertexBuffer
        Dim BoxIndexBuffer As IndexBuffer
        Private Sub CreateBoxBuffer()

            '---VERTICES---
            Dim genboxvert = New VertexPositionColor(23) {}
            'Left|Top|Front
            genboxvert(0) = New VertexPositionColor(New Vector3(0, 0, 0), Color.Lime)
            genboxvert(1) = New VertexPositionColor(New Vector3(20, 20, 0), Color.Lime)
            genboxvert(2) = New VertexPositionColor(New Vector3(0, 20, 0), Color.Lime)
            'Right|Top|Front
            genboxvert(3) = New VertexPositionColor(New Vector3(1920, 0, 0), Color.Lime)
            genboxvert(4) = New VertexPositionColor(New Vector3(1920, 20, 0), Color.Lime)
            genboxvert(5) = New VertexPositionColor(New Vector3(1900, 20, 0), Color.Lime)
            'Left|Bottom|Front
            genboxvert(6) = New VertexPositionColor(New Vector3(1900, 1060, 0), Color.Lime)
            genboxvert(7) = New VertexPositionColor(New Vector3(1920, 1060, 0), Color.Lime)
            genboxvert(8) = New VertexPositionColor(New Vector3(1920, 1080, 0), Color.Lime)
            'Right|Bottom|Front
            genboxvert(9) = New VertexPositionColor(New Vector3(0, 1080, 0), Color.Lime)
            genboxvert(10) = New VertexPositionColor(New Vector3(0, 1060, 0), Color.Lime)
            genboxvert(11) = New VertexPositionColor(New Vector3(20, 1060, 0), Color.Lime)
            'Left|Top|Back
            genboxvert(12) = New VertexPositionColor(New Vector3(0, 0, 1000), Color.Magenta)
            genboxvert(13) = New VertexPositionColor(New Vector3(20, 20, 1000), Color.Magenta)
            genboxvert(14) = New VertexPositionColor(New Vector3(0, 20, 1000), Color.Magenta)
            'Right|Top|Back
            genboxvert(15) = New VertexPositionColor(New Vector3(1920, 0, 1000), Color.Magenta)
            genboxvert(16) = New VertexPositionColor(New Vector3(1920, 20, 1000), Color.Magenta)
            genboxvert(17) = New VertexPositionColor(New Vector3(1900, 20, 1000), Color.Magenta)
            'Left|Bottom|Back
            genboxvert(18) = New VertexPositionColor(New Vector3(1900, 1060, 1000), Color.Magenta)
            genboxvert(19) = New VertexPositionColor(New Vector3(1920, 1060, 1000), Color.Magenta)
            genboxvert(20) = New VertexPositionColor(New Vector3(1920, 1080, 1000), Color.Magenta)
            'Right|Bottom|Back
            genboxvert(21) = New VertexPositionColor(New Vector3(0, 1080, 1000), Color.Magenta)
            genboxvert(22) = New VertexPositionColor(New Vector3(0, 1060, 1000), Color.Magenta)
            genboxvert(23) = New VertexPositionColor(New Vector3(20, 1060, 1000), Color.Magenta)


            '---INDICES---
            Dim genboxind As Integer() = {0, 3, 4,
                                          0, 4, 2,
                                          5, 4, 7,
                                          5, 7, 6,
                                          10, 7, 8,
                                          10, 8, 9,
                                          2, 1, 11,
                                          2, 11, 10,
                                          12, 15, 16,
                                          12, 16, 14,
                                          17, 16, 19,
                                          17, 19, 18,
                                          22, 19, 20,
                                          22, 20, 21,
                                          14, 13, 23,
                                          14, 23, 22,
                                          12, 0, 2, 'Side Left
                                          12, 2, 14,
                                          22, 10, 9,
                                          22, 9, 21,
                                          3, 15, 16,
                                          3, 16, 4,
                                          7, 19, 20,
                                          7, 20, 8}

            BoxVertexBuffer = New VertexBuffer(dev, GetType(VertexPositionColor), genboxvert.Length, BufferUsage.WriteOnly)
            BoxVertexBuffer.SetData(genboxvert)

            BoxIndexBuffer = New IndexBuffer(Dev, GetType(Integer), genboxind.Length, BufferUsage.WriteOnly)
            BoxIndexBuffer.SetData(genboxind)
        End Sub
        Public Sub Init()

            Dev = StandardAssets.Graphx.GraphicsDevice
            If Loading = LoadType.Immediate Then
                For Each element In FrameBuffer
                    element.Load(TextureBuffer)
                Next
            End If

            Effect = ContentMan.Load(Of Effect)("fx\fx_quad")
            DebugEffect = ContentMan.Load(Of Effect)("fx\fx_debugquad")
            Stencil = New DepthStencilState With {.DepthBufferEnable = True, .DepthBufferFunction = CompareFunction.Less}


            camUp = Vector3.Transform(New Vector3(0, -1, 0), Matrix.CreateRotationZ(camera2Drotation)) * 10.0F
            View = Matrix.CreateScale(1, 1, 1 / 1080) * Matrix.CreateLookAt(New Vector3(0, 0, -1), New Vector3(0, 0, 0), camUp)
            Projection = Matrix.CreateScale(100) * Matrix.CreatePerspective(Dev.Viewport.Width, Dev.Viewport.Height, 1, 100000)
            CreateBoxBuffer()
        End Sub

        Public Sub ActivateFrame(nr As Integer)
            If nr >= 0 And nr < FrameBuffer.Count Then
                currentframe = nr
                If Loading = LoadType.FrameAfterFrame Then
                    FrameBuffer(currentframe).Load(TextureBuffer)
                End If
                For Each field In FrameBuffer(currentframe)
                    field.UpdateBuffers(StandardAssets.Graphx.GraphicsDevice)
                Next
            ElseIf nr < 0 Then
                currentframe = nr
            End If
        End Sub

        Public Sub Draw(gameTime As GameTime)
            Dev.BlendState = BlendState.AlphaBlend
            Dev.SamplerStates(0) = SamplerState.AnisotropicClamp
            Dev.RasterizerState = RasterizerState.CullNone

            If currentframe >= 0 Then
                For i As Integer = 0 To FrameBuffer(currentframe).Count - 1
                    Dim f As Field = FrameBuffer(currentframe)(i)
                    If f.Disable Then Continue For

                    Effect.Parameters("s0").SetValue(f.Texture)
                    Effect.Parameters("alpha").SetValue(f.Alpha)
                    Effect.Parameters("tint").SetValue(Color.White.ToVector4)

                    For Each t In Effect.CurrentTechnique.Passes
                        t.Apply()
                        Dev.SetVertexBuffer(f.VertexBuffer)
                        Dev.Indices = f.IndexBuffer

                        Dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, f.Indices.Length / 3)
                    Next
                Next

            End If
        End Sub

        Public Sub DrawDebug()
            DebugEffect.Parameters("World").SetValue(World)
            DebugEffect.Parameters("View").SetValue(View)
            DebugEffect.Parameters("Projection").SetValue(Projection)

            For Each t In DebugEffect.CurrentTechnique.Passes
                t.Apply()
                Dev.SetVertexBuffer(BoxVertexBuffer)
                Dev.Indices = BoxIndexBuffer

                Dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, BoxIndexBuffer.IndexCount / 3)
            Next
        End Sub


        Public Sub Update(gameTime As GameTime)

            'If currentframe >= 0 Then
            '    For Each element In FrameBuffer(currentframe)
            '        element.Update(gameTime, cam.Location)
            '    Next
            'End If
            World = Matrix.CreateFromYawPitchRoll(CamPos.Yaw, CamPos.Pitch, CamPos.Roll) * Matrix.CreateTranslation(CamPos.Location - New Vector3(GameSize.X / 2, GameSize.Y / 2, 0))
        End Sub

        Public Sub Start(enabledepthbuffer As Boolean)

            If enabledepthbuffer Then Dev.DepthStencilState = Stencil Else Dev.DepthStencilState = DepthStencilState.None
            Dev.BlendState = BlendState.AlphaBlend
            Dev.SamplerStates(0) = SamplerState.LinearClamp
            Dev.RasterizerState = RasterizerState.CullNone

            Effect.Parameters("World").SetValue(World)
            Effect.Parameters("View").SetValue(View)
            Effect.Parameters("Projection").SetValue(Projection)
        End Sub

        Public Sub SaveToFile(filename As String)
            Dim sb As StringBuilder

            '--------xxx_dsc.ele--------
            sb = New StringBuilder
            'Write header
            sb.AppendLine("<?xml version=""1.0""?>")
            sb.AppendLine("<cutscene description=""" & Description & """ load=""" & Loading.ToString & """ bgm=""" & MusicPath.ToString & """>")
            sb.AppendLine("<texturebuffer>")
            For i As Integer = 0 To TextureBuffer.Count - 1
                Dim tex As Texture2D = TextureBuffer(i)
                sb.AppendLine("<obj name=""" & tex.Name & """/>")
            Next
            sb.AppendLine("</texturebuffer>")
            sb.AppendLine("<framebuffer>")
            For i As Integer = 0 To FrameBuffer.Count - 1
                Dim frame As Frame = FrameBuffer(i)
                sb.AppendLine("<frame description=""" & frame.Description & """ duration=""" & frame.WaitTime & """ blackout=""" & frame.Blackout & """ fadetime=""" & frame.FadeTime & """ trans=""" & frame.MovementTransition.ToString & """>")
                sb.AppendLine("<keyframes>")
                sb.AppendLine("<start x=""" & frame.StartPos.X & """ y=""" & frame.StartPos.Y & """ z=""" & frame.StartPos.Z & """ w=""" & frame.StartPos.Yaw & """ p=""" & frame.StartPos.Pitch & """ r=""" & frame.StartPos.Roll & """/>")
                sb.AppendLine("<end x=""" & frame.EndPos.X & """ y=""" & frame.EndPos.Y & """ z=""" & frame.EndPos.Z & """ w=""" & frame.EndPos.Yaw & """ p=""" & frame.EndPos.Pitch & """ r=""" & frame.EndPos.Roll & """/>")
                sb.AppendLine("</keyframes>")
                For j As Integer = 0 To frame.Count - 1
                    Dim field As Field = frame(j)
                    sb.AppendLine("<field description=""" & field.Description & """ depth=""" & field.Depth & """ texture=""" & field.TextureNr & """ vertices=""" & field.Vertices.Length & """ indices=""" & field.Indices.Length & """>")
                    For k As Integer = 0 To field.Vertices.Length - 1
                        Dim vertex As VertexPositionColorTexture = field.Vertices(k)
                        sb.AppendLine("<vertex id=""" & k & """ x=""" & vertex.Position.X & """ y=""" & vertex.Position.Y & """ z=""" & vertex.Position.Z & """ r=""" & vertex.Color.R & """ g=""" & vertex.Color.G &
                                      """ b=""" & vertex.Color.B & """ a=""" & vertex.Color.A & """ UVx=""" & vertex.TextureCoordinate.X & """ UVy=""" & vertex.TextureCoordinate.Y & """/>")
                    Next
                    For k As Integer = 0 To (field.Indices.Length / 3) - 1
                        sb.AppendLine("<tri id=""" & k & """ A=""" & field.Indices((k * 3) + 0) & """ B=""" & field.Indices((k * 3) + 1) & """ C=""" & field.Indices((k * 3) + 2) & """/>")
                    Next
                    sb.AppendLine("</field>")
                Next

                sb.AppendLine("</frame>")
            Next
            sb.AppendLine("</framebuffer>")
            'Write footer
            sb.AppendLine("</cutscene>")
            'Save file
            IO.File.WriteAllText(filename, sb.ToString)
        End Sub

        Public Shared Function Load(filename As String) As CutsceneTypeA
            Dim res As New CutsceneTypeA
            Dim m_xmld_tls As XmlDocument
            m_xmld_tls = New XmlDocument
            m_xmld_tls.Load(filename)
            res.Description = m_xmld_tls.SelectNodes("/cutscene")(0).Attributes.GetNamedItem("description").Value
            res.MusicPath = m_xmld_tls.SelectNodes("/cutscene")(0).Attributes.GetNamedItem("bgm").Value
            res.Loading = DirectCast([Enum].Parse(GetType(LoadType), m_xmld_tls.SelectNodes("/cutscene")(0).Attributes.GetNamedItem("load").Value), LoadType)
            For Each element As XmlNode In m_xmld_tls.SelectNodes("/cutscene")(0).ChildNodes
                Select Case element.Name.ToLower
                    Case "framebuffer"
                        For Each elementB As XmlNode In element.ChildNodes
                            If elementB.Name.ToLower = "frame" Then
                                Dim obj As New Frame
                                obj.Description = elementB.Attributes.GetNamedItem("description").Value
                                obj.WaitTime = CInt(elementB.Attributes.GetNamedItem("duration").Value)
                                obj.Blackout = CBool(elementB.Attributes.GetNamedItem("blackout").Value)
                                obj.FadeTime = CInt(elementB.Attributes.GetNamedItem("fadetime").Value)
                                obj.MovementTransition = DirectCast([Enum].Parse(GetType(TransitionType), elementB.Attributes.GetNamedItem("trans").Value), TransitionType)
                                For Each elementC As XmlNode In elementB.ChildNodes
                                    Select Case elementC.Name.ToLower
                                        Case "keyframes"
                                            For Each elementD As XmlNode In elementC.ChildNodes
                                                Select Case elementD.Name.ToLower
                                                    Case "start"
                                                        obj.StartPos = New CamKeyframe(CInt(elementD.Attributes.GetNamedItem("x").Value), CInt(elementD.Attributes.GetNamedItem("y").Value), CInt(elementD.Attributes.GetNamedItem("z").Value),
                                                                     CInt(elementD.Attributes.GetNamedItem("w").Value), CInt(elementD.Attributes.GetNamedItem("p").Value), CInt(elementD.Attributes.GetNamedItem("r").Value))
                                                    Case "end"
                                                        obj.EndPos = New CamKeyframe(CInt(elementD.Attributes.GetNamedItem("x").Value), CInt(elementD.Attributes.GetNamedItem("y").Value), CInt(elementD.Attributes.GetNamedItem("z").Value),
                                                                    CInt(elementD.Attributes.GetNamedItem("w").Value), CInt(elementD.Attributes.GetNamedItem("p").Value), CInt(elementD.Attributes.GetNamedItem("r").Value))
                                                End Select
                                            Next

                                        Case "field"
                                            Dim objB As New Field
                                            objB.Description = elementC.Attributes.GetNamedItem("description").Value
                                            objB.Depth = CSng(elementC.Attributes.GetNamedItem("depth").Value)
                                            objB.TextureNr = CInt(elementC.Attributes.GetNamedItem("texture").Value)
                                            objB.Vertices = New VertexPositionColorTexture(CInt(elementC.Attributes.GetNamedItem("vertices").Value) - 1) {}
                                            objB.Indices = New Integer(CInt(elementC.Attributes.GetNamedItem("indices").Value) - 1) {}
                                            For Each elementD As XmlNode In elementC.ChildNodes
                                                Select Case elementD.Name.ToLower
                                                    Case "vertex"
                                                        Dim pos As New Vector3(CInt(elementD.Attributes.GetNamedItem("x").Value), CInt(elementD.Attributes.GetNamedItem("y").Value), CInt(elementD.Attributes.GetNamedItem("z").Value))
                                                        Dim color As New Color(CInt(elementD.Attributes.GetNamedItem("r").Value), CInt(elementD.Attributes.GetNamedItem("g").Value), CInt(elementD.Attributes.GetNamedItem("b").Value), CInt(elementD.Attributes.GetNamedItem("a").Value))
                                                        Dim UV As New Vector2(CInt(elementD.Attributes.GetNamedItem("UVx").Value), CInt(elementD.Attributes.GetNamedItem("UVy").Value))
                                                        objB.Vertices(CInt(elementD.Attributes.GetNamedItem("id").Value)) = New VertexPositionColorTexture(pos, color, UV)
                                                    Case "tri"
                                                        Dim id As Integer = CInt(elementD.Attributes.GetNamedItem("id").Value) * 3
                                                        objB.Indices(id + 0) = CInt(elementD.Attributes.GetNamedItem("A").Value)
                                                        objB.Indices(id + 1) = CInt(elementD.Attributes.GetNamedItem("B").Value)
                                                        objB.Indices(id + 2) = CInt(elementD.Attributes.GetNamedItem("C").Value)
                                                End Select
                                            Next
                                            obj.Add(objB)
                                    End Select
                                Next
                                res.FrameBuffer.Add(obj)
                            End If
                        Next
                    Case "texturebuffer"
                        For Each elementB As XmlNode In element.ChildNodes
                            If elementB.Name.ToLower = "obj" Then
                                Dim txt As Texture2D = Texture2D.FromStream(StandardAssets.Graphx.GraphicsDevice, New StreamReader(ContentMan.RootDirectory & "\debug\mps\media\" & elementB.Attributes.GetNamedItem("name").Value & ".png").BaseStream)
                                txt.Name = elementB.Attributes.GetNamedItem("name").Value
                                res.TextureBuffer.Add(txt)
                            End If
                        Next
                End Select
            Next
            Return res
        End Function

        Public Function GetScreenRay(ByVal screenPosition As Vector2) As Ray
            Dim viewport As Viewport = Dev.Viewport
            Dim world As Matrix = Matrix.CreateTranslation(-New Vector3(GameSize.X / 2, GameSize.Y / 2, 0))
            Dim nearPoint As Vector3 = viewport.Unproject(New Vector3(screenPosition, 0F), Projection, View, world)
            Dim farPoint As Vector3 = viewport.Unproject(New Vector3(screenPosition, 1.0F), Projection, View, world)
            Return New Ray(nearPoint, Vector3.Normalize(farPoint - nearPoint))
        End Function
    End Class

    Public Enum LoadType
        Immediate
        FrameAfterFrame
    End Enum

End Namespace