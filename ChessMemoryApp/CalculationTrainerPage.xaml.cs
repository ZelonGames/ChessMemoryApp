using ChessMemoryApp.Model;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Threat_Finder;

namespace ChessMemoryApp;

public partial class CalculationTrainerPage : ContentPage
{
	public CalculationTrainerPage()
	{
        string fen = "rnb1k2r/pp2qppp/5n2/2bp4/8/1B1QB2P/PPP2PPN/RN2K2R b KQkq - 2 10";
        //fen = "8/8/8/5p2/4p3/5PP1/8/8 w - - 0 1";
        var a = ThreatEngine.GetThreats("rnb1k2r/pp2qppp/5n2/3p4/Bb6/3QB2P/PPP2PPN/RN2K2R b KQkq - 2 10");
        var threatEngine = new ThreatEngine(5);
        threatEngine.CalculateMoves(fen);
        var lines = threatEngine.GetLines();

        InitializeComponent();
        Appearing += OnAppearing;
    }

    private void OnAppearing(object sender, EventArgs e)
    {
        ChessboardGenerator chessboard = new(mainChessBoard, columnChessBoard, false);
        chessboard.LoadSquares();
        chessboard.LoadChessBoardFromFen(FenHelper.STARTING_FEN);
        SizeChanged += chessboard.UpdateSquaresViewSize;
    }
}