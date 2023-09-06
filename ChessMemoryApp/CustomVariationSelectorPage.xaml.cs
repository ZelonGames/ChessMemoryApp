using ChessMemoryApp.Model;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Variations;
using ChessMemoryApp.Services;
using ChessMemoryApp.ViewModel;

namespace ChessMemoryApp;

public partial class CustomVariationSelectorPage : ContentPage
{
    private readonly CustomVariationSelectorViewModel customVariationSelectorViewModel;
    private readonly SelectorPageController<CourseChessboard> selectorPageController;
    private readonly List<CourseChessboard> customVariationBoards = new();
    private readonly Size boardSize = new(200);
    private Course selectedCourse = null;

    public CustomVariationSelectorPage(CustomVariationSelectorViewModel customVariationSelectorViewModel)
    {
        InitializeComponent();
        this.customVariationSelectorViewModel = customVariationSelectorViewModel;
        BindingContext = customVariationSelectorViewModel;
        selectorPageController = new SelectorPageController<CourseChessboard>(customVariationBoards, coursesLayout);
        Appearing += CustomVariationSelectorPage_Appearing;
    }

    private void CustomVariationSelectorPage_Appearing(object sender, EventArgs e)
    {
        selectedCourse = customVariationSelectorViewModel.Course;
        ReloadUI(customVariationSelectorViewModel.Course);
    }

    private async void CustomVariationBoard_DeleteClicked(CustomVariationChessboard customVariationChessBoard)
    {
        await CustomVariationService.Remove(customVariationChessBoard.customVariation);
        ReloadUI(selectedCourse);
    }

    public async void GotToSearchPage(object sender, EventArgs args)
    {
        var parameters = new Dictionary<string, object>()
                {
                    { "course", selectedCourse },
                };
        await Shell.Current.GoToAsync(nameof(SearchPage), parameters);
    }

    private async Task UpdateVariationSortingOrder(Dictionary<string, CustomVariation> customVariations, string previewFen, int sortingOrder)
    {
        previewFen = previewFen.Split(' ')[0];
        CustomVariation customVariation = customVariations.Where(x => x.Value.PreviewFen.Split(' ')[0] == previewFen).FirstOrDefault().Value;
        customVariation.SortingOrder = sortingOrder;
        await CustomVariationService.Update(customVariation);
    }

    public async void ReloadUI(Course course)
    {
        customVariationBoards.ForEach(x => x.ClearBoard());
        customVariationBoards.Clear();
        coursesLayout.Clear();
        var customVariations = await CustomVariationService.GetAllFromCourse(course);
        CustomVariationChessboard customVariationBoard = null;

        AddNewCustomVariationBoardButton(customVariationBoard, course.PlayAsBlack ? Piece.ColorType.Black : Piece.ColorType.White);

        foreach (var customVariation in customVariations.OrderBy(x => x.Value.SortingOrder))
        {
            customVariationBoard = new CustomVariationChessboard(customVariation.Value, coursesLayout, boardSize);
            customVariationBoard.Clicked += PracticeCustomVariation_Clicked;
            customVariationBoard.DeleteClicked += VariationManager.DeleteCustomVariation;
            customVariationBoard.DeleteClicked += CustomVariationBoard_DeleteClicked;
            customVariationBoard.EditClicked += CustomVariationBoard_EditClicked;
            customVariationBoard.colorToPlay = course.PlayAsBlack ? Piece.ColorType.Black : Piece.ColorType.White;
            customVariationBoard.LoadChessBoardFromFen(customVariation.Value.PreviewFen);
            customVariationBoards.Add(customVariationBoard);
        }

        selectorPageController.Window_SizeChanged(this, null);
    }

    private void AddNewCustomVariationBoardButton(CustomVariationChessboard customVariationBoard, Piece.ColorType colorToPlay)
    {
        customVariationBoard = new CustomVariationChessboard(coursesLayout, boardSize);
        customVariationBoard.colorToPlay = colorToPlay;
        customVariationBoard.LoadSquares();
        customVariationBoards.Add(customVariationBoard);
        customVariationBoard.Clicked += AddCustomVariation_Clicked;
    }

    private async void PracticeCustomVariation_Clicked(CustomVariationChessboard customVariationChessBoard)
    {
        var parameters = new Dictionary<string, object>()
                {
                    { "customVariation", customVariationChessBoard.customVariation },
                };
        await Shell.Current.GoToAsync(nameof(CustomVariationPage), parameters);
    }

    private async void CustomVariationBoard_EditClicked(CustomVariationChessboard customVariationChessBoard)
    {
        var parameters = new Dictionary<string, object>()
                {
                    { "course", customVariationChessBoard.customVariation.Course },
                    { "editingCustomVariation", customVariationChessBoard.customVariation },
                };
        await Shell.Current.GoToAsync(nameof(MainPage), parameters);
    }

    private async void AddCustomVariation_Clicked(CustomVariationChessboard customVariationChessBoard)
    {
        var parameters = new Dictionary<string, object>()
                {
                    { "course", selectedCourse },
                    { "editingCustomVariation", null },
                };
        await Shell.Current.GoToAsync(nameof(MainPage), parameters);
    }
}