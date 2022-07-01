using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Goooal
{
    public class HelpViewModel: NotifyPropertyChanged
    {
        private List<StrPair> m_lst = new List<StrPair>
        {
            new StrPair("F1", "помощь"),
            new StrPair("Enter", "ввод данных"),
            new StrPair("Esc", "выход из программы"),
            new StrPair("Q", "Команда-1 минус очко"),
            new StrPair("W", "Команда-1 плюс очко"),
            new StrPair("E", "Команда-2 минус очко"),
            new StrPair("R", "Команда-2 плюс очко"),
            new StrPair("A", "Команда-1 минус фол"),
            new StrPair("S", "Команда-1 плюс фол"),
            new StrPair("D", "Команда-2 минус фол"),
            new StrPair("F", "Команда-2 плюс фол"),
            new StrPair("Пробел", "стоп/старт таймер"),
            new StrPair("Ctrl-пробел", "Сброс таймера"),
            new StrPair("Z", "Старт таймер атаки 15 сек"),
            new StrPair("X", "Старт таймер атаки 24 сек"),
            new StrPair("Стрелка Лево/Право", "(+/- таймер атаки)"),
            new StrPair("Стрелка Вверх/Вниз", "(+/- таймер общего времени)")
        };

        public List<StrPair> lst
        {
            get => m_lst;
            set
            {
                m_lst = value;
                OnPropertyChanged("lst");
            }
        }
    }

    public class StrPair
    {
        public StrPair(string item1, string item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        public string Item1 { get; set; }
        public string Item2 { get; set; }
    }
}
