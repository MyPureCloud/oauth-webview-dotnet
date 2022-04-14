namespace GenesysCloudOAuthWebView.WinForms
{
    partial class OAuthWebViewForm
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
            GenesysCloudOAuthWebView.Core.OAuthConfig oAuthConfig2 = new GenesysCloudOAuthWebView.Core.OAuthConfig();
            this.oAuthWebView = new GenesysCloudOAuthWebView.WinForms.OAuthWebView();
            ((System.ComponentModel.ISupportInitialize)(this.oAuthWebView)).BeginInit();
            this.SuspendLayout();
            // 
            // oAuthWebView
            // 
            oAuthConfig2.ClientId = null;
            oAuthConfig2.Environment = "mypurecloud.com";
            oAuthConfig2.ForceLoginPrompt = false;
            oAuthConfig2.Org = "";
            oAuthConfig2.Provider = "";
            oAuthConfig2.RedirectUri = null;
            oAuthConfig2.RedirectUriIsFake = false;
            oAuthConfig2.State = "";
            this.oAuthWebView.Config = oAuthConfig2;
            this.oAuthWebView.CreationProperties = null;
            this.oAuthWebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.oAuthWebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.oAuthWebView.Location = new System.Drawing.Point(0, 0);
            this.oAuthWebView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.oAuthWebView.MinimumSize = new System.Drawing.Size(23, 23);
            this.oAuthWebView.Name = "oAuthWebView";
            this.oAuthWebView.Size = new System.Drawing.Size(623, 647);
            this.oAuthWebView.TabIndex = 0;
            this.oAuthWebView.ZoomFactor = 1D;
            // 
            // OAuthWebViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 647);
            this.Controls.Add(this.oAuthWebView);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "OAuthWebViewForm";
            this.Text = "Genesys Cloud Login";
            ((System.ComponentModel.ISupportInitialize)(this.oAuthWebView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public OAuthWebView oAuthWebView;
    }
}