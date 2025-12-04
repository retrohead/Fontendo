using Fontendo.FontProperties;
using System.ComponentModel;
using static Fontendo.FontProperties.GlyphProperties;
using static Fontendo.FontProperties.PropertyList;

namespace Fontendo.Extensions
{
    public class Glyph
    {
        public class GlyphSettings : INotifyPropertyChanged
        {
            private GlyphPropertyRegistry Properties { get; set; } = new GlyphPropertyRegistry();

            private List<GlyphProperty> CreatedProperties = new List<GlyphProperty>();

            public int Index
            {
                get 
                { 
                    return Properties.GetValue<UInt16>(GlyphProperty.Index); 
                }
                set
                {
                    if (!CreatedProperties.Contains(GlyphProperty.Index))
                    {
                        Properties.SetValue(GlyphProperty.Index, value);
                        CreatedProperties.Add(GlyphProperty.Index);
                        return;
                    }
                    if (Index != value)
                    {
                        Properties.SetValue(GlyphProperty.Index, value);
                        OnPropertyChanged(nameof(Index));
                    }
                }
            }
            public UInt16 CodePoint
            {
                get
                {
                    return Properties.GetValue<UInt16>(GlyphProperty.Code);
                }
                set
                {
                    if (!CreatedProperties.Contains(GlyphProperty.Code))
                    {
                        Properties.SetValue(GlyphProperty.Code, value);
                        CreatedProperties.Add(GlyphProperty.Code);
                        return;
                    }
                    if (CodePoint != value)
                    {
                        Properties.SetValue(GlyphProperty.Code, value);
                        OnPropertyChanged(nameof(CodePoint));
                    }
                }
            }
            public byte GlyphWidth
            {
                get
                {
                    return Properties.GetValue<Byte>(GlyphProperty.GlyphWidth);
                }
                set
                {
                    if (!CreatedProperties.Contains(GlyphProperty.GlyphWidth))
                    {
                        Properties.SetValue(GlyphProperty.GlyphWidth, value);
                        CreatedProperties.Add(GlyphProperty.GlyphWidth);
                        return;
                    }
                    if (GlyphWidth != value)
                    {
                        Properties.SetValue(GlyphProperty.GlyphWidth, value);
                        OnPropertyChanged(nameof(GlyphWidth));
                    }
                }
            }
            public byte CharWidth
            {
                get
                {
                    return Properties.GetValue<Byte>(GlyphProperty.CharWidth);
                }
                set
                {
                    if (!CreatedProperties.Contains(GlyphProperty.CharWidth))
                    {
                        Properties.SetValue(GlyphProperty.CharWidth, value);
                        CreatedProperties.Add(GlyphProperty.CharWidth);
                        return;
                    }
                    if (CharWidth != value)
                    {
                        Properties.SetValue(GlyphProperty.CharWidth, value);
                        OnPropertyChanged(nameof(CharWidth));
                    }
                }
            }
            public sbyte Left
            {
                get
                {
                    return Properties.GetValue<SByte>(GlyphProperty.Left);
                }
                set
                {
                    if (!CreatedProperties.Contains(GlyphProperty.Left))
                    {
                        Properties.SetValue(GlyphProperty.Left, value);
                        CreatedProperties.Add(GlyphProperty.Left);
                        return;
                    }
                    if (Left != value)
                    {
                        Properties.SetValue(GlyphProperty.Left, value);
                        OnPropertyChanged(nameof(Left));
                    }
                }
            }
            public Bitmap Image
            {
                get
                {
                    return Properties.GetValue<Bitmap>(GlyphProperty.Image);
                }
                set
                {
                    if (!CreatedProperties.Contains(GlyphProperty.Image))
                    {
                        Properties.SetValue(GlyphProperty.Image, value);
                        CreatedProperties.Add(GlyphProperty.Image);
                        return;
                    }
                    if (Image != value)
                    {
                        Properties.SetValue(GlyphProperty.Image, value);
                        OnPropertyChanged(nameof(Image));
                    }
                }
            }

