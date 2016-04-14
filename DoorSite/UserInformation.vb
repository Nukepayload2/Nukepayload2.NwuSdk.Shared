Imports System.Net
#If NET_PORTABLE Then
Imports ImageSource = System.String
#End If
Namespace DoorSite
    Public Class UserInformation
        Public ReadOnly Property Photo As ImageSource
        Public ReadOnly Property RealName$
        Public ReadOnly Property UserID$
        Public ReadOnly Property Identity$
        Public ReadOnly Property Department$
        Public ReadOnly Property LastLoginTime As Date
        Public ReadOnly Property LastLoginIP As IPAddress
        Sub New(Photo As ImageSource,
         RealName$,
            UserID$,
            Identity$,
            Department$,
            LastLoginTime As Date,
         LastLoginIP As IPAddress)
            Me.Photo = Photo
            Me.RealName = RealName
            Me.UserID = UserID
            Me.Identity = Identity
            Me.Department = Department
            Me.LastLoginIP = LastLoginIP
            Me.LastLoginTime = LastLoginTime
        End Sub
    End Class
End Namespace
