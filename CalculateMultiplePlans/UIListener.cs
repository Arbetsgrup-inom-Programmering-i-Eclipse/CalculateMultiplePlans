using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;

namespace CalculateMultiplePlans
{
    /// <summary>
    /// This class can listen to UI events in the PTW TBA Scanning software. The design of this is to use the PTW software to run a task list,
    /// and you can use a LINACController to change LINAC parameters as new tasks are required. Run the TBA software and then use find
    /// </summary>
    public class UIListener
    {
        public delegate void ApplicatorChangeHandler(string applicatorId, TbaPopup popup);

        public delegate void EnergyChangeHandler(string energyId, TbaPopup popup);

        public delegate void FieldSizeChangeHandler(double x, double y, TbaPopup popup);

        public delegate void PopupOpsCompletionHandler(TbaPopup popup);

        public delegate void PopupHandler(TbaPopup popup);

        private readonly int _processId;

        private readonly Timer t = new Timer(2000);

        private UIListener(int processId)
        {
            _processId = processId;
        }

        public List<IntPtr> Children { get; set; }

        public bool ByPassOutOfBoundsPopups { get; set; } = true;

        public static UIListener Find()
        {

            IntPtr hWnd = IntPtr.Zero;
            int tbaAppId = Process.GetProcessesByName("ExternalBeam").First().Id;
            var tbaApp = new UIListener(tbaAppId);
            return tbaApp;

        }

        public void UpdateChildren()
        {
            Children = WinAPI.EnumerateProcessWindowHandles(_processId).ToList();
        }

        public void ListenForPopup(double msInterval = 2000)
        {
            t.Elapsed += t_Elapsed;
            t.Start();
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Children = WinAPI.EnumerateProcessWindowHandles(_processId).ToList();
            var captions = Children.Select(c => new { Ptr = c, Caption = WinAPI.GetWindowCaption(c), Instructions = WinAPI.GetAllChildrenWindowHandles(c, 10).Select(WinAPI.GetWindowCaption).ToList() }).ToList();
            var outOfLimits = captions.FirstOrDefault(c => c.Caption == "" && c.Instructions.Count == 2 && c.Instructions.Last().Contains("Field size could not be determined"));
            if (outOfLimits != null)
            {
                t.Stop();
                t.Elapsed -= t_Elapsed;

                var pop = new TbaPopup(outOfLimits.Ptr);

                if (ByPassOutOfBoundsPopups)
                {
                    //We will hanlde here
                    pop.PressOk();
                    t.Start();
                    t.Elapsed += t_Elapsed;
                }
                else
                {
                    //Handle elsewhere
                    OnPopupRaised(pop);
                }
            }

            IntPtr popup = Children.FirstOrDefault(c => WinAPI.GetWindowCaption(c).Contains("PTWtbaScan20"));
            if (popup != IntPtr.Zero)
            {
                var pop = new TbaPopup(popup);
                t.Stop();
                t.Elapsed -= t_Elapsed;
                ParseInstructions(pop);
            }
        }

        private void ParseInstructions(TbaPopup popup)
        {
            OpPopupOpsCompletion(popup);
        }

        public event PopupHandler PopupRaised;

        public void OnPopupRaised(TbaPopup popup)
        {
            PopupRaised?.Invoke(popup);
        }

        public event PopupOpsCompletionHandler PopupOpsCompleted;

        public void OpPopupOpsCompletion(TbaPopup popup)
        {
            PopupOpsCompleted?.Invoke(popup);
        }
    }
}
