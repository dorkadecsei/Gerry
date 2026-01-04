using System;
using System.Threading.Tasks;
using gerry.Persistence;

namespace gerry.Model
{
    /// <summary>
    /// Játéknehézség felsorolási típusa.
    /// </summary>
    public enum GameDifficulty { Easy, Medium, Hard }

    /// <summary>
    /// Sudoku játék típusa.
    /// </summary>
    public class GerryGameModel
    {

        #region Fields

        private GerryTable _table; // játéktábla
        
        #endregion

        #region Properties

        /// <summary>
        /// Játéktábla méretének lekérdezése.
        /// </summary>
        public Int32 TableSize => _table.Size;

        /// <summary>
        /// Mező értékének lekérdezése.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>Mező értéke.</returns>
        public Int32 this[Int32 x, Int32 y] => _table[x, y];

       
        
        /// <summary>
        /// Játék végének lekérdezése.
        /// </summary>
        public Boolean IsGameOver
        {
            get { return (_table.IsFilled); }
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Mező megváltozásának eseménye.
        /// </summary>
        public event EventHandler<GerryFieldEventArgs>? FieldChanged;

        /// <summary>
        /// Játék előrehaladásának eseménye.
        /// </summary>
        public event EventHandler<GerryEventArgs>? GameAdvanced;

        /// <summary>
        /// Játék végének eseménye.
        /// </summary>
        public event EventHandler<GerryEventArgs>? GameOver;

        /// <summary>
        /// Játék létrehozásának eseménye.
        /// </summary>
        public event EventHandler<GerryEventArgs>? GameCreated;

        #endregion

        #region Constructor

        /// <summary>
        /// Sudoku játék példányosítása.
        /// </summary>
        /// <param name="dataAccess">Az adatelérés.</param>
        public GerryGameModel()
        {
            _table = new GerryTable();
        }

        #endregion

        #region Public table accessors

        /// <summary>
        /// Mező értékének lekérdezése.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>A mező értéke.</returns>
        public Int32 GetValue(Int32 x, Int32 y) => _table.GetValue(x, y);

        /// <summary>
        /// Mező kitöltetlenségének lekérdezése.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>Igaz, ha a mező ki van töltve, egyébként hamis.</returns>
        public Boolean IsEmpty(Int32 x, Int32 y) => _table.IsEmpty(x, y);

        /// <summary>
        /// Mező zároltságának lekérdezése.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        /// <returns>Igaz, ha a mező zárolva van, különben hamis.</returns>
        public Boolean IsLocked(Int32 x, Int32 y) => _table.IsLocked(x, y);

        #endregion

        #region Public game methods

        /// <summary>
        /// Új játék kezdése.
        /// </summary>
        public void NewGame()
        {
            _table = new GerryTable();

            OnGameCreated();
        }


        /// <summary>
        /// Táblabeli lépés végrehajtása.
        /// </summary>
        /// <param name="x">Vízszintes koordináta.</param>
        /// <param name="y">Függőleges koordináta.</param>
        public void Step(Int32 x, Int32 y)
        {
            if (IsGameOver) // ha már vége a játéknak, nem játszhatunk
                return;
            if (_table.IsLocked(x, y)) // ha a mező zárolva van, nem léphetünk
                return;

            _table.StepValue(x, y);
            OnFieldChanged(x, y);

            OnGameAdvanced();

            if (_table.IsFilled) // ha vége a játéknak, jelezzük, hogy győztünk
            {
                OnGameOver(true);
            }
        }

        #endregion

        #region Private game methods

        /// <summary>
        /// Mezők generálása.
        /// </summary>
        /// <param name="count">Mezők száma.</param>
        private void GenerateFields(Int32 count)
        {
            Random random = new Random();

            for (Int32 i = 0; i < count; i++)
            {
                Int32 x, y;

                do
                {
                    x = random.Next(_table.Size);
                    y = random.Next(_table.Size);
                } while (!_table.IsEmpty(x, y)); // üres mező véletlenszerű kezelése

                do
                {
                    for (Int32 j = random.Next(10) + 1; j >= 0; j--) // véletlenszerű növelés
                    {
                        _table.StepValue(x, y);
                    }
                } while (_table.IsEmpty(x, y));

                _table.SetLock(x, y);
            }
        }

        #endregion

        #region Private event methods

        /// <summary>
        /// Mező változás eseményének kiváltása.
        /// <param name="x">Mező X koordináta.</param>
        /// <param name="y">Mező Y koordináta.</param>
        /// </summary>
        private void OnFieldChanged(Int32 x, Int32 y)
        {
            FieldChanged?.Invoke(this, new GerryFieldEventArgs(x, y));
        }

        /// <summary>
        /// Játékidő változás eseményének kiváltása.
        /// </summary>
        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, new GerryEventArgs(false));
        }

        /// <summary>
        /// Játék vége eseményének kiváltása.
        /// </summary>
        /// <param name="isWon">Győztünk-e a játékban.</param>
        private void OnGameOver(Boolean isWon)
        {
            GameOver?.Invoke(this, new GerryEventArgs(isWon));
        }

        /// <summary>
        /// Játék létrehozás eseményének kiváltása.
        /// </summary>
        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, new GerryEventArgs(false));
        }

        #endregion
    }
}
