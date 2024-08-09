using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Schedulers.Tasks;

// TODO: the idea is to make a task that can update the prices in case a listing becomes
// TODO: "Try searching again" message
// TODO: Ideally not via a signature cause im dumb but also because we can click the "Advance Search button"
// TODO: And then click "Search"
// TODO: With a check to only do this when that mesage apears!
// TODO: Can use a lot of code from the  "RefreshMarketPrices.cs" file!
internal class TaskUpdateRetainerSellPrice
{
    // DISCOVERY: If using the 'A' button (KEYBIND works also) instead of clicking on a item to sell 
    // We bypass the context menu and immediately use the first entry on the context menu


    // TODO: Option that uses "Have Retainer Sell Items" if an item is below a certain value
}
