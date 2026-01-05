Imports Baccarat.Shared.Model
Imports Baccarat.Shared.Protocol

Namespace Baccarat.Client.UI
    Public Class UiBinder
        ' Bridge between GameState and FormGame controls
        Public Sub UpdateStatus(form As Forms.FormGame, state As GameState)
            form.lblPhase.Text = state.Phase.ToString()
            form.lblRound.Text = state.RoundIndex.ToString()
            ' TODO: update names/chips once bindings exist
        End Sub
    End Class
End Namespace
