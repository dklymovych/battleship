using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Client.MVVM.Model;


class User
{
    public string access_token { get; set; }

    public User(string accessToken)
    {
        access_token = accessToken;
    }

    public void Logout()
    {
        access_token = "";
    }
    public override string ToString()
    {
        return access_token;
    }
}