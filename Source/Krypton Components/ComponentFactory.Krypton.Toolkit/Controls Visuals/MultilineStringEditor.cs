﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ComponentFactory.Krypton.Toolkit {
    /// <summary>
    /// Multiline String Editor Window.
    /// </summary>
    public class MultilineStringEditor : Form {
        #region Instance Members
        private bool saveChanges = true;
        private KryptonTextBox textBox = new KryptonTextBox();
        private KryptonTextBox owner;
        private VisualStyleRenderer sizeGripRenderer;
        #endregion

        #region Identity
        /// <summary>
        /// Initializes a new instance of the MultilineStringEditor class.
        /// </summary>
        /// <param name="owner"></param>
        public MultilineStringEditor(KryptonTextBox owner) : base() {
            SuspendLayout();
            textBox.Dock = DockStyle.Fill;
            textBox.Multiline = true;
            textBox.KeyDown += new KeyEventHandler(OnKeyDownTextBox);
            textBox.StateCommon.Border.Draw = InheritBool.False;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 262);
            ControlBox = false;
            Controls.Add(textBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            Padding = new Padding(1, 1, 1, 16);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(100, 20);
            AutoSize = false;
            DoubleBuffered = true;
            ResizeRedraw = true;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            this.owner = owner;
            ResumeLayout(false);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Shows the multiline string editor.
        /// </summary>
        public void ShowEditor() {
            Location = owner.PointToScreen(Point.Empty);
            textBox.Text = owner.Text;
            Show();
        }
        #endregion

        #region Protected Override
        /// <summary>
        /// Closes the multiline string editor.
        /// </summary>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        protected override void OnDeactivate(EventArgs e) {
            base.OnDeactivate(e);
            CloseEditor();
        }

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="e">
        /// A PaintEventArgs that contains the event data.
        /// </param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            // Paint the sizing grip.
            if (e.Graphics == null)
                return;
            using (var gripImage = new Bitmap(0x10, 0x10)) {
                using (var g = Graphics.FromImage(gripImage)) {
                    if (Application.RenderWithVisualStyles) {
                        if (sizeGripRenderer == null)
                            sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                        sizeGripRenderer.DrawBackground(g, new Rectangle(0, 0, 0x10, 0x10));
                    } else {
                        ControlPaint.DrawSizeGrip(g, BackColor, 0, 0, 0x10, 0x10);
                    }
                }
                e.Graphics.DrawImage(gripImage, ClientSize.Width - 0x10, ClientSize.Height - 0x10 + 1, 0x10, 0x10);
            }
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Gray, ButtonBorderStyle.Solid);
        }

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">
        /// The Windows Message to process.
        /// </param>
        protected override void WndProc(ref Message m) {
            const int WM_NCHITTEST = 0x0084;
            const int WM_GETMINMAXINFO = 0x0024;
            bool handled = false;
            if (m.Msg == WM_NCHITTEST)
                handled = OnNcHitTest(ref m);
            else if (m.Msg == WM_GETMINMAXINFO)
                handled = OnGetMinMaxInfo(ref m);
            if (!handled)
                base.WndProc(ref m);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Closes the editor form.
        /// </summary>
        private void CloseEditor() {
            if (saveChanges)
                owner.Text = textBox.Text;
            Close();
        }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        /// <param name="sender">
        /// The control.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnKeyDownTextBox(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                saveChanges = false;
                CloseEditor();
            }
        }

        /// <summary>
        /// Occurs when the MinMaxInfo needs to be retrieved by the operating system.
        /// </summary>
        /// <param name="m">
        /// The window message.
        /// </param>
        /// <returns>
        /// true if the message was handled; otherwise false.
        /// </returns>
        private bool OnGetMinMaxInfo(ref Message m) {
            var minmax = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));
            if (!MaximumSize.IsEmpty)
                minmax.maxTrackSize = MaximumSize;
            minmax.minTrackSize = MinimumSize;
            Marshal.StructureToPtr(minmax, m.LParam, false);
            return true;
        }

        /// <summary>
        /// Occurs when the operating system needs to determine what part of the window corresponds
        /// to a particular screen coordinate.
        /// </summary>
        /// <param name="m">
        /// The window message.
        /// </param>
        /// <returns>
        /// true if the message was handled; otherwise false.
        /// </returns>
        private bool OnNcHitTest(ref Message m) {
            const int HTRIGHT = 11,
                      HTBOTTOM = 15,
                      HTBOTTOMRIGHT = 17;
            var clientLocation = PointToClient(Cursor.Position);
            var gripBounds = new GripBounds(ClientRectangle);
            if (gripBounds.BottomRight.Contains(clientLocation))
                m.Result = (IntPtr)HTBOTTOMRIGHT;
            else if (gripBounds.Bottom.Contains(clientLocation))
                m.Result = (IntPtr)HTBOTTOM;
            else if (gripBounds.Right.Contains(clientLocation))
                m.Result = (IntPtr)HTRIGHT;

            return m.Result != IntPtr.Zero;
        }
        #endregion

        #region Internal
        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO {
            public Point reserved;
            public Size maxSize;
            public Point maxPosition;
            public Size minTrackSize;
            public Size maxTrackSize;
        }

        internal struct GripBounds {
            private const int GripSize = 6;
            private const int CornerGripSize = GripSize << 1;

            public GripBounds(Rectangle clientRectangle) {
                this.clientRectangle = clientRectangle;
            }

            private Rectangle clientRectangle;
            public Rectangle ClientRectangle {
                get { return clientRectangle; }
                //set { clientRectangle = value; }
            }

            public Rectangle Bottom {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.Y = rect.Bottom - GripSize + 1;
                    rect.Height = GripSize;
                    return rect;
                }
            }

            public Rectangle BottomRight {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.Y = rect.Bottom - CornerGripSize + 1;
                    rect.Height = CornerGripSize;
                    rect.X = rect.Width - CornerGripSize + 1;
                    rect.Width = CornerGripSize;
                    return rect;
                }
            }

            public Rectangle Top {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.Height = GripSize;
                    return rect;
                }
            }

            public Rectangle TopRight {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.Height = CornerGripSize;
                    rect.X = rect.Width - CornerGripSize + 1;
                    rect.Width = CornerGripSize;
                    return rect;
                }
            }

            public Rectangle Left {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.Width = GripSize;
                    return rect;
                }
            }

            public Rectangle BottomLeft {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.Width = CornerGripSize;
                    rect.Y = rect.Height - CornerGripSize + 1;
                    rect.Height = CornerGripSize;
                    return rect;
                }
            }

            public Rectangle Right {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.X = rect.Right - GripSize + 1;
                    rect.Width = GripSize;
                    return rect;
                }
            }

            public Rectangle TopLeft {
                get {
                    Rectangle rect = ClientRectangle;
                    rect.Width = CornerGripSize;
                    rect.Height = CornerGripSize;
                    return rect;
                }
            }
        }
        #endregion
    }
}
