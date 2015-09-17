using FastNetwork;
using PluginEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginEngine
{
    [Plugin("祝君", "E-mail:10588690@qq.com", "测试组件", "(C)copyright 2015 祝君", "测试插件系统。")]
    public class DefaultPlugin:IPlugin
    {
        public void Initialize()
        {
            Console.WriteLine("DefaultPlugin Initialize");
        }

        public void Startup()
        {
            Console.WriteLine("DefaultPlugin Startup");
        }

        public void ShutDown()
        {
            Console.WriteLine("DefaultPlugin ShutDown");
        }

        public bool SupportUnload
        {
            get { return true; }
        }

        public bool SupportLoad
        {
            get { return true; }
        }

        public bool CheckCanLoad(bool isFirstCall)
        {
            return true;
        }

        public string Version
        {
            get
            {
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            }
        }
    }
}
