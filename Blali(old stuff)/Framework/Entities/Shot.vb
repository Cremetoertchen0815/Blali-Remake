Imports System.Collections.Generic
Imports Emmond.Framework.Physics
Imports Microsoft.Xna.Framework

Namespace Framework.Entities

    <TestState(TestState.NearCompletion)>
    Public Class Shot
        Public Raycast As Raycast2D 'Contains the rays(subsegments) the bullet will travel along
        Public EnemyDmg As Dictionary(Of Entity, Single) 'Lists all enemies and when they receive the hit
        Public Damage As Single 'Determines how much damage is dealt
        Public Color As Color 'Indicates the color of the bullet
        Public OnScreenTime As Single 'Flag for helping to calculate the length / position along the ray
    End Class
End Namespace