using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace SpaceInvaders
{
    class SpaceShip : SimpleObject
    {
        Missile missile;
        private bool isVisible = true;
        private double comptTir = 0;
        public double SpeedPixelPerSecond { get; set; }

        /// <summary>
        /// Constructeur de la classe SpaceShip.
        /// </summary>
        /// <param name="v">Position initiale du vaisseau.</param>
        /// <param name="lives">Nombre de vies du vaisseau.</param>
        /// <param name="img">Image du vaisseau.</param>
        /// <param name="camp">Camp auquel appartient le vaisseau.</param>
        /// <param name="gameInstance">Instance du jeu.</param>
        public SpaceShip(Vecteur2D v, int lives, Bitmap img, Side camp, Game gameInstance) : base(camp, gameInstance)
        {
            this.SpeedPixelPerSecond = 200;
            this.Position = v;
            this.Lives = lives;
            this.Image = img;
        }

        /// <summary>
        /// Permet au vaisseau de tirer en fonction du type de tir choisi.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        /// <param name="chooseShoot">Type de tir choisi.</param>
        public void Shoot(Game gameInstance, double deltaT, int chooseShoot)
        {
            switch (chooseShoot)
            {
                case 1:
                    this.ShootMultiple(gameInstance, deltaT, 2);
                    break;
                case 2:
                    this.ShootSingle(gameInstance, deltaT, null);
                    break;
                case 3:
                    this.ShootSingle(gameInstance, deltaT, SpaceInvaders.Properties.Resources.shoot3);
                    break;
                default:
                    this.Shoot(gameInstance);
                    break;
            }
        }

        /// <summary>
        /// Vérifie si le vaisseau est encore en vie.
        /// </summary>
        /// <returns>True si le vaisseau a encore des vies, sinon False.</returns>
        public override bool IsAlive()
        {
            return this.Lives > 0;
        }

        /// <summary>
        /// Permet au vaisseau de tirer un missile.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        public void Shoot(Game gameInstance)
        {
            if (this.missile == null || !this.missile.IsAlive())
            {
                Missile m = new Missile(this.Position, 1, SpaceInvaders.Properties.Resources.shoot1, this.Camp, gameInstance);
                this.missile = m;
                gameInstance.AddNewGameObject(this.missile);
            }
        }

        /// <summary>
        /// Permet au vaisseau de tirer un seul missile.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        /// <param name="img">Image du missile.</param>
        public void ShootSingle(Game gameInstance, double deltaT, Bitmap img)
        {
            if (img == null) { img = SpaceInvaders.Properties.Resources.shoot1; }
            this.comptTir += deltaT;

            if (this.comptTir >= 0.2)
            {
                Missile m = new Missile(Position, 1, img, this.Camp, gameInstance);
                this.missile = m;
                gameInstance.AddNewGameObject(this.missile);
                this.comptTir = 0;
            }
        }

        /// <summary>
        /// Permet au vaisseau de tirer plusieurs missiles.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        /// <param name="count">Nombre de missiles à tirer.</param>
        public void ShootMultiple(Game gameInstance, double deltaT, int count)
        {
            this.comptTir += deltaT;
            if ((this.missile == null || !this.missile.IsAlive()) && (this.comptTir >= 0.6))
            {
                for (int i = 0; i < count; i++)
                {
                    Missile m = new Missile(new Vecteur2D(this.Position.X + i * 30, this.Position.Y), 1, SpaceInvaders.Properties.Resources.shoot1, this.Camp, gameInstance);
                    this.missile = m;
                    gameInstance.AddNewGameObject(this.missile);
                }
                this.comptTir = 0;
            }
        }

        /// <summary>
        /// Gère la collision entre le vaisseau et un missile.
        /// </summary>
        /// <param name="missile">Missile en collision avec le vaisseau.</param>
        /// <param name="numberOfPixelsInCollision">Nombre de pixels en collision.</param>
        protected override void OnCollision(Missile missile, int numberOfPixelsInCollision)
        {
            this.PlaySound("Resources/shoot.wav");
            this.Lives -= Math.Min(this.Lives, missile.Lives);
            missile.Lives -= numberOfPixelsInCollision;

            if (this.IsAlive())
            {
                Clignot(1, 0.1);
            }
            else if (this.Camp == Side.Enemy)
            {
                Random random = new Random();
                if (random.NextDouble() <= 0.20)
                {
                    Bonus bonus = new Bonus(new Vecteur2D(this.Position.X, this.Position.Y), SpaceInvaders.Properties.Resources.bonus2, this.GameInstance, "vie");
                    this.GameInstance.AddNewGameObject(bonus);
                }
                else
                {
                    Bonus coin = new Bonus(new Vecteur2D(this.Position.X, this.Position.Y), SpaceInvaders.Properties.Resources.bonus, this.GameInstance, "coin");
                    this.GameInstance.AddNewGameObject(coin);
                }
            }
        }

        /// <summary>
        /// Fait clignoter le vaisseau.
        /// </summary>
        /// <param name="totalDuree">Durée totale du clignotement.</param>
        /// <param name="interval">Intervalle de clignotement.</param>
        public void Clignot(double totalDuree, double interval)
        {
            double t = 0;
            Timer timer = new Timer { Interval = (int)(interval * 1000) };

            timer.Tick += (sender, args) =>
            {
                this.isVisible = !this.isVisible;
                t += interval;

                if (t >= totalDuree)
                {
                    timer.Stop();
                    this.isVisible = true;
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Dessine le vaisseau sur l'écran.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="graphics">Objet Graphics pour dessiner.</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            if (isVisible)
            {
                graphics.DrawImage(this.Image, (float)this.Position.X, (float)this.Position.Y, this.Image.Width, this.Image.Height);
            }
        }

        /// <summary>
        /// Met à jour l'état du vaisseau.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="deltaT">Temps écoulé.</param>
        public override void Update(Game gameInstance, double deltaT)
        {
        }
    }
}