Imports Nukepayload2.NwuSdk
Namespace EduAdminSys
    Public Class PageContent
        Public Property PageCode As String
        Public Property LoginType As String
        Public ReadOnly Property UserInfo As UserInfo
        Sub New(PageCode As String, UserInfo As UserInfo)
            Me.PageCode = PageCode
            Me.UserInfo = UserInfo
        End Sub
    End Class
End Namespace
