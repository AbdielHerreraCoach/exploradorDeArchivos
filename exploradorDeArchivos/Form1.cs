using System;
using System.IO;
using System.Windows.Forms;

namespace exploradorDeArchivos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Opcional: Cargar el disco C por defecto al arrancar el programa
            txtRuta.Text = @"C:\";
            CargarDirectorio(txtRuta.Text);
            ConfigurarPanelNavegacion();
            ConfigurarMenuContextual();
        }
        // Variables globales para el menú contextual
        private ContextMenuStrip menuContextual;
        private ToolStripMenuItem menuAbrir;
        private ToolStripMenuItem menuNuevaCarpeta;
        private ToolStripMenuItem menuRenombrar;
        private ToolStripMenuItem menuEliminar;
        private ToolStripMenuItem menuPropiedades;

        // Nuestro portapapeles interno
        private string rutaPortapapeles = "";
        private bool esOperacionCortar = false;

        // Los nuevos botones del menú
        private ToolStripMenuItem menuCopiar;
        private ToolStripMenuItem menuCortar;
        private ToolStripMenuItem menuPegar;
        private void ConfigurarPanelNavegacion()
        {
            treeNavegacion.ImageList = imgNavegacion;

            treeNavegacion.Nodes.Clear();

            // --- 2. DISEÑO DE TEXTO (FUENTE) ---
            // Creamos una fuente estilo Windows 10/11
            Font fuenteNodos = new Font("Segoe UI", 10F, FontStyle.Regular);

            // --- 3. CREACIÓN Y ESTILO DE NODOS ---

            // ESCRITORIO
            TreeNode nodoEscritorio = new TreeNode(" Escritorio"); // Espacio extra para separar del icono
            nodoEscritorio.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            nodoEscritorio.NodeFont = fuenteNodos;
            nodoEscritorio.ImageIndex = 0; // Usa la imagen 0 de tu imgListPequeños (Carpeta)
            nodoEscritorio.SelectedImageIndex = 0;

            // DOCUMENTOS
            TreeNode nodoDocumentos = new TreeNode(" Documentos");
            nodoDocumentos.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            nodoDocumentos.NodeFont = fuenteNodos;
            nodoDocumentos.ImageIndex = 2;
            nodoDocumentos.SelectedImageIndex = 2;

            // MÚSICA
            TreeNode nodoMusica = new TreeNode(" Música");
            nodoMusica.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            nodoMusica.NodeFont = fuenteNodos;
            // Si tuviéramos un icono de música en el índice 3, lo pondríamos así:
            nodoMusica.ImageIndex = 4;
            nodoMusica.SelectedImageIndex = 4;

            // IMÁGENES
            TreeNode nodoImagenes = new TreeNode(" Imágenes");
            nodoImagenes.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            nodoImagenes.NodeFont = fuenteNodos;
            nodoImagenes.ImageIndex = 3; // Índice de imagen en tu imgListPequeños
            nodoImagenes.SelectedImageIndex = 3;

            // DESCARGAS
            TreeNode nodoDescargas = new TreeNode(" Descargas");
            string rutaUsuario = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            nodoDescargas.Tag = System.IO.Path.Combine(rutaUsuario, "Downloads");
            nodoDescargas.NodeFont = fuenteNodos;
            nodoDescargas.ImageIndex = 1;
            nodoDescargas.SelectedImageIndex = 1;

            // Agregamos todo al árbol
            treeNavegacion.Nodes.Add(nodoEscritorio);
            treeNavegacion.Nodes.Add(nodoDescargas);
            treeNavegacion.Nodes.Add(nodoDocumentos);
            treeNavegacion.Nodes.Add(nodoImagenes);
            treeNavegacion.Nodes.Add(nodoMusica);
        }

        private void ConfigurarMenuContextual()
        {
            menuContextual = new ContextMenuStrip();

            menuAbrir = new ToolStripMenuItem("Abrir");
            menuCopiar = new ToolStripMenuItem("Copiar");
            menuCortar = new ToolStripMenuItem("Cortar");
            menuPegar = new ToolStripMenuItem("Pegar");
            menuNuevaCarpeta = new ToolStripMenuItem("Nueva carpeta");
            menuRenombrar = new ToolStripMenuItem("Renombrar");
            menuEliminar = new ToolStripMenuItem("Eliminar");
            menuPropiedades = new ToolStripMenuItem("Propiedades");

            // Agregamos en un orden lógico (como en Windows)
            menuContextual.Items.Add(menuAbrir);
            menuContextual.Items.Add(new ToolStripSeparator());
            menuContextual.Items.Add(menuCopiar);
            menuContextual.Items.Add(menuCortar);
            menuContextual.Items.Add(menuPegar);
            menuContextual.Items.Add(new ToolStripSeparator());
            menuContextual.Items.Add(menuNuevaCarpeta);
            menuContextual.Items.Add(menuRenombrar);
            menuContextual.Items.Add(menuEliminar);
            menuContextual.Items.Add(new ToolStripSeparator());
            menuContextual.Items.Add(menuPropiedades);

            listViewArchivos.ContextMenuStrip = menuContextual;

            // Conectamos los eventos
            menuContextual.Opening += MenuContextual_Opening;
            menuAbrir.Click += MenuAbrir_Click;
            menuNuevaCarpeta.Click += MenuNuevaCarpeta_Click;
            menuRenombrar.Click += MenuRenombrar_Click;
            menuEliminar.Click += MenuEliminar_Click;
            menuPropiedades.Click += MenuPropiedades_Click;

            // Conectamos los eventos del portapapeles
            menuCopiar.Click += MenuCopiar_Click;
            menuCortar.Click += MenuCortar_Click;
            menuPegar.Click += MenuPegar_Click;
        }
        private void MenuContextual_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool haySeleccion = listViewArchivos.SelectedItems.Count > 0;

            menuAbrir.Enabled = haySeleccion;
            menuRenombrar.Enabled = haySeleccion;
            menuEliminar.Enabled = haySeleccion;
            menuPropiedades.Enabled = haySeleccion;

            // Lógica del portapapeles
            menuCopiar.Enabled = haySeleccion;
            menuCortar.Enabled = haySeleccion;

            // Solo activamos Pegar si nuestra variable de memoria no está vacía
            menuPegar.Enabled = !string.IsNullOrEmpty(rutaPortapapeles);

            menuNuevaCarpeta.Enabled = true;
        }

        private void MenuAbrir_Click(object sender, EventArgs e)
        {
            // Simplemente llamamos a la función del doble clic que ya tienes hecha.
            // Le mandamos (null, null) porque esa función en realidad no usa esos parámetros internos.
            listViewArchivos_DoubleClick(null, null);
        }

        private void MenuNuevaCarpeta_Click(object sender, EventArgs e)
        {
            try
            {
                // Nos aseguramos de estar en una ruta válida
                if (string.IsNullOrEmpty(txtRuta.Text)) return;

                string rutaBase = txtRuta.Text;
                string nombreCarpeta = "Nueva carpeta";
                string rutaCompleta = System.IO.Path.Combine(rutaBase, nombreCarpeta);

                // Lógica por si ya existe una "Nueva carpeta" (Crea "Nueva carpeta (1)", "(2)", etc.)
                int contador = 1;
                while (System.IO.Directory.Exists(rutaCompleta))
                {
                    nombreCarpeta = $"Nueva carpeta ({contador})";
                    rutaCompleta = System.IO.Path.Combine(rutaBase, nombreCarpeta);
                    contador++;
                }

                // Le pedimos a Windows que cree el folder físicamente
                System.IO.Directory.CreateDirectory(rutaCompleta);

                // Refrescamos tu explorador para que aparezca inmediatamente
                CargarDirectorio(rutaBase);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo crear la carpeta: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------------------------------------------------------
        // Dejamos los cascarones vacíos de las otras funciones para que no marque error.
        // Las llenaremos en nuestro próximo paso.

        // Función auxiliar para pedir el nuevo nombre al usuario
        private string PedirNuevoNombre(string nombreActual)
        {
            // Creamos un formulario nuevo 100% desde el código
            Form prompt = new Form()
            {
                Width = 400,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Renombrar",
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false
            };

            Label lblTexto = new Label() { Left = 20, Top = 20, Width = 340, Text = "Introduce el nuevo nombre:" };
            TextBox txtInput = new TextBox() { Left = 20, Top = 50, Width = 340, Text = nombreActual };
            Button btnAceptar = new Button() { Text = "Aceptar", Left = 260, Width = 100, Top = 80, DialogResult = DialogResult.OK };

            prompt.Controls.Add(lblTexto);
            prompt.Controls.Add(txtInput);
            prompt.Controls.Add(btnAceptar);
            prompt.AcceptButton = btnAceptar;

            // Si el usuario da Aceptar, devolvemos lo que escribió, si no, devolvemos vacío
            return prompt.ShowDialog() == DialogResult.OK ? txtInput.Text : "";
        }

        private void MenuRenombrar_Click(object sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0) return;

            ListViewItem item = listViewArchivos.SelectedItems[0];
            string nombreActual = item.Text;
            string tipo = item.SubItems[1].Text;
            string rutaActual = item.SubItems[2].Text;
            string rutaBase = txtRuta.Text; // La carpeta donde estamos parados

            // 1. Llamamos a nuestra ventanita para pedir el nombre
            string nuevoNombre = PedirNuevoNombre(nombreActual);

            // 2. Si el usuario escribió algo y es diferente al original, procedemos
            if (!string.IsNullOrWhiteSpace(nuevoNombre) && nuevoNombre != nombreActual)
            {
                try
                {
                    // Construimos la nueva ruta completa
                    string nuevaRuta = System.IO.Path.Combine(rutaBase, nuevoNombre);

                    if (tipo == "Carpeta")
                    {
                        System.IO.Directory.Move(rutaActual, nuevaRuta);
                    }
                    else
                    {
                        System.IO.File.Move(rutaActual, nuevaRuta);
                    }

                    // Refrescamos visualmente
                    CargarDirectorio(rutaBase);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo renombrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void MenuEliminar_Click(object sender, EventArgs e)
        {
            // Verificamos que haya algo seleccionado
            if (listViewArchivos.SelectedItems.Count == 0) return;

            // Obtenemos los datos del elemento a borrar
            ListViewItem item = listViewArchivos.SelectedItems[0];
            string nombreArchivo = item.Text;
            string tipo = item.SubItems[1].Text;
            string rutaActual = item.SubItems[2].Text;

            // 1. EL SEGURO: Preguntamos antes de disparar
            DialogResult confirmacion = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar permanentemente '{nombreArchivo}'?\nEsta acción no se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion == DialogResult.Yes)
            {
                try
                {
                    // 2. Evaluamos si es carpeta o archivo para usar la herramienta correcta
                    if (tipo == "Carpeta")
                    {
                        // El 'true' significa que borrará la carpeta y TODO su contenido recursivamente
                        System.IO.Directory.Delete(rutaActual, true);
                    }
                    else
                    {
                        System.IO.File.Delete(rutaActual);
                    }

                    // 3. Refrescamos tu explorador para que el archivo desaparezca visualmente
                    CargarDirectorio(txtRuta.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo eliminar: " + ex.Message, "Error de permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void MenuPropiedades_Click(object sender, EventArgs e)
        {
            // Verificamos que haya algo seleccionado
            if (listViewArchivos.SelectedItems.Count == 0) return;

            ListViewItem item = listViewArchivos.SelectedItems[0];
            string tipo = item.SubItems[1].Text;
            string rutaActual = item.SubItems[2].Text;

            try
            {
                string informacion = "";

                if (tipo == "Carpeta")
                {
                    // Extraemos la metadata de la carpeta
                    System.IO.DirectoryInfo infoCarpeta = new System.IO.DirectoryInfo(rutaActual);

                    informacion += $"Nombre: {infoCarpeta.Name}\n";
                    informacion += $"Tipo: Carpeta de archivos\n";
                    informacion += $"Ubicación: {infoCarpeta.Parent?.FullName}\n\n";

                    informacion += $"Fecha de creación: {infoCarpeta.CreationTime}\n";
                    informacion += $"Última modificación: {infoCarpeta.LastWriteTime}\n";

                    // Nota: No calculamos el tamaño de la carpeta aquí porque si es una carpeta 
                    // de sistema muy pesada, congelaría tu programa mientras suma todos los bytes.
                }
                else
                {
                    // Extraemos la metadata del archivo
                    System.IO.FileInfo infoArchivo = new System.IO.FileInfo(rutaActual);

                    informacion += $"Nombre: {infoArchivo.Name}\n";
                    informacion += $"Tipo: Archivo {infoArchivo.Extension.ToUpper()}\n";
                    informacion += $"Ubicación: {infoArchivo.DirectoryName}\n\n";

                    // Matemáticas simples para mostrar el tamaño en un formato legible
                    double tamañoKB = infoArchivo.Length / 1024.0;
                    double tamañoMB = tamañoKB / 1024.0;

                    if (tamañoMB >= 1)
                        informacion += $"Tamaño: {tamañoMB:F2} MB ({infoArchivo.Length:N0} bytes)\n\n";
                    else
                        informacion += $"Tamaño: {tamañoKB:F2} KB ({infoArchivo.Length:N0} bytes)\n\n";

                    informacion += $"Fecha de creación: {infoArchivo.CreationTime}\n";
                    informacion += $"Última modificación: {infoArchivo.LastWriteTime}\n";
                }

                // Mostramos el resumen en una ventana de diálogo nativa
                MessageBox.Show(informacion, "Propiedades", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron leer las propiedades: " + ex.Message, "Error de Lectura", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MenuCortar_Click(object sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0) return;

            // Guardamos la ruta y aseguramos que SÍ es cortar
            rutaPortapapeles = listViewArchivos.SelectedItems[0].SubItems[2].Text;
            esOperacionCortar = true;

            // EFECTO VISUAL: Ponemos el texto del archivo en color gris para que sepas que está "Cortado"
            listViewArchivos.SelectedItems[0].ForeColor = System.Drawing.Color.Gray;
        }

        private void MenuCopiar_Click(object sender, EventArgs e)
        {
            if (listViewArchivos.SelectedItems.Count == 0) return;

            rutaPortapapeles = listViewArchivos.SelectedItems[0].SubItems[2].Text;
            esOperacionCortar = false;

            // Si estaba gris porque antes le dimos cortar, lo regresamos a la normalidad (negro)
            listViewArchivos.SelectedItems[0].ForeColor = System.Drawing.Color.Black;
        }

        private void MenuPegar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(rutaPortapapeles)) return;

            try
            {
                string rutaDestinoActual = txtRuta.Text;
                string nombreElemento = System.IO.Path.GetFileName(rutaPortapapeles);
                string rutaFinal = System.IO.Path.Combine(rutaDestinoActual, nombreElemento);

                if (rutaPortapapeles == rutaFinal)
                {
                    MessageBox.Show("El archivo ya está en esta carpeta.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool esCarpeta = System.IO.Directory.Exists(rutaPortapapeles);

                if (esOperacionCortar) // --- SI FUE CORTAR (Mover) ---
                {
                    if (esCarpeta)
                        System.IO.Directory.Move(rutaPortapapeles, rutaFinal);
                    else
                        System.IO.File.Move(rutaPortapapeles, rutaFinal);

                    // Vaciamos la memoria obligatoriamente para no pegarlo 2 veces
                    rutaPortapapeles = "";
                    esOperacionCortar = false;
                }
                else // --- SI FUE COPIAR (Clonar) ---
                {
                    if (esCarpeta)
                        CopiarCarpetaRecursiva(rutaPortapapeles, rutaFinal);
                    else
                        System.IO.File.Copy(rutaPortapapeles, rutaFinal, true);
                }

                // Refrescamos la vista
                CargarDirectorio(rutaDestinoActual);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al transferir: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Función auxiliar maestra para copiar carpetas con todo su contenido
        private void CopiarCarpetaRecursiva(string origen, string destino)
        {
            System.IO.Directory.CreateDirectory(destino);

            // Copiamos todos los archivos de esta capa
            foreach (string archivoPath in System.IO.Directory.GetFiles(origen))
            {
                string archivoDestino = System.IO.Path.Combine(destino, System.IO.Path.GetFileName(archivoPath));
                System.IO.File.Copy(archivoPath, archivoDestino, true);
            }

            // Buscamos subcarpetas y nos llamamos a nosotros mismos (Recursividad)
            foreach (string carpetaPath in System.IO.Directory.GetDirectories(origen))
            {
                string carpetaDestino = System.IO.Path.Combine(destino, System.IO.Path.GetFileName(carpetaPath));
                CopiarCarpetaRecursiva(carpetaPath, carpetaDestino);
            }
        }

        // Este es el método principal de esta fase
        private void CargarDirectorio(string ruta)
        {
            try
            {
                // Limpiamos la lista antes de cargar nuevos datos
                listViewArchivos.Items.Clear();
                DirectoryInfo directorioActual = new DirectoryInfo(ruta);

                // 1. Primero leemos y agregamos las subcarpetas
                foreach (DirectoryInfo dir in directorioActual.GetDirectories())
                {
                    ListViewItem item = new ListViewItem(dir.Name, 0); // ¡El 0 asigna el icono!
                    item.SubItems.Add("Carpeta");
                    item.SubItems.Add(dir.FullName);
                    listViewArchivos.Items.Add(item);
                }

                // 2. Después leemos y agregamos los archivos
                foreach (FileInfo archivo in directorioActual.GetFiles())
                {
                    // Llamamos a la función para saber qué icono le toca
                    int indiceIcono = ObtenerIndiceIcono(archivo.Extension);

                    ListViewItem item = new ListViewItem(archivo.Name, indiceIcono); // Asignamos el icono
                    item.SubItems.Add(archivo.Extension);
                    item.SubItems.Add(archivo.FullName);
                    listViewArchivos.Items.Add(item);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Windows tiene carpetas protegidas, esto evita que el programa se congele o cierre
                MessageBox.Show("No tienes permiso para acceder a esta carpeta.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento del botón para buscar la ruta escrita en el TextBox
        private void btnIr_Click(object sender, EventArgs e)
        {
            CargarDirectorio(txtRuta.Text);
        }

        private void listViewArchivos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listViewArchivos_DoubleClick(object sender, EventArgs e)
        {
            // Verificamos que realmente haya un elemento seleccionado
            if (listViewArchivos.SelectedItems.Count > 0)
            {
                // Obtenemos el elemento al que le dimos doble clic
                ListViewItem itemSeleccionado = listViewArchivos.SelectedItems[0];

                // Guardamos el tipo y la ruta para que el código sea más limpio
                string tipo = itemSeleccionado.SubItems[1].Text;
                string ruta = itemSeleccionado.SubItems[2].Text;

                if (tipo == "Carpeta")
                {
                    // --- LÓGICA PARA CARPETAS ---
                    txtRuta.Text = ruta;
                    CargarDirectorio(ruta);
                }
                else
                {
                    // --- LÓGICA PARA ARCHIVOS ---

                    // 1. Obtenemos la extensión del archivo y la pasamos a minúsculas (.csv, .txt, etc.)
                    string extension = System.IO.Path.GetExtension(ruta).ToLower();

                    // 2. Evaluamos la extensión con un switch
                    switch (extension)
                    {
                        case ".csv":
                            CSVProcessor.Form1 visorCsv = new CSVProcessor.Form1(ruta);
                            visorCsv.Show();
                            break;

                        case ".txt":
                            TxtProcessor.MainForm visorTxt = new TxtProcessor.MainForm(ruta);
                            visorTxt.Show();
                            break;

                        case ".json":
                            Jsonprocessor.MainForm visorJson = new Jsonprocessor.MainForm(ruta);
                            visorJson.Show();
                            break;

                        // --- NUEVO CASO PARA XML ---
                        case ".xml":
                            // Llamamos explícitamente a través del namespace del procesador XML
                            XMLProcessor.Form1 visorXml = new XMLProcessor.Form1(ruta);

                            // Se muestra como ventana flotante por encima de la lista de archivos
                            visorXml.Show();
                            break;

                        // --- CASOS PARA IMÁGENES ---
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                        case ".gif":
                            FormVisorImagenes visorImg = new FormVisorImagenes(ruta);
                            visorImg.Show(); // Lo abrimos como ventana independiente flotante
                            break;
                        case ".mp3":
                        case ".wav":
                            FormReproductor reproductor = new FormReproductor(ruta);
                            reproductor.Show();
                            break;

                        default:
                            try
                            {
                                System.Diagnostics.ProcessStartInfo infoApertura = new System.Diagnostics.ProcessStartInfo();
                                infoApertura.FileName = ruta;
                                infoApertura.UseShellExecute = true;
                                System.Diagnostics.Process.Start(infoApertura);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("No se pudo abrir el archivo: " + ex.Message, "Error al abrir", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                    }
                }
            }
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            try
            {
                // Tomamos la ruta actual del TextBox
                DirectoryInfo directorioActual = new DirectoryInfo(txtRuta.Text);

                // Verificamos que tenga un directorio padre (si estamos en C:\, no hay padre)
                if (directorioActual.Parent != null)
                {
                    string rutaPadre = directorioActual.Parent.FullName;

                    // Actualizamos el TextBox y cargamos la ruta padre
                    txtRuta.Text = rutaPadre;
                    CargarDirectorio(rutaPadre);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo retroceder: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int ObtenerIndiceIcono(string extension)
        {
            // Convertimos la extensión a minúsculas para evitar errores (ej. .TXT y .txt)
            switch (extension.ToLower())
            {
                case ".txt":
                    return 1; // Índice del icono de texto
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    return 2; // Índice del icono de imagen
                case ".mp3":
                case ".wav":
                    return 3; // Índice del icono de audio
                case ".mp4":
                case ".avi":
                case ".mkv":
                    return 4; // Índice del icono de video
                default:
                    return 5; // Índice del archivo genérico para extensiones desconocidas
            }
        }

        private void cmbVistas_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Evaluamos el texto de la opción seleccionada y cambiamos la vista del ListView
            switch (cmbVistas.SelectedItem.ToString())
            {
                case "Detalles":
                    listViewArchivos.View = View.Details;
                    break;
                case "Iconos Grandes":
                    listViewArchivos.View = View.LargeIcon;
                    break;
                case "Iconos Pequeños":
                    listViewArchivos.View = View.SmallIcon;
                    break;
                case "Lista":
                    listViewArchivos.View = View.List;
                    break;
            }
        }
        // Método que se llama a sí mismo para profundizar en las carpetas
        private void ContarElementosRecursivo(string rutaActual, ref int contadorCarpetas, ref int contadorTextos)
        {
            try
            {
                DirectoryInfo dirActual = new DirectoryInfo(rutaActual);

                // 1. Primero, buscamos y contamos los archivos de texto (.txt) en la carpeta actual
                foreach (FileInfo archivo in dirActual.GetFiles())
                {
                    if (archivo.Extension.ToLower() == ".txt")
                    {
                        contadorTextos++;
                    }
                }

                // 2. Luego, obtenemos las subcarpetas
                foreach (DirectoryInfo subcarpeta in dirActual.GetDirectories())
                {
                    contadorCarpetas++; // Contamos esta subcarpeta que acabamos de encontrar

                    // ¡AQUÍ ESTÁ LA RECURSIVIDAD!
                    // El método se llama a sí mismo pasándole la ruta de la nueva subcarpeta
                    ContarElementosRecursivo(subcarpeta.FullName, ref contadorCarpetas, ref contadorTextos);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // En una búsqueda profunda es 100% seguro que chocaremos con carpetas ocultas de Windows.
                // Este catch vacío es vital para que el programa simplemente ignore esas carpetas y siga contando.
            }
            catch (Exception)
            {
                // Ignoramos otros errores de lectura para no interrumpir el proceso masivo
            }
        }

        private void btnContar_Click(object sender, EventArgs e)
        {
            // Inicializamos nuestros contadores en cero
            int totalCarpetas = 0;
            int totalTextos = 0;

            // Tomamos la ruta en la que estamos parados actualmente en el explorador
            string rutaBase = txtRuta.Text;

            // Cambiamos el cursor a un reloj de arena, porque si buscas en todo el disco C: puede tardar unos segundos
            Cursor.Current = Cursors.WaitCursor;

            // Llamamos a nuestro método recursivo por primera vez
            ContarElementosRecursivo(rutaBase, ref totalCarpetas, ref totalTextos);

            // Regresamos el cursor a la normalidad
            Cursor.Current = Cursors.Default;

            // Mostramos los resultados al profesor
            MessageBox.Show($"Exploración terminada.\n\nTotal de subcarpetas: {totalCarpetas}\nTotal de archivos de texto (.txt): {totalTextos}",
                            "Resultado de Recursividad",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void treeNavegacion_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Verificamos que el nodo tenga una ruta guardada en su Tag
            if (e.Node.Tag != null)
            {
                // Extraemos la ruta
                string rutaSeleccionada = e.Node.Tag.ToString();

                // Actualizamos tu barra de direcciones superior
                txtRuta.Text = rutaSeleccionada;

                // Llamamos a la función que ya construiste para leer y mostrar los archivos
                CargarDirectorio(rutaSeleccionada);
            }
        }
    }
}