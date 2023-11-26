using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
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

    private void OnCopyMovesCoordinatesClicked(object sender, EventArgs e)
    {
        string moves = "";
        foreach (MoveHistory.Move move in customVariation.moves)
            moves += move.moveNotationCoordinates + " ";

        Clipboard.SetTextAsync(moves);
    }

    private void OnCopyMovesDigitCoordinatesClicked(object sender, EventArgs e)
    {
        string columns = "abcdefgh";
        string moves = "";
        foreach (MoveHistory.Move move in customVariation.moves)
        {
            string column = (columns.IndexOf(move.moveNotationCoordinates[0]) + 1).ToString();
            string row = move.moveNotationCoordinates[1].ToString();
            string column2 = (columns.IndexOf(move.moveNotationCoordinates[2]) + 1).ToString();
            string row2 = move.moveNotationCoordinates[3].ToString();
            moves += column + row + column2 + row2 + " ";
        }

        Clipboard.SetTextAsync(moves);
    }

    private void OnCopyMovesDigitToCoordinatesClicked(object sender, EventArgs e)
    {
        string columns = "abcdefgh";
        string moves = "";
        foreach (MoveHistory.Move move in customVariation.moves)
        {
            if (move.moveNotation is "O-O" or "O-O-O")
            {
                string row = move.color == Piece.ColorType.White ? "1" : "8";
                moves += move.moveNotation == "O-O" ? "7" + row + " ": "3" + row + " ";
                continue;
            }

            string column2 = (columns.IndexOf(move.moveNotationCoordinates[2]) + 1).ToString();
            string row2 = move.moveNotationCoordinates[3].ToString();

            moves += column2 + row2 + " ";
        }

        Clipboard.SetTextAsync(moves);
    }

    private void OnCopyMovesClicked(object sender, EventArgs e)
    {
        string moves = "";
        foreach (MoveHistory.Move move in customVariation.moves)
            moves += move.moveNotation + " ";

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
        var chessableUrlLabel = new ChessableUrlLabel(chessableUrl, chessBoardUI, customVariation);
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