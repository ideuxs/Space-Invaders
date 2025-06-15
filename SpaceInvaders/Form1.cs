using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {
        #region fields
        /// <summary>
        /// Instance of the game
        /// </summary>
        private Game game;
        private Size gameSize;
        private float zoom = 0.6f;
        private Image invadersImage;
        private Image backgroundImage;
        #region time management
        /// <summary>
        /// Game watch
        /// </summary>
        Stopwatch watch = new Stopwatch();

        /// <summary>
        /// Last update time
        /// </summary>
        long lastTime = 0;
        #endregion

        #endregion

        #region constructor
        /// <summary>
        /// Create form, create game
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            this.PlaySong();
            this.ClientSize = new Size(800, 750);
            this.gameSize = new Size((int)(this.ClientSize.Width * this.zoom), (int)(this.ClientSize.Height * this.zoom));
            this.invadersImage = Image.FromFile("Resources/invaders.png");
            this.backgroundImage = Image.FromFile("Resources/background.png");
            this.game = Game.CreateGame(this.gameSize);
            this.watch.Start();
            this.WorldClock.Start();
            
        }
        #endregion

        #region events
        /// <summary>
        /// Paint event of the form, see msdn for help => paint game with double buffering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(e.Graphics, e.ClipRectangle);
            Graphics g = bg.Graphics;
            g.Clear(Color.White);

            int x = (this.ClientSize.Width - this.gameSize.Width) / 2;
            int y = (this.ClientSize.Height - this.gameSize.Height)-50;

            
            g.DrawImage(this.invadersImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            g.DrawImage(this.backgroundImage, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            g.TranslateTransform(x, y);
            this.game.Draw(g);
            g.ResetTransform(); 

            bg.Render();
            bg.Dispose();
        }

        /// <summary>
        /// Joue la musique de fond.
        /// </summary>
        private void PlaySong()
        {
            Task.Run(() =>
            {
                using (var audioFile = new AudioFileReader("Resources/theme.wav"))
                using (var outputDevice = new WaveOutEvent())
                {
                    audioFile.Volume = 0.2f;
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Task.Delay(100).Wait();
                    }
                }
            });
        }

        /// <summary>
        /// Tick event => update game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorldClock_Tick(object sender, EventArgs e)
        {
            int maxDelta = 5;

            long nt = this.watch.ElapsedMilliseconds;
            double deltaT = (nt - this.lastTime);

            for (; deltaT >= maxDelta; deltaT -= maxDelta)
                this.game.Update(maxDelta / 1000.0);

            this.game.Update(deltaT / 1000.0);
            this.lastTime = nt;

            Invalidate();
        }

        /// <summary>
        /// Key down event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            this.game.keyPressed.Add(e.KeyCode);
        }

        /// <summary>
        /// Key up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            this.game.keyPressed.Remove(e.KeyCode);
        }

        #endregion

        private void GameForm_Load(object sender, EventArgs e)
        {

        }
    }
}

