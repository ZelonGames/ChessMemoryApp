using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.UI_Helpers.Main_Page
{
    public class UIHelper
    {
        public UIHelper() { }

        public static int PieceSize;

        #region Get

        public static bool GetCheckBox_PlayAsBlack_Value()
        {
            return false;
        }

        public static bool GetCheckBox_RepeatVariation_value()
        {
            return false;
        }

        public static int GetNumericUpDown_Start_Value()
        {
            return 0;
        }

        public static int GetNumericUpDown_Depth_Value()
        {
            return 0;
        }

        public static int GetCurrentChessableCourseID()
        {
            return 0;
        }

        public static string GetFenTextBoxValue()
        {
            return "";
        }

        #endregion

        #region Set

        public static void SetNumericUpDown_Start_Value(int value)
        {
        }

        public static void SetNumericUpDown_Depth_Value(int value)
        {
        }

        public static string SetChessUrlText(string chessUrl)
        {
            return chessUrl;
        }

        public static string SetCheckBox_CurrentMove_Value(string value)
        {
            return value;
        }

        public static bool SetCheckBox_LichessResponses(bool value)
        {
            return value;
        }

        public static string SetTextBox_Fen_Value(string fen)
        {
            return fen;
        }

        #endregion

        #region Update

        public static void UpdateCurrentMoveText()
        {

        }

        #endregion
    }
}
