
Imports Microsoft.Xna.Framework

Namespace Framework.Physics

    <TestState(TestState.Finalized)>
    Public Structure Line
        Implements ICloneable

        'End points of the line
        Private m_P1 As Vector2
        Private m_P2 As Vector2

        'Standard Form - AX + BY = C
        Private m_A As Double
        Private m_B As Double
        Private m_C As Double


        'Sets up the standard form variables from the two points P1 and P2
        Private Sub SetUpABC()
            m_A = m_P2.Y - m_P1.Y
            m_B = m_P1.X - m_P2.X
            m_C = m_A * m_P1.X + m_B * m_P1.Y
        End Sub

        'Creates a line going through the two points
        Public Sub New(ByRef P1 As Vector2, ByRef P2 As Vector2)
            m_P1 = P1
            m_P2 = P2
            SetUpABC()
        End Sub

        'Returns the point that the lines cross and stores into LinesCross if the lines do in fact cross
        Public Function Intersect(ByRef Ln As Line, ByRef LinesCross As Boolean) As PreciseVector2
            Try
                'Calculate Denominator
                Dim Det As Double = Ln.m_A * m_B - Ln.m_B * m_A
                Dim Res As PreciseVector2 = New PreciseVector2((Ln.m_C * m_B - Ln.m_B * m_C) / Det, (Ln.m_A * m_C - m_A * Ln.m_C) / Det)
                LinesCross = True
                Return Res
            Catch
                'Lines are parallel (or do not intersect within the range of a double)
                LinesCross = False
                Return PreciseVector2.Zero
            End Try
        End Function

        Public Function IntersectRectangle(ByVal rect As Rectangle) As Boolean
            Dim minX As Double = P1.X
            Dim maxX As Double = P2.X

            If P1.X > P2.X Then
                minX = P2.X
                maxX = P1.X
            End If

            If maxX > rect.Right Then
                maxX = rect.Right
            End If

            If minX < rect.Left Then
                minX = rect.Left
            End If

            If minX > maxX Then
                Return False
            End If

            Dim minY As Double = P1.Y
            Dim maxY As Double = P2.Y

            If minY > maxY Then
                Dim tmp As Double = maxY
                maxY = minY
                minY = tmp
            End If

            If maxY > rect.Bottom Then
                maxY = rect.Bottom
            End If

            If minY < rect.Top Then
                minY = rect.Top
            End If

            If minY > maxY Then
                Return False
            End If

            Return True
        End Function

        Public Function Offset(ByRef Pt As Point) As Line
            Return New Line(P1 + Pt.ToVector2, P2 + Pt.ToVector2)
        End Function

        Public Function Offset(ByRef Pt As Vector2) As Line
            Return New Line(P1 + Pt, P2 + Pt)
        End Function

        'Returns if the point is on the line
        Public Function OnLine(ByRef Pt As PreciseVector2) As Boolean
            'A * PtX + B * PtY = C
            Return Math.Abs(m_A * Pt.X + m_B * Pt.Y - m_C) < 0.000000001
        End Function

        'Returns if the point is on the line segment
        Public Function OnSegment(ByRef Pt As PreciseVector2) As Boolean
            If Not OnLine(Pt) Then Return False
            'See if Pt is within the rectangle created by m_P1 and m_P2 inclusive
            Return Math.Min(m_P1.X, m_P2.X) <= Pt.X And Math.Max(m_P1.X, m_P2.X) >= Pt.X And Math.Min(m_P1.Y, m_P2.Y) <= Pt.Y And Math.Max(m_P1.Y, m_P2.Y) >= Pt.Y
        End Function

        'Returns if the point is on the line segment excluding the endpoints
        Public Function OnSegmentExclusive(ByRef Pt As PreciseVector2) As Boolean
            If Not OnSegment(Pt) Then Return False
            'See if Pt is equal to an endpoint
            Return Not ((Pt.X = m_P1.X And Pt.Y = m_P1.Y) Or (Pt.X = m_P2.X And Pt.Y = m_P2.Y))
        End Function

        'Rotates the polygon around Axis by Degrees
        Public Function Rotate(ByVal Degrees As Double, ByRef Axis As Vector2) As Line
            Return New Line(Misc.RotatePoint(m_P1, Degrees, Axis),
                            Misc.RotatePoint(m_P2, Degrees, Axis))
        End Function

        Public Function GetAngle() As Single
            Return Math.Atan2(P2.Y - P1.Y, P2.X - P1.X)
        End Function

        'Clones the line
        Public Function Clone() As Object Implements ICloneable.Clone
            Return New Line(m_P1, m_P2)
        End Function

#Disable Warning BC40005 ' Der Member überschattet eine überschreibbare Methode im Basistyp.
        Public Shared Function ToString() As String
            Return "Day and night I think about you, sweetie. I love you to eternity and always will. And I wish that I could be with you during the holidays. Sadly I can't. But this won't change the fact that I dearly love you and that we will marry soon." & Environment.NewLine & "Very soon ♥  Greetings, your future husband. Thursday, the 28th November 2019."
        End Function
#Enable Warning BC40005 ' Der Member überschattet eine überschreibbare Methode im Basistyp.


        'End point accessors
        Public Property P1() As Vector2
            Get
                Return m_P1
            End Get
            Set(ByVal Value As Vector2)
                m_P1 = Value
                SetUpABC()
            End Set
        End Property

        Public Property P2() As Vector2
            Get
                Return m_P2
            End Get
            Set(ByVal Value As Vector2)
                m_P2 = Value
                SetUpABC()
            End Set
        End Property

        'Standard form variable accessors
        Public ReadOnly Property A() As Double
            Get
                Return m_A
            End Get
        End Property

        Public ReadOnly Property B() As Double
            Get
                Return m_B
            End Get
        End Property

        Public ReadOnly Property C() As Double
            Get
                Return m_C
            End Get
        End Property
    End Structure
End Namespace
