﻿Imports System.Runtime.InteropServices

Namespace Framework.Tweening.TransitionTypes

    <TestState(TestState.Finalized)>
    Public Class TransitionType_CriticalDamping
        Implements ITransitionType

        Public Sub New(ByVal iTransitionTime As Integer)
            If iTransitionTime <= 0 Then
                Throw New Exception("Transition time must be greater than zero.")
            End If

            m_dTransitionTime = iTransitionTime
        End Sub

        Public Sub onTimer(ByVal iTime As Integer, <Out> ByRef dPercentage As Double, <Out> ByRef bCompleted As Boolean) Implements ITransitionType.onTimer
            Dim dElapsed As Double = iTime / m_dTransitionTime
            dPercentage = (1.0 - Math.Exp(-1.0 * dElapsed * 5)) / 0.993262053

            If dElapsed >= 1.0 Then
                dPercentage = 1.0
                bCompleted = True
            Else
                bCompleted = False
            End If
        End Sub

        Private m_dTransitionTime As Double = 0.0
    End Class
End Namespace
