using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Core
{
    internal static class TimingConsts
    {
        public const int TimeToComeInLibrary = 300;
        public const int TimeToTakePlaceInQueue = 1000;
        public const int TimeToGoToLibrary = 500;
        public const int TimeToReturnToStoika = 500;
        public const int TimeBetweenDays = 10000;
        public const int TimeToLeaveFromLibrary = 150;
    }
}
