using System;
using System.Drawing;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace HardwareMonitor
{
    public partial class Form2 : Form
    {
        private ComputerInfo computerInfo;
        private Color backgroundColor = Color.FromArgb(40, 40, 40);
        private Color textColor = Color.FromArgb(220, 220, 220);
        private Color headerColor = Color.FromArgb(0, 120, 215);
        private Font headerFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        private Font contentFont = new Font("Segoe UI", 9F);

        public Form2()
        {
            InitializeComponent();
            // Empêcher le redimensionnement de la fenêtre
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Désactiver le bouton Maximiser
            this.MaximizeBox = false;

            // (Facultatif) Désactiver le bouton Agrandir (Alt + Space)
            this.MinimizeBox = true; // Laisser activé ou désactivé selon votre besoin
            this.FormClosing += MainForm_FormClosing;
            SetupCustomUI();
            computerInfo = new ComputerInfo();
            this.BackColor = backgroundColor;
            this.ForeColor = textColor;

            // Actualisation des informations au démarrage
            UpdateCPUInfo();
            UpdateRAMInfo();
            UpdateGPUInfo();
            UpdateSystemInfo();
            UpdateDirectXInfo();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            // Afficher Form2 pendant que Form2 se charge
            Form2 loadingForm = new Form2();
            loadingForm.Show();

            // Attendre que les tâches se terminent
            await LoadForm2Async();

            // Fermer Form2 une fois que Form2 est prêt
            loadingForm.Invoke(new Action(() => loadingForm.Close()));
        }

        private async Task LoadForm2Async()
        {
            // Simuler une tâche longue comme le chargement de Form2
            await Task.Delay(5000); // Utilisez Task.Delay au lieu de Thread.Sleep pour ne pas bloquer l'UI

            // Vous pouvez aussi charger des ressources ici, par exemple :
            // - Connexion à une base de données
            // - Lecture de fichiers
            // - Initialisation des composants complexes
        }


        private void LoadForm2()
        {
            // Simuler une tâche longue comme le chargement de Form2
            Thread.Sleep(5000); // Simule un délai de 5 secondes

            // Vous pouvez aussi charger des ressources ici, par exemple :
            // - Connexion à une base de données
            // - Lecture de fichiers
            // - Initialisation des composants complexes
        }

        private void SetupCustomUI()
        {
            this.Size = new Size(800, 600);
            this.Text = "System Information HW - HardwareMonitor by o2Cloud";
            this.MinimumSize = new Size(800, 600);

            TabControl tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(100, 30);
            tabControl.SizeMode = TabSizeMode.Fixed;

            CreateTabPages(tabControl);

            this.Controls.Add(tabControl);

            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += TabControl_DrawItem;
        }

        private void CreateTabPages(TabControl tabControl)
        {
            string[] tabNames = { "CPU", "RAM", "GPU", "System", "DirectX" };
            foreach (string tabName in tabNames)
            {
                TabPage tabPage = new TabPage(tabName);
                tabPage.BackColor = backgroundColor;

                Panel infoPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10),
                    BackColor = backgroundColor
                };

                TableLayoutPanel tableLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 0,
                    AutoSize = true,
                    AutoScroll = true,
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                    BackColor = Color.FromArgb(255, 186, 254, 255) // Couleur des entre colone
                };

                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

                infoPanel.Controls.Add(tableLayout);
                tabPage.Controls.Add(infoPanel);
                tabControl.TabPages.Add(tabPage);

                tableLayout.Tag = tabName;
                tableLayout.Name = $"{tabName.ToLower()}Layout";
            }
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle tabBounds = tabControl.GetTabRect(e.Index);

            Color selectedTabColor = Color.FromArgb(60, 60, 60);
            Color unselectedTabColor = Color.FromArgb(45, 45, 45);

            using (SolidBrush brush = new SolidBrush(e.State == DrawItemState.Selected ? selectedTabColor : unselectedTabColor))
            {
                e.Graphics.FillRectangle(brush, tabBounds);
            }

            string tabText = tabPage.Text;
            Color textColor;

            // Définir la couleur du texte en fonction du nom de l'onglet
            switch (tabText)
            {
                case "CPU":
                    textColor = Color.Red; // Exemple de couleur pour "CPU"
                    break;
                case "RAM":
                    textColor = Color.Green; // Exemple de couleur pour "RAM"
                    break;
                case "GPU":
                    textColor = Color.Blue; // Exemple de couleur pour "GPU"
                    break;
                case "System":
                    textColor = Color.Orange; // Exemple de couleur pour "System"
                    break;
                case "DirectX":
                    textColor = Color.Purple; // Exemple de couleur pour "DirectX"
                    break;
                default:
                    textColor = Color.White; // Couleur par défaut
                    break;
            }

            // Dessiner le texte avec la couleur spécifiée
            using (SolidBrush brush = new SolidBrush(textColor))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString(tabText, headerFont, brush, tabBounds, sf);
            }
        }
        private Color firstColumnColor = Color.FromArgb(255, 108, 255, 255);
        private Color secondColumnColor = Color.FromArgb(255, 80, 160, 255);
        private Color textColor2 = Color.FromArgb(255, 80, 160, 255);
        private Color textColor3 = Color.FromArgb(255, 255, 255, 255);
        private void AddInfoRow(TableLayoutPanel table, string label, string value)
        {
            // Si le contrôle doit être invoqué sur le thread UI, utilisez Invoke
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddInfoRow(table, label, value)));
                return;
            }

            // Ajoutez une nouvelle ligne à la TableLayoutPanel
            int rowIndex = table.RowCount++;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Créez un contrôle Label pour le label (nom de l'information)
            Label labelControl = new Label
            {
                Text = label,
                Dock = DockStyle.Fill,
                Font = contentFont,
                ForeColor = textColor2,
                BackColor = backgroundColor,
                Padding = new Padding(5),
                AutoSize = true
            };

            // Créez un contrôle Label pour la valeur de l'information
            Label valueControl = new Label
            {
                Text = value,
                Dock = DockStyle.Fill,
                Font = contentFont,
                ForeColor = textColor3,
                BackColor = backgroundColor,
                Padding = new Padding(5),
                AutoSize = true
            };

            // Ajouter les contrôles à la TableLayoutPanel dans la ligne spécifiée
            table.Controls.Add(labelControl, 0, rowIndex);
            table.Controls.Add(valueControl, 1, rowIndex);
        }
        private void ClearTable(TableLayoutPanel table)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ClearTable(table)));
                return;
            }

            table.Controls.Clear();
            table.RowStyles.Clear();
            table.RowCount = 0;
        }



        private async Task UpdateCPUInfo()
        {
            var table = (TableLayoutPanel)Controls.Find("cpuLayout", true)[0];
            ClearTable(table);

            try
            {
                // Informations sur le processeur
                AddInfoRow(table, "Section", "== Informations Processeur ==");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Processeur", obj["Name"].ToString());
                        AddInfoRow(table, "Fabricant", obj["Manufacturer"].ToString());
                        AddInfoRow(table, "Nombre de cœurs", obj["NumberOfCores"].ToString());
                        AddInfoRow(table, "Threads logiques", obj["NumberOfLogicalProcessors"].ToString());
                        AddInfoRow(table, "Architecture", $"{obj["AddressWidth"]} bits");
                        AddInfoRow(table, "Fréquence de base", $"{obj["MaxClockSpeed"]} MHz");
                        AddInfoRow(table, "Socket", obj["SocketDesignation"].ToString());
                        AddInfoRow(table, "Cache L2", $"{obj["L2CacheSize"]} Ko");
                        AddInfoRow(table, "Cache L3", $"{obj["L3CacheSize"]} Ko");
                    }
                }

                // Informations de performance
                AddInfoRow(table, "Performances CPU en temp reel", "== Performances CPU à l'ouverture de Hardware Monitor valeur fixe  ==");

                // Démarrage de la mise à jour périodique
                await UpdateCpuUsagePeriodically(table);
            }
            catch (Exception ex)
            {
                AddInfoRow(table, "Erreur", ex.Message);
            }
        }

        private async Task UpdateCpuUsagePeriodically(TableLayoutPanel table)
        {
            try
            {
                // Mise à jour de l'utilisation du CPU toutes les 3 secondes
                while (true)
                {
                    await UpdateCpuUsage(table);
                    await Task.Delay(3000); // Attente de 3 secondes avant la prochaine mise à jour
                }
            }
            catch (Exception ex)
            {
                AddInfoRow(table, "Erreur", ex.Message);
            }
        }

        private async Task UpdateCpuUsage(TableLayoutPanel table)
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name='_Total'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        var existingControl = table.Controls.OfType<Control>()
                            .FirstOrDefault(c => c is Label && c.Text.Contains("Utilisation CPU"));

                        if (existingControl != null)
                        {
                            // Mise à jour de l'utilisation CPU si elle existe déjà
                            existingControl.Text = $"Utilisation CPU : {obj["PercentProcessorTime"]}%";
                        }
                        else
                        {
                            // Sinon, ajoutez une nouvelle ligne
                            AddInfoRow(table, "Utilisation CPU", $"{obj["PercentProcessorTime"]}%");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddInfoRow(table, "Erreur", ex.Message);
            }
        }


        private void UpdateDirectXInfo()
        {
            var table = (TableLayoutPanel)Controls.Find("directxLayout", true)[0];
            ClearTable(table);

            try
            {
                AddInfoRow(table, "Section", "== Informations DirectX ==");

                // Utilisation de WMI pour obtenir des informations DirectX
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Nom du Contrôleur Graphique", obj["Name"].ToString());
                        AddInfoRow(table, "Version du Pilote DirectX", obj["DriverVersion"].ToString());
                    }
                }

                // Recherche des versions DirectX disponibles
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Version de l'OS", obj["Version"].ToString()); // Version DirectX basée sur l'OS
                    }
                }

                // Recherche des informations sur les capacités DirectX
                AddInfoRow(table, "Section", "== Capacités DirectX ==");

                // Recherche les capacités des API DirectX
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Capacité DirectX", obj["VideoProcessor"].ToString());
                    }
                }

                // Exécution de dxdiag pour obtenir des informations supplémentaires
                string dxdiagInfo = GetDxDiagInfo();
                if (!string.IsNullOrEmpty(dxdiagInfo))
                {
                    // Extraire la version de DirectX à partir du fichier dxdiag
                    string directXVersion = ExtractDirectXVersion(dxdiagInfo);
                    if (!string.IsNullOrEmpty(directXVersion))
                    {
                        AddInfoRow(table, "Version DirectX", directXVersion);  // Affichage de la version DirectX
                    }
                    else
                    {
                        AddInfoRow(table, "Version DirectX", "Non disponible");
                    }
                }
            }
            catch (Exception ex)
            {
                AddInfoRow(table, "Erreur", ex.Message);
            }
        }

        private string ExtractDirectXVersion(string dxdiagInfo)
        {
            // Recherche de la version de DirectX dans le texte de dxdiag
            string directXVersion = string.Empty;
            string[] lines = dxdiagInfo.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                if (line.Contains("DirectX Version"))
                {
                    directXVersion = line.Split(':')[1].Trim();
                    break;
                }
            }

            return directXVersion;
        }

        private string GetDxDiagInfo()
        {
            try
            {
                // Exécuter dxdiag en mode texte
                string dxdiagOutput = Path.Combine(Path.GetTempPath(), "dxdiag.txt");
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "dxdiag",
                        Arguments = "/t \"" + dxdiagOutput + "\"",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };

                process.Start();
                process.WaitForExit();

                // Lire le fichier de sortie
                if (File.Exists(dxdiagOutput))
                {
                    string dxdiagText = File.ReadAllText(dxdiagOutput);
                    return dxdiagText;
                }
                return "Erreur lors de la récupération des informations dxdiag.";
            }
            catch (Exception ex)
            {
                return "Erreur lors de l'exécution de dxdiag: " + ex.Message;
            }
        }




        private void UpdateRAMInfo()
        {
            var table = (TableLayoutPanel)Controls.Find("ramLayout", true)[0];
            ClearTable(table);

            try
            {
                AddInfoRow(table, "Section", "== Modules Mémoire ==");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        double capacityGB = Convert.ToDouble(obj["Capacity"]) / (1024 * 1024 * 1024);
                        AddInfoRow(table, "Fabricant", obj["Manufacturer"].ToString());
                        AddInfoRow(table, "Capacité", $"{capacityGB:F2} GB");
                        AddInfoRow(table, "Fréquence", $"{obj["Speed"]} MHz");
                        AddInfoRow(table, "Numéro de série", obj["SerialNumber"].ToString());
                        AddInfoRow(table, "Type", GetMemoryType(Convert.ToInt32(obj["MemoryType"])));
                        AddInfoRow(table, "Banc", obj["DeviceLocator"].ToString());
                        AddInfoRow(table, "Section", "------------------------");
                    }
                }

                AddInfoRow(table, "Section", "== Utilisation Mémoire ==");

                ulong totalMemory = computerInfo.TotalPhysicalMemory;
                ulong availableMemory = computerInfo.AvailablePhysicalMemory;
                ulong usedMemory = totalMemory - availableMemory;

                AddInfoRow(table, "Mémoire totale", $"{totalMemory / (1024 * 1024 * 1024):F2} GB");
                AddInfoRow(table, "Mémoire utilisée", $"{usedMemory / (1024 * 1024 * 1024):F2} GB");
                AddInfoRow(table, "Mémoire disponible", $"{availableMemory / (1024 * 1024 * 1024):F2} GB");
                AddInfoRow(table, "Pourcentage utilisé", $"{(usedMemory * 100.0 / totalMemory):F1}%");

                AddInfoRow(table, "Section", "== Fichier d'échange ==");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PageFileUsage"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Emplacement", obj["Name"].ToString());
                        AddInfoRow(table, "Taille totale", $"{obj["AllocatedBaseSize"]} MB");
                        AddInfoRow(table, "Utilisation actuelle", $"{obj["CurrentUsage"]} MB");
                    }
                }
            }
            catch (Exception ex)
            {
                AddInfoRow(table, "Erreur", ex.Message);
            }
        }

        private void UpdateGPUInfo()
        {
            var table = (TableLayoutPanel)Controls.Find("gpuLayout", true)[0];
            ClearTable(table);

            try
            {
                AddInfoRow(table, "Section", "== Cartes Graphiques ==");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Carte graphique", obj["Name"].ToString());
                        AddInfoRow(table, "Fabricant", obj["VideoProcessor"].ToString());

                        if (obj["AdapterRAM"] != null)
                        {
                            double vram = Convert.ToDouble(obj["AdapterRAM"]) / (1024 * 1024 * 1024);
                            AddInfoRow(table, "Mémoire vidéo", $"{vram:F2} GB");
                        }

                        AddInfoRow(table, "Résolution", $"{obj["CurrentHorizontalResolution"]}x{obj["CurrentVerticalResolution"]}");
                        AddInfoRow(table, "Fréquence", $"{obj["CurrentRefreshRate"]} Hz");
                        AddInfoRow(table, "Pilote version", obj["DriverVersion"].ToString());
                        AddInfoRow(table, "Date du pilote", obj["DriverDate"].ToString());
                        AddInfoRow(table, "Mode d'affichage", obj["VideoModeDescription"].ToString());
                        AddInfoRow(table, "Section", "------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                AddInfoRow(table, "Erreur", ex.Message);
            }
        }

        private void UpdateSystemInfo()
        {
            var table = (TableLayoutPanel)Controls.Find("systemLayout", true)[0];
            ClearTable(table);

            try
            {
                AddInfoRow(table, "Section", "== Système d'exploitation ==");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "OS", obj["Caption"].ToString());
                        AddInfoRow(table, "Version", obj["Version"].ToString());
                        AddInfoRow(table, "Architecture", obj["OSArchitecture"].ToString());
                        AddInfoRow(table, "Utilisateur", obj["RegisteredUser"].ToString());
                        AddInfoRow(table, "Organisation", obj["Organization"].ToString());
                        AddInfoRow(table, "Dernier démarrage", ManagementDateTimeConverter.ToDateTime(obj["LastBootUpTime"].ToString()).ToString());
                    }
                }

                AddInfoRow(table, "Section", "== Carte mère ==");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Modèle", obj["Product"].ToString());
                        AddInfoRow(table, "Fabricant", obj["Manufacturer"].ToString());
                        AddInfoRow(table, "Numéro de série", obj["SerialNumber"].ToString());
                    }
                }

                AddInfoRow(table, "Section", "== BIOS ==");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        AddInfoRow(table, "Version", obj["Version"].ToString());
                        AddInfoRow(table, "Date", obj["ReleaseDate"].ToString());
                        AddInfoRow(table, "Fabricant", obj["Manufacturer"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                AddInfoRow(table, "Erreur", ex.Message);
            }
        }

        private string GetMemoryType(int type)
        {
            switch (type)
            {
                case 0: return "Inconnu";
                case 1: return "Autre";
                case 2: return "DRAM";
                case 3: return "Synchronous DRAM";
                case 4: return "Cache DRAM";
                case 5: return "EDO";
                case 6: return "EDRAM";
                case 7: return "VRAM";
                case 8: return "SRAM";
                case 9: return "RAM";
                case 10: return "ROM";
                case 11: return "Flash";
                case 12: return "EPROM";
                case 13: return "EEPROM";
                case 14: return "FEPROM";
                case 15: return "NV-RAM";
                case 16: return "RAM mémoire non volatile";
                default: return "Inconnu";
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Mettez à jour les informations système à chaque intervalle
            UpdateCPUInfo();
            UpdateRAMInfo();
            UpdateGPUInfo();
            UpdateSystemInfo();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Appeler KillAllProcesses avant de quitter
            KillAllProcesses();
        }
        private void KillAllProcesses()
        {
            // Obtenez tous les processus enfants ou spécifiques à votre application.
            foreach (Process process in Process.GetProcessesByName("HardwareMonitor"))
            {
                try
                {
                    process.Kill(); // Tuer le processus
                    process.WaitForExit(); // S'assurer que le processus est bien terminé
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur en fermant le processus {process.ProcessName}: {ex.Message}");
                }
            }

            // Fermer la fenêtre principale (utile si d'autres threads sont actifs)
            Environment.Exit(0);
        }
    }
}
