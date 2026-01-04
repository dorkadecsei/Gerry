using gerry.Model;
using gerry.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace gerry.ViewModel
{
    /// <summary>
    /// Sudoku nézetmodell típusa.
    /// </summary>
    public class GerryViewModel : ViewModelBase
    {
        #region Fields

        private GerryGameModel _model; // modell

        #endregion

        #region Properties

        /// <summary>
        /// Új játék kezdése parancs lekérdezése.
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }

        /// <summary>
        /// Játék betöltése parancs lekérdezése.
        /// </summary>
        public DelegateCommand LoadGameCommand { get; private set; }

        /// <summary>
        /// Játék mentése parancs lekérdezése.
        /// </summary>
        public DelegateCommand SaveGameCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Játékmező gyűjtemény lekérdezése.
        /// </summary>
        public ObservableCollection<GerryField> Fields { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Új játék eseménye.
        /// </summary>
        public event EventHandler? NewGame;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler? ExitGame;

        #endregion

        #region Constructors

        /// <summary>
        /// Sudoku nézetmodell példányosítása.
        /// </summary>
        /// <param name="model">A modell típusa.</param>
        public GerryViewModel(GerryGameModel model)
        {
            // játék csatlakoztatása
            _model = model;
            _model.FieldChanged += new EventHandler<GerryFieldEventArgs>(Model_FieldChanged);
            _model.GameAdvanced += new EventHandler<GerryEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<GerryEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<GerryEventArgs>(Model_GameCreated);

            // parancsok kezelése
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            // játéktábla létrehozása
            Fields = new ObservableCollection<GerryField>();
            for (Int32 i = 0; i < _model.TableSize; i++) // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model.TableSize; j++)
                {
                    Fields.Add(new GerryField
                    {
                        IsLocked = true,
                        Text = String.Empty,
                        X = i,
                        Y = j,
                        StepCommand = new DelegateCommand(param =>
                        {
                            if (param is Tuple<Int32, Int32> position)
                                StepGame(position.Item1, position.Item2);
                        })
                        // ha egy mezőre léptek, akkor jelezzük a léptetést, változtatjuk a lépésszámot
                    });
                }
            }

            RefreshTable();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Tábla frissítése.
        /// </summary>
        private void RefreshTable()
        {
            foreach (GerryField field in Fields) // inicializálni kell a mezőket is
            {
                field.Text = !_model.IsEmpty(field.X, field.Y) ? _model[field.X, field.Y].ToString() : String.Empty;
                field.IsLocked = _model.IsLocked(field.X, field.Y);
            }

        }

        /// <summary>
        /// Játék léptetése eseménykiváltása.
        /// </summary>
        /// <param name="x">A lépett mező X koordinátája.</param>
        /// <param name="y">A lépett mező Y koordinátája.</param>
        private void StepGame(Int32 x, Int32 y)
        {
            _model.Step(x, y);
        }

        #endregion

        #region Game event handlers

        /// <summary>
        /// Játékmodell mező megváltozásának eseménykezelője.
        /// </summary>
        private void Model_FieldChanged(object? sender, GerryFieldEventArgs e)
        {
            // mező frissítése
            GerryField field = Fields.Single(f => f.X == e.X && f.Y == e.Y);

            field.Text =
                !_model.IsEmpty(field.X, field.Y)
                    ? _model[field.X, field.Y].ToString()
                    : String.Empty; // visszaírjuk a szöveget
        }

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object? sender, GerryEventArgs e)
        {
            foreach (GerryField field in Fields)
            {
                field.IsLocked = true; // minden mezőt lezárunk
            }
        }

        /// <summary>
        /// Játék előrehaladásának eseménykezelője.
        /// </summary>
        private void Model_GameAdvanced(object? sender, GerryEventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess()) // hamisat ad vissza, ha nem a dispatcher thread-en vagyunk
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(() => { Model_GameAdvanced(sender, e); });
                return;
            }
        }

        /// <summary>
        /// Játék létrehozásának eseménykezelője.
        /// </summary>
        private void Model_GameCreated(object? sender, GerryEventArgs e)
        {
            RefreshTable();
        }

        #endregion

        #region Event methods

        /// <summary>
        /// Új játék indításának eseménykiváltása.
        /// </summary>
        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Játékból való kilépés eseménykiváltása.
        /// </summary>
        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
