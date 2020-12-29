Imports Windows.Storage

Module Configuracion

    Public Sub Cargar()

        Dim recursos As New Resources.ResourceLoader()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonAbrirConfig As Button = pagina.FindName("botonAbrirConfig")

        AddHandler botonAbrirConfig.Click, AddressOf AbrirConfigClick
        AddHandler botonAbrirConfig.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonAbrirConfig.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

        Dim botonAbrirMasTiles As Button = pagina.FindName("botonAbrirMasTiles")

        AddHandler botonAbrirMasTiles.Click, AddressOf AbrirMasTilesClick
        AddHandler botonAbrirMasTiles.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonAbrirMasTiles.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

        Dim botonConfigImagen As Button = pagina.FindName("botonConfigImagen")

        AddHandler botonConfigImagen.Click, AddressOf AbrirImagenClick
        AddHandler botonConfigImagen.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonConfigImagen.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

        Dim cbDiscordConfigModosTiles As ComboBox = pagina.FindName("cbDiscordConfigModosTiles")
        cbDiscordConfigModosTiles.Items.Add(recursos.getstring("Discord_ConfigImport1"))
        cbDiscordConfigModosTiles.Items.Add(recursos.getstring("Discord_ConfigImport2"))

        If ApplicationData.Current.LocalSettings.Values("modo_tiles") Is Nothing Then
            ApplicationData.Current.LocalSettings.Values("modo_tiles") = 0
            cbDiscordConfigModosTiles.SelectedIndex = 0

            Dim spDiscordConfigModo1 As StackPanel = pagina.FindName("spDiscordConfigModo1")
            spDiscordConfigModo1.Visibility = Visibility.Visible
        Else
            cbDiscordConfigModosTiles.SelectedIndex = ApplicationData.Current.LocalSettings.Values("modo_tiles")

            Dim spDiscordConfigModo1 As StackPanel = pagina.FindName("spDiscordConfigModo1")
            Dim spDiscordConfigModo2 As StackPanel = pagina.FindName("spDiscordConfigModo2")

            If cbDiscordConfigModosTiles.SelectedIndex = 0 Then
                spDiscordConfigModo1.Visibility = Visibility.Visible
                spDiscordConfigModo2.Visibility = Visibility.Collapsed
            Else
                spDiscordConfigModo1.Visibility = Visibility.Collapsed
                spDiscordConfigModo2.Visibility = Visibility.Visible
            End If
        End If

        AddHandler cbDiscordConfigModosTiles.SelectionChanged, AddressOf CambiarProcedenciaSelect
        AddHandler cbDiscordConfigModosTiles.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Basico
        AddHandler cbDiscordConfigModosTiles.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Basico

        Dim botonBuscarCarpetaDiscord As Button = pagina.FindName("botonBuscarCarpetaDiscord")

        AddHandler botonBuscarCarpetaDiscord.Click, AddressOf BuscarDiscordClick
        AddHandler botonBuscarCarpetaDiscord.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonBuscarCarpetaDiscord.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

        Dim botonBuscarFicheroDiscord As Button = pagina.FindName("botonBuscarFicheroDiscord")

        AddHandler botonBuscarFicheroDiscord.Click, AddressOf BuscarDiscordClick
        AddHandler botonBuscarFicheroDiscord.PointerEntered, AddressOf Interfaz.EfectosHover.Entra_Boton_IconoTexto
        AddHandler botonBuscarFicheroDiscord.PointerExited, AddressOf Interfaz.EfectosHover.Sale_Boton_IconoTexto

    End Sub

    Private Sub AbrirConfigClick(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridConfig As Grid = pagina.FindName("gridConfig")

        Dim recursos As New Resources.ResourceLoader()
        Interfaz.Pestañas.Visibilidad(gridConfig, recursos.GetString("Config"), sender)

    End Sub

    Private Sub AbrirMasTilesClick(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonMasTiles As Button = pagina.FindName("botonMasTiles")
        botonMasTiles.Flyout.ShowAt(botonMasTiles)

    End Sub

    Private Sub AbrirImagenClick(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim grid As Grid = pagina.FindName("gridConfigImagen")
        Dim icono As FontAwesome5.FontAwesome = pagina.FindName("iconoConfigImagen")

        If grid.Visibility = Visibility.Collapsed Then
            grid.Visibility = Visibility.Visible
            icono.Icon = FontAwesome5.EFontAwesomeIcon.Solid_AngleDoubleUp
        Else
            grid.Visibility = Visibility.Collapsed
            icono.Icon = FontAwesome5.EFontAwesomeIcon.Solid_AngleDoubleDown
        End If

    End Sub

    Private Sub CambiarProcedenciaSelect(sender As Object, e As SelectionChangedEventArgs)

        Dim cb As ComboBox = sender
        ApplicationData.Current.LocalSettings.Values("modo_tiles") = cb.SelectedIndex

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim spDiscordConfigModo1 As StackPanel = pagina.FindName("spDiscordConfigModo1")
        Dim spDiscordConfigModo2 As StackPanel = pagina.FindName("spDiscordConfigModo2")

        If cb.SelectedIndex = 0 Then
            spDiscordConfigModo1.Visibility = Visibility.Visible
            spDiscordConfigModo2.Visibility = Visibility.Collapsed
        Else
            spDiscordConfigModo1.Visibility = Visibility.Collapsed
            spDiscordConfigModo2.Visibility = Visibility.Visible
        End If

    End Sub

    Private Sub BuscarDiscordClick(sender As Object, e As RoutedEventArgs)

        Discord.Generar(True)

    End Sub

    Public Sub Estado(estado As Boolean)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonAbrirConfig As Button = pagina.FindName("botonAbrirConfig")
        botonAbrirConfig.IsEnabled = estado

        Dim botonConfigImagen As Button = pagina.FindName("botonConfigImagen")
        botonConfigImagen.IsEnabled = estado

        Dim cbDiscordConfigModosTiles As ComboBox = pagina.FindName("cbDiscordConfigModosTiles")
        cbDiscordConfigModosTiles.IsEnabled = estado

        Dim botonBuscarCarpetaDiscord As Button = pagina.FindName("botonBuscarCarpetaDiscord")
        botonBuscarCarpetaDiscord.IsEnabled = estado

        Dim botonBuscarFicheroDiscord As Button = pagina.FindName("botonBuscarFicheroDiscord")
        botonBuscarFicheroDiscord.IsEnabled = estado

    End Sub

End Module
