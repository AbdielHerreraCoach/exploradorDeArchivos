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
        }

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