Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports Nukepayload2.NwuSdk
Namespace EduAdminSys
    Public Class LoginHelper
        Private Sub GenerateRandomCode()
            Dim rnd As New Random
            Dim sb As New StringBuilder
            For i = 1 To 24
                Dim ch As Char
                Dim r = CInt(Math.Floor(rnd.NextDouble * 36))
                If r < 10 Then
                    ch = ChrW(48 + r \ 2) 'num: 48-57 
                Else
                    ch = ChrW(87 + r) 'lcase: 97-122 
                End If
                sb.Append(ch)
            Next
            LoginCheck = sb.ToString
            Debug.WriteLine("generated login code: " & LoginCheck)
        End Sub
        Private Sub AddCommonHeaders(client As HttpClient)
            With client.DefaultRequestHeaders
                .Accept.ParseAdd("text/html, application/xhtml+xml, image/jxr, */*")
                .AcceptEncoding.ParseAdd("gzip, deflate")
                .AcceptLanguage.ParseAdd("zh-Hans-CN, zh-Hans; q=0.8, en-US; q=0.6, en; q=0.4, ja; q=0.2")
                .Connection.ParseAdd("Keep-Alive")
                .Host = "jwxt.nwu.edu.cn"
                .UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586")
            End With
        End Sub
        Public Async Function RefreshLoginCheck() As Task
            Dim handler As New HttpClientHandler
            Dim client As New HttpClient(handler)
            Dim GotData = Await client.GetAsync("http://jwxt.nwu.edu.cn/")
            AddCommonHeaders(client)
            Debug.WriteLine("获取会话ID : (应为FOUND) " & GotData.StatusCode)
            Dim req = GotData.RequestMessage.RequestUri.ToString
            LoginCheck = req.Substring(req.IndexOf("(") + 1, 24)
            Debug.WriteLine("会话ID : " & LoginCheck)
        End Function
        Public LoginCheck$ = ""  ' tested "f10asx55vj0ual45udmwiynx"
        Public ReadOnly Property LoginUri$
            Get
                Return "http://jwxt.nwu.edu.cn/(" & LoginCheck & ")/Default2.aspx"
            End Get
        End Property


        Public Async Function LoginAsync(LoginData As LoginInfo) As Task(Of PageContent)
            Dim handler As New HttpClientHandler
            Dim client As New HttpClient(handler)
            Dim postdata = $"__VIEWSTATE=dDwyODE2NTM0OTg7Oz6ZmvWn7xzjizifHN9MgLoDNTRtjQ%3D%3D&txtUserName={LoginData.UserName}&TextBox2={LoginData.Password}&txtSecretCode={LoginData.CheckCode}&RadioButtonList1={UrlEncodeGB2312(LoginData.LoginType)}&Button1=&lbLanguage=&hidPdrs=&hidsc="
            Debug.WriteLine($"Post 数据: {postdata}")
            Dim args As New StringContent(postdata)
            args.Headers.ContentType = Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
            AddCommonHeaders(client)
            client.DefaultRequestHeaders.Referrer = New Uri(LoginUri)
            Dim response = Await client.PostAsync(LoginUri, args)
            Debug.WriteLine($"登录 Post 状态(应为FOUND) : {response.StatusCode}")
            Dim gotdata = Await client.GetAsync($"http://jwxt.nwu.edu.cn/(" & LoginCheck & $")/xs_main.aspx?xh={LoginData.UserName}")
            Debug.WriteLine($"登录 Get 状态(应为OK):{gotdata.StatusCode}")
            Dim dat = Await gotdata.Content.ReadAsByteArrayAsync
            Dim str = CodePagesEncodingProvider.Instance.GetEncoding("gb2312").GetString(dat)
            If str.Contains("欢迎您") Then
                Dim regex As New Regex("(?<=>)\w+(?=同学)")
                ActualName = regex.Match(str).Value
                Debug.WriteLine("姓名: " & ActualName)
            End If
            Return New PageContent(str, New UserInfo(ActualName, LoginData))
        End Function
        Public Async Function LogoutAsync(LogoutData As LoginInfo) As Task(Of String)
            Dim handler As New HttpClientHandler
            Dim client As New HttpClient(handler)
            Dim postdata = "__EVENTTARGET=likTc&__EVENTARGUMENT=&__VIEWSTATE=dDwxMDQ4ODYxMzk7Oz5w3s%2F1Bw4XlwCginhsYY%2Bx2hdCJQ%3D%3D"
            Dim args As New StringContent(postdata)
            args.Headers.ContentType = Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
            AddCommonHeaders(client)
            client.DefaultRequestHeaders.Referrer = New Uri("http://jwxt.nwu.edu.cn/(" & LoginCheck & ")xs_main.aspx?xh=" & LogoutData.UserName)
            Dim response = Await client.PostAsync(LoginUri, args)
            Debug.WriteLine($"退出 Post 状态(应为FOUND) : {response.StatusCode}")
            Dim gotdata = Await client.GetAsync($"http://jwxt.nwu.edu.cn/({LoginCheck})/logout.aspx")
            Debug.WriteLine($"退出 Get 状态(应为OK):{gotdata.StatusCode}")
            Dim dat = Await gotdata.Content.ReadAsByteArrayAsync
            ActualName = ""
            Return CodePagesEncodingProvider.Instance.GetEncoding("gb2312").GetString(dat)
        End Function
        Dim ActualName$
        Private Async Function PostCommonInfoAsync(UserInfo As LoginInfo, postdata$, gnmkdm$, InfoName$) As Task(Of String)
            If String.IsNullOrEmpty(ActualName) Then
                Return "请登录"
            End If
            Dim handler As New HttpClientHandler
            Dim client As New HttpClient(handler)
            AddCommonHeaders(client)
            Dim args As New StringContent(postdata)
            args.Headers.ContentType = Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded")
            client.DefaultRequestHeaders.Referrer = New Uri("http://jwxt.nwu.edu.cn/(" & LoginCheck & ")xs_main.aspx?xh=" & UserInfo.UserName)
            Dim gotdata = Await client.PostAsync($"http://jwxt.nwu.edu.cn/({LoginCheck})/xskbcx.aspx?xh={UserInfo.UserName}&xm={UrlEncodeGB2312(ActualName)}&gnmkdm={gnmkdm}", args)
            Debug.WriteLine($"{InfoName} Get 状态(应为OK):{gotdata.StatusCode}")
            Dim dat = Await gotdata.Content.ReadAsByteArrayAsync
            Return CodePagesEncodingProvider.Instance.GetEncoding("gb2312").GetString(dat)
        End Function
        Private Async Function GetCommonInfoAsync(UserInfo As LoginInfo, gnmkdm$, InfoName$) As Task(Of String)
            If String.IsNullOrEmpty(ActualName) Then
                Return "请登录"
            End If
            Dim handler As New HttpClientHandler
            Dim client As New HttpClient(handler)
            AddCommonHeaders(client)
            client.DefaultRequestHeaders.Referrer = New Uri("http://jwxt.nwu.edu.cn/(" & LoginCheck & ")xs_main.aspx?xh=" & UserInfo.UserName)
            Dim gotdata = Await client.GetAsync($"http://jwxt.nwu.edu.cn/({LoginCheck})/xskbcx.aspx?xh={UserInfo.UserName}&xm={UrlEncodeGB2312(ActualName)}&gnmkdm={gnmkdm}")
            Debug.WriteLine($"{InfoName} Get 状态(应为OK):{gotdata.StatusCode}")
            Dim dat = Await gotdata.Content.ReadAsByteArrayAsync
            Return CodePagesEncodingProvider.Instance.GetEncoding("gb2312").GetString(dat)
        End Function
        Public Async Function GetScheduleAsync(UserInfo As LoginInfo) As Task(Of String)
            Return Await GetCommonInfoAsync(UserInfo, "121603", "课表")
        End Function
        Public Async Function GetScoreAsync(UserInfo As LoginInfo) As Task(Of String)
            Return Await GetCommonInfoAsync(UserInfo, "121605", "成绩")
        End Function
        Public Async Function GetLevelTestAsync(UserInfo As LoginInfo) As Task(Of String)
            Return Await GetCommonInfoAsync(UserInfo, "121606", "等级考试")
        End Function
        Public Async Function PostAllScoresAsync(UserInfo As LoginInfo) As Task(Of String)
            Return Await PostCommonInfoAsync(UserInfo, "", "120605", "历年成绩")
        End Function
    End Class

End Namespace
