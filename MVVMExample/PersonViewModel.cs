using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MVVMExample
{
    public class PersonViewModel : INotifyPropertyChanged
    {

        private PersonModel _person;
        public ICommand IncrementAgeCommand { get; private set; }

        public RelayCommand OpenParamCommand { get; set; } = new RelayCommand((param) =>
        {
            MessageBox.Show(param.ToString());
        });
        public PersonViewModel()
        {
            _person = new PersonModel
            {
                Name = "John Doe",
                Age = 30
            };
            IncrementAgeCommand=new RelayCommand(IncrementAge);
        }

        public string? Name
        {
            get => _person.Name;
            set
            {
                if (_person.Name != value)
                {
                    _person.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public int? Age
        {
            get => _person.Age;
            set
            {
                if (_person.Age != value)
                {
                    _person.Age = value;
                    OnPropertyChanged(nameof(Age));
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        // 实现 INotifyPropertyChanged 接口
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 更新 Age 的方法
        public void IncrementAge()
        {
            Age++;
        }
    }
}
