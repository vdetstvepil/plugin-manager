using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Manager.Class
{
    public class PluginDX : Plugin
    {
        private readonly RegistryKey Key;
        private readonly string SubKey;

        /// <summary>
        /// Тип плагина
        /// </summary>
        public override string type
        {
            get => "DX";
        }


        /// <summary>
        /// Заводское имя плагина
        /// </summary>
        public override string description
        {
            get => (string)Key.OpenSubKey(SubKey).GetValue("Description");
        }

        /// <summary>
        /// Имя (псевдоним) плагина
        /// </summary>
        public override string FullName
        {
            get => (string)Key.OpenSubKey(SubKey).GetValue("Description");
        }
    

        /// <summary>
        /// Универсальный идентификатор CLSID
        /// </summary>
        public override string CLSID
        {
            get => (string)Key.OpenSubKey(SubKey).Name;
        }


        public PluginDX(RegistryKey readKey, string subKey)
        {
            Key = readKey;
            SubKey = subKey;
        }
    }
}
