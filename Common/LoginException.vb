Public Class LoginException
    Inherits Exception
    Sub New(Reason$)
        MyBase.New("登录失败。" & Reason)
    End Sub
End Class
