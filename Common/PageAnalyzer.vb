Imports System.Net
Imports System.Xml

Public MustInherit Class PageAnalyzer
    Public MustOverride ReadOnly Property TargetPageUrl$
    Public Shared Async Function RequestDataAsync(URL As String) As Task(Of Stream)
        Return Await Response(WebRequest.CreateHttp(URL))
    End Function
    Public Shared Async Function RequestDataAsync(Link As Uri) As Task(Of Stream)
        Return Await Response(WebRequest.CreateHttp(Link))
    End Function

    Private Shared Async Function Response(Req As HttpWebRequest) As Task(Of Stream)
        Dim Resp As HttpWebResponse = CType(Await Req.GetResponseAsync, HttpWebResponse)
        Dim strm = Resp.GetResponseStream
        Return strm
    End Function

    Public Shared Function OpenXmlReader(Strm As Stream) As XmlReader
        Dim settings As New XmlReaderSettings With {
            .ConformanceLevel = ConformanceLevel.Document,
            .IgnoreWhitespace = True,
            .IgnoreComments = True,
            .CloseInput = True,
            .Async = True,
            .IgnoreProcessingInstructions = False,
            .CheckCharacters = False,
            .DtdProcessing = DtdProcessing.Ignore
        }
        Return XmlReader.Create(Strm, settings)
    End Function
    Public Shared Async Function OpenXmlReaderAsync(Link As Uri) As Task(Of XmlReader)
        Return OpenXmlReader((Await RequestDataAsync(Link)))
    End Function
    Public Shared Async Function OpenXmlReaderAsync(URL As String) As Task(Of XmlReader)
        Return OpenXmlReader((Await RequestDataAsync(URL)))
    End Function
End Class
