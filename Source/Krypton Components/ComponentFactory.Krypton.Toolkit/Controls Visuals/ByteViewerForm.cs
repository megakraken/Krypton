﻿using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;

namespace ComponentFactory.Krypton.Toolkit {
    /// <summary>
    /// A simple hex-editor form for displaying binary data.
    /// </summary>
    public class ByteViewerForm : KryptonForm {
        #region Instance Members
        KryptonByteViewer byteViewer;
        IContainer components = null;
        #endregion

        #region Identity
        /// <summary>
        /// Initializes a new instance of the ByteViewerForm class.
        /// </summary>
        public ByteViewerForm() {
            InitializeComponent();
        }
        #endregion

        #region Protected Overrides
        /// <summary>
        /// Raises the Load event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            // We re-use the Tag property as input/output mechanism, so we don't have to create
            // a new interface just for that. Kind of a hack, I know.
            var bytes = Tag as byte[];
            if (bytes != null)
                byteViewer.SetBytes(bytes);
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Private
        private void OnCheckedButtonChanged(object sender, EventArgs e) {
            KryptonCheckSet checkset = (KryptonCheckSet)sender; 
            DisplayMode mode;
            switch (checkset.CheckedButton.Text) {
                case "ANSI":
                    mode = DisplayMode.Ansi;
                    break;
                case "Unicode":
                    mode = DisplayMode.Unicode;
                    break;
                case "Hex":
                default:
                    mode = DisplayMode.Hexdump;
                    break;
            }
            // Sets the display mode.
            if(byteViewer != null && byteViewer.GetDisplayMode() != mode)
                byteViewer.SetDisplayMode(mode);
        }

        private void OnClickExport(object sender, EventArgs e) {
            var sfd = new SaveFileDialog() {
                CheckFileExists = false,
                CheckPathExists = false,
                Filter = "All Files (*.*)|*.*"
            };
            if (sfd.ShowDialog(this) == DialogResult.OK) {
                var bytes = Tag as byte[];
                if (bytes != null) {
                    File.WriteAllBytes(sfd.FileName, bytes);
                    // FIXME: string literal.
                    KryptonMessageBox.Show($"Data exported to {sfd.FileName}", "Data Export",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Initializes the Form's components.
        /// </summary>
        private void InitializeComponent() {
            components = new Container();
            KryptonPanel topPanel;
            KryptonPanel bottomPanel;

            KryptonGroupBox groupBox;
            KryptonCheckButton unicodeButton;
            KryptonCheckButton hexButton;
            KryptonCheckButton ansiButton;
            KryptonCheckSet displayModeCheckset;
            KryptonButton export;
            byteViewer = new KryptonByteViewer();
            bottomPanel = new KryptonPanel();
            topPanel = new KryptonPanel();
            groupBox = new KryptonGroupBox();
            unicodeButton = new KryptonCheckButton();
            hexButton = new KryptonCheckButton();
            ansiButton = new KryptonCheckButton();
            displayModeCheckset = new KryptonCheckSet(components);
            export = new KryptonButton();
            ((ISupportInitialize)(topPanel)).BeginInit();
            topPanel.SuspendLayout();
            ((ISupportInitialize)(groupBox)).BeginInit();
            groupBox.Panel.BeginInit();
            groupBox.Panel.SuspendLayout();
            groupBox.SuspendLayout();
            ((ISupportInitialize)(displayModeCheckset)).BeginInit();
            ((ISupportInitialize)(bottomPanel)).BeginInit();
            SuspendLayout();
            // 
            // topPanel
            // 
            topPanel.AutoSize = true;
            topPanel.Controls.Add(groupBox);
            topPanel.Controls.Add(export);
            topPanel.Dock = DockStyle.Top;
            topPanel.Location = new System.Drawing.Point(0, 0);
            topPanel.Name = "topPanel";
            topPanel.Padding = new Padding(5);
            topPanel.Size = new System.Drawing.Size(639, 65);
            topPanel.TabIndex = 0;
            // 
            // groupBox
            // 
            groupBox.AutoSize = true;
            groupBox.Location = new System.Drawing.Point(5, 0);
            groupBox.Name = "groupBox";
            // 
            // groupBox.Panel
            // 
            groupBox.Panel.Controls.Add(unicodeButton);
            groupBox.Panel.Controls.Add(hexButton);
            groupBox.Panel.Controls.Add(ansiButton);
            groupBox.Size = new System.Drawing.Size(280, 57);
            groupBox.TabIndex = 0;
            groupBox.Values.Heading = "Display Mode";
            // 
            // unicodeButton
            // 
            unicodeButton.Location = new System.Drawing.Point(141, 3);
            unicodeButton.Name = "unicodeButton";
            unicodeButton.Size = new System.Drawing.Size(63, 25);
            unicodeButton.TabIndex = 3;
            unicodeButton.Values.Text = "Unicode";
            // 
            // hexButton
            // 
            hexButton.Checked = true;
            hexButton.Location = new System.Drawing.Point(3, 3);
            hexButton.Name = "hexButton";
            hexButton.Size = new System.Drawing.Size(63, 25);
            hexButton.TabIndex = 2;
            hexButton.Values.Text = "Hex";
            // 
            // ansiButton
            // 
            ansiButton.Location = new System.Drawing.Point(72, 3);
            ansiButton.Name = "ansiButton";
            ansiButton.Size = new System.Drawing.Size(63, 25);
            ansiButton.TabIndex = 1;
            ansiButton.Values.Text = "ANSI";
            // 
            // displayModeCheckset
            // 
            displayModeCheckset.CheckButtons.Add(ansiButton);
            displayModeCheckset.CheckButtons.Add(hexButton);
            displayModeCheckset.CheckButtons.Add(unicodeButton);
            displayModeCheckset.CheckedButton = hexButton;
            displayModeCheckset.CheckedButtonChanged += new EventHandler(OnCheckedButtonChanged);
            // 
            // export
            // 
            export.Location = new System.Drawing.Point(535, 22);
            export.Name = "export";
            export.Size = new System.Drawing.Size(80, 25);
            export.TabIndex = 4;
            export.Values.Text = "Export...";
            export.Click += new EventHandler(OnClickExport);
            // 
            // bottomPanel
            // 
            bottomPanel.Dock = DockStyle.Fill;
            bottomPanel.Location = new System.Drawing.Point(0, 65);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(639, 401);
            bottomPanel.TabIndex = 1;
            bottomPanel.Controls.Add(byteViewer);
            //
            // byteViewer
            //
            byteViewer.Dock = DockStyle.Fill;
            byteViewer.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(639, 466);
            Controls.Add(bottomPanel);
            Controls.Add(topPanel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            StartPosition = FormStartPosition.CenterParent;
            Name = "Binary Viewer";
            Text = "Binary Viewer";
            ((ISupportInitialize)(topPanel)).EndInit();
            topPanel.ResumeLayout(false);
            topPanel.PerformLayout();
            groupBox.Panel.EndInit();
            groupBox.Panel.ResumeLayout(false);
            ((ISupportInitialize)(groupBox)).EndInit();
            groupBox.ResumeLayout(false);
            ((ISupportInitialize)(displayModeCheckset)).EndInit();
            ((ISupportInitialize)(bottomPanel)).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
    }
}
