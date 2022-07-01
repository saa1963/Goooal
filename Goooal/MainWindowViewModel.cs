using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Stateless;
using System.IO;
using Stateless.Graph;
using System.Windows.Media;
using Goooal.Properties;

namespace Goooal
{
    enum TabloStates
    {
        Начало, СбросТаймераБроска, Игра, Остановка, Конец
    }
    enum TabloProcesses
    {
        Space, Z, Self, Timer, Timer_1, Ctrl_Space, X, Ctrl_Z
    }
    public class MainWindowViewModel: NotifyPropertyChanged
    {
        private const int Attack_1 = 24;
        private TimeSpan m_InitIntervalValue = new TimeSpan(0, 10, 0);
        private TimeSpan m_InitIntervalValue_1 = new TimeSpan(0, 0, 15);
        private TimeSpan m_InitIntervalValue_2 = new TimeSpan(0, 0, Attack_1);
        private TimeSpan m_АкуальноеНачальноеЗначениеТаймераБроска;
        private readonly TimeSpan second = new TimeSpan(0, 0, 1);
        private DispatcherTimer Timer { get; set; }
        private DispatcherTimer Timer_1 { get; set; }

        public MainWindowViewModel()
        {
            m_InitIntervalValue = 
                new TimeSpan(0, Settings.Default.Interval == 0 ? 10 : Settings.Default.Interval, 0);
            m_InitIntervalValue_1 = 
                new TimeSpan(0, 0, Settings.Default.Interval1 == 0 ? 15 : Settings.Default.Interval1);
            Team1 = String.IsNullOrWhiteSpace(Settings.Default.Team1) ? "Команда 1" : Settings.Default.Team1;
            Team2 = String.IsNullOrWhiteSpace(Settings.Default.Team2) ? "Команда 2" : Settings.Default.Team2;
            PlayName = String.IsNullOrWhiteSpace(Settings.Default.PlayName) ? "Название игры" : Settings.Default.PlayName;
            LogoColor = Settings.Default.LogoColor ?? Brushes.White;
            NormalTextColor = Settings.Default.NormalTextColor ?? Brushes.LightBlue;
            SelectedTextColor = Settings.Default.SelectedTextColor ?? Brushes.Red;
            BackgroundColor = Settings.Default.BackgroundColor ?? Brushes.Black;

            stateMachine = new StateMachine<TabloStates, TabloProcesses>(TabloStates.Начало);
            stateMachine.Configure(TabloStates.Начало)
                .OnEntry(() => Init_Начало_State1())
                .PermitReentry(TabloProcesses.Ctrl_Space)
                .PermitReentry(TabloProcesses.Space)
                .Permit(TabloProcesses.Z, TabloStates.СбросТаймераБроска)
                .Permit(TabloProcesses.X, TabloStates.СбросТаймераБроска);
            stateMachine.Configure(TabloStates.СбросТаймераБроска)
                .OnEntry((x) => Init_СбросТаймераБроска_State5(x))
                .Permit(TabloProcesses.Ctrl_Space, TabloStates.Начало)
                .Permit(TabloProcesses.Self, TabloStates.Игра)
                .PermitReentry(TabloProcesses.Space)
                .PermitReentry(TabloProcesses.Z)
                .PermitReentry(TabloProcesses.X);
            stateMachine.Configure(TabloStates.Игра)
                .OnEntry(() => Init_Игра_State6())
                .Permit(TabloProcesses.Ctrl_Space, TabloStates.Начало)
                .Permit(TabloProcesses.Z, TabloStates.СбросТаймераБроска)
                .Permit(TabloProcesses.X, TabloStates.СбросТаймераБроска)
                .Permit(TabloProcesses.Space, TabloStates.Остановка)
                .Permit(TabloProcesses.Timer_1, TabloStates.Остановка)
                .Permit(TabloProcesses.Timer, TabloStates.Конец);
            stateMachine.Configure(TabloStates.Остановка)
                .OnEntry((x) => Init_Остановка_State9_11(x))
                .Permit(TabloProcesses.Z, TabloStates.СбросТаймераБроска)
                .Permit(TabloProcesses.X, TabloStates.СбросТаймераБроска)
                .Permit(TabloProcesses.Ctrl_Space, TabloStates.Начало)
                .PermitIf(TabloProcesses.Space, TabloStates.Игра, () => Interval_1.Seconds > 0)
                .PermitReentryIf(TabloProcesses.Space, () => Interval_1.Seconds <= 0);
            stateMachine.Configure(TabloStates.Конец)
                .OnEntry(() => Init_Конец_State16())
                .Permit(TabloProcesses.Ctrl_Space, TabloStates.Начало)
                .PermitReentry(TabloProcesses.Space)
                .PermitReentry(TabloProcesses.Z)
                .PermitReentry(TabloProcesses.X);
            SetTimer();
            SetTimer_1();
            stateMachine.Fire(TabloProcesses.Ctrl_Space);
        }

