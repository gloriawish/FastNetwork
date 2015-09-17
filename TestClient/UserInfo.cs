using FastNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestClient
{
    public class UserInfo:IMessage
    {
        public string username;
        public int age;
        public UserInfo(string name, int age)
        {
            this.username = name;
            this.age = age;
        }
    }
}
