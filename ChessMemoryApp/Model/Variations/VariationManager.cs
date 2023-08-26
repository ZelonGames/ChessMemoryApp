using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.File_System;
using ChessMemoryApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Variations
{
    /// <summary>
    /// Deletes and Saves and Updates CustomVariations
    /// </summary>
    public class VariationManager
    {
        private CustomVariation variationToSave;
        private ChessboardGenerator chessboard;

        public VariationManager(ChessboardGenerator chessboard)
        {
            this.chessboard = chessboard;
        }

        public void SubscribeToEvents(Button buttonSaveCustomVariation)
        {
            buttonSaveCustomVariation.Clicked += SaveCustomVariation;
        }

        public static async void DeleteCustomVariation(CustomVariationChessboard customVariationChessBoard)
        {
            await CustomVariationService.Remove(customVariationChessBoard.customVariation);
        }

        public void SetVariationToSave(CustomVariation variationToSave)
        {
            this.variationToSave = variationToSave;
        }

        public async void SaveCustomVariation(object sender, EventArgs args)
        {
            if (variationToSave == null || !FenHelper.IsValidFen(variationToSave.PreviewFen))
                return;
            
            variationToSave.CourseName = variationToSave.Course.Name;
            await CustomVariationService.Add(variationToSave);
        }
    }
}
