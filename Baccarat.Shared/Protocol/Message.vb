Imports System

Namespace Protocol
    ''' <summary>
    ''' 通信メッセージのデータ構造
    ''' </summary>
    ''' <remarks>
    ''' 【通信プロトコル仕様】
    ''' 
    ''' - 形式: テキストベース (CSV風)
    ''' - 区切り文字: カンマ (`,`)
    ''' - 終端文字: 改行 (`\n`)
    ''' - 構成: `COMMAND,Param1,Param2,...`
    ''' 
    ''' (例) `HELLO,Yamada`
    '''      `PHASE,BETTING,1`
    ''' </remarks>
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
