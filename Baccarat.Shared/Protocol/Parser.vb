Imports System
Imports System.Linq

Namespace Protocol
    Public Module Parser
        Public Function TryParse(line As String, ByRef message As Message) As Boolean
            If String.IsNullOrWhiteSpace(line) Then Return False

            Dim parts = line.Split(","c)
            If parts.Length = 0 Then Return False

            Dim cmd = parts(0).Trim().ToUpperInvariant()
            Dim args = parts.Skip(1).Select(Function(p) p.Trim()).ToArray()
            message = New Message(cmd, args)
            Return True
        End Function
    End Module
End Namespace
