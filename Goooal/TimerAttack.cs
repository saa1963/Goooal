using Goooal.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Goooal
{
    public class TimerAttack : DispatcherTimer, INotifyPropertyChanged
    {
        public event EventHandler eComplete;
        public event EventHandler eTick;
        public event PropertyChangedEventHandler PropertyChanged;

        private TimeSpan m_ОсталосьВремени;
        private KindOfAttack m_KindOfAttack;
        public TimerAttack() : base()
        {
            Interval = TimeSpan.FromSeconds(1);
            //TotalTime = TimeSpan.FromMinutes(Settings.Default.Interval);
            Tick += M_Timer_Tick;
        }
        public void Start(KindOfAttack kindOfAttack, bool restart)
        {
            if (restart)
            {
                m_KindOfAttack = kindOfAttack;
                TotalTime = MaxTotalTime;
            }
            Start();
        }
        private TimeSpan MaxTotalTime
            => m_KindOfAttack == KindOfAttack.Attack15 ? TimeSpan.FromSeconds(Settings.Default.Interval1) 
                : TimeSpan.FromSeconds(24);
        private void OnPropertyChanged()
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs("TotalTime");
                handler(this, e);
            }
        }
        private void M_Timer_Tick(object sender, EventArgs e)
        {
            TotalTime = TotalTime.Subtract(Interval);
            eTick?.Invoke(this, null);
        }
        public TimeSpan TotalTime
        {
            get
            {
                return m_ОсталосьВремени;
            }
            set
            {
                if (m_ОсталосьВремени != value)
                {
                    m_ОсталосьВремени = value;
                    if (m_ОсталосьВремени == TimeSpan.Zero)
                    {
                        Stop();
                        eComplete?.Invoke(this, null);
                    }
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Добавить секунду
        /// </summary>
        public void AddTotalTime()
        {
            if (TotalTime < MaxTotalTime)
            {
                TotalTime = TotalTime.Add(TimeSpan.FromSeconds(1));
                eTick?.Invoke(this, null);
            }
        }
        /// <summary>
        /// Вычесть секунду
        /// </summary>
        public void SubtractTotalTime()
        {
            if (TotalTime > TimeSpan.Zero)
            {
                TotalTime = TotalTime.Subtract(TimeSpan.FromSeconds(1));
                eTick?.Invoke(this, null);
            }
        }
    }
}
