using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardwareMonitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Empêcher le redimensionnement de la fenêtre
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Désactiver le bouton Maximiser
            this.MaximizeBox = false;

            // (Facultatif) Désactiver le bouton Agrandir (Alt + Space)
            this.MinimizeBox = true; // Laisser activé ou désactivé selon votre besoin
            // Charge Form2 après 20 secondes
            LoadForm2InBackground();
        }

        private async void LoadForm2InBackground()
        {
            // Affiche Form1 immédiatement
            this.Show();

            // Attend 20 secondes avant d'ouvrir Form2
            await Task.Delay(9000);  // 20 000 millisecondes = 20 secondes

            // Crée et affiche Form2 dans un thread séparé sans bloquer Form1
            Form2 form2 = new Form2();
            form2.Show(); // Affiche Form2 de manière non bloquante

            // Ferme uniquement Form1, mais ne ferme pas l'application
            this.Invoke(new Action(() =>
            {
                this.Hide();  // Utilisez Hide() au lieu de Close() pour cacher Form1 sans fermer l'application
            }));
        }
    }
}
