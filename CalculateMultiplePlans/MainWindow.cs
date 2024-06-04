using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;

namespace CalculateMultiplePlans
{
    public partial class MainWindow : Form
    {
        private PlanManager pm;

        public MainWindow(Patient patient, Course course)
        {
            patient.BeginModifications();
            InitializeComponent();
            pm = new PlanManager();
            foreach (var plan in course.ExternalPlanSetups)
            {
                lsbAvailablePlans.Items.Add(plan);
            }
        }
        

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            UIListener tba = null;
            foreach (var plan in pm.ListOfPlans)
            {
                tba = UIListener.Find();
                tba.ListenForPopup();
                plan.CalculateDose();
                tba.ListenForPopup();
            }
            MessageBox.Show("Done!");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            pm.Add((ExternalPlanSetup)lsbAvailablePlans.SelectedItem);
            UpdateList();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (pm.CheckIndex(lsbSelectedPlans.SelectedIndex))
                pm.Delete(lsbSelectedPlans.SelectedIndex);
            UpdateList();
        }
        private void UpdateList()
        {
            lsbSelectedPlans.Items.Clear();
            foreach (var ionPlan in pm.ListOfPlans)
            {
                lsbSelectedPlans.Items.Add(ionPlan);
            }
        }
    }
}
