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
    [QueryProperty("EditingCustomVariation", "editingCustomVariation")]
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        Course course;

        [ObservableProperty]
        CustomVariation editingCustomVariation;
    }
}
