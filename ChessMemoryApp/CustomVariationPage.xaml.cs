using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Components;
using ChessMemoryApp.Model.Variations;
using ChessMemoryApp.Services;
using ChessMemoryApp.ViewModel;

namespace ChessMemoryApp;

public partial class CustomVariationPage : ContentPage
{
    private readonly CustomVariationViewModel customVariationViewModel;
    private CustomVariation customVariation;

    public CustomVariationPage(CustomVariationViewModel customVariationViewModel)
	{
		InitializeComponent();
        BindingContext = customVariationViewModel;
        this.customVariationViewModel = customVariationViewModel;
        Appearing += CustomVariationPage_Appearing;
	}

    private void OnCopyMovesClicked(object sender, EventArgs e)
    {
        string moves = "";
        foreach (MoveHistory.Move move in customVariation.moves)
            moves += move.moveNotationCoordinates + " ";

        Clipboard.SetTextAsync(moves);
    }

    private async void CustomVariationPage_Appearing(object sender, EventArgs e)
    {
        customVariation = customVariationViewModel.CustomVariation;
        Course course = customVariation.Course;

        var chessBoard = new ChessboardGenerator(course.PlayAsBlack);
        var chessBoardUI = new UIChessBoard(chessBoard, mainChessBoard, columnChessBoard);
        var moveNotationGenerator = new MoveNotationGenerator(chessBoardUI);
        var customVariationMoveNavigator = new CustomVariationMoveNavigator(customVariation);
        var pieceMover = new PieceMoverManual(chessBoardUI, moveNotationGenerator, customVariationMoveNavigator);
        var chessableUrlLabel = new ChessableUrlLabel(chessableUrl, chessBoardUI, course);
        var commentLoader = new CommentLoader(editorComment);
        var commentManager = new CommentManager(editorComment, chessBoard);

        chessBoard.AddPiecesFromFen(customVariation.GetStartingFen());
        chessBoardUI.Render();

        customVariationMoveNavigator.SubscribeToEvents(moveNotationGenerator, buttonStart, buttonPrevious, buttonNext, buttonEnd);
        moveNotationGenerator.SubscribeToEvents(customVariationMoveNavigator);
        commentLoader.SubscribeToEvents(customVariationMoveNavigator);
        commentManager.SubscribeToEvents(buttonCommentManager, commentLoader);

        SizeChanged += chessBoardUI.UpdateSquaresViewSize;

        await commentLoader.LoadComment(chessBoard.GetPositionFen());
    }
}