Imports System.Net
Imports System.Net.Http
#If NET_PORTABLE Then
Imports Nukepayload2.NwuSdk.Portable.DoorSite
#Else
Imports Nukepayload2.NwuSdk.DoorSite
#End If
Namespace EduAdminSys
    Public Class ScheduleConsulter
        Inherits PageAnalyzer
        Public Overrides ReadOnly Property TargetPageUrl As String
            Get
                Return PageURLs.Schedule
            End Get
        End Property
        Public ReadOnly Property CachedAccessId As Uri
        Public Async Function ConsultAsync(Cookies As DoorSiteCookies) As Task(Of String)
            Const Refer = "http://my.nwu.edu.cn/index.portal"
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                handler.AllowAutoRedirect = True
                AddCommonHeadersForText(client)
                handler.UseCookies = True
                Dim CookiesUri As New Uri(Refer)
                handler.CookieContainer.SetCookies(CookiesUri, $"iPlanetDirectoryPro={Cookies.PlanetDirectoryProfile}")
                client.DefaultRequestHeaders.Referrer = CookiesUri
                Dim RequestJwxt = "http://jwxt.nwu.edu.cn/xskbcx_jzjk.aspx"
                Dim JwxtData = Await client.GetAsync(RequestJwxt)
                Dim gotstrm = New BinaryReader(Await JwxtData.Content.ReadAsStreamAsync)
                Dim JwxtHtml = Text.CodePagesEncodingProvider.Instance.GetEncoding("gb2312").GetString(gotstrm.ReadBytes(gotstrm.BaseStream.Length))
                '去掉选框
                JwxtHtml = JwxtHtml.Replace("<TR>.+?<TD align=""center"">.+?</TD>.+?</TR>", "", Text.RegularExpressions.RegexOptions.Singleline)
                '添加字样式
                JwxtHtml = New CssInjectHelper().InjectThemeFontColorAndTableLines(JwxtHtml)
                If JwxtHtml.Contains("__VIEWSTATE") Then
                    _CachedAccessId = JwxtData.RequestMessage.RequestUri
                    Return JwxtHtml
                Else
                    Throw New WebException("网站正在维护，因此不能读取课程表。")
                End If
            End Using
        End Function
    End Class
End Namespace