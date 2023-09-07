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
    public class FenSettings
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
                return chessBoard.boardColorOrientation == Piece.ColorType.Black ? BLACK : WHITE;
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

        public string AppliedFenSettings => GetAppliedSettings(_SpaceEncoding);
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
        [JsonProperty("enPassantSquare")]
        private string enPassantSquare;
        [JsonProperty("spaceEncoding")]
        public string _SpaceEncoding { get; private set; }
        
        public FenSettings()
        {
            _SpaceEncoding = SpaceEncoding.PERCENT;
        }

        public FenSettings Copy()
        {
            return (FenSettings)MemberwiseClone();
        }

        public bool CanWhiteCastleKingSide => whiteKingSideSetting == "K";
        public bool CanWhiteCastleQueenSide => whiteQueenSideSetting == "Q";
        public bool CanBlackCastleKingSide => blackKingSideSetting == "k";
        public bool CanBlackCastleQueenSide => blackQueenSideSetting == "q";
        public bool CanEnPassant => GetEnPassantSquare() != "-";

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

        /// <summary>
        /// Begins with a spaceEncoding
        /// </summary>
        /// <param name="spaceEncoding"></param>
        /// <returns></returns>
        public string GetAppliedSettings(string spaceEncoding)
        {
            return spaceEncoding + colorToPlaySetting + spaceEncoding + GetCastlingSettings(spaceEncoding) + spaceEncoding + GetEnPassantSquare();
        }

        public FenSettings SetSpaceEncoding(string spaceEncoding)
        {
            this._SpaceEncoding = spaceEncoding;
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

        public void EnableWhiteKingSideCastling()
        {
            whiteKingSideSetting = "K";
        }

        public void DisableWhiteKingSideCastling()
        {
            whiteKingSideSetting = "";
        }

        public void EnableBlackKingSideCastling()
        {
            blackKingSideSetting = "k";
        }

        public void DisableBlackKingSideCastling()
        {
            blackKingSideSetting = "";
        }

        public void EnableWhiteQueenSideCastling()
        {
            whiteQueenSideSetting = "Q";
        }

        public void DisableWhiteQueenSideCastling()
        {
            whiteQueenSideSetting = "";
        }

        public void EnableBlackQueenSideCastling()
        {
            blackQueenSideSetting = "q";
        }

        public void DisableBlackQueenSideCastling()
        {
            blackQueenSideSetting = "";
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns null or the coordinates of the square</returns>
        public string GetEnPassantSquare()
        {
            return enPassantSquare != null && enPassantSquare.Length == 2 ? enPassantSquare : "-";
        }

        public FenSettings SetEnPassantSquare(string enPassantSquare)
        {
            this.enPassantSquare = enPassantSquare;
            return this;
        }

        public FenSettings DisableEnPassant()
        {
            enPassantSquare = "-";
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

