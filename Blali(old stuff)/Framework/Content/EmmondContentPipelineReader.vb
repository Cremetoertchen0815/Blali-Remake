Imports System.Collections.Generic
Imports Emmond.Framework.Level
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Namespace Framework.Content.EmmondContentPipelineReader

    <TestState(TestState.Finalized)>
    Public Class XNBStringContainerReader
        Inherits ContentTypeReader(Of XNBStringContainer)

        Protected Overrides Function Read(input As ContentReader, existingInstance As XNBStringContainer) As XNBStringContainer
            Try
                Dim dec As String = input.ReadString
                If dec = XNBStringContainer.Stdcode Then
                    Return New XNBStringContainer(VrnCoding.NormalStringBros64(input.ReadString))
                Else
                    Return New XNBStringContainer(VrnCoding.VrnCode(input.ReadString, dec)) With {.code = dec}
                End If
                'Dim dec As String = VrnCoding.NormalStringBros64(input.ReadString)
                'Return New XNBStringContainer(VrnCoding.VrnCode(input.ReadString, dec))
            Catch ex As Exception
                Throw New Exception("Stop tampering around, you little shit!  Sincerly, your mom.")
            End Try
        End Function
    End Class

    <TestState(TestState.WorkInProgress)>
    Public Class LvlReader
        Inherits ContentTypeReader(Of LvlDataContainer)

        Const Header As String = "!BeaconLvlData"

        Protected Overrides Function Read(ByVal input As ContentReader, ByVal existingInstance As LvlDataContainer) As LvlDataContainer
            Dim ret As New LvlDataContainer
            'Try
            'Read Header
            Dim initiator As String = input.ReadString : If initiator <> Header Then Throw New Exception
            ret.Version = {input.ReadDouble, input.ReadDouble}
            ret.Type = input.ReadInt32
            ret.Key = input.ReadString
            ret.AlternativeProcessingMethod = input.ReadBoolean
            ret.Comment = input.ReadString

            'Read Body
            Select Case ret.Type
                Case LvlDataType.Descriptor
                    Dim tmp As New LvlDescriptorData
                    tmp.Name = input.ReadString
                    tmp.Instr1 = input.ReadString
                    tmp.Instr2 = input.ReadString
                    tmp.Instr3 = input.ReadString
                    tmp.Instr4 = input.ReadString
                    tmp.Instr5 = input.ReadString
                    tmp.SubLevelCount = input.ReadUInt32
                    tmp.TileSetPath = input.ReadString

                    ret.Data = tmp
                Case LvlDataType.Map
                    Dim tmp As New TileSet
                    Dim len As Integer = input.ReadInt32 - 1
                    For i As Integer = 0 To len
                        If input.ReadBoolean Then
                            Dim tmpobjKey As Integer
                            Dim tmpobjValue As New Tile
                            tmpobjKey = input.ReadUInt16
                            tmpobjValue.OriginRectangle = New Rectangle(input.ReadVector2.ToPoint, input.ReadVector2.ToPoint)
                            tmpobjValue.DisableWallJump = input.ReadBoolean
                            tmpobjValue.Collision = input.ReadInt32
                            tmpobjValue.IsSlope = input.ReadBoolean
                            tmpobjValue.HeightMap = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                            For k As Integer = 0 To 15
                                tmpobjValue.HeightMap(k) = (input.ReadByte)
                            Next
                            tmp.Tiles.Add(tmpobjKey, tmpobjValue)
                            For j As Integer = 0 To input.ReadInt32 - 1
                                Dim ke As Vector3 = input.ReadVector3
                                Select Case ke.Z
                                    Case 0
                                        tmp.BackLayer.Add(New Vector2(ke.X, ke.Y), tmpobjKey)
                                    Case 1
                                        tmp.Add(New Vector2(ke.X, ke.Y), tmpobjKey)
                                    Case 2
                                        tmp.FrontLayer.Add(New Vector2(ke.X, ke.Y), tmpobjKey)
                                End Select
                            Next
                        End If
                    Next

                    ret.Data = tmp
                Case LvlDataType.Background
                    Dim tmp As New List(Of LvlBGData)
                    For i As Integer = 0 To input.ReadInt32 - 1
                        Dim tmpobj As New LvlBGData
                        tmpobj.Type = input.ReadUInt16
                        tmpobj.Location = input.ReadVector2
                        tmpobj.Scale = input.ReadVector2
                        tmpobj.VectorScale = input.ReadSingle
                        tmpobj.Color = input.ReadColor
                        tmpobj.Layer = input.ReadSingle
                        tmpobj.Path = input.ReadString
                        tmp.Add(tmpobj)
                    Next

                    ret.Data = tmp
                Case LvlDataType.Trigger
                    Dim tmp As New Dictionary(Of UInteger, Trigger)
                    For i As Integer = 0 To input.ReadInt32 - 1
                        Dim tmpobj As New Trigger
                        tmpobj.ID = input.ReadInt32
                        tmpobj.Type = input.ReadInt32
                        tmpobj.ExecType = input.ReadInt32
                        tmpobj.Location = input.ReadVector2
                        tmpobj.Orientation = input.ReadInt32
                        tmpobj.Length = input.ReadUInt32
                        tmpobj.VectorArgument = input.ReadVector2
                        tmp.Add(tmpobj.ID, tmpobj)
                    Next

                    ret.Data = tmp
                Case LvlDataType.FX
                    Dim tmp As New LvlFXData
                    tmp.EnableLighting = input.ReadBoolean
                    tmp.AmbientColor = input.ReadColor
                    tmp.EnableBloom = input.ReadBoolean
                    tmp.BloomPreset = input.ReadInt32
                    tmp.BloomThreshold = input.ReadSingle
                    tmp.BloomStrengthMultiplier = input.ReadSingle
                    tmp.Hulls = New List(Of Vector2())
                    For i As Integer = 0 To input.ReadInt32 - 1
                        Dim veclst As New List(Of Vector2)
                        For j As Integer = 0 To input.ReadInt32 - 1
                            veclst.Add(input.ReadVector2)
                        Next
                        tmp.Hulls.Add(veclst.ToArray)
                    Next

                    tmp.Lights = New List(Of LvlFXData.Light)
                    For i As Integer = 0 To input.ReadInt32 - 1
                        Dim tmpobj As New LvlFXData.Light
                        tmpobj.Type = input.ReadInt32
                        tmpobj.CastsShadows = input.ReadBoolean
                        tmpobj.Color = input.ReadColor
                        tmpobj.ConeDecay = input.ReadSingle
                        tmpobj.Enabled = input.ReadBoolean
                        tmpobj.Intensity = input.ReadSingle
                        tmpobj.Origin = input.ReadVector2
                        tmpobj.Position = input.ReadVector2
                        tmpobj.Radius = input.ReadSingle
                        tmpobj.Rotation = input.ReadSingle
                        tmpobj.Scale = input.ReadVector2
                        tmpobj.ShadowTypeC = input.ReadInt32
                        tmp.Lights.Add(tmpobj)
                    Next

                    ret.Data = tmp
                Case LvlDataType.Objects
                    Dim tmp As New List(Of LvlObj)
                    For i As Integer = 0 To input.ReadInt32 - 1
                        Dim tmpobj As New LvlObj
                        tmpobj.Location = input.ReadVector2
                        tmpobj.Group = input.ReadUInt16
                        tmpobj.ID = input.ReadUInt16
                        tmpobj.Type = input.ReadUInt16
                        tmp.Add(tmpobj)
                    Next
                    ret.Data = tmp
            End Select
            input.Close()
            'Catch ex As Exception
            '    Throw New Exception("Stop tampering around, you little shit!  Sincerly, your mom.", ex)
            'End Try

            Return ret
        End Function
    End Class
End Namespace