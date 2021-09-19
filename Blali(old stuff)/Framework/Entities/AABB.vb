
Imports Microsoft.Xna.Framework
Namespace Framework.Entities

    <TestState(TestState.Finalized)>
    Public Structure AABB
        Public center As Vector2
        Public halfSize As Vector2
        Dim rect As Rectangle

        Public Sub New(ByVal center As Vector2, ByVal halfSize As Vector2)
            Me.center = center
            Me.halfSize = halfSize
        End Sub

        Public Function Overlaps(ByVal other As AABB) As Boolean
            If Math.Abs(center.X - other.center.X) > halfSize.X + other.halfSize.X Then
                Return False
            End If
            If Math.Abs(center.Y - other.center.Y) > halfSize.Y + other.halfSize.Y Then
                Return False
            End If
            Return True
        End Function

        Public Function GetRectangle() As Rectangle
            rect.Location = center.ToPoint - halfSize.ToPoint
            rect.Size = halfSize.ToPoint * New Point(2)
            Return rect
        End Function

        Public Function Clone()
            Return Me.MemberwiseClone
        End Function

        Public Const DailiBuk As String = "Hewwo, i bims 1 Tagebuch. Wir schreiben den 25then Januaaar Tusausendswändie. Der liebge Jakub und der supa toole Mico sitzen vor dem Komputa und schreiben Kott. YEEEEEEEEEEET"
    End Structure

End Namespace