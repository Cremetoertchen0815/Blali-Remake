Namespace Framework.Cutscenes.TypeA.Editors
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    <Global.System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726")>
    Partial Class TriEditor
        Inherits System.Windows.Forms.Form

        'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub
        Friend WithEvents LogoPictureBox As System.Windows.Forms.PictureBox
        Friend WithEvents UsernameLabel As System.Windows.Forms.Label
        Friend WithEvents PasswordLabel As System.Windows.Forms.Label
        Friend WithEvents OK As System.Windows.Forms.Button
        Friend WithEvents Cancel As System.Windows.Forms.Button

        'Wird vom Windows Form-Designer benötigt.
        Private components As System.ComponentModel.IContainer

        'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
        'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
        'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TriEditor))
            Me.LogoPictureBox = New System.Windows.Forms.PictureBox()
            Me.UsernameLabel = New System.Windows.Forms.Label()
            Me.PasswordLabel = New System.Windows.Forms.Label()
            Me.OK = New System.Windows.Forms.Button()
            Me.Cancel = New System.Windows.Forms.Button()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
            Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown()
            Me.NumericUpDown3 = New System.Windows.Forms.NumericUpDown()
            CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'LogoPictureBox
            '
            Me.LogoPictureBox.Image = CType(resources.GetObject("LogoPictureBox.Image"), System.Drawing.Image)
            Me.LogoPictureBox.Location = New System.Drawing.Point(0, 0)
            Me.LogoPictureBox.Name = "LogoPictureBox"
            Me.LogoPictureBox.Size = New System.Drawing.Size(165, 193)
            Me.LogoPictureBox.TabIndex = 0
            Me.LogoPictureBox.TabStop = False
            '
            'UsernameLabel
            '
            Me.UsernameLabel.Location = New System.Drawing.Point(172, 1)
            Me.UsernameLabel.Name = "UsernameLabel"
            Me.UsernameLabel.Size = New System.Drawing.Size(220, 23)
            Me.UsernameLabel.TabIndex = 0
            Me.UsernameLabel.Text = "Vertex A"
            Me.UsernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'PasswordLabel
            '
            Me.PasswordLabel.Location = New System.Drawing.Point(171, 87)
            Me.PasswordLabel.Name = "PasswordLabel"
            Me.PasswordLabel.Size = New System.Drawing.Size(220, 23)
            Me.PasswordLabel.TabIndex = 2
            Me.PasswordLabel.Text = "Vertex C"
            Me.PasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'OK
            '
            Me.OK.Location = New System.Drawing.Point(197, 161)
            Me.OK.Name = "OK"
            Me.OK.Size = New System.Drawing.Size(94, 23)
            Me.OK.TabIndex = 4
            Me.OK.Text = "&OK"
            '
            'Cancel
            '
            Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel.Location = New System.Drawing.Point(300, 161)
            Me.Cancel.Name = "Cancel"
            Me.Cancel.Size = New System.Drawing.Size(94, 23)
            Me.Cancel.TabIndex = 5
            Me.Cancel.Text = "&Abbrechen"
            '
            'Label1
            '
            Me.Label1.Location = New System.Drawing.Point(171, 44)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(220, 23)
            Me.Label1.TabIndex = 6
            Me.Label1.Text = "Vertex B"
            Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'NumericUpDown1
            '
            Me.NumericUpDown1.Location = New System.Drawing.Point(174, 21)
            Me.NumericUpDown1.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
            Me.NumericUpDown1.Name = "NumericUpDown1"
            Me.NumericUpDown1.Size = New System.Drawing.Size(219, 20)
            Me.NumericUpDown1.TabIndex = 8
            '
            'NumericUpDown2
            '
            Me.NumericUpDown2.Location = New System.Drawing.Point(173, 64)
            Me.NumericUpDown2.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
            Me.NumericUpDown2.Name = "NumericUpDown2"
            Me.NumericUpDown2.Size = New System.Drawing.Size(219, 20)
            Me.NumericUpDown2.TabIndex = 9
            '
            'NumericUpDown3
            '
            Me.NumericUpDown3.Location = New System.Drawing.Point(173, 107)
            Me.NumericUpDown3.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
            Me.NumericUpDown3.Name = "NumericUpDown3"
            Me.NumericUpDown3.Size = New System.Drawing.Size(219, 20)
            Me.NumericUpDown3.TabIndex = 10
            '
            'TriEditor
            '
            Me.AcceptButton = Me.OK
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel
            Me.ClientSize = New System.Drawing.Size(401, 192)
            Me.Controls.Add(Me.NumericUpDown3)
            Me.Controls.Add(Me.NumericUpDown2)
            Me.Controls.Add(Me.NumericUpDown1)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.Cancel)
            Me.Controls.Add(Me.OK)
            Me.Controls.Add(Me.PasswordLabel)
            Me.Controls.Add(Me.UsernameLabel)
            Me.Controls.Add(Me.LogoPictureBox)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "TriEditor"
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Tri Editor"
            CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
        Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
        Friend WithEvents NumericUpDown3 As System.Windows.Forms.NumericUpDown
    End Class

End Namespace