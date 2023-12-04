using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Client.MVVM.Model;

class Globals
{
    public static User LogginInUser { get; set; }
    public static string GameCode { get; set; }
    public static string MyUsername { get; set; }
    public static string EnemyUsername { get; set; }
    public static bool MyMove { get; set; }
}