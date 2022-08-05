using Goooal.Properties;
using Stateless;
using Stateless.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace Goooal
{
    enum TStates
    {
        Начало, СбросАтаки15, СбросАтаки24, Атака15, Атака24, НетАтаки, СтопИгра, Конец, Ошибка
    }
    enum TProcesses
    {
        Space, Ctrl_Space, Z, X, Self, Timer, Timer_1
    }
    public enum KindOfAttack { Attack15, Attack24 }
    internal class MainWindowViewModel_1 : NotifyPropertyChanged
    {
        private StateMachine<TStates, TProcesses> stateMachine;
        private KindOfAttack? kindOfAttack;
        public TimerCommon mainTimer { get; set; }
        public TimerAttack attackTimer { get; set; }
        public bool IsShowAttack_1
        {
            get
            {
                if (attackTimer.TotalTime == TimeSpan.Zero)
                    return false;
                else
                    if (!kindOfAttack.HasValue)
                    return false;
                else
                    return mainTimer.TotalTime > attackTimer.TotalTime;
                                //(kindOfAttack == KindOfAttack.Attack24 ? TimeSpan.FromSeconds(24) : TimeSpan.FromSeconds(Attack));
            }
        }
        public MainWindowViewModel_1()
        {
            Init();
            stateMachine = new StateMachine<TStates, TProcesses>(TStates.Начало);
            stateMachine.Configure(TStates.Начало).OnEntry(() => Начало_Init())
                .Ignore(TProcesses.Space)
                .PermitReentry(TProcesses.Ctrl_Space)
                .Permit(TProcesses.Z, TStates.СбросАтаки15)
                .Permit(TProcesses.X, TStates.СбросАтаки24)
                .Permit(TProcesses.Timer, TStates.Конец)
                .Ignore(TProcesses.Timer_1);
            stateMachine.Configure(TStates.СбросАтаки15).OnEntry(() => СбросАтаки15_Init())
                .Ignore(TProcesses.Space)
                .Ignore(TProcesses.Ctrl_Space)
                .Ignore(TProcesses.Z)
                .Ignore(TProcesses.X)
                .Permit(TProcesses.Self, TStates.Атака15)
                .Permit(TProcesses.Timer, TStates.Конец)
                .Permit(TProcesses.Timer_1, TStates.Ошибка);
            stateMachine.Configure(TStates.СбросАтаки24).OnEntry(() => СбросАтаки24_Init())
                .Ignore(TProcesses.Space)
                .Ignore(TProcesses.Ctrl_Space)
                .Ignore(TProcesses.Z)
                .Ignore(TProcesses.X)
                .Permit(TProcesses.Self, TStates.Атака24)
                .Permit(TProcesses.Timer, TStates.Конец)
                .Permit(TProcesses.Timer_1, TStates.Ошибка);
            stateMachine.Configure(TStates.Атака15).OnEntry((x) => Атака15_Init(x))
                .Permit(TProcesses.Space, TStates.СтопИгра)
                .Permit(TProcesses.Ctrl_Space, TStates.Начало)
                .Permit(TProcesses.Z, TStates.СбросАтаки15)
                .Permit(TProcesses.X, TStates.СбросАтаки24)
                .Ignore(TProcesses.Self)
                .Permit(TProcesses.Timer, TStates.Конец)
                .Permit(TProcesses.Timer_1, TStates.НетАтаки);
            stateMachine.Configure(TStates.Атака24).OnEntry((x) => Атака24_Init(x))
                .Permit(TProcesses.Space, TStates.СтопИгра)
                .Permit(TProcesses.Ctrl_Space, TStates.Начало)
                .Permit(TProcesses.Z, TStates.СбросАтаки15)
                .Permit(TProcesses.X, TStates.СбросАтаки24)
                .Ignore(TProcesses.Self)
                .Permit(TProcesses.Timer, TStates.Конец)
                .Permit(TProcesses.Timer_1, TStates.НетАтаки);
            stateMachine.Configure(TStates.НетАтаки).OnEntry((x) => НетАтаки_Init(x))
                .PermitIf(TProcesses.Space, TStates.СтопИгра, () => Settings.Default.IsDirtyTime)
                .IgnoreIf(TProcesses.Space, () => !Settings.Default.IsDirtyTime)
                .Permit(TProcesses.Ctrl_Space, TStates.Начало)
                .Permit(TProcesses.Z, TStates.СбросАтаки15)
                .Permit(TProcesses.X, TStates.СбросАтаки24)
                .PermitIf(TProcesses.Self, TStates.СтопИгра, () => !Settings.Default.IsDirtyTime)
                .IgnoreIf(TProcesses.Self, () => Settings.Default.IsDirtyTime)
                .Permit(TProcesses.Timer, TStates.Конец)
                .Ignore(TProcesses.Timer_1);
            stateMachine.Configure(TStates.СтопИгра).OnEntry(() => СтопИгра_Init())
                .PermitIf(TProcesses.Space, TStates.НетАтаки,
                    () => attackTimer.TotalTime == TimeSpan.Zero && Settings.Default.IsDirtyTime)
                .IgnoreIf(TProcesses.Space, () => attackTimer.TotalTime == TimeSpan.Zero && !Settings.Default.IsDirtyTime)
                .PermitIf(TProcesses.Space, TStates.Атака15, 
                    () => kindOfAttack == KindOfAttack.Attack15 && attackTimer.TotalTime != TimeSpan.Zero)
                .PermitIf(TProcesses.Space, TStates.Атака24,
                    () => kindOfAttack == KindOfAttack.Attack24 && attackTimer.TotalTime != TimeSpan.Zero)
                .Permit(TProcesses.Ctrl_Space, TStates.Начало)
                .Permit(TProcesses.Z, TStates.СбросАтаки15)
                .Permit(TProcesses.X, TStates.СбросАтаки24)
                .Ignore(TProcesses.Self)
                .Permit(TProcesses.Timer, TStates.Конец)
                .Ignore(TProcesses.Timer_1);
            stateMachine.Configure(TStates.Конец).OnEntry(() => Конец_Init())
                .Ignore(TProcesses.Space)
                .Permit(TProcesses.Ctrl_Space, TStates.Начало)
                .Ignore(TProcesses.Z)
                .Ignore(TProcesses.X)
                .Ignore(TProcesses.Self)
                .Ignore(TProcesses.Timer)
                .Ignore(TProcesses.Timer_1);
            stateMachine.Configure(TStates.Ошибка).OnEntry((x) => Ошибка_Init(x));
            mainTimer = new TimerCommon();
            mainTimer.eComplete += (sender, value) => stateMachine.Fire(TProcesses.Timer);
            mainTimer.eTick += (sender, value) => OnPropertyChanged("IsShowAttack_1");
            attackTimer = new TimerAttack();
            attackTimer.eComplete += (sender, value) => stateMachine.Fire(TProcesses.Timer_1);
            attackTimer.eTick += (sender, value) => OnPropertyChanged("IsShowAttack_1");
            Начало_Init();
        }
        private void Init()
        {
            Team1 = Settings.Default.Team1 ?? "Команда-1";
            Team2 = Settings.Default.Team2 ?? "Команда-2";
            PlayName = Settings.Default.PlayName ?? "Название игры";
            Minutes = Settings.Default.Interval <= 0 ? 10 : Settings.Default.Interval;
            Attack = Settings.Default.Interval1 <= 0 ? 15 : Settings.Default.Interval1;
            IsDirtyTime = Settings.Default.IsDirtyTime;
            LogoColor = Settings.Default.LogoColor ?? Brushes.White;
            NormalTextColor = Settings.Default.NormalTextColor ?? Brushes.LightBlue;
            SelectedTextColor = Settings.Default.SelectedTextColor ?? Brushes.Red;
            BackgroundColor = Settings.Default.BackgroundColor ?? Brushes.Black;
        }
        private void Ошибка_Init(StateMachine<TStates, TProcesses>.Transition transition)
        {
#if DEBUG
            PlayName = $"Ошибка <{Enum.GetName(typeof(TStates), transition.Source)}> " +
                $"<{Enum.GetName(typeof(TProcesses), transition.Trigger)}> " +
                $"<{Enum.GetName(typeof(TStates), transition.Destination)}>";
#endif
        }

        private void Конец_Init()
        {
            TimerStopped = true;
#if DEBUG
            PlayName = $"Конец {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
        }

        private void СтопИгра_Init()
        {
            if (!IsDirtyTime)
                TimerStopped = true;
            attackTimer.Stop();
#if DEBUG
            PlayName = $"СтопИгра {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
        }

        private void НетАтаки_Init(StateMachine<TStates, TProcesses>.Transition x)
        {
#if DEBUG
            PlayName = $"НетАтаки {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
            if (x.Source == TStates.СтопИгра) TimerStopped = false;
            stateMachine.Fire(TProcesses.Self);
        }

        private void Атака24_Init(StateMachine<TStates, TProcesses>.Transition x)
        {
            TimerStopped = false;
            attackTimer.Start(KindOfAttack.Attack24, x.Source == TStates.СбросАтаки24);
#if DEBUG
            PlayName = $"Атака24 {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
        }

        private void Атака15_Init(StateMachine<TStates, TProcesses>.Transition x)
        {
            TimerStopped = false;
            attackTimer.Start(KindOfAttack.Attack15, x.Source == TStates.СбросАтаки15);
#if DEBUG
            PlayName = $"Атака15 {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
        }

        private void СбросАтаки24_Init()
        {
#if DEBUG
            PlayName = $"СбросАтаки24 {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
            kindOfAttack = KindOfAttack.Attack24;
            stateMachine.Fire(TProcesses.Self);
        }

        private void СбросАтаки15_Init()
        {
#if DEBUG
            PlayName = $"СбросАтаки15 {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
            kindOfAttack = KindOfAttack.Attack15;
            stateMachine.Fire(TProcesses.Self);
        }

        private void Начало_Init()
        {
            kindOfAttack = null;
            TimerStopped = true;
            mainTimer.Init();
            attackTimer.TotalTime = TimeSpan.Zero;
            OnPropertyChanged("IsShowAttack_1");
#if DEBUG
            PlayName = $"Начало {mainTimer?.IsEnabled.ToString() ?? "null"} {attackTimer?.IsEnabled.ToString() ?? "null"}";
#endif
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
            if (stateMachine.IsInState(TStates.СтопИгра))
            {
                mainTimer.AddTotalTime();
            }
        }

        public RelayCommand DownCommand
        {
            get => new RelayCommand(Down);
        }

        private void Down(object obj)
        {
            if (stateMachine.IsInState(TStates.СтопИгра))
            {
                mainTimer.SubtractTotalTime();
            }
        }

        private void Right(object obj)
        {
            if (stateMachine.IsInState(TStates.СтопИгра))
            {
                attackTimer.AddTotalTime();
            }
        }

        private void Left(object obj)
        {
            if (stateMachine.IsInState(TStates.СтопИгра))
            {
                attackTimer.SubtractTotalTime();
            }
        }

        private void EditData(object obj)
        {
            if (stateMachine.State == TStates.Начало)
            {
                var vm = new EditDataViewModel()
                {
                    Team1 = this.Team1,
                    Team2 = this.Team2,
                    PlayName = this.PlayName,
                    Minutes = this.Minutes,
                    Attack = this.Attack,
                    IsDirtyTime = this.IsDirtyTime,
                    LogoColor = this.LogoColor.Color,
                    NormalTextColor = this.NormalTextColor.Color,
                    SelectedTextColor = this.SelectedTextColor.Color,
                    BackgroundColor = this.BackgroundColor.Color
                };
                var f = new EditDataView() { DataContext = vm };
                if (f.ShowDialog() ?? false)
                {
                    Settings.Default.LogoColor = new SolidColorBrush(vm.LogoColor.Value);
                    Settings.Default.NormalTextColor = new SolidColorBrush(vm.NormalTextColor.Value);
                    Settings.Default.SelectedTextColor = new SolidColorBrush(vm.SelectedTextColor.Value);
                    Settings.Default.BackgroundColor = new SolidColorBrush(vm.BackgroundColor.Value);
                    Settings.Default.Team1 = vm.Team1;
                    Settings.Default.Team2 = vm.Team2;
                    Settings.Default.PlayName = vm.PlayName;
                    Settings.Default.Interval = vm.Minutes;
                    Settings.Default.Interval1 = vm.Attack;
                    Settings.Default.IsDirtyTime = vm.IsDirtyTime;
                    Settings.Default.Save();
                    Init();
                    stateMachine.Fire(TProcesses.Ctrl_Space);
                }
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
        public bool TimerStopped
        {
            get => !(mainTimer?.IsEnabled ?? true);
            set
            {
                if (value) mainTimer?.Stop();
                else mainTimer?.Start();
                OnPropertyChanged("TimerStopped");
            }
        }

        public int Minutes { get; private set; }
        public int Attack { get; private set; }
        public bool IsDirtyTime { get; private set; }

        private void ResetTimer(object obj)
        {
            stateMachine.Fire(TProcesses.Ctrl_Space);
        }

        private void SwitchTimer(object obj)
        {
            stateMachine.Fire(TProcesses.Space);
        }

        private void SwitchTimer_1(object obj)
        {
            stateMachine.Fire(TProcesses.Z);
        }

        private void SwitchTimer_2(object obj)
        {
            stateMachine.Fire(TProcesses.X);
        }
    }
}
