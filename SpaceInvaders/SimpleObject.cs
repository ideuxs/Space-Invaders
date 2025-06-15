using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;

namespace SpaceInvaders
{
    /// <summary>
    /// Classe abstraite représentant un objet dans le jeu.
    /// </summary>
    abstract class SimpleObject : GameObject
    {
        public Vecteur2D Position { get; set; }
        public Bitmap Image { get; set; }
        public Game GameInstance { get; private set; }
        public int Lives { get; set; }

        /// <summary>
        /// Constructeur de la classe SimpleObject.
        /// </summary>
        /// <param name="camp">Camp auquel appartient l'objet.</param>
        /// <param name="gameInstance">Instance du jeu.</param>
        protected SimpleObject(Side camp, Game gameInstance) : base(camp)
        {
            this.GameInstance = gameInstance;
        }

        /// <summary>
        /// Dessine l'objet sur l'écran.
        /// </summary>
        /// <param name="gameInstance">Instance du jeu.</param>
        /// <param name="graphics">Objet Graphics pour dessiner.</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            graphics.DrawImage(this.Image, (float)this.Position.X, (float)this.Position.Y, this.Image.Width, this.Image.Height);
        }

        /// <summary>
        /// Vérifie si l'objet est encore en vie.
        /// </summary>
        /// <returns>True si l'objet a encore des vies, sinon False.</returns>
        public override bool IsAlive()
        {
            return this.Lives > 0;
        }

        /// <summary>
        /// Gère la collision entre l'objet et un missile.
        /// </summary>
        /// <param name="missile">Missile en collision avec l'objet.</param>
        public override void Collision(Missile missile)
        {
            if (this.Camp == missile.Camp)
            {
                return;
            }

            Rectangle objectRectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Image.Width), (int)(this.Image.Height));
            Rectangle missileRectangle = new Rectangle((int)missile.Position.X, (int)missile.Position.Y, (int)(missile.Image.Width), (int)(missile.Image.Height));
            if (!RectanglesIntersect(objectRectangle, missileRectangle))
            {
                return;
            }

            int collisionPixelCount = PixelCollision(missile, objectRectangle, missileRectangle);

            if (collisionPixelCount > 0)
            {
                OnCollision(missile, collisionPixelCount);
            }
        }

        /// <summary>
        /// Vérifie si deux rectangles se chevauchent.
        /// </summary>
        /// <param name="rect1">Premier rectangle.</param>
        /// <param name="rect2">Deuxième rectangle.</param>
        /// <returns>True si les rectangles se chevauchent, sinon False.</returns>
        public bool RectanglesIntersect(Rectangle rect1, Rectangle rect2)
        {
            return !(rect1.Right <= rect2.Left || rect1.Bottom <= rect2.Top ||
                     rect2.Right <= rect1.Left || rect2.Bottom <= rect1.Top);
        }

        /// <summary>
        /// Vérifie la collision au niveau des pixels entre l'objet et un missile.
        /// </summary>
        /// <param name="missile">Missile en collision avec l'objet.</param>
        /// <param name="objectRectangle">Rectangle de l'objet.</param>
        /// <param name="missileRect">Rectangle du missile.</param>
        /// <returns>Nombre de pixels en collision.</returns>
        private int PixelCollision(Missile missile, Rectangle objectRectangle, Rectangle missileRect)
        {
            Rectangle intersection = Rectangle.Intersect(objectRectangle, missileRect);
            int collisionPixelCount = 0;

            for (int y = intersection.Top; y < intersection.Bottom; y++)
            {
                for (int x = intersection.Left; x < intersection.Right; x++)
                {
                    int bunkerX = (int)(x - (int)Position.X);
                    int bunkerY = (int)(y - (int)Position.Y);
                    int missileX = (int)(x - (int)missile.Position.X);
                    int missileY = (int)(y - (int)missile.Position.Y);

                    Color bunkerPixel = Image.GetPixel(bunkerX, bunkerY);
                    Color missilePixel = missile.Image.GetPixel(missileX, missileY);

                    if (IsCollisionPixel(bunkerPixel, missilePixel))
                    {
                        collisionPixelCount++;
                        if (this.Camp != Side.Enemy && this.Camp != Side.Jeton)
                        {
                            this.Image.SetPixel(bunkerX, bunkerY, Color.Transparent);
                        }
                    }
                }
            }

            return collisionPixelCount;
        }

        /// <summary>
        /// Vérifie si deux pixels sont en collision.
        /// </summary>
        /// <param name="bunkerPixel">Pixel de l'objet.</param>
        /// <param name="missilePixel">Pixel du missile.</param>
        /// <returns>True si les pixels sont en collision, sinon False.</returns>
        private bool IsCollisionPixel(Color bunkerPixel, Color missilePixel)
        {
            bool isBunkerPixel = (bunkerPixel.A == 255 && bunkerPixel.R == 255 && bunkerPixel.G == 255 && bunkerPixel.B == 255)
                || (bunkerPixel.A == 255 && bunkerPixel.R == 136 && bunkerPixel.G == 0 && bunkerPixel.B == 21)
                || (bunkerPixel.A == 255 && bunkerPixel.R == 0 && bunkerPixel.G == 0 && bunkerPixel.B == 0)
                || (bunkerPixel.A == 255 && bunkerPixel.R == 213 && bunkerPixel.G == 159 && bunkerPixel.B == 128);

            bool isMissilePixel = missilePixel.A > 0;
            return isBunkerPixel && isMissilePixel;
        }

        /// <summary>
        /// Méthode abstraite pour gérer la collision entre l'objet et un missile.
        /// </summary>
        /// <param name="missile">Missile en collision avec l'objet.</param>
        /// <param name="numberOfPixelsInCollision">Nombre de pixels en collision.</param>
        protected abstract void OnCollision(Missile missile, int numberOfPixelsInCollision);

        /// <summary>
        /// Joue un son.
        /// </summary>
        /// <param name="song">Chemin du fichier audio à jouer.</param>
        public void PlaySound(String song)
        {
            Task.Run(() =>
            {
                using (var audioFile = new AudioFileReader(song))
                using (var outputDevice = new WaveOutEvent())
                {
                    audioFile.Volume = 0.1f;
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Task.Delay(100).Wait();
                    }
                }
            });
        }
    }
}
