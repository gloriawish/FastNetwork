using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginEngine
{
    /// <summary>
    /// 服务提供标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Serializable]
    public class PluginAttribute : System.Attribute
    {
        /// <summary>
        /// 创建 PluginAttribute class 的新实例
        /// </summary>
        public PluginAttribute(string author, string contact, string name, string copyRight, string description)
        {
            Description = new PluginDescription(author, contact, name, copyRight, description);
        }

        /// <summary>
        /// 创建 PluginAttribute class 的新实例
        /// </summary>
        public PluginAttribute(string author, string contact, string name, string copyRight, string description, bool defaultEnabled)
        {
            Description = new PluginDescription(author, contact, name, copyRight, description, defaultEnabled);
        }

        /// <summary>
        /// 描述
        /// </summary>
        public PluginDescription Description { get; set; }


    }
}
