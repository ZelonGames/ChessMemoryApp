using ChessMemoryApp.Model;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Threat_Finder;
using ChessMemoryApp.Model.UI_Components;

namespace ChessMemoryApp;

public partial class CalculationTrainerPage : ContentPage
{
	public CalculationTrainerPage()
	{
        //string fen = "rnb1k2r/pp2qppp/5n2/2bp4/8/1B1QB2P/PPP2PPN/RN2K2R b KQkq - 2 10";
        //fen = "8/8/8/5p2/4p3/5PP1/8/8 w - - 0 1";
        //var threatEngine = new ThreatEngine(new ChessboardGenerator(), 5);
        //threatEngine.GetThreats("rnb1k2r/pp2qppp/5n2/3p4/Bb6/3QB2P/PPP2PPN/RN2K2R b KQkq - 2 10");
        //threatEngine.CalculateMoves(fen);
        //var lines = threatEngine.GetLines();

        InitializeComponent();
        Appearing += OnAppearing;
    }

    private void OnAppearing(object sender, EventArgs e)
    {
        //var chessboard = new ChessboardGenerator(mainChessBoard, columnChessBoard, false);

        var chessBoard = new ChessboardGenerator(false);
        var uiChessBoard = new UIChessBoard(chessBoard, mainChessBoard, columnChessBoard);
        chessBoard.AddPiecesFromFen(FenHelper.STARTING_FEN);
        uiChessBoard.Render();

        /*
        var moveNotationGenerator = new MoveNotationGenerator(chessboard);
        var threatCalculationTeacher = new ThreatCalculationTeacher(chessboard);
        chessboard.LoadSquares();
        chessboard.LoadChessBoardFromFen(FenHelper.STARTING_FEN);
        */

        SizeChanged += uiChessBoard.UpdateSquaresViewSize;
    }
}