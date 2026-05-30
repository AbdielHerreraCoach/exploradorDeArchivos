using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib; // Referencia del motor de Windows

namespace exploradorDeArchivos
{
    public partial class FormReproductor : Form
    {
        // Controles de tu interfaz
        private PictureBox picPortada;
        private Label lblTitulo;
        private Label lblTiempo;
        private TrackBar trackTiempo;
        private Button btnAnterior, btnPlayPausa, btnSiguiente;
        private System.Windows.Forms.Timer timerUI;

        // Lógica de lista de reproducción
        private List<string> playlist = new List<string>();
        private int indiceActual = 0;
        private bool arrastrandoBarra = false;

        public FormReproductor()
        {
            InitializeComponent();
        }

        public FormReproductor(string rutaInicial)
        {
            InitializeComponent();
            this.Size = new Size(380, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Text = "Reproductor de Música";

            ConfigurarInterfaz();
            CargarPlaylist(rutaInicial);

            // Apagamos la música si el usuario cierra la ventana
            this.FormClosing += (s, e) => reproductorWMP.Ctlcontrols.stop();
        }

        private void ConfigurarInterfaz()
        {
            // 1. Portada del Disco
            picPortada = new PictureBox()
            {
                Size = new Size(250, 250),
                Location = new Point(55, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            // 2. Título de la canción
            lblTitulo = new Label()
            {
                Location = new Point(20, 290),
                Size = new Size(320, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            // 3. Barra de navegación de tiempo (TrackBar)
            trackTiempo = new TrackBar()
            {
                Location = new Point(30, 330),
                Size = new Size(300, 45),
                TickStyle = TickStyle.None
            };
            trackTiempo.MouseDown += (s, e) => arrastrandoBarra = true;
            trackTiempo.MouseUp += TrackTiempo_MouseUp;

            // 4. Etiqueta de tiempo (Ej: 01:20 / 03:45)
            lblTiempo = new Label()
            {
                Location = new Point(30, 365),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 8, FontStyle.Regular)
            };

            // 5. Botones de control
            btnAnterior = CrearBoton("⏮ Anterior", 60, 400);
            btnPlayPausa = CrearBoton("⏸ Pausa", 140, 400);
            btnSiguiente = CrearBoton("⏭ Sig.", 220, 400);

            btnAnterior.Click += (s, e) => CambiarCancion(-1);
            btnSiguiente.Click += (s, e) => CambiarCancion(1);
            btnPlayPausa.Click += BtnPlayPausa_Click;

            // 6. Timer para actualizar la barra y el tiempo cada medio segundo
            timerUI = new System.Windows.Forms.Timer() { Interval = 500 };
            timerUI.Tick += TimerUI_Tick;

            // Ensamblamos todo
            // Apagamos el timer y la música si el usuario cierra la ventana
            this.FormClosing += (s, e) =>
            {
                if (timerUI != null)
                    timerUI.Stop(); // ¡Esta es la línea que salva el día!

                if (reproductorWMP != null)
                    reproductorWMP.Ctlcontrols.stop();
            };
            this.Controls.Add(picPortada);
            this.Controls.Add(lblTitulo);
            this.Controls.Add(trackTiempo);
            this.Controls.Add(lblTiempo);
            this.Controls.Add(btnAnterior);
            this.Controls.Add(btnPlayPausa);
            this.Controls.Add(btnSiguiente);
        }

        private Button CrearBoton(string texto, int x, int y)
        {
            return new Button()
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(80, 40),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        // ─── LÓGICA DEL REPRODUCTOR ─────────────────────────────────────────────

        private void CargarPlaylist(string rutaInicial)
        {
            string directorio = Path.GetDirectoryName(rutaInicial);

            // Buscamos todas las canciones en esa misma carpeta para los botones Sig/Ant
            playlist = Directory.GetFiles(directorio)
                                .Where(f => f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                                            f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                                .ToList();

            indiceActual = playlist.IndexOf(rutaInicial);
            ReproducirCancionActual();
        }

        private void ReproducirCancionActual()
        {
            if (indiceActual < 0 || indiceActual >= playlist.Count) return;

            string ruta = playlist[indiceActual];

            // Le damos la ruta al motor invisible y damos play
            reproductorWMP.URL = ruta;
            reproductorWMP.Ctlcontrols.play();

            ExtraerMetadatosYPortada(ruta);

            btnPlayPausa.Text = "⏸ Pausa";
            timerUI.Start();
        }

        private void CambiarCancion(int direccion)
        {
            indiceActual += direccion;

            // Lógica para que la lista sea circular (si llega al final, vuelve a la primera)
            if (indiceActual < 0) indiceActual = playlist.Count - 1;
            if (indiceActual >= playlist.Count) indiceActual = 0;

            ReproducirCancionActual();
        }

        private void BtnPlayPausa_Click(object sender, EventArgs e)
        {
            // Verificamos el estado actual del motor invisible
            if (reproductorWMP.playState == WMPPlayState.wmppsPlaying)
            {
                reproductorWMP.Ctlcontrols.pause();
                btnPlayPausa.Text = "▶ Play";
            }
            else
            {
                reproductorWMP.Ctlcontrols.play();
                btnPlayPausa.Text = "⏸ Pausa";
            }
        }

        // ─── LÓGICA DE INTERFAZ Y BARRA DE TIEMPO ───────────────────────────────

        private void TimerUI_Tick(object sender, EventArgs e)
        {
            // Seguro 1: Si la ventana ya se está cerrando, abortamos
            if (this.IsDisposed || reproductorWMP == null) return;

            try
            {
                // Ahora sí, hacemos la validación normal
                if (reproductorWMP.playState == WMPPlayState.wmppsPlaying && !arrastrandoBarra)
                {
                    if (trackTiempo.Maximum != (int)reproductorWMP.currentMedia.duration)
                        trackTiempo.Maximum = (int)reproductorWMP.currentMedia.duration;

                    trackTiempo.Value = (int)reproductorWMP.Ctlcontrols.currentPosition;

                    lblTiempo.Text = $"{reproductorWMP.Ctlcontrols.currentPositionString} / {reproductorWMP.currentMedia.durationString}";
                }
            }
            catch (Exception)
            {
                // Seguro 2: Si el motor de Windows falla al cerrarse, lo ignoramos para que el programa no explote
                timerUI.Stop();
            }
        }

        private void TrackTiempo_MouseUp(object sender, MouseEventArgs e)
        {
            // Cuando el usuario suelta la barra, adelantamos la canción a ese punto
            reproductorWMP.Ctlcontrols.currentPosition = trackTiempo.Value;
            arrastrandoBarra = false;
        }

        // ─── MAGIA DE TAGLIB: EXTRAER PORTADA ───────────────────────────────────

        private void ExtraerMetadatosYPortada(string ruta)
        {
            lblTitulo.Text = Path.GetFileNameWithoutExtension(ruta);
            picPortada.Image = null; // Limpiamos la portada anterior

            try
            {
                // Usamos la librería TagLib para leer el MP3
                var archivoAudio = TagLib.File.Create(ruta);

                // Si tiene título en las etiquetas, lo usamos, si no, dejamos el nombre del archivo
                if (!string.IsNullOrEmpty(archivoAudio.Tag.Title))
                    lblTitulo.Text = $"{archivoAudio.Tag.Title} - {archivoAudio.Tag.FirstPerformer}";

                // Extraemos la imagen incrustada (Cover Art)
                if (archivoAudio.Tag.Pictures.Length > 0)
                {
                    var bytesImagen = (byte[])(archivoAudio.Tag.Pictures[0].Data.Data);
                    using (MemoryStream ms = new MemoryStream(bytesImagen))
                    {
                        picPortada.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    // Opcional: Si no hay portada, puedes cargar una imagen local por defecto
                    // picPortada.Image = Image.FromFile("C:\\ruta\\a\\disco_vinilo.png");
                }
            }
            catch (Exception)
            {
                // Si el MP3 no tiene etiquetas o hay un error, simplemente no muestra imagen, no pasa nada
            }
        }
    }
}