using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    class PlayerSpaceship : SpaceShip
    {
        private int chooseShoot = 0;
        private int palier = 200;

        public int Money { get; set; }

        /// <summary>
        /// Constructeur de la classe PlayerSpaceship.
        /// </summary>
        /// <param name="v">Position initiale du vaisseau.</param>
        /// <param name="lives">Nombre de vies du vaisseau.</param>
        /// <param name="gameInstance">Instance du jeu.</param>
        public PlayerSpaceship(Vecteur2D v, int lives, Game gameInstance) : base(v, lives, SpaceInvaders.Properties.Resources.ship3, Side.Ally, gameInstance) { }

        /// <summary>
        /// Met à jour l'état du vaisseau joueur.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            if (gameInstance.keyPressed.Contains(System.Windows.Forms.Keys.U))
            {
                UpgradeShoot();
            }
            if (gameInstance.keyPressed.Contains(System.Windows.Forms.Keys.Space))
            {
                Shoot(gameInstance, deltaT, this.chooseShoot);
            }
            if (gameInstance.keyPressed.Contains(System.Windows.Forms.Keys.Right))
            {
                if (this.Position.X < gameInstance.GameSize.Width - this.Image.Width)
                {
                    this.Position = this.Position + new Vecteur2D(this.SpeedPixelPerSecond * deltaT, 0);
                }
            }
            else if (gameInstance.keyPressed.Contains(System.Windows.Forms.Keys.Left))
            {
                if (this.Position.X > 0)
                {
                    this.Position = Position - new Vecteur2D(this.SpeedPixelPerSecond * deltaT, 0);
                }
            }
        }

        /// <summary>
        /// Améliore le type de tir du vaisseau joueur.
        /// </summary>
        public void UpgradeShoot()
        {
            if (this.Money - this.palier >= 0)
            {
                if (this.chooseShoot <= 3)
                {
                    this.Money -= this.palier;
                    this.chooseShoot++;
                    this.palier *= 2;
                }
                return;
            }
        }

        /// <summary>
        /// Dessine le vaisseau joueur.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="g">Objet Graphics pour dessiner.</param>
        public override void Draw(Game gameInstance, Graphics g)
        {
            base.Draw(gameInstance, g);
            AfficheStats(gameInstance, g);
        }

        /// <summary>
        /// Affiche les statistiques du vaisseau joueur.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="g">Objet Graphics pour dessiner.</param>
        private void AfficheStats(Game gameInstance, Graphics g)
        {
            // Affichage des vies
            string text = "" + this.Lives + " ♥";
            SizeF textSize = g.MeasureString(text, new Font("Times New Roman", 60, FontStyle.Bold, GraphicsUnit.Pixel));
            float x = gameInstance.GameSize.Width + 10;
            float y = gameInstance.GameSize.Height - textSize.Height + 15;
            g.DrawString(text, new Font("Times New Roman", 30, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, x, 10);

            // Affichage de l'argent
            string argent = "" + this.Money + " $";
            g.DrawString(argent, new Font("Times New Roman", 20, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, x, 40);
        }
    }
}
