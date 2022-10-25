using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Models
{
    public enum DriverType
    {
        Edge,
        Chrome,
        Firefox
    }
    public class Driver
    {
        public string? Title { get; set; }
        public string? Path { get; set; }
    }
}
