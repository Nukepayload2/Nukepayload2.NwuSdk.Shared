Public Module WebNotificationExtension
#If Not NET_PORTABLE Then
    <Extension>
    Public Async Function DownloadFile(noti As TextLinkPair) As Task(Of String)
        Dim err As Exception = Nothing
        Try
            Dim href = Await GetDownloadLink(noti.Link)
            Dim r As Boolean = Await Windows.System.Launcher.LaunchUriAsync(New Uri(href, UriKind.Absolute))
            If Not r Then
                Return "未能启动浏览器"
            End If
        Catch ex As Exception
            err = ex
        End Try
        If err IsNot Nothing Then
            Return "无法下载"
        End If
        Return String.Empty
    End Function
#End If

End Module
