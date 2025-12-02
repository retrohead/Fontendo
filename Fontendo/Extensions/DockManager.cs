using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Fontendo.Extensions
{
    public static class DockManager
    {
        private class DockInfo
        {
            public Control OriginalParent = null!;
            public TableLayoutPanel? OwnerTable;
            public Control? CellHost;            // The control that occupies the TLP cell (either the control itself or its wrapper)
            public int Row;
            public int Col;
            public Control? Placeholder;
            public Form? FloatingForm;
        }

        private static readonly Dictionary<Control, DockInfo> _registry = new();

        /// <summary>
        /// Register a dockable control after layout is stable (Form.Shown or after TableLayoutPanel.Layout).
        /// </summary>
        public static void Register(Control control)
        {
            var parent = control.Parent ?? throw new InvalidOperationException("Control must have a parent before registering.");

            // Find the owning TableLayoutPanel (if any)
            TableLayoutPanel? tlp = null;
            Control? chain = parent;
            while (chain != null)
            {
                if (chain is TableLayoutPanel t) { tlp = t; break; }
                chain = chain.Parent;
            }

            var info = new DockInfo
            {
                OriginalParent = parent,
                OwnerTable = tlp
            };

            // Determine the actual cell host and cell coordinates
            if (tlp != null)
            {
                if (parent == tlp)
                {
                    // Control is directly in the table cell
                    info.CellHost = control;
                    info.Row = tlp.GetRow(control);
                    info.Col = tlp.GetColumn(control);
                }
                else if (parent.Parent == tlp)
                {
                    // Control is inside a wrapper that sits in the table cell
                    info.CellHost = parent;
                    info.Row = tlp.GetRow(parent);
                    info.Col = tlp.GetColumn(parent);
                }
                else
                {
                    // Nested deeper: find the first ancestor that is directly hosted by the table
                    Control? p = parent;
                    while (p != null && p.Parent != tlp) p = p.Parent;
                    info.CellHost = p ?? control;
                    info.Row = tlp.GetRow(info.CellHost);
                    info.Col = tlp.GetColumn(info.CellHost);
                }
            }
            else
            {
                info.CellHost = null;
                info.Row = -1;
                info.Col = -1;
            }

            _registry[control] = info;
        }

        private static void EnsurePlaceholder(DockInfo info)
        {
            if (info.Placeholder != null) return;

            if (info.OwnerTable != null && info.CellHost != null)
            {
                info.Placeholder = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.LightGray // 👈 make it visible
                };
                info.OwnerTable.Controls.Add(info.Placeholder);
                info.OwnerTable.SetCellPosition(info.Placeholder,
                    new TableLayoutPanelCellPosition(info.Col, info.Row));
            }
            else
            {
                // Simple container: keep order
                int index = info.OriginalParent.Controls.GetChildIndex(info.CellHost ?? info.OriginalParent);
                info.Placeholder = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.LightGray // 👈 visible placeholder
                };
                info.OriginalParent.Controls.Add(info.Placeholder);
                info.OriginalParent.Controls.SetChildIndex(info.Placeholder, Math.Max(0, index));
            }
        }

        public static void PopOut(Control control)
        {
            if (!_registry.TryGetValue(control, out var info)) return;
            if (info.FloatingForm != null) return;

            EnsurePlaceholder(info);

            // Get the screen position of the control before removing it
            var screenRect = control.RectangleToScreen(control.ClientRectangle);

            // Remove control from its immediate parent (wrapper or table)
            info.OriginalParent.Controls.Remove(control);

            var floatForm = new Form
            {
                Text = control.Name,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                StartPosition = FormStartPosition.Manual
            };

            // 1) Make the client area match the original control
            floatForm.ClientSize = screenRect.Size;

            // 2) Set a provisional location, then measure the client’s actual screen position
            floatForm.Location = screenRect.Location;

            // 3) Compute the delta between where the client currently is and where it should be
            //    This accounts for caption/borders/DPI accurately.
            var clientTopLeft = floatForm.RectangleToScreen(floatForm.ClientRectangle).Location;
            var dx = screenRect.Left - clientTopLeft.X;
            var dy = screenRect.Top - clientTopLeft.Y;

            // 4) Apply the delta so the client area aligns perfectly
            floatForm.Location = new Point(floatForm.Left + dx, floatForm.Top + dy);

            // 5) Optional: nudge a bit so it doesn’t overlap the placeholder instantly
            const int offset = 16;
            floatForm.Location = new Point(floatForm.Left + offset, floatForm.Top + offset);

            control.Dock = DockStyle.Fill;
            floatForm.Controls.Add(control);

            floatForm.FormClosed += (s, e) => Redock(control);

            info.FloatingForm = floatForm;

            // Highlight placeholders while moving
            // Highlight placeholders while moving
            floatForm.Move += (s, e) =>
            {
                foreach (var kvp in _registry.Values)
                {
                    if (kvp.Placeholder != null)
                    {
                        var screenRect = kvp.Placeholder.RectangleToScreen(kvp.Placeholder.ClientRectangle);

                        if (screenRect.IntersectsWith(floatForm.Bounds))
                        {
                            kvp.Placeholder.BackColor = Color.LightBlue; // highlight target
                        }
                        else
                        {
                            kvp.Placeholder.BackColor = Color.LightGray; // reset
                        }
                    }
                }
            };

            // Fires when the user releases the mouse after dragging the window
            floatForm.MouseCaptureChanged += (s, e) =>
            {
                foreach (var kvp in _registry)
                {
                    var info = kvp.Value;
                    if (info.Placeholder != null)
                    {
                        var screenRect = info.Placeholder.RectangleToScreen(info.Placeholder.ClientRectangle);

                        if (screenRect.IntersectsWith(floatForm.Bounds))
                        {
                            Redock(kvp.Key);
                            // Close the floating form itself
                            floatForm.Close();
                            break;
                        }
                    }
                }
            };

            floatForm.Show(info.OriginalParent.FindForm());
        }

        public static void Redock(Control control)
        {
            if (!_registry.TryGetValue(control, out var info)) return;
            if (info.FloatingForm == null) return;

            info.FloatingForm.Controls.Remove(control);
            info.FloatingForm = null;

            if (info.OwnerTable != null && info.CellHost != null)
            {
                if (info.Placeholder != null && info.OwnerTable.Controls.Contains(info.Placeholder))
                {
                    info.OwnerTable.Controls.Remove(info.Placeholder);
                    info.Placeholder.Dispose();
                    info.Placeholder = null; // 👈 reset so EnsurePlaceholder can recreate later
                }

                if (!info.OwnerTable.Controls.Contains(info.CellHost))
                    info.OwnerTable.Controls.Add(info.CellHost);

                info.OwnerTable.SetCellPosition(info.CellHost,
                    new TableLayoutPanelCellPosition(info.Col, info.Row));

                if (!info.OriginalParent.Controls.Contains(control))
                    info.OriginalParent.Controls.Add(control);
                control.Dock = DockStyle.Fill;
            }
            else
            {
                int index = info.Placeholder != null
                    ? info.OriginalParent.Controls.GetChildIndex(info.Placeholder)
                    : info.OriginalParent.Controls.Count;

                if (info.Placeholder != null && info.OriginalParent.Controls.Contains(info.Placeholder))
                {
                    info.OriginalParent.Controls.Remove(info.Placeholder);
                    info.Placeholder.Dispose();
                    info.Placeholder = null; // 👈 reset
                }

                if (!info.OriginalParent.Controls.Contains(control))
                    info.OriginalParent.Controls.Add(control);

                info.OriginalParent.Controls.SetChildIndex(control, Math.Max(0, index));
                control.Dock = DockStyle.Fill;
            }
        }


    }
}
