Module DiscordBBDD

    Public Function Listado()
        Dim lista As New List(Of DiscordBBDDImagenVertical) From {
            New DiscordBBDDImagenVertical("460940655984771072", "https://i.imgur.com/x1CWEXh.png"),
            New DiscordBBDDImagenVertical("469807750084296716", "https://i.imgur.com/F1U0U7Y.png"),
            New DiscordBBDDImagenVertical("487683851519393822", "https://i.imgur.com/w2t8OQw.png"),
            New DiscordBBDDImagenVertical("492162663976140810", "https://i.imgur.com/NoHWBU9.png")
        }

        Return lista
    End Function

End Module

Public Class DiscordBBDDImagenVertical

    Public ID As String
    Public Enlace As String

    Public Sub New(ByVal id As String, ByVal enlace As String)
        Me.ID = id
        Me.Enlace = enlace
    End Sub

End Class
