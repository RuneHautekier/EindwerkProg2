using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FitnessAPI.DTO;
using FitnessBL.Model;

namespace FitnessWPF
{
    /// <summary>
    /// Interaction logic for WindowMakeReservation.xaml
    /// </summary>
    public partial class WindowMakeReservation : Window
    {
        private readonly FitnessClient client;
        private Member member;
        private List<Time_slot> selectedTimeSlots = new List<Time_slot>();
        private DateTime date;
        private List<Equipment> equipments;

        public WindowMakeReservation(FitnessClient client, Member member)
        {
            InitializeComponent();
            this.client = client;
            this.member = member;
            Loaded += MaakReservatieWindow_Loaded;
        }

        private async void MaakReservatieWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Laad tijdsloten
                List<Time_slot> timeSlots = await client.GetTimeSlots();
                listboxTimeSlots.ItemsSource = timeSlots;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error on loading: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void listboxTimeSlots_DoubleClick(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e
        )
        {
            // Get the selected TimeSlot object
            if (listboxTimeSlots.SelectedItem is Time_slot selectedTS)
            {
                // Store the selected TimeSlot in the field
                selectedTimeSlots.Add(selectedTS);

                lblSelectedTimeSlot.Content = $"You've selected: \n {selectedTimeSlots.First()}";
            }
        }

        private async void ExtraTimeSlot_Checked(object sender, RoutedEventArgs e)
        {
            Time_slot selected = selectedTimeSlots.First();
            Time_slot extraTimeSlot = await client.GetTimeSlotId(selected.Time_slot_id + 1);
            selectedTimeSlots.Add(extraTimeSlot);

            lblSelectedTimeSlot.Content =
                $"You've selected: \n {selectedTimeSlots.First()} \n {selectedTimeSlots.Last()}";
        }

        private async void ExtraTimeSlot_Unchecked(object sender, RoutedEventArgs e)
        {
            selectedTimeSlots.RemoveAt(1);
            lblSelectedTimeSlot.Content = $"You've selected: \n {selectedTimeSlots.First()}";
        }

        private void Calendar_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if the clicked element is a CalendarDayButton
            if (e.OriginalSource is DependencyObject originalSource)
            {
                var dayButton = FindParent<CalendarDayButton>(originalSource);

                if (dayButton != null && dayButton.DataContext is DateTime selectedDate)
                {
                    if (selectedDate < DateTime.Now)
                    {
                        MessageBox.Show(
                            "Date date must be in the future!",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    else
                    {
                        date = selectedDate;
                        lblSelectedDate.Content = date;
                    }
                }
            }
        }

        // Helper method to find a parent of a specific type in the visual tree
        private static T FindParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }

        private void FindEquipment_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectEquipment windowSelectEquipment = new WindowSelectEquipment(
                client,
                selectedTimeSlots,
                date
            );
            if (windowSelectEquipment.ShowDialog() == true)
            {
                equipments = windowSelectEquipment.ListEquipment;
                lblEquipmentTS1.Content = equipments.First().ToString();
                if (checkBoxExtraTimeSlot.IsChecked == true)
                {
                    lblEquipmentTS2.Content = equipments.Last().ToString();
                }
            }
        }

        private async void buttonMakeReservation_Click(object sender, RoutedEventArgs e)
        {
            List<TimeslotEquipmentDTO> tseDTOs = new List<TimeslotEquipmentDTO>();
            TimeslotEquipmentDTO tseDTO = new TimeslotEquipmentDTO
            {
                Time_slot_id = selectedTimeSlots.First().Time_slot_id,
                Equipment_id = equipments.First().Equipment_id,
            };
            tseDTOs.Add(tseDTO);

            if (selectedTimeSlots.Count == 2)
            {
                TimeslotEquipmentDTO tseDTO2 = new TimeslotEquipmentDTO
                {
                    Time_slot_id = selectedTimeSlots.Last().Time_slot_id,
                    Equipment_id = equipments.Last().Equipment_id,
                };
                tseDTOs.Add(tseDTO2);
            }

            ReservationAanmakenDTO reservationDTO = new ReservationAanmakenDTO
            {
                MemberId = member.Member_id,
                Date = date,
                EquipmentPerTimeslot = tseDTOs
            };

            string error = await client.CreateReservation(reservationDTO);

            if (string.IsNullOrEmpty(error))
            {
                MessageBox.Show(
                    "The reservation has been made",
                    "Succes!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                MessageBox.Show(
                    $"{error}",
                    "Error!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}
