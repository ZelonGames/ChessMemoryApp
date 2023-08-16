using ChessMemoryApp.Model.Variations;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.ViewModel
{
    [QueryProperty("CustomVariation", "customVariation")]
    public partial class CustomVariationViewModel : ObservableObject
    {
        [ObservableProperty]
        CustomVariation customVariation;
    }
}
