Imports System
Imports System.IO
Imports System.Linq
Imports System.Text
Imports Microsoft.Xna.Framework
Namespace Framework.Misc

    Public Module StaticFunctions
        Public Const DegToRad = Math.PI / 180
        Public Function RotateVector(vec As Vector2, radians As Double) As Vector2
            Dim ca As Double = Math.Cos(radians)
            Dim sa As Double = Math.Sin(radians)
            Return New Vector2(ca * vec.X - sa * vec.Y, sa * vec.X + ca * vec.Y)
        End Function
        Public Function RotateAboutOrigin(ByVal point As Vector2, ByVal origin As Vector2, ByVal rotation As Single) As Vector2
            Dim u = point - origin
            If u = Vector2.Zero Then Return point
            Dim a = CSng(Math.Atan2(u.Y, u.X))
            a += rotation
            u = u.Length() * New Vector2(Math.Cos(a), Math.Sin(a))
            Return u + origin
        End Function
        Public Function WrapTextDifferently(ByVal text As String, ByVal width As Integer, ByVal overflow As Boolean) As String
            Dim result As StringBuilder = New StringBuilder()
            Dim index As Integer = 0
            Dim column As Integer = 0

            While index < text.Length
                Dim spaceIndex As Integer = text.IndexOfAny({" "c, Microsoft.VisualBasic.vbTab, Microsoft.VisualBasic.vbCr, Microsoft.VisualBasic.vbLf}, index)

                If spaceIndex = -1 Then
                    Exit While
                ElseIf spaceIndex = index Then
                    index += 1
                Else
                    AddWord(text.Substring(index, spaceIndex - index), width, overflow, column, result)
                    index = spaceIndex + 1
                End If
            End While

            If index < text.Length Then AddWord(text.Substring(index), width, overflow, column, result)
            Return result.ToString()
        End Function

        Private Sub AddWord(ByVal word As String, ByVal width As Integer, ByVal overflow As Boolean, ByRef column As Integer, ByRef result As StringBuilder)
            If Not overflow AndAlso word.Length > width Then
                Dim wordIndex As Integer = 0

                While wordIndex < word.Length
                    Dim subWord As String = word.Substring(wordIndex, Math.Min(width, word.Length - wordIndex))
                    AddWord(subWord, width, overflow, column, result)
                    wordIndex += subWord.Length
                End While
            Else

                If column + word.Length >= width Then

                    If column > 0 Then
                        result.AppendLine()
                        column = 0
                    End If
                ElseIf column > 0 Then
                    result.Append(" ")
                    column += 1
                End If

                result.Append(word)
                column += word.Length
            End If
        End Sub

        Public Function RemIllegalChars(text As String, font As NezSpriteFont) As String
            Dim result As String = text
            For Each element In text.Distinct
                If Not font.HasCharacter(element) Or element = "'"c Then result = result.Replace(element, "#"c)
            Next
            Return result
        End Function
        Public Function interpolate(ByVal d1 As Double, ByVal d2 As Double, ByVal dPercentage As Double) As Double
            Dim dDifference As Double = d2 - d1
            Dim dDistance As Double = dDifference * dPercentage
            Dim dResult As Double = d1 + dDistance
            Return dResult
        End Function

        Public Function interpolate(ByVal i1 As Integer, ByVal i2 As Integer, ByVal dPercentage As Double) As Integer
            Return interpolate(i1, CDbl(i2), dPercentage)
        End Function

        Public Function interpolate(ByVal f1 As Single, ByVal f2 As Single, ByVal dPercentage As Double) As Single
            Return interpolate(f1, CDbl(f2), dPercentage)
        End Function

        Public Function convertLinearToEaseInEaseOut(dElapsed As Double) As Double
            Dim dFirstHalfTime As Double = If((dElapsed > 0.5), 0.5, dElapsed)
            Dim dSecondHalfTime As Double = If((dElapsed > 0.5), dElapsed - 0.5, 0.0)
            Dim dResult As Double = 2 * dFirstHalfTime * dFirstHalfTime + 2 * dSecondHalfTime * (1.0 - dSecondHalfTime)
            Return dResult
        End Function

        Public Function convertLinearToAcceleration(dElapsed As Double) As Double
            Return dElapsed * dElapsed
        End Function

        Public Function convertLinearToDeceleration(dElapsed As Double) As Double
            Return dElapsed * (2.0 - dElapsed)
        End Function
        Public Function FilenameIsOK(fileName As String) As Boolean
            Dim file As String = Path.GetFileName(fileName)
            Dim directory As String = Path.GetDirectoryName(fileName)

            Return Not (file.Intersect(Path.GetInvalidFileNameChars()).Any() _
                OrElse
                directory.Intersect(Path.GetInvalidPathChars()).Any())
        End Function

        Public Function PointsToRectangle(a As Vector2, b As Vector2, inflation As Vector2) As Rectangle
            Dim smallestX As Integer = Math.Min(a.X, b.X)
            Dim smallestY As Integer = Math.Min(a.Y, b.Y)
            Dim largestX As Integer = Math.Max(a.X, b.X)
            Dim largestY As Integer = Math.Max(a.Y, b.Y)
            Dim width As Integer = largestX - smallestX
            Dim height As Integer = largestY - smallestY
            Dim res As New Rectangle(smallestX - inflation.X, smallestY - inflation.Y, width + inflation.X * 2, height + inflation.Y * 2)
            Return res
        End Function

    End Module

End Namespace
