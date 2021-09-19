Namespace Framework.SceneManager

    <TestState(TestState.WorkInProgress)>
    Public Structure SceneSwitch
        Public Property ID As Integer
        Public Property Argument As Integer

        Public Sub New(IDs As Integer, Optional parameter As Integer = -1)
            ID = IDs
            Argument = parameter
        End Sub

        Public Shared Operator <>(ByVal a As SceneSwitch, ByVal b As SceneSwitch) As Boolean
            Return (a = b) = False
        End Operator

        Public Shared Operator =(ByVal a As SceneSwitch, ByVal b As SceneSwitch) As Boolean
            If a.ID <> b.ID Then Return False
            If a.Argument <> b.Argument Then Return False

            Return True
        End Operator

        Public Shared None As SceneSwitch = New SceneSwitch With {.Argument = 0, .ID = -1}

    End Structure

End Namespace