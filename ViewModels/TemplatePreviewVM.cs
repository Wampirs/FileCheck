using FileCheck.Models;
using FileCheck.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace FileCheck.ViewModels
{
    internal class TemplatePreviewVM : BaseVM
    {
        private FsTemplate template;
        public FsTemplate Template
        {
            get => template;
            set => Set(ref template, value);
        }


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
            var dialog = new SaveFileDialog();
            dialog.Filter = "файл шаблону (*.xml)|*.xml";
            dialog.FilterIndex = 0;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (dialog.ShowDialog() == true)
            {
                using (var sw = new FileStream(dialog.FileName, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FsTemplate));
                    serializer.Serialize(sw, template);
                    MessageBox.Show("Шаблон збережено");
                }
            }
        }
        private bool CanCreateTemplateExecute(object o) => true;


    }
}
