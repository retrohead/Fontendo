using Fontendo.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Fontendo.Extensions
{
    public static class DockManager
    {

        private class CollapseTimer
        {
            public Timer Timer = null!;
            public SplitContainer Container = null!;
            public bool Hide;
            public int TargetSize;
            const int step = 30; // pixels per tick

            public CollapseTimer(SplitContainer? Container)
            {
                if(Container == null)
                    return;
                this.Container = Container;
                Timer = new Timer();
                Timer.Tick += CollapseTimer_Tick;
                Timer.Interval = 1;
            }

            private void CollapseTimer_Tick(object? sender, EventArgs e)
            {
                int current = Container.SplitterDistance;
                if (Math.Abs(current - TargetSize) <= step)
                {
                    Container.SplitterDistance = TargetSize;
                    Timer.Stop();
                    Timer.Dispose();
                }
                else
                {
                    Container.SplitterDistance += (current < TargetSize ? step : -step);
                }
            }
        }
        private class PlaceHolderInfo
        {
            public Control ParentControl = null!;
            public Control? Placeholder = null!;
            // helpers for when parent is TableLayoutPanel
            public TableLayoutPanel? OwnerTable = null;
            public int Row;
            public int Col;
            public int RowSpan;
            public int ColSpan;
            public DockInfo? AttachedDockInfo;
            public bool IsShowing = false;
            // timer for expanding split container
            public CollapseTimer? TimerInfo;

            public PlaceHolderInfo(Control ParentControl) 
            {
                this.ParentControl = ParentControl;
                if (ParentControl.GetType() == typeof(SplitterPanel))
                    TimerInfo = new CollapseTimer(ParentControl.Parent as SplitContainer);
            }
        }

        private class DockInfo
        {
            public DockablePanel DockPanel = null!;
            public UserControl ContentControl = null!;
            public Form? FloatingForm = null!;
            public string Name = null!;
            public PlaceHolderInfo? PlaceHolderInfo;
            // orig min width for split container panel
            public int ParentOrigMinWidth = -1;

            // Track drag state for threshold logic for floating form
            public Point dragOrigin = Point.Empty;
            public bool isDragging = false;
            public bool movedEnough = false;
        }

        private static readonly Dictionary<Control, DockInfo> _dock_registry = new();
        private static readonly List<PlaceHolderInfo> _placeholder_registry = new();


        /// <summary>
        /// Register a dockable control after layout is stable (Form.Shown or after TableLayoutPanel.Layout).
        /// </summary>
        public static void Register(DockablePanel DockPanel, UserControl ContentControl, string Name)
        {
            var parent = DockPanel.Parent ?? throw new InvalidOperationException("Control must have a parent before registering.");

            // initialize the dock panel
            DockPanel.InitializePanel(Name, ContentControl);
            var info = new DockInfo()
            {
                DockPanel = DockPanel,
                Name = Name,
                ContentControl = ContentControl
            };


            // Find the owning TableLayoutPanel (if any)
            TableLayoutPanel? tlp = null;
            Control? chain = parent;
            if (chain is TableLayoutPanel t)
                tlp = t;

            // initilize the placeholder info
            var plinfo = new PlaceHolderInfo(parent)
            {
                OwnerTable = tlp,
                AttachedDockInfo = info,
                IsShowing = true,
            };

            // Determine the actual cell host and cell coordinates
            if (tlp != null)
            {
                // dock panel is a table cell
                plinfo.Row = tlp.GetRow(DockPanel);
                plinfo.Col = tlp.GetColumn(DockPanel);
                plinfo.RowSpan = tlp.GetRowSpan(DockPanel);
                plinfo.ColSpan = tlp.GetColumnSpan(DockPanel);
            }
            else
            {
                plinfo.Row = -1;
                plinfo.Col = -1;
                plinfo.RowSpan = -1;
                plinfo.ColSpan = -1;
            }
            info.PlaceHolderInfo = plinfo;
            _dock_registry[ContentControl] = info;
            _placeholder_registry.Add(plinfo);
        }

        private static void EnsurePlaceholder(DockInfo info)
        {
            if(info.PlaceHolderInfo == null || info.PlaceHolderInfo.Placeholder != null)
                return; // already exists

            if (info.PlaceHolderInfo.OwnerTable != null)
            {
                info.PlaceHolderInfo.Placeholder = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.LightGray // 👈 make it visible
                };
                info.PlaceHolderInfo.OwnerTable.Controls.Add(info.PlaceHolderInfo.Placeholder);
                info.PlaceHolderInfo.OwnerTable.SetCellPosition(info.PlaceHolderInfo.Placeholder,
                    new TableLayoutPanelCellPosition(info.PlaceHolderInfo.Col, info.PlaceHolderInfo.Row));
                info.PlaceHolderInfo.OwnerTable.SetColumnSpan(info.PlaceHolderInfo.Placeholder, info.PlaceHolderInfo.ColSpan);
                info.PlaceHolderInfo.OwnerTable.SetRowSpan(info.PlaceHolderInfo.Placeholder, info.PlaceHolderInfo.RowSpan);
            }
            else
            {
                // Simple container: keep order
                int index = info.PlaceHolderInfo.ParentControl.Controls.GetChildIndex(info.DockPanel);
                info.PlaceHolderInfo.Placeholder = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.LightGray // 👈 visible placeholder
                };
                info.PlaceHolderInfo.ParentControl.Controls.Add(info.PlaceHolderInfo.Placeholder);
                info.PlaceHolderInfo.ParentControl.Controls.SetChildIndex(info.PlaceHolderInfo.Placeholder, Math.Max(0, index));
            }


        }
        const int DragThreshold = 8; // pixels
        public static void PopOut(Control control)
        {
            if (!_dock_registry.TryGetValue(control, out var info)) return;
            if (info.FloatingForm != null) return;
            if(info.PlaceHolderInfo == null) return;

            EnsurePlaceholder(info);
            
            // Instead of measuring the control itself, measure the placeholder
            var placeholderRect = info.PlaceHolderInfo.Placeholder.RectangleToScreen(info.PlaceHolderInfo.Placeholder.ClientRectangle);

            DockablePanel dockablePanel = info.DockPanel;
            // Remove control from its immediate parent (wrapper or table)
            info.PlaceHolderInfo.ParentControl.Controls.Remove(dockablePanel);
            dockablePanel.Controls.Remove(info.ContentControl);

            var floatForm = new Form
            {
                Text = info.DockPanel.HeaderText,
                FormBorderStyle = FormBorderStyle.Sizable,
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
                info.PlaceHolderInfo.ParentControl.Width + 50,
                placeholderRect.Height < minHeight ? minHeight : placeholderRect.Height + 50,
                BoundsSpecified.Location | BoundsSpecified.Size);

            // Apply intended offset
            const int offset = 16;
            floatForm.Location = new Point(floatForm.Left + offset, floatForm.Top + offset);
            control.Dock = DockStyle.Fill;
            floatForm.Controls.Add(control);

            floatForm.FormClosed += (s, e) => Redock(control);

            info.FloatingForm = floatForm;

            // Highlight placeholders while moving
            floatForm.Move += FloatingForm_Move;
            // redocking logic
            floatForm.MouseCaptureChanged += FloatForm_MouseCaptureChanged;
            floatForm.FormClosed += (s, e) => Redock(control);

            // finalize
            info.FloatingForm = floatForm;
            UpdateSplitContainer(info.PlaceHolderInfo, true, false);
            floatForm.Show(info.PlaceHolderInfo.ParentControl.FindForm());
        }

        private static void FloatForm_MouseCaptureChanged(object? sender, EventArgs e)
        {
            if (sender is not Form floatForm) return;
            if (floatForm.Controls.Count == 0) return;
            if (!_dock_registry.TryGetValue(floatForm.Controls[0], out var info)) return;
            if (info.FloatingForm == null) return;
            if (info.PlaceHolderInfo == null) return;

            // End of drag: only redock if movedEnough and overlapping a placeholder
            try
            {
                if (!info.movedEnough)
                    return; // ignore slight bumps / unintended drags

                foreach (var plh in _placeholder_registry)
                {
                    if (plh.Placeholder == null || plh.Placeholder.IsDisposed) continue;

                    var rect = plh.Placeholder.RectangleToScreen(plh.Placeholder.ClientRectangle);
                    if (rect.IntersectsWith(floatForm.Bounds))
                    {
                        info.PlaceHolderInfo.AttachedDockInfo = null; // detach from old placeholder
                        info.PlaceHolderInfo = plh; // set to drop into the new placeholder
                        floatForm.Close();
                        break;
                    }
                }
            }
            finally
            {
                // Reset drag state
                info.isDragging = false;
                info.movedEnough = false;
                info.dragOrigin = Point.Empty;
            }
        }

        private static void FloatingForm_Move(object? sender, EventArgs e)
        {
            if (sender is not Form floatForm) return;
            if(floatForm.Controls.Count == 0) return;
            if (!_dock_registry.TryGetValue(floatForm.Controls[0], out var info)) return;
            if (info.FloatingForm == null) return;
            if (info.PlaceHolderInfo == null) return;

            // Only consider moves while mouse is down (actual drag), not when the owner form moves
            if (Control.MouseButtons != MouseButtons.None)
                {
                    if (!info.isDragging)
                    {
                        info.isDragging = true;
                        info.movedEnough = false;
                        info.dragOrigin = floatForm.Location;
                    }
                    else if (!info.movedEnough)
                    {
                        int dx = floatForm.Left - info.dragOrigin.X;
                        int dy = floatForm.Top - info.dragOrigin.Y;
                        if ((dx * dx + dy * dy) >= DragThreshold * DragThreshold) // Euclidean threshold
                            info.movedEnough = true;
                    }
                }

                // Optional: only highlight placeholders after threshold to reduce jitter
                foreach (var kvp in _placeholder_registry)
                {
                    if (kvp.Placeholder == null) continue;
                    var rect = kvp.Placeholder.RectangleToScreen(kvp.Placeholder.ClientRectangle);
                    if (info.movedEnough && rect.IntersectsWith(floatForm.Bounds))
                    {
                        kvp.Placeholder.BackColor = Color.LightBlue;
                        UpdateSplitContainer(kvp, false, false);
                    }
                    else
                    {
                        kvp.Placeholder.BackColor = Color.LightGray;
                        UpdateSplitContainer(kvp, true, false);
                    }
                }
        }

        private static void UpdateSplitContainer(PlaceHolderInfo info, bool hide, bool docking)
        {
            if (info.AttachedDockInfo == null)
                return;
            // if the original parent is a split container, shrink the panel down
            if (info.ParentControl.GetType() == typeof(SplitterPanel))
            {
                SplitContainer? cont = (SplitContainer?)info.ParentControl.Parent;
                if (cont == null) throw new Exception("Split container panel does not have a parent, odd! :)");
                if (info.AttachedDockInfo.ParentOrigMinWidth == -1)
                {
                    info.AttachedDockInfo.ParentOrigMinWidth = (cont.Panel1 == info.ParentControl) ? cont.Panel1MinSize : cont.Panel2MinSize;
                }
                if(cont.Panel1 == info.ParentControl)
                {
                    cont.Panel1MinSize = 0;
                }
                else
                {
                    cont.Panel2MinSize = 0;
                }
                if (!info.IsShowing && hide) return; // already hidden and want to hide
                if (info.IsShowing && !hide) return; // already shown and dont want to hide
                info.IsShowing = !info.IsShowing;
                if (docking)
                {
                    // dont animate if docking, just pop straight in
                    if (cont.Panel1 == info.ParentControl)
                    {
                        var dinfo = info.AttachedDockInfo;
                        info.AttachedDockInfo = null;
                        cont.SplitterDistance = info.AttachedDockInfo.ParentOrigMinWidth;
                        cont.Panel1MinSize = dinfo.ParentOrigMinWidth;
                        info.AttachedDockInfo = dinfo;
                    }
                    else
                    {
                        // detatch control from dock to stop it resizing
                        var dinfo = info.AttachedDockInfo;
                        info.AttachedDockInfo = null;
                        cont.SplitterDistance = cont.Panel1.Width + cont.Panel2.Width - dinfo.ParentOrigMinWidth;
                        cont.Panel2MinSize = dinfo.ParentOrigMinWidth;
                        info.AttachedDockInfo = dinfo;
                    }
                }
                else
                {
                    AnimateCollapse(info, cont, hide, cont.Panel1 == info.ParentControl, info.AttachedDockInfo.ParentOrigMinWidth);
                }
            }
        }

        private static void AnimateCollapse(PlaceHolderInfo info, SplitContainer cont, bool hide, bool panel1, int minWidth)
        {
            if(info.TimerInfo == null) return;
            if(info.TimerInfo.Timer.Enabled)
            {
                info.TimerInfo.Timer.Stop();
            }
            info.TimerInfo.TargetSize = hide ? 0 : minWidth; // collapsed vs expanded size
            if (!panel1)
            {
                if (hide)
                    info.TimerInfo.TargetSize = cont.Panel1.Width + cont.Panel2.Width;
                else
                    info.TimerInfo.TargetSize = cont.Panel1.Width - minWidth;
            }
            info.TimerInfo.Timer.Start();
        }

        public static void Redock(Control control)
        {
            if (!_dock_registry.TryGetValue(control, out var info)) return;
            if (info.FloatingForm == null) return;
            if (info.PlaceHolderInfo == null) throw new Exception("No placeholder info to drop into");

            info.FloatingForm.Controls.Remove(control);
            info.DockPanel.Reattach(control);
            info.FloatingForm = null;

            if (info.PlaceHolderInfo.OwnerTable != null)
            {

                if (!info.PlaceHolderInfo.OwnerTable.Controls.Contains(info.DockPanel))
                    info.PlaceHolderInfo.OwnerTable.Controls.Add(info.DockPanel);

                info.PlaceHolderInfo.OwnerTable.SetCellPosition(info.DockPanel,
                    new TableLayoutPanelCellPosition(info.PlaceHolderInfo.Col, info.PlaceHolderInfo.Row));

                info.PlaceHolderInfo.OwnerTable.SetColumnSpan(info.DockPanel, info.PlaceHolderInfo.ColSpan);
                info.PlaceHolderInfo.OwnerTable.SetRowSpan(info.DockPanel, info.PlaceHolderInfo.RowSpan);

                if (info.PlaceHolderInfo.Placeholder != null && info.PlaceHolderInfo.OwnerTable.Controls.Contains(info.PlaceHolderInfo.Placeholder))
                {
                    info.PlaceHolderInfo.OwnerTable.Controls.Remove(info.PlaceHolderInfo.Placeholder);
                    info.PlaceHolderInfo.Placeholder.Dispose();
                    info.PlaceHolderInfo.Placeholder = null; // 👈 reset so EnsurePlaceholder can recreate later
                }

                if (!info.PlaceHolderInfo.ParentControl.Controls.Contains(info.DockPanel))
                    info.PlaceHolderInfo.ParentControl.Controls.Add(info.DockPanel);
                info.DockPanel.Dock = DockStyle.Fill;
            }
            else
            {
                int index = info.PlaceHolderInfo.Placeholder != null
                    ? info.PlaceHolderInfo.ParentControl.Controls.GetChildIndex(info.PlaceHolderInfo.Placeholder)
                    : info.PlaceHolderInfo.ParentControl.Controls.Count;

                if (info.PlaceHolderInfo.Placeholder != null && info.PlaceHolderInfo.ParentControl.Controls.Contains(info.PlaceHolderInfo.Placeholder))
                {
                    info.PlaceHolderInfo.ParentControl.Controls.Remove(info.PlaceHolderInfo.Placeholder);
                    info.PlaceHolderInfo.Placeholder.Dispose();
                    info.PlaceHolderInfo.Placeholder = null; // 👈 reset
                }

                if (!info.PlaceHolderInfo.ParentControl.Controls.Contains(info.DockPanel))
                    info.PlaceHolderInfo.ParentControl.Controls.Add(info.DockPanel);

                info.PlaceHolderInfo.ParentControl.Controls.SetChildIndex(info.DockPanel, Math.Max(0, index));
            }
            info.PlaceHolderInfo.AttachedDockInfo = info;
            UpdateSplitContainer(info.PlaceHolderInfo, false, true);
        }


    }
}
