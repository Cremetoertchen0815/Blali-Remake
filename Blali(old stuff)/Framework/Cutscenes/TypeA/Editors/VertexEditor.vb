Imports Microsoft.Xna.Framework
Namespace Framework.Cutscenes.TypeA.Editors
    Public Class VertexEditor

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

        Private Sub Button1_Click(sender As Object, e As System.EventArgs) Handles Button1.Click
            If ColorDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Panel1.BackColor = ColorDialog1.Color
                Label3.Text = "{" & ColorDialog1.Color.R & ", " & ColorDialog1.Color.G & ", " & ColorDialog1.Color.B & "}"
            End If
        End Sub

        Private Sub VertexEditor_Load(sender As Object, e As System.EventArgs) Handles MyBase.Load
            If Label3.Text = "" Then
                SetColor(Color.White)
            End If
        End Sub

        Friend Sub SetColor(color As Color)
            ColorDialog1.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B)
            Panel1.BackColor = ColorDialog1.Color
            Label3.Text = "{" & ColorDialog1.Color.R & ", " & ColorDialog1.Color.G & ", " & ColorDialog1.Color.B & "}"
        End Sub

        Friend Function GetColor() As Color
            Return New Color(ColorDialog1.Color.R, ColorDialog1.Color.G, ColorDialog1.Color.B, ColorDialog1.Color.A)
        End Function
    End Class

End Namespace