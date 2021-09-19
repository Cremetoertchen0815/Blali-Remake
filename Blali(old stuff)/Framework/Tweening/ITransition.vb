Imports Microsoft.Xna.Framework

Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Public Interface ITransition
        Sub Update(gameTime As GameTime)

        Property Method As ITransitionType
        Property TransitionStater As TransitionState
    End Interface
    Public Enum TransitionState
        Idle
        InProgress
        Done
    End Enum

    Public Enum RepeatJob
        None
        JumpBack
        Reverse
    End Enum
End Namespace