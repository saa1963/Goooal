using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goooal
{
    internal class EditTimersViewModel: NotifyPropertyChanged
    {
        private int m_Minutes;
        public int Minutes
        {
            get => m_Minutes;
            set
            {
                m_Minutes = value;
                OnPropertyChanged("Minutes");
            }
        }
        private int m_Sec;
        public int Sec
        {
            get => m_Sec;
            set
            {
                m_Sec = value;
                OnPropertyChanged("Sec");
            }
        }
        private int m_Tenth;
        public int Tenth
        {
            get => m_Tenth;
            set
            {
                m_Tenth = value;
                OnPropertyChanged("Tenth");
            }
        }

        private int m_Minutes1;
        public int Minutes1
        {
            get => m_Minutes1;
            set
            {
                m_Minutes1 = value;
                OnPropertyChanged("Minutes1");
            }
        }
        private int m_Sec1;
        public int Sec1
        {
            get => m_Sec1;
            set
            {
                m_Sec1 = value;
                OnPropertyChanged("Sec1");
            }
        }
        private int m_Tenth1;
        public int Tenth1
        {
            get => m_Tenth1;
            set
            {
                m_Tenth1 = value;
                OnPropertyChanged("Tenth1");
            }
        }
    }
}
