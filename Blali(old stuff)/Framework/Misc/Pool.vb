Imports System.Collections
Imports System.Collections.Generic

Namespace Framework.Misc
    <TestState(TestState.Finalized)>
    Public Interface IPoolable

        Sub Initialize()
        Sub Release()
        Property PoolIsValid As Boolean
        Property PoolIsFree As Boolean
        Property ShowDebug As Boolean
    End Interface

    Public Class ObjectPool(Of T As {IPoolable, New})
        ' I use a Stack data structure for storing the objects as it should be 
        ' more efficient than List and we don't have to worry about indexing 
        Private stack As Stack(Of T)
        ' The total capacity of the pool - this I only really use this for debugging
        Private capacity As Integer


        Public Sub New(ByVal capacity As Integer)
            Me.capacity = capacity
            stack = New Stack(Of T)(Me.capacity)

            For i As Integer = 0 To capacity - 1
                AddNewObject()
            Next
        End Sub

        Private Sub AddNewObject()
            Dim obj As T = New T()
            obj.PoolIsValid = True
            stack.Push(obj)
            capacity += 1
            If obj.ShowDebug Then Console.WriteLine("[Pool(" & GetType(T).Name & ")]: Increased pool capacity!")
        End Sub


        Public Sub Release(ByVal obj As T)
            If obj.PoolIsFree Then
                Throw New Exception("Object already released " & obj.ToString)
            ElseIf Not obj.PoolIsValid Then
                Throw New Exception("Object not valid " & obj.ToString)
            End If

            obj.Release()
            obj.PoolIsFree = True
            stack.Push(obj)
            If obj.ShowDebug Then Console.WriteLine("[Pool(" & GetType(T).Name & ")]: Released item!")
        End Sub

        Public Function [Get]() As T
            If stack.Count = 0 Then
                AddNewObject()
            End If

            Dim obj As T = stack.Pop()
            obj.Initialize()
            obj.PoolIsFree = False
            Return obj
        End Function
    End Class
End Namespace