        private void Init_СбросТаймераБроска_State5(StateMachine<TabloStates, TabloProcesses>.Transition trans)
        {
#if DEBUG
            PlayName = "AI_5";
#endif
            Timer.Start();
            Timer_1.Stop();
            TimerStopped = false;
            Interval_1 = m_АкуальноеНачальноеЗначениеТаймераБроска; 
            stateMachine.Fire(TabloProcesses.Self);
        }

        private void Init_Остановка_State9_11(StateMachine<TabloStates, TabloProcesses>.Transition trans)
        {
#if DEBUG
            PlayName = "PI_9";
#endif
            Timer.Stop();
            Timer_1.Stop();
            TimerStopped = true;
        }

        private void Init_Конец_State16()
        {
#if DEBUG
            PlayName = "EE_16";
#endif
            Timer.Stop();
            Timer_1.Stop();
            TimerStopped = false;
        }

        private void Init_Игра_State6()
        {
#if DEBUG
            PlayName = "AA_6";
#endif
            Timer.Start();
            Timer_1.Start();
            TimerStopped = false;
        }

        private void Init_Начало_State1()
        {
#if DEBUG
            PlayName = "II_1";
#endif
            Timer.Stop();
            Timer_1.Stop();
            TimerStopped = false;
            Interval = m_InitIntervalValue;
            Interval_1 = TimeSpan.Zero;
        }

        private bool m_TimerStopped = false;
        public bool TimerStopped
        {
            get => m_TimerStopped;
            set
            {
                m_TimerStopped = value;
                OnPropertyChanged("TimerStopped");
            }
        }

        private void ResetTimer(object obj)
        {
            stateMachine.Fire(TabloProcesses.Ctrl_Space);
        }

        private void SwitchTimer(object obj)
        {
            stateMachine.Fire(TabloProcesses.Space);
        }

        private void SwitchTimer_1(object obj)
        {
            m_АкуальноеНачальноеЗначениеТаймераБроска = m_InitIntervalValue_1;
            stateMachine.Fire(TabloProcesses.Z);
        }

        private void SwitchTimer_2(object obj)
        {
            m_АкуальноеНачальноеЗначениеТаймераБроска = m_InitIntervalValue_2;
            stateMachine.Fire(TabloProcesses.X);
        }

        private void SetTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += M_Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void SetTimer_1()
        {
            Timer_1 = new DispatcherTimer();
            Timer_1.Tick += M_Timer_1_Tick;
            Timer_1.Interval = new TimeSpan(0, 0, 1);
        }

        private void M_Timer_Tick(object sender, EventArgs e)
        {
            if (Interval.TotalSeconds > 0)
            {
                Interval = Interval.Subtract(second);
            }
            else
            {
                Interval = Interval.Subtract(second);
                stateMachine.Fire(TabloProcesses.Timer);
            }
        }

        private void M_Timer_1_Tick(object sender, EventArgs e)
        {
            if (Interval_1.TotalSeconds > 1)
            {
                Interval_1 = Interval_1.Subtract(second);
            }
            else
            {
                Interval_1 = Interval_1.Subtract(second);
                stateMachine.Fire(TabloProcesses.Timer_1);
            }
        }

