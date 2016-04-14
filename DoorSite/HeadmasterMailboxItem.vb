Namespace DoorSite
    Public Class HeadmasterMailboxItem
        Inherits TextLinkPair
        Public ReadOnly Property Unit$
        Public ReadOnly Property ConsultTime As Date
        Public ReadOnly Property Status$
        Public ReadOnly Property Reply$
        Sub New(Header$, Link As Uri, Unit$, ConsultTime As Date, Status$, Reply$)
            MyBase.New(Header, Link)
            Me.Unit = Unit
            Me.ConsultTime = ConsultTime
            Me.Status = Status
            Me.Reply = Reply
        End Sub
    End Class
End Namespace
