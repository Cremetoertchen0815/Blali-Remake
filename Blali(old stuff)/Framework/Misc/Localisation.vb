Imports System.Collections.Generic
Imports Emmond.Framework.Content
Imports Microsoft.Xna.Framework.Content
Imports Newtonsoft.Json

Namespace Framework.Misc

    <TestState(TestState.Finalized)>
    Public Class Localisation
        Public Shared Function GetLocalisationData(code As String) As Dictionary(Of UInteger, String)
            Dim aa As String = ContentMan.Load(Of XNBStringContainer)("text\ge_" & code).s
            Return JsonConvert.DeserializeObject(Of Dictionary(Of UInteger, String))(aa)
        End Function

        Public Shared Function GetLangName(dic As Dictionary(Of UInteger, String), lang As Integer) As String
            Select Case CType(lang, Languages)
                Case Languages.en
                    Return dic.Item(15)
                Case Languages.de
                    Return dic.Item(16)
                Case Languages.sw
                    Return dic.Item(17)
                Case Languages.fn
                    Return dic.Item(41)
                Case Else
                    Return dic.Item(15)
            End Select
        End Function

        Public Shared Function GetConditionName(dic As Dictionary(Of UInteger, String), condition As Boolean) As String
            If condition Then
                Return dic.Item(18)
            Else
                Return dic.Item(19)
            End If
        End Function

        Public Enum Languages
            en = 0
            de = 1
            sw = 2
            fn = 3
        End Enum
    End Class

    Public Class TxtReader
        Inherits ContentTypeReader(Of XNBStringContainer)

        Protected Overrides Function Read(input As ContentReader, existingInstance As XNBStringContainer) As XNBStringContainer
            Return New XNBStringContainer(input.ReadString)
        End Function
    End Class
End Namespace