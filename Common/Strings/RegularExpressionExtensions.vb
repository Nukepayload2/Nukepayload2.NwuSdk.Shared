Imports System.Text
Imports System.Text.RegularExpressions

Public Module RegularExpressionExtensions
    <Extension>
    Public Function Replace(Text As String, Regexp As String, ReplaceTo As String, Options As RegexOptions) As String
        Return New Regex(Regexp, Options).Replace(Text, ReplaceTo)
    End Function
    <Extension>
    Public Function Match$(Text$, Regexp$, Options As RegexOptions)
        Return New Regex(Regexp, Options).Matches(Text).OfType(Of Match).FirstOrDefault?.Value
    End Function
    <Extension>
    Public Function Match$(Text$, Regexp$)
        Return New Regex(Regexp).Matches(Text).OfType(Of Match).FirstOrDefault?.Value
    End Function
End Module
