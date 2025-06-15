using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    class Bunker : SimpleObject
    {
        Bitmap imagebunker = SpaceInvaders.Properties.Resources.bunker;

        /// <summary>
        /// Constructeur de la classe Bunker.
        /// </summary>
        /// <param name="position">Position initiale du bunker.</param>
        /// <param name="gameInstance">Instance du jeu.</param>
        public Bunker(Vecteur2D position, Game gameInstance) : base(Side.Neutral, gameInstance)
        {
            this.Position = position;
            this.Image = imagebunker;
            this.Lives = 3;
        }

        /// <summary>
        /// Met à jour l'état du bunker.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        public override void Update(Game gameInstance, double deltaT){}

        /// <summary>
        /// Gère la collision entre le bunker et un missile.
        /// </summary>
        /// <param name="missile">Missile en collision avec le bunker.</param>
        /// <param name="numberOfPixelsInCollision">Nombre de pixels en collision.</param>
        protected override void OnCollision(Missile missile, int numberOfPixelsInCollision)
        {
            missile.Lives -= numberOfPixelsInCollision;
        }
    }
}
