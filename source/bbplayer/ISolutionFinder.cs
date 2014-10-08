using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bbplayer
{
    interface ISolutionFinder
    {
        Solution[] FindSolutions(Solution previousSolution);
    }
}
