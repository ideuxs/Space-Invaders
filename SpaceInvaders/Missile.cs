using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpaceInvaders
{
    class Missile : SimpleObject
    {
        private double vitesse = 500;

        /// <summary>
        /// Constructeur de la classe Missile.
        /// </summary>
        /// <param name="pos">Position initiale du missile.</param>
        /// <param name="lives">Nombre de vies du missile.</param>
        /// <param name="img">Image du missile.</param>
        /// <param name="camp">Camp auquel appartient le missile.</param>
        /// <param name="gameInstance">Instance du jeu.</param>
        public Missile(Vecteur2D pos, int lives, Bitmap img, Side camp, Game gameInstance) : base(camp, gameInstance)
        {
            this.Position = pos;
            this.Lives = lives;
            this.Image = img;
        }

        /// <summary>
        /// Met à jour l'état du missile.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            if (this.Position.Y > 0)
            {
                double direction = this.Camp == Side.Enemy ? 1 : -1;
                this.Position = this.Position + new Vecteur2D(0, deltaT * this.vitesse * direction);

                if (this.Camp == Side.Enemy && this.Position.Y > gameInstance.playerShip.Position.Y + 20)
                {
                    this.Lives = 0;
                }

                foreach (GameObject gameObj in gameInstance.gameObjects)
                {
                    gameObj.Collision(this);
                }
            }
            else if (this.Position.Y < gameInstance.GameSize.Height - this.Image.Height || this.Position.Y > 0)
            {
                this.Lives = 0;
            }
        }

        /// <summary>
        /// Gère la collision entre le missile et un autre missile.
        /// </summary>
        /// <param name="missile">Missile en collision avec le missile courant.</param>
        /// <param name="numberOfPixelsInCollision">Nombre de pixels en collision.</param>
        protected override void OnCollision(Missile missile, int numberOfPixelsInCollision)
        {
            this.Lives -= numberOfPixelsInCollision;
            missile.Lives -= numberOfPixelsInCollision;
        }
    }
}
