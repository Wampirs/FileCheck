using FileCheck.Models;
using FileCheck.ViewModels.Base;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;

namespace FileCheck.ViewModels
{
    public class MainVM : BaseVM
    {
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
        private void OnCreateTemplateExecuted(object o)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult res = dialog.ShowDialog();
            if (res != CommonFileDialogResult.Ok) return;

            string path = dialog.FileName;
            var template = GetTemplate(path);

            TemplatePreviewVM vm = new TemplatePreviewVM
            {
                Template = template
            };
            CustomDialog.ShowDialog(vm);
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
        private void OnSelectPathExecuted(object o)
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
                    SelectedDir = GetTemplate(dialog.FileName);
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
        private void OnCheckTemplateExecuted(object o)
        {
            CheckResult checkResult;
            if (PathToTemplate == null && PathToDirectory == null)
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
                using (FileStream sr = new FileStream(PathToTemplate, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FsTemplate));
                    FsTemplate fileTemplate = serializer.Deserialize(sr) as FsTemplate;
                    if (fileTemplate == null)
                    {
                        MessageBox.Show("Невдалося відновити шаблон");
                        return;
                    }
                    checkResult = SelectedDir.Check(fileTemplate);
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
            CustomDialog.ShowDialog(checkResult);

        }
        private bool CanCheckTemplateExecute(object o) => true;
        #endregion

        private FsTemplate GetTemplate(string path)
        {
            List<string> directories = Directory.GetDirectories(path, "", SearchOption.AllDirectories).ToList();
            List<string> files = Directory.GetFiles(path, "", SearchOption.AllDirectories).ToList();
            FsTemplate template = new FsTemplate();

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
                template.Items.Add(item);
                template.FileCount++;
            }
            return template;
        }
    }
}
