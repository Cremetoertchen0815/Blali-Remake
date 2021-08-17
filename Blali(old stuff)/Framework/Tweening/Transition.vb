

Imports Microsoft.Xna.Framework

Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Public Class Transition(Of T)
        Implements ITransition

        Sub New(TransitionMethod As ITransitionType, StartValue As T, EndValue As T, FinishAction As FinishedDelegate)
            'Check whether the tweening manager supports this type
            Dim type As Type = GetType(T)
            If Automator.m_mapManagedTypes.ContainsKey(type) Then
                ManagedType = Automator.m_mapManagedTypes(type)

                Me.StartValue = StartValue
                Me.EndValue = EndValue
                Me.Method = TransitionMethod
                Me.FinishAction = FinishAction
                Me.TransitionStater = TransitionState.Idle
                Me.Value = StartValue
            Else
                Throw New NotImplementedException("The tweening manager doesn't support " & GetType(T).Name & ".")
            End If
        End Sub

        Sub New()
            'Check whether the tweening manager supports this type
            Dim type As Type = GetType(T)
            If Automator.m_mapManagedTypes.ContainsKey(type) Then
                ManagedType = Automator.m_mapManagedTypes(type)

                Me.TransitionStater = TransitionState.Idle
            Else
                Throw New NotImplementedException("The tweening manager doesn't support " & GetType(T).Name & ".")
            End If
        End Sub

        Sub Update(gameTime As GameTime) Implements ITransition.Update
            If Enabled And TransitionStater = TransitionState.InProgress Then
                Timer += gameTime.ElapsedGameTime.TotalMilliseconds

                'Calculate values
                Dim percentage As Double
                Dim completed As Boolean
                Method.onTimer(Timer, percentage, completed)

                'Set value
                Value = ManagedType.copy(ManagedType.getIntermediateValue(StartValue, EndValue, percentage))

                If completed Then
                    Select Case Repeat
                        Case RepeatJob.None
                            TransitionStater = TransitionState.Done
                        Case RepeatJob.Reverse
                            'Swap Start and Stop values
                            Dim tEnd As T = ManagedType.copy(EndValue)
                            EndValue = ManagedType.copy(StartValue)
                            StartValue = tEnd
                            'Reset Values
                            Value = ManagedType.copy(StartValue)
                            Timer = 0
                        Case RepeatJob.JumpBack
                            'Reset Values
                            Value = ManagedType.copy(StartValue)
                            Timer = 0
                    End Select
                    TriggerAction()
                End If
            End If
        End Sub

        Private Sub TriggerAction()
            If FinishAction IsNot Nothing Then FinishAction.Invoke(Me)
            RaiseEvent TransitionCompletedEvent(Me, New EventArgs)
        End Sub


        Public Property StartValue As T
        Public Property EndValue As T
        Public Property Value As T
        Public Property Method As ITransitionType Implements ITransition.Method
        Public Property FinishAction As FinishedDelegate 'A delegate to be executed when the transition is complete/the transition loops
        Public Property Enabled As Boolean = True
        Public Property Repeat As RepeatJob = RepeatJob.None
        Public Property TransitionStater As TransitionState Implements ITransition.TransitionStater
        Public ReadOnly Property ElapsedTime As Integer
            Get
                Return Timer
            End Get
        End Property

        Private Timer As Integer 'Keeps track of the elapsed time
        Private ManagedType As IManagedType 'Interface for converting/calculating values for the specified type

        Public Event TransitionCompletedEvent(sender As Object, e As EventArgs) 'An event to be executed when the transition is complete/the transition loops
        Public Delegate Sub FinishedDelegate(sender As Transition(Of T)) 'A delegate to be executed when the transition is complete/the transition loops

    End Class

End Namespace
