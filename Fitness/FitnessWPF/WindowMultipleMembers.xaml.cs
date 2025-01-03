using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    /// Interaction logic for WindowMultipleMembers.xaml
    /// </summary>
    public partial class WindowMultipleMembers : Window
    {
        public WindowMultipleMembers(List<Member> members)
        {
            InitializeComponent();
            listboxMembers.ItemsSource = members;
        }

        private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listboxMembers.SelectedItem != null)
            {
                Member member = (Member)listboxMembers.SelectedItem;
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
