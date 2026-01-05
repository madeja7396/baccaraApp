Imports System.Text

Namespace Protocol
    ' Line-oriented framer to reconstruct messages split by TCP.
    Public Class LineFramer
        Private ReadOnly _buf As New StringBuilder()

        Public Event LineReceived(line As String)

        Public Sub Push(chunk As String)
            If String.IsNullOrEmpty(chunk) Then Return
            _buf.Append(chunk)

            While True
                Dim s As String = _buf.ToString()
                Dim idx As Integer = s.IndexOf(vbLf, StringComparison.Ordinal)
                If idx < 0 Then Exit While

                Dim line As String = s.Substring(0, idx).TrimEnd(ChrW(13)) ' remove CR
                _buf.Remove(0, idx + 1)
                RaiseEvent LineReceived(line)
            End While
        End Sub
    End Class
End Namespace
