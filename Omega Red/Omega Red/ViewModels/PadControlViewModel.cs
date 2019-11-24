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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Omega_Red.ViewModels
{
    class PadControlViewModel : ItemsPanelTemplateManager
    {
        public PadControlViewModel(){
            PadControlManager.Instance.PadConfigPanelEvent += Instance_PadConfigPanelEvent;

            PadConfigPanel = PadControlManager.Instance.PadConfigPanel;
        }

        private void Instance_PadConfigPanelEvent(object obj)
        {
            PadConfigPanel = obj;
        }

        public ICollectionView PadControlCollection
        {
            get { return PadControlManager.Instance.Collection; }
        }

        private object mPadConfigPanel = null;

        public object PadConfigPanel
        {
            get { return mPadConfigPanel; }
            set
            {
                mPadConfigPanel = value;

                RaisePropertyChangedEvent("PadConfigPanel");
            }
        }
    }
}
