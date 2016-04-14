Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Public Module JwcPageAnalyzer
    Public Const MainPageURL As String = "http://jwc.nwu.edu.cn/index.php?s=/Public/index"
    Public Const NotificationPageURL As String = "http://jwc.nwu.edu.cn/index.php?s=/Public/posts/post_type_id/93"
    Public Const DownloadSheetsPageURL As String = "http://jwc.nwu.edu.cn/index.php?s=/Public/posts/post_type_id/72"
    Public Const BaseURL As String = "http://jwc.nwu.edu.cn"

    Friend Function GetNotificationLink(PageID As Integer) As String
        If PageID < 1 Then
            Throw New ArgumentOutOfRangeException(NameOf(PageID))
        End If
        Return "http://jwc.nwu.edu.cn/index.php?s=%2FPublic%2Fposts%2Fpost_type_id%2F93post_type_id%3D93&status=1&pageNum=" & PageID
    End Function

    Friend Async Function GetGenericPageItems(GetFirst As Boolean, URL As String) As Task(Of IEnumerable(Of TextLinkPair))
        Dim xm = Await PageAnalyzer.OpenXmlReaderAsync(URL)
        Dim l As New List(Of TextLinkPair)
        With xm
            .Read()
            .ReadToFollowing("ul")
            Try
                Dim co = If(GetFirst, 0, 19)
                For i As Integer = 0 To co
                    .ReadToFollowing("a")
                    Dim href = .GetAttribute(0)
                    If Not href.StartsWith("http") Then
                        href = BaseURL + href
                    End If
                    Dim txt = .ReadElementContentAsString
                    l.Add(New TextLinkPair(txt, href))
                Next
            Catch ex As XmlException
            End Try
            .Dispose()
        End With
        Return l
    End Function
    Public Async Function GetMoreNotifications(PageURL As String, Optional GetFirst As Boolean = False) As Task(Of IEnumerable(Of TextLinkPair))
        Return Await GetGenericPageItems(GetFirst, PageURL)
    End Function
    Public Async Function GetMoreNotifications(PageID As Integer, Optional GetFirst As Boolean = False) As Task(Of IEnumerable(Of TextLinkPair))
        Return Await GetGenericPageItems(GetFirst, GetNotificationLink(PageID))
    End Function
    Public Async Function GetNotifications(Optional GetFirst As Boolean = False) As Task(Of IEnumerable(Of TextLinkPair))
        Return Await GetGenericPageItems(GetFirst, NotificationPageURL)
    End Function
    Public Async Function GetDownloadLink(PageHRef As Uri) As Task(Of String)
        Return Await GetDownloadLink(PageHRef.OriginalString)
    End Function
    Public Async Function GetDownloadLink(PageHRef As String) As Task(Of String)
        Dim rd = Await PageAnalyzer.OpenXmlReaderAsync(PageHRef)
        With rd
            .Read()
            .ReadToFollowing("a")
            .ReadToFollowing("a")
            Dim href = .GetAttribute(0)
            If Not href.StartsWith("http") Then
                href = BaseURL + href
            End If
            If Not href.Substring(href.Length - 5).Contains(".") Then
                Throw New WebException("分析的页面不是指向下载的，
或者下载链接有问题.")
            End If
            .Dispose()
            Return href
        End With
    End Function
    Public Async Function GetDownloads(Optional GetFirst As Boolean = False) As Task(Of IEnumerable(Of TextLinkPair))
        Return Await GetGenericPageItems(GetFirst, DownloadSheetsPageURL)
    End Function

    Public Function GetAnnouncementsSrcCode(ResponseStream As Stream) As String
        Dim ms As New MemoryStream
        Dim writer As New StreamWriter(ms)
        Dim BadXml As String
        Using reader As New StreamReader(ResponseStream)
            Dim xm As New StringBuilder
            Dim tmp As New StringBuilder
            Dim iamhereVeritfyBegin = False
            Do Until reader.EndOfStream
                Dim ch = ChrW(reader.Read)
                xm.Append(ch)
                If ch = "<"c Then
                    iamhereVeritfyBegin = True
                ElseIf ch = ">"c Then
                    iamhereVeritfyBegin = False
                    tmp.Clear()
                End If
                If iamhereVeritfyBegin Then
                    tmp.Append(ch)
                    If tmp.ToString = "<div class=""iamhere""" Then
                        Exit Do
                    End If
                End If
            Loop
            BadXml = xm.ToString
        End Using
        Dim GoodXml = BadXml.Replace("&nbsp;", " ").Replace("<\w+?:.+?>", "<p>", RegexOptions.None).Replace("</\w+?:.+?>", "</p>", RegexOptions.None).Replace("<!--.*?-->", "", RegexOptions.None)
        Dim GXm As New StringBuilder
        Dim RegP As New Regex("(?<=</.+?>)[^<^>]+?(?=<.+?>)")
        Dim LastStop As Integer = 0
        For Each m As Match In RegP.Matches(GoodXml)
            GXm.Append(GoodXml.Substring(LastStop, m.Index - LastStop)).Append("<p>").Append(m.Value).Append("</p>")
            LastStop = m.Index + m.Length
        Next
        GXm.Append(GoodXml.Substring(LastStop))
        GoodXml = GXm.ToString
        writer.Write(GoodXml)
        '垃圾字符，防止检查标记是否关闭
        For i = 1 To 512
            writer.Write("garb")
        Next
        Dim rd = PageAnalyzer.OpenXmlReader(ms)
        With rd
            ms.Position = 0
            .Read()
            For i = 1 To 6
                .ReadToFollowing("div")
            Next
            Dim cls = .GetAttribute(0)
            If cls <> "mcontent" Then
                Throw New WebException("在限定的循环中无法找到公告相关元素")
            End If
            Dim Level = 0
            Dim Begin = True
            Dim St = .ReadSubtree
            Dim sb As New StringBuilder
            Try
                Do While St.Read
                    If St.NodeType = XmlNodeType.Text Then
                        sb.AppendLine(St.ReadContentAsString)
                    End If
                    If St.Name = "a" Then
                        Dim hr = St.GetAttribute("href")
                        If Not String.IsNullOrEmpty(hr) Then
                            If Not hr.StartsWith("http://") Then
                                hr = BaseURL + hr
                            End If
                            sb.AppendLine(hr)
                        End If
                    End If
                Loop
            Catch ex As XmlException
                sb.AppendLine($"*解析出现问题,详情请查看原网页*
--开发人员信息--
{ex.Message}
--部分html数据--
{GoodXml}")
            End Try
            .Dispose()
            Return sb.ToString
        End With
    End Function
End Module