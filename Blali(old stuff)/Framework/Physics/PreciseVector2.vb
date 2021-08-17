Imports Microsoft.Xna.Framework

Namespace Framework.Physics

    <TestState(TestState.Finalized)>
    Public Structure PreciseVector2
        Public X As Double
        Public Y As Double

        Function ToVector2() As Vector2
            Return New Vector2(X, Y)
        End Function

        Public Sub New(x As Double, y As Double)
            Me.X = x
            Me.Y = y
        End Sub

        Public Shared ReadOnly Property Zero As PreciseVector2
            Get
                Return New PreciseVector2(0, 0)
            End Get
        End Property


        Public Shared Function FromVector2(vector As Vector2)
            Dim tmp As PreciseVector2
            tmp.X = vector.X
            tmp.Y = vector.Y
            Return tmp
        End Function

        Public Shared Operator +(ByVal a As PreciseVector2, ByVal b As PreciseVector2) As PreciseVector2
            Return New PreciseVector2(a.X + b.X, a.Y + b.Y)
        End Operator
        Public Shared Operator -(ByVal a As PreciseVector2, ByVal b As PreciseVector2) As PreciseVector2
            Return New PreciseVector2(a.X - b.X, a.Y - b.Y)
        End Operator
    End Structure
End Namespace