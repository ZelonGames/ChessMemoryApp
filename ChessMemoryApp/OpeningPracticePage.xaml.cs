using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Opening_Practice;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Components;
using ChessMemoryApp.ViewModel;

namespace ChessMemoryApp;

public partial class OpeningPracticePage : ContentPage
{
    private readonly OpeningPracticeViewModel viewModel;
    private ChessBot chessBot;
    public string chessBotState;

    public OpeningPracticePage(OpeningPracticeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = this.viewModel = viewModel;
        viewModel.State = "State: ";
        Appearing += OnAppearing;
        
    }

    private async void OnAppearing(object sender, EventArgs e)
    {
        var courseLoader = new CourseLoader();
        await courseLoader.LoadCoursesFromDatabase();
        Dictionary<string, Course> courses = courseLoader.GetCourses();
        int startingCourseCount = courses.Count();

        if (startingCourseCount == 0)
            await courseLoader.LoadCoursesFromFile();

        Course chebanenko = courses["Lifetime Repertoires Chebanenko Slav"];
        Chapter Chebanenko_Nf3AndNc3 = chebanenko.GetChapter("White plays Nf3 and Nc3");
        string fenPosition = "rnbqkb1r/pp2pppp/2p2n2/3p4/2PP4/2N2N2/PP2PPPP/R1BQKB1R";
        chessBot = new ChessBot(Piece.ColorType.Black, $"{fenPosition} b KQkq - 3 4");
        chessBot.BotStateChanged += OnBotStateChanged;

        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Ne5 #1");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Nh4 #1");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Bd3 #1");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Be2 #1");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Qb3 #1");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 c5 and 6 Qb3");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 c5 and 6 Bf4 #1");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 a4 and 6 Bf4 #1");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 a4 and 6 Bf4 #2");
        chessBot.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 a4 and 6 g3 #1");

        var chessBoardUI = new UIChessBoard(chessBot.chessboardGenerator, mainChessBoard, columnChessBoard);
        chessBoardUI.Render();
        chessBoardUI.UpdateSquaresViewSize(null, e);
        SizeChanged += chessBoardUI.UpdateSquaresViewSize;

        var openingPracticeInputController = new OpeningPracticeInputController(chessBot, chessBoardUI);
    }

    private void OnBotStateChanged(ChessBot.BotState state)
    {
        viewModel.State = "State: " + state;
    }
}