using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace SpaceInvaders
{
    /// <summary>
    /// Cette classe représente le jeu entier.
    /// </summary>
    class Game
    {

        #region Gestion des objets de jeu
        /// <summary>
        /// Ensemble de tous les objets de jeu actuellement dans le jeu.
        /// </summary>
        public HashSet<GameObject> gameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Ensemble des nouveaux objets de jeu à ajouter au jeu.
        /// </summary>
        private HashSet<GameObject> pendingNewGameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Planifie un nouvel objet pour l'ajouter au jeu.
        /// Le nouvel objet sera ajouté au début de la prochaine boucle de mise à jour.
        /// </summary>
        /// <param name="gameObject">Objet à ajouter.</param>
        public void AddNewGameObject(GameObject gameObject)
        {
            pendingNewGameObjects.Add(gameObject);
        }
        #endregion

        #region Éléments techniques du jeu
        /// <summary>
        /// Taille de la zone de jeu.
        /// </summary>
        public Size GameSize;

        /// <summary>
        /// État du clavier.
        /// </summary>
        public HashSet<Keys> keyPressed = new HashSet<Keys>();
        private EnemyBlock enemies;
        private EnemyBlock enemies2;
        private EnemyBlock enemies3;

        enum GameState
        {
            Play,
            Pause,
            Win,
            Lost
        }

        GameState state;
        #endregion

        #region Champs statiques (helpers)
        /// <summary>
        /// Instance du jeu.
        /// </summary>
        public static Game game { get; private set; }

        /// <summary>
        /// Pinceau noir.
        /// </summary>
        private static Brush blackBrush = new SolidBrush(Color.Black);
        #endregion

        Bunker bunker_un;
        Bunker bunker_deux;
        Bunker bunker_trois;
        Bitmap imagebunk = SpaceInvaders.Properties.Resources.bunker;
        public PlayerSpaceship playerShip;
        Bitmap img = SpaceInvaders.Properties.Resources.ship3;
        Bitmap img2 = SpaceInvaders.Properties.Resources.ship1;
        Bitmap img5 = SpaceInvaders.Properties.Resources.ship5;
        Bitmap img8 = SpaceInvaders.Properties.Resources.ship8;
        Bitmap img7 = SpaceInvaders.Properties.Resources.ship7;
        Bitmap img4 = SpaceInvaders.Properties.Resources.ship4;
        Bitmap img9 = SpaceInvaders.Properties.Resources.ship9;

        #region Constructeurs
        /// <summary>
        /// Constructeur du jeu.
        /// </summary>
        /// <param name="gameSize">Taille de la zone de jeu.</param>
        /// <returns>Instance du jeu.</returns>
        public static Game CreateGame(Size gameSize)
        {
            if (game == null)
                game = new Game(gameSize);
            return game;
        }

        /// <summary>
        /// Constructeur privé.
        /// </summary>
        /// <param name="gameSize">Taille de la zone de jeu.</param>
        private Game(Size gameSize)
        {
            this.GameSize = gameSize;
            CreateObject();
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Crée les objets de jeu initiaux.
        /// </summary>
        public void CreateObject()
        {
            int nbBunker = 3;
            int bunkerWidth = (int)(this.imagebunk.Width);
            int largeur = this.GameSize.Width - (nbBunker * bunkerWidth);
            int interval = largeur / (nbBunker + 1);
            this.bunker_un = new Bunker(new Vecteur2D((interval), this.GameSize.Height - 230), this);
            this.bunker_deux = new Bunker(new Vecteur2D((interval * 2 + bunkerWidth), this.GameSize.Height - 230), this);
            this.bunker_trois = new Bunker(new Vecteur2D((interval * 3 + bunkerWidth * 2), this.GameSize.Height - 230), this);

            AddNewGameObject(this.bunker_un);
            AddNewGameObject(this.bunker_deux);
            AddNewGameObject(this.bunker_trois);
            this.playerShip = new PlayerSpaceship(new Vecteur2D((this.GameSize.Width - this.img.Width) / 2, this.GameSize.Height - this.img.Height - 115), 3, this);
            AddNewGameObject(this.playerShip);
            InitEnemies();
        }

        /// <summary>
        /// Initialise les ennemis.
        /// </summary>
        private void InitEnemies()
        {
            this.enemies = new EnemyBlock(new Vecteur2D(0, 0), (int)(this.GameSize.Width * 0.6), 10);
            this.enemies.AddLine(3, 1, img8, this);
            this.enemies.AddLine(7, 1, img5, this);
            this.enemies.AddLine(4, 2, img2, this);
            AddNewGameObject(this.enemies);

            this.enemies2 = new EnemyBlock(new Vecteur2D(0, 0), (int)(this.GameSize.Width * 0.6), 30);
            this.enemies2.AddLine(5, 4, img8, this);
            this.enemies2.AddLine(7, 2, img7, this);
            this.enemies2.AddLine(3, 6, img4, this);

            this.enemies3 = new EnemyBlock(new Vecteur2D(0, 0), (int)(this.GameSize.Width * 0.6), 20);
            this.enemies3.AddLine(5, 10, img9, this);
            this.enemies3.AddLine(7, 2, img7, this);
            this.enemies3.AddLine(5, 6, img4, this);
            this.enemies3.AddLine(6, 6, img9, this);
        }

        /// <summary>
        /// Lance la deuxième vague d'ennemis.
        /// </summary>
        public void Vague2()
        {
            AddNewGameObject(this.enemies2);
        }

        /// <summary>
        /// Lance la troisième vague d'ennemis.
        /// </summary>
        public void Vague3()
        {
            AddNewGameObject(this.enemies3);
        }

        /// <summary>
        /// Ignore une touche donnée dans les mises à jour suivantes jusqu'à ce que l'utilisateur la retape ou que le système la réactive automatiquement.
        /// </summary>
        /// <param name="key">Touche à ignorer.</param>
        public void ReleaseKey(Keys key)
        {
            this.keyPressed.Remove(key);
        }

        /// <summary>
        /// Dessine le jeu entier.
        /// </summary>
        /// <param name="g">Objet Graphics pour dessiner.</param>
        public void Draw(Graphics g)
        {
            Font customFont = new Font("Times New Roman", 20, FontStyle.Bold, GraphicsUnit.Pixel);
            if (this.state == GameState.Pause)
            {
                string pauseText = "             Pause\n Press 'P' to unpause\n Press 'U' to upgrade";
                SizeF textSize = g.MeasureString(pauseText, customFont);
                float x = (this.GameSize.Width - textSize.Width) / 2;
                float y = -50 + (this.GameSize.Height - textSize.Height) / 2;

                g.DrawString(pauseText, customFont, Brushes.White, x, y);
            }
            else if (this.state == GameState.Lost)
            {
                string lostText = "             LOST \n Press 'R' to restart";
                SizeF textSize = g.MeasureString(lostText, customFont);
                float x = (this.GameSize.Width - textSize.Width) / 2;
                float y = -50 + (this.GameSize.Height - textSize.Height) / 2;

                g.DrawString(lostText, customFont, Brushes.White, x, y);
            }
            else if (this.state == GameState.Win)
            {
                string lostText = "             WIN \n Press 'R' to restart";
                SizeF textSize = g.MeasureString(lostText, customFont);
                float x = (this.GameSize.Width - textSize.Width) / 2;
                float y = -50 + (this.GameSize.Height - textSize.Height) / 2;

                g.DrawString(lostText, customFont, Brushes.White, x, y);
            }
            else if (this.state == GameState.Play)
            {
                foreach (GameObject gameObject in this.gameObjects)
                {
                    gameObject.Draw(this, g);
                }
            }
        }

        /// <summary>
        /// Met à jour le jeu.
        /// </summary>
        /// <param name="deltaT">Temps écoulé.</param>
        public void Update(double deltaT)
        {
            // Ajouter de nouveaux objets de jeu
            this.gameObjects.UnionWith(this.pendingNewGameObjects);
            this.pendingNewGameObjects.Clear();

            if (this.keyPressed.Contains(Keys.P))
            {
                if (this.state == GameState.Pause)
                {
                    this.state = GameState.Play;
                }
                else
                {
                    this.state = GameState.Pause;
                }
                ReleaseKey(Keys.P);
            }

            if (this.state == GameState.Play)
            {
                foreach (GameObject gameObject in this.gameObjects)
                {
                    gameObject.Update(this, deltaT);
                }
                if (this.enemies.Position.Y + this.enemies.Size.Height >= this.playerShip.Position.Y - this.playerShip.Image.Height
                    || this.enemies2.Position.Y + this.enemies2.Size.Height >= this.playerShip.Position.Y - this.playerShip.Image.Height
                    || this.enemies3.Position.Y + this.enemies3.Size.Height >= this.playerShip.Position.Y - this.playerShip.Image.Height
                    )
                {
                    this.playerShip.Lives = 0;
                }
                if (!this.playerShip.IsAlive())
                {
                    this.state = GameState.Lost;
                }
                if (!this.enemies.IsAlive())
                {
                    this.Vague2();
                }
                if (!this.enemies2.IsAlive())
                {
                    this.Vague3();
                }
                if (!this.enemies3.IsAlive())
                {
                    this.state = GameState.Win;
                }
            }

            if (this.state == GameState.Win || this.state == GameState.Lost)
            {
                if (this.keyPressed.Contains(Keys.R))
                {
                    RestartGame();
                }
            }

            gameObjects.RemoveWhere(gameObject => !gameObject.IsAlive());
        }

        /// <summary>
        /// Redémarre le jeu.
        /// </summary>
        private void RestartGame()
        {
            this.state = GameState.Play;
            this.gameObjects.Clear();
            this.enemies.reset();
            this.enemies2.reset();
            this.enemies3.reset();
            CreateObject();
        }
        #endregion
    }
}
