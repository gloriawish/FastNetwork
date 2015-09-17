using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginEngine
{
    public class PluginList : List<PluginInfo>
    {
        /// <summary>
        /// 对集合执行操作
        /// </summary>
        /// <param name="action">要执行的函数</param>
        public void ProviderExecute(Action<IPlugin> action)
        {
            this.ForEach(s =>
            {
                if (s.PluginProvider == null) return;
                action(s.PluginProvider);
            });
        }
    }
}