        private string m_Team1;
        public string Team1
        {
            get => m_Team1;
            set
            {
                m_Team1 = value;
                OnPropertyChanged("Team1");
            }
        }
        private string m_Team2;
        public string Team2
        {
            get => m_Team2;
            set
            {
                m_Team2 = value;
                OnPropertyChanged("Team2");
            }
        }
        private int m_Fouls1 = 0;
        public int Fouls1
        {
            get => m_Fouls1;
            set
            {
                m_Fouls1 = value;
                OnPropertyChanged("Fouls1");
            }
        }
        private int m_Fouls2 = 0;
        public int Fouls2
        {
            get => m_Fouls2;
            set
            {
                m_Fouls2 = value;
                OnPropertyChanged("Fouls2");
            }
        }
        private string m_PlayName;
        public string PlayName
        {
            get => m_PlayName;
            set
            {
                m_PlayName = value;
                OnPropertyChanged("PlayName");
            }
        }
        private int m_Score1 = 0;
        public int Score1
        {
            get => m_Score1;
            set
            {
                m_Score1 = value;
                OnPropertyChanged("Score1");
            }
        }
        private int m_Score2 = 0;
        public int Score2
        {
            get => m_Score2;
            set
            {
                m_Score2 = value;
                OnPropertyChanged("Score2");
            }
        }
        private TimeSpan m_Interval;
        private StateMachine<TabloStates, TabloProcesses> stateMachine;

        public TimeSpan Interval
        {
            get => m_Interval;
            set
            {
                m_Interval = value;
                OnPropertyChanged("Interval");
            }
        }
        private TimeSpan m_Interval_1;
        public TimeSpan Interval_1
        {
            get => m_Interval_1;
            set
            {
                m_Interval_1 = value;
                OnPropertyChanged("Interval_1");
            }
        }
        private SolidColorBrush m_BackgroundColor;
        public SolidColorBrush BackgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                m_BackgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }
        private SolidColorBrush m_NormalTextColor;
        public SolidColorBrush NormalTextColor
        {
            get => m_NormalTextColor;
            set
            {
                m_NormalTextColor = value;
                OnPropertyChanged("NormalTextColor");
            }
        }
        private SolidColorBrush m_SelectedTextColor;
        public SolidColorBrush SelectedTextColor
        {
            get => m_SelectedTextColor;
            set
            {
                m_SelectedTextColor = value;
                OnPropertyChanged("SelectedTextColor");
            }
        }
        private SolidColorBrush m_LogoColor;
        public SolidColorBrush LogoColor
        {
            get => m_LogoColor;
            set
            {
                m_LogoColor = value;
                OnPropertyChanged("LogoColor");
            }
        }
        public RelayCommand Score2IncCommand
        {
            get => new RelayCommand(s => Score2++);
        }
        public RelayCommand Score2DecCommand
        {
            get => new RelayCommand(s => Score2--);
        }
        public RelayCommand Score1IncCommand
        {
            get => new RelayCommand(s => Score1++);
        }
        public RelayCommand Score1DecCommand
        {
            get => new RelayCommand(s => Score1--);
        }
        public RelayCommand SwitchTimerCommand
        {
            get => new RelayCommand(SwitchTimer);
        }
        public RelayCommand SwitchTimer_1Command
        {
            get => new RelayCommand(SwitchTimer_1);
        }
        public RelayCommand SwitchTimer_2Command
        {
            get => new RelayCommand(SwitchTimer_2);
        }
        public RelayCommand ResetTimerCommand
        {
            get => new RelayCommand(ResetTimer);
        }
        public RelayCommand Fouls2IncCommand
        {
            get => new RelayCommand(s => Fouls2++);
        }
        public RelayCommand Fouls2DecCommand
        {
            get => new RelayCommand(s => Fouls2--);
        }
        public RelayCommand Fouls1IncCommand
        {
            get => new RelayCommand(s => Fouls1++);
        }
        public RelayCommand Fouls1DecCommand
        {
            get => new RelayCommand(s => Fouls1--);
        }
        public RelayCommand EditDataCommand
        {
            get => new RelayCommand(EditData);
        }
        public RelayCommand LeftCommand
        {
            get => new RelayCommand(Left);
        }
        public RelayCommand RightCommand
        {
            get => new RelayCommand(Right);
        }
        public RelayCommand UpCommand
        {
            get => new RelayCommand(Up);
        }

