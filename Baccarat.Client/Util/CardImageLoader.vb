Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Namespace Util
    Public Module CardImageLoader

        ' suit: 0=♠, 1=♥, 2=♦, 3=♣
        ' rank: 0=A, 1=2, ... 12=K
        Public Function CardPath(rank As Integer, suit As Integer) As String
            Dim fileName = $"{rank:00}_{suit:00}.bmp"
            Return Path.Combine(Application.StartupPath, "assets", "cards", fileName)
        End Function

        Public Function LoadCardImage(rank As Integer, suit As Integer) As Image
            Dim path = CardPath(rank, suit)
            If Not File.Exists(path) Then
                Throw New FileNotFoundException("Card image not found: " & path)
            End If

            ' 重要：Image.FromFile はファイルロックするので、コピーして返す
            Using tmp As Image = Image.FromFile(path)
                Return CType(tmp.Clone(), Image)
            End Using
        End Function

        ' "00_01" のような文字列から Image を返す
        Public Function LoadCardImage(cardId As String) As Image
            Dim parts = cardId.Trim().Split("_"c)
            If parts.Length <> 2 Then Throw New ArgumentException("Invalid cardId: " & cardId)

            Dim rank As Integer = Integer.Parse(parts(0))
            Dim suit As Integer = Integer.Parse(parts(1))
            Return LoadCardImage(rank, suit)
        End Function

        ' バカラ点数（A=1,2..9=そのまま,10/J/Q/K=0）
        Public Function BaccaratPoint(rank As Integer) As Integer
            If rank = 0 Then Return 1          ' A
            If rank >= 1 AndAlso rank <= 8 Then Return rank + 1 ' 2..9
            Return 0                           ' 10,J,Q,K
        End Function

        Public Function BaccaratPoint(cardId As String) As Integer
            Dim parts = cardId.Trim().Split("_"c)
            Dim rank As Integer = Integer.Parse(parts(0))
            Return BaccaratPoint(rank)
        End Function

    End Module
End Namespace
