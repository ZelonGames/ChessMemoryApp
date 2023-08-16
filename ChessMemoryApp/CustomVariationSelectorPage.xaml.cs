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
        this.selectorPageController = new SelectorPageController<CourseChessboard>(customVariationBoards, coursesLayout, selectedCourse);
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

    public async void ReloadUI(Course course)
    {
        customVariationBoards.ForEach(x => x.ClearBoard());
        customVariationBoards.Clear();
        coursesLayout.Clear();
        var customVariations = await CustomVariationService.GetAll(course);

        CustomVariationChessboard customVariationBoard = null;

        bool shouldAddFirst = customVariations.Count > 6;
        if (shouldAddFirst)
            AddNewCustomVariationBoardButton(customVariationBoard, course.PlayAsBlack);

        foreach (var customVariation in customVariations)
        {
            customVariationBoard = new CustomVariationChessboard(customVariation.Value, coursesLayout, boardSize);
            customVariationBoard.Clicked += PracticeCustomVariation_Clicked;
            customVariationBoard.DeleteClicked += VariationManager.DeleteCustomVariation;
            customVariationBoard.DeleteClicked += CustomVariationBoard_DeleteClicked;
            customVariationBoard.EditClicked += CustomVariationBoard_EditClicked;
            customVariationBoard.playAsBlack = course.PlayAsBlack;
            customVariationBoard.LoadChessBoard();
            customVariationBoard.LoadPieces(customVariation.Value.PreviewFen);
            customVariationBoards.Add(customVariationBoard);
        }

        if (!shouldAddFirst)
            AddNewCustomVariationBoardButton(customVariationBoard, course.PlayAsBlack);

        selectorPageController.Window_SizeChanged(this, null);
    }

    private void AddNewCustomVariationBoardButton(CustomVariationChessboard customVariationBoard, bool playAsBlack)
    {
        customVariationBoard = new CustomVariationChessboard(coursesLayout, boardSize);
        customVariationBoard.playAsBlack = playAsBlack;
        customVariationBoard.LoadChessBoard();
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