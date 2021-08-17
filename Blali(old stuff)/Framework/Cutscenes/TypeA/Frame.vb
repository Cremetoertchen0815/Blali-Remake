Imports System.Collections.Generic
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Cutscenes.TypeA

    <TestState(TestState.WorkInProgress)>
    Public Class Frame
        Inherits List(Of Field)

        Public Property Description As String = "None"
        Public Property FadeTime As Integer
        Public Property WaitTime As Integer
        Public Property Blackout As Boolean
        Public Property StartPos As CamKeyframe
        Public Property EndPos As CamKeyframe
        Public Property MovementTransition As TransitionType = TransitionType.Linear

        Public Sub Load(textatlas As List(Of Texture2D))
            For i As Integer = 0 To Me.Count - 1
                Dim field As Field = Me(i)
                field.Texture = textatlas(field.TextureNr)
                Me(i) = field
            Next
            Sort()
        End Sub


        Public Sub SetNLoad(texturenr As Integer, textatlas As List(Of Texture2D))
            For i As Integer = 0 To Me.Count - 1
                Dim field As Field = Me(i)
                field.TextureNr = texturenr
                field.Texture = textatlas(field.TextureNr)
                Me(i) = field
            Next

            Sort()
        End Sub

        Public Function Clone()
            Return Me.MemberwiseClone
        End Function
    End Class

    Public Enum TransitionType
        Acceleration
        Bounce
        CriticalDamping
        Deceleration
        EaseInEaseOut
        Flash
        Linear
        ThrowAndCatch
    End Enum
End Namespace