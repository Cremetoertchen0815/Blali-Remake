Namespace Framework.Cutscenes.TypeA.Editors
    Public Class TriEditor

        ' TODO: Code zum Durchf�hren der benutzerdefinierten Authentifizierung mithilfe des angegebenen Benutzernamens und des Kennworts hinzuf�gen 
        ' (Siehe https://go.microsoft.com/fwlink/?LinkId=35339).  
        ' Der benutzerdefinierte Prinzipal kann anschlie�end wie folgt an den Prinzipal des aktuellen Threads angef�gt werden: 
        '     My.User.CurrentPrincipal = CustomPrincipal
        ' wobei CustomPrincipal die IPrincipal-Implementierung ist, die f�r die Durchf�hrung der Authentifizierung verwendet wird. 
        ' Anschlie�end gibt My.User Identit�tsinformationen zur�ck, die in das CustomPrincipal-Objekt gekapselt sind,
        ' z. B. den Benutzernamen, den Anzeigenamen usw.

        Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End Sub

        Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

    End Class
End Namespace