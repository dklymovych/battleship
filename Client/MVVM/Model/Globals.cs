using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.MVVM.View;

namespace Client.MVVM.Model;

class Globals
{
    public static User LogginInUser { get; set; }
    public static string GameCode { get; set; }
    public static string MyUsername { get; set; }
    public static string EnemyUsername { get; set; }
    public static bool MyMove { get; set; }
    public static string Winner { get; set; }
    public static List<int> Battlefield { get; set; }
    public static string Url { get; } = "http://localhost:5199";
    public static void ShowDialog(string text)
    {
        CustomDialog dialog = new CustomDialog("", text);
        dialog.ShowDialog();
    }
}