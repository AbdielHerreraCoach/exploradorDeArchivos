namespace exploradorDeArchivos
{
    partial class FormReproductor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReproductor));
            reproductorWMP = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)reproductorWMP).BeginInit();
            SuspendLayout();
            // 
            // reproductorWMP
            // 
            reproductorWMP.Enabled = true;
            reproductorWMP.Location = new Point(51, 116);
            reproductorWMP.Name = "reproductorWMP";
            reproductorWMP.OcxState = (AxHost.State)resources.GetObject("reproductorWMP.OcxState");
            reproductorWMP.Size = new Size(271, 170);
            reproductorWMP.TabIndex = 0;
            reproductorWMP.Visible = false;
            // 
            // FormReproductor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(384, 511);
            Controls.Add(reproductorWMP);
            Name = "FormReproductor";
            Text = "FormReproductor";
            ((System.ComponentModel.ISupportInitialize)reproductorWMP).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer reproductorWMP;
    }
}