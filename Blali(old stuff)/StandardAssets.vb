Imports System.Collections.Generic
Imports Emmond.Framework.Level
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.Graphics


''' <summary>
''' Contains standard assets that are shared withing the entire runtime.
''' </summary>
Public Class StandardAssets
    'Graphics
    Public Shared Property Graphx As GraphicsDeviceManager 'The graphics device used
    Public Shared Property Backbuffer As RenderTarget2D 'The rendering backbuffer
    Public Shared Property MainScalingMatrix As Matrix 'Matrix to scale the rendered graphics from the Basic Resolution to the Temporary Buffer Resolution
    Public Shared Property GameSize As Vector2 'Basic Resolution
    Public Shared Property BufferSize As Vector2 'Global RenderTarget Resolution

    'Global instances
    Public Shared Property SpriteBat As SpriteBatch 'The SpriteBatch that is used for rendering
    Public Shared Property SpriteCount As Integer 'FRM
    Public Shared Property EmmondInstance As Emmond 'Main game class instance
    Public Shared Property LevelInstance As Level
    Public Shared Property ResolutionList As DisplayMode()
    Public Shared Property DebugMode As Boolean = False
    Public Shared Property SceneMan As Framework.SceneManager.SceneManager 'Scene Manager
    Public Shared Property ContentMan As ContentManager 'Global Content Manager
    Public Shared Property Automator As Framework.Tweening.TweenManager 'Effect/Transition Manager instance
    Public Shared Property Lighting As Penumbra.PenumbraComponent
    Public Shared Property SettingsMan As Framework.Data.SettingsManager
    Public Shared Property InputMan As Framework.Input.InputManager
    Public Shared Property Rand As New Random

    'Global assets/debug
    Public Shared Property Texts As Dictionary(Of UInteger, String) 'Contains all texts(localisation)
    Public Shared Property AudioEng As AudioEngine 'Sound engine
    Public Shared Property SFXWaveBank As WaveBank 'Contains the bgm(0) and sfx(1) wave banks
    Public Shared Property DebugFont As SpriteFont 'Standard font for all kinds of debug display
    Public Shared Property DebugSmolFont As SpriteFont
    Public Shared Property ReferencePixel As Texture2D 'Contains a single white pixel(1x1), who's used as texture for drawing lines and polygons
    Public Shared Property XboxButtons As Texture2D
    Public Shared Property RoundedCorner As Texture2D
    Public Shared Property DebugTexture As Texture2D
    Public Shared Property DebugTextureB As Texture2D
    Public Shared Property ErrorHandlerMessages As String() = {"No error yet, mate.", "Here could go the message that the exception deliveres", "Here could go the stack trace"}

End Class
