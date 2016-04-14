Imports System.Net.Http

Namespace EduAdminSys
    Public Class ScoreConsulter
        Public Async Function ConsultAsync(PreviousRequestUri As Uri, Cookies As DoorSite.DoorSiteCookies, UserId$, Year%, Month%) As Task(Of String)
            Const MatchViewState = "(?<=name=""__VIEWSTATE"" value="").+(?="")"
            Const MatchLoginId = "(?<=\().+(?=\))"
            Dim ReqUri = "http://" & $"jwxt.nwu.edu.cn/({PreviousRequestUri.ToString.Match(MatchLoginId)})/xscjcx_jzjk.aspx?xh=" & UserId
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                handler.AllowAutoRedirect = True
                AddCommonHeadersForText(client, "jwxt.nwu.edu.cn")
                handler.UseCookies = True
                Dim CookiesUri As New Uri("http://jwxt.nwu.edu.cn")
                handler.CookieContainer.SetCookies(CookiesUri, $"iPlanetDirectoryPro={Cookies.PlanetDirectoryProfile}")
                client.DefaultRequestHeaders.Referrer = New Uri(ReqUri)
                Dim gb2312 = Text.CodePagesEncodingProvider.Instance.GetEncoding("gb2312")
                Dim PreviousPageContent$ = Await New StreamReader(Await client.GetStreamAsync(ReqUri), gb2312).ReadToEndAsync
                Dim ViewState = PreviousPageContent.Match(MatchViewState)
                ViewState = UrlEncodeGB2312(ViewState)
                Dim PostSrc = $"__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE={ViewState}&hidLanguage=&ddlXN={Year - 1}-{Year}&ddlXQ={1 + (Month - 1) \ 6}&ddl_kcxz=&btn_xq=%D1%A7%C6%DA%B3%C9%BC%A8"
                Dim Cont As New StringContent(PostSrc, gb2312)
                Cont.Headers.ContentType = Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
                Dim JwxtData = Await client.PostAsync(ReqUri, Cont)
                Dim gotstrm = New BinaryReader(Await JwxtData.Content.ReadAsStreamAsync)
                Dim JwxtHtml = gb2312.GetString(gotstrm.ReadBytes(gotstrm.BaseStream.Length))
                '去掉选框
                JwxtHtml = JwxtHtml.Replace("<input name=""hidLanguage.+?<!-- 内容显示区开始 -->", "", Text.RegularExpressions.RegexOptions.Singleline)
                '去掉图片
                JwxtHtml = JwxtHtml.Replace("<img .+?/>", "", Text.RegularExpressions.RegexOptions.Singleline)
                '去掉链接
                JwxtHtml = JwxtHtml.Replace("href="".+?""", "", Text.RegularExpressions.RegexOptions.Singleline)
                '添加白色字样式
                JwxtHtml = New CssInjectHelper().InjectThemeFontColorAndTableLines(JwxtHtml)
                Return JwxtHtml
            End Using
        End Function
    End Class
End Namespace
