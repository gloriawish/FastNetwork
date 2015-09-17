using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using FastNetwork;

namespace PluginEngine
{
    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 执行初始化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 插件启动
        /// </summary>
        void Startup();

        /// <summary>
        /// 插件卸载
        /// </summary>
        void ShutDown();

        /// <summary>
        /// 是否支持中途卸载
        /// </summary>
        bool SupportUnload { get; }

        /// <summary>
        /// 是否支持中途加载
        /// </summary>
        bool SupportLoad { get; }

        /// <summary>
        /// 检查是否可以加载
        /// </summary>
        /// <param name="isFirstCall">是否是飞鸽传书初始化时候的加载</param>
        /// <returns></returns>
        bool CheckCanLoad(bool isFirstCall);

        /// <summary>
        /// 当前运行的插件版本
        /// </summary>
        string Version { get; }

        #region 事件
        /// <summary>
        /// 请求加载配置事件
        /// </summary>
        //event EventHandler<EventArgs> RequireLoadConfig;

        /// <summary>
        /// 请求保存配置事件
        /// </summary>
        //event EventHandler<EventArgs> ReuqireSaveConfig;
        #endregion
    }
}
