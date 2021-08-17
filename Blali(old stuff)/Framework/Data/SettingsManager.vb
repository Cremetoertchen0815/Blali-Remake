Imports System.IO

Namespace Framework.Data
    <TestState(TestState.WorkInProgress)>
    Public Class SettingsManager
        Const version As Byte = 4

        Public EndMyLife As Boolean = False 'Switch to the weird screen
        Dim Data As Byte()
        Dim CurrentGUID As Guid
        Friend ReadOnly path As String = ""
        Friend ReadOnly folder As String = ""
        Sub New()
            Try
                folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\Emmond"
                path = folder & "\config.dat"
                If Not File.Exists(path) Then GenerateFile()
                Data = IO.File.ReadAllBytes(path)
                If Data(1) <> version Then Throw New Exception("Savefile version doesn't correspond")
                CurrentGUID = New Guid({Data(2), Data(3), Data(4), Data(5), Data(6), Data(7), Data(8), Data(9), Data(10), Data(11), Data(12), Data(13), Data(14), Data(15), Data(16), Data(17)})
            Catch ex As Exception
                If cIgnoreBadSavefile Then GenerateFile() Else EndMyLife = True
            End Try
        End Sub

        Public Property ValueBool(key As Short, Optional AutoSave As Boolean = False) As Boolean
            Get
                Return Data(key) >= 1
            End Get
            Set(value As Boolean)
                If value Then Data(key) = 1 Else Data(key) = 0
                If AutoSave Then Save()
            End Set
        End Property

        Public Property Value(key As Short, Optional AutoSave As Boolean = False) As Byte
            Get
                Return Data(key)
            End Get
            Set(value As Byte)
                Data(key) = value
                If AutoSave Then Save()
            End Set
        End Property

        Sub Save()
            If Not Directory.Exists(folder) Then Directory.CreateDirectory(folder)
            File.WriteAllBytes(path, Data)
        End Sub

        Public Sub Regenerate()
            GenerateFile(version)
        End Sub

        Friend Function ReturnData() As Byte()
            Return Data
        End Function

        Private Sub GenerateFile(Optional VersionS As Byte = version)
            Select Case VersionS
                Case 4
                    CurrentGUID = Guid.NewGuid
                    Dim GUIDBytes As Byte() = CurrentGUID.ToByteArray
                    Data = Nothing
                    ReDim Data(480)
                    '---Header($0-$17)---
                    Value(0) = 226
                    Value(1) = VersionS
                    For i As Integer = 0 To 15
                        Value(2 + i) = GUIDBytes(i)
                    Next
                    '---General Settings($18-$39)---
                    ValueBool(18) = True
                    Value(19) = 0
                    ValueBool(20) = 1
                    Value(21) = ResolutionList.Length - 1
                    ValueBool(22) = False
                    ValueBool(23) = True
                    ValueBool(24) = False
                    ValueBool(25) = False
                    Value(26) = 1
                    Value(27) = 0
                    Value(28) = 18
                    ValueBool(29) = False
                    ValueBool(30) = False
                    Value(31) = 70
                    Value(32) = 100
                    '---Controls($40-$79)---
                    Value(40) = 1
                    Value(41) = 0
                    Value(42) = 1
                    Value(43) = 2
                    Value(44) = 255
                    Value(45) = 9
                    Value(46) = 8
                    Value(47) = 2
                    Value(48) = 1
                    Value(49) = 3
                    Value(50) = 0
                    Value(51) = 6
                    Value(52) = 7
                    Value(53) = 4
                    Value(54) = 5
                    Value(55) = 0
                    Value(56) = 13
                    Value(57) = 0
                    Value(58) = 70
                    Value(59) = 0
                    Value(60) = 32
                    Value(61) = 255
                    Value(62) = 3
                    Value(63) = 0
                    Value(64) = 69
                    Value(65) = 0
                    Value(66) = 81
                    Value(67) = 255
                    Value(68) = 6
                    Value(69) = 255
                    Value(70) = 7
                    Value(71) = 255
                    Value(72) = 1
                    Value(73) = 0
                    Value(74) = 160
                    Value(75) = 0
                    Value(76) = 0
                    Value(77) = 0
                    Value(78) = 0
                    Value(79) = 0
                    '---RESERVED($80-$89)---
                    Value(80) = 2
                    Value(81) = 2
                    Value(82) = 0
                    Value(83) = 6
                    Value(84) = 2
                    Value(85) = 0
                    Value(86) = 0
                    Value(87) = 3
                    Value(88) = 245
                    Value(89) = 69
                    '---Player Backpack($90-$143)---
                    For i As Integer = 90 To 143
                        Value(i) = 0
                    Next
                    '---Unlockables + General Gameplay Flags($144-$159)---
                    For i As Integer = 144 To 159
                        ValueBool(i) = True
                    Next
                    '---Power-Ups unlocked($160-$175)---
                    For i As Integer = 160 To 175
                        ValueBool(i) = False
                    Next
                    '---Story flags($176-$299)---
                    For i As Integer = 160 To 175
                        ValueBool(i) = False
                    Next
                    Value(176) = CByte(DateTime.Now.Day)
                    Value(177) = CByte(DateTime.Now.Month)
                    Dim tmp As Long
                    Value(178) = CByte(Math.DivRem(DateTime.Now.Year, Byte.MaxValue, tmp))
                    Value(179) = CByte(tmp)
                    Value(180) = CByte(DateTime.Now.Hour)
                    Value(181) = CByte(DateTime.Now.Minute)
                    Value(182) = 0
                    Value(183) = 0
                    Value(184) = 2
                    Value(185) = 0
                    For i As Integer = 186 To 299
                        Value(i) = 0
                    Next

                    For i As Integer = 300 To 480
                        Value(i) = 0
                    Next
                    Save()
            End Select
        End Sub

        Friend Sub ResetSavefile()
            GenerateFile()
        End Sub

    End Class
End Namespace