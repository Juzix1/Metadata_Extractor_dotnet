using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MODEL.Plugins;
using System.Collections;
using CoreLibrary;

namespace MetaDataLibrary {
    public partial class MainWindow : Window {
        private readonly PluginLoader _pluginLoader;
        private IMetadataPlugin _selectedPlugin;

        public MainWindow() {
            InitializeComponent();
            _pluginLoader = new PluginLoader();
            LoadPlugins();
        }

        private void LoadPlugins() {
            try {
                _pluginLoader.LoadPlugins();
                pluginsMenu.Items.Clear();

                foreach (var plugin in _pluginLoader.Plugins) {
                    var menuItem = new MenuItem {
                        Header = plugin.Name,
                        Tag = plugin
                    };
                    menuItem.Click += PluginMenuItem_Click;
                    pluginsMenu.Items.Add(menuItem);
                }

                // Domyślny wybór pierwszego pluginu
                if (_pluginLoader.Plugins.Any()) {
                    _selectedPlugin = _pluginLoader.Plugins.First();
                    //MessageBox.Show($"Default plugin selected: {_selectedPlugin.Name}");
                }
            } catch (Exception ex) {
                Debug.WriteLine($"Error loading plugins: {ex.Message}");
                MessageBox.Show("Error loading plugins. Check logs for details.");
            }
        }

        private void PluginMenuItem_Click(object sender, RoutedEventArgs e) {
            if (sender is MenuItem menuItem && menuItem.Tag is IMetadataPlugin plugin) {
                _selectedPlugin = plugin;
                MessageBox.Show($"Selected plugin: {_selectedPlugin.Name}");
            }
        }

        private async void SearchFile(object sender, RoutedEventArgs e) {
            if (_selectedPlugin == null) {
                MessageBox.Show("No plugin selected!");
                return;
            }
            var extension = _selectedPlugin.Name;
            string[] subs = extension.Split(' ');
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = $"{subs[0]} files (*.{subs[0]})|*.{subs[0]}"

            };

            if (openFileDialog.ShowDialog() == true) {
                filePath.Text = openFileDialog.FileName;
            }

            if (string.IsNullOrEmpty(filePath.Text) || !File.Exists(filePath.Text)) {
                MessageBox.Show("Invalid file path!");
                return;
            }

            try {
                dllInfoTree.Items.Clear();
                var plugin = await _selectedPlugin.Read(filePath.Text);
                var result = _selectedPlugin.GetAssemblyInfo();

                if (result is AssemblyInfo assemblyInfo) {
                    var rootNode = ConvertAssemblyToTreeViewItem(assemblyInfo);
                    dllInfoTree.Items.Add(rootNode);
                }


            } catch (Exception ex) {
                Debug.WriteLine($"Error processing file: {ex.Message}");
                MessageBox.Show($"Error processing file: {ex.Message}");
            }
        }

        private void RefreshView(object sender, RoutedEventArgs e) {
            dllInfoTree.Items.Clear();
            filePath.Clear();
        }

        private TreeViewItem ConvertAssemblyToTreeViewItem(AssemblyInfo assemblyInfo) {
            var assemblyNode = new TreeViewItem {
                Header = $"{assemblyInfo.Name}"
            };

            foreach (var type in assemblyInfo.Types) {
                var typeNode = new TreeViewItem {
                    Header = $"{type.TypeName}"
                };

                foreach (var method in type.Methods) {
                    var methodNode = new TreeViewItem {
                        Header = $"{method.MethodName}"
                    };
                    typeNode.Items.Add(methodNode);
                }
                assemblyNode.Items.Add(typeNode);

            }
            return assemblyNode;
        }
    }
}
