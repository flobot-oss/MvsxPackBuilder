using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MvsxPackBuilder
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node1");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.FbaSupportedPlatformComboBox = new System.Windows.Forms.ComboBox();
            this.fba_treeView1 = new System.Windows.Forms.TreeView();
            this.FbaContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.FBA_AddRomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HylostickGameIniComboBox = new System.Windows.Forms.ComboBox();
            this.Hylo_treeView2 = new System.Windows.Forms.TreeView();
            this.GameIniContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Hylo_RemoveRomMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RemoveCategoryButton = new System.Windows.Forms.Button();
            this.AddCategoryButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMVSXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFbaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.ExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.coverArtButton1 = new System.Windows.Forms.Button();
            this.FbaContextMenuStrip1.SuspendLayout();
            this.GameIniContextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FbaSupportedPlatformComboBox
            // 
            this.FbaSupportedPlatformComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.FbaSupportedPlatformComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FbaSupportedPlatformComboBox.FormattingEnabled = true;
            this.FbaSupportedPlatformComboBox.Location = new System.Drawing.Point(3, 19);
            this.FbaSupportedPlatformComboBox.Name = "FbaSupportedPlatformComboBox";
            this.FbaSupportedPlatformComboBox.Size = new System.Drawing.Size(508, 23);
            this.FbaSupportedPlatformComboBox.TabIndex = 0;
            this.FbaSupportedPlatformComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // fba_treeView1
            // 
            this.fba_treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fba_treeView1.ContextMenuStrip = this.FbaContextMenuStrip1;
            this.fba_treeView1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.fba_treeView1.Location = new System.Drawing.Point(3, 48);
            this.fba_treeView1.Name = "fba_treeView1";
            treeNode1.Checked = true;
            treeNode1.Name = "Node1";
            treeNode1.Text = "Node1";
            this.fba_treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.fba_treeView1.ShowNodeToolTips = true;
            this.fba_treeView1.Size = new System.Drawing.Size(506, 714);
            this.fba_treeView1.TabIndex = 1;
            this.fba_treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // FbaContextMenuStrip1
            // 
            this.FbaContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FBA_AddRomMenuItem});
            this.FbaContextMenuStrip1.Name = "FbaContextMenuStrip1";
            this.FbaContextMenuStrip1.Size = new System.Drawing.Size(97, 26);
            // 
            // FBA_AddRomMenuItem
            // 
            this.FBA_AddRomMenuItem.Name = "FBA_AddRomMenuItem";
            this.FBA_AddRomMenuItem.Size = new System.Drawing.Size(96, 22);
            this.FBA_AddRomMenuItem.Text = "Add";
            this.FBA_AddRomMenuItem.Click += new System.EventHandler(this.FBA_AddRomMenuItem_Click);
            // 
            // HylostickGameIniComboBox
            // 
            this.HylostickGameIniComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HylostickGameIniComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HylostickGameIniComboBox.FormattingEnabled = true;
            this.HylostickGameIniComboBox.Location = new System.Drawing.Point(3, 19);
            this.HylostickGameIniComboBox.Name = "HylostickGameIniComboBox";
            this.HylostickGameIniComboBox.Size = new System.Drawing.Size(429, 23);
            this.HylostickGameIniComboBox.TabIndex = 2;
            this.HylostickGameIniComboBox.SelectedIndexChanged += new System.EventHandler(this.HylostickGameIni_SelectedIndexChanged);
            // 
            // Hylo_treeView2
            // 
            this.Hylo_treeView2.AllowDrop = true;
            this.Hylo_treeView2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Hylo_treeView2.ContextMenuStrip = this.GameIniContextMenuStrip1;
            this.Hylo_treeView2.Location = new System.Drawing.Point(4, 48);
            this.Hylo_treeView2.Name = "Hylo_treeView2";
            this.Hylo_treeView2.Size = new System.Drawing.Size(506, 714);
            this.Hylo_treeView2.TabIndex = 1;
            this.Hylo_treeView2.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterSelect);
            // 
            // GameIniContextMenuStrip1
            // 
            this.GameIniContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Hylo_RemoveRomMenuItem});
            this.GameIniContextMenuStrip1.Name = "GameIniMenuStrip1";
            this.GameIniContextMenuStrip1.Size = new System.Drawing.Size(118, 26);
            // 
            // Hylo_RemoveRomMenuItem
            // 
            this.Hylo_RemoveRomMenuItem.Name = "Hylo_RemoveRomMenuItem";
            this.Hylo_RemoveRomMenuItem.Size = new System.Drawing.Size(117, 22);
            this.Hylo_RemoveRomMenuItem.Text = "Remove";
            this.Hylo_RemoveRomMenuItem.Click += new System.EventHandler(this.Hylo_RemoveRomMenuItem_Click);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(11, 534);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(274, 273);
            this.propertyGrid1.TabIndex = 1;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.FbaSupportedPlatformComboBox);
            this.groupBox1.Controls.Add(this.fba_treeView1);
            this.groupBox1.Location = new System.Drawing.Point(0, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 819);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Final Burn Alpha Romset";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox3.Location = new System.Drawing.Point(3, 768);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(508, 48);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search";
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(3, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(502, 23);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.RemoveCategoryButton);
            this.groupBox2.Controls.Add(this.AddCategoryButton);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.HylostickGameIniComboBox);
            this.groupBox2.Controls.Add(this.Hylo_treeView2);
            this.groupBox2.Location = new System.Drawing.Point(512, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(514, 819);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Hylostick Categories";
            // 
            // RemoveCategoryButton
            // 
            this.RemoveCategoryButton.Enabled = false;
            this.RemoveCategoryButton.Image = ((System.Drawing.Image)(resources.GetObject("RemoveCategoryButton.Image")));
            this.RemoveCategoryButton.Location = new System.Drawing.Point(476, 13);
            this.RemoveCategoryButton.Name = "RemoveCategoryButton";
            this.RemoveCategoryButton.Size = new System.Drawing.Size(32, 32);
            this.RemoveCategoryButton.TabIndex = 5;
            this.RemoveCategoryButton.UseVisualStyleBackColor = true;
            this.RemoveCategoryButton.Click += new System.EventHandler(this.RemoveCategoryButton_Click);
            // 
            // AddCategoryButton
            // 
            this.AddCategoryButton.Enabled = false;
            this.AddCategoryButton.Image = ((System.Drawing.Image)(resources.GetObject("AddCategoryButton.Image")));
            this.AddCategoryButton.Location = new System.Drawing.Point(438, 13);
            this.AddCategoryButton.Name = "AddCategoryButton";
            this.AddCategoryButton.Size = new System.Drawing.Size(32, 32);
            this.AddCategoryButton.TabIndex = 4;
            this.AddCategoryButton.UseVisualStyleBackColor = true;
            this.AddCategoryButton.Click += new System.EventHandler(this.AddCategoryButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox2);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox4.Location = new System.Drawing.Point(3, 768);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(508, 48);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Search";
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(3, 19);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(502, 23);
            this.textBox2.TabIndex = 0;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(4, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(286, 321);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1329, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMVSXToolStripMenuItem,
            this.openFbaToolStripMenuItem,
            this.toolStripSeparator,
            this.ExportToolStripMenuItem,
            this.ExportAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openMVSXToolStripMenuItem
            // 
            this.openMVSXToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openMVSXToolStripMenuItem.Image")));
            this.openMVSXToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openMVSXToolStripMenuItem.Name = "openMVSXToolStripMenuItem";
            this.openMVSXToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.openMVSXToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.openMVSXToolStripMenuItem.Text = "&Open MVSX Hack Folder";
            this.openMVSXToolStripMenuItem.Click += new System.EventHandler(this.openMVSXToolStripMenuItem_Click);
            // 
            // openFbaToolStripMenuItem
            // 
            this.openFbaToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openFbaToolStripMenuItem.Image")));
            this.openFbaToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFbaToolStripMenuItem.Name = "openFbaToolStripMenuItem";
            this.openFbaToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openFbaToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.openFbaToolStripMenuItem.Text = "&Set Fba Roms Folder";
            this.openFbaToolStripMenuItem.Click += new System.EventHandler(this.openFbaToolStripMenuItem_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(243, 6);
            // 
            // ExportToolStripMenuItem
            // 
            this.ExportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ExportToolStripMenuItem.Image")));
            this.ExportToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem";
            this.ExportToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.ExportToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.ExportToolStripMenuItem.Text = "&Export";
            this.ExportToolStripMenuItem.Click += new System.EventHandler(this.ExportToolStripMenuItem_Click);
            // 
            // ExportAsToolStripMenuItem
            // 
            this.ExportAsToolStripMenuItem.Name = "ExportAsToolStripMenuItem";
            this.ExportAsToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.ExportAsToolStripMenuItem.Text = "Export &As";
            this.ExportAsToolStripMenuItem.Click += new System.EventHandler(this.ExportAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(243, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(246, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.coverArtButton1);
            this.panel1.Controls.Add(this.propertyGrid1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1032, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(297, 819);
            this.panel1.TabIndex = 7;
            // 
            // coverArtButton1
            // 
            this.coverArtButton1.Location = new System.Drawing.Point(4, 330);
            this.coverArtButton1.Name = "coverArtButton1";
            this.coverArtButton1.Size = new System.Drawing.Size(286, 41);
            this.coverArtButton1.TabIndex = 7;
            this.coverArtButton1.Text = "Select Cover";
            this.coverArtButton1.UseVisualStyleBackColor = true;
            this.coverArtButton1.Click += new System.EventHandler(this.coverArtButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1329, 843);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(1345, 882);
            this.Name = "Form1";
            this.Text = "MVSX Pack Builder";
            this.FbaContextMenuStrip1.ResumeLayout(false);
            this.GameIniContextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TreeView fba_treeView1;
        private System.Windows.Forms.TreeView Hylo_treeView2;
        private System.Windows.Forms.ComboBox FbaSupportedPlatformComboBox;
        private System.Windows.Forms.ComboBox HylostickGameIniComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ContextMenuStrip GameIniContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Hylo_RemoveRomMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMVSXToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem ExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFbaToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip FbaContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FBA_AddRomMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExportAsToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button coverArtButton1;
        private System.Windows.Forms.Button RemoveCategoryButton;
        private System.Windows.Forms.Button AddCategoryButton;
    }
}

