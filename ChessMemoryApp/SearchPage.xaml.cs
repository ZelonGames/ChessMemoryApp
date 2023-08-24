using ChessMemoryApp.Model;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.ViewModel;

namespace ChessMemoryApp;

public partial class SearchPage : ContentPage
{
    public readonly SearchViewModel searchViewModel;
    private readonly SelectorPageController<ChessableChessboard> selectorPageController;
    private readonly List<ChessableChessboard> customVariationBoards = new();
    private readonly Size boardSize = new(200);

    private List<(Variation variation, Move lastSearchMove)> filteredVariations = new();
    private SearchEngine searchEngine;
    private int variationIndex = 0;
    private int variationChunkSize = 9;

    public SearchPage(SearchViewModel searchViewModel)
    {
        InitializeComponent();
        this.searchViewModel = searchViewModel;
        BindingContext = searchViewModel;

        selectorPageController = new SelectorPageController<ChessableChessboard>(customVariationBoards, coursesLayout, searchViewModel.Course);
        
        SizeChanged += selectorPageController.Window_SizeChanged;
        scrollViewCourses.Scrolled += ScrollViewCourses_Scrolled;
    }

    private void ScrollViewCourses_Scrolled(object sender, ScrolledEventArgs e)
    {
        var scrollView = (ScrollView)sender;

        if (scrollView.ScrollY + scrollView.Height >= scrollView.ContentSize.Height)
        {
            variationIndex += variationChunkSize;
            AddVariationsToScrollView();
        }
    }

    public void FindChessableVariations(object sender, EventArgs e)
    {

        foreach (var chessboard in customVariationBoards)
        {
            chessboard.Clicked -= OnBoardClicked;
            chessboard.ClearBoard();
        }
        customVariationBoards.Clear();
        coursesLayout.Clear();
        variationIndex = 0;

        searchEngine = new SearchEngine(searchViewModel.Course, textBoxFen.Text);
        filteredVariations = searchEngine.ExcludeVariationsByMoveNotation(searchEngine.GetVariationsFromSearchPattern());
        AddVariationsToScrollView();
    }

    public void OpenChessableUrl(object sender, EventArgs e)
    {
        OnBoardClicked(textBoxFen.Text);
    }

    private void AddVariationsToScrollView()
    {
        for (int i = variationIndex; i < variationIndex + variationChunkSize; i++)
        {
            if (i > filteredVariations.Count - 1)
                break;

            var customVariationBoard = new ChessableChessboard(coursesLayout, boardSize);
            customVariationBoard.fen = filteredVariations[i].lastSearchMove.Fen;
            customVariationBoard.LoadChessBoardFromFen(customVariationBoard.fen);
            customVariationBoard.playAsBlack = searchViewModel.Course.PlayAsBlack;
            customVariationBoard.Clicked += OnBoardClicked;
            customVariationBoards.Add(customVariationBoard);
        }

        selectorPageController.Window_SizeChanged(this, null);
    }

    private async void OnBoardClicked(string fen)
    {
        try
        {
            await Launcher.OpenAsync(FenHelper.ConvertFenToChessableUrl(fen, searchViewModel.Course.chessableCourseID.ToString()));
        }
        catch
        {

        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        textBoxFen.SelectionLength = textBoxFen.Text.Length;
    }

    private async void textBoxFen_Focused(object sender, FocusEventArgs e)
    {
        if (textBoxFen.Text != null)
        {
            await Task.Delay(100);
            textBoxFen.CursorPosition = 0;
            textBoxFen.SelectionLength = textBoxFen.Text.Length;
        }
    }
}