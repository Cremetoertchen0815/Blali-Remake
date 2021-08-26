Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Namespace Framework.UI
    Public Class GuiSystem
        Inherits RenderableComponent
        Implements IParent, IUpdatable

        Public Property Controls As New List(Of GuiControl)
        Public Property GlobalFont As NezSpriteFont Implements IParent.Font
        Public Overrides ReadOnly Property Bounds As RectangleF
            Get
                Return New RectangleF(__Bounds.X, __Bounds.Y, __Bounds.Width, __Bounds.Height)
            End Get
        End Property
        Private ReadOnly Property LocalBounds As Rectangle Implements IParent.Bounds
            Get
                Return __Bounds
            End Get
        End Property
        Private ReadOnly Property IUpdatable_Enabled As Boolean = True Implements IUpdatable.Enabled
        Private ReadOnly Property IUpdatable_UpdateOrder As Integer = 0 Implements IUpdatable.UpdateOrder
        Private __Bounds As New Rectangle(0, 0, 1920, 1080)

        Public Shared FastScrollThreshold As UInteger = 400

        'Fast Scrolling
        Private lens As Integer
        Private cnt As Integer
        Private fullblast As Boolean

        Private lastmstate As MouseState

        Public Overrides Sub OnAddedToEntity()
            GlobalFont = New NezSpriteFont(Core.Content.Load(Of SpriteFont)("font/fnt_HKG_17_M"))
            Material = New Material(DepthStencilState.None)

            For Each element In Controls
                element.Init(Me)
            Next
            MyBase.OnAddedToEntity()
        End Sub

        Private Sub Init(parent As IParent) Implements IParent.Init
        End Sub

        Public Sub Unload()
            For Each element In Controls
                element.Unload()
                element = Nothing
            Next
        End Sub

        Public Overrides Sub Render(batcher As Batcher, camera As Camera)
            RenderInternal(batcher, Color)
        End Sub
        Private Sub RenderInternal(batcher As Batcher, color As Color) Implements IParent.Render
            batcher.End()
            batcher.Begin(Material, Transform.LocalToWorldTransform)
            SetLayerDepth(0)
            Core.GraphicsDevice.DepthStencilState = DepthStencilState.None
            For Each element In Controls
                If element.Active Then element.Render(batcher, color)
            Next
        End Sub

        Public Sub Update(cstate As GuiInput, offset As Vector2) Implements IParent.Update

        End Sub

        Public Sub Update() Implements IUpdatable.Update
            Dim mstate As MouseState = Mouse.GetState
            'Fullblast
            If mstate.LeftButton = ButtonState.Pressed Then
                lens += CInt(Time.DeltaTime * 1000)
            Else
                lens = 0
                fullblast = False
            End If

            If lens > FastScrollThreshold Then
                cnt += CInt(Time.DeltaTime * 1000)
                If cnt > 30 Then
                    fullblast = True
                    cnt = 0
                Else
                    fullblast = False
                End If
            End If

            Dim cstate As New GuiInput With {
            .MousePosition = Vector2.Transform(mstate.Position.ToVector2, Matrix.Invert(ScaleMatrix)),
            .MousePositionTransformed = Vector2.Transform(mstate.Position.ToVector2, ScaleMatrix),
            .LeftClick = mstate.LeftButton = ButtonState.Pressed,
            .LeftClickOneshot = mstate.LeftButton = ButtonState.Pressed And lastmstate.LeftButton = ButtonState.Released,
            .ScrollDifference = (mstate.ScrollWheelValue - lastmstate.ScrollWheelValue) / 120.0F,
            .LeftClickFullBlast = fullblast
            }

            For Each element In Controls
                If element.Active Then element.Update(cstate, Vector2.Zero)
            Next

            lastmstate = mstate
        End Sub
    End Class
End Namespace