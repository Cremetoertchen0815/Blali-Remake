Imports Microsoft.Xna.Framework

Namespace IG
    Public Class RankHelper
        Public Shared Function ByScore(score As Integer) As Rank
            If score < cScoreRankC Then
                Return Rank.D
            ElseIf score < cScoreRankB Then
                Return Rank.C
            ElseIf score < cScoreRankA Then
                Return Rank.B
            ElseIf score < cScoreRankS Then
                Return Rank.A
            Else
                Return Rank.S
            End If
        End Function

        Public Shared Function GetColor(Rank As Rank) As Color
            Select Case Rank
                Case Rank.S
                    Return New Color(190, 160, 0, 255)
                Case Rank.A
                    Return New Color(0, 200, 40, 255)
                Case Rank.B
                    Return New Color(0, 150, 130, 255)
                Case Rank.C
                    Return New Color(35, 60, 255, 255)
                Case Rank.D
                    Return New Color(250, 20, 160, 255)
            End Select
        End Function
    End Class
    Public Enum Rank
        S
        A
        B
        C
        D
    End Enum
End Namespace