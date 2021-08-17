Imports System.Collections.Generic
Imports Emmond.Framework.Graphics.PostProcessing
Imports Microsoft.Xna.Framework
Imports Penumbra

Namespace Framework.Graphics

    <TestState(TestState.NearCompletion)>
    Public Class FXData
        'Bloom
        Public Property EnableBloom As Boolean
        Public Property BloomPreset As BloomFilter.BloomPresets
        Public Property BloomThreshold As Single
        Public Property BloomStrengthMultiplier As Single

        'Lighting
        Public Property EnableLighting As Boolean
        Public Property AmbientColor As Color
        Public Property Hulls As New List(Of Hull)
        Public Property Lights As New List(Of Light)

        'Particles

        'Misc
        Public Property EnableOtherFX As Boolean
    End Class
End Namespace