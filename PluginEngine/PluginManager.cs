using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PluginEngine
{
    /// <summary>
    /// 插件服务管理
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// 内置的插件类名映射字典
        /// </summary>
        public readonly static Dictionary<InnerPlugin, string> InnerPluginTypeList;



        static PluginManager()
		{
            InnerPluginTypeList = new Dictionary<InnerPlugin, string>();
            InnerPluginTypeList.Add(InnerPlugin.DefaultPlugin, typeof(DefaultPlugin).FullName);
		}

        /// <summary>
        /// 通过文件路径来查找所有服务
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public static PluginInfo[] GetPluginsInAssembly(string assemblyPath)
        {
            try
            {
                return GetPluginsInAssembly(Assembly.LoadFile(assemblyPath));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 查找指定程序集中所有的服务类
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static PluginInfo[] GetPluginsInAssembly(Assembly assembly)
        {
            System.Type[] types = assembly.GetTypes();

            List<PluginInfo> typeList = new List<PluginInfo>();
            Array.ForEach(types, s =>
            {
                if (!s.IsPublic || s.IsAbstract) return;
                Type t = s.GetInterface(typeof(IPlugin).FullName);
                if (t == null) return;

                object[] infos = s.GetCustomAttributes(typeof(PluginAttribute), true);

                PluginInfo info = new PluginInfo();
                info.Assembly = System.IO.Path.GetFileName(assembly.Location);
                info.TypeName = s.FullName;

                if (infos == null || infos.Length == 0)
                {
                    info.Enabled = true;
                }
                else
                {
                    info.Enabled = (infos[0] as PluginAttribute).Description.DefaultEnabled;
                }
                typeList.Add(info);
            });

            return typeList.ToArray();
        }

        /// <summary>
        /// 获得内置的插件
        /// </summary>
        /// <returns></returns>
        public static PluginInfo[] GetPluginsInAssembly()
        {
            return GetPluginsInAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 在指定的路径中查找服务提供者
        /// </summary>
        /// <param name="loaderPath">文件夹列表</param>
        /// <returns>查找的结果</returns>
        public static PluginList GetPlugins(params string[] loaderPath)
        {
            PluginList list = new PluginList();
            Action<string> loader = s =>
            {
                PluginInfo[] slist = GetPluginsInAssembly(s);
                if (slist != null) list.AddRange(slist);
            };
            Action<string> folderLoader = s =>
            {
                if (!System.IO.Path.IsPathRooted(s))
                    s = System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), s);

                string[] files = System.IO.Directory.GetFiles(s, "*.exe");
                Array.ForEach(files, loader);

                files = System.IO.Directory.GetFiles(s, "*.dll");
                Array.ForEach(files, loader);
            };

            folderLoader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            Array.ForEach(loaderPath, folderLoader);

            return list;
        }
    }
}
