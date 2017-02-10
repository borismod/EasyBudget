using System;
using System.Reactive.Linq;

namespace EasyBudgetApp.ViewModels
{
    public class DateStateViewModel : BaseViewModel
    {
        private int _month;
        public int Month
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
                OnPropertyChanged(() => Month);
            }
        }

        private int _year;
        public int Year
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
                OnPropertyChanged(() => Year);
            }
        }

        private RelayCommand _increaseMonth;
        public RelayCommand IncreaseMonth
        {
            get
            {
                return _increaseMonth;
            }

            set
            {
                _increaseMonth = value;
                OnPropertyChanged(() => IncreaseMonth);
            }
        }

        private RelayCommand _reduceMonth;
        public RelayCommand ReduceMonth
        {
            get
            {
                return _reduceMonth;
            }

            set
            {
                _reduceMonth = value;
                OnPropertyChanged(() => ReduceMonth);
            }
        }

        private event EventHandler<DateStateEventArgs> DateStateChanged;
        public IObservable<DateStateViewModel> WhenDateStateChanged
        {
            get
            {
                return Observable.FromEventPattern<DateStateEventArgs>(
                        h => DateStateChanged += h,
                        h => DateStateChanged -= h)
                    .Select(x => x.EventArgs.DateState);
            }
        }
        public DateStateViewModel()
        {
            var now = DateTime.Now;
            _month = now.Month;
            _year = now.Year;
            
            CreateCommands();
        }

        private void CreateCommands()
        {
            IncreaseMonth = new RelayCommand((o) =>
            {
                int newMonth = (Month + 1) % 12;
                Month = newMonth == 0 ? 12 : newMonth;
                Year = newMonth == 1 ? Year + 1 : Year;

                OnMonthChanged();
            });

            ReduceMonth = new RelayCommand((o) =>
            {
                int newMonth = Month - 1;
                Month = newMonth == 0 ? 12 : newMonth;
                Year = newMonth == 0 ? Year - 1 : Year;

                OnMonthChanged();
            });
        }
        private void OnMonthChanged()
        {
            DateStateChanged?.Invoke(null, new DateStateEventArgs(this));
        }

        private class DateStateEventArgs : EventArgs
        {
            public DateStateViewModel DateState { get; private set; }

            internal DateStateEventArgs(DateStateViewModel dateState)
            {
                DateState = dateState;
            }
        }
    }
}
