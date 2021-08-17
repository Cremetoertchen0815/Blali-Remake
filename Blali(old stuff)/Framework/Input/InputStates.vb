Imports Microsoft.Xna.Framework

Namespace Framework.Input
    <TestState(TestState.NearCompletion)>
    Public Structure MenuInputState
        Public Property Movement As Vector2
        Public Property Start As Boolean 'BT1
        Public Property Back As Boolean 'BT2
        Public Property Confirm As Boolean 'BT3
        Public Property XtraX As Boolean 'BT4
        Public Property XtraY As Boolean 'BT5

        Sub New(startB As Boolean)
            Start = startB
        End Sub
    End Structure

    <TestState(TestState.NearCompletion)>
    Public Structure CameraInputState
        Public Property Movement As Vector2
        Public Property Zoom As Single
        Public Property ShiftFunction As Boolean
    End Structure

    <TestState(TestState.NearCompletion)>
    Public Structure GameInputState
        Public Property Movement As Vector2 'LS
        Public Property Camera As Vector2 'D-Pad
        Public Property Aim As Double 'RS
        Public Property Start As Boolean 'Start
        Public Property Backpack As Boolean 'Back
        Public Property Jump As Boolean 'A
        Public Property UseItem As Boolean 'B
        Public Property Interact As Boolean 'X
        Public Property DropItem As Boolean 'Y
        Public Property Shoot As Boolean 'RT
        Public Property AimMode As Boolean 'LT
        Public Property SwitchR As Boolean 'RB
        Public Property SwitchL As Boolean 'LB

        Sub New(startB As Boolean)
            Start = startB
        End Sub
    End Structure
End Namespace