using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golden_Phi.Managers
{
    internal interface IManager
    {
        void createItem();

        void removeItem(object a_Item);

        System.ComponentModel.ICollectionView Collection { get; }

        object View { get; }

        bool IsConfirmed { get; }
    }
}
