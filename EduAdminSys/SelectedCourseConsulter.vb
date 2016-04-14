Imports System.Net.Http

Namespace EduAdminSys
    Public Class SelectedCourseConsulter
        Public Async Function ConsultAsync(PreviousEduAdminRequestUri As Uri, UserID%, RealName$) As Task(Of Tuple(Of Uri, String))
            Const MatchLoginId = "(?<=\().+(?=\))"
            Dim ReqUri = "http://" & $"jwxt.nwu.edu.cn/({PreviousEduAdminRequestUri.ToString.Match(MatchLoginId)})/xf_xsqxxxk.aspx?xh={UserID}&xm={UrlEncodeGB2312(RealName)}&gnmkdm=N121109"
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                handler.AllowAutoRedirect = True
                AddCommonHeadersForText(client, "jwxt.nwu.edu.cn")
                handler.UseCookies = True
                client.DefaultRequestHeaders.Referrer = New Uri(ReqUri)
                Dim gb2312 = Text.CodePagesEncodingProvider.Instance.GetEncoding("gb2312")
                Dim JwxtData = Await client.GetAsync(ReqUri)
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
                Return New Tuple(Of Uri, String)(New Uri(ReqUri), JwxtHtml)
            End Using
        End Function
        Public Async Function SearchCourseAsync(ConsultRequestUri As Uri) As Task(Of Tuple(Of Uri, String))
            '            Button2
            '            È·¶¨
            'TextBox1
            '            ÍøÂç
            '            __EVENTARGUMENT
            '            __EVENTTARGET
            '            ddl_kcgs
            '            ddl_kcxz
            '            ddl_sksj
            '            ddl_xqbs
            '1
            'ddl_ywyl
            '            ÓÐ
            'dpkcmcGrid: txtChoosePage
            '1
            'dpkcmcGrid: txtPageSize
            '15
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                handler.AllowAutoRedirect = True
                AddCommonHeadersForText(client, "jwxt.nwu.edu.cn")
                handler.UseCookies = True
                client.DefaultRequestHeaders.Referrer = ConsultRequestUri
                Dim gb2312 = Text.CodePagesEncodingProvider.Instance.GetEncoding("gb2312")
                Dim JwxtData = Await client.GetAsync(ConsultRequestUri)
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
                Return New Tuple(Of Uri, String)(ConsultRequestUri, JwxtHtml)
            End Using
        End Function
        Public Async Function SubmitCourseAsync(ConsultRequestUri As Uri) As Task(Of Tuple(Of Uri, String))
            '测试：提交 搜索框 网络 选择 化石趣谈
            '            Button1
            '            Ìá½»  
            'TextBox1
            '            ÍøÂç
            '            __EVENTARGUMENT
            '            __EVENTTARGET
            '            ddl_kcgs
            '            ddl_kcxz
            '            ddl_sksj
            '            ddl_xqbs
            '1
            'ddl_ywyl
            '            ÓÐ
            'dpkcmcGrid: txtChoosePage
            '1
            'dpkcmcGrid: txtPageSize
            '15
            'kcmcGrid:   _ctl2 : xk
            '            On
            Dim handler As New HttpClientHandler
            Using client As New HttpClient(handler)
                handler.AllowAutoRedirect = True
                AddCommonHeadersForText(client, "jwxt.nwu.edu.cn")
                handler.UseCookies = True
                client.DefaultRequestHeaders.Referrer = ConsultRequestUri
                Dim gb2312 = Text.CodePagesEncodingProvider.Instance.GetEncoding("gb2312")
                Dim JwxtData = Await client.GetAsync(ConsultRequestUri)
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
                Return New Tuple(Of Uri, String)(ConsultRequestUri, JwxtHtml)
            End Using
        End Function
    End Class
End Namespace