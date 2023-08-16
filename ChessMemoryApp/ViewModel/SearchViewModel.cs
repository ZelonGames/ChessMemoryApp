using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Variations;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ChessMemoryApp.ViewModel
{
    [QueryProperty("Course", "course")]
    public partial class SearchViewModel : ObservableObject
    {
        [ObservableProperty]
        Course course;
    }
}
