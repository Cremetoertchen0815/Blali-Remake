
Imports Microsoft.Xna.Framework

Namespace Framework.Entities

    <TestState(TestState.WorkInProgress)>
    Public Class EntityAttributes
        Sub New(UniqueID As Integer)
            Me.UniqueID = UniqueID
        End Sub
        Public Property ID As SByte = -1
        Public Property UniqueID As SByte = -1

        Public Property Lifetime As TimeSpan
        Public Property Lives As Single = 1
        Public Property Score As Single
        Public Property DeathCount As Byte

        Public Property Health As Single
        Public Property MaxHealth As Single
        Public Property Energy As Single
        Public Property TouchDamage As Single
        Public Property ReleaseScore As Single

        Public Property ShootDamage As Single
        Public Property BulletColor As Color
        Public Property HitTrigger As Hit

        Public Delegate Function Hit(impactspeedX As Integer, dmg As Single) As Boolean
    End Class
End Namespace