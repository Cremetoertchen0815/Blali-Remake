Namespace Framework.Level
    Public Enum ExecType
        OnceTotal = 0
        OncePerTouch = 1
        Continuous = 2
    End Enum
    Public Enum ObjectActivationType
        PlayerTouch
        Interact
    End Enum
    Public Enum TriggerType
        UserDefined = 0 'A trigger that executes a method that id user-defined
        FinishLine = 1 'A trigger that triggers the end of a level
        Checkpoint = 2 'A trigger that triggers the logging of a checkpoint
        PlayerBarrier = 3 'A trigger that doesn't allow the player to pass
        CameraBarrier = 4 'A trigger that doesn't allow the player to pass
        CameraLock = 5 'A trigger that locks the camera to a certain position
        CameraRelease = 6 'A trigger that releases a locked camera
        DeathPlain = 7 'A trigger that kills the player
        ScrollLock = 8 'Prevents the user from manually moving the camera
        ScrollRelease = 9 'Allow the player to manually move the camera
    End Enum
    Public Enum TriggerOrientation
        Horizontal = 1
        Vertical = 2
    End Enum
    Public Enum PauseMode
        Play
        Capture
        Fade
        Refade
    End Enum
    Public Enum TileCollisionType
        None = 0
        HalfSolidFloor = 1
        Solid = 2
        TopSolid = 3
    End Enum
    Public Enum FinishedMode
        Nottin
        TouchedFinishLine
        OutOfView
    End Enum
End Namespace