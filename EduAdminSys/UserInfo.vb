Namespace EduAdminSys
    Public Class UserInfo
        Public ReadOnly Property RealName$
        Public ReadOnly Property LoginInfo As LoginInfo

        Sub New(RealName$, LoginInfo As LoginInfo)
            Me.RealName = RealName
            Me.LoginInfo = LoginInfo
        End Sub
    End Class
End Namespace
