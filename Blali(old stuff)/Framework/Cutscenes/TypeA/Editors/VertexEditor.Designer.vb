Namespace Framework.Cutscenes.TypeA.Editors
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    <Global.System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726")>
    Partial Class VertexEditor
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

        'Wird vom Windows Form-Designer benötigt.
        Private components As System.ComponentModel.IContainer

        'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
        'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
        'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VertexEditor))
            Me.NumericUpDown3 = New System.Windows.Forms.NumericUpDown()
            Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown()
            Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.Cancel = New System.Windows.Forms.Button()
            Me.OK = New System.Windows.Forms.Button()
            Me.PasswordLabel = New System.Windows.Forms.Label()
            Me.UsernameLabel = New System.Windows.Forms.Label()
            Me.LogoPictureBox = New System.Windows.Forms.PictureBox()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.Button1 = New System.Windows.Forms.Button()
            Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.Label3 = New System.Windows.Forms.Label()
            Me.NumericUpDown4 = New System.Windows.Forms.NumericUpDown()
            Me.NumericUpDown5 = New System.Windows.Forms.NumericUpDown()
            Me.Label4 = New System.Windows.Forms.Label()
            Me.Label5 = New System.Windows.Forms.Label()
            CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.NumericUpDown5, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'NumericUpDown3
            '
            Me.NumericUpDown3.Location = New System.Drawing.Point(176, 107)
            Me.NumericUpDown3.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
            Me.NumericUpDown3.Minimum = New Decimal(New Integer() {100000, 0, 0, -2147483648})
            Me.NumericUpDown3.Name = "NumericUpDown3"
            Me.NumericUpDown3.Size = New System.Drawing.Size(118, 20)
            Me.NumericUpDown3.TabIndex = 19
            '
            'NumericUpDown2
            '
            Me.NumericUpDown2.Location = New System.Drawing.Point(176, 64)
            Me.NumericUpDown2.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
            Me.NumericUpDown2.Minimum = New Decimal(New Integer() {100000, 0, 0, -2147483648})
            Me.NumericUpDown2.Name = "NumericUpDown2"
            Me.NumericUpDown2.Size = New System.Drawing.Size(118, 20)
            Me.NumericUpDown2.TabIndex = 18
            '
            'NumericUpDown1
            '
            Me.NumericUpDown1.Location = New System.Drawing.Point(177, 21)
            Me.NumericUpDown1.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
            Me.NumericUpDown1.Minimum = New Decimal(New Integer() {100000, 0, 0, -2147483648})
            Me.NumericUpDown1.Name = "NumericUpDown1"
            Me.NumericUpDown1.Size = New System.Drawing.Size(118, 20)
            Me.NumericUpDown1.TabIndex = 17
            '
            'Label1
            '
            Me.Label1.Location = New System.Drawing.Point(174, 44)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(119, 23)
            Me.Label1.TabIndex = 16
            Me.Label1.Text = "Y Position"
            Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Cancel
            '
            Me.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel.Location = New System.Drawing.Point(303, 161)
            Me.Cancel.Name = "Cancel"
            Me.Cancel.Size = New System.Drawing.Size(94, 23)
            Me.Cancel.TabIndex = 15
            Me.Cancel.Text = "&Abbrechen"
            '
            'OK
            '
            Me.OK.Location = New System.Drawing.Point(200, 161)
            Me.OK.Name = "OK"
            Me.OK.Size = New System.Drawing.Size(94, 23)
            Me.OK.TabIndex = 14
            Me.OK.Text = "&OK"
            '
            'PasswordLabel
            '
            Me.PasswordLabel.Location = New System.Drawing.Point(174, 87)
            Me.PasswordLabel.Name = "PasswordLabel"
            Me.PasswordLabel.Size = New System.Drawing.Size(119, 23)
            Me.PasswordLabel.TabIndex = 13
            Me.PasswordLabel.Text = "Z Position"
            Me.PasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'UsernameLabel
            '
            Me.UsernameLabel.Location = New System.Drawing.Point(175, 1)
            Me.UsernameLabel.Name = "UsernameLabel"
            Me.UsernameLabel.Size = New System.Drawing.Size(120, 23)
            Me.UsernameLabel.TabIndex = 11
            Me.UsernameLabel.Text = "X Position"
            Me.UsernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'LogoPictureBox
            '
            Me.LogoPictureBox.Image = CType(resources.GetObject("LogoPictureBox.Image"), System.Drawing.Image)
            Me.LogoPictureBox.Location = New System.Drawing.Point(3, 0)
            Me.LogoPictureBox.Name = "LogoPictureBox"
            Me.LogoPictureBox.Size = New System.Drawing.Size(165, 193)
            Me.LogoPictureBox.TabIndex = 12
            Me.LogoPictureBox.TabStop = False
            '
            'Label2
            '
            Me.Label2.Location = New System.Drawing.Point(301, 1)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(96, 23)
            Me.Label2.TabIndex = 20
            Me.Label2.Text = "Color"
            Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Button1
            '
            Me.Button1.Location = New System.Drawing.Point(304, 21)
            Me.Button1.Name = "Button1"
            Me.Button1.Size = New System.Drawing.Size(28, 23)
            Me.Button1.TabIndex = 21
            Me.Button1.Text = "..."
            Me.Button1.UseVisualStyleBackColor = True
            '
            'ColorDialog1
            '
            Me.ColorDialog1.Color = System.Drawing.Color.White
            '
            'Panel1
            '
            Me.Panel1.BackColor = System.Drawing.Color.White
            Me.Panel1.Location = New System.Drawing.Point(338, 21)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(28, 23)
            Me.Panel1.TabIndex = 22
            '
            'Label3
            '
            Me.Label3.Location = New System.Drawing.Point(372, 1)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(48, 43)
            Me.Label3.TabIndex = 23
            Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'NumericUpDown4
            '
            Me.NumericUpDown4.Location = New System.Drawing.Point(304, 64)
            Me.NumericUpDown4.Name = "NumericUpDown4"
            Me.NumericUpDown4.Size = New System.Drawing.Size(118, 20)
            Me.NumericUpDown4.TabIndex = 27
            '
            'NumericUpDown5
            '
            Me.NumericUpDown5.Location = New System.Drawing.Point(304, 107)
            Me.NumericUpDown5.Name = "NumericUpDown5"
            Me.NumericUpDown5.Size = New System.Drawing.Size(118, 20)
            Me.NumericUpDown5.TabIndex = 26
            '
            'Label4
            '
            Me.Label4.Location = New System.Drawing.Point(301, 44)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(131, 23)
            Me.Label4.TabIndex = 25
            Me.Label4.Text = "Texture Coordinate X(%)"
            Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'Label5
            '
            Me.Label5.Location = New System.Drawing.Point(301, 87)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(131, 23)
            Me.Label5.TabIndex = 24
            Me.Label5.Text = "Texture Coordinate Y(%)"
            Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'VertexEditor
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(431, 192)
            Me.Controls.Add(Me.NumericUpDown4)
            Me.Controls.Add(Me.NumericUpDown5)
            Me.Controls.Add(Me.Label4)
            Me.Controls.Add(Me.Label5)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Panel1)
            Me.Controls.Add(Me.Button1)
            Me.Controls.Add(Me.Label2)
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
            Me.Name = "VertexEditor"
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Vertex Editor"
            CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown4, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.NumericUpDown5, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents NumericUpDown3 As System.Windows.Forms.NumericUpDown
        Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
        Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Cancel As System.Windows.Forms.Button
        Friend WithEvents OK As System.Windows.Forms.Button
        Friend WithEvents PasswordLabel As System.Windows.Forms.Label
        Friend WithEvents UsernameLabel As System.Windows.Forms.Label
        Friend WithEvents LogoPictureBox As System.Windows.Forms.PictureBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Button1 As System.Windows.Forms.Button
        Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
        Friend WithEvents Panel1 As System.Windows.Forms.Panel
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents NumericUpDown4 As System.Windows.Forms.NumericUpDown
        Friend WithEvents NumericUpDown5 As System.Windows.Forms.NumericUpDown
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
    End Class
End Namespace
