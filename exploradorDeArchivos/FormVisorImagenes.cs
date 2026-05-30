using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace exploradorDeArchivos
{
    public partial class FormVisorImagenes : Form
    {
        // Controles adicionales creados por código
        private Label lblCoordenadas;
        private Panel panelHerramientas;
        private Panel panelVista;

        public FormVisorImagenes()
        {
            InitializeComponent();
            lblCoordenadas = new Label();
        }

        public FormVisorImagenes(string ruta)
        {
            InitializeComponent();

            this.Size = new Size(850, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Construimos la nueva interfaz antes de cargar la foto
            ConfigurarInterfazExtendida();

            this.Load += (s, e) => CargarImagenYMetadatos(ruta);
        }

        private void ConfigurarInterfazExtendida()
        {
            // 1. Barra superior de herramientas
            panelHerramientas = new Panel();
            panelHerramientas.Dock = DockStyle.Top;
            panelHerramientas.Height = 45;
            panelHerramientas.BackColor = Color.FromArgb(240, 240, 240); // Gris claro

            Button btnRotar = new Button() { Text = "↻ Rotar 90°", Location = new Point(10, 8), Size = new Size(100, 30) };
            Button btnAcercar = new Button() { Text = "🔍 Acercar (+)", Location = new Point(120, 8), Size = new Size(100, 30) };
            Button btnAlejar = new Button() { Text = "🔎 Alejar (-)", Location = new Point(230, 8), Size = new Size(100, 30) };
            Button btnAjustar = new Button() { Text = "🔳 Ajustar", Location = new Point(340, 8), Size = new Size(100, 30) };

            // Conectamos los eventos a los botones
            btnRotar.Click += BtnRotar_Click;
            btnAcercar.Click += (s, e) => AplicarZoom(1.25); // Aumenta 25%
            btnAlejar.Click += (s, e) => AplicarZoom(0.80);  // Reduce 20%
            btnAjustar.Click += BtnAjustar_Click;

            panelHerramientas.Controls.AddRange(new Control[] { btnRotar, btnAcercar, btnAlejar, btnAjustar });

            // 2. Etiqueta GPS inferior
            lblCoordenadas = new Label();
            lblCoordenadas.Dock = DockStyle.Bottom;
            lblCoordenadas.Height = 30;
            lblCoordenadas.TextAlign = ContentAlignment.MiddleCenter;
            lblCoordenadas.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // 3. Panel central con Scroll Automático (La magia del zoom)
            panelVista = new Panel();
            panelVista.Dock = DockStyle.Fill;
            panelVista.AutoScroll = true;
            panelVista.BackColor = Color.FromArgb(30, 30, 30); // Fondo oscuro estilo Photoshop

            // 4. Adaptamos tu PictureBox original del diseñador
            this.Controls.Remove(pictureBox1); // Lo quitamos temporalmente del fondo
            pictureBox1.Dock = DockStyle.None; // Le quitamos el Fill para poder cambiar su tamaño
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            panelVista.Controls.Add(pictureBox1); // Lo metemos a nuestro panel con scroll

            // 5. Agregamos las 3 piezas al formulario
            this.Controls.Add(panelVista);
            this.Controls.Add(panelHerramientas);
            this.Controls.Add(lblCoordenadas);

            // Evento para que, si el usuario maximiza la ventana, la foto se centre sola
            this.Resize += (s, e) => CentrarImagen();
        }

        public void CargarImagenYMetadatos(string ruta)
        {
            try
            {
                this.Text = $"Visor de Imágenes — {Path.GetFileName(ruta)}";

                // Cargamos la foto
                pictureBox1.Image = Image.FromFile(ruta);

                // La ajustamos al tamaño de la pantalla inicialmente
                BtnAjustar_Click(null, null);

                // --- Lógica del GPS ---
                string coordenadas = ExtraerGPS(ruta);

                if (!string.IsNullOrEmpty(coordenadas))
                {
                    lblCoordenadas.Text = $"📍 Ubicación GPS: {coordenadas}";
                    lblCoordenadas.ForeColor = Color.DarkBlue;
                }
                else
                {
                    lblCoordenadas.Text = "Esta imagen no contiene datos geográficos (GPS).";
                    lblCoordenadas.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la imagen: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── FUNCIONES DE HERRAMIENTAS (ZOOM Y ROTACIÓN) ─────────────────────────

        private void BtnRotar_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // C# tiene rotación nativa integrada en la clase Image
                pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox1.Refresh();

                // Reajustamos las proporciones tras girar
                BtnAjustar_Click(null, null);
            }
        }

        private void AplicarZoom(double multiplicador)
        {
            if (pictureBox1.Image == null) return;

            // Calculamos el nuevo tamaño
            int nuevoAncho = (int)(pictureBox1.Width * multiplicador);
            int nuevoAlto = (int)(pictureBox1.Height * multiplicador);

            // Le ponemos un límite para que no se haga microscópica ni colapse la memoria RAM
            if (nuevoAncho < 100 || nuevoAncho > 15000) return;

            pictureBox1.Width = nuevoAncho;
            pictureBox1.Height = nuevoAlto;

            CentrarImagen();
        }

        private void BtnAjustar_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;

            // Hacemos que el PictureBox sea casi del tamaño exacto del panel para que se vea completa
            pictureBox1.Size = new Size(panelVista.Width - 10, panelVista.Height - 10);
            CentrarImagen();
        }

        private void CentrarImagen()
        {
            // Matemáticas simples para que la foto siempre esté en el medio del panel gris oscuro
            int x = (panelVista.Width > pictureBox1.Width) ? (panelVista.Width - pictureBox1.Width) / 2 : 0;
            int y = (panelVista.Height > pictureBox1.Height) ? (panelVista.Height - pictureBox1.Height) / 2 : 0;

            pictureBox1.Location = new Point(x, y);
        }

        // ─── EXTRACCIÓN DE GPS ───────────────────────────────────────────────────

        private string ExtraerGPS(string ruta)
        {
            try
            {
                var directorios = ImageMetadataReader.ReadMetadata(ruta);
                var gpsDirectorio = directorios.OfType<GpsDirectory>().FirstOrDefault();

                if (gpsDirectorio != null)
                {
                    var localizacion = gpsDirectorio.GetGeoLocation();

                    if (localizacion != null)
                    {
                        return $"{localizacion.Value.Latitude:F5}, {localizacion.Value.Longitude:F5}";
                    }
                }
            }
            catch (Exception) { }

            return string.Empty;
        }
    }
}