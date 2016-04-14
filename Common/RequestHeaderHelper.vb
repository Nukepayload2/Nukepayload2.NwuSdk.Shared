Imports System.Net.Http
Imports System.Text

Module RequestHeaderHelper
    Friend Sub AddCommonHeadersForText(client As HttpClient, Optional Host$ = "my.nwu.edu.cn")
        With client.DefaultRequestHeaders
            .Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
            .AcceptEncoding.ParseAdd("gzip, deflate")
            .AcceptLanguage.ParseAdd("zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3")
            .Connection.ParseAdd("Keep-Alive")
            .Host = Host
            .UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586")
        End With
    End Sub
    Function UrlEncodeGB2312$(chs$)
        Dim sb As New StringBuilder
        Dim gb2312 = CodePagesEncodingProvider.Instance.GetEncoding("gb2312")
        For i = 0 To chs.Length - 1
            Dim ch = chs(i)
            Dim safe = False
            If ch >= "a"c AndAlso ch <= "z"c OrElse ch >= "A"c AndAlso ch <= "Z"c OrElse ch >= "0"c AndAlso ch <= "9"c Then
                safe = True
            Else
                Select Case ch
                    Case "-"c, "_"c, "."c, "!"c, "*"c, "("c, ")"c
                        safe = True
                End Select
            End If
            If safe Then
                sb.Append(ch)
            Else
                For Each b In gb2312.GetBytes(ch)
                    sb.Append("%" & b.ToString("X"))
                Next
            End If
        Next
        Return sb.ToString
    End Function
End Module

