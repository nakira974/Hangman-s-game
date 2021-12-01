using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GameLib;
using JeuxDuPendu.MyControls;

namespace JeuxDuPendu
{
    public partial class GameForm : Form
    {
        // Initialisation de l'instance de la classe d'affichage du pendu.
        HangmanViewer _hangmanViewer = new HangmanViewer();
        private HangmanGame _ruler = new HangmanGame();
        /// <summary>
        /// Constructeur du formulaire de jeux
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            InitializeMyComponent();
            StartNewGame();
            lCrypedWord.Text = _ruler.HashedWord.ToString();
        }

        /// <summary>
        /// Initialisations des composant specifique a l'application
        /// </summary>
        private void InitializeMyComponent()
        {
            // On positionne le controle d'affichage du pendu dans panel1 : 
            panel1.Controls.Add(_hangmanViewer);

            // à la position 0,0
            _hangmanViewer.Location = new Point(0, 0);

            // et de la même taille que panel1
            _hangmanViewer.Size = panel1.Size;
        }

        /// <summary>
        /// Initialise une nouvelle partie
        /// </summary>
        public void StartNewGame()
        {
            _ruler.NewGame();
            lCrypedWord.Text = _ruler.HashedWord.ToString(); 
            // Methode de reinitialisation classe d'affichage du pendu.
            _hangmanViewer.Reset();

            //Affichage du mot à trouver dans le label.
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
            textBox1.Text = e.KeyChar.ToString();
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
            _ruler.LastCharDiscovered.Add(letter);
            listBox1.Items.Add(_ruler.LastCharDiscovered.Last());
            // On avance le pendu d'une etape
            if (_ruler.TryAppendCharacter(letter).Result)
            {
                lCrypedWord.Text = _ruler.HashedWord.ToString();
            }
            else
            {
                _hangmanViewer.MoveNextStep();
            }

            // Si le pendu est complet, le joueur à perdu.
            if (_hangmanViewer.IsGameOver)
            {
                MessageBox.Show("Vous avez perdu !");
                StartNewGame();
            }
            else if (_ruler.IsGameWon)
            {
                MessageBox.Show("Vous avez gagné !");
                StartNewGame();

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KeyPressed(textBox1.Text.First());
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //TO DO 
            //Implémenter les règles du jeu ici. 
            
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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}