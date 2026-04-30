using GameTrackerWPF.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTrackerWPF.Models
{
    public class AppSettings
    {
        public SortingMethod SortingMethod { get; set; } = SortingMethod.Added;
        public bool IsAscending { get; set; } = false;
    }
}
