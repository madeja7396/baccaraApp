Imports System
Imports System.IO

Namespace Util
    ' Lightweight logger that can write to UI callback and optional file.
    Public Class Logger
        Private ReadOnly _writeAction As Action(Of String)
        Private ReadOnly _filePath As String

        Public Sub New(writeAction As Action(Of String), Optional filePath As String = Nothing)
            _writeAction = writeAction
            _filePath = filePath
        End Sub

        Public Sub Info(message As String)
            Write("EVT", message)
        End Sub

        Public Sub Send(handle As String, message As String)
            Write("SND", $"[{handle}] {message}")
        End Sub

        Public Sub Receive(handle As String, message As String)
            Write("RCV", $"[{handle}] {message}")
        End Sub

        Public Sub [Error](message As String)
            Write("ERR", message)
        End Sub

        Private Sub Write(kind As String, message As String)
            Dim line = $"[{DateTime.Now:HH:mm:ss}] [{kind}] {message}"
            _writeAction?.Invoke(line)

            If Not String.IsNullOrEmpty(_filePath) Then
                File.AppendAllText(_filePath, line & Environment.NewLine)
            End If
        End Sub
    End Class
End Namespace
