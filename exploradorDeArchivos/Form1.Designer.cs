namespace exploradorDeArchivos
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            txtRuta = new TextBox();
            btnIr = new Button();
            listViewArchivos = new ListView();
            Nombre = new ColumnHeader();
            Tipo = new ColumnHeader();
            Ruta = new ColumnHeader();
            imgListGrandes = new ImageList(components);
            imgListPequeños = new ImageList(components);
            btnAtras = new Button();
            cmbVistas = new ComboBox();
            btnContar = new Button();
            treeNavegacion = new TreeView();
            imgNavegacion = new ImageList(components);
            SuspendLayout();
            // 
            // txtRuta
            // 
            txtRuta.Location = new Point(196, 12);
            txtRuta.Name = "txtRuta";
            txtRuta.Size = new Size(245, 23);
            txtRuta.TabIndex = 0;
            // 
            // btnIr
            // 
            btnIr.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnIr.Location = new Point(447, 10);
            btnIr.Name = "btnIr";
            btnIr.Size = new Size(32, 26);
            btnIr.TabIndex = 1;
            btnIr.Text = "🔍";
            btnIr.UseVisualStyleBackColor = true;
            btnIr.Click += btnIr_Click;
            // 
            // listViewArchivos
            // 
            listViewArchivos.Columns.AddRange(new ColumnHeader[] { Nombre, Tipo, Ruta });
            listViewArchivos.LargeImageList = imgListGrandes;
            listViewArchivos.Location = new Point(139, 41);
            listViewArchivos.Name = "listViewArchivos";
            listViewArchivos.Size = new Size(649, 397);
            listViewArchivos.SmallImageList = imgListPequeños;
            listViewArchivos.TabIndex = 2;
            listViewArchivos.UseCompatibleStateImageBehavior = false;
            listViewArchivos.View = View.Details;
            listViewArchivos.SelectedIndexChanged += listViewArchivos_SelectedIndexChanged;
            listViewArchivos.DoubleClick += listViewArchivos_DoubleClick;
            // 
            // Nombre
            // 
            Nombre.Tag = "";
            Nombre.Text = "Nombre";
            Nombre.Width = 120;
            // 
            // Tipo
            // 
            Tipo.Tag = "";
            Tipo.Text = "Tipo";
            Tipo.Width = 120;
            // 
            // Ruta
            // 
            Ruta.Tag = "";
            Ruta.Text = "Ruta";
            Ruta.Width = 120;
            // 
            // imgListGrandes
            // 
            imgListGrandes.ColorDepth = ColorDepth.Depth32Bit;
            imgListGrandes.ImageStream = (ImageListStreamer)resources.GetObject("imgListGrandes.ImageStream");
            imgListGrandes.TransparentColor = Color.Transparent;
            imgListGrandes.Images.SetKeyName(0, "Carpeta.png");
            imgListGrandes.Images.SetKeyName(1, "Texto.png");
            imgListGrandes.Images.SetKeyName(2, "Paisaje.png");
            imgListGrandes.Images.SetKeyName(3, "Musica.png");
            imgListGrandes.Images.SetKeyName(4, "Video.png");
            imgListGrandes.Images.SetKeyName(5, "Generico.png");
            // 
            // imgListPequeños
            // 
            imgListPequeños.ColorDepth = ColorDepth.Depth32Bit;
            imgListPequeños.ImageStream = (ImageListStreamer)resources.GetObject("imgListPequeños.ImageStream");
            imgListPequeños.TransparentColor = Color.Transparent;
            imgListPequeños.Images.SetKeyName(0, "Carpeta");
            imgListPequeños.Images.SetKeyName(1, "Archivo de texto");
            imgListPequeños.Images.SetKeyName(2, "Imagen");
            imgListPequeños.Images.SetKeyName(3, "Audio");
            imgListPequeños.Images.SetKeyName(4, "Video");
            imgListPequeños.Images.SetKeyName(5, "Archivo generico");
            // 
            // btnAtras
            // 
            btnAtras.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnAtras.Location = new Point(151, 11);
            btnAtras.Name = "btnAtras";
            btnAtras.Size = new Size(39, 23);
            btnAtras.TabIndex = 3;
            btnAtras.Text = "🠔";
            btnAtras.UseVisualStyleBackColor = true;
            btnAtras.Click += btnAtras_Click;
            // 
            // cmbVistas
            // 
            cmbVistas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVistas.FormattingEnabled = true;
            cmbVistas.Items.AddRange(new object[] { "Detalles", "Iconos Grandes", "Iconos Pequeños", "Lista" });
            cmbVistas.Location = new Point(528, 11);
            cmbVistas.Name = "cmbVistas";
            cmbVistas.Size = new Size(121, 23);
            cmbVistas.TabIndex = 4;
            cmbVistas.SelectedIndexChanged += cmbVistas_SelectedIndexChanged;
            // 
            // btnContar
            // 
            btnContar.Location = new Point(655, 10);
            btnContar.Name = "btnContar";
            btnContar.Size = new Size(75, 23);
            btnContar.TabIndex = 5;
            btnContar.Text = "Contar";
            btnContar.UseVisualStyleBackColor = true;
            btnContar.Click += btnContar_Click;
            // 
            // treeNavegacion
            // 
            treeNavegacion.BorderStyle = BorderStyle.None;
            treeNavegacion.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeNavegacion.ItemHeight = 30;
            treeNavegacion.Location = new Point(12, 41);
            treeNavegacion.Name = "treeNavegacion";
            treeNavegacion.ShowLines = false;
            treeNavegacion.Size = new Size(121, 397);
            treeNavegacion.TabIndex = 7;
            treeNavegacion.AfterSelect += treeNavegacion_AfterSelect;
            // 
            // imgNavegacion
            // 
            imgNavegacion.ColorDepth = ColorDepth.Depth32Bit;
            imgNavegacion.ImageStream = (ImageListStreamer)resources.GetObject("imgNavegacion.ImageStream");
            imgNavegacion.TransparentColor = Color.Transparent;
            imgNavegacion.Images.SetKeyName(0, "pantala.png");
            imgNavegacion.Images.SetKeyName(1, "45162.png");
            imgNavegacion.Images.SetKeyName(2, "canva-document-icon-MAB_htUUeMk.png");
            imgNavegacion.Images.SetKeyName(3, "image_icon_153794.png");
            imgNavegacion.Images.SetKeyName(4, "canva-musical-note-MAD38rw6Ees.png");
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(treeNavegacion);
            Controls.Add(btnContar);
            Controls.Add(cmbVistas);
            Controls.Add(btnAtras);
            Controls.Add(listViewArchivos);
            Controls.Add(btnIr);
            Controls.Add(txtRuta);
            IsMdiContainer = true;
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtRuta;
        private Button btnIr;
        private ListView listViewArchivos;
        private ColumnHeader Nombre;
        private ColumnHeader Tipo;
        private ColumnHeader Ruta;
        private Button btnAtras;
        private ImageList imgListPequeños;
        private ComboBox cmbVistas;
        private ImageList imgListGrandes;
        private Button btnContar;
        private TreeView treeNavegacion;
        private ImageList imgNavegacion;
    }
}
