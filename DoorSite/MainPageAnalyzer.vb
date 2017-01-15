Imports System.Net.Http
Imports Windows.Graphics.Imaging

Namespace DoorSite
    Public Class MainPageAnalyzer
        Inherits PageAnalyzer
        Const Refer = "http://my.nwu.edu.cn/"
        Public Overrides ReadOnly Property TargetPageUrl As String = PageURLs.MainPage
        Dim HtmlText As String, Cookies As DoorSiteCookies
        Private Sub New(HtmlText$, Cookies As DoorSiteCookies)
            Me.HtmlText = HtmlText
            Me.Cookies = Cookies
        End Sub
        Public Shared Async Function CreateAsync(Cookies As DoorSiteCookies) As Task(Of MainPageAnalyzer)
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                AddCommonHeadersForText(client)
                handler.UseCookies = True
                Dim CookiesUri As New Uri(Refer)
                handler.CookieContainer.SetCookies(CookiesUri, $"JSESSIONID={Cookies.SessionId},iPlanetDirectoryPro={Cookies.PlanetDirectoryProfile}")
                client.DefaultRequestHeaders.Referrer = CookiesUri
                Dim GotData = Await client.GetAsync("http://my.nwu.edu.cn//index.portal")
                Dim HtmlText = Await GotData.Content.ReadAsStringAsync
                If Not HtmlText.Contains("欢迎您") Then
                    Throw New LoginException("学生基本信息页面获取出错，请稍后重试。
如果成绩查询网页正常运行，但是本程序不能工作，请联系客服。")
                End If
                Return New MainPageAnalyzer(HtmlText, Cookies)
            End Using
        End Function
        Public Async Function AnalizeAsync() As Task(Of MainPageAnalizeResult)
            Const MatchRealName = "(?<=欢迎您：).+(?=<)"
            Const MatchID = "(?<=工号：).+(?=\r\n)"
            Const MatchIdentity = "(?<=身份：.+?<span>).+?(?=</span>)"
            Const MatchTimestamp = "((?:2|1)\d{3}(?:-|\/)(?:(?:0[1-9])|(?:1[0-2]))(?:-|\/)(?:(?:0[1-9])|(?:[1-2][0-9])|(?:3[0-1]))(?:T|\s)(?:(?:[0-1][0-9])|(?:2[0-3])):(?:[0-5][0-9]):(?:[0-5][0-9]))"
            Const MatchIp = "(\d{1,3}\.){3}\d{1,3}"
            Const MatchPhoto = "(?<=src="")attachment.+?(?="")"
            Const MatchDepartment = "(?<=部门：).+(?=[\r\n])"
            Dim bmpPath = "http://my.nwu.edu.cn/" & HtmlText.Match(MatchPhoto)
#If NET_PORTABLE Then
            Dim bmp = bmpPath
#Else
            ' 新改动：头像不再直接显示。需要使用一个 Http 请求来获得下载图片的地址。
            Dim bmp As ImageSource = Nothing
#End If
            Dim CardInf As CardInformation
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                AddCommonHeadersForText(client)
                handler.UseCookies = True
                Dim CookiesUri As New Uri(Refer)
                handler.CookieContainer.SetCookies(CookiesUri, $"JSESSIONID={Cookies.SessionId},iPlanetDirectoryPro={Cookies.PlanetDirectoryProfile}")
                client.DefaultRequestHeaders.Referrer = CookiesUri
                bmp = New BitmapImage(New Uri(bmpPath))
#If WINDOWS_UWP Then
                Dim avatar = Await client.GetAsync(bmpPath)
                Using txt = Await avatar.Content.ReadAsStreamAsync
                    Dim ima = Await BitmapDecoder.CreateAsync(txt.AsRandomAccessStream)
                    Using sbmp = Await ima.GetSoftwareBitmapAsync
                        Dim writeable As New WriteableBitmap(sbmp.PixelWidth, sbmp.PixelHeight)
                        sbmp.CopyToBuffer(writeable.PixelBuffer)
                        bmp = writeable
                    End Using
                End Using
#End If
                Dim resp = Await client.PostAsync("http://my.nwu.edu.cn/pnull.portal?.f=f501&.pmn=view&action=informationCenterAjax&r=0.6712131211141332&.ia=false&.pen=pe281", New StreamContent(New MemoryStream, 1))
                Dim json = Await resp.Content.ReadAsStringAsync
                Dim addr = json.Match("(?<=Ajax.Request\(').+(?=')")
                If Not String.IsNullOrEmpty(addr) Then
                    resp = Await client.PostAsync("http://my.nwu.edu.cn/" & addr, New StreamContent(New MemoryStream, 1))
                    json = Await resp.Content.ReadAsStringAsync
                End If
                Dim cardnum = json.Match("(?<=您的一卡通账户<span>)\d+?(?=<)")
                Dim cardcash = json.Match("(?<=span>)\d+\.\d{2}?(?=<)")
                CardInf = New CardInformation(cardnum, cardcash)
            End Using
            Dim UserInf As New UserInformation(bmp,
                                               HtmlText.Match(MatchRealName), HtmlText.Match(MatchID),
                                               HtmlText.Match(MatchIdentity, Text.RegularExpressions.RegexOptions.Singleline),
                                               HtmlText.Match(MatchDepartment),
                                               Date.Parse(HtmlText.Match(MatchTimestamp)),
                                               Net.IPAddress.Parse(HtmlText.Match(MatchIp)))
            Dim fake As New MainPageAnalizeResult(UserInf, CardInf, 0, Cookies, {}, {}, {}, {})
            Return fake
        End Function
    End Class
End Namespace