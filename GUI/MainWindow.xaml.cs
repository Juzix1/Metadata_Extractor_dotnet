using Castle.Core.Logging;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Windows;
using LoggingLibrary;
using CoreLibrary;
using MODEL;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace MetaDataLibrary
{

    public partial class MainWindow : Window
    {
        private readonly LoggingLibrary.ILogger _logger;
        MetadataExtractor metadataExtractor;
        public MainWindow()
        {
            InitializeComponent();
            _logger = new Logger();
        }
        System.Windows.Controls.Button saveButton;



        private async void SearchFile(object sender, RoutedEventArgs e)
        {
            resetSaveButton();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DLL files (*.dll)|*.dll|EXE files (*.exe)|*.exe";

            if (openFileDialog.ShowDialog() == true)
            {
                filePath.Text = openFileDialog.FileName;
            }

            try
            {
                metadataExtractor = new MetadataExtractor(_logger);
                AssemblyInfo assemblyInfo = await metadataExtractor.ExtractMetadataAsync(filePath.Text);
                dllInfo.Items.Clear();

                foreach(var type in assemblyInfo.Types)
                {
                    dllInfo.Items.Add($"Type: {type.TypeName}");

                    foreach(var method in type.Methods) {
                        dllInfo.Items.Add($"  Method: {method.MethodName}");

                    }
                }
                    saveButton = new System.Windows.Controls.Button() {
                    Content = string.Format("Save to File"),
                    Tag = "ADD",

                };

                saveButton.Click += new RoutedEventHandler(saveToRepository);

                saveStack.Children.Add(saveButton);




                
            }catch(Exception se)
            {
                await _logger.LogErrorAsync();
                MessageBox.Show($"Error in reading the DLL file: {se.Message}");
            }
        }

        private void RefreshView(object sender, RoutedEventArgs e)
        {

            dllInfo.Items.Clear();
            filePath.Clear();
            resetSaveButton();
        }
        private void saveToRepository(object sender, RoutedEventArgs e) {
            saveButton.IsEnabled = false;


            try {
                SaveTo save = new SaveTo(_logger);
                save.XmlFile(metadataExtractor.getDataList());
            } catch {
                Debug.WriteLine("test");
            }
        }
        public void resetSaveButton() {
            saveButton = null;
            saveStack.Children.Clear();
        }
    }
}
