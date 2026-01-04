using System;

namespace gerry.Model
{
    /// <summary>
    /// Sudoku eseményargumentum típusa.
    /// </summary>
    public class GerryEventArgs : EventArgs
    {
        private Boolean _isWon;

        /// <summary>
        /// Győzelem lekérdezése.
        /// </summary>
        public Boolean IsWon { get { return _isWon; } }

        /// <summary>
        /// Sudoku eseményargumentum példányosítása.
        /// </summary>
        /// <param name="isWon">Győzelem lekérdezése.</param>
        /// <param name="gameStepCount">Lépésszám.</param>
        /// <param name="gameTime">Játékidő.</param>
        public GerryEventArgs(Boolean isWon)
        {
            _isWon = isWon;
        }
    }
}
