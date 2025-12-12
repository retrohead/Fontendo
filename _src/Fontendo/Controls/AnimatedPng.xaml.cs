using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fontendo.Controls
{
    /// <summary>
    /// Interaction logic for AnimatedPng.xaml
    /// </summary>
    public partial class AnimatedPng : UserControl
    {

        private BackgroundWorker _animBgWorker;
        private int _currentImage;
        private bool _paused;

        public AnimatedPng()
        {
            InitializeComponent();
            DefaultStyleKeyProperty.OverrideMetadata(
            typeof(AnimatedPng),
            new FrameworkPropertyMetadata(typeof(AnimatedPng)));

            _animBgWorker = new BackgroundWorker();
            _animBgWorker.DoWork += AnimBgWorker_DoWork;
            _animBgWorker.RunWorkerCompleted += AnimBgWorker_RunWorkerCompleted;

            _currentImage = 0;

            if (ImagesSource != null && ImagesSource.Count > 0)
            {
                AnimatedImageSource.Source = ImagesSource[_currentImage];
            }

            DataContext = AnimatedImageSource; // Ensure DataContext is set to the control itself
        }

        public class AnimatedImageSourceType : INotifyPropertyChanged
        {
            private ImageSource? _source; // Make nullable to fix CS8618
            public event PropertyChangedEventHandler? PropertyChanged; // Make nullable to fix CS8612 and CS8618

            public ImageSource? Source
            {
                get => _source;
                set
                {
                    _source = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Source)));
                }
            }

            protected void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                PropertyChanged?.Invoke(this, e);
            }
        }

        public AnimatedImageSourceType AnimatedImageSource { get; } = new AnimatedImageSourceType();

        public List<ImageSource?>? ImagesSource
        {
            get => (List<ImageSource?>?)GetValue(ImagesSourceProperty);
            set => SetValue(ImagesSourceProperty, value);
        }

        public static readonly DependencyProperty ImagesSourceProperty =
            DependencyProperty.Register(
                nameof(ImagesSource),
                typeof(List<ImageSource>),
                typeof(AnimatedPng),
                new PropertyMetadata(null, OnImagesSourceChanged));

        private static void OnImagesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AnimatedPng animatedPng && animatedPng.ImagesSource != null && animatedPng.ImagesSource.Count > 0)
            {
                animatedPng._currentImage = 0;
                animatedPng.AnimatedImageSource.Source = animatedPng.ImagesSource[animatedPng._currentImage];
            }
        }

        public void Play()
        {
            _paused = false;
            if (!_animBgWorker.IsBusy)
            {
                _animBgWorker.RunWorkerAsync();
            }
        }

        public void Pause()
        {
            _paused = true;
        }

        private void AnimBgWorker_DoWork(object? sender, DoWorkEventArgs e) // Use nullable sender to fix CS8622
        {
            Thread.Sleep(50);
        }

        private void AnimBgWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e) // Use nullable sender to fix CS8622
        {
            if (ImagesSource != null && ImagesSource.Count > 0)
            {
                AnimatedImageSource.Source = ImagesSource[_currentImage];
                this.DataContext = AnimatedImageSource;
            }
            else
            {
                if (!_paused)
                {
                    _animBgWorker.RunWorkerAsync();
                }
                return;
            }

            _currentImage++;
            if (_currentImage >= ImagesSource.Count)
            {
                _currentImage = 0;
            }

            if (!_paused)
            {
                _animBgWorker.RunWorkerAsync();
            }
        }
    }
}
