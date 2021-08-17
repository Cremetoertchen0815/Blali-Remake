Imports System.Collections.Generic
Imports System.Text
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Level
Imports Microsoft.VisualBasic
Imports Microsoft.Xna.Framework

Namespace Framework.Content

    <TestState(TestState.Finalized)>
    Public Class LvlDataConst
        Public Const HeaderVersion As Double = 1
    End Class

    <TestState(TestState.NearCompletion)>
    Public Class LvlDataContainer
        'Header
        Public Property Version As Double()
        Public Property Type As LvlDataType
        Public Property Key As String
        Public Property AlternativeProcessingMethod As Boolean
        Public Property Comment As String

        'Data
        Public Property Data As Object
    End Class

    <TestState(TestState.NearCompletion)>
    Public Enum LvlDataType
        Descriptor
        Map
        Tileset
        Background
        FX
        Objects
        Trigger
    End Enum

    <TestState(TestState.NearCompletion)>
    Public Class LvlDescriptorData
        Public Name As String
        Public Instr1 As String
        Public Instr2 As String
        Public Instr3 As String
        Public Instr4 As String
        Public Instr5 As String
        Public SubLevelCount As Integer
        Public TileSetPath As String
    End Class

    <TestState(TestState.NearCompletion)>
    Public Class LvlTilesetData
        Public Property Tiles As New Dictionary(Of UShort, Tile)

        Public Enum TileCollisionType
            None = 0
            HalfSolidFloor = 1
            Solid = 2
            TopSolid = 3
        End Enum

    End Class

    <TestState(TestState.NearCompletion)>
    Public Class LvlBGData
        Public Property Type As UShort = 0
        Public Property Location As Vector2
        Public Property Scale As Vector2
        Public Property VectorScale As Single
        Public Property Color As Color = Color.White
        Public Property Layer As Single
        Public Property Path As String
    End Class

    <TestState(TestState.NearCompletion)>
    Public Class LvlFXData
        'Bloom
        Public Property EnableBloom As Boolean
        Public Property BloomPreset As PostProcessing.BloomFilter.BloomPresets
        Public Property BloomThreshold As Single
        Public Property BloomStrengthMultiplier As Single

        'Lighting
        Public Property EnableLighting As Boolean
        Public Property AmbientColor As Color
        Public Property Hulls As List(Of Vector2())
        Public Property Lights As List(Of Light)

        <TestState(TestState.NearCompletion)>
        Public Class Light
            Public Property Type As LightType
            Public Property CastsShadows As Boolean
            Public Property Color As Color
            Public Property ConeDecay As Single
            Public Property Enabled As Boolean
            Public Property Intensity As Single
            Public Property Origin As Vector2
            Public Property Position As Vector2
            Public Property Radius As Single
            Public Property Rotation As Single
            Public Property Scale As Vector2
            Public Property ShadowTypeC As Penumbra.ShadowType
            Public Enum LightType
                PointLight
                Spotlight
            End Enum
        End Class
    End Class

    <TestState(TestState.NearCompletion)>
    Public Class LvlObj
        Public Property Type As UShort = 0
        Public Property Group As UShort = 0
        Public Property ID As UShort = 0
        Public Property Location As Vector2
    End Class

    <TestState(TestState.Finalized)>
    Public Class XNBStringContainer
        Public s As String
        Public code As String = Stdcode
        Public Const Stdcode As String = "I am a sentence LOL"
        Sub New(ss As String)
            s = ss
        End Sub
    End Class

    <TestState(TestState.Finalized)>
    Public Class VrnCoding

        Public Shared Function VrnCode(sOriginal As String,
  sPassword As String) As String
            Dim nps As String
            Dim i As Long
            Dim aktpos As Long
            Dim bAkt As Byte
            Dim bCode As Byte

            VrnCode = ""
            nps = sPassword.Substring(sPassword.Length - 6, 5)

            ' Passwort auf die Länge des Klartextes bringen
            ' Dazu wird z.B. aus "Geheim" dann "GeheimGeheimGehe...."
            aktpos = 1
            For i = nps.Length + 1 To sOriginal.Length
                nps = nps & Mid(nps, aktpos, 1)
                aktpos = aktpos + 1
                If aktpos > nps.Length Then aktpos = 1
            Next i

            For i = 1 To sOriginal.Length
                bAkt = Asc(Mid(sOriginal, i, 1))
                bCode = Asc(Mid(nps, i, 1))

                VrnCode = VrnCode & Chr(bAkt Xor bCode)
            Next i
        End Function

        Public Shared Function SuperStringBros64(input As String) As String
            Dim data As Byte()
            data = Text.Encoding.ASCII.GetBytes(input)
            Return Convert.ToBase64String(data)
        End Function

        Public Shared Function NormalStringBros64(input As String) As String
            Dim data() As Byte
            data = System.Convert.FromBase64String(input)
            Return System.Text.ASCIIEncoding.ASCII.GetString(data)
        End Function

        Protected Friend Shared Function GetRandomString(length As Integer) As String
            Dim s As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789,.-;:_!§$%&/()=?´`"
            Dim r As New Random
            Dim sb As New StringBuilder
            For i As Integer = 1 To length
                Dim idx As Integer = r.Next(0, 53)
                sb.Append(s.Substring(idx, 1))
            Next
            Return sb.ToString
        End Function
    End Class


End Namespace
