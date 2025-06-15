using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SpaceInvaders
{
    class EnemyBlock : GameObject
    {
        private HashSet<SpaceShip> enemyShips;
        private int baseWidth;
        public Vecteur2D Position { get; private set; }
        public Size Size { get; private set; }
        private int speed;
        private int direction = 1;
        private bool toBottom = false;
        private double randomShootProbability = 1;

        /// <summary>
        /// Constructeur de la classe EnemyBlock.
        /// </summary>
        /// <param name="initialPosition">Position initiale du bloc d'ennemis.</param>
        /// <param name="initialWidth">Largeur initiale du bloc d'ennemis.</param>
        /// <param name="pSpeed">Vitesse de déplacement du bloc d'ennemis.</param>
        public EnemyBlock(Vecteur2D initialPosition, int initialWidth, int pSpeed) : base(Side.Enemy)
        {
            this.speed = pSpeed;
            this.Position = initialPosition;
            this.baseWidth = initialWidth;
            this.enemyShips = new HashSet<SpaceShip>();
            UpdateSize();
        }

        /// <summary>
        /// Réinitialise le bloc d'ennemis.
        /// </summary>
        public void reset()
        {
            this.enemyShips.Clear();
        }

        /// <summary>
        /// Ajoute une ligne d'ennemis au bloc.
        /// </summary>
        /// <param name="nbShips">Nombre de vaisseaux ennemis à ajouter.</param>
        /// <param name="nbLives">Nombre de vies de chaque vaisseau ennemi.</param>
        /// <param name="shipImage">Image du vaisseau ennemi.</param>
        /// <param name="gameInstance">Instance du jeu.</param>
        public void AddLine(int nbShips, int nbLives, Bitmap shipImage, Game gameInstance)
        {
            float intervalX = (float)(this.baseWidth - (nbShips * shipImage.Width)) / (nbShips - 1);
            float intervalY;
            if (this.enemyShips.Count > 0)
            {
                intervalY = (float)(this.enemyShips.Max(enemyShip => enemyShip.Position.Y) + shipImage.Height + 10);
            }
            else
            {
                intervalY = (float)this.Position.Y;
            }

            for (int i = 0; i < nbShips; i++)
            {
                float currentX = (float)this.Position.X + i * (shipImage.Width + intervalX);
                SpaceShip enemy = new SpaceShip(new Vecteur2D(currentX, intervalY), nbLives, shipImage, Side.Enemy, gameInstance);
                this.enemyShips.Add(enemy);
            }
            UpdateSize();
        }

        /// <summary>
        /// Met à jour la taille du bloc d'ennemis.
        /// </summary>
        public void UpdateSize()
        {
            if (this.enemyShips.Count == 0)
            {
                this.Size = new Size(0, 0);
                return;
            }

            double minX = float.MaxValue, maxX = float.MinValue;
            float minY = float.MaxValue, maxY = float.MinValue;

            foreach (SpaceShip enemyShip in this.enemyShips)
            {
                if (enemyShip.Position.X < minX) { minX = (float)enemyShip.Position.X; }
                if (enemyShip.Position.X + enemyShip.Image.Width > maxX) { maxX = (float)enemyShip.Position.X + enemyShip.Image.Width; }
                if (enemyShip.Position.Y < minY) { minY = (float)enemyShip.Position.Y; }
                if (enemyShip.Position.Y + enemyShip.Image.Height > maxY) { maxY = (float)enemyShip.Position.Y + enemyShip.Image.Height; }
            }

            this.Position = new Vecteur2D(minX, minY);
            this.Size = new Size((int)(maxX - minX), (int)(maxY - minY));
        }

        /// <summary>
        /// Met à jour l'état du bloc d'ennemis.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        public override void Update(Game gameInstance, double deltaT)
        {
            this.ShootRandom(gameInstance, deltaT);
            int step = 0;

            this.Position.X += deltaT * this.speed * this.direction;

            if (this.Position.X <= 0 || this.Position.X + this.Size.Width > gameInstance.GameSize.Width)
            {
                this.direction *= -1;

                if (!this.toBottom)
                {
                    step = 10;
                    this.Position.Y += step;
                    this.randomShootProbability += 0.01;
                    this.toBottom = true;
                }

                if (this.speed <= 160)
                {
                    this.speed += 10;
                }
            }
            else
            {
                this.toBottom = false;
            }

            foreach (SpaceShip enemyShip in this.enemyShips)
            {
                enemyShip.Position.X += deltaT * this.speed * this.direction;
                enemyShip.Position.Y += step;
            }
        }

        /// <summary>
        /// Fait tirer aléatoirement les vaisseaux ennemis.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        public void ShootRandom(Game gameInstance, double deltaT)
        {
            Random random = new Random();
            foreach (SpaceShip enemyShip in this.enemyShips)
            {
                if (random.NextDouble() <= this.randomShootProbability * deltaT)
                {
                    enemyShip.Shoot(gameInstance, deltaT, 0);
                }
            }
        }

        /// <summary>
        /// Dessine le bloc d'ennemis.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="graphics">Objet Graphics pour dessiner.</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            foreach (SpaceShip enemyShip in this.enemyShips)
            {
                enemyShip.Draw(gameInstance, graphics);
            }
        }

        /// <summary>
        /// Vérifie si le bloc d'ennemis est encore en vie.
        /// </summary>
        /// <returns>True si au moins un vaisseau ennemi est en vie, sinon False.</returns>
        public override bool IsAlive()
        {
            foreach (SpaceShip enemyShip in this.enemyShips)
            {
                if (enemyShip.IsAlive())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gère la collision avec un missile.
        /// </summary>
        /// <param name="m">Missile en collision avec le bloc d'ennemis.</param>
        public override void Collision(Missile m)
        {
            foreach (SpaceShip enemy in this.enemyShips)
            {
                if (!enemy.IsAlive())
                {
                    continue;
                }

                enemy.Collision(m);

                if (!m.IsAlive())
                {
                    break;
                }
            }

            this.enemyShips.RemoveWhere(enemy => !enemy.IsAlive());
            this.UpdateSize();
        }
    }
}
