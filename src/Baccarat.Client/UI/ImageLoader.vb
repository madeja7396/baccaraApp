Imports System.Drawing
Imports Baccarat.Shared.Model

Namespace Baccarat.Client.UI
    Public Class ImageLoader
        Private ReadOnly _basePath As String

        Public Sub New(basePath As String)
            _basePath = basePath
        End Sub

        Public Function GetCardBitmap(card As Card) As Bitmap
            Dim code = card.ToCode().Replace("-", "_")
            Dim path = IO.Path.Combine(_basePath, $"{code}.bmp")
            If IO.File.Exists(path) Then
                Return New Bitmap(path)
            End If
            Dim backPath = IO.Path.Combine(_basePath, "BACK.bmp")
            If IO.File.Exists(backPath) Then
                Return New Bitmap(backPath)
            End If
            Return Nothing
        End Function
    End Class
End Namespace
