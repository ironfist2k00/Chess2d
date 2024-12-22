using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Scripts.Core
{
    public class ChessPlayerPlacementHandler : MonoBehaviour
    {
        [SerializeField] public int row, column;
        [SerializeField] private PieceType pieceType;
        
        

        private void Start()
        {
            // Position the piece on the board
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;
            
        }

        private void OnMouseDown()
        {
            // Clear previous highlights and calculate possible moves for the selected piece
            ChessBoardPlacementHandler.Instance.ClearHighlights();
            HighlightMoves();
        }
       /*private void HighlightMoves()
        {
            List<(int, int)> moves = GetValidMoves();

            foreach (var (targetRow, targetCol) in moves)
            {
                var targetTile = ChessBoardPlacementHandler.Instance.GetTile(targetRow, targetCol);

                if (targetTile != null)
                {
                    ChessBoardPlacementHandler.Instance.Highlight(targetRow, targetCol);
                }
            }
        }*/
        private void HighlightMoves()
        {
            List<(int, int)> moves = GetValidMoves();

            foreach (var (targetRow, targetCol) in moves)
            {
                var targetTile = ChessBoardPlacementHandler.Instance.GetTile(targetRow, targetCol);
                if (targetTile != null)
                {
                    // Highlight the valid move tile
                    ChessBoardPlacementHandler.Instance.Highlight(targetRow, targetCol);
                }
                if (targetTile == null) 
                { 
                    continue;
                }
            }
        }
        public void UpdatePiecePosition()
        {
            var targetTile = ChessBoardPlacementHandler.Instance.GetTile(row, column);
            if (targetTile != null)
            {
                transform.position = targetTile.transform.position;
            }
            else
            {
                Debug.LogError($"Tile at row {row}, column {column} not found!");
            }
        }

        /// <summary>
        /// Updates the row and column, and automatically aligns the piece visually.
        /// </summary>
        public void SetPosition(int newRow, int newColumn)
        {
            row = newRow;
            column = newColumn;
            UpdatePiecePosition();
        }

        private List<(int, int)> GetValidMoves()
        {
            // Determine valid moves based on the piece type
            List<(int, int)> moves = new List<(int, int)>();
            switch (pieceType)
            {
                case PieceType.King:
                    AddMovesForKing(moves);
                    break;
                case PieceType.Queen:
                    AddMovesForQueen(moves);
                    break;
                case PieceType.Rook:
                    AddMovesForRook(moves);
                    break;
                case PieceType.Bishop:
                    AddMovesForBishop(moves);
                    break;
                case PieceType.Knight:
                    AddMovesForKnight(moves);
                    break;
                case PieceType.Pawn:
                    AddMovesForPawn(moves);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return moves;
        }
        

        private void AddMovesForKing(List<(int, int)> moves)
        {
            int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                int newRow = row + rowOffsets[i];
                int newCol = column + colOffsets[i];
                if (IsInBounds(newRow, newCol))
                {
                    moves.Add((newRow, newCol));
                }
            }
        }

        private void AddMovesForQueen(List<(int, int)> moves)
        {
            // Add logic for queen's moves (diagonal, horizontal, vertical)
            AddDiagonalMoves(moves);
            AddStraightMoves(moves);
        }

        private void AddMovesForRook(List<(int, int)> moves)
        {
            AddStraightMoves(moves);
        }

        private void AddStraightMoves(List<(int, int)> moves)
        {
            // Horizontal and vertical directions
            int[] rowOffsets = { -1, 1, 0, 0 };
            int[] colOffsets = { 0, 0, -1, 1 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                for (int step = 1; step < 8; step++)
                {
                    int newRow = row + rowOffsets[i] * step;
                    int newCol = column + colOffsets[i] * step;

                    if (!IsInBounds(newRow, newCol))
                    {
                        break;
                    }

                    moves.Add((newRow, newCol));

                    // Stop if a piece is in the way (optional, for capturing logic)
                    if (IsOccupied(newRow, newCol))
                    {
                        break;
                    }
                }
            }
        }
        private void AddMovesForBishop(List<(int, int)> moves)
        {
            AddDiagonalMoves(moves);
        }

        private void AddDiagonalMoves(List<(int, int)> moves)
        {
            // Diagonal directions
            int[] rowOffsets = { -1, -1, 1, 1 };
            int[] colOffsets = { -1, 1, -1, 1 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                for (int step = 1; step < 8; step++)
                {
                    int newRow = row + rowOffsets[i] * step;
                    int newCol = column + colOffsets[i] * step;

                    if (!IsInBounds(newRow, newCol)) break;

                    moves.Add((newRow, newCol));

                    // Stop if a piece is in the way (optional, for capturing logic)
                    if (IsOccupied(newRow, newCol)) break;
                }
            }
        }
        
        private void AddMovesForKnight(List<(int, int)> moves)
        {
            int[] rowOffsets = { -2, -1, 1, 2, 2, 1, -1, -2 };
            int[] colOffsets = { 1, 2, 2, 1, -1, -2, -2, -1 };

            for (int i = 0; i < rowOffsets.Length; i++)
            {
                int newRow = row + rowOffsets[i];
                int newCol = column + colOffsets[i];
                if (IsInBounds(newRow, newCol))
                {
                    moves.Add((newRow, newCol));
                }
            }
        }

        private void AddMovesForPawn(List<(int, int)> moves)
        {
            // Black pawns move in the negative direction on the row axis.
            int direction = IsBlack() ? -1 : 1;

            // Single forward move
            if (IsInBounds(row + direction, column))
            {
                moves.Add((row + direction, column));
            }

            // Double move on the first turn
            if (IsFirstMove() && IsInBounds(row + 2 * direction, column))
            {
                moves.Add((row + 2 * direction, column));
            }
        }
        private bool IsInBounds(int r, int c)
        {
            return r >= 0 && r < 8 && c >= 0 && c < 8;
        }
        private bool IsOccupied(int r, int c)
        {
            GameObject tile = ChessBoardPlacementHandler.Instance.GetTile(r, c);
            return tile != null && tile.transform.childCount > 0;
        }
        
        private bool IsBlack()
        {
            // Example: Use tag or another property to determine the piece color.
            return tag == "Black";
        }
        private bool IsFirstMove()
        {
            return (IsBlack() && row == 6) || (!IsBlack() && row == 1);
        }

        private enum PieceType
        {
            King,
            Queen,
            Rook,
            Bishop,
            Knight,
            Pawn
        }
    }
}
