namespace GameOfLife.ViewModel {
    using GalaSoft.MvvmLight;

    public class TileViewModel : ObservableObject {
        private readonly double m_Width = 15;
        public double Width {
            get {
                return m_Width;
            }
        }

        private readonly double m_Height = 15;
        public double Height {
            get {
                return m_Height;
            }
        }

        private double m_X;
        public double X {
            get {
                return m_X;
            }
        }

        private double m_Y;
        public double Y {
            get {
                return m_Y;
            }
        }

        public double Top {
            get {
                return m_Y * m_Height;
            }
        }
        public double Left {
            get {
                return m_X * m_Width;
            }
        }

        private bool m_IsAlive;
        public bool IsAlive {
            get {
                return m_IsAlive;
            }
            set {
                if (m_IsAlive != value) {
                    m_IsAlive = value;
                    RaisePropertyChanged("IsAlive");
                }
            }
        }

        public bool ShouldBeAlive { get; set; }

        public void Update() {
            IsAlive = ShouldBeAlive;
        }

        public TileViewModel(int x, int y) {
            m_X = x;
            m_Y = y;
        }
    }
}