            public void AddProperty(
                GlyphProperty property,
                string name,
                PropertyValueType type,
                EditorType editorType = EditorType.None,
                (long Min, long Max)? range = null)
            {
                Properties.AddProperty(property, name, type, editorType, range);
            }

            public void UpdateValueRange(GlyphProperty property, (long Min, long Max) range)
            {
                Properties.UpdateValueRange(property, range);
            }

            public bool GetBindingForObject(GlyphProperty property, out Binding? binding)
            {
                switch (property)
                {
                    case GlyphProperty.Index:
                        binding = null;
                        return false;
                    case GlyphProperty.Left:
                        binding = new Binding("Value", this, nameof(Left), false, DataSourceUpdateMode.OnPropertyChanged);
                        return true;
                    case GlyphProperty.GlyphWidth:
                        binding = new Binding("Value", this, nameof(GlyphWidth), false, DataSourceUpdateMode.OnPropertyChanged);
                        return true;
                    case GlyphProperty.CharWidth:
                        binding = new Binding("Value", this, nameof(CharWidth), false, DataSourceUpdateMode.OnPropertyChanged);
                        return true;
                    case GlyphProperty.Code:
                        binding = new Binding("HexValue", this, nameof(CodePoint), false, DataSourceUpdateMode.OnPropertyChanged);
                        return true;
                    case GlyphProperty.Image:
                        binding = new Binding("Image", this, nameof(Image), false, DataSourceUpdateMode.OnPropertyChanged);
                        return true;
                    default:
                        throw new NotImplementedException($"No binding implemented for property {property}");
                }
            }

            public T? GetValue<T>(GlyphProperty property)
            {
                return Properties.GetValue<T>(property);
            }

            public Dictionary<GlyphProperty, GlyphPropertyDescriptor> PropertyDescriptors
            {
                get
                {
                    return Properties.GlyphPropertyDescriptors;
                }
                set
                {
                    if (Properties.GlyphPropertyDescriptors != value)
                    {
                        Properties.GlyphPropertyDescriptors.Clear();
                        foreach (var kvp in value)
                        {
                            Properties.GlyphPropertyDescriptors.Add(kvp.Key, kvp.Value);
                        }
                        OnPropertyChanged(nameof(PropertyDescriptors));
                    }
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GlyphSettings Settings 
        { 
            get; 
            private set; 
        }
        public int CMAPId { get; set; }
        public int Index { get; set; }
        public int Sheet { get; set; }

        public Glyph(int sheet, int index, int cmap_id, Dictionary<GlyphProperty, object>? propertyValues = null)
        {
            Sheet = sheet;
            Index = index;
            CMAPId = cmap_id;

            Settings = new GlyphSettings();
            Settings.AddProperty(GlyphProperty.Index, "Index", PropertyValueType.UInt16, EditorType.None);
            Settings.AddProperty(GlyphProperty.Code, "Code point", PropertyValueType.UInt16, EditorType.CodePointPicker);
            Settings.AddProperty(GlyphProperty.Left, "Left", PropertyValueType.SByte, EditorType.NumberBox, (-0x7F, 0x7F));
            Settings.AddProperty(GlyphProperty.GlyphWidth, "Glyph width", PropertyValueType.Byte, EditorType.NumberBox, (0x0, 0xFF));
            Settings.AddProperty(GlyphProperty.CharWidth, "Char width", PropertyValueType.Byte, EditorType.NumberBox, (0x0, 0xFF));
            Settings.AddProperty(GlyphProperty.Image, "Image", PropertyValueType.Image, EditorType.None);
        }

        public Rectangle SelectionRect =>
            new Rectangle(-1, -1, Settings.Image?.Width ?? 0 + 1, Settings.Image?.Height ?? 0 + 1);

        public Rectangle BoundingRect =>
            new Rectangle(0, 0, Settings.Image?.Width ?? 0, Settings.Image?.Height ?? 0);

        public bool Selected { get; set; }
    }
}
