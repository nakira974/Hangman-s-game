using System;
using System.Drawing;
using System.Windows.Forms;
using JeuxDuPendu.MyControls;

namespace JeuxDuPendu
{
    public partial class GameForm : Form
    {
        // Initialisation de l'instance de la classe d'affichage du pendu.
        HangmanViewer _HangmanViewer = new HangmanViewer();

        /// <summary>
        /// Constructeur du formulaire de jeux
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            InitializeMyComponent();
            StartNewGame();
        }

        /// <summary>
        /// Initialisations des composant specifique a l'application
        /// </summary>
        private void InitializeMyComponent()
        {
            // On positionne le controle d'affichage du pendu dans panel1 : 
            panel1.Controls.Add(_HangmanViewer);

            // à la position 0,0
            _HangmanViewer.Location = new Point(0, 0);

            // et de la même taille que panel1
            _HangmanViewer.Size = panel1.Size;
        }

        /// <summary>
        /// Initialise une nouvelle partie
        /// </summary>
        public void StartNewGame()
        {
            // Methode de reinitialisation classe d'affichage du pendu.
            _HangmanViewer.Reset();

            //Affichage du mot à trouver dans le label.
            lCrypedWord.Text = "_____";
        }


        /// <summary>
        /// Methode appelé lors de l'appui d'un touche du clavier, lorsque le focus est sur le bouton "Nouvelle partie"
        /// </summary>
        private void bReset_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressed(e.KeyChar);
        }

        /// <summary>
        /// Methode appelé lors de l'appui d'un touche du clavier, lorsque le focus est sur le forulaire
        /// </summary>
        private void GameForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressed(e.KeyChar);
        }

        /// <summary>
        /// Methode appelé lors de l'appui sur le bouton "Nouvelle partie"
        /// </summary>
        private void bReset_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void KeyPressed(char letter)
        {
            // On avance le pendu d'une etape
            _HangmanViewer.MoveNextStep();

            // Si le pendu est complet, le joueur à perdu.
            if (_HangmanViewer.IsGameOver)
            {
                MessageBox.Show("Vous avez perdu !");
                StartNewGame();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //TO DO 
            //Implémenter les règles du jeu ici. 
            lCrypedWord.Text = textBox1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            ConnectionForm connectionMenu = new ConnectionForm();
            connectionMenu.Closed += (s, args) => this.Close();
            connectionMenu.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}