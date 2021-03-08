/*  Omega Red - Client PS2 Emulator for PCs
*
*  Omega Red is free software: you can redistribute it and/or modify it under the terms
*  of the GNU Lesser General Public License as published by the Free Software Found-
*  ation, either version 3 of the License, or (at your option) any later version.
*
*  Omega Red is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
*  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
*  PURPOSE.  See the GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License along with Omega Red.
*  If not, see <http://www.gnu.org/licenses/>.
*/

using Omega_Red.Managers;
using Omega_Red.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Omega_Red.ViewModels
{
    class ControlViewModel : BaseViewModel
    {
        private void exit()
        {
            Managers.MediaRecorderManager.Instance.StartStop(false, true);

            if (App.Current.MainWindow != null)
                App.Current.MainWindow.Close();
        }

        public ICommand ExitCommand
        {
            get { return new DelegateCommand(exit); }
        }

        private void about()
        {
            LockScreenManager.Instance.showAbout();
        }

        public ICommand AboutCommand
        {
            get { return new DelegateCommand(about); }
        }

        protected override Managers.IManager Manager
        {
            get { return null; }
        }
    }
}
