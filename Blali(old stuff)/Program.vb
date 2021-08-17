

Public Module Program
    ' <summary>
    ' The main entry point for the application.
    ' </summary>
    <STAThread>
    Sub Main()
#If DEBUG Then
        EmmondInstance = New Emmond
        EmmondInstance.Run()
#Else
        Try
            EmmondInstance = New Emmond
            EmmondInstance.Run()
        Catch ex As Exception
            NoteError(ex, True)
            System.Windows.Forms.MessageBox.Show("There was an cwwitical ewwor while pwwaying the game! P... Pweease tell the deweloper!" & Environment.NewLine & Environment.NewLine & "Message: " & ex.Message & Environment.NewLine & "Stack Trace: " & ex.StackTrace, "OwO, what's this? *notices error*", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
            'System.Windows.Forms.MessageBox.Show("No, seriously! It would be a huge help if you sent a message to the Luminous Friends. They'll give you further assistance in fixing the issue ^w^.", "*notices another message box*", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
            'System.Windows.Forms.MessageBox.Show("If that sounds like too much work to you, it would already be really helpful if you sent us a photo of the error message box via our social media. ", "*notices another message box*", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
            'System.Windows.Forms.MessageBox.Show("What? Oh yeah, you already closed it... " & Environment.NewLine & Environment.NewLine & "You know what? I'll reopen it. Just for you ^w^", "*notices another message box*", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
            'System.Windows.Forms.MessageBox.Show("There was an cwwitical ewwor while pwwaying the game! P... Pweease tell the deweloper!" & Environment.NewLine & Environment.NewLine & "Message: " & ex.Message & Environment.NewLine & "Stack Trace: " & ex.StackTrace, "OwO, what's this? *notices error*", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
            'System.Windows.Forms.MessageBox.Show("Here we go! Have a good day!" & Environment.NewLine & Environment.NewLine & "Yours truly, " & Environment.NewLine & "Creme, your next door programmer", "I seriously don't know what to put here...", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly)
        End Try
#End If


    End Sub
End Module