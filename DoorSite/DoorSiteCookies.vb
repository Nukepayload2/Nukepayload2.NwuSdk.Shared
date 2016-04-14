Namespace DoorSite
    ''' <summary>
    ''' 在门户网站使用各个功能需要这两个参数
    ''' </summary>
    Public Class DoorSiteCookies
        ''' <summary>
        ''' 原名 JSESSIONID，获取登录页面时得到这个Cookie
        ''' </summary>
        ''' <returns></returns>
        Public Property SessionId$
        ''' <summary>
        ''' 原名 iPlanetDirectoryPro，登录成功后得到这个Cookie
        ''' </summary>
        ''' <returns></returns>
        Public Property PlanetDirectoryProfile$
        Friend Sub New()

        End Sub
    End Class
End Namespace
