using Microsoft.Win32;
using Plugin_Manager.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace Plugin_Manager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            Plugins.pluginsListBox = this.listbox;
            Name_sort_tb.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            commandButtonNewMenu.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            AdminCheck();
            Plugins.SetupDirectory();
            GetListOfPluginsasync(pb);
            ShowStatus("Добро пожаловать!", false);
            //((ListBoxItem)listbox.Items[0]).IsSelected = true;

        }

        public void AdminCheck()
        {
            if (IsAdmin())
            {
                adminButton.Visibility = Visibility.Hidden;
                
            }
            else
            {
                Color color = (Color)ColorConverter.ConvertFromString("#FFE9591D");
                adminButton.Foreground = new SolidColorBrush(color);
                
            }
        }

        /// <summary>
        /// Двойной клик по элементу из списка всех плагинов
        /// </summary>       
        public void item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((TreeViewItem)treeview.Items[0]).IsSelected && ((TreeViewItem)treeview.Items[0]).Tag.ToString() == "Folder")
            {
                int Category = 0;
                if (Plugins.ListOfPlugins[listbox.SelectedIndex] is PluginVST)
                {
                    if (Plugins.ListOfPlugins[listbox.SelectedIndex].isVst3)
                        if (Plugins.ListOfPlugins[listbox.SelectedIndex].isSynth)
                            TypePluginLabel.Content = "VST3i";
                        else Category = 9;
                    else if (Plugins.ListOfPlugins[listbox.SelectedIndex].isVst || Plugins.ListOfPlugins[listbox.SelectedIndex].isInternal)
                        if (Plugins.ListOfPlugins[listbox.SelectedIndex].isSynth)
                            Category = 3;
                        else Category = 1;
                    else TypePluginLabel.Content = "...";
                }
                else Category = 2;

                ((TreeViewItem)treeview.Items[0]).Items.Add(new TreeViewItem
                {
                    Header = Plugins.ListOfPlugins[listbox.SelectedIndex].FullName,
                    Foreground = new SolidColorBrush(color),
                    IsExpanded = true,
                    Tag = Plugins.ListOfPlugins[listbox.SelectedIndex].CLSID.ToString() + @"|"
                        + Category + @"|" + Plugins.ListOfPlugins[listbox.SelectedIndex].FullName.ToString() + @"|" +
                        Plugins.ListOfPlugins[listbox.SelectedIndex].description.ToString()
                });
            }
            else
                AddPlugin((TreeViewItem)treeview.Items[0]);

            HighlightPluginsIncluded();
            StateOnlyIncluded_Click(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Добавить выбранный плагин (из списка всех плагинов) в меню
        /// </summary>       
        void AddPlugin(TreeViewItem item)
        {
            ShowStatus("Добавлен плагин в меню", false);
            for (int k = 0; k < item.Items.Count; k++)
                try
                {
                    if (((TreeViewItem)item.Items[k]).Tag.ToString() == "Folder")
                    {
                        if (((TreeViewItem)item.Items[k]).IsSelected)
                        {
                            int Category = 0;
                            if (Plugins.ListOfPlugins[listbox.SelectedIndex] is PluginVST)
                            {
                                if (Plugins.ListOfPlugins[listbox.SelectedIndex].isVst3)
                                    if (Plugins.ListOfPlugins[listbox.SelectedIndex].isSynth)
                                        TypePluginLabel.Content = "VST3i";
                                    else Category = 9;
                                else if (Plugins.ListOfPlugins[listbox.SelectedIndex].isVst || Plugins.ListOfPlugins[listbox.SelectedIndex].isInternal)
                                    if (Plugins.ListOfPlugins[listbox.SelectedIndex].isSynth)
                                        Category = 3;
                                    else Category = 1;
                                else TypePluginLabel.Content = "...";
                            }
                            else Category = 2;

                            ((TreeViewItem)item.Items[k]).Items.Add(new TreeViewItem
                            {
                                Header = Plugins.ListOfPlugins[listbox.SelectedIndex].FullName,
                                Foreground = new SolidColorBrush(color),
                                IsExpanded = true,

                                Tag = Plugins.ListOfPlugins[listbox.SelectedIndex].CLSID.ToString() + @"|"
                                + Category.ToString() + @"|" + Plugins.ListOfPlugins[listbox.SelectedIndex].FullName.ToString() + @"|" +
                                Plugins.ListOfPlugins[listbox.SelectedIndex].description.ToString()
                            });
                        }

                        else
                            AddPlugin(((TreeViewItem)item.Items[k]));
                    }
                }
                catch (Exception) { };
        }


        /// <summary>
        /// Показать сообщение для выбранного плагина
        /// </summary>
        private void MessageShow(string type, string text)
        {
            pluginStatusLabel.Visibility = Visibility.Visible;
            pluginStatusLabel.Content = text;

            switch (type)
            {
                case "Notify":
                    break;
                case "Warning":
                    warnIcon.Visibility = Visibility.Visible;
                    break;
                case "Error":
                    break;
            }
        }



        //Асихронное получение списка плагинов
        static async void GetListOfPluginsasync(ProgressBar progressBar)
        {
            progressBar.Visibility = Visibility.Visible;
            await Task.Run(() => Plugins.GetListOfPlugins());

            progressBar.Visibility = Visibility.Hidden;
        }

        private void ShowStatus(string Message, bool ShowPB)
        {
            statusGrid.Visibility = Visibility.Visible;
            if (ShowPB == false) pb.Visibility = Visibility.Collapsed;
            else pb.Visibility = Visibility.Visible;
            statusLabel.Content = Message;
        }

        private void HideStatus()
        {
            statusGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Скрыть сообщение для выбранного плагина
        /// </summary>
        private void MessageHide() 
        {
            pluginStatusLabel.Visibility = Visibility.Hidden;
            warnIcon.Visibility = Visibility.Hidden;
        }

        Color colorFore = (Color)ColorConverter.ConvertFromString("#FF949494");
        Color colorBack = (Color)ColorConverter.ConvertFromString("#FF4A4C4D");

        /// <summary>
        /// Событие при изменении выбранного плагина
        /// </summary>
        private void listArray_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                StackPanel1.Children.Clear();
                NameLabel.Content = Plugins.ListOfPlugins[listbox.SelectedIndex].FullName;
                

                if (Plugins.ListOfPlugins[listbox.SelectedIndex] is PluginVST)
                {
                    if (Plugins.ListOfPlugins[listbox.SelectedIndex].isX64 == false)
                        MessageShow("Warning", "Рекомендуется найти и установить 64-х разрядную версию плагина");
                    else
                        MessageHide();

                    
                    if (Plugins.ListOfPlugins[listbox.SelectedIndex].isVst3)
                        if (Plugins.ListOfPlugins[listbox.SelectedIndex].isSynth)
                            TypePluginLabel.Content = "VST3i";
                        else TypePluginLabel.Content = "VST3";
                    else if (Plugins.ListOfPlugins[listbox.SelectedIndex].isVst || Plugins.ListOfPlugins[listbox.SelectedIndex].isInternal)
                        if (Plugins.ListOfPlugins[listbox.SelectedIndex].isSynth)
                            TypePluginLabel.Content = "VSTi";
                        else TypePluginLabel.Content = "VST";
                    else TypePluginLabel.Content = "...";

                    TextBox[] txtboxes = new TextBox[3];

                    for (int i = 0; i < txtboxes.Length; i++)
                    {
                        var txtbox = new TextBox
                        {
                            FontSize = 18,
                            Foreground = new SolidColorBrush(colorFore),
                            Background = null,
                            BorderBrush = null,
                        };

                        Label label = new Label
                        {
                            FontSize = 14,
                            Foreground = new SolidColorBrush(colorFore),
                        };
                        switch (i)
                        {
                            case 0:
                                label.Content = "Заводское название";
                                StackPanel1.Children.Add(label);

                                txtbox.Text = Plugins.ListOfPlugins[listbox.SelectedIndex].description;
                                txtbox.Tag = "description";
                                txtbox.IsEnabled = false;
                                break;
                            case 1:
                                label.Content = "Версия .dll";
                                StackPanel1.Children.Add(label);

                                txtbox.Text = Plugins.ListOfPlugins[listbox.SelectedIndex].dllVersion;
                                txtbox.Tag = "dllVersion";
                                txtbox.IsEnabled = false;
                                break;
                            case 2:
                                label.Content = "Размещение";
                                StackPanel1.Children.Add(label);

                                txtbox.Text = Plugins.ListOfPlugins[listbox.SelectedIndex].FullPath;
                                txtbox.Tag = "Path";
                                txtbox.IsEnabled = false;
                                txtbox.Height = Double.NaN + 30;
                                txtbox.AcceptsReturn = true;
                                txtbox.TextWrapping = TextWrapping.Wrap;

                                break;
                        }
                        StackPanel1.Children.Add(txtbox);
                    }

                    CheckBox[] btns = new CheckBox[11];

                    for (int i = 0; i < btns.Length; i++)
                    {
                        var btn = new CheckBox
                        {
                            FontSize = 14,
                            Background = new SolidColorBrush(colorBack),
                            Foreground = new SolidColorBrush(colorFore),
                        };
                        switch (i)
                        {
                            case 0:
                                btn.Content = "Использовать как плагин";
                                btn.Tag = "registerAsPlug";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsPlug;
                                break;
                            case 1:
                                btn.Content = "Сконфигурировать как синтезатор";
                                btn.Tag = "registerAsSynth";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsSynth;
                                break;
                            case 2:
                                btn.Content = "Настроить для работы с темпом";
                                btn.Tag = "registerAsTempoBasedEffect";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsTempoBasedEffect;
                                break;
                            case 3:
                                btn.Content = "Принудительное стерео";
                                btn.Tag = "forceStereo";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].forceStereo;
                                break;
                            case 4:
                                btn.Content = "Не перехватывать NRPN";
                                btn.Tag = "nrpnPassThrough";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].nrpnPassThrough;
                                break;
                            case 5:
                                btn.Content = "Компенсировать задержку";
                                btn.Tag = "delayCompensation";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].delayCompensation;
                                break;
                            case 6:
                                btn.Content = "Последовательный доступ к хосту";
                                btn.Tag = "seralizeDispatcher";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].seralizeDispatcher;
                                break;
                            case 7:
                                btn.Content = "Разрешить монообработку";
                                btn.Tag = "forceMono";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].forceMono;
                                break;
                            case 8:
                                btn.Content = "Переводить смену банков/программ";
                                btn.Tag = "translateProgramChanges";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].translateProgramChanges;
                                break;
                            case 9:
                                btn.Content = "Приостанавливать при останове";
                                btn.Tag = "forceSuspendOnStop";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].forceSuspendOnPlay;
                                break;
                            case 10:
                                btn.Content = "Приостанавливать при воспроизведении";
                                btn.Tag = "forceSuspendOnPlay";
                                btn.IsChecked = Plugins.ListOfPlugins[listbox.SelectedIndex].forceSuspendOnPlay;
                                break;
                        }
                        btn.Checked += CheckBox_CheckedStateChanged;
                        btn.Unchecked += CheckBox_CheckedStateChanged;

                        StackPanel1.Children.Add(btn);
                    }
                }
                else
                {
                    TypePluginLabel.Content = "DX";
                }
            }
            catch (Exception) { } //при пересортировке индекс сползает, выскакивает ошибка

        }

        /// <summary>
        /// Обработчик события изменения параметров
        /// </summary>
        private void CheckBox_CheckedStateChanged(object sender, RoutedEventArgs e)
        {
            ShowStatus("Параметры изменены", false);
            CheckBox btn = sender as CheckBox;
            if (btn.IsChecked == true)
                switch (btn.Tag)
                {
                    case "registerAsPlug":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsPlug = true;
                        break;
                    case "registerAsSynth":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsSynth = true;
                        break;
                    case "registerAsTempoBasedEffect":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsTempoBasedEffect = true;
                        break;
                    case "forceStereo":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceStereo = true;
                        break;
                    case "nrpnPassThrough":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].nrpnPassThrough = true;
                        break;
                    case "delayCompensation":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].delayCompensation = true;
                        break;
                    case "seralizeDispatcher":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].seralizeDispatcher = true;
                        break;
                    case "forceMono":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceMono = true;
                        break;
                    case "translateProgramChanges":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].translateProgramChanges = true;
                        break;
                    case "forceSuspendOnStop":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceSuspendOnStop = true;
                        break;
                    case "forceSuspendOnPlay":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceSuspendOnPlay = true;
                        break;
                }
            if (btn.IsChecked == false)
                switch (btn.Tag)
                {
                    case "registerAsPlug":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsPlug = false;
                        break;
                    case "registerAsSynth":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsSynth = false;
                        break;
                    case "registerAsTempoBasedEffect":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].registerAsTempoBasedEffect = false;
                        break;
                    case "forceStereo":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceStereo = false;
                        break;
                    case "nrpnPassThrough":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].nrpnPassThrough = false;
                        break;
                    case "delayCompensation":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].delayCompensation = false;
                        break;
                    case "seralizeDispatcher":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].seralizeDispatcher = false;
                        break;
                    case "forceMono":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceMono = false;
                        break;
                    case "translateProgramChanges":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].translateProgramChanges = false;
                        break;
                    case "forceSuspendOnStop":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceSuspendOnStop = false;
                        break;
                    case "forceSuspendOnPlay":
                        Plugins.ListOfPlugins[listbox.SelectedIndex].forceSuspendOnPlay = false;
                        break;
                }
        }

        public string FilePathOpen { get; set; }
        public string FilePathSave { get; set; }

        /// <summary>
        /// Вызов диалога открытия
        /// </summary>
        private void OpenMenu()
        {
          
            if (OpenFileDialog() == true)
            {
                FilePathOpen = FilePathOpen.Substring(0, FilePathOpen.Length - 3) + "xml";
                System.IO.File.Move(FilePathOpen.Substring(0, FilePathOpen.Length - 3) + "pgl", FilePathOpen);
                BuildTree(treeview, XDocument.Load(FilePathOpen));
                FilePathOpen = FilePathOpen.Substring(0, FilePathOpen.Length - 3) + "pgl";
                System.IO.File.Move(FilePathOpen.Substring(0, FilePathOpen.Length - 3) + "xml", FilePathOpen);
                HighlightAddedPluginsToTreeviewRecursion((TreeViewItem)treeview.Items[0]);
            }
            
        }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Plug-in Menu Layouts (*.pgl)|*.pgl";
            openFileDialog.FilterIndex = 0;
            openFileDialog.DefaultExt = "pgl";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Cakewalk\Cakewalk Core\Plug-in Menu Layouts";
            if (openFileDialog.ShowDialog() == true)
            {
                FilePathOpen = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Построение дерева из файла
        /// </summary>
        private void BuildTree(TreeView treeView, XDocument doc)
        {

            treeview.Items.Clear();
            Color color = (Color)ColorConverter.ConvertFromString("#FFE9591D");

            TreeViewItem treeNode = new TreeViewItem
            {
                Header = doc.Root.Attributes().First(s => s.Name == "Name").Value,
                Foreground = new SolidColorBrush(color),
                IsExpanded = true,

            };
            menuNameEditText.Text = treeNode.Header.ToString();
            treeNode.Tag = "Folder";

            treeView.Items.Add(treeNode);
            BuildNodes(treeNode, doc.Root);
        }
 
        Color color = (Color)ColorConverter.ConvertFromString("#FFDED5C1");
        private void BuildNodes(TreeViewItem treeNode, XElement element)
        {

            foreach (XNode child in element.Nodes())
            {
                switch (child.NodeType)
                {
                    case XmlNodeType.Element:
                        XElement childElement = child as XElement;
                        TreeViewItem childTreeNode = null;

                        try
                        {
                            childTreeNode = new TreeViewItem
                            {
                                Header = childElement.Attributes().First(s => s.Name == "Name").Value.ToString(),
                                Foreground = new SolidColorBrush(color),
                                IsExpanded = true,
                            };

                            try
                            {
                                childTreeNode.Tag = childElement.Attributes().First(s => s.Name == "CLSID").Value.ToString() + @"|" +
                                childElement.Attributes().First(s => s.Name == "Category").Value.ToString() + @"|" +
                                childElement.Attributes().First(s => s.Name == "Name").Value.ToString() + @"|" +
                                childElement.Attributes().First(s => s.Name == "OriginalName").Value.ToString();

                                

                                treeNode.Items.Add(childTreeNode);
                                BuildNodes(childTreeNode, childElement);
                            }
                            catch (Exception)
                            {
                                childTreeNode.Header = (childElement.Attributes().First(s => s.Name == "Name").Value.ToString());
                                childTreeNode.Tag = "Folder";

                                treeNode.Items.Add(childTreeNode);
                                BuildNodes(childTreeNode, childElement);
                            }
                        }
                        catch (Exception)
                        {
                            treeNode.Items.Add(new TreeViewItem { Header = "---разделитель---", Tag = "Separator"});
                        }

                        break;
                }
            }
        }


        /// <summary>
        /// Вызов диалога сохранения
        /// </summary>
        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Plug-in Menu Layouts (*.pgl)|*.pgl";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.DefaultExt = "pgl";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Cakewalk\Cakewalk Core\Plug-in Menu Layouts";
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePathSave = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Сохранение меню
        /// </summary>
        XElement menuXML;
        private void SaveMenu()
        {
           
            if (SaveFileDialog() == true)
            {
                XDocument xdoc = new XDocument();
                menuXML = new XElement("PluginMenuLayout");

                TreeViewItem x = (TreeViewItem)treeview.Items[0];

                XAttribute docSchemaAttr = new XAttribute("DocSchema", "1");
                XAttribute namelocalAttr = new XAttribute("Name", x.Header.ToString());

                menuXML.Add(docSchemaAttr);
                menuXML.Add(namelocalAttr);

                foreach (TreeViewItem pmxnt in treeview.Items)
                    Obhod(pmxnt);

                xdoc.Add(menuXML);
                xdoc.Save(FilePathSave);

                File.Move(FilePathSave, FilePathSave.Substring(0, FilePathSave.Length - 3) + "txt");
                string line;
                using (StreamReader sr = new StreamReader(FilePathSave.Substring(0, FilePathSave.Length - 3) + "txt"))
                {
                    line = sr.ReadToEnd();
                }
                string podstroka = "<Folder-end />";
                string podstroka2 = "<Folder-begin";

                line = StrReplace(line, podstroka, "</Folder>");
                line = StrReplace2(line, podstroka2, "<Folder");
                Debug.WriteLine(line);

                File.WriteAllText(FilePathSave.Substring(0, FilePathSave.Length - 3) + "txt", line);

                File.Move(FilePathSave.Substring(0, FilePathSave.Length - 3) + "txt", FilePathSave.Substring(0, FilePathSave.Length - 3) + "pgl");
            }
            
        }

        private string StrReplace(string t, string f, string zam)
        {
            int pos;
            do
            {
                pos = t.IndexOf(f, StringComparison.Ordinal);
                if (pos == -1)
                    break;
                t = t.Remove(pos, f.Length).Insert(pos, zam.ToString(CultureInfo.InvariantCulture));
            } while (pos != -1);
            return t;
        }

        private string StrReplace2(string t, string f, string zam)
        {
            int pos;
            do
            {
                pos = t.IndexOf(f, StringComparison.Ordinal);
                if (pos == -1)
                    break;
                t = t.Remove(pos, f.Length).Insert(pos, zam.ToString(CultureInfo.InvariantCulture));
                int kek = t.IndexOf("/>", pos);
                if (kek != -1)
                    t = t.Remove(kek, 2).Insert(kek, ">");
            } while (pos != -1);
            return t;
        }

        void Obhod(TreeViewItem tw)
        {
            if (tw.Tag.ToString() == "Separator")
            {
                XElement unit = new XElement("Separator");
                menuXML.Add(unit);
            }
            else
            {
                TreeViewItem treew = (TreeViewItem)tw;
                foreach (TreeViewItem pmxnt in treew.Items)
                {
                    if (pmxnt.Tag.ToString() == "Separator")
                    {
                        XElement unit = new XElement("Separator");
                        menuXML.Add(unit);
                    }
                    else
                    {
                        TreeViewItem pmxnttt = (TreeViewItem)pmxnt;

                        bool thisisfolder = false;
                        if (pmxnttt.Tag.ToString() == "Folder")
                        {
                            XElement unitfoldbegin = new XElement("Folder-begin");
                            XAttribute nameFolderAttr = new XAttribute("Name", pmxnttt.Header.ToString());

                            unitfoldbegin.Add(nameFolderAttr);
                            menuXML.Add(unitfoldbegin);
                            thisisfolder = true;
                        }
                        else
                        {
                            XElement unit = new XElement("Plugin");

                            try
                            {
                                string attr = pmxnttt.Tag.ToString();
                                string[] atrrs = attr.Split(new char[] { '|' }, 4);

                                string clsidAttrStr = atrrs[0];
                                string categoryAttrStr = atrrs[1];
                                string nameAttrStr = atrrs[2];
                                string orignameAttrStr = atrrs[3];

                                XAttribute clsidAttr = new XAttribute("CLSID", clsidAttrStr);
                                XAttribute categoryAttr = new XAttribute("Category", categoryAttrStr);
                                XAttribute nameAttr = new XAttribute("Name", nameAttrStr);
                                XAttribute originalAttr = new XAttribute("OriginalName", orignameAttrStr);

                                unit.Add(clsidAttr);
                                unit.Add(categoryAttr);
                                unit.Add(nameAttr);
                                unit.Add(originalAttr);

                                menuXML.Add(unit);
                            }
                            catch (Exception) { }
                        }


                        Obhod(pmxnt);

                        if (thisisfolder == true)
                        {
                            XElement unitfoldbegin = new XElement("Folder-end");
                            menuXML.Add(unitfoldbegin);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Проверка наличия прав администратора
        /// </summary>
        public static bool IsAdmin()
        {
            System.Security.Principal.WindowsIdentity id = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal p = new System.Security.Principal.WindowsPrincipal(id);

            return p.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Запрос прав администратора и перезапуск приложения (при успешном получении прав администратора)
        /// </summary>
        private void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startInfo.Arguments = "restart " + this.Left + " "
                + this.Top + " " + this.Width + " " + this.Height;

            startInfo.Verb = "runas";
            try
            {
                Process p = Process.Start(startInfo);
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Отказано в доступе",
                                              "UAC control",
                                              MessageBoxButton.OK,
                                              MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик события нажатия кнопок сортировки списка всех плагинов
        /// </summary>
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.ToggleButton btn;
            btn = sender as System.Windows.Controls.Primitives.ToggleButton;

            Name_sort_tb.IsChecked = false;
            Developer_sort_tb.IsChecked = false;
            Type_sort_tb.IsChecked = false;

            if (btn == Name_sort_tb)
            {
                Name_sort_tb.IsChecked = true;
                Plugins.Sort_ABC();
                ShowStatus("Список плагинов отсортирован по имени", false);
            }
            else if (btn == Developer_sort_tb)
            {
                Developer_sort_tb.IsChecked = true;
                Plugins.Sort_Developer();
                ShowStatus("Список плагинов отсортирован по производителю", false);
            }
            else if (btn == Type_sort_tb)
            {
                Type_sort_tb.IsChecked = true;
                Plugins.Sort_Type();
                ShowStatus("Список плагинов отсортирован по типу", false);
            }
        }

        /// <summary>
        /// Обработчик события изменения текста в поисковой строке
        /// </summary>
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Plugins.Search(Search.Text);
        }

        /// <summary>
        /// Обработчик события нажатия на кнопку вручного перезапуска приложения
        /// </summary>
        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            RestartAsAdmin();
        }

        /// <summary>
        /// Обработчик события нажатия командных кнопок для управления меню
        /// </summary>
        private void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            switch (button.Name)
            {
                case "commandButtonCollapse":
                    ShowStatus("Элементы свернуты", false);
                    CollapseTreeViewItems((TreeViewItem)treeview.Items[0]);
                    break;
                case "commandButtonExpand":
                    ShowStatus("Элементы развернуты", false);
                    ExpandTreeViewItems((TreeViewItem)treeview.Items[0]);
                    break;
                case "commandButtonRemove":
                    ShowStatus("Плагин удален из меню", false);
                    RemoveTreeViewItem((TreeViewItem)treeview.Items[0]);
                    HighlightPluginsIncluded();
                    StateOnlyIncluded_Click(this, new RoutedEventArgs());
                    break;
                case "commandButtonOpenMenu":
                    OpenMenu();
                    
                    break;
                case "commandButtonSaveMenu":
                    SaveMenu();
                    break;
                case "commandButtonCreateFolder":
                    ShowStatus("Новая папка создана в меню", false);
                    if (((TreeViewItem)treeview.Items[0]).IsSelected && ((TreeViewItem)treeview.Items[0]).Tag.ToString() == "Folder")
                        ((TreeViewItem)treeview.Items[0]).Items.Add(new TreeViewItem
                        {
                            Header = "New Folder",
                            Foreground = new SolidColorBrush(color),
                            IsExpanded = true,
                            Tag = "Folder"
                        });
                    else
                        CreateFolder((TreeViewItem)treeview.Items[0]);
                    break;
                case "commandButtonAddSeparator":
                    ShowStatus("Разделитель добавлен в меню", false);
                    if (((TreeViewItem)treeview.Items[0]).IsSelected && ((TreeViewItem)treeview.Items[0]).Tag.ToString() == "Folder")
                        ((TreeViewItem)treeview.Items[0]).Items.Add(new TreeViewItem { Header = "--разделитель---", Tag = "Separator" });
                    else
                        AddSeparator((TreeViewItem)treeview.Items[0]);
                    break;
                case "commandButtonNewMenu":
                    ShowStatus("Новое меню создано", false);
                    NewMenu();
                    HighlightPluginsIncluded();
                    StateOnlyIncluded_Click(this, new RoutedEventArgs());
                    break;               
            }
        }

        /// <summary>
        /// Обработчик события нажатия кнопок в окне параметров меню
        /// </summary>
        private void menuSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            switch (button.Name)
            {
                case "menuSettingsShowHide":
                    ShowHide();
                    break;
                case "menuSettingsButtonApply":
                    Apply();
                    break;
            }

            void ShowHide()
            {
                if (SettingsMenuPanel.Visibility == Visibility.Visible)
                    SettingsMenuPanel.Visibility = Visibility.Hidden;
                else SettingsMenuPanel.Visibility = Visibility.Visible;
            }

            void Apply()
            {
                ((TreeViewItem)treeview.Items[0]).Header = menuNameEditText.Text;
                ShowHide();
            }
        }

        /// <summary>
        /// Подсветка плагинов в списке всех плагинов, уже включенных в меню
        /// </summary>
        void HighlightAddedPluginsToTreeviewRecursion(TreeViewItem item)
        {
            for (int k = 0; k < item.Items.Count; k++)
                try
                {
                    if (((TreeViewItem)item.Items[k]).HasItems)
                    {
                        HighlightAddedPluginsToTreeviewRecursion(((TreeViewItem)item.Items[k]));
                    }
                    else
                    {
                        string attr = ((TreeViewItem)item.Items[k]).Tag.ToString();
                        string[] atrrs = attr.Split(new char[] { '|' }, 4);

                        string clsidAttrStr = atrrs[0];
                        string orignameAttrStr = atrrs[3];

                        for (int b = 0; b < Plugins.ListOfPlugins.Count; b++)
                            if (Plugins.ListOfPlugins[b].CLSID == clsidAttrStr.ToString() && Plugins.ListOfPlugins[b].description == orignameAttrStr.ToString())
                            {
                                ((ListBoxItem)listbox.Items[b]).Background = new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + "/Images/activate.png")));
                            }
                    }

                }
                catch (Exception) { };
        }


        private void HighlightPluginsIncluded()
        {
            for (int b = 0; b < Plugins.ListOfPlugins.Count; b++)
            {
                ((ListBoxItem)listbox.Items[b]).Background = new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory.ToString() + "/Images/none.png")));
            }

            HighlightAddedPluginsToTreeviewRecursion((TreeViewItem)treeview.Items[0]);
        }


        /// <summary>
        /// Создать новое меню
        /// </summary>
        void NewMenu()
        {
            treeview.Items.Clear();
            menuNameEditText.Text = "Новое меню";
            Color color2 = (Color)ColorConverter.ConvertFromString("#FFE9591D");

            TreeViewItem treeNode = new TreeViewItem
            {
                Header = "Новое меню",
                Foreground = new SolidColorBrush(color2),
                IsExpanded = true,
                Tag = "Folder"
            };
            treeNode.IsSelected = true;
            treeview.Items.Add(treeNode);
            HighlightPluginsIncluded();
            StateOnlyIncluded_Click(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Добавить разделитель
        /// </summary>
        void AddSeparator(TreeViewItem item)
        {
            for (int k = 0; k < item.Items.Count; k++)
                try
                {
                    if (((TreeViewItem)item.Items[k]).HasItems)
                    {
                        if (((TreeViewItem)item.Items[k]).IsSelected && ((TreeViewItem)item.Items[k]).Tag.ToString() == "Folder")
                            ((TreeViewItem)item.Items[k]).Items.Add(new TreeViewItem { Header = "--разделитель---", Tag = "Separator" });
                        else
                            AddSeparator(((TreeViewItem)item.Items[k]));
                    }
                    else
                    if (((TreeViewItem)item.Items[k]).IsSelected && ((TreeViewItem)item.Items[k]).Tag.ToString() == "Folder")
                        ((TreeViewItem)item.Items[k]).Items.Add(new TreeViewItem { Header = "--разделитель---", Tag = "Separator" });
                }
                catch (Exception) { };
        }


        /// <summary>
        /// Создать папку
        /// </summary>
        void CreateFolder(TreeViewItem item)
        {
            for (int k = 0; k < item.Items.Count; k++)
                try
                {
                    if (((TreeViewItem)item.Items[k]).HasItems)
                        if (((TreeViewItem)item.Items[k]).IsSelected && ((TreeViewItem)item.Items[k]).Tag.ToString() == "Folder")
                            ((TreeViewItem)item.Items[k]).Items.Add(new TreeViewItem
                            {
                                Header = "New Folder",
                                Foreground = new SolidColorBrush(color),
                                IsExpanded = true,
                                Tag = "Folder"
                            });
                        else
                            CreateFolder(((TreeViewItem)item.Items[k]));
                    else
                    if (((TreeViewItem)item.Items[k]).IsSelected && ((TreeViewItem)item.Items[k]).Tag.ToString() == "Folder")
                    {
                        ((TreeViewItem)item.Items[k]).Items.Add(new TreeViewItem
                        {
                            Header = "New Folder",
                            Foreground = new SolidColorBrush(color),
                            IsExpanded = true,
                            Tag = "Folder"
                        });
                    }
                }
                catch (Exception) { }
        }

        /// <summary>
        /// Удалить элемент дерева
        /// </summary>
        void RemoveTreeViewItem(TreeViewItem item)
        {
            for (int k = 0; k < item.Items.Count; k++)
                try
                {
                    if (((TreeViewItem)item.Items[k]).HasItems)
                    {
                        if (((TreeViewItem)item.Items[k]).IsSelected)
                            item.Items.Remove(item.Items[k]);
                        else
                            RemoveTreeViewItem(((TreeViewItem)item.Items[k]));
                    }
                    else
                        if (((TreeViewItem)item.Items[k]).IsSelected)
                        item.Items.Remove(item.Items[k]);
                }
                catch (Exception) { };
            
        }

        /// <summary>
        /// Развернуть все папки
        /// </summary>
        void ExpandTreeViewItems(TreeViewItem item)
        {
            for (int k = 0; k < item.Items.Count; k++)
                try
                {
                    if (((TreeViewItem)item.Items[k]).HasItems)
                    {
                        ((TreeViewItem)item.Items[k]).IsExpanded = true;
                        ExpandTreeViewItems(((TreeViewItem)item.Items[k]));
                    }

                }
                catch (Exception) { };
        }

        /// <summary>
        /// Свернуть все папки
        /// </summary>
        void CollapseTreeViewItems(TreeViewItem item)
        {
            for (int k = 0; k < item.Items.Count; k++)
                try
                {
                    if (((TreeViewItem)item.Items[k]).HasItems)
                    {
                        ((TreeViewItem)item.Items[k]).IsExpanded = false;
                        CollapseTreeViewItems(((TreeViewItem)item.Items[k]));
                    }

                }
                catch (Exception) { };
        }

        /// <summary>
        /// Показать только те плагины, которые не включены в текущее меню
        /// </summary>
        void StatePluginsIncludedInMenu(TreeViewItem item)
        {
            if (stateOnlyIncluded.IsChecked == true)
            {
                for (int k = 0; k < item.Items.Count; k++)
                    try
                    {
                        if (((TreeViewItem)item.Items[k]).Tag.ToString() == "Folder")
                        {
                            StatePluginsIncludedInMenu((TreeViewItem)treeview.Items[k]);
                        }
                        else
                        {
                            string attr = ((TreeViewItem)item.Items[k]).Tag.ToString();
                            string[] atrrs = attr.Split(new char[] { '|' }, 4);

                            string clsidAttrStr = atrrs[0];
                            string orignameAttrStr = atrrs[3];

                            for (int b = 0; b < Plugins.ListOfPlugins.Count; b++)
                                if (Plugins.ListOfPlugins[b].CLSID == clsidAttrStr.ToString())
                                {
                                    ((ListBoxItem)listbox.Items[b]).Visibility = Visibility.Collapsed;
                                   // break;

                                }
                            

                        }

                    }
                    catch (Exception) { };
            }
            else
            {
                for (int b = 0; b < Plugins.ListOfPlugins.Count; b++)
                    ((ListBoxItem)listbox.Items[b]).Visibility = Visibility.Visible;
            }
        }

        private void StateOnlyIncluded_Click(object sender, RoutedEventArgs e)
        {
            for (int b = 0; b < Plugins.ListOfPlugins.Count; b++)
                    ((ListBoxItem)listbox.Items[b]).Visibility = Visibility.Visible;
                    StatePluginsIncludedInMenu((TreeViewItem)treeview.Items[0]);
        }



        bool _isDragging = false;

        void treeViewl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && _isDragging == true)
                _isDragging = false;
            if (!_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;

                try
                {
                    TreeViewItem tvi = treeview.SelectedValue as TreeViewItem;
                    if (tvi.Parent != null)
                        DragDrop.DoDragDrop(tvi.Parent, tvi, DragDropEffects.Move);
                    else
                        DragDrop.DoDragDrop(treeview, tvi, DragDropEffects.Move);
                }
                catch (Exception) { };
            }
        }

        //drag&drop
        void treeViewl_DragOver(object sender, DragEventArgs e)
        {
            if (_isDragging)
            {
                if (e.Data.GetDataPresent(typeof(TreeViewItem)))
                    e.Effects = DragDropEffects.Move;
                else
                    e.Effects = DragDropEffects.None;
            }
        }

        void treeViewl_Drop(object sender, DragEventArgs e)
        {
            if (_isDragging)
            {
                TreeViewItem treeViewItem = e.Source as TreeViewItem;

                TreeViewItem obj = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;

                if (treeViewItem != null)
                    if (treeViewItem.Tag.ToString() == "Folder")
                        try
                        {
                            if ((obj.Parent as TreeViewItem) != null)
                            {
                                (obj.Parent as TreeViewItem).Items.Remove(obj);
                            }
                            treeViewItem.Items.Remove(obj); treeViewItem.Items.Insert(0, obj); e.Handled = true;
                            _isDragging = false;
                            treeViewItem.IsSelected = true;
                        }
                        catch (Exception) { };
                _isDragging = false;
            }
        }


    }
}