﻿using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Variations
{
    /// <summary>
    /// Saves and Removes comments
    /// </summary>
    public class CommentManager
    {
        private readonly Editor editorComment;
        private readonly ChessboardGenerator chessboard;
        private Button buttonCommentManager;
        private Comment loadedComment;

        public CommentManager(Editor editorComment, ChessboardGenerator chessboard)
        {
            this.editorComment = editorComment;
            this.chessboard = chessboard;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribers">Button, CommentLoader</param>
        public void SubscribeToEvents(Button buttonSaveComment, CommentLoader commentLoader)
        {
            buttonSaveComment.Clicked += ButtonCommentManager_Clicked;
            buttonCommentManager = buttonSaveComment;
            commentLoader.LoadedComment += CommentLoader_LoadedComment;
        }

        private void CommentLoader_LoadedComment(Comment loadedComment)
        {
            this.loadedComment = loadedComment;
        }

        private void ButtonCommentManager_Clicked(object sender, EventArgs e)
        {
            if (editorComment.IsEnabled)
                SaveComment(editorComment.Text);

            editorComment.IsEnabled = !editorComment.IsEnabled;
            buttonCommentManager.Text = editorComment.IsEnabled ? "Save Comment" : "Edit Comment";
        }

        private async void SaveComment(string text)
        {
            if (text == "")
            {
                await CommentService.Remove(loadedComment);
                loadedComment = null;
                return;
            }

            if (loadedComment != null)
            {
                loadedComment.Text = text;
                await CommentService.Update(loadedComment);
            }
            else
            {
                var comment = new Comment()
                {
                    Fen = chessboard.GetPositionFen(),
                    Text = text,
                };

                await CommentService.Add(comment);
            }
        }
    }
}
