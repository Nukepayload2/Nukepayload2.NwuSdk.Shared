Imports System.Net
Imports System.Net.Http

Namespace DoorSite
    ''' <summary>
    ''' 登录门户网站
    ''' </summary>
    Public Class DoorSiteLogin
        Sub New()

        End Sub

        Public Async Function LoginAsync(Credential As UserCredential) As Task(Of DoorSiteCookies)
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                AddCommonHeadersForText(client)
                handler.UseCookies = True
                Dim CookiesUri As New Uri("http://my.nwu.edu.cn")
                client.DefaultRequestHeaders.Referrer = New Uri("http://my.nwu.edu.cn/index.portal")
                Dim GotData = Await client.GetAsync("http://my.nwu.edu.cn/index.portal")
                Dim SessionId = handler.CookieContainer.GetCookies(CookiesUri).Item("JSESSIONID").Value
                If String.IsNullOrEmpty(SessionId) Then
                    Throw New LoginException($"未能获取{NameOf(SessionId)}")
                End If
                Dim args As New StringContent($"Login.Token1={Credential.UserID}&Login.Token2={Credential.Password}&goto=http%3A%2F%2Fmy.nwu.edu.cn%2FloginSuccess.portal&gotoOnFail=http%3A%2F%2Fmy.nwu.edu.cn%2FloginFailure.portal")
                args.Headers.ContentType = Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
                client.DefaultRequestHeaders.Referrer = CookiesUri
                Dim ReplyData = Await client.PostAsync("http://my.nwu.edu.cn/userPasswordValidate.portal", args)
                Dim iPlanetDirectoryPro = handler.CookieContainer.GetCookies(CookiesUri).Item("iPlanetDirectoryPro").Value
                If String.IsNullOrEmpty(iPlanetDirectoryPro) Then
                    Throw New LoginException($"未能获取{NameOf(iPlanetDirectoryPro)}")
                End If
                Return New DoorSiteCookies With {.PlanetDirectoryProfile = iPlanetDirectoryPro, .SessionId = SessionId}
            End Using
        End Function
    End Class
End Namespace
