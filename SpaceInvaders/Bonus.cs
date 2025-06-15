using SpaceInvaders;
using static SpaceInvaders.GameObject;
using System.Drawing;

class Bonus : SimpleObject
{
    string whichBonus;

    /// <summary>
    /// Constructeur de la classe Bonus.
    /// </summary>
    /// <param name="position">Position initiale du bonus.</param>
    /// <param name="image">Image du bonus.</param>
    /// <param name="gameInstance">Instance du jeu.</param>
    /// <param name="pWhichBonus">Type de bonus.</param>
    public Bonus(Vecteur2D position, Bitmap image, Game gameInstance, string pWhichBonus) : base(Side.Jeton, gameInstance)
    {
        this.whichBonus = pWhichBonus;
        this.Position = position;
        this.Image = image;
        this.Lives = 1;
    }

    /// <summary>
    /// Gère la collision entre le bonus et un missile.
    /// </summary>
    /// <param name="missile">Missile en collision avec le bonus.</param>
    /// <param name="numberOfPixelsInCollision">Nombre de pixels en collision.</param>
    protected override void OnCollision(Missile missile, int numberOfPixelsInCollision) { }

    /// <summary>
    /// Met à jour l'état du bonus.
    /// </summary>
    /// <param name="gameInstance">Instance du jeu.</param>
    /// <param name="deltaT">Temps écoulé.</param>
    public override void Update(Game gameInstance, double deltaT)
    {
        this.Position = new Vecteur2D(this.Position.X, this.Position.Y + 100 * deltaT);

        if (this.Position.Y > gameInstance.playerShip.Position.Y + 20)
        {
            this.Lives = 0;
        }

        PlayerSpaceship player = gameInstance.playerShip;
        Rectangle bonusRectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Image.Width, this.Image.Height);
        Rectangle playerRectangle = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Image.Width, player.Image.Height);

        if (RectanglesIntersect(bonusRectangle, playerRectangle))
        {
            this.Lives = 0;
            ApplyBonus(player);
        }
    }

    /// <summary>
    /// Applique le bonus au joueur.
    /// </summary>
    /// <param name="player">Le vaisseau du joueur.</param>
    private void ApplyBonus(PlayerSpaceship player)
    {
        if (this.whichBonus.Equals("coin"))
        {
            player.Money += 100;
        }
        else
        {
            player.Lives += 1;
        }
    }
}
