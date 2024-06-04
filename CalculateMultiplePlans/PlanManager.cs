using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace CalculateMultiplePlans
{
    public class PlanManager
    {
        private List<ExternalPlanSetup> listOfPlans;

        public List<ExternalPlanSetup> ListOfPlans
        {
            get { return listOfPlans; }
            set { listOfPlans = value; }
        }
        
        public PlanManager()
        {
            listOfPlans = new List<ExternalPlanSetup>();
        }
        public void Add(ExternalPlanSetup plan)
        {
            listOfPlans.Add(plan);
        }
        public void Delete(int index)
        {
            listOfPlans.RemoveAt(index);
        }
        public bool CheckIndex(int index)
        {
            if (index >= 0 && index < listOfPlans.Count)
                return true;
            else
                return false;
        }
    }
}
