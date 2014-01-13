namespace GameOfLife.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    public class BoardViewModel : ObservableObject
    {
        private int m_SizeX = 40;
        private int m_SizeY = 20;
        private Timer m_Timer;

        private List<List<TileViewModel>> m_Tiles;

        public IEnumerable<TileViewModel> Tiles
        {
            get
            {
                return m_Tiles.SelectMany(tiles => tiles);
            }
        }

        private bool m_IsRunning;

        public bool IsRunning
        {
            get
            {
                return m_IsRunning;
            }
            set
            {
                if (m_IsRunning != value)
                {
                    m_IsRunning = value;
                    RaisePropertyChanged("IsRunning");
                }
            }
        }

        private double m_GenerationLength = TimeSpan.FromSeconds(1).TotalMilliseconds;

        public double GenerationLength
        {
            get
            {
                return m_GenerationLength;
            }
            set
            {
                if (m_GenerationLength != value)
                {
                    m_GenerationLength = value;
                    RaisePropertyChanged("GenerationLength");
                }
            }
        }

        public double MinGenerationLength
        {
            get
            {
                return TimeSpan.FromSeconds(0.1).TotalMilliseconds;
            }
        }

        public double MaxGenerationLength
        {
            get
            {
                return TimeSpan.FromSeconds(2).TotalMilliseconds;
            }
        }

        private int m_CurrentGeneration;

        public int CurrentGeneration
        {
            get
            {
                return m_CurrentGeneration;
            }
            set
            {
                if (m_CurrentGeneration != value)
                {
                    m_CurrentGeneration = value;
                    RaisePropertyChanged("CurrentGeneration");
                }
            }
        }

        private RelayCommand m_ClearCommand;

        public ICommand ClearCommand
        {
            get
            {
                if (m_ClearCommand == null)
                {
                    m_ClearCommand = new RelayCommand(Clear, CanClear);
                }
                return m_ClearCommand;
            }
        }

        private RelayCommand m_StartCommand;

        public ICommand StartCommand
        {
            get
            {
                if (m_StartCommand == null)
                {
                    m_StartCommand = new RelayCommand(Start, CanStart);
                }
                return m_StartCommand;
            }
        }

        private RelayCommand m_StopCommand;

        public ICommand StopCommand
        {
            get
            {
                if (m_StopCommand == null)
                {
                    m_StopCommand = new RelayCommand(Stop, CanStop);
                }
                return m_StopCommand;
            }
        }

        private void Clear()
        {
            foreach (var tile in Tiles)
            {
                tile.IsAlive = false;
            }
            CurrentGeneration = 0;
        }

        private bool CanClear()
        {
            return !IsRunning;
        }

        private void Start()
        {
            IsRunning = true;
            CurrentGeneration = 0;
            m_Timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(GenerationLength));
        }

        private bool CanStart()
        {
            return !IsRunning;
        }

        private void Stop()
        {
            m_Timer.Change(Timeout.Infinite, Timeout.Infinite);
            IsRunning = false;
        }

        private bool CanStop()
        {
            return IsRunning;
        }

        private void Update(object thisIsNull)
        {
            for (int y = 0; y < m_Tiles.Count; y++)
            {
                for (int x = 0; x < m_Tiles[y].Count; x++)
                {

                    int livingNeighbors = 0;
                    bool yAbove = y > 0;
                    bool yBelow = y < m_SizeY - 1;
                    bool xAbove = x > 0;
                    bool xBelow = x < m_SizeX - 1;

                    if (yAbove && xAbove && m_Tiles[y - 1][x - 1].IsAlive)
                        livingNeighbors++;
                    if (yAbove && m_Tiles[y - 1][x].IsAlive)
                        livingNeighbors++;
                    if (yAbove && xBelow && m_Tiles[y - 1][x + 1].IsAlive)
                        livingNeighbors++;
                    if (xBelow && m_Tiles[y][x + 1].IsAlive)
                        livingNeighbors++;
                    if (yBelow && xBelow && m_Tiles[y + 1][x + 1].IsAlive)
                        livingNeighbors++;
                    if (yBelow && m_Tiles[y + 1][x].IsAlive)
                        livingNeighbors++;
                    if (yBelow && xAbove && m_Tiles[y + 1][x - 1].IsAlive)
                        livingNeighbors++;
                    if (xAbove && m_Tiles[y][x - 1].IsAlive)
                        livingNeighbors++;

                    bool isAlive = m_Tiles[y][x].IsAlive;
                    bool shouldBeAlive = (isAlive && (livingNeighbors == 2 || livingNeighbors == 3)) ||
                                         livingNeighbors == 3;

                    m_Tiles[y][x].ShouldBeAlive = shouldBeAlive;
                }
            }

            for (int y = 0; y < m_Tiles.Count; y++)
            {
                for (int x = 0; x < m_Tiles[y].Count; x++)
                {
                    m_Tiles[y][x].Update();
                }
            }

            CurrentGeneration++;
        }

        public BoardViewModel()
        {
            m_Tiles =
                (from x in Enumerable.Range(0, m_SizeX)
                    from y in Enumerable.Range(0, m_SizeY)
                    select new TileViewModel(x, y))
                    .GroupBy(t => t.Y)
                    .Select(t => t.ToList())
                    .ToList();

            m_Timer = new Timer(Update);
        }
    }
}