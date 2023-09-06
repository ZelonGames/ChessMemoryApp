using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.Variations;
using ChessMemoryApp.Model.Lichess;
using ChessMemoryApp.ViewModel;
using ChessMemoryApp.Model.UI_Components;
using ChessMemoryApp.Model.Lichess.Lichess_API;
using ChessMemoryApp.Services;

namespace ChessMemoryApp;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel mainViewModel;
    private VariationLoader variationLoader;
    private ChessboardGenerator chessBoard;

    public static MainPage Instance { get; private set; }

    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        this.mainViewModel = mainViewModel;
        BindingContext = mainViewModel;
        Instance = this;
        Appearing += MainPage_Appearing;
    }

    private void MainPage_Appearing(object sender, EventArgs e)
    {
        Course selectedCourse = mainViewModel.Course;
        CustomVariation editingVariation = mainViewModel.EditingCustomVariation;

        #region Create Chessboards
        chessBoard = new ChessboardGenerator(mainChessBoard, columnChessBoard, selectedCourse.PlayAsBlack);
        chessBoard.LoadSquares();
        chessBoard.LoadChessBoardFromFen(selectedCourse.PreviewFen);

        var fenChessBoard = new FenChessboard(fenChessBoardLayout, new Size(200, 200), chessBoard.colorToPlay);
        fenChessBoard.LoadSquares();
        #endregion

        #region Initialize Chess Logic Objects
        var moveNotationGenerator = new MoveNotationGenerator(chessBoard);
        variationLoader = new VariationLoader(variationsList, chessBoard);
        var courseMoveNavigator = new CourseMoveNavigator(chessBoard, variationLoader, selectedCourse);
        var pieceMover = new PieceMover(moveNotationGenerator, false);
        var moveHistory = new MoveHistory(chessBoard);
        var fenSettingsUpdater = new FenSettingsChessBoardUpdater(chessBoard);
        var lichessMoveExplorer = new LichessMoveExplorer(chessBoard);
        var customVariation = editingVariation ?? new CustomVariation(fenChessBoard, customVariationMovesList, selectedCourse);
        var customVariationSaver = new VariationManager(chessBoard);
        customVariationSaver.SetVariationToSave(customVariation);
        #endregion

        #region Initialize UI Objects
        var chessableUrlLabel = new ChessableUrlLabel(chessableUrl, chessBoard, selectedCourse);
        var lichessFenLabel = new LichessFenLabel(labelLichessFen, chessBoard);
        #endregion

        moveHistory.AddFirstHistoryMove(courseMoveNavigator);

        #region Event Subscriptions
        courseMoveNavigator.SubscribeToEvents(buttonNext);
        pieceMover.SubscribeToEvents(courseMoveNavigator);
        moveHistory.SubscribeToEvents(pieceMover, buttonStart, buttonPrevious);
        fenSettingsUpdater.SubscribeToEvents(pieceMover, moveHistory);
        lichessMoveExplorer.SubscribeToEvents(pieceMover);
        variationLoader.SubscribeToEvents(lichessMoveExplorer, pieceMover, moveHistory);
        customVariation.SubscribeToEvents(moveHistory);
        customVariationSaver.SubscribeToEvents(buttonSaveVariation);
        lichessFenLabel.SubscribeToEvents(fenSettingsUpdater);
        SizeChanged += chessBoard.UpdateSquaresViewSize;
        #endregion

        if (editingVariation != null)
        {
            editingVariation.Initialize(fenChessBoard, customVariationMovesList);
            editingVariation.ReloadListButtons();
            chessBoard.LoadChessBoardFromFen(editingVariation.GetLastFen());
            chessBoard.fenSettings = editingVariation.GetLastFenSettings();
            lichessFenLabel.FenSettingsUpdater_UpdatedFen(editingVariation.GetLastFenSettings().GetLichessFen(chessBoard.currentFen));
            var backupMoves = new List<MoveHistory.Move>(editingVariation.moves);
            editingVariation.moves.Clear();
            foreach (var move in backupMoves)
                moveHistory.AddMove(move);
            fenChessBoard.LoadChessBoardFromFen(editingVariation.PreviewFen);
        }

        lichessMoveExplorer.GetLichessMoves(chessBoard.currentFen);
    }

    private async void LabelLichessFromPlayer_Tapped(object sender, TappedEventArgs e)
    {
        checkBoxLichessFromPlayer.IsChecked = !checkBoxLichessFromPlayer.IsChecked;
        LichessRequestHelper.openingsFromPlayer = checkBoxLichessFromPlayer.IsChecked;

        Piece.ColorType color = FenHelper.GetColorFromFen(labelLichessFen.Text);
        bool isYourTurn = chessBoard.colorToPlay == color;

        if (isYourTurn)
            return;

        var task = Task.Run(async () =>
        {
            OpeningExplorer openingExplorer = await LichessRequestHelper.GetOpeningMoves(
                chessBoard.fenSettings, chessBoard.currentFen);

            return openingExplorer;
        });

        OpeningExplorer openingExplorer = await task;
        variationLoader.LoadLichessVariations(chessBoard.currentFen, openingExplorer);
    }
}

