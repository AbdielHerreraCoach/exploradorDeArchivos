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
        // SOLO declaramos el Label por código (¡Borramos el PictureBox porque ya existe en tu diseñador!)
        private Label lblCoordenadas;

        // 1. Constructor por defecto
        public FormVisorImagenes()
        {
            InitializeComponent();

            // Inicializamos el Label aquí vacío para que C# 10 no se queje de los valores nulos
            lblCoordenadas = new Label();
        }

        // 2. Constructor que usamos desde el explorador
        public FormVisorImagenes(string ruta)
        {
            InitializeComponent();

            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Configuramos la etiqueta de texto para el GPS
            lblCoordenadas = new Label();
            lblCoordenadas.Dock = DockStyle.Bottom;
            lblCoordenadas.Height = 30;
            lblCoordenadas.TextAlign = ContentAlignment.MiddleCenter;
            lblCoordenadas.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Agregamos SOLO el Label a la ventana (El PictureBox ya lo trae el InitializeComponent)
            this.Controls.Add(lblCoordenadas);

            // Cargamos la imagen y los datos
            this.Load += (s, e) => CargarImagenYMetadatos(ruta);
        }

        public void CargarImagenYMetadatos(string ruta)
        {
            try
            {
                this.Text = $"Visor de Imágenes — {Path.GetFileName(ruta)}";

                // Usamos directamente tu pictureBox1 del diseñador
                pictureBox1.Image = Image.FromFile(ruta);

                // Buscamos las coordenadas
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

        // Método mágico de MetadataExtractor ajustado a la sintaxis nativa de C#
        private string ExtraerGPS(string ruta)
        {
            try
            {
                var directorios = ImageMetadataReader.ReadMetadata(ruta);
                var gpsDirectorio = directorios.OfType<GpsDirectory>().FirstOrDefault();

                if (gpsDirectorio != null)
                {
                    // En C#, GetGeoLocation devuelve un 'GeoLocation?' (un objeto que puede ser nulo)
                    var localizacion = gpsDirectorio.GetGeoLocation();

                    // En lugar de IsZero, simplemente verificamos que la variable no sea nula
                    if (localizacion != null)
                    {
                        // Para extraer la latitud y longitud, le agregamos el ".Value"
                        return $"{localizacion.Value.Latitude:F5}, {localizacion.Value.Longitude:F5}";
                    }
                }
            }
            catch (Exception)
            {
                // Si la imagen no tiene EXIF o está corrupta, ignoramos el error
            }

            return string.Empty;
        }
    }
}