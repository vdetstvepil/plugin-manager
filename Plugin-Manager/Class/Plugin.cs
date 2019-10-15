using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin_Manager.Class
{
    public abstract class Plugin
    {
        /// <summary>
        /// Заводское имя плагина
        /// </summary>
        public virtual string description
        {
            get;
        }

        /// <summary>
        /// Тип плагина
        /// </summary>
        public virtual string type
        {
            get;
        }

        /// <summary>
        /// Имя (псевдоним) плагина
        /// </summary>
        public virtual string FullName
        {
            get; set;
        }

        /// <summary>
        /// Версия .dll
        /// </summary>
        public virtual string dllVersion
        {
            get;
        }

        /// <summary>
        /// Принадлежность плагина к 64-рязрядной версии
        /// </summary>
        public virtual bool isX64
        {
            get;
        }

        /// <summary>
        /// Путь к плагину
        /// </summary>
        public virtual string FullPath
        {
            get;
        }

        /// <summary>
        /// Использовать как плагин
        /// </summary>
        public virtual bool registerAsPlug
        {
            get; set;
        }


        /// <summary>
        /// Сконфигурировать как синтезатор
        /// </summary>
        public virtual bool registerAsSynth
        {
            get; set;
        }

        /// <summary>
        /// Настроить для работы с темпом
        /// </summary>
        public virtual bool registerAsTempoBasedEffect
        {
            get; set;
        }

        /// <summary>
        /// Принудительное стерео
        /// </summary>
        public virtual bool forceStereo
        {
            get; set;
        }

        /// <summary>
        /// Не перехватывать NRPN
        /// </summary>
        public virtual bool nrpnPassThrough
        {
            get; set;
        }

        /// <summary>
        /// Компенсировать задержку
        /// </summary>
        public virtual bool delayCompensation
        {
            get; set;
        }

        /// <summary>
        /// Последовательный доступ к хосту
        /// </summary>
        public virtual bool seralizeDispatcher
        {
            get; set;
        }

        /// <summary>
        /// Сервер Bitbridge
        /// </summary>
        public virtual int bitBridgeServerId
        {
            get; set;
        }

        /// <summary>
        /// Разрешить монообработку
        /// </summary>
        public virtual bool forceMono
        {
            get; set;
        }

        /// <summary>
        /// Переводить смену банков/программ
        /// </summary>
        public virtual bool translateProgramChanges
        {
            get; set;
        }

        /// <summary>
        /// Приостанавливать при останове
        /// </summary>
        public virtual bool forceSuspendOnStop
        {
            get; set;
        }

        /// <summary>
        /// Приостанавливать при воспроизведении
        /// </summary>
        public virtual bool forceSuspendOnPlay
        {
            get; set;
        }

        /// <summary>
        /// Уникальный ID
        /// </summary>
        public virtual int uniqueId
        {
            get;
        }

        /// <summary>
        /// Число входов
        /// </summary>
        public virtual int numInputs
        {
            get; set;
        }

        /// <summary>
        /// Число выходов
        /// </summary>
        public virtual int numOutputs
        {
            get;
        }

        /// <summary>
        /// Синтезатор
        /// </summary>
        public virtual bool isSynth
        {
            get;
        }

        /// <summary>
        /// Принимает сообщения
        /// </summary>
        public virtual bool wantEvents
        {
            get;
        }

        /// <summary>
        /// Передает сообщения
        /// </summary>
        public virtual bool generateEvents
        {
            get;
        }

        /// <summary>
        /// Является VST плагином
        /// </summary>
        public virtual bool isVst
        {
            get;
        }

        /// <summary>
        /// Является VST3 плагином
        /// </summary>
        public virtual bool isVst3
        {
            get;
        }

        public virtual bool isARA { get; }
        public virtual bool isBad { get; }
        public virtual bool isInternal { get; }
        public virtual bool isShell { get; }
        public virtual bool isShellRoot { get; }

        public virtual string Vendor
        {
            get;
        }

        /// <summary>
        /// Универсальный идентификатор CLSID
        /// </summary>
        public virtual string CLSID
        {
            get;
        }

    }
}
