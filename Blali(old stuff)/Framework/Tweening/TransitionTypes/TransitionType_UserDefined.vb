Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Namespace Framework.Tweening.TransitionTypes

    <TestState(TestState.Finalized)>
    Public Class TransitionType_UserDefined
        Implements ITransitionType

        Public Sub New()
        End Sub

        Public Sub New(ByVal elements As IList(Of TransitionElement), ByVal iTransitionTime As Integer)
            setup(elements, iTransitionTime)
        End Sub

        Public Sub setup(ByVal elements As IList(Of TransitionElement), ByVal iTransitionTime As Integer)
            m_Elements = elements
            m_dTransitionTime = iTransitionTime

            If elements.Count = 0 Then
                Throw New Exception("The list of elements passed to the constructor of TransitionType_UserDefined had zero elements. It must have at least one element.")
            End If
        End Sub

        Public Sub onTimer(ByVal iTime As Integer, <Out> ByRef dPercentage As Double, <Out> ByRef bCompleted As Boolean) Implements ITransitionType.onTimer
            Dim dTransitionTimeFraction As Double = iTime / m_dTransitionTime
            Dim dElementStartTime As Double
            Dim dElementEndTime As Double
            Dim dElementStartValue As Double
            Dim dElementEndValue As Double
            Dim eInterpolationMethod As InterpolationMethod
            getElementInfo(dTransitionTimeFraction, dElementStartTime, dElementEndTime, dElementStartValue, dElementEndValue, eInterpolationMethod)
            Dim dElementInterval As Double = dElementEndTime - dElementStartTime
            Dim dElementElapsedTime As Double = dTransitionTimeFraction - dElementStartTime
            Dim dElementTimeFraction As Double = dElementElapsedTime / dElementInterval
            Dim dElementDistance As Double

            Select Case eInterpolationMethod
                Case InterpolationMethod.Linear
                    dElementDistance = dElementTimeFraction
                Case InterpolationMethod.Accleration
                    dElementDistance = convertLinearToAcceleration(dElementTimeFraction)
                Case InterpolationMethod.Deceleration
                    dElementDistance = convertLinearToDeceleration(dElementTimeFraction)
                Case InterpolationMethod.EaseInEaseOut
                    dElementDistance = convertLinearToEaseInEaseOut(dElementTimeFraction)
                Case Else
                    Throw New Exception("Interpolation method not handled: " & eInterpolationMethod.ToString())
            End Select

            dPercentage = interpolate(dElementStartValue, dElementEndValue, dElementDistance)

            If iTime >= m_dTransitionTime Then
                bCompleted = True
                dPercentage = dElementEndValue
            Else
                bCompleted = False
            End If
        End Sub

        Private Sub getElementInfo(ByVal dTimeFraction As Double, <Out> ByRef dStartTime As Double, <Out> ByRef dEndTime As Double, <Out> ByRef dStartValue As Double, <Out> ByRef dEndValue As Double, <Out> ByRef eInterpolationMethod As InterpolationMethod)
            Dim iCount As Integer = m_Elements.Count

            While m_iCurrentElement < iCount
                Dim element As TransitionElement = m_Elements(m_iCurrentElement)
                Dim dElementEndTime As Double = element.EndTime / 100.0

                If dTimeFraction < dElementEndTime Then
                    Exit While
                End If

                m_iCurrentElement += 1
            End While

            If m_iCurrentElement = iCount Then
                m_iCurrentElement = iCount - 1
            End If

            dStartTime = 0.0
            dStartValue = 0.0

            If m_iCurrentElement > 0 Then
                Dim previousElement As TransitionElement = m_Elements(m_iCurrentElement - 1)
                dStartTime = previousElement.EndTime / 100.0
                dStartValue = previousElement.EndValue / 100.0
            End If

            Dim currentElement As TransitionElement = m_Elements(m_iCurrentElement)
            dEndTime = currentElement.EndTime / 100.0
            dEndValue = currentElement.EndValue / 100.0
            eInterpolationMethod = currentElement.InterpolationMethod
        End Sub

        Private m_Elements As IList(Of TransitionElement) = Nothing
        Private m_dTransitionTime As Double = 0.0
        Private m_iCurrentElement As Integer = 0
    End Class
End Namespace
