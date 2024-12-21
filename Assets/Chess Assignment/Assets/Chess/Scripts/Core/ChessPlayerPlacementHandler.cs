using System;
using UnityEngine;

namespace Chess.Scripts.Core {
    public class ChessPlayerPlacementHandler : MonoBehaviour {
        [SerializeField] public int row, column;


        private void Start() {
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;
        }
        private void OnMouseDown()
        {
            // When the piece is clicked, calculate and highlight moves
            ChessBoardPlacementHandler.Instance.ClearHighlights();

            // Check the piece type by tag or name and call the appropriate method
            string pieceType = gameObject.name; // E.g., "Knight", "King", etc.
            switch (pieceType)
            {
                case "Knight":
                    ChessBoardPlacementHandler.Instance.CalculateKnightMoves(row, column);
                    break;
                case "King":
                    ChessBoardPlacementHandler.Instance.CalculateKingMoves(row, column);
                    break;
                // Add other cases as needed (Rook, Bishop, etc.)
                default:
                    Debug.LogError("Unhandled piece type: " + pieceType);
                    break;
            }
        }
    }
}
 
