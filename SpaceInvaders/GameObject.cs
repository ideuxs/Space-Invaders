using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SpaceInvaders
{
    /// <summary>
    /// This is the generic abstact base class for any entity in the game
    /// </summary>
    abstract class GameObject
    {
        public enum Side
        {
            Ally,
            Enemy,
            Neutral,
            Jeton,
        }
        public Side Camp { get; set; }
        /// <summary>
        /// Constructeur de la classe GameObject.
        /// </summary>
        /// <param name="camp">Camp auquel appartient l'objet.</param>
        public GameObject(Side camp)
        {
            this.Camp = camp;
        }

        /// <summary>
        /// Update the state of a game objet
        /// </summary>
        /// <param name="gameInstance">instance of the current game</param>
        /// <param name="deltaT">time ellapsed in seconds since last call to Update</param>
        public abstract void Update(Game gameInstance, double deltaT);

        /// <summary>
        /// Render the game object
        /// </summary>
        /// <param name="gameInstance">instance of the current game</param>
        /// <param name="graphics">graphic object where to perform rendering</param>
        public abstract void Draw(Game gameInstance, Graphics graphics);

        /// <summary>
        /// Determines if object is alive. If false, the object will be removed automatically.
        /// </summary>
        /// <returns>Am I alive ?</returns>
        public abstract bool IsAlive();


        /// <summary>
        /// Gère la collision avec un missile.
        /// </summary>
        /// <param name="m">Missile en collision avec l'objet.</param>
        public abstract void Collision(Missile m);
    }
}
