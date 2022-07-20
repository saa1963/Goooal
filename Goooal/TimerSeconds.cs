using Goooal.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Goooal
{
    internal enum KindOfInterval
    {
        Секунды, Десятые
    }
    internal class TimerCommon: DispatcherTimer
    {
        public event EventHandler eComplete;
        public event EventHandler eOneMinute;
        public event EventHandler eCompleteAttack15;
        public event EventHandler eCompleteAttack24;
        public event EventHandler eTick;

        private readonly TimeSpan Attack15;
        private readonly TimeSpan Attack24;
        private readonly TimeSpan OneMinute;
        private TimeSpan m_ОсталосьВремени;
        public TimerCommon():base()
        {
            Attack15 = TimeSpan.FromSeconds(Settings.Default.Interval1);
            Attack24 = TimeSpan.FromSeconds(24);
            OneMinute = TimeSpan.FromMinutes(1);
            m_ОсталосьВремени = TimeSpan.FromMinutes(Settings.Default.Interval);
            Interval = TimeSpan.FromSeconds(1);
            Tick += M_Timer_Tick;
        }

        private void M_Timer_Tick(object sender, EventArgs e)
        {
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

            }
        }
    }
}
