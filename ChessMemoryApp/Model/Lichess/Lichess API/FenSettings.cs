using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using Newtonsoft.Json;

namespace ChessMemoryApp.Model.Lichess.Lichess_API
{
    public struct FenSettings
    {
        public struct FenColor
        {
            public const string WHITE = "w";
            public const string BLACK = "b";

            public static string GetColorFromPieceColor(Piece.ColorType colorType)
            {
                return colorType == Piece.ColorType.White ? WHITE : BLACK;
            }

            public static string GetColorFromChessBoard(ChessboardGenerator chessBoard)
            {
                return chessBoard.playAsBlack ? BLACK : WHITE;
            }

            public static string GetOppositeColor(string color)
            {
                return color == WHITE ? BLACK : WHITE;
            }

            public static Piece.ColorType GetPieceColor(string fenColor)
            {
                return fenColor == WHITE ? Piece.ColorType.White : Piece.ColorType.Black;
            }
        }

        public struct SpaceEncoding
        {
            /// <summary>
            /// Used for lichess requests
            /// </summary>
            [JsonProperty("percent")]
            public const string PERCENT = "%20";

            /// <summary>
            /// Used for chessable url
            /// </summary>
            [JsonProperty("underscore")]
            public const string UNDERSCORE = "_";

            /// <summary>
            /// Used to display normal fen
            /// </summary>
            [JsonProperty("space")]
            public const string SPACE = " ";
        }

        public string AppliedFenSettings => GetAppliedSettings(spaceEncoding);
        [JsonProperty("plyCountSincePawnMoveSetting")]
        private char plyCountSincePawnMoveSetting = '0';
        [JsonProperty("moveCount")]
        private char moveCount = '1';
        [JsonProperty("colorToPlaySetting")]
        private string colorToPlaySetting;
        [JsonProperty("whiteKingSideSetting")]
        private string whiteKingSideSetting;
        [JsonProperty("blackKingSideSetting")]
        private string blackKingSideSetting;
        [JsonProperty("whiteQueenSideSetting")]
        private string whiteQueenSideSetting;
        [JsonProperty("blackQueenSideSetting")]
        private string blackQueenSideSetting;
        [JsonProperty("spaceEncoding")]
        private string spaceEncoding = SpaceEncoding.PERCENT;

        public FenSettings()
        {
        }

        public bool CanWhiteCastleKingSide => whiteKingSideSetting == "K";
        public bool CanWhiteCastleQueenSide => whiteQueenSideSetting == "Q";
        public bool CanBlackCastleKingSide => blackKingSideSetting == "k";
        public bool CanBlackCastleQueenSide => blackQueenSideSetting == "q";

        public void UpdateMoveAndPlyCount(string fen)
        {
            string[] fenComponents = fen.Split('-')[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            plyCountSincePawnMoveSetting = Convert.ToChar(fenComponents[0]);
            moveCount = Convert.ToChar(fenComponents[1]);
        }

        public string GetLichessFen(string fen)
        {
            return fen.Split(' ')[0] + GetAppliedSettings(SpaceEncoding.SPACE);
        }

        public string GetAppliedSettings(string spaceEncoding)
        {
            return spaceEncoding + colorToPlaySetting + spaceEncoding + GetCastlingSettings(spaceEncoding);
        }

        public FenSettings SetSpaceEncoding(string spaceEncoding)
        {
            this.spaceEncoding = spaceEncoding;
            return this;
        }

        public FenSettings SetColorToPlay(string color)
        {
            colorToPlaySetting = color;
            return this;
        }

        public FenSettings SwitchColor()
        {
            colorToPlaySetting = colorToPlaySetting == FenColor.WHITE ? FenColor.BLACK : FenColor.WHITE;
            return this;
        }

        public FenSettings IncreasePlyCount()
        {
            plyCountSincePawnMoveSetting += (char)1;
            return this;
        }

        public FenSettings ResetPlyCount()
        {
            plyCountSincePawnMoveSetting = '0';
            return this;
        }

        public FenSettings IncreaseMoveCount()
        {
            moveCount += (char)1;
            return this;
        }

        public FenSettings DecreaseMoveCount()
        {
            moveCount -= (char)1;
            return this;
        }

        public FenSettings ResetMoveCount()
        {
            moveCount = '1';
            return this;
        }

        public FenSettings EnableWhiteKingSideCastling()
        {
            whiteKingSideSetting = "K";
            return this;
        }

        public FenSettings DisableWhiteKingSideCastling()
        {
            whiteKingSideSetting = "";
            return this;
        }

        public FenSettings EnableBlackKingSideCastling()
        {
            blackKingSideSetting = "k";
            return this;
        }

        public FenSettings DisableBlackKingSideCastling()
        {
            blackKingSideSetting = "";
            return this;
        }

        public FenSettings EnableWhiteQueenSideCastling()
        {
            whiteQueenSideSetting = "Q";
            return this;
        }

        public FenSettings DisableWhiteQueenSideCastling()
        {
            whiteQueenSideSetting = "";
            return this;
        }

        public FenSettings EnableBlackQueenSideCastling()
        {
            blackQueenSideSetting = "q";
            return this;
        }

        public FenSettings DisableBlackQueenSideCastling()
        {
            blackQueenSideSetting = "";
            return this;
        }

        public FenSettings EnableAllCastleMoves(FenSettings fenSettings)
        {
            fenSettings.EnableBlackKingSideCastling();
            fenSettings.EnableBlackQueenSideCastling();
            fenSettings.EnableWhiteKingSideCastling();
            fenSettings.EnableWhiteQueenSideCastling();

            return fenSettings;
        }

        public FenSettings EnableAllCastleMoves()
        {
            EnableBlackKingSideCastling();
            EnableBlackQueenSideCastling();
            EnableWhiteKingSideCastling();
            EnableWhiteQueenSideCastling();

            return this;
        }

        public string GetColorToPlaySetting()
        {
            return colorToPlaySetting;
        }

        private string GetCastlingSettings(string spaceEncoding)
        {
            string castling = whiteKingSideSetting + whiteQueenSideSetting + blackKingSideSetting + blackQueenSideSetting;
            if (castling == string.Empty)
                return spaceEncoding == SpaceEncoding.PERCENT ? spaceEncoding + "-" + spaceEncoding : "";
            else
                return castling + spaceEncoding;
        }
    }
}

