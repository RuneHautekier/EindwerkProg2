using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
using FitnessAPI.Controllers;
using FitnessBL.Model;

namespace FitnessWPF
{
    /// <summary>
    /// Interaction logic for WindowSearchMember.xaml
    /// </summary>
    public partial class WindowSearchMember : Window
    {
        private MemberController memberController;
        private readonly FitnessClient client;
        private Member member;

        public WindowSearchMember()
        {
            InitializeComponent();
            client = new FitnessClient();
        }

        private async void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNameMember.Text))
            {
                MessageBox.Show(
                    "Enter a name!",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            else
            {
                string fullname = txtNameMember.Text.Trim();
                int spaceIndex = fullname.IndexOf(' '); // Zoek de eerste spatie

                if (spaceIndex < 0)
                {
                    MessageBox.Show(
                        "Enter a name as \"firstname lastname\"!",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
                else
                {
                    string firstName = fullname.Substring(0, spaceIndex);
                    string lastName = fullname.Substring(spaceIndex + 1);

                    List<Member> members = await client.GetMember(firstName, lastName);

                    if (members.Count == 1)
                    {
                        member = members.First();
                        lblIdFoundMember.Content = member.Member_id;
                        lblNameFoundMember.Content = $"{member.FirstName} {member.LastName}";
                        lblAdresFoundMember.Content = member.Address;

                        List<Reservation> reservationList = await client.GetReservationsMember(
                            member
                        );
                        lblCountReservations.Content = reservationList.Count.ToString();
                    }
                    else
                    {
                        if (members.Count >= 2)
                        {
                            WindowMultipleMembers multipleMembers = new WindowMultipleMembers(
                                members
                            );
                            if (multipleMembers.ShowDialog() == true)
                            {
                                member = (Member)multipleMembers.listboxMembers.SelectedItem;
                                lblIdFoundMember.Content = member.Member_id;
                                lblNameFoundMember.Content =
                                    $"{member.FirstName} {member.LastName}";
                                lblAdresFoundMember.Content = member.Address;

                                List<Reservation> reservationList =
                                    await client.GetReservationsMember(member);
                                lblCountReservations.Content = reservationList.Count.ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                "There must be a member selected!",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                        }
                    }
                }
            }
        }

        private void buttonMakeReservation_Click(object sender, RoutedEventArgs e)
        {
            if (member == null)
                MessageBox.Show(
                    "There must be a member selected!",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            else
            {
                WindowMakeReservation windowMakeReservation = new WindowMakeReservation(
                    client,
                    member
                );

                windowMakeReservation.Show();
                this.Close();
            }
        }
    }
}
