using System;


namespace SpaceInvaders
{
    class Vecteur2D
    {
        private double norme;

        /// <summary>
        /// Coordonnée X du vecteur.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Coordonnée Y du vecteur.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Norme du vecteur.
        /// </summary>
        public double Norme
        {
            get
            {
                return Math.Sqrt(this.X * this.X + this.Y * this.Y);
            }
        }

        /// <summary>
        /// Constructeur de la classe Vecteur2D.
        /// </summary>
        /// <param name="X">Coordonnée X initiale.</param>
        /// <param name="Y">Coordonnée Y initiale.</param>
        public Vecteur2D(double X = 0, double Y = 0)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Additionne deux vecteurs.
        /// </summary>
        /// <param name="v1">Premier vecteur.</param>
        /// <param name="v2">Deuxième vecteur.</param>
        /// <returns>Vecteur résultant de l'addition.</returns>
        public static Vecteur2D operator +(Vecteur2D v1, Vecteur2D v2)
        {
            return new Vecteur2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        /// <summary>
        /// Soustrait deux vecteurs.
        /// </summary>
        /// <param name="v1">Premier vecteur.</param>
        /// <param name="v2">Deuxième vecteur.</param>
        /// <returns>Vecteur résultant de la soustraction.</returns>
        public static Vecteur2D operator -(Vecteur2D v1, Vecteur2D v2)
        {
            return new Vecteur2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// Négatif d'un vecteur.
        /// </summary>
        /// <param name="v1">Vecteur à négativer.</param>
        /// <returns>Vecteur négatif.</returns>
        public static Vecteur2D operator -(Vecteur2D v1)
        {
            return new Vecteur2D(-v1.X, v1.Y);
        }

        /// <summary>
        /// Multiplie un vecteur par un scalaire.
        /// </summary>
        /// <param name="v1">Vecteur.</param>
        /// <param name="d">Scalaire.</param>
        /// <returns>Vecteur résultant de la multiplication.</returns>
        public static Vecteur2D operator *(Vecteur2D v1, double d)
        {
            return new Vecteur2D(v1.X * d, v1.Y * d);
        }

        /// <summary>
        /// Multiplie un scalaire par un vecteur.
        /// </summary>
        /// <param name="d">Scalaire.</param>
        /// <param name="v1">Vecteur.</param>
        /// <returns>Vecteur résultant de la multiplication.</returns>
        public static Vecteur2D operator *(double d, Vecteur2D v1)
        {
            return new Vecteur2D(d * v1.X, d * v1.Y);
        }

        /// <summary>
        /// Divise un vecteur par un scalaire.
        /// </summary>
        /// <param name="v1">Vecteur.</param>
        /// <param name="d">Scalaire.</param>
        /// <returns>Vecteur résultant de la division.</returns>
        /// <exception cref="DivideByZeroException">Erreur lorsque le scalaire est égal à zéro.</exception>
        public static Vecteur2D operator /(Vecteur2D v1, double d)
        {
            if (d == 0)
                throw new DivideByZeroException("Division par zéro n'est pas permise.");
            return new Vecteur2D(v1.X / d, v1.Y / d);
        }
    }
}
