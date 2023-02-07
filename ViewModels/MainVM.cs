using FileCheck.Models;
using FileCheck.ViewModels.Base;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace FileCheck.ViewModels
{
    public class MainVM : BaseVM
    {

        private bool useHash;
        public bool UseHash
        {
            get => useHash;
            set => Set(ref useHash, value);
        }

        private string pathToDirectory;
        public string PathToDirectory
        {
            get => pathToDirectory;
            set
            {
                pathToDirectory = value;
                OnPropertyChanged();
            }
        }

        private string pathToTemplate;
        public string PathToTemplate
        {
            get => pathToTemplate;
            set
            {
                pathToTemplate = value;
                OnPropertyChanged();
            }
        }

        private FsTemplate selectedDir;
        public FsTemplate SelectedDir
        {
            get => selectedDir;
            set => Set(ref selectedDir, value);
        }


        #region CreateTemplateRegion
        private ICommand createTemplate;
        public ICommand CreateTemplate
        {
            get
            {
                if (createTemplate == null) createTemplate = new RelayCommand(OnCreateTemplateExecuted, CanCreateTemplateExecute);
                return createTemplate;
            }
        }
        private async void OnCreateTemplateExecuted(object o)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult res = dialog.ShowDialog();
            if (res != CommonFileDialogResult.Ok) return;

            string path = dialog.FileName;
            var template = await GetTemplate(path,true);

            TemplatePreviewVM vm = new TemplatePreviewVM
            {
                Template = template
            };
            new CustomDialog().ShowDialog(vm);
        }
        private bool CanCreateTemplateExecute(object o) => true;
        #endregion

        #region SelectPathCommand
        private ICommand selectPath;
        public ICommand SelectPath
        {
            get
            {
                if (selectPath == null) selectPath = new RelayCommand(OnSelectPathExecuted, CanSelectPathExecute);
                return selectPath;
            }
        }
        private async void OnSelectPathExecuted(object o)
        {
            Button? button = o as Button;
            if (button == null) return;

            var dialog = new CommonOpenFileDialog();
            dialog.Multiselect = false;
            if (button.Name == "SelectTemplate")
            {
                dialog.IsFolderPicker = false;
                dialog.Filters.Add(new CommonFileDialogFilter("файл шаблону", "*.xml"));
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else dialog.IsFolderPicker = true;
            CommonFileDialogResult res = dialog.ShowDialog();

            if (res == CommonFileDialogResult.Ok)
            {
                if (button.Name == "SelectDirectory")
                {
                    PathToDirectory = dialog.FileName;
                    SelectedDir = await GetTemplate(dialog.FileName, false);
                }
                if (button.Name == "SelectTemplate") PathToTemplate = dialog.FileName;
            }
        }
        private bool CanSelectPathExecute(object o) => true;
        #endregion

        #region CheckTemplate
        private ICommand checkTemplate;
        public ICommand CheckTemplate
        {
            get
            {
                if (checkTemplate == null) checkTemplate = new RelayCommand(OnCheckTemplateExecuted, CanCheckTemplateExecute);
                return checkTemplate;
            }
        }
        private async void OnCheckTemplateExecuted(object o)
        {

            CheckResult checkResult;
            if (PathToTemplate == null || PathToDirectory == null)
            {
                MessageBox.Show("Не обрано шлях до папки або шаблону");
                return;
            }
            if (!File.Exists(PathToTemplate))
            {
                MessageBox.Show("Вибраний файл недоступний ");
                return;
            }
            if (!Directory.Exists(PathToDirectory))
            {
                MessageBox.Show("Вибрана папка недоступна");
                return;
            }
            try
            {
                SelectedDir = await GetTemplate(PathToDirectory,UseHash);
                using (FileStream sr = new FileStream(PathToTemplate, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FsTemplate));
                    FsTemplate fileTemplate = serializer.Deserialize(sr) as FsTemplate;
                    if (fileTemplate == null)
                    {
                        MessageBox.Show("Невдалося відновити шаблон");
                        return;
                    }
                    checkResult = SelectedDir.Check(fileTemplate,UseHash);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка при перевірці");
                MessageBox.Show(ex.Message);
                return;
            }
            if(checkResult.IsValid)
            {
                MessageBox.Show("Перевірку пройдено\nПапка відповідає наданому шаблону");
                return;
            }
            new CustomDialog().ShowDialog(checkResult);

        }
        private bool CanCheckTemplateExecute(object o) => true;
        #endregion

        private async Task<FsTemplate> GetTemplate(string path, bool useHash)
        {
            if (useHash)
            {
                WaitWindow.ShowWaiter();
            }
            FsTemplate template = new FsTemplate();

            Task compute = new Task(() =>
            {
                List<string> directories = Directory.GetDirectories(path, "", SearchOption.AllDirectories).ToList();
                List<string> files = Directory.GetFiles(path, "", SearchOption.AllDirectories).ToList();

                foreach (string dir in directories)
                {
                    FsItem item = new FsItem();
                    item.IsFolder = true;
                    item.Path = Path.GetRelativePath(path, dir);
                    item.Name = Path.GetFileName(dir);
                    template.Items.Add(item);
                    template.DirCount++;
                }
                foreach (string file in files)
                {
                    FsItem item = new FsItem();
                    item.IsFolder = false;
                    item.Name = Path.GetFileName(file);
                    item.Path = Path.GetRelativePath(path, file);
                    if (useHash)
                        try
                        {
                            using (FileStream fs = new FileStream(file, FileMode.Open))
                                item.Hash = GetMD5Hash(fs);
                        } 
                        catch (Exception ex)
                        {
                            MessageBox.Show("Сталася помилка при читанні файлів");
                            MessageBox.Show(ex.Message);
                            return;
                        }
                    template.Items.Add(item);
                    template.FileCount++;
                }
            });
            compute.Start();
            await compute;
            WaitWindow.HideWaiter();
            return template;
        }

        private string GetMD5Hash(Stream s)
        {
            using (var md5 = MD5.Create())
            {
                md5.ComputeHash(s);
                return BitConverter.ToString(md5.Hash).Replace("-", "").ToLower();
            }
        }
    }
}
