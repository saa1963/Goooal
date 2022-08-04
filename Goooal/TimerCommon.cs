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
    public class TimerCommon : DispatcherTimer, INotifyPropertyChanged
    {
        public event EventHandler eComplete;
        public event EventHandler eAttack15Plus;
        public event EventHandler eAttack15Minus;
        public event EventHandler eAttack24Minus;
        public event EventHandler eAttack24Plus;
        public event EventHandler eTick;
        public event PropertyChangedEventHandler PropertyChanged;

        private TimeSpan IntervalAttack15;
        private TimeSpan IntervalAttack15Plus;
        private readonly TimeSpan IntervalAttack24;
        private readonly TimeSpan IntervalAttack24Plus;
        private readonly TimeSpan OneMinute;
        private readonly TimeSpan OneMinutePlus;

        private TimeSpan m_ОсталосьВремени;
        private TimeSpan m_MaxTotalTime;
        public TimerCommon() : base()
        {
            IntervalAttack24 = TimeSpan.FromSeconds(24);
            IntervalAttack24Plus = IntervalAttack24.Add(TimeSpan.FromSeconds(1));
            OneMinute = TimeSpan.FromMinutes(1);
            OneMinutePlus = TimeSpan.FromSeconds(61);
            Interval = TimeSpan.FromSeconds(1);
            Init();
            Tick += M_Timer_Tick;
        }
        public void Init()
        {
            m_MaxTotalTime = TotalTime = TimeSpan.FromMinutes(Settings.Default.Interval);
            IntervalAttack15 = TimeSpan.FromSeconds(Settings.Default.Interval1);
            IntervalAttack15Plus = IntervalAttack15.Add(TimeSpan.FromSeconds(1));
        }
        public new TimeSpan Interval
        {
            get => base.Interval;
            set => base.Interval = value;
        }
            
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
                    var oldValue = m_ОсталосьВремени;
                    m_ОсталосьВремени = value;
                    if (m_ОсталосьВремени == OneMinute && (oldValue > m_ОсталосьВремени || oldValue == TimeSpan.Zero))
                    {
                        Interval = new TimeSpan(0, 0, 0, 0, 100);
                    }
                    if (m_ОсталосьВремени == OneMinutePlus && oldValue < m_ОсталосьВремени && oldValue != TimeSpan.Zero)
                    {
                        Interval = TimeSpan.FromSeconds(1);
                    }
                    if (m_ОсталосьВремени == IntervalAttack24 && (oldValue > m_ОсталосьВремени)) 
                        eAttack24Minus?.Invoke(this, null);
                    if (m_ОсталосьВремени == IntervalAttack24Plus && (oldValue < m_ОсталосьВремени))
                        eAttack24Plus?.Invoke(this, null);
                    if (m_ОсталосьВремени == IntervalAttack15 && (oldValue > m_ОсталосьВремени))
                        eAttack15Minus?.Invoke(this, null);
                    if (m_ОсталосьВремени == IntervalAttack15Plus && (oldValue < m_ОсталосьВремени))
                        eAttack15Plus?.Invoke(this, null);
                    if (m_ОсталосьВремени == TimeSpan.Zero) eComplete?.Invoke(this, null);
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Добавить секунду
        /// </summary>
        public void AddTotalTime()
        {
            if (TotalTime < m_MaxTotalTime)
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
