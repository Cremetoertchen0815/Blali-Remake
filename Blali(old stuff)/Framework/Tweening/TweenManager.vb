Imports System.Collections.Generic
Imports Emmond.Framework.Tweening.ManagedTypes
Imports Microsoft.Xna.Framework

Namespace Framework.Tweening

    <TestState(TestState.Finalized)>
    Public Class TweenManager

        Sub New()
            'Register Managed Types
            registerType(New ManagedType_Int())
            registerType(New ManagedType_Single())
            registerType(New ManagedType_Double())
            registerType(New ManagedType_Color())
            registerType(New ManagedType_String())
            registerType(New ManagedType_Vector2())
            registerType(New ManagedType_Vector3())
            registerType(New ManagedType_Vector4())
            registerType(New ManagedType_CamKeyframe())

            m_Transitions = New List(Of ITransition)
        End Sub

        Private Sub registerType(ByVal transitionType As IManagedType)
            Dim type As Type = transitionType.getManagedType()
            m_mapManagedTypes.Add(type, transitionType)
        End Sub

        Public Sub Add(ByVal transition As ITransition)
            SyncLock m_Lock
                transition.TransitionStater = TransitionState.InProgress
                m_TransCommands.Add(New Tuple(Of TransCommand, ITransition)(TransCommand.Add, transition))
            End SyncLock
        End Sub
        Public Sub Remove(ByVal transition As ITransition)
            SyncLock m_Lock
                m_TransCommands.Add(New Tuple(Of TransCommand, ITransition)(TransCommand.Remove, transition))
            End SyncLock
        End Sub
        Public Sub Clear()
            SyncLock m_Lock
                m_TransCommands.Add(New Tuple(Of TransCommand, ITransition)(TransCommand.Clear, Nothing))
            End SyncLock
        End Sub

        Public Sub Update(gameTime As GameTime)
            SyncLock m_Lock
                For Each transition In m_Transitions
                    transition.Update(gameTime)
                    If transition.TransitionStater = TransitionState.Done Then m_TransCommands.Add(New Tuple(Of TransCommand, ITransition)(TransCommand.Remove, transition))
                Next

                For Each com In m_TransCommands
                    Select Case com.Item1
                        Case TransCommand.Add
                            If Not m_Transitions.Contains(com.Item2) Then m_Transitions.Add(com.Item2)
                        Case TransCommand.Remove
                            If m_Transitions.Contains(com.Item2) Then m_Transitions.Remove(com.Item2)
                        Case TransCommand.Clear
                            m_Transitions.Clear()
                    End Select
                Next
                m_TransCommands.Clear()
            End SyncLock
        End Sub

        Friend Function GetCount() As Integer
            Return m_Transitions.Count
        End Function

        Friend m_mapManagedTypes As IDictionary(Of Type, IManagedType) = New Dictionary(Of Type, IManagedType)()
        Private m_Transitions As List(Of ITransition)
        Private m_TransCommands As New List(Of Tuple(Of TransCommand, ITransition))
        Private m_Lock As Object = New Object()

        Private Enum TransCommand
            Add
            Remove
            Clear
        End Enum
    End Class
End Namespace
