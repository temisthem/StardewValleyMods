using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterTruffles
{
    public interface IBetterPigsApi
    {
        bool CanDigUpProduce(FarmAnimal animal);
    }
}
