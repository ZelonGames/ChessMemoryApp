﻿using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Services;
using ChessMemoryApp.Model;
using ChessMemoryApp.Model.File_System;
using Microsoft.Maui.Controls.Internals;
using ChessMemoryApp.Model.Game_Analysing;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Threat_Finder;
using ChessMemoryApp.Model.PegList;
using ChessMemoryApp.Model.Opening_Practice;

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
        selectorPageController = new SelectorPageController<CourseChessboard>(customVariationBoards, coursesLayout);
        Appearing += Appeared;
        SizeChanged += selectorPageController.Window_SizeChanged;
    }



    private static int GetAmountOfUniquePositions(Dictionary<string, Course> courses)
    {
        var positions = new HashSet<string>();

        foreach (Course course in courses.Values)
        {
            foreach (var chapter in course.GetChapters())
            {
                foreach (var variation in chapter.Value.GetVariations())
                {
                    if (variation.Key.Contains("Information"))
                        continue;

                    foreach (var move in variation.Value.moves)
                    {
                        positions.Add(move.PositionFen);
                    }
                }
            }
        }

        return positions.Count;
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

        var chessToPeg = new ChessToPegConverter(await PegCollection.LoadPegCollection());
        Chapter quickstarterGuide = courses["Lifetime Repertoires Kan Sicilian"].GetChapter("Quickstarter Guide");
        var a = chessToPeg.GetPegListsFromChapter(quickstarterGuide);

        #region Load Chessboards from courses
        foreach (var course in courses)
        {
            var chessBoard = new ChessboardGenerator(course.Value.PlayAsBlack);
            chessBoard.AddPiecesFromFen(course.Value.PreviewFen);
            var courseBoard = new CourseChessboard(chessBoard, course.Value, coursesLayout, boardSize);
            courseBoard.Clicked += CourseBoard_Clicked;
            courseBoard.Render();
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