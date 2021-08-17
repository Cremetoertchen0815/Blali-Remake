Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Xml
Imports Emmond.Framework.Content
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Level.Background
Imports Emmond.Framework.Level.Background.BGs
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

'Not much to comment here, lol

Namespace Framework.Level

    <TestState(TestState.WorkInProgress)>
    Public Class LevelParser
        Public Shared Function LoadLevel(ID As String) As Level
            'Load level data from XML(debug mode)
            If DebugMode Then Return LoadLevelFromXML(ID)

            'Prepare flags, variables and the XML parser
            Dim res As New Level

            'Load main level properties
            Dim maindoc As LvlDescriptorData = DirectCast(ContentMan.Load(Of LvlDataContainer)("lvl\" & ID & "\dsc").Data, LvlDescriptorData)
            res.Header = New LevelHeader
            res.Header.Description = maindoc.Name
            res.Header.Instructions = {maindoc.Instr1, maindoc.Instr2, maindoc.Instr3, maindoc.Instr4, maindoc.Instr5}
            res.Header.LoadedID = ID

            ''Load camera
            'res.Camera = New Camera.Camera(True) With {
            '    .AllowHorizontal = maindoc.AllowHorizontal,
            '    .AllowVertical = maindoc.AllowVertical,
            '    .DefaultLocation = New Vector2(maindoc.StdHorizontal, maindoc.StdVertical),
            '    .LevelBounds = New Rectangle(0, 1080 - maindoc.Height * cTileSize, maindoc.Width * cTileSize, maindoc.Height * cTileSize)
            ' }



            'Load Map
            res.Map = DirectCast(ContentMan.Load(Of LvlDataContainer)("lvl\" & ID & "\map").Data, TileSet).Clone
            res.Map.TileSpriteSheetPath = "lvl\" & ID & "\" & maindoc.TileSetPath


            'Load Backgrounds
            Dim tmpBG As List(Of LvlBGData) = DirectCast(ContentMan.Load(Of LvlDataContainer)("lvl\" & ID & "\bg").Data, List(Of LvlBGData))
            'res.Background = New BackgroundManager With {.skyboxPath = maindoc.SkyboxPath}
            For Each element In tmpBG
                    Select Case element.Type
                        Case 0
                            Dim newBG As New BackgroundStatic With {
                                .Layer = element.Layer,
                                .Color = element.Color,
                                .Location = element.Location,
                                .Path = element.Path,
                                .Size = element.Scale,
                                .VectorScale = element.VectorScale
                            }
                            res.Background.Add(newBG)
                        Case 1
                            Dim newBG As New BackgroundFlex With {
                                .Layer = element.Layer,
                                .Color = element.Color,
                                .Location = element.Location,
                                .Path = element.Path,
                                .Size = element.Scale,
                                .VectorScale = element.VectorScale
                            }
                            res.Background.Add(newBG)
                    End Select
                Next


            'Load Triggers
            res.TriggerMan = DirectCast(ContentMan.Load(Of LvlDataContainer)("lvl\" & ID & "\trg").Data, Dictionary(Of UInteger, Trigger))


            'Load FX
            Dim tmpFX As LvlFXData = DirectCast(ContentMan.Load(Of LvlDataContainer)("lvl\" & ID & "\fx").Data, LvlFXData)
            res.FXData = New FXData With {
                    .EnableLighting = tmpFX.EnableLighting,
                    .AmbientColor = tmpFX.AmbientColor,
                    .EnableBloom = tmpFX.EnableBloom,
                    .BloomPreset = tmpFX.BloomPreset,
                    .BloomStrengthMultiplier = tmpFX.BloomStrengthMultiplier,
                    .BloomThreshold = tmpFX.BloomThreshold,
                    .Hulls = New List(Of Penumbra.Hull)
                }
                For Each element In tmpFX.Hulls
                    res.FXData.Hulls.Add(New Penumbra.Hull(element))
                Next
                res.FXData.Lights = New List(Of Penumbra.Light)
                For Each element In tmpFX.Lights
                    Select Case element.Type
                        Case LvlFXData.Light.LightType.PointLight
                            Dim newL As New Penumbra.PointLight With {
                                .CastsShadows = element.CastsShadows,
                                .Color = element.Color,
                                .Intensity = element.Intensity,
                                .Origin = element.Origin,
                                .Position = element.Position,
                                .Radius = element.Radius,
                                .Rotation = element.Rotation,
                                .Scale = element.Scale,
                                .ShadowType = element.ShadowTypeC
                            }
                            res.FXData.Lights.Add(newL)
                        Case LvlFXData.Light.LightType.Spotlight
                            Dim newL As New Penumbra.Spotlight With {
                                .CastsShadows = element.CastsShadows,
                                .Color = element.Color,
                                .ConeDecay = element.ConeDecay,
                                .Intensity = element.Intensity,
                                .Origin = element.Origin,
                                .Position = element.Position,
                                .Radius = element.Radius,
                                .Rotation = element.Rotation,
                                .Scale = element.Scale,
                                .ShadowType = element.ShadowTypeC
                            }
                            res.FXData.Lights.Add(newL)
                    End Select
                Next

                'Load Objects
                res.ItemMan = New ObjectManager.ObjectManager
                res.Entities = New List(Of Entities.Entity) From {Nothing}
            Dim tmpObj As List(Of LvlObj) = DirectCast(ContentMan.Load(Of LvlDataContainer)("lvl\" & ID & "\obj").Data, List(Of LvlObj))
            For Each obj In tmpObj
                    If obj.Group = 0 Then
                    Select Case obj.Type
                        Case 3
                            res.ItemMan.Add(obj.ID, New ObjectManager.Objects.SublevelDoor(res, obj.Location))
                        Case 4
                            res.ItemMan.Add(obj.ID, New ObjectManager.Objects.Spring(res, obj.Location))
                    End Select
                ElseIf obj.Group = 1 Then
                    Select Case obj.Type
                    End Select
                End If
            Next

            res.MSFX = New MSFXcontainer

            Return res
        End Function

        Private Shared Function LoadLevelFromXML(ID As String, Optional LoadHeaderOnly As Boolean = False, Optional backup As Boolean = False) As Level
            'Try
            Dim ending As String = ".ele"
            If backup Then ending = ending & ".backup"

            'Prepare flags, variables and the XML parser
            Dim res As New Level

            'Load main level properties
            Dim MainNode As XmlNode
            Dim XmlDoc As New XmlDocument
            XmlDoc.Load(ContentMan.RootDirectory & "\debug\" & ID & "\dsc.ele")
            MainNode = XmlDoc.SelectNodes("/lvldata")(0)

            Dim maindoc As New LvlDescriptorData With {
                .Name = MainNode.SelectNodes("/lvldata/name")(0).InnerXml,
                .Instr1 = MainNode.SelectNodes("/lvldata/inst1")(0).InnerXml,
                .Instr2 = MainNode.SelectNodes("/lvldata/inst2")(0).InnerXml,
                .Instr3 = MainNode.SelectNodes("/lvldata/inst3")(0).InnerXml,
                .Instr4 = MainNode.SelectNodes("/lvldata/inst4")(0).InnerXml,
                .Instr5 = MainNode.SelectNodes("/lvldata/inst5")(0).InnerXml,
                .SubLevelCount = MainNode.SelectNodes("/lvldata/SubLevelCount")(0).InnerXml,
                .TileSetPath = MainNode.SelectNodes("/lvldata/TileSetPath")(0).InnerXml
            }

            res.Header = New LevelHeader
            res.Header.LoadedID = ID
            res.Header.SubLvlCount = maindoc.SubLevelCount
            res.Header.TileSetPath = maindoc.TileSetPath
            res.Header.Description = maindoc.Name
            res.Header.Instructions = {maindoc.Instr1, maindoc.Instr2, maindoc.Instr3, maindoc.Instr4, maindoc.Instr5}

            res.MSFX = New MSFXcontainer

            Return res
        End Function

        Friend Shared Sub LoadSubLevel(sublvl As Integer, lvl As Level, Optional backup As Boolean = False)
            'Load main level properties
            Dim m_xmld_tls As XmlDocument
            Dim XmlDoc As New XmlDocument

            Dim ID As String = lvl.Header.LoadedID
            Dim ending As String = ".ele"
            If backup Then ending = ending & ".backup"

            lvl.Header.Loaded = False
            lvl.Header.LoadedSublvl = sublvl

            'Load Map & Tiles
            lvl.Map = New TileSet
            Using sr As New StreamReader(ContentMan.RootDirectory & "\debug\" & ID & "\" & lvl.Header.TileSetPath & ".png")
                lvl.Map.TileSpriteSheet = Texture2D.FromStream(StandardAssets.Graphx.GraphicsDevice, sr.BaseStream)
                sr.Close()
            End Using
            m_xmld_tls = New XmlDocument
            m_xmld_tls.Load(ContentMan.RootDirectory & "\debug\" & ID & "\map_" & sublvl & ending)
            Dim sv As Integer() = {0, 0, 0, 0, 0, 0}
            For Each element As XmlNode In m_xmld_tls.SelectNodes("/lvldata")(0).ChildNodes
                Select Case element.Name
                    Case "obj"
                        Dim vec As New Vector2(CInt(element.Attributes.GetNamedItem("x").Value), CInt(element.Attributes.GetNamedItem("y").Value))
                        Select Case CInt(element.Attributes.GetNamedItem("z").Value)
                            Case 0
                                lvl.Map.BackLayer.Add(vec, CInt(element.Attributes.GetNamedItem("tile").Value))
                            Case 1
                                lvl.Map.Add(vec, CInt(element.Attributes.GetNamedItem("tile").Value))
                            Case 2
                                lvl.Map.FrontLayer.Add(vec, CInt(element.Attributes.GetNamedItem("tile").Value))
                        End Select
                    Case "tiles"
                        For Each elementB In element.ChildNodes
                            If elementB.Name.ToLower = "obj" Then
                                Dim IDs As Integer = CInt(elementB.Attributes.GetNamedItem("ID").Value)
                                Dim LoadCollision As TileCollisionType = DirectCast([Enum].Parse(GetType(TileCollisionType), elementB.Attributes.GetNamedItem("collision").Value), TileCollisionType)
                                Dim DisableWallJump As Boolean = CBool(elementB.Attributes.GetNamedItem("disablewalljump").Value)
                                Dim rect As New Rectangle(CInt(elementB.Attributes.GetNamedItem("originX").Value), CInt(elementB.Attributes.GetNamedItem("originY").Value),
                                                  CInt(elementB.Attributes.GetNamedItem("originWidth").Value), CInt(elementB.Attributes.GetNamedItem("originHeight").Value))
                                Dim HeightMap As Byte() = Convert.FromBase64String(CStr(elementB.Attributes.GetNamedItem("heightmap").Value))
                                Dim IsSlope As Boolean = CBool(elementB.Attributes.GetNamedItem("slope").Value)
                                lvl.Map.Tiles.Add(IDs, New Tile With {.Collision = LoadCollision, .DisableWallJump = DisableWallJump, .OriginRectangle = rect, .HeightMap = HeightMap, .IsSlope = IsSlope})
                            End If
                        Next
                    Case "tlg"
                        For Each elementB As XmlNode In element.ChildNodes
                            If elementB.Name.ToLower = "group" Then
                                Dim grp As New TileGroup With {
                                .OriginalTile = CInt(elementB.Attributes.GetNamedItem("tile").Value),
                                .Size = New Vector2(CInt(elementB.Attributes.GetNamedItem("sizeX").Value), CInt(elementB.Attributes.GetNamedItem("sizeY").Value)),
                                .MapTiles = New Dictionary(Of Vector2, UShort),
                                .Origin = New Rectangle(CInt(elementB.Attributes.GetNamedItem("originX").Value), CInt(elementB.Attributes.GetNamedItem("originY").Value),
                                                  CInt(elementB.Attributes.GetNamedItem("originWidth").Value), CInt(elementB.Attributes.GetNamedItem("originHeight").Value))
                            }
                                For Each elementC In elementB.SelectNodes("map")(0).ChildNodes
                                    grp.MapTiles.Add(New Vector2(CInt(elementC.Attributes.GetNamedItem("x").Value), CInt(elementC.Attributes.GetNamedItem("y").Value)), CInt(elementC.Attributes.GetNamedItem("tile").Value))
                                Next
                                lvl.Map.TileGroups.Add(grp)
                            End If
                        Next
                    Case "width"
                        sv(0) = CInt(element.InnerText)
                    Case "height"
                        sv(1) = CInt(element.InnerText)
                    Case "psX"
                        sv(2) = CInt(element.InnerText)
                    Case "psY"
                        sv(3) = CInt(element.InnerText)
                    Case "StdHorizontal"
                        sv(4) = CInt(element.InnerText)
                    Case "StdVertical"
                        sv(5) = CInt(element.InnerText)
                End Select
            Next
            lvl.Map.Size = New Vector2(sv(0), sv(1))
            lvl.Map.Spawn = New Vector2(sv(2), sv(3))
            lvl.Map.StdCamPosition = New Vector2(sv(4), sv(5))

            m_xmld_tls = New XmlDocument
            m_xmld_tls.Load(ContentMan.RootDirectory & "\debug\" & ID & "\bg_" & sublvl & ending)
            lvl.Background = New BackgroundManager
            For Each element As XmlNode In m_xmld_tls.SelectNodes("/lvldata")(0).ChildNodes
                If element.Name.ToLower = "obj" Then
                    Select Case CInt(element.Attributes.GetNamedItem("type").Value)
                        Case 0 'If the background type is static
                            Dim obj As New BackgroundStatic()
                            For Each elementC In element.ChildNodes
                                If elementC.Name.ToLower = "property" Then
                                    Select Case elementC.Attributes.GetNamedItem("name").Value
                                        Case "layer"
                                            obj.Layer = CSng(elementC.Attributes.GetNamedItem("value").Value)
                                        Case "path"
                                            obj.Path = CStr(elementC.Attributes.GetNamedItem("value").Value)
                                            Using sr As New StreamReader(ContentMan.RootDirectory & "\debug\" & ID & "\media\" & obj.Path & ".png")
                                                obj.Texture = Texture2D.FromStream(StandardAssets.Graphx.GraphicsDevice, sr.BaseStream)
                                                sr.Close()
                                            End Using
                                        Case "location"
                                            obj.Location = New Vector2(Convert.ToDouble(elementC.Attributes.GetNamedItem("valueX").Value),
                                                                                   Convert.ToDouble(elementC.Attributes.GetNamedItem("valueY").Value))
                                        Case "vectorscale"
                                            obj.VectorScale = CSng(elementC.Attributes.GetNamedItem("value").Value)
                                        Case "scale"
                                            obj.Size = New Vector2(Convert.ToDouble(elementC.Attributes.GetNamedItem("valueX").Value),
                                                                                   Convert.ToDouble(elementC.Attributes.GetNamedItem("valueY").Value))
                                        Case "color"
                                            obj.Color = New Color(CInt(elementC.Attributes.GetNamedItem("valueR").Value),
                                                                              CInt(elementC.Attributes.GetNamedItem("valueG").Value),
                                                                              CInt(elementC.Attributes.GetNamedItem("valueB").Value),
                                                                              CInt(elementC.Attributes.GetNamedItem("valueA").Value))
                                    End Select
                                End If
                            Next
                            lvl.Background.Add(obj)
                        Case 1 'If the background type is flex
                            Dim obj As New BackgroundFlex()
                            For Each elementC In element.ChildNodes
                                If elementC.Name.ToLower = "property" Then
                                    Select Case elementC.Attributes.GetNamedItem("name").Value
                                        Case "layer"
                                            obj.Layer = CSng(elementC.Attributes.GetNamedItem("value").Value)
                                        Case "path"
                                            obj.Path = CStr(elementC.Attributes.GetNamedItem("value").Value)
                                            Using sr As New StreamReader(ContentMan.RootDirectory & "\debug\" & ID & "\media\" & obj.Path & ".png")
                                                obj.Texture = Texture2D.FromStream(StandardAssets.Graphx.GraphicsDevice, sr.BaseStream)
                                                sr.Close()
                                            End Using
                                        Case "location"
                                            obj.Location = New Vector2(Convert.ToDouble(elementC.Attributes.GetNamedItem("valueX").Value),
                                                                                   Convert.ToDouble(elementC.Attributes.GetNamedItem("valueY").Value))
                                        Case "vectorscale"
                                            obj.VectorScale = CSng(elementC.Attributes.GetNamedItem("value").Value)
                                        Case "scale"
                                            obj.Size = New Vector2(Convert.ToDouble(elementC.Attributes.GetNamedItem("valueX").Value),
                                                                                   Convert.ToDouble(elementC.Attributes.GetNamedItem("valueY").Value))
                                        Case "color"
                                            obj.Color = New Color(CInt(elementC.Attributes.GetNamedItem("valueR").Value),
                                                                              CInt(elementC.Attributes.GetNamedItem("valueG").Value),
                                                                              CInt(elementC.Attributes.GetNamedItem("valueB").Value),
                                                                              CInt(elementC.Attributes.GetNamedItem("valueA").Value))
                                    End Select
                                End If
                            Next
                            lvl.Background.Add(obj)
                    End Select
                ElseIf element.Name.ToLower = "skybox" Then
                    lvl.Background = New BackgroundManager With {
                        .skyboxPath = element.InnerText
                    }
                    If File.Exists(ContentMan.RootDirectory & "\debug\" & ID & "\" & lvl.Background.skyboxPath & ".png") Then
                        Using sr As New StreamReader(ContentMan.RootDirectory & "\debug\" & ID & "\" & lvl.Background.skyboxPath & ".png")
                            lvl.Background.skybox = Texture2D.FromStream(StandardAssets.Graphx.GraphicsDevice, sr.BaseStream)
                            sr.Close()
                        End Using
                    End If
                End If
            Next

            lvl.TriggerMan = New Dictionary(Of UInteger, Trigger)
            m_xmld_tls = New XmlDocument
            m_xmld_tls.Load(ContentMan.RootDirectory & "\debug\" & ID & "\trg_" & sublvl & ending)
            For Each element In m_xmld_tls.SelectNodes("/lvldata")(0).ChildNodes
                If element.Name.ToLower = "obj" Then
                    Dim obj As New Trigger
                    Dim tmpID As UInteger = CUInt(element.Attributes.GetNamedItem("id").Value)
                    obj.ID = tmpID
                    obj.Type = DirectCast([Enum].Parse(GetType(TriggerType), element.Attributes.GetNamedItem("type").Value), TriggerType)
                    For Each elementC In element.ChildNodes
                        If elementC.Name.ToLower = "property" Then
                            Select Case elementC.Attributes.GetNamedItem("name").Value
                                Case "location"
                                    obj.Location = New Vector2(Convert.ToDouble(elementC.Attributes.GetNamedItem("valueX").Value),
                                                                                   Convert.ToDouble(elementC.Attributes.GetNamedItem("valueY").Value))
                                Case "length"
                                    obj.Length = CInt(elementC.Attributes.GetNamedItem("value").Value)
                                Case "orientation"
                                    obj.Orientation = DirectCast([Enum].Parse(GetType(TriggerOrientation), elementC.Attributes.GetNamedItem("value").Value), TriggerOrientation)
                                Case "vecarg"
                                    obj.VectorArgument = New Vector2(Convert.ToDouble(elementC.Attributes.GetNamedItem("valueX").Value),
                                                                                   Convert.ToDouble(elementC.Attributes.GetNamedItem("valueY").Value))
                                Case "ExecType"
                                    obj.ExecType = DirectCast([Enum].Parse(GetType(ExecType), elementC.Attributes.GetNamedItem("value").Value), ExecType)
                            End Select
                        End If
                    Next
                    lvl.TriggerMan.Add(tmpID, obj)
                End If
            Next

            m_xmld_tls = New XmlDocument
            m_xmld_tls.Load(ContentMan.RootDirectory & "\debug\" & ID & "\fx_" & sublvl & ending)
            Dim m_xmld_tls_node As XmlNode = m_xmld_tls.SelectNodes("/lvldata/fx")(0)
            lvl.FXData = New FXData With {
                .EnableLighting = CBool(m_xmld_tls_node.Attributes.GetNamedItem("active").Value),
                .AmbientColor = New Color(CInt(m_xmld_tls_node.Attributes.GetNamedItem("r").Value), CInt(m_xmld_tls_node.Attributes.GetNamedItem("g").Value),
                                         CInt(m_xmld_tls_node.Attributes.GetNamedItem("b").Value), CInt(m_xmld_tls_node.Attributes.GetNamedItem("a").Value)),
                .Lights = New List(Of Penumbra.Light),
                .Hulls = New List(Of Penumbra.Hull)
            }
            For Each element As XmlNode In m_xmld_tls_node.ChildNodes
                Select Case element.Name.ToLower
                    Case "src"
                        For Each elementB As XmlNode In element.ChildNodes
                            If elementB.Name.ToLower = "obj" Then
                                Select Case elementB.Attributes.GetNamedItem("type").Value
                                    Case "Spotlight"
                                        Dim tmpElement As New Penumbra.Spotlight
                                        Dim tmpColor As New Color
                                        Dim tmpOrigin As New Vector2
                                        Dim tmpPosition As New Vector2
                                        Dim tmpScale As New Vector2
                                        For Each elementC As XmlAttribute In elementB.Attributes
                                            Select Case elementC.Name
                                                Case "CastsShadows"
                                                    tmpElement.CastsShadows = CBool(elementC.Value)
                                                Case "r"
                                                    tmpColor.R = CInt(elementC.Value)
                                                Case "g"
                                                    tmpColor.G = CInt(elementC.Value)
                                                Case "b"
                                                    tmpColor.B = CInt(elementC.Value)
                                                Case "a"
                                                    tmpColor.A = CInt(elementC.Value)
                                                Case "ConeDecay"
                                                    tmpElement.ConeDecay = CSng(elementC.Value)
                                                Case "Enabled"
                                                    tmpElement.Enabled = CBool(elementC.Value)
                                                Case "Intensity"
                                                    tmpElement.Intensity = CSng(elementC.Value)
                                                Case "OriginX"
                                                    tmpOrigin.X = CSng(elementC.Value)
                                                Case "OriginY"
                                                    tmpOrigin.Y = CSng(elementC.Value)
                                                Case "PositionX"
                                                    tmpPosition.X = CSng(elementC.Value)
                                                Case "PositionY"
                                                    tmpPosition.Y = CSng(elementC.Value)
                                                Case "Radius"
                                                    tmpElement.Radius = CSng(elementC.Value)
                                                Case "Rotation"
                                                    tmpElement.Rotation = CSng(elementC.Value)
                                                Case "ScaleX"
                                                    tmpScale.X = CSng(elementC.Value)
                                                Case "ScaleY"
                                                    tmpScale.Y = CSng(elementC.Value)
                                                Case "ShadowType"
                                                    tmpElement.ShadowType = DirectCast([Enum].Parse(GetType(Penumbra.ShadowType), CStr(elementC.Value)), Penumbra.ShadowType)
                                            End Select
                                        Next
                                        tmpElement.Color = tmpColor
                                        tmpElement.Origin = tmpOrigin
                                        tmpElement.Position = tmpPosition
                                        tmpElement.Scale = tmpScale
                                        tmpElement.Enabled = True
                                        lvl.FXData.Lights.Add(tmpElement)
                                    Case "PointLight"
                                        Dim tmpElement As New Penumbra.PointLight
                                        Dim tmpColor As New Color
                                        Dim tmpOrigin As New Vector2
                                        Dim tmpPosition As New Vector2
                                        Dim tmpScale As New Vector2
                                        For Each elementC As XmlAttribute In elementB.Attributes
                                            Select Case elementC.Name
                                                Case "CastsShadows"
                                                    tmpElement.CastsShadows = CBool(elementC.Value)
                                                Case "r"
                                                    tmpColor.R = CInt(elementC.Value)
                                                Case "g"
                                                    tmpColor.G = CInt(elementC.Value)
                                                Case "b"
                                                    tmpColor.B = CInt(elementC.Value)
                                                Case "a"
                                                    tmpColor.A = CInt(elementC.Value)
                                                Case "Enabled"
                                                    tmpElement.Enabled = CBool(elementC.Value)
                                                Case "Intensity"
                                                    tmpElement.Intensity = CSng(elementC.Value)
                                                Case "OriginX"
                                                    tmpOrigin.X = CSng(elementC.Value)
                                                Case "OriginY"
                                                    tmpOrigin.Y = CSng(elementC.Value)
                                                Case "PositionX"
                                                    tmpPosition.X = CSng(elementC.Value)
                                                Case "PositionY"
                                                    tmpPosition.Y = CSng(elementC.Value)
                                                Case "Radius"
                                                    tmpElement.Radius = CSng(elementC.Value)
                                                Case "Rotation"
                                                    tmpElement.Rotation = CSng(elementC.Value)
                                                Case "ScaleX"
                                                    tmpScale.X = CSng(elementC.Value)
                                                Case "ScaleY"
                                                    tmpScale.Y = CSng(elementC.Value)
                                                Case "ShadowType"
                                                    tmpElement.ShadowType = DirectCast([Enum].Parse(GetType(Penumbra.ShadowType), CStr(elementC.Value)), Penumbra.ShadowType)
                                            End Select
                                        Next
                                        tmpElement.Color = tmpColor
                                        tmpElement.Origin = tmpOrigin
                                        tmpElement.Position = tmpPosition
                                        tmpElement.Scale = tmpScale
                                        tmpElement.Enabled = True
                                        lvl.FXData.Lights.Add(tmpElement)
                                End Select
                            End If
                        Next
                    Case "hulls"
                        For Each elementB As XmlNode In element.ChildNodes
                            If elementB.Name.ToLower = "obj" Then
                                Dim lst As New List(Of Vector2)
                                For Each elementC As XmlNode In elementB.ChildNodes
                                    If elementC.Name.ToLower = "vertex" Then lst.Add(New Vector2(CInt(elementC.Attributes.GetNamedItem("x").Value), CInt(elementC.Attributes.GetNamedItem("y").Value)))
                                Next
                                lvl.FXData.Hulls.Add(New Penumbra.Hull(lst.ToArray))
                            End If
                        Next
                    Case "bloom"
                        With lvl.FXData
                            .EnableBloom = CBool(element.Attributes.GetNamedItem("activate").Value)
                            .BloomPreset = DirectCast([Enum].Parse(GetType(PostProcessing.BloomFilter.BloomPresets), CStr(element.Attributes.GetNamedItem("preset").Value)), PostProcessing.BloomFilter.BloomPresets)
                            .BloomThreshold = CSng(element.Attributes.GetNamedItem("threshold").Value)
                            .BloomStrengthMultiplier = CSng(element.Attributes.GetNamedItem("strength").Value)
                        End With
                End Select

            Next

            'Load Items & Entities
            lvl.ItemMan = New ObjectManager.ObjectManager
            lvl.Entities = New List(Of Entities.Entity) From {Nothing}
            m_xmld_tls = New XmlDocument
            m_xmld_tls.Load(ContentMan.RootDirectory & "\debug\" & ID & "\obj_" & sublvl & ending)
            For Each element In m_xmld_tls.SelectNodes("/lvldata")(0).ChildNodes
                If element.Name.ToLower = "obj" Then
                    Dim x As Integer = CInt(element.Attributes.GetNamedItem("x").Value)
                    Dim y As Integer = CInt(element.Attributes.GetNamedItem("y").Value)
                    Dim group As String = CStr(element.Attributes.GetNamedItem("group").Value)
                    Dim IDs As Integer = CInt(element.Attributes.GetNamedItem("ID").Value)
                    Dim type As Integer = CInt(element.Attributes.GetNamedItem("type").Value)
                    If group.ToLower = "item" Then
                        Select Case type
                            Case 3
                                lvl.ItemMan.Add(IDs, New ObjectManager.Objects.SublevelDoor(lvl, New Vector2(x, y)) With {.SublvlDestination = CInt(element.Attributes.GetNamedItem("destination").Value)})
                            Case 4
                                lvl.ItemMan.Add(IDs, New ObjectManager.Objects.Spring(lvl, New Vector2(x, y)))
                            Case 5
                                lvl.ItemMan.Add(IDs, New ObjectManager.Objects.YellowCoin(lvl, New Vector2(x, y)))
                            Case 6
                                lvl.ItemMan.Add(IDs, New ObjectManager.Objects.RedCoin(lvl, New Vector2(x, y)))
                            Case 7
                                lvl.ItemMan.Add(IDs, New ObjectManager.Objects.AnimeTyp(lvl, New Vector2(x, y)))
                            Case 8
                                lvl.ItemMan.Add(IDs, New ObjectManager.Objects.Gun(lvl, New Vector2(x, y)))
                        End Select
                    ElseIf group.ToLower = "entity" Then
                        Select Case type
                            Case 10
                                lvl.Entities.Add(New IG.Entities.Navi(IDs) With {.mSpawn = New Vector2(x, y)})
                            Case 11
                                lvl.Entities.Add(New IG.Entities.BlenemyA(IDs) With {.mSpawn = New Vector2(x, y)})
                            Case 12
                                lvl.Entities.Add(New IG.Entities.BlenemyB(IDs) With {.mSpawn = New Vector2(x, y)})
                            Case 13
                                lvl.Entities.Add(New IG.Entities.BlenemyC(IDs) With {.mSpawn = New Vector2(x, y)})
                        End Select
                    End If
                End If
            Next

            'Load camera
            lvl.Camera = New Camera.Camera(True) With {
            .AllowHorizontal = True,
            .AllowVertical = True,
            .DefaultLocation = lvl.Map.StdCamPosition,
            .LevelBounds = New Rectangle(0, 1080 - lvl.Map.Size.Y * cTileSize, lvl.Map.Size.X * cTileSize, lvl.Map.Size.Y * cTileSize)
         }

        End Sub

        Public Shared Sub SaveBackup(ID As String, sublvl As Integer)
            Dim tmplvl As Level = LoadLevelFromXML(ID, True)
            LoadSubLevel(sublvl, tmplvl, False)
            File.Copy(ContentMan.RootDirectory & "\debug\" & ID & "\map_" & sublvl & ".ele", ContentMan.RootDirectory & "\debug\" & ID & "\map_" & sublvl & ".ele.backup", True)
            File.Copy(ContentMan.RootDirectory & "\debug\" & ID & "\bg_" & sublvl & ".ele", ContentMan.RootDirectory & "\debug\" & ID & "\bg_" & sublvl & ".ele.backup", True)
            File.Copy(ContentMan.RootDirectory & "\debug\" & ID & "\fx_" & sublvl & ".ele", ContentMan.RootDirectory & "\debug\" & ID & "\fx_" & sublvl & ".ele.backup", True)
            File.Copy(ContentMan.RootDirectory & "\debug\" & ID & "\obj_" & sublvl & ".ele", ContentMan.RootDirectory & "\debug\" & ID & "\obj_" & sublvl & ".ele.backup", True)
            File.Copy(ContentMan.RootDirectory & "\debug\" & ID & "\trg_" & sublvl & ".ele", ContentMan.RootDirectory & "\debug\" & ID & "\trg_" & sublvl & ".ele.backup", True)
        End Sub

        Public Shared Function LoadBackup(ID As String, sublvl As Integer) As Level
            If File.Exists(ContentMan.RootDirectory & "\debug\" & ID & "\dsc_" & sublvl & ".ele.backup") Then
                Return LoadLevelFromXML(ID, False, True)
            Else
                Return Nothing
            End If
        End Function

        Public Shared Sub SaveLevel(Lvl As Level, Optional backup As Boolean = True)
            Try

                Dim ID As String = Lvl.Header.LoadedID
                If backup Then SaveBackup(ID, Lvl.Header.LoadedSublvl)

                Dim sb As New StringBuilder()

                '--------xxx_dsc.ele--------
                sb.Clear()
                'Write header
                sb.AppendLine("<?xml version=""1.0""?>")
                sb.AppendLine("<lvldata verH=""1"" verB=""1"" type=""Descriptor"" key="""" alt=""False"" com=""Built for: " & cCurrentVersion & """>")
                'Write data
                sb.AppendLine("<name>" & Lvl.Header.Description & "</name>")
                For i As Integer = 0 To Lvl.Header.Instructions.Length - 1
                    sb.AppendLine("<inst" & (i + 1).ToString & ">" & Lvl.Header.Instructions(i) & "</inst" & (i + 1).ToString & ">")
                Next
                sb.AppendLine("<SubLevelCount>" & Lvl.Header.SubLvlCount & "</SubLevelCount>")
                sb.AppendLine("<TileSetPath>media\tileset</TileSetPath>")
                'Write footer
                sb.AppendLine("</lvldata>")
                'Save file
                IO.File.WriteAllText(ContentMan.RootDirectory & "\debug\" & ID & "\dsc.ele", sb.ToString)


                '--------xxx_map.ele--------
                sb.Clear()
                'Write header
                sb.AppendLine("<?xml version=""1.0""?>")
                sb.AppendLine("<lvldata verH=""1"" verB=""2"" type=""Map"" key="""" alt=""False"" com=""Built for: " & cCurrentVersion & """>")
                sb.AppendLine("<width>" & Lvl.Map.Size.X & "</width>")
                sb.AppendLine("<height>" & Lvl.Map.Size.Y & "</height>")
                sb.AppendLine("<psX>" & Lvl.Map.Spawn.X & "</psX>")
                sb.AppendLine("<psY>" & Lvl.Map.Spawn.Y & "</psY>")
                sb.AppendLine("<StdHorizontal>" & Lvl.Map.StdCamPosition.X & "</StdHorizontal>")
                sb.AppendLine("<StdVertical>" & Lvl.Map.StdCamPosition.Y & "</StdVertical>")
                'Write data
                For Each element In Lvl.Map.BackLayer
                    If element.Value <> 0 Then
                        sb.AppendLine("<obj x=""" & element.Key.X & """ y=""" & element.Key.Y & """ z=""0"" tile=""" & element.Value & """/>")
                    End If
                Next
                For Each element In Lvl.Map
                    If element.Value <> 0 Then
                        sb.AppendLine("<obj x=""" & element.Key.X & """ y=""" & element.Key.Y & """ z=""1"" tile=""" & element.Value & """/>")
                    End If
                Next
                For Each element In Lvl.Map.FrontLayer
                    If element.Value <> 0 Then
                        sb.AppendLine("<obj x=""" & element.Key.X & """ y=""" & element.Key.Y & """ z=""2"" tile=""" & element.Value & """/>")
                    End If
                Next
                sb.AppendLine("<tiles>")
                For Each element In Lvl.Map.Tiles
                    sb.AppendLine("<obj ID=""" & element.Key.ToString & """ collision=""" & element.Value.Collision.ToString &
                              """ disablewalljump=""" & element.Value.DisableWallJump.ToString & """ originX=""" & element.Value.OriginRectangle.X & """ originY=""" & element.Value.OriginRectangle.Y &
                              """ originWidth=""" & element.Value.OriginRectangle.Width & """ originHeight=""" & element.Value.OriginRectangle.Height &
                              """ heightmap=""" & Convert.ToBase64String(element.Value.HeightMap) & """ slope=""" & element.Value.IsSlope.ToString & """/>")
                Next
                sb.AppendLine("</tiles>")


                sb.AppendLine("<tlg>")

                For i As Integer = 0 To Lvl.Map.TileGroups.Count - 1
                    Dim element As TileGroup = Lvl.Map.TileGroups(i)
                    sb.AppendLine("<group sizeX=""" & element.Size.X & """ sizeY=""" & element.Size.Y &
                              """ tile=""" & element.OriginalTile & """ originX=""" & element.Origin.X & """ originY=""" & element.Origin.Y &
                              """ originWidth=""" & element.Origin.Width & """ originHeight=""" & element.Origin.Height & """>")

                    sb.AppendLine("<map>")
                    For Each elementB In element.MapTiles
                        If elementB.Value <> 0 Then
                            sb.AppendLine("<obj x=""" & elementB.Key.X & """ y=""" & elementB.Key.Y & """ tile=""" & elementB.Value & """/>")
                        End If
                    Next
                    sb.AppendLine("</map>")
                    sb.AppendLine("</group>")

                Next
                sb.AppendLine("</tlg>")
                'Write footer
                sb.AppendLine("</lvldata>")
                'Save file
                IO.File.WriteAllText(ContentMan.RootDirectory & "\debug\" & ID & "\map_" & Lvl.Header.LoadedSublvl & ".ele", sb.ToString)



                '--------xxx_bg.ele--------
                sb.Clear()
                'Write header
                sb.AppendLine("<?xml version=""1.0""?>")
                sb.AppendLine("<lvldata verH=""1"" verB=""1"" type=""Background"" key="""" alt=""False"" com=""Built for: " & cCurrentVersion & """>")
                'Write data
                sb.AppendLine("<skybox>" & Lvl.Background.skyboxPath & "</skybox>")
                For Each element In Lvl.Background
                    sb.AppendLine("<obj type=""" & element.Type.ToString & """>")
                    Select Case element.Type
                        Case 0
                            sb.AppendLine("<property name=""layer"" value=""" & element.Layer.ToString & """/>")
                            sb.AppendLine("<property name=""path"" value=""" & element.Path.ToString & """/>")
                            sb.AppendLine("<property name=""location"" valueX=""" & element.Location.X.ToString & """ valueY=""" & element.Location.Y.ToString & """/>")
                            sb.AppendLine("<property name=""vectorscale"" value=""" & element.VectorScale.ToString & """/>")
                            sb.AppendLine("<property name=""scale"" valueX=""" & element.Size.X.ToString & """ valueY=""" & element.Size.Y.ToString & """/>")
                            sb.AppendLine("<property name=""color"" valueR=""" & element.Color.R.ToString & """ valueG=""" & element.Color.G.ToString &
                                      """ valueB=""" & element.Color.B.ToString & """ valueA=""" & element.Color.A.ToString & """/>")
                        Case 1
                            sb.AppendLine("<property name=""layer"" value=""" & element.Layer.ToString & """/>")
                            sb.AppendLine("<property name=""path"" value=""" & element.Path.ToString & """/>")
                            sb.AppendLine("<property name=""location"" valueX=""" & element.Location.X.ToString & """ valueY=""" & element.Location.Y.ToString & """/>")
                            sb.AppendLine("<property name=""vectorscale"" value=""" & element.VectorScale.ToString & """/>")
                            sb.AppendLine("<property name=""scale"" valueX=""" & element.Size.X.ToString & """ valueY=""" & element.Size.Y.ToString & """/>")
                            sb.AppendLine("<property name=""color"" valueR=""" & element.Color.R.ToString & """ valueG=""" & element.Color.G.ToString &
                                      """ valueB=""" & element.Color.B.ToString & """ valueA=""" & element.Color.A.ToString & """/>")
                    End Select
                    sb.AppendLine("</obj>")
                Next
                'Write footer
                sb.AppendLine("</lvldata>")
                'Save file
                IO.File.WriteAllText(ContentMan.RootDirectory & "\debug\" & ID & "\bg_" & Lvl.Header.LoadedSublvl & ".ele", sb.ToString)

                '--------xxx_trg.ele--------
                sb.Clear()
                'Write header
                sb.AppendLine("<?xml version=""1.0""?>")
                sb.AppendLine("<lvldata verH=""1"" verB=""1"" type=""Trigger"" key="""" alt=""False"" com=""Built for: " & cCurrentVersion & """>")
                'Write data
                For Each element In Lvl.TriggerMan
                    sb.AppendLine("<obj id=""" & element.Key & """ type=""" & element.Value.Type.ToString & """>")
                    Select Case element.Value.Type
                        Case TriggerType.Checkpoint
                            sb.AppendLine("<property name=""location"" valueX=""" & element.Value.Location.X.ToString & """ valueY=""" & element.Value.Location.Y.ToString & """/>")
                            sb.AppendLine("<property name=""length"" value=""" & element.Value.Length.ToString & """/>")
                            sb.AppendLine("<property name=""orientation"" value=""" & element.Value.Orientation.ToString & """/>")
                            sb.AppendLine("<property name=""ExecType"" value=""" & element.Value.ExecType.ToString & """/>")
                            sb.AppendLine("<property name=""vecarg"" valueX=""" & element.Value.VectorArgument.X.ToString & """ valueY=""" & element.Value.VectorArgument.Y.ToString & """/>")
                        Case TriggerType.CameraLock
                            sb.AppendLine("<property name=""location"" valueX=""" & element.Value.Location.X.ToString & """ valueY=""" & element.Value.Location.Y.ToString & """/>")
                            sb.AppendLine("<property name=""length"" value=""" & element.Value.Length.ToString & """/>")
                            sb.AppendLine("<property name=""orientation"" value=""" & element.Value.Orientation.ToString & """/>")
                            sb.AppendLine("<property name=""ExecType"" value=""" & element.Value.ExecType.ToString & """/>")
                            sb.AppendLine("<property name=""vecarg"" valueX=""" & element.Value.VectorArgument.X.ToString & """ valueY=""" & element.Value.VectorArgument.Y.ToString & """/>")
                        Case TriggerType.FinishLine
                            sb.AppendLine("<property name=""location"" valueX=""" & element.Value.Location.X.ToString & """ valueY=""" & element.Value.Location.Y.ToString & """/>")
                            sb.AppendLine("<property name=""length"" value=""" & element.Value.Length.ToString & """/>")
                            sb.AppendLine("<property name=""orientation"" value=""" & element.Value.Orientation.ToString & """/>")
                            sb.AppendLine("<property name=""ExecType"" value=""" & element.Value.ExecType.ToString & """/>")
                            sb.AppendLine("<property name=""vecarg"" valueX=""" & element.Value.VectorArgument.X.ToString & """ valueY=""" & element.Value.VectorArgument.Y.ToString & """/>")
                        Case Else
                            sb.AppendLine("<property name=""location"" valueX=""" & element.Value.Location.X.ToString & """ valueY=""" & element.Value.Location.Y.ToString & """/>")
                            sb.AppendLine("<property name=""length"" value=""" & element.Value.Length.ToString & """/>")
                            sb.AppendLine("<property name=""orientation"" value=""" & element.Value.Orientation.ToString & """/>")
                            sb.AppendLine("<property name=""ExecType"" value=""" & element.Value.ExecType.ToString & """/>")
                    End Select
                    sb.AppendLine("</obj>")
                Next
                'Write footer
                sb.AppendLine("</lvldata>")
                'Save file
                IO.File.WriteAllText(ContentMan.RootDirectory & "\debug\" & ID & "\trg_" & Lvl.Header.LoadedSublvl & ".ele", sb.ToString)

                '--------xxx_fx.ele--------
                sb.Clear()
                'Write header
                sb.AppendLine("<?xml version=""1.0""?>")
                sb.AppendLine("<lvldata verH=""1"" verB=""1"" type=""FX"" key="""" alt=""False"" com=""Built for: " & cCurrentVersion & """>")
                sb.AppendLine("<fx active=""" & Lvl.FXData.EnableLighting.ToString & """ r=""" & Lvl.FXData.AmbientColor.R.ToString &
                          """ g=""" & Lvl.FXData.AmbientColor.G.ToString & """ b=""" & Lvl.FXData.AmbientColor.B.ToString & """ a=""" & Lvl.FXData.AmbientColor.A.ToString & """>")
                'Write hulls
                sb.AppendLine("<hulls>")
                For Each element In Lvl.FXData.Hulls
                    sb.AppendLine("<obj>")
                    For i As Integer = 0 To element.Points.Count - 1
                        sb.AppendLine("<vertex x=""" & element.Points(i).X & """ y=""" & element.Points(i).Y & """/>")
                    Next
                    sb.AppendLine("</obj>")
                Next
                sb.AppendLine("</hulls>")
                'Write light sources
                sb.AppendLine("<src>")
                For Each element In Lvl.FXData.Lights

                    sb.AppendLine("<obj type=""PointLight"" CastsShadows=""" & element.CastsShadows.ToString & """ Radius=""" & element.Radius.ToString &
                               """ Intensity=""" & element.Intensity.ToString & """ ScaleX=""" & element.Scale.X.ToString & """ ScaleY=""" &
                              element.Scale.Y.ToString & """ PositionX=""" & element.Position.X.ToString & """ PositionY=""" & element.Position.Y.ToString &
                              """ OriginX=""" & element.Origin.X.ToString & """ OriginY=""" & element.Origin.Y.ToString &
                              """ ShadowType=""" & element.ShadowType.ToString & """ r=""" & element.Color.R.ToString & """ g=""" & element.Color.G.ToString &
                               """ b=""" & element.Color.B.ToString & """ a=""" & element.Color.A.ToString & """/>")
                Next
                sb.AppendLine("</src>")
                'Write bloom

                sb.AppendLine("<bloom activate=""" & Lvl.FXData.EnableBloom.ToString & """ preset=""" & Lvl.FXData.BloomPreset.ToString & """ threshold=""" &
                          Lvl.FXData.BloomThreshold.ToString & """ strength=""" & Lvl.FXData.BloomStrengthMultiplier.ToString & """/>")
                'Write footer
                sb.AppendLine("</fx>")
                sb.AppendLine("</lvldata>")
                'Save file
                IO.File.WriteAllText(ContentMan.RootDirectory & "\debug\" & ID & "\fx_" & Lvl.Header.LoadedSublvl & ".ele", sb.ToString)

                '--------xxx_obj.ele--------
                sb.Clear()
                'Write header
                sb.AppendLine("<?xml version=""1.0""?>")
                sb.AppendLine("<lvldata verH=""1"" verB=""1"" type=""Objects"" key="""" alt=""False"" com=""Built for: " & cCurrentVersion & """>")
                'Write data
                For Each element In Lvl.ItemMan
                    Select Case element.Value.Type
                        Case 3
                            sb.AppendLine("<obj x=""" & element.Value.CenterLocation.X & """ y=""" & element.Value.CenterLocation.Y & """ group=""item"" ID=""" & element.Key & """ type=""" & element.Value.Type & """ destination=""" & CType(element.Value, ObjectManager.Objects.SublevelDoor).SublvlDestination.ToString & """/>")
                        Case Else
                            sb.AppendLine("<obj x=""" & element.Value.CenterLocation.X & """ y=""" & element.Value.CenterLocation.Y & """ group=""item"" ID=""" & element.Key & """ type=""" & element.Value.Type & """/>")
                    End Select
                Next
                For i As Integer = 0 To Lvl.Entities.Count - 1
                    If i > 0 Then
                        Dim entity As Entities.Entity = Lvl.Entities(i)
                        sb.AppendLine("<obj x=""" & entity.mSpawn.X & """ y=""" & entity.mSpawn.Y & """ group=""entity"" ID=""" & entity.mAttributes.UniqueID & """ type=""" & entity.mAttributes.ID & """/>")
                    End If
                Next

                'Write footer
                sb.AppendLine("</lvldata>")
                'Save file
                IO.File.WriteAllText(ContentMan.RootDirectory & "\debug\" & ID & "\obj_" & Lvl.Header.LoadedSublvl & ".ele", sb.ToString)
            Catch ex As Exception
                MessageBox.Show("Error while saving", ex.ToString, {"OK"})
            End Try
        End Sub

        Public Shared Function GenerateTestLevel(Optional blank As Boolean = False) As Level 'Generates a test level for the PlayGround Class
            Dim res As New Level With {
                .Header = New LevelHeader With {.SubLvlCount = 1, .LoadedSublvl = 0, .Loaded = True},
                .Background = New BackgroundManager,
                .TriggerMan = New Dictionary(Of UInteger, Trigger),
                .Camera = New Camera.Camera(True) With {.DebugCam = False, .AllowHorizontal = True, .AllowVertical = True},
                .Map = New TileSet With {.Tiles = New Dictionary(Of UShort, Tile) From {
                {1, New Tile With {.Collision = TileCollisionType.Solid, .OriginRectangle = New Rectangle(0, 0, 1, 1)}}}, .TileGroups = New List(Of TileGroup), .TileSpriteSheet = ReferencePixel,
                .Size = New Vector2(Math.Ceiling(GameSize.X / cTileSize), 32), .Spawn = New Vector2(4, 4.5)},
                .ItemMan = New ObjectManager.ObjectManager,
                .Entities = New List(Of Entities.Entity),
                .FXData = New FXData,
                .MSFX = New MSFXcontainer
            }

            If Not blank Then
                'Generate Floor
                For x As Integer = 1 To Math.Ceiling(GameSize.X / cTileSize) + 1
                    res.Map.Add(New Vector2(x, 0), 1)
                Next
                For y As Integer = 1 To Math.Ceiling(GameSize.Y / cTileSize)
                    res.Map.Add(New Vector2(0, y), 1)
                    res.Map.Add(New Vector2((GameSize.X / cTileSize) + 2, y), 1)
                Next
            End If
            Return res
        End Function

        Public Shared Sub DeleteSublvl(lvl As Level)
            File.Delete(ContentMan.RootDirectory & "\debug\" & lvl.Header.LoadedID & "\map_" & lvl.Header.LoadedSublvl & ".ele")
            File.Delete(ContentMan.RootDirectory & "\debug\" & lvl.Header.LoadedID & "\bg_" & lvl.Header.LoadedSublvl & ".ele")
            File.Delete(ContentMan.RootDirectory & "\debug\" & lvl.Header.LoadedID & "\fx_" & lvl.Header.LoadedSublvl & ".ele")
            File.Delete(ContentMan.RootDirectory & "\debug\" & lvl.Header.LoadedID & "\obj_" & lvl.Header.LoadedSublvl & ".ele")
            File.Delete(ContentMan.RootDirectory & "\debug\" & lvl.Header.LoadedID & "\trg_" & lvl.Header.LoadedSublvl & ".ele")
        End Sub
    End Class
End Namespace