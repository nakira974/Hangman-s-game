using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JeuxDuPendu.MyControls
{
    class HangmanViewer : PictureBox
    {
        // Etape en cours
        private int _actualStep = 0;

        // Nombre totale d'etapes
        private const int StepCount = 11;


        /// <summary>
        /// Proprieté permettant de savoir si la partie est terminée
        /// </summary>
        public bool IsGameOver
        {
            get { return _actualStep + 1 >= StepCount; }
        }

        /// <summary>
        /// Contructeur de la classe
        /// </summary>
        public HangmanViewer()
        {
            ProcessStepChanged();
        }


        /// <summary>
        /// Relise a zero de l'instance.
        /// </summary>
        public void Reset()
        {
            _actualStep = 0;
            ProcessStepChanged();
        }


        /// <summary>
        /// Avance l'affichage d'une étape.
        /// </summary>
        public void MoveNextStep()
        {
            _actualStep++;

            if (_actualStep >= StepCount)
                _actualStep = StepCount - 1;

            ProcessStepChanged();
        }

        public async Task MoveNextStep(short step)
        {
            _actualStep = step;

            if (_actualStep >= StepCount)
                _actualStep = StepCount - 1;

            ProcessStepChanged();
        }

        /// <summary>
        /// Change l'image affichée
        /// </summary>
        private void ProcessStepChanged()
        {
            this.Image = GetImageFromResourcesByStepNumber(_actualStep);
        }

        /// <summary>
        /// Retourne l'image correpondant au numero de l'étape.
        /// </summary>
        /// <param name="step"> numero de l'etape </param>
        /// <returns> Image à afficher</returns>
        private Image GetImageFromResourcesByStepNumber(int step)
        {
            switch (step)
            {
                case 0:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._0);
                case 1:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._1);
                case 2:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._2);
                case 3:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._3);
                case 4:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._4);
                case 5:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._5);
                case 6:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._6);
                case 7:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._7);
                case 8:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._8);
                case 9:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._9);
                case 10:
                    return new Bitmap(JeuxDuPendu.Properties.Resources._10);
                default:
                    throw new Exception("L'étape " + step + "n'existe pas");
                    return null;
            }
        }
    }
}