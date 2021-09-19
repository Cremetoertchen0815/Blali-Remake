
Imports Microsoft.Xna.Framework

Namespace Framework.Physics

    <TestState(TestState.NearCompletion)>
    Public Structure Segment
        Public P1 As Vector2
        Public P2 As Vector2
        Public Offset As Single
        Public ReadOnly Property Length As Single
            Get
                Return Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2))
            End Get
        End Property

        Public Function GetSubLocation(len As Single) As Vector2
            Dim slen As Single = len - Offset
            Dim Dir As Vector2 = (P2 - P1) / Length
            Return P1 + Dir * slen
        End Function
    End Structure
End Namespace