        private void Up(object obj)
        {
            if (stateMachine.IsInState(TabloStates.Остановка))
            {
                if (Interval < m_InitIntervalValue)
                {
                    Interval = Interval.Add(TimeSpan.FromSeconds(1));
                }
            }
        }

        public RelayCommand DownCommand
        {
            get => new RelayCommand(Down);
        }

        private void Down(object obj)
        {
            if (stateMachine.IsInState(TabloStates.Остановка))
            {
                if (Interval > TimeSpan.Zero)
                {
                    Interval = Interval.Subtract(TimeSpan.FromSeconds(1));
                }
            }
        }

        private void Right(object obj)
        {
            if (stateMachine.IsInState(TabloStates.Остановка))
            {
                if (Interval_1 < m_InitIntervalValue_1 || Interval_1 < m_InitIntervalValue_2)
                {
                    Interval_1 = Interval_1.Add(TimeSpan.FromSeconds(1));
                }
            }
        }

        private void Left(object obj)
        {
            if (stateMachine.IsInState(TabloStates.Остановка))
            {
                if (Interval_1.Seconds > 0)
                {
                    Interval_1 = Interval_1.Subtract(TimeSpan.FromSeconds(1));
                }
            }
        }

        private void EditData(object obj)
        {
            var vm = new EditDataViewModel()
            {
                Team1 = Team1,
                Team2 = Team2,
                PlayName = PlayName,
                Minutes = m_InitIntervalValue.Minutes,
                Attack = m_InitIntervalValue_1.Seconds,
                LogoColor = m_LogoColor.Color,
                NormalTextColor = m_NormalTextColor.Color,
                SelectedTextColor = m_SelectedTextColor.Color,
                BackgroundColor = m_BackgroundColor.Color
            };
            var f = new EditDataView() { DataContext = vm };
            if (f.ShowDialog() ?? false)
            {
                Team1 = vm.Team1;
                Team2 = vm.Team2;
                PlayName = vm.PlayName;
                var oldInterval = m_InitIntervalValue;
                m_InitIntervalValue = new TimeSpan(0, vm.Minutes, 0);
                m_InitIntervalValue_1 = new TimeSpan(0, 0, vm.Attack);
                LogoColor = new SolidColorBrush(vm.LogoColor.Value);
                NormalTextColor = new SolidColorBrush(vm.NormalTextColor.Value);
                SelectedTextColor = new SolidColorBrush(vm.SelectedTextColor.Value);
                BackgroundColor = new SolidColorBrush(vm.BackgroundColor.Value);
                if (!Timer.IsEnabled && Interval.Minutes == oldInterval.Minutes)
                {
                    Interval = m_InitIntervalValue;
                }
                Settings.Default.Team1 = Team1;
                Settings.Default.Team2 = Team2;
                Settings.Default.PlayName = PlayName;
                Settings.Default.Interval = vm.Minutes;
                Settings.Default.Interval1 = vm.Attack;
                Settings.Default.LogoColor = LogoColor;
                Settings.Default.NormalTextColor = NormalTextColor;
                Settings.Default.SelectedTextColor = SelectedTextColor;
                Settings.Default.BackgroundColor = BackgroundColor;
                Settings.Default.Save();
            }
        }

        public RelayCommand HelpCommand
        {
            get => new RelayCommand(Help);
        }

        private void Help(object obj)
        {
            var vm = new HelpViewModel();
            var f = new HelpView() { DataContext = vm };
            f.ShowDialog();
        }

        public RelayCommand GraphCommand
        {
            get => new RelayCommand(Graph);
        }

        private void Graph(object obj)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UmiDotGraph.dot");
            var graph = UmlDotGraph.Format(stateMachine.GetInfo());
            File.WriteAllText(path, graph);
        }
    }
}
