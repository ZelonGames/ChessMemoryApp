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
    private LichessMoveLoader lichessMovesLoader;
    private ChessboardGenerator chessBoard;
    private UIChessBoard chessBoardUI;

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

        chessBoard = new ChessboardGenerator(selectedCourse.PlayAsBlack);
        chessBoardUI = new UIChessBoard(chessBoard, mainChessBoard, columnChessBoard);
        
        chessBoard.AddPiecesFromFen(selectedCourse.PreviewFen);
        chessBoardUI.Render();

        // TODO: Fix colorToPlay
        var fenChessBoard = new ChessboardGenerator(chessBoard.boardColorOrientation);
        var fenChessBoardUI = new FenChessboard(fenChessBoard, fenChessBoardLayout, new Size(200, 200));
        fenChessBoardUI.Render();
        #endregion

        #region Initialize Chess Logic Objects
        var moveNotationGenerator = new MoveNotationGenerator(chessBoardUI);
        lichessMovesLoader = new LichessMoveLoader(variationsList, chessBoardUI);
        var courseMoveNavigator = new CourseMoveNavigator(chessBoard, lichessMovesLoader, selectedCourse);
        var pieceMover = new PieceMoverAuto(chessBoard);
        var moveHistory = new MoveHistory(chessBoard);
        var fenSettingsUpdater = new FenSettingsChessBoardUpdater(chessBoard);
        var lichessMoveExplorer = new LichessMoveExplorer(chessBoard);
        var customVariation = editingVariation ?? new CustomVariation(fenChessBoardUI, customVariationMovesList, selectedCourse);
        var customVariationSaver = new VariationManager(chessBoard);
        customVariationSaver.SetVariationToSave(customVariation);
        #endregion

        #region Initialize UI Objects
        ChessableUrlLabel.Install(chessableUrl, chessBoardUI, selectedCourse);
        var lichessFenLabel = new LichessFenLabel(labelLichessFen, chessBoard);
        UILichessButtonsUpdater.Install(customVariation, lichessMovesLoader);
        #endregion

        moveHistory.AddFirstHistoryMove(courseMoveNavigator);

        #region Event Subscriptions
        courseMoveNavigator.SubscribeToEvents(buttonNext);
        pieceMover.SubscribeToEvents(courseMoveNavigator);
        moveHistory.SubscribeToEvents(pieceMover, buttonStart, buttonPrevious);
        fenSettingsUpdater.SubscribeToEvents(pieceMover, moveHistory);
        lichessMoveExplorer.SubscribeToEvents(pieceMover);
        lichessMovesLoader.SubscribeToEvents(lichessMoveExplorer, pieceMover, moveHistory);
        customVariation.SubscribeToEvents(moveHistory);
        customVariationSaver.SubscribeToEvents(buttonSaveVariation);
        lichessFenLabel.SubscribeToEvents(fenSettingsUpdater);
        SizeChanged += chessBoardUI.UpdateSquaresViewSize;
        #endregion

        if (editingVariation != null)
        {
            editingVariation.Initialize(fenChessBoardUI, customVariationMovesList);
            editingVariation.ReloadListButtons();
            chessBoard.AddPiecesFromFen(editingVariation.GetLastFen());
            chessBoard.fenSettings = editingVariation.GetLastFenSettings();
            lichessFenLabel.OnUpdatedFen(editingVariation.GetLastFenSettings().GetLichessFen(chessBoard.GetPositionFen()));
            var backupMoves = new List<MoveHistory.Move>(editingVariation.moves);
            editingVariation.moves.Clear();
            foreach (var move in backupMoves)
                moveHistory.AddMove(move);
            fenChessBoard.AddPiecesFromFen(editingVariation.PreviewFen);
        }

        lichessMoveExplorer.GetLichessMoves(chessBoard.GetPositionFen());
    }

    private async void LabelLichessFromPlayer_Tapped(object sender, TappedEventArgs e)
    {
        checkBoxLichessFromPlayer.IsChecked = !checkBoxLichessFromPlayer.IsChecked;
        LichessRequestHelper.openingsFromPlayer = checkBoxLichessFromPlayer.IsChecked;

        Piece.ColorType color = FenHelper.GetColorFromFen(labelLichessFen.Text);
        bool isYourTurn = chessBoard.boardColorOrientation == color;

        if (isYourTurn)
            return;

        string fen = chessBoard.GetPositionFen();

        var task = Task.Run(async () =>
        {
            OpeningExplorer openingExplorer = await LichessRequestHelper.GetOpeningMoves(
                chessBoard.fenSettings, fen);

            return openingExplorer;
        });

        OpeningExplorer openingExplorer = await task;
        lichessMovesLoader.LoadLichessVariations(fen, openingExplorer);
    }
}

