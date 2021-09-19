Namespace Framework.Input

    <TestState(TestState.Finalized)>
    Public Enum DpadMode
        Camera
        Movement
    End Enum

    <TestState(TestState.Finalized)>
    Public Enum GamePadType
        Undefined
        Xbox
        Generic
    End Enum

    <TestState(TestState.Finalized)>
    Public Enum HintMode
        None
        Xbox
        Keyboard
    End Enum

    <TestState(TestState.Finalized)>
    Public Enum HintInput
        A
        B
        X
        Y
        Start
        LS
        RS
        DUp
        DDown
        DLeft
        DRight
        LB
        RB
        LT
        RT
    End Enum
End Namespace