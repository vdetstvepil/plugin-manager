using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Plugin_Manager.Class;

namespace Plugin_Manager
{
    public static class Plugins
    {
        private static RegistryKey readKeyVST;
        private static RegistryKey readKeyDX;
        public static ListBox pluginsListBox;

        public static void SetupDirectory()
        {
            try
            {
                readKeyVST = Registry.CurrentUser.OpenSubKey(@"Software\Cakewalk Music Software\Cakewalk\Cakewalk VST X64\Inventory");
                readKeyDX = Registry.ClassesRoot.OpenSubKey(@"MfxSoftSynths");
                GetListOfPlugins();
            }
            catch (Exception)
            {
                MessageBox.Show("Не найдена необходимая директория." +
                    "\nПроверьте, установлена ли приложение Cakewalk на Ваш компьютер. Затем перезапустите Plugin Manager.\n",
                                              "Критическая ошибка",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Error);
                Environment.Exit(0);
            }

        }
        public static void UpdateList()
        {
            
            pluginsListBox.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                pluginsListBox.Items.Clear();
                /// что-нибудь делаем
            })); 
            Color colorWarn = (Color)ColorConverter.ConvertFromString("#FFE9591D");
            Color colorUsual = (Color)ColorConverter.ConvertFromString("#FFDED5C1");

            foreach (Plugin unit in Plugins.ListOfPlugins)
            {
                if (unit.isX64 == false)
                {
                    pluginsListBox.Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        ListBoxItem item = new ListBoxItem()
                        {
                            Content = unit.FullName,
                            Tag = unit.CLSID,

                        };
                        if (unit is PluginVST)
                            item.Background = new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + "/Images/warn.png")));
                        pluginsListBox.Items.Add(item);
                    }));
                   
                }
                else
                {
                    pluginsListBox.Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        ListBoxItem item = new ListBoxItem()
                        {
                            Content = unit.FullName,
                            Tag = unit.CLSID,
                            //BorderBrush = new SolidColorBrush(colorUsual)
                        };
                        pluginsListBox.Items.Add(item);
                        /// что-нибудь делаем
                    })); 
                }
            }

        }
        public static int a = 0;

        public static List<Plugin> ListOfPlugins = new List<Plugin>();
        public static List<Plugin> FullListOfPlugins = new List<Plugin>();

        public static void GetListOfPlugins()
        {
            ListOfPlugins.Clear();

            string[] subKeys;
            subKeys = readKeyVST.GetSubKeyNames();
            for (int i = 0; i < subKeys.Length; i++)
            {
                PluginVST pluginVST = new PluginVST(readKeyVST, subKeys[i]);
                if (pluginVST.CLSID == "{00000000-0000-0000-0000-000000000000}")
                    continue;
                ListOfPlugins.Add(pluginVST);
                FullListOfPlugins.Add(pluginVST);
            }
            subKeys = readKeyDX.GetSubKeyNames();
            for (int i = 0; i < subKeys.Length; i++)
            {
                PluginDX pluginDX = new PluginDX(readKeyDX, subKeys[i]);
                ListOfPlugins.Add(pluginDX);
                FullListOfPlugins.Add(pluginDX);
            }

            UpdateList();
            
        }

        public static void RestoreListOfPlugins()
        {
            ListOfPlugins.Clear();
            foreach (Plugin unit in FullListOfPlugins)
                ListOfPlugins.Add(unit);
                
        }
        public static void Search(string S)
        {
            if (S == "\n")
            {
                RestoreListOfPlugins();
                UpdateList();
            } else

            if (pluginsListBox != null)
            {                
                //RestoreListOfPlugins();

                for (int i = 0; i<Plugins.ListOfPlugins.Count;i++)
                {
                    if (Plugins.ListOfPlugins[i].FullName.Length < S.Length)
                    {
                        ((ListBoxItem)pluginsListBox.Items[i]).Visibility = Visibility.Collapsed;
                        continue;
                    }
                        if (Plugins.ListOfPlugins[i].FullName.ToUpper().Contains(S.ToUpper()))
                            ((ListBoxItem)pluginsListBox.Items[i]).Visibility = Visibility.Visible;
                        else
                            ((ListBoxItem)pluginsListBox.Items[i]).Visibility = Visibility.Collapsed;
                }
            }
        }


        public static void Sort_ABC()
        {
            List<string> names = new List<string>();

            foreach (Plugin unit in Plugins.ListOfPlugins)
                names.Add(unit.FullName);

            names.Sort();

            List<Plugin> testlist = new List<Plugin>();
            foreach (string name in names)
                foreach (Plugin unit in Plugins.ListOfPlugins.ToArray())
                    if (unit.FullName == name)
                    {
                        testlist.Add(unit);
                        ListOfPlugins.Remove(unit);
                    }
            ListOfPlugins = testlist;

            UpdateList();
        }
        public static void Sort_Developer()
        {
            List<Plugin> testlist = new List<Plugin>();

            foreach (Plugin unit in Plugins.ListOfPlugins)
                testlist.Add(unit);

            ListOfPlugins.Clear();
            pluginsListBox.Items.Clear();

            do
            {
                string Vendor = testlist[0].Vendor;
                ListOfPlugins.Add(testlist[0]);
                testlist.RemoveAt(0);
                if (testlist.Count == 0) break;
                pluginsListBox.Items.Add(testlist[0].FullName);


                foreach (Plugin unit in testlist.ToArray())
                    if (unit.Vendor == Vendor)
                    {
                        ListOfPlugins.Add(unit);
                        testlist.Remove(unit);
                        pluginsListBox.Items.Add(unit.FullName);

                    }
                pluginsListBox.Items.Add(new Separator());

            } while (testlist.Count != 0);


            UpdateList();
        }

        public static void Sort_Type()
        {
            List<Plugin> testlist = new List<Plugin>();

            foreach (Plugin unit in Plugins.ListOfPlugins)
                testlist.Add(unit);

            ListOfPlugins.Clear();
            pluginsListBox.Items.Clear();

            do
            {
                string type = testlist[0].type;
                ListOfPlugins.Add(testlist[0]);
                testlist.RemoveAt(0);
                if (testlist.Count == 0) break;
                pluginsListBox.Items.Add(testlist[0].FullName);


                foreach (Plugin unit in testlist.ToArray())
                    if (unit.type == type)
                    {
                        ListOfPlugins.Add(unit);
                        testlist.Remove(unit);
                        pluginsListBox.Items.Add(unit.FullName);

                    }
                pluginsListBox.Items.Add(new Separator());

            } while (testlist.Count != 0);

            UpdateList();
        }
    }
}
