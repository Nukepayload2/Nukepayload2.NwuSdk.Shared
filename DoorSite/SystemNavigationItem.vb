#If NET_PORTABLE Then
Imports FrameworkElement = System.Object
#End If
Namespace DoorSite
    Public Class SystemNavigationItem
        Inherits TextLinkPair
        Public ReadOnly Property Icon As FrameworkElement
        Sub New(Text$, Link$, Icon As FrameworkElement)
            MyBase.New(Text, Link)
            Me.Icon = Icon
        End Sub
    End Class
End Namespace
