using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ERPWan
{
    public class GlobalTools
    {
        public static void goToTab(TabControl tc, int index)
        {
            tc.SelectedIndex = index;
        }
        public static String OnlyNum(String Numeros)
        {
            Regex rgx = new Regex("[^0-9]");
            String NumRgx = rgx.Replace(Numeros, "");
            return NumRgx;
        }
    }
}
