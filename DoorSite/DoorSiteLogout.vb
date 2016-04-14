Imports System.Net.Http
Namespace DoorSite
    Public Class DoorSiteLogout
        Public Async Function LogoutAsync(Cookies As DoorSiteCookies) As Task
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                AddCommonHeadersForText(client)
                handler.UseCookies = True
                Dim CookiesUri As New Uri("http://my.nwu.edu.cn")
                client.DefaultRequestHeaders.Referrer = New Uri("http://my.nwu.edu.cn/index.portal")
                handler.CookieContainer.SetCookies(CookiesUri, $"JSESSIONID={Cookies.SessionId},iPlanetDirectoryPro={Cookies.PlanetDirectoryProfile}")
                client.DefaultRequestHeaders.Referrer = CookiesUri
                Dim GotData = Await client.GetAsync("http://my.nwu.edu.cn/logout.portal")
                Dim script = Await GotData.Content.ReadAsStringAsync
                Dim matched = script.Match("(?<=location=').+(?=')")
                If String.IsNullOrEmpty(matched) Then
                    Throw New Exception("登出失败")
                Else
                    Await client.GetAsync("http://my.nwu.edu.cn/" & matched)
                End If
            End Using
        End Function
    End Class
End Namespace
