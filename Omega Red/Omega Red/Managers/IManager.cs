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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Red.Managers
{
    public interface IManager
    {
        void removeItem(object a_Item);

        void persistItemAsync(object a_Item);

        void loadItemAsync(object a_Item);

        bool accessPersistItem(object a_Item);

        bool accessLoadItem(object a_Item);

        void createItem();

        void registerItem(object a_Item);

        System.ComponentModel.ICollectionView Collection { get; }
    }
}
