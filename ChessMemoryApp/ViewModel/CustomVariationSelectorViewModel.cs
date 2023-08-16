using ChessMemoryApp.Model.CourseMaker;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.ViewModel
{
    [QueryProperty("Course", "course")]
    public partial class CustomVariationSelectorViewModel : ObservableObject
    {
        [ObservableProperty]
        Course course;
    }
}
