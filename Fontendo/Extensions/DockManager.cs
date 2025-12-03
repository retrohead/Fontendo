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
            public int RowSpan;
            public int ColSpan;
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
                    info.RowSpan = tlp.GetRowSpan(control);
                    info.ColSpan = tlp.GetColumnSpan(control);
                }
                else if (parent.Parent == tlp)
                {
                    // Control is inside a wrapper that sits in the table cell
                    info.CellHost = parent;
                    info.Row = tlp.GetRow(parent);
                    info.Col = tlp.GetColumn(parent);
                    info.RowSpan = tlp.GetRowSpan(parent);
                    info.ColSpan = tlp.GetColumnSpan(parent);
                }
                else
                {
                    // Nested deeper: find the first ancestor that is directly hosted by the table
                    Control? p = parent;
                    while (p != null && p.Parent != tlp) p = p.Parent;
                    info.CellHost = p ?? control;
                    info.Row = tlp.GetRow(info.CellHost);
                    info.Col = tlp.GetColumn(info.CellHost);
                    info.RowSpan = tlp.GetRowSpan(info.CellHost);
                    info.ColSpan = tlp.GetColumnSpan(info.CellHost);
                }
            }
            else
            {
                info.CellHost = null;
                info.Row = -1;
                info.Col = -1;
                info.RowSpan = -1;
                info.ColSpan = -1;
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
                info.OwnerTable.SetColumnSpan(info.Placeholder, info.ColSpan);
                info.OwnerTable.SetRowSpan(info.Placeholder, info.RowSpan);
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
        const int DragThreshold = 8; // pixels
        public static void PopOut(Control control)
        {
            if (!_registry.TryGetValue(control, out var info)) return;
            if (info.FloatingForm != null) return;

            EnsurePlaceholder(info);
            
            // Instead of measuring the control itself, measure the placeholder
            var placeholderRect = info.Placeholder.RectangleToScreen(info.Placeholder.ClientRectangle);


            // Remove control from its immediate parent (wrapper or table)
            info.OriginalParent.Controls.Remove(control);

            var floatForm = new Form
            {
                Text = control.Name,
                FormBorderStyle = FormBorderStyle.Fixed3D,
                StartPosition = FormStartPosition.Manual,
                MinimizeBox = false,
                MaximizeBox = false,
                Icon = Properties.Resources.fontendo_logo
            };
            // Tell Windows: "I want this client rect at this location"
            int minHeight = 300;
            floatForm.SetBounds(
                placeholderRect.Left,
                placeholderRect.Top,
                info.OriginalParent.Width + 50,
                placeholderRect.Height < minHeight ? minHeight : placeholderRect.Height + 50,
                BoundsSpecified.Location | BoundsSpecified.Size);

            // Apply your intended offset
            const int offset = 16;
            floatForm.Location = new Point(floatForm.Left + offset, floatForm.Top + offset);
            control.Dock = DockStyle.Fill;

            // if there is a table inside the control, hide the first row
            if (control.Controls[0].GetType() == typeof(TableLayoutPanel))
            {
                TableLayoutPanel tbl = (TableLayoutPanel)control.Controls[0];
                var scrollpanel = tbl.Controls[1];
                var contentpanel = scrollpanel.Controls[0];
                var label = contentpanel.Controls[0];
                tbl.RowStyles[0].Height = 1;
                // check the first control in the table to see if its it label, if so use the contents for the form header
                if (label.GetType() == typeof(Label))
                {
                    floatForm.Text = label.Text;
                }
            }

            floatForm.Controls.Add(control);

            floatForm.FormClosed += (s, e) => Redock(control);

            info.FloatingForm = floatForm;

            // Track drag state for threshold logic
            Point dragOrigin = Point.Empty;
            bool isDragging = false;
            bool movedEnough = false;

            // Highlight placeholders while moving
            floatForm.Move += (s, e) =>
            {
                // Only consider moves while mouse is down (actual drag), not when the owner form moves
                if (Control.MouseButtons != MouseButtons.None)
                {
                    if (!isDragging)
                    {
                        isDragging = true;
                        movedEnough = false;
                        dragOrigin = floatForm.Location;
                    }
                    else if (!movedEnough)
                    {
                        int dx = floatForm.Left - dragOrigin.X;
                        int dy = floatForm.Top - dragOrigin.Y;
                        if ((dx * dx + dy * dy) >= DragThreshold * DragThreshold) // Euclidean threshold
                            movedEnough = true;
                    }
                }

                // Optional: only highlight placeholders after threshold to reduce jitter
                foreach (var kvp in _registry.Values)
                {
                    if (kvp.Placeholder == null) continue;
                    var rect = kvp.Placeholder.RectangleToScreen(kvp.Placeholder.ClientRectangle);
                    kvp.Placeholder.BackColor = (movedEnough && rect.IntersectsWith(floatForm.Bounds))
                        ? Color.LightBlue
                        : Color.LightGray;
                }
            };
            // End of drag: only redock if movedEnough and overlapping a placeholder
            floatForm.MouseCaptureChanged += (s, e) =>
            {
                try
                {
                    if (!movedEnough)
                        return; // ignore slight bumps / unintended drags

                    foreach (var kvp in _registry)
                    {
                        var i = kvp.Value;
                        if (i.Placeholder == null || i.Placeholder.IsDisposed) continue;

                        var rect = i.Placeholder.RectangleToScreen(i.Placeholder.ClientRectangle);
                        if (rect.IntersectsWith(floatForm.Bounds))
                        {
                            // Redock into the matched placeholder
                            i.Placeholder.Controls.Add(kvp.Key);
                            kvp.Key.Dock = DockStyle.Fill;

                            floatForm.Close();
                            break;
                        }
                    }
                }
                finally
                {
                    // Reset drag state
                    isDragging = false;
                    movedEnough = false;
                    dragOrigin = Point.Empty;
                }
            };

            floatForm.FormClosed += (s, e) => Redock(control); // keep your existing fallback
            info.FloatingForm = floatForm;

            floatForm.Show(info.OriginalParent.FindForm());
        }

        public static void Redock(Control control)
        {
            if (!_registry.TryGetValue(control, out var info)) return;
            if (info.FloatingForm == null) return;

            info.FloatingForm.Controls.Remove(control);
            info.FloatingForm = null;

            // if there is a table inside the control, show the first row
            if (control.Controls[0].GetType() == typeof(TableLayoutPanel))
            {
                ((TableLayoutPanel)control.Controls[0]).RowStyles[0].Height = 20;
            }

            if (info.OwnerTable != null && info.CellHost != null)
            {

                if (!info.OwnerTable.Controls.Contains(info.CellHost))
                    info.OwnerTable.Controls.Add(info.CellHost);

                info.OwnerTable.SetCellPosition(info.CellHost,
                    new TableLayoutPanelCellPosition(info.Col, info.Row));

                if (info.Placeholder != null && info.OwnerTable.Controls.Contains(info.Placeholder))
                {
                    info.OwnerTable.Controls.Remove(info.Placeholder);
                    info.Placeholder.Dispose();
                    info.Placeholder = null; // 👈 reset so EnsurePlaceholder can recreate later
                }

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
