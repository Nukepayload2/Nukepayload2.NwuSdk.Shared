Namespace DoorSite
    ''' <summary>
    ''' 表示用户登录my.nwu.edu.com的凭据
    ''' </summary>
    Public Class UserCredential
#If NET_PORTABLE Then
        Public Property UserID$
        Public Property RememberPassword As Boolean
#Else
        Public Property UserID$
            Get
                Return LoadValue(Of String)(NameOf(UserID))
            End Get
            Set(value$)
                SaveValue(NameOf(UserID), value)
            End Set
        End Property
        Public Property RememberPassword As Boolean
            Get
                Return LoadValue(Of Boolean)("Door" & NameOf(RememberPassword))
            End Get
            Set(value As Boolean)
                SaveValue("Door" & NameOf(RememberPassword), value)
            End Set
        End Property
        Public Sub SaveOrClearPassword()
            If RememberPassword Then
                SaveCredential(ResID, UserID, Password)
            Else
                SaveCredential(ResID, UserID, "")
            End If
        End Sub
        Public Sub FillPasswordByUserID()
            If String.IsNullOrEmpty(Password) Then
                If Not String.IsNullOrEmpty(UserID) Then
                    Password = LoadCredential(ResID, UserID)
                End If
            End If
        End Sub
#End If

        Public Property Password$

        Const ResID$ = "my.nwu.edu.com"

    End Class
End Namespace
