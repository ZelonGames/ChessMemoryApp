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

    public CustomVariationPage(CustomVariationViewModel customVariationViewModel)
	{
		InitializeComponent();
        BindingContext = customVariationViewModel;
        this.customVariationViewModel = customVariationViewModel;
        Appearing += CustomVariationPage_Appearing;
	}

    private async void CustomVariationPage_Appearing(object sender, EventArgs e)
    {
        CustomVariation customVariation = customVariationViewModel.CustomVariation;
        Course course = customVariation.Course;
        var chessboard = new ChessboardGenerator(mainChessBoard, columnChessBoard, course.PlayAsBlack);
        var moveNotationHelper = new MoveNotationGenerator(chessboard);
        var customVariationMoveNavigator = new CustomVariationMoveNavigator(customVariation);
        var pieceMover = new PieceMover(moveNotationHelper, customVariationMoveNavigator, true);
        var chessableUrlLabel = new ChessableUrlLabel(chessableUrl, chessboard, course);
        var commentLoader = new CommentLoader(editorComment);
        var commentManager = new CommentManager(editorComment, chessboard);

        chessboard.LoadSquares();
        chessboard.LoadChessBoardFromFen(customVariation.GetStartingFen());

        customVariationMoveNavigator.SubscribeToEvents(moveNotationHelper, buttonStart, buttonPrevious, buttonNext, buttonEnd);
        moveNotationHelper.SubscribeToEvents(customVariationMoveNavigator);
        commentLoader.SubscribeToEvents(customVariationMoveNavigator);
        commentManager.SubscribeToEvents(buttonCommentManager, commentLoader);

        SizeChanged += chessboard.UpdateSquaresViewSize;

        await commentLoader.LoadComment(chessboard.GetFen());
    }
}