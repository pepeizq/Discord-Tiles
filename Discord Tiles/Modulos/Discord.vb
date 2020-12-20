Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.Storage.Streams
Imports Windows.UI

Module Discord

    Public anchoColumna As Integer = 180
    Dim dominioImagenes As String = "https://cdn.cloudflare.steamstatic.com"

    Dim clave1 As String = "DiscordCarpeta2"
    Dim clave2 As String = "DiscordFichero"

    Public Async Sub Generar(buscar As Boolean)

        Dim modo As Integer = ApplicationData.Current.LocalSettings.Values("modo_tiles")

        Dim helper As New LocalObjectStorageHelper

        Dim recursos As New Resources.ResourceLoader()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim pbProgreso As ProgressBar = pagina.FindName("pbProgreso")
        pbProgreso.Value = 0

        Dim tbProgreso As TextBlock = pagina.FindName("tbProgreso")
        tbProgreso.Text = String.Empty

        Configuracion.Estado(False)
        Cache.Estado(False)

        Dim gv As AdaptiveGridView = pagina.FindName("gvTiles")
        gv.DesiredWidth = anchoColumna
        gv.Items.Clear()

        Dim listaJuegos As New List(Of Tile)

        If Await helper.FileExistsAsync("juegos" + modo.ToString) = True Then
            listaJuegos = Await helper.ReadFileAsync(Of List(Of Tile))("juegos" + modo.ToString)
        End If

        If listaJuegos Is Nothing Then
            listaJuegos = New List(Of Tile)
        End If

        If modo = 0 Then
            Dim carpeta As StorageFolder = Nothing

            Try
                If buscar = True Then
                    Dim carpetapicker As New FolderPicker()

                    carpetapicker.FileTypeFilter.Add("*")
                    carpetapicker.ViewMode = PickerViewMode.List

                    carpeta = Await carpetapicker.PickSingleFolderAsync()

                    If Not carpeta Is Nothing Then
                        StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave1, carpeta)
                    End If
                End If
            Catch ex As Exception

            End Try

            Try
                If carpeta Is Nothing Then
                    carpeta = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave1)
                End If
            Catch ex As Exception

            End Try

            If Not carpeta Is Nothing Then
                Dim gridProgreso As Grid = pagina.FindName("gridProgreso")
                Interfaz.Pestañas.Visibilidad_Pestañas(gridProgreso, Nothing)

                Dim carpetasJuegos As IReadOnlyList(Of StorageFolder) = Await carpeta.GetFoldersAsync()

                Dim k As Integer = 0
                For Each carpetaJuego As StorageFolder In carpetasJuegos
                    Dim ficheros As IReadOnlyList(Of StorageFile) = Await carpetaJuego.GetFilesAsync()

                    For Each fichero As StorageFile In ficheros
                        Dim nombreFichero As String = fichero.DisplayName.ToLower

                        If nombreFichero = "application_info" Then
                            Dim datosTexto As String = Await FileIO.ReadTextAsync(fichero)
                            Dim datos As DiscordDatos = JsonConvert.DeserializeObject(Of DiscordDatos)(datosTexto)

                            If Not datos Is Nothing Then
                                Dim titulo As String = datos.Titulo

                                Dim añadir As Boolean = True
                                Dim i As Integer = 0
                                While i < listaJuegos.Count
                                    If listaJuegos(i).ID = datos.IDJuego Then
                                        añadir = False
                                    End If
                                    i += 1
                                End While

                                If añadir = True Then
                                    Dim imagenLogo As String = Await Cache.DescargarImagen(Nothing, datos.IDJuego, "logo")
                                    Dim imagenAncha As String = Await Cache.DescargarImagen(Nothing, datos.IDJuego, "ancha")
                                    Dim imagenGrande As String = Await Cache.DescargarImagen(Nothing, datos.IDJuego, "grande")

                                    Dim imagenIcono As String = String.Empty

                                    Try
                                        imagenIcono = Await Cache.DescargarImagen("https://cdn.discordapp.com/game-assets/" + datos.IDJuego + "/" + datos.IDIcono + ".png?size=1024", datos.IDJuego, "icono")
                                    Catch ex As Exception

                                    End Try

                                    Dim idSteam As String = Nothing
                                    Dim htmlSteam As String = Await Decompiladores.HttpClient(New Uri("https://store.steampowered.com/search/?term=" + datos.Titulo.Replace(" ", "+")))

                                    If Not htmlSteam = Nothing Then
                                        Dim temp5, temp6 As String
                                        Dim int5, int6 As Integer

                                        int5 = htmlSteam.IndexOf("<!-- List Items -->")

                                        If Not int5 = -1 Then
                                            temp5 = htmlSteam.Remove(0, int5)

                                            int5 = temp5.IndexOf("<span class=" + ChrW(34) + "title" + ChrW(34) + ">" + datos.Titulo + "</span>")

                                            If Not int5 = -1 Then
                                                temp5 = temp5.Remove(int5, temp5.Length - int5)

                                                int5 = temp5.LastIndexOf("data-ds-appid=")
                                                temp5 = temp5.Remove(0, int5 + 15)

                                                int6 = temp5.IndexOf(ChrW(34))
                                                temp6 = temp5.Remove(int6, temp5.Length - int6)

                                                idSteam = temp6.Trim

                                                If imagenAncha = String.Empty Then
                                                    Try
                                                        imagenAncha = Await Cache.DescargarImagen(dominioImagenes + "/steam/apps/" + idSteam + "/header.jpg", idSteam, "ancha")
                                                    Catch ex As Exception

                                                    End Try
                                                End If

                                                If imagenGrande = String.Empty Then
                                                    Try
                                                        imagenGrande = Await Cache.DescargarImagen(dominioImagenes + "/steam/apps/" + idSteam + "/library_600x900.jpg", idSteam, "grande")
                                                    Catch ex As Exception

                                                    End Try

                                                    Try
                                                        imagenGrande = Await Cache.DescargarImagen(dominioImagenes + "/steam/apps/" + idSteam + "/capsule_616x353.jpg", idSteam, "grande")
                                                    Catch ex As Exception

                                                    End Try
                                                End If
                                            End If
                                        End If
                                    End If

                                    Dim juego As New Tile(titulo, datos.IDJuego, "discord:///library/" + datos.IDJuego + "/launch",
                                                          imagenIcono, imagenLogo, imagenAncha, imagenGrande)

                                    listaJuegos.Add(juego)
                                End If
                            End If
                        End If
                    Next

                    pbProgreso.Value = CInt((100 / carpetasJuegos.Count) * k)
                    tbProgreso.Text = k.ToString + "/" + carpetasJuegos.Count.ToString
                    k += 1
                Next
            End If
        ElseIf modo = 1 Then
            Dim fichero As StorageFile = Nothing

            Try
                If buscar = True Then
                    Dim ficheroPicker As New FileOpenPicker()

                    ficheroPicker.FileTypeFilter.Add(".log")
                    ficheroPicker.ViewMode = PickerViewMode.List

                    fichero = Await ficheroPicker.PickSingleFileAsync
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave2, fichero)
                Else
                    fichero = Await StorageApplicationPermissions.FutureAccessList.GetFileAsync(clave2)
                End If
            Catch ex As Exception

            End Try

            If Not fichero Is Nothing Then
                Dim gridProgreso As Grid = pagina.FindName("gridProgreso")
                Interfaz.Pestañas.Visibilidad_Pestañas(gridProgreso, Nothing)

                If fichero.FileType.Contains(".log") Then
                    Dim stream As IRandomAccessStreamWithContentType = Await fichero.OpenReadAsync()
                    Dim stream2 As Stream = stream.AsStreamForRead

                    Dim lineas As String = String.Empty
                    Using stream3 As New StreamReader(stream2)
                        lineas = lineas + stream3.ReadLineAsync.Result
                    End Using

                    Dim i As Integer = 0
                    While i < 1000
                        If Not lineas.Contains(ChrW(34) + "distributor" + ChrW(34) + ":" + ChrW(34) + "steam" + ChrW(34)) Then
                            Exit While
                        Else
                            Dim temp, temp2, temp3 As String
                            Dim int, int2, int3 As Integer

                            int = lineas.IndexOf(ChrW(34) + "distributor" + ChrW(34) + ":" + ChrW(34) + "steam" + ChrW(34))
                            temp = lineas.Remove(int, lineas.Length - int)

                            lineas = lineas.Remove(0, int + 5)

                            int2 = temp.LastIndexOf(ChrW(34) + "sku" + ChrW(34))
                            temp2 = temp.Remove(0, int2 + 7)

                            int3 = temp2.IndexOf(ChrW(34))
                            temp3 = temp2.Remove(int3, temp2.Length - int3)

                            Dim id As String = temp3.Trim

                            Dim añadir As Boolean = True
                            Dim j As Integer = 0
                            While j < listaJuegos.Count
                                If listaJuegos(j).ID = id Then
                                    añadir = False
                                End If
                                j += 1
                            End While

                            If añadir = True Then
                                Dim htmlAPI As String = Await HttpClient(New Uri("https://store.steampowered.com/api/appdetails/?appids=" + id))

                                If Not htmlAPI = Nothing Then
                                    Dim temp4 As String
                                    Dim int4 As Integer

                                    int4 = htmlAPI.IndexOf(":")
                                    temp4 = htmlAPI.Remove(0, int4 + 1)
                                    temp4 = temp4.Remove(temp4.Length - 1, 1)

                                    Dim api As SteamAPI = JsonConvert.DeserializeObject(Of SteamAPI)(temp4)

                                    If Not api Is Nothing Then
                                        If Not api.Datos Is Nothing Then
                                            Dim imagenLogo As String = String.Empty

                                            Try
                                                imagenLogo = Await Cache.DescargarImagen(dominioImagenes + "/steam/apps/" + id + "/logo.png", id, "logo")
                                            Catch ex As Exception

                                            End Try

                                            Dim imagenAncha As String = String.Empty

                                            Try
                                                imagenAncha = Await Cache.DescargarImagen(dominioImagenes + "/steam/apps/" + id + "/header.jpg", id, "ancha")
                                            Catch ex As Exception

                                            End Try

                                            Dim imagenGrande As String = String.Empty

                                            Try
                                                imagenGrande = Await Cache.DescargarImagen(dominioImagenes + "/steam/apps/" + id + "/library_600x900.jpg", id, "grande")
                                            Catch ex As Exception

                                            End Try

                                            If imagenGrande = String.Empty Then
                                                Try
                                                    imagenGrande = Await Cache.DescargarImagen(dominioImagenes + "/steam/apps/" + id + "/capsule_616x353.jpg", id, "grande")
                                                Catch ex As Exception

                                                End Try
                                            End If

                                            Dim juego As New Tile(api.Datos.Titulo, id, "steam://rungameid/" + id, Nothing, imagenLogo, imagenAncha, imagenGrande)

                                            listaJuegos.Add(juego)
                                        End If
                                    End If
                                End If
                            End If
                        End If

                        pbProgreso.Value = i.ToString
                        tbProgreso.Text = i.ToString
                        i += 1
                    End While
                End If
            End If

            If Not listaJuegos Is Nothing Then
                If listaJuegos.Count > 0 Then
                    StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave2, fichero)
                End If
            End If
        End If

        Try
            Await helper.SaveFileAsync(Of List(Of Tile))("juegos" + modo.ToString, listaJuegos)
        Catch ex As Exception

        End Try

        If Not listaJuegos Is Nothing Then
            If listaJuegos.Count > 0 Then
                Dim gridJuegos As Grid = pagina.FindName("gridJuegos")
                Interfaz.Pestañas.Visibilidad_Pestañas(gridJuegos, recursos.GetString("Games"))

                listaJuegos.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

                gv.Items.Clear()

                For Each juego In listaJuegos
                    BotonEstilo(juego, gv)
                Next
            Else
                Dim gridAvisoNoJuegos As Grid = pagina.FindName("gridAvisoNoJuegos")
                Interfaz.Pestañas.Visibilidad_Pestañas(gridAvisoNoJuegos, Nothing)
            End If
        Else
            Dim gridAvisoNoJuegos As Grid = pagina.FindName("gridAvisoNoJuegos")
            Interfaz.Pestañas.Visibilidad_Pestañas(gridAvisoNoJuegos, Nothing)
        End If

        Configuracion.Estado(True)
        Cache.Estado(True)

    End Sub

    Public Sub BotonEstilo(juego As Tile, gv As GridView)

        Dim panel As New DropShadowPanel With {
            .Margin = New Thickness(10, 10, 10, 10),
            .ShadowOpacity = 0.9,
            .BlurRadius = 10,
            .MaxWidth = anchoColumna + 20,
            .HorizontalAlignment = HorizontalAlignment.Center,
            .VerticalAlignment = VerticalAlignment.Center
        }

        Dim boton As New Button

        Dim imagen As New ImageEx With {
            .Source = juego.ImagenGrande,
            .IsCacheEnabled = True,
            .Stretch = Stretch.UniformToFill,
            .Padding = New Thickness(0, 0, 0, 0),
            .HorizontalAlignment = HorizontalAlignment.Center,
            .VerticalAlignment = VerticalAlignment.Center,
            .EnableLazyLoading = True
        }

        boton.Tag = juego
        boton.Content = imagen
        boton.Padding = New Thickness(0, 0, 0, 0)
        boton.Background = New SolidColorBrush(Colors.Transparent)

        panel.Content = boton

        Dim tbToolTip As TextBlock = New TextBlock With {
            .Text = juego.Titulo,
            .FontSize = 16,
            .TextWrapping = TextWrapping.Wrap
        }

        ToolTipService.SetToolTip(boton, tbToolTip)
        ToolTipService.SetPlacement(boton, PlacementMode.Mouse)

        AddHandler boton.Click, AddressOf BotonTile_Click
        AddHandler boton.PointerEntered, AddressOf Interfaz.Entra_Boton_Imagen
        AddHandler boton.PointerExited, AddressOf Interfaz.Sale_Boton_Imagen

        gv.Items.Add(panel)

    End Sub

    Private Async Sub BotonTile_Click(sender As Object, e As RoutedEventArgs)

        Trial.Detectar()
        Interfaz.AñadirTile.ResetearValores()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonJuego As Button = e.OriginalSource
        Dim juego As Tile = botonJuego.Tag

        Dim botonAñadirTile As Button = pagina.FindName("botonAñadirTile")
        botonAñadirTile.Tag = juego

        Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
        imagenJuegoSeleccionado.Source = juego.ImagenAncha

        Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
        tbJuegoSeleccionado.Text = juego.Titulo

        Dim gridAñadirTile As Grid = pagina.FindName("gridAñadirTile")
        Interfaz.Pestañas.Visibilidad_Pestañas(gridAñadirTile, juego.Titulo)

        '---------------------------------------------

        Dim imagenPequeña As ImageEx = pagina.FindName("imagenTilePequeña")
        imagenPequeña.Source = Nothing

        Dim imagenMediana As ImageEx = pagina.FindName("imagenTileMediana")
        imagenMediana.Source = Nothing

        Dim imagenAncha As ImageEx = pagina.FindName("imagenTileAncha")
        imagenAncha.Source = Nothing

        Dim imagenGrande As ImageEx = pagina.FindName("imagenTileGrande")
        imagenGrande.Source = Nothing

        Try
            juego.ImagenPequeña = Await Cache.DescargarImagen(Await SacarIcono(juego.ID), juego.ID, "icono")
        Catch ex As Exception

        End Try

        If Not juego.ImagenPequeña = Nothing Then
            imagenPequeña.Source = juego.ImagenPequeña
            imagenPequeña.Tag = juego.ImagenPequeña
        End If

        If Not juego.ImagenAncha = Nothing Then
            If Not juego.ImagenMediana = Nothing Then
                imagenMediana.Source = juego.ImagenMediana
                imagenMediana.Tag = juego.ImagenMediana
            Else
                imagenMediana.Source = juego.ImagenAncha
                imagenMediana.Tag = juego.ImagenAncha
            End If

            imagenAncha.Source = juego.ImagenAncha
            imagenAncha.Tag = juego.ImagenAncha
        End If

        If Not juego.ImagenGrande = Nothing Then
            imagenGrande.Source = juego.ImagenGrande
            imagenGrande.Tag = juego.ImagenGrande
        End If

    End Sub

    Public Async Function SacarIcono(id As String) As Task(Of String)

        Dim modo As Integer = ApplicationData.Current.LocalSettings.Values("modo_tiles")

        Dim helper As New LocalObjectStorageHelper

        If Await helper.FileExistsAsync("juegos" + modo.ToString) = True Then
            Dim listaJuegos As List(Of Tile) = Await helper.ReadFileAsync(Of List(Of Tile))("juegos" + modo.ToString)

            For Each juego In listaJuegos
                If id = juego.ID Then
                    If Not juego.ImagenPequeña = Nothing Then
                        Return juego.ImagenPequeña
                    End If
                End If
            Next
        End If

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://store.steampowered.com/app/" + id + "/"))
        Dim urlIcono As String = String.Empty

        If Not html = Nothing Then
            If html.Contains("<div class=" + ChrW(34) + "apphub_AppIcon") Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = html.IndexOf("<div class=" + ChrW(34) + "apphub_AppIcon")
                temp = html.Remove(0, int)

                int = temp.IndexOf("<img src=")
                temp = temp.Remove(0, int + 10)

                int2 = temp.IndexOf(ChrW(34))
                temp2 = temp.Remove(int2, temp.Length - int2)

                temp2 = temp2.Replace("%CDN_HOST_MEDIA_SSL%", "steamcdn-a.akamaihd.net")

                urlIcono = temp2.Trim
            End If
        End If

        If urlIcono = Nothing Then
            html = Await Decompiladores.HttpClient(New Uri("https://steamdb.info/app/" + id + "/"))

            If Not html = Nothing Then
                If html.Contains("<img class=" + ChrW(34) + "app-icon avatar") Then
                    Dim temp, temp2 As String
                    Dim int, int2 As Integer

                    int = html.IndexOf("<img class=" + ChrW(34) + "app-icon avatar")
                    temp = html.Remove(0, int)

                    int = temp.IndexOf("src=")
                    temp = temp.Remove(0, int + 5)

                    int2 = temp.IndexOf(ChrW(34))
                    temp2 = temp.Remove(int2, temp.Length - int2)

                    urlIcono = temp2.Trim
                End If
            End If
        End If

        If Not urlIcono = String.Empty Then
            If Await helper.FileExistsAsync("juegos" + modo.ToString) = True Then
                Dim listaJuegos As List(Of Tile) = Await helper.ReadFileAsync(Of List(Of Tile))("juegos" + modo.ToString)

                For Each juego In listaJuegos
                    If id = juego.ID Then
                        juego.ImagenPequeña = Await Cache.DescargarImagen(urlIcono, id, "icono")
                    End If
                Next

                Await helper.SaveFileAsync(Of List(Of Tile))("juegos" + modo.ToString, listaJuegos)
            End If
        End If

        Return urlIcono
    End Function

End Module

Public Class DiscordDatos

    <JsonProperty("name")>
    Public Titulo As String

    <JsonProperty("application_id")>
    Public IDJuego As String

    <JsonProperty("icon_hash")>
    Public IDIcono As String

End Class
