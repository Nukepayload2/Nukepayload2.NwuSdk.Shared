Public Class TextLinkPair
    Public Property Text As String
    Public Property Link As Uri
    Sub New(Text As String, Link As Uri)
        Me.Text = Text
        Me.Link = Link
    End Sub
    Sub New(Text As String, Link As String)
        MyClass.New(Text, New Uri(Link, UriKind.Absolute))
    End Sub
End Class
