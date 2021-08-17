Namespace IG
    Public Enum PlayerStatus
        Idle = 0 'Static
        Jumping = 1 'Animation(2 Frames, Oneshot)
        ExJumping = 2 'Animation(6 Frames, Loop)
        Falling = 3 'Animation(3 Frames, Loop)
        Walking = 4 '
        Run = 5
        Skid = 6
        WallClinging = 7
        JumpingFromWall = 8
        Hit = 9
        Landed = 10
    End Enum
End Namespace