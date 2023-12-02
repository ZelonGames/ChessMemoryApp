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
    [QueryProperty("State", "state")]
    public partial class OpeningPracticeViewModel : ObservableObject
    {
        [ObservableProperty]
        public string state;
    }
}
