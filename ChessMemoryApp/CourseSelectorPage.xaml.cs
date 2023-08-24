using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Services;
using ChessMemoryApp.Model;
using ChessMemoryApp.Model.File_System;
using Microsoft.Maui.Controls.Internals;
using ChessMemoryApp.Model.Game_Analysing;

namespace ChessMemoryApp;

public partial class CourseSelectorPage : ContentPage
{
    public CourseLoader courseLoader = new();

    private readonly SelectorPageController<CourseChessboard> selectorPageController;
    private readonly List<CourseChessboard> customVariationBoards = new();
    private readonly Size boardSize = new(200, 200);
    private Course selectedCourse = null;

    public CourseSelectorPage()
    {
        InitializeComponent();
        selectorPageController = new SelectorPageController<CourseChessboard>(customVariationBoards, coursesLayout, selectedCourse);
        Appearing += Appeared;
        SizeChanged += selectorPageController.Window_SizeChanged;
    }



    public async void Appeared(object sender, EventArgs e)
    {
        //var stockfish = new StockfishAnalyser(25);

        #region Reset Page
        customVariationBoards.ForEach(x => x.ClearBoard());
        customVariationBoards.Clear();
        coursesLayout.Clear();
        #endregion

        //await CourseService.RemoveAll();

        #region Load Courses
        await courseLoader.LoadCoursesFromDatabase();
        Dictionary<string, Course> courses = courseLoader.GetCourses();
        int startingCourseCount = courses.Count();

        if (startingCourseCount == 0)
            await courseLoader.LoadCoursesFromFile();
        #endregion

        #region Load Chessboards from courses
        foreach (var course in courses)
        {
            var courseBoard = new CourseChessboard(course.Value, coursesLayout, boardSize);
            courseBoard.Clicked += CourseBoard_Clicked;
            courseBoard.playAsBlack = course.Value.PlayAsBlack;
            courseBoard.LoadChessBoardFromFen(course.Value.PreviewFen);
            customVariationBoards.Add(courseBoard);
            if (startingCourseCount == 0)
                await CourseService.Add(course.Value);
        }
        #endregion

        selectorPageController.Window_SizeChanged(this, null);
    }

    private async void CourseBoard_Clicked(Course course)
    {
        selectedCourse = course;
        var parameters = new Dictionary<string, object>()
                {
                    { "course", selectedCourse },
                };
        await Shell.Current.GoToAsync(nameof(CustomVariationSelectorPage), parameters);
    }
}