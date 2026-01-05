Imports System.Text.RegularExpressions
Imports Baccarat.Shared.Model

Namespace Baccarat.Shared.Util
    Public Module Validate
        Private ReadOnly IpPattern As New Regex("^(?:\\d{1,3}\\.){3}\\d{1,3}$")

        Public Function Nickname(name As String) As Boolean
            If String.IsNullOrWhiteSpace(name) Then Return False
            Return name.Length <= Constants.NicknameMaxLen
        End Function

        Public Function IpAddress(ip As String) As Boolean
            Return Not String.IsNullOrWhiteSpace(ip) AndAlso IpPattern.IsMatch(ip)
        End Function

        Public Function Bet(betInfo As Model.BetInfo, chips As Integer) As Boolean
            If betInfo Is Nothing Then Return False
            If betInfo.Amount <= 0 Then Return False
            If betInfo.Amount > chips Then Return False
            Return True
        End Function
    End Module
End Namespace
