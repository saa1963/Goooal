using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Goooal
{
    public class EditDataViewModel: NotifyPropertyChanged
    {
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
        private int m_Attack;
        public int Attack
        {
            get => m_Attack;
            set
            {
                m_Attack = value;
                OnPropertyChanged("Attack");
            }
        }
        private bool m_IsDirtyTime;
        public bool IsDirtyTime
        {
            get => m_IsDirtyTime;
            set
            {
                m_IsDirtyTime = value;
                OnPropertyChanged("IsDirtyTime");
            }
        }
        private Color? m_LogoColor;
        public Color? LogoColor
        {
            get => m_LogoColor;
            set
            {
                m_LogoColor = value;
                OnPropertyChanged("LogoColor");
            }
        }
        private Color? m_NormalTextColor;
        public Color? NormalTextColor
        {
            get => m_NormalTextColor;
            set
            {
                m_NormalTextColor = value;
                OnPropertyChanged("NormalTextColor");
            }
        }
        private Color? m_SelectedTextColor;
        public Color? SelectedTextColor
        {
            get => m_SelectedTextColor;
            set
            {
                m_SelectedTextColor = value;
                OnPropertyChanged("SelectedTextColor");
            }
        }
        private Color? m_BackgroundColor;
        public Color? BackgroundColor
        {
            get => m_BackgroundColor;
            set
            {
                m_BackgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }
    }
}
