using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ekzXleb
{
    public class Хлеб
    {
        public string Название { get; set; }
        public double Вес { get; set; }
        public double ЦенаЗаКг { get; set; }

        public double GetСтоимость(double количество)
        {
            return Вес * количество * ЦенаЗаКг;
        }
    }

    public class Заказ
    {
        public ObservableCollection<Хлеб> Хлеба { get; set; }
        public Dictionary<Хлеб, double> Количество { get; set; }

        public Заказ()
        {
            Хлеба = new ObservableCollection<Хлеб>();
            Количество = new Dictionary<Хлеб, double>();
        }

        public void Добавить(Хлеб хлеб, double количество)
        {
            if (Количество.ContainsKey(хлеб))
            {
                Количество[хлеб] += количество;
            }
            else
            {
                Количество.Add(хлеб, количество);
                Хлеба.Add(хлеб);
            }
        }

        public double GetОбщаяСтоимость()
        {
            return Количество.Sum(x => x.Value * x.Key.GetСтоимость(1));
        }
    }

    public class Склад
    {
        public Dictionary<Хлеб, double> Остаток { get; set; }

        public Склад()
        {
            Остаток = new Dictionary<Хлеб, double>();
        }

        public void Add(Хлеб хлеб, double количество)
        {
            Остаток.Add(хлеб, количество);
        }

        public bool IsEnough(Заказ заказ, Хлеб хлеб, double количество)
        {
            if (!Остаток.ContainsKey(хлеб) || Остаток[хлеб] < количество)
            {
                return false;
            }
            return true;
        }

        public void Убрать(Хлеб хлеб, double количество)
        {
            if (Остаток.ContainsKey(хлеб))
            {
                Остаток[хлеб] -= количество;
            }
        }
    }

    public partial class MainWindow : Window
    {
        private ObservableCollection<Хлеб> _хлеба;
        private Заказ _заказ;
        private Склад _склад;

        public MainWindow()
        {
            InitializeComponent();


            _хлеба = new ObservableCollection<Хлеб>()
            {
                new Хлеб { Название = "Ржаной", Вес = 0.5, ЦенаЗаКг = 120 },
                new Хлеб { Название = "Пшеничный", Вес = 0.6, ЦенаЗаКг = 80 },
                new Хлеб { Название = "Батон", Вес = 0.3, ЦенаЗаКг = 60 }
            };
            _заказ = new Заказ();
            _склад = new Склад();


            _склад.Add(_хлеба[0], 10);
            _склад.Add(_хлеба[1], 15);
            _склад.Add(_хлеба[2], 20);

            // Привязка данных к элементам управления
            cbХлеб.ItemsSource = _хлеба;
            cbХлеб.DisplayMemberPath = "Название";
            lbЗаказ.ItemsSource = _заказ.Хлеба;
            lbЗаказ.DisplayMemberPath = "Название";
            tbОбщаяСтоимость.SetBinding(TextBlock.TextProperty, new Binding("ОбщаяСтоимость") { StringFormat = "{0:F2}" });


            btnAdd.Click += (s, e) =>
            {
                if (cbХлеб.SelectedItem != null && tbКоличество.Text.Trim() != "")
                {
                    var хлеб = (Хлеб)cbХлеб.SelectedItem;
                    double количество = double.Parse(tbКоличество.Text);


                    if (_склад.IsEnough(_заказ, хлеб, количество))
                    {

                        _заказ.Добавить(хлеб, количество);


                        lbЗаказ.Items.Refresh();
                        tbОбщаяСтоимость.Text = _заказ.GetОбщаяСтоимость().ToString();


                        _склад.Убрать(хлеб, количество);
                    }
                    else
                    {
                        MessageBox.Show("Недостаточно хлеба на складе");
                    }
                }
            };
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
