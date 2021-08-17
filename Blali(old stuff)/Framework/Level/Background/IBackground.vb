
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.Background

    <TestState(TestState.NearCompletion)>
    Public MustInherit Class IBackground
        Implements IPoolable

        'Properties
        Property Type As UShort 'Defines the type of bg layer
        Property Layer As Single 'Z-Index
        Property Path As String
        Property Location As Vector2 'Position of the bg layer
        Property Size As Vector2 'Scale of the bg layer
        Property VectorScale As Single 'Scales the movement of the bg layer
        Property Color As Color 'Represents the color of ther bg layer
        Property Texture As Texture2D 'Represents the color of ther bg layer
        Property Description As String = "-"
        Friend IsFront As Boolean
        MustOverride ReadOnly Property Visible As Boolean

        Public Property PoolIsValid As Boolean Implements IPoolable.PoolIsValid
        Public Property PoolIsFree As Boolean Implements IPoolable.PoolIsFree
        Public Property ShowDebug As Boolean Implements IPoolable.ShowDebug

        'Methods
        MustOverride Sub Draw(GameTime As GameTime, Optional drawgreen As Boolean = False) 'Draws the bg layer
        MustOverride Sub Update(gameTime As GameTime, camloc As Vector2) 'Updates things if necessary
        MustOverride Function GetDrawRectangle() As Rectangle 'Returns the Display Rectangle

        Public MustOverride Sub Initialize() Implements IPoolable.Initialize
        Public MustOverride Sub Release() Implements IPoolable.Release
    End Class
End Namespace