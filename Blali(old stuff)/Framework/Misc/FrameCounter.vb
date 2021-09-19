Imports System.Linq
Namespace Framework.Misc

    <TestState(TestState.NearCompletion)>
    Public Class FrameCounter
        Dim worker As Func(Of Single, Single) = Function(i) i
        Public Property TotalFrames As Integer
        Public Property TotalSeconds As Single
        Public Property AverageFramesPerSecond As Double
        Public Property CurrentFramesPerSecond As Double
        Public Property MinimalFramesPerSecond As Double
        Public Property MaximalFramesPerSecond As Double
        Public Const MAXIMUM_SAMPLES As Integer = 150
        Dim counter As Integer = 0
        Dim MinMaxResetVector As Single = 0
        Private _sampleBuffer As Single()

        Sub New()
            ReDim _sampleBuffer(MAXIMUM_SAMPLES)
            For i As Integer = 0 To MAXIMUM_SAMPLES - 1
                _sampleBuffer(i) = 60
            Next
        End Sub

        Public Function Update(ByVal deltaTime As Double) As Boolean
            CurrentFramesPerSecond = 1 / (deltaTime / 1000)
            _sampleBuffer(counter) = (CurrentFramesPerSecond)

            counter += 1
            If counter >= MAXIMUM_SAMPLES Then counter = 0
            AverageFramesPerSecond = Math.Round(_sampleBuffer.Average(worker))

            MinimalFramesPerSecond = Math.Min(MinimalFramesPerSecond, CurrentFramesPerSecond)
            MaximalFramesPerSecond = Math.Max(MaximalFramesPerSecond, CurrentFramesPerSecond)
            MinMaxResetVector += deltaTime
            If MinMaxResetVector >= cMinMaxResetSequence Then MinimalFramesPerSecond = CurrentFramesPerSecond : MaximalFramesPerSecond = CurrentFramesPerSecond : MinMaxResetVector = 0

            Try
                TotalFrames += 1
                TotalSeconds += (deltaTime / 1000)
            Catch ex As OverflowException
                TotalFrames = 0
                TotalSeconds = 0
                DebugMode = True
            End Try
            Return True
        End Function
    End Class
End Namespace