Imports System

Namespace Baccarat.Shared.Protocol
    Public Class Message
        Public Property Command As String
        Public Property Params As String()

        Public Sub New(cmd As String, ParamArray args As String())
            Command = cmd
            Params = args
        End Sub

        Public Function ToLine() As String
            If Params Is Nothing OrElse Params.Length = 0 Then
                Return Command & "\n"
            End If
            Return Command & "," & String.Join(",", Params) & "\n"
        End Function
    End Class
End Namespace
