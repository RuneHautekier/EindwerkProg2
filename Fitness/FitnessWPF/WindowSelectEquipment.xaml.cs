using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using FitnessBL.Model;

namespace FitnessWPF
{
    /// <summary>
    /// Interaction logic for WindowSelectEquipment.xaml
    /// </summary>
    public partial class WindowSelectEquipment : Window
    {
        private readonly FitnessClient client;
        public List<Equipment> ListEquipment = new List<Equipment>();
        private List<Time_slot> TimeSlots;
        private DateTime Date;

        public WindowSelectEquipment(FitnessClient client, List<Time_slot> timeSlots, DateTime date)
        {
            InitializeComponent();
            this.client = client;
            this.TimeSlots = timeSlots;
            this.Date = date;
            Loaded += WindowSelectEquipment_Loaded;
        }

        private async void WindowSelectEquipment_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Equipment> equipments1 = await client.GetAvailableEquipment(
                    Date,
                    TimeSlots.First().Time_slot_id
                );
                listboxEquipment1.ItemsSource = equipments1;

                if (TimeSlots.Count() != 2)
                {
                    listboxEquipment2.Visibility = Visibility.Hidden;
                    lblEquipment2Header.Visibility = Visibility.Hidden;
                }
                else
                {
                    List<Equipment> equipments2 = await client.GetAvailableEquipment(
                        Date,
                        TimeSlots.Last().Time_slot_id
                    );
                    listboxEquipment2.ItemsSource = equipments2;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fout bij het laden: {ex.Message}",
                    "Fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void Equipment1_MouseDoubleClick(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            if (listboxEquipment1.SelectedItem is Equipment selectedEquipment)
            {
                ListEquipment.Add(selectedEquipment);
            }
        }

        private void Equipment2_MouseDoubleClick(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            if (listboxEquipment2.SelectedItem is Equipment selectedEquipment)
            {
                ListEquipment.Add(selectedEquipment);
            }
        }

        private void buttonSaveEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (TimeSlots.Count == 2)
            {
                if (ListEquipment.Count != 2)
                {
                    MessageBox.Show(
                        "You must select an equipment for each time slot",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                if (ListEquipment.Count != 1)
                {
                    MessageBox.Show(
                        "You must select an equipment for the time slot",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
        }
    }
}
