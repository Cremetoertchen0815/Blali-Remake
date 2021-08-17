
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Public Class ShaderTransition
        Implements ITransition

        Sub New(TransitionMethod As ITransitionType, StartValue As Object, EndValue As Object, shader As Effect, parameter As Integer, FinishAction As FinishedDelegate)
            'Check whether the tweening manager supports this type

            Dim typeA As Type = StartValue.GetType()
            Dim typeB As Type = StartValue.GetType()

            If typeA = typeB AndAlso Automator.m_mapManagedTypes.ContainsKey(typeA) Then
                ManagedType = Automator.m_mapManagedTypes(typeA)

                Me.StartValue = StartValue
                Me.EndValue = EndValue
                Me.Method = TransitionMethod
                Me.Shader = shader
                Me.Parameter = parameter
                Me.FinishAction = FinishAction

                'If the type is correct, set parameter
                Try
                    shader.Parameters(parameter).SetValue(StartValue)
                    Me.TransitionStater = TransitionState.Idle
                Catch ex As InvalidCastException
                    Throw New NotImplementedException("The named parameter doesn't correspond to the type of the start/end value!")
                End Try
                Me.TransitionStater = TransitionState.Idle
            ElseIf typeA <> typeB Then
                Throw New NotImplementedException("Start and End value are different types!")
            Else
                Throw New NotImplementedException("The tweening manager doesn't support " & typeA.Name & ".")
            End If

        End Sub

        Sub New()
            ManagedType = Automator.m_mapManagedTypes(GetType(Single))
            Me.TransitionStater = TransitionState.Idle
        End Sub

        Sub Update(gameTime As GameTime) Implements ITransition.Update
            If Enabled And TransitionStater = TransitionState.InProgress Then
                Timer += gameTime.ElapsedGameTime.TotalMilliseconds

                'Calculate values
                Dim percentage As Double
                Dim completed As Boolean
                Method.onTimer(Timer, percentage, completed)

                'Set value
                Shader.Parameters(Parameter).SetValue(ManagedType.getIntermediateValue(StartValue, EndValue, percentage))

                If completed Then
                    Select Case Repeat
                        Case RepeatJob.None
                            TransitionStater = TransitionState.Done
                        Case RepeatJob.Reverse
                            'Swap Start and Stop values
                            Dim tEnd As Single = ManagedType.copy(EndValue)
                            EndValue = ManagedType.copy(StartValue)
                            StartValue = tEnd
                            'Reset Values
                            Shader.Parameters(Parameter).SetValue(StartValue)
                            Timer = 0
                        Case RepeatJob.JumpBack
                            'Reset Values
                            Shader.Parameters(Parameter).SetValue(StartValue)
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

        Public Property StartValue As Object
        Public Property EndValue As Object
        Public Property Shader As Effect
        Public Property Parameter As Integer
        Public Property Method As ITransitionType Implements ITransition.Method
        Public Property FinishAction As FinishedDelegate
        Public Property Enabled As Boolean = True
        Public Property Repeat As RepeatJob = RepeatJob.None
        Public Property TransitionStater As TransitionState Implements ITransition.TransitionStater
        Public ReadOnly Property ElapsedTime As Integer
            Get
                Return Timer
            End Get
        End Property

        Private Timer As Integer
        Private ManagedType As IManagedType

        Public Event TransitionCompletedEvent(sender As Object, e As EventArgs)
        Public Delegate Sub FinishedDelegate(sender As ShaderTransition)

    End Class

End Namespace