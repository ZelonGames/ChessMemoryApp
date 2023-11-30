using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Opening_Practice;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.UI_Components;

namespace ChessMemoryApp;

public partial class OpeningPracticePage : ContentPage
{
    private ChessEngine chessEngine;

    public OpeningPracticePage()
    {
        InitializeComponent();
        Appearing += OpeningPracticePage_Appearing;
    }

    private async void OpeningPracticePage_Appearing(object sender, EventArgs e)
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
        chessEngine = new ChessEngine(Piece.ColorType.Black, $"{fenPosition} b KQkq - 3 4");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Ne5 #1");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Nh4 #1");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Bd3 #1");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Be2 #1");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 e3 and 6 Qb3 #1");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 c5 and 6 Qb3");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 c5 and 6 Bf4 #1");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 a4 and 6 Bf4 #1");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 a4 and 6 Bf4 #2");
        chessEngine.AddVariation(Chebanenko_Nf3AndNc3, "White Plays Nf3 and Nc3: 5 a4 and 6 g3 #1");

        var chessBoardUI = new UIChessBoard(chessEngine.chessboardGenerator, mainChessBoard, columnChessBoard);
        chessBoardUI.Render();
        SizeChanged += chessBoardUI.UpdateSquaresViewSize;

        var openingPracticeInputController = new OpeningPracticeInputController(chessEngine, chessBoardUI);
    }
}