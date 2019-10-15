using Microsoft.Win32;
using Plugin_Manager.Class;
using System;
using System.Xml.Serialization;

namespace Plugin_Manager
{
    public class PluginVST : Plugin
    {
        private readonly RegistryKey Key;
        private readonly string SubKey;

        /// <summary>
        /// Заводское имя плагина
        /// </summary>
        public override string description
        {
            get => (string)Key.OpenSubKey(SubKey).GetValue("description");  
        }

        /// <summary>
        /// Тип плагина
        /// </summary>
        public override string type
        {
            get
            {
                if (isVst3)
                    if (isSynth)
                        return "VST3i";
                    else return "VST3";
                else if (isVst)
                    if (isSynth)
                        return "VSTi";
                    else return "VST";
                else return "...";
            }
        }

        /// <summary>
        /// Имя (псевдоним) плагина
        /// </summary>
        public override string FullName
        {
            get => (string)Key.OpenSubKey(SubKey).GetValue("FullName");
            set => Key.OpenSubKey(SubKey, true).SetValue("FullName", value);
        }

        /// <summary>
        /// Версия .dll
        /// </summary>
        public override string dllVersion
        {
            get => (string)Key.OpenSubKey(SubKey).GetValue("dllVersion");
        }

        /// <summary>
        /// Принадлежность плагина к 64-рязрядной версии
        /// </summary>
        public override bool isX64
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isX64"));
        }

        /// <summary>
        /// Путь к плагину
        /// </summary>
        [XmlAttribute("FullPath")]
        public override string FullPath
        {
            get => (string)Key.OpenSubKey(SubKey).GetValue("FullPath");
        }

        /// <summary>
        /// Использовать как плагин
        /// </summary>
        public override bool registerAsPlug
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("registerAsPlug"));
            set => Key.OpenSubKey(SubKey, true).SetValue("registerAsPlug", value);
        }


        /// <summary>
        /// Сконфигурировать как синтезатор
        /// </summary>
        public override bool registerAsSynth
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("registerAsSynth"));
            set => Key.OpenSubKey(SubKey, true).SetValue("registerAsSynth", value);
        }

        /// <summary>
        /// Настроить для работы с темпом
        /// </summary>
        public override bool registerAsTempoBasedEffect
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("registerAsTempoBasedEffect"));
            set => Key.OpenSubKey(SubKey, true).SetValue("registerAsTempoBasedEffect", value);
        }

        /// <summary>
        /// Принудительное стерео
        /// </summary>
        public override bool forceStereo
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("forceStereo"));
            set => Key.OpenSubKey(SubKey, true).SetValue("forceStereo", value);
        }

        /// <summary>
        /// Не перехватывать NRPN
        /// </summary>
        public override bool nrpnPassThrough
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("nrpnPassThrough"));
            set => Key.OpenSubKey(SubKey, true).SetValue("nrpnPassThrough", value);
        }

        /// <summary>
        /// Компенсировать задержку
        /// </summary>
        public override bool delayCompensation
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("delayCompensation"));
            set => Key.OpenSubKey(SubKey, true).SetValue("delayCompensation", value);
        }

        /// <summary>
        /// Последовательный доступ к хосту
        /// </summary>
        public override bool seralizeDispatcher
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("seralizeDispatcher"));
            set => Key.OpenSubKey(SubKey, true).SetValue("seralizeDispatcher", value);
        }

        /// <summary>
        /// Сервер Bitbridge
        /// </summary>
        public override int bitBridgeServerId
        {
            get => (int)Key.OpenSubKey(SubKey).GetValue("bitBridgeServerId");
            set => Key.OpenSubKey(SubKey, true).SetValue("bitBridgeServerId", value);
        }

        /// <summary>
        /// Разрешить монообработку
        /// </summary>
        public override bool forceMono
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("forceMono"));
            set => Key.OpenSubKey(SubKey, true).SetValue("forceMono", value);
        }

        /// <summary>
        /// Переводить смену банков/программ
        /// </summary>
        public override bool translateProgramChanges
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("translateProgramChanges"));
            set => Key.OpenSubKey(SubKey, true).SetValue("translateProgramChanges", value);
        }

        /// <summary>
        /// Приостанавливать при останове
        /// </summary>
        public override bool forceSuspendOnStop
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("forceSuspendOnStop"));
            set => Key.OpenSubKey(SubKey, true).SetValue("forceSuspendOnStop", value);
        }

        /// <summary>
        /// Приостанавливать при воспроизведении
        /// </summary>
        public override bool forceSuspendOnPlay
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("forceSuspendOnPlay"));
            set => Key.OpenSubKey(SubKey, true).SetValue("forceSuspendOnPlay", value);
        }

        /// <summary>
        /// Уникальный ID
        /// </summary>
        public override int uniqueId
        {
            get => (int)Key.OpenSubKey(SubKey).GetValue("uniqueId");
        }

        /// <summary>
        /// Число входов
        /// </summary>
        public override int numInputs
        {
            get => (int)Key.OpenSubKey(SubKey).GetValue("numInputs");
            set => Key.OpenSubKey(SubKey, true).SetValue("numInputs", value);
        }

        /// <summary>
        /// Число выходов
        /// </summary>
        public override int numOutputs
        {
            get => (int)Key.OpenSubKey(SubKey).GetValue("numOutputs");
        }

        /// <summary>
        /// Синтезатор
        /// </summary>
        public override bool isSynth
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isSynth"));
        }

        /// <summary>
        /// Принимает сообщения
        /// </summary>
        public override bool wantEvents
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("wantEvents"));
        }

        /// <summary>
        /// Передает сообщения
        /// </summary>
        public override bool generateEvents
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("generateEvents"));
        }

        /// <summary>
        /// Является VST плагином
        /// </summary>
        public override bool isVst
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isVst"));
        }

        /// <summary>
        /// Является VST3 плагином
        /// </summary>
        public override bool isVst3
        {
            get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isVst3"));
        }

        public override bool isARA { get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isARA")); }
        public override bool isBad { get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isBad")); }
        public override bool isInternal { get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isInternal")); }
        public override bool isShell { get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isShell")); }
        public override bool isShellRoot { get => Convert.ToBoolean(Key.OpenSubKey(SubKey).GetValue("isShellRoot")); }

        public override string Vendor
        {
            get => (string)Key.OpenSubKey(SubKey).GetValue("Vendor");
        }

        /// <summary>
        /// Универсальный идентификатор CLSID
        /// </summary>
        public override string CLSID
        {
           get => (string)Key.OpenSubKey(SubKey).GetValue("clsidPlug");
        }

        public PluginVST(RegistryKey readKey, string subKey)
        {
            Key = readKey;
            SubKey = subKey;
        }

    }
}
