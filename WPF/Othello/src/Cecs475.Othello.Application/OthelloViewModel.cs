﻿using System.Collections.Generic;
using System.Linq;
using Cecs475.Othello.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace Cecs475.Othello.Application {
	public class OthelloSquare : INotifyPropertyChanged {
		private int mPlayer;
		public int Player {
			get { return mPlayer; }
			set {
				if (value != mPlayer) {
					mPlayer = value;
					OnPropertyChanged(nameof(Player));
				}
			}
		}

		public BoardPosition Position {
			get; set;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public class OthelloViewModel : INotifyPropertyChanged {
		private OthelloBoard mBoard;
		private ObservableCollection<OthelloSquare> mSquares;

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string name) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
        
        public UndoCommand UndoCommand { get; set; }

        public OthelloViewModel() {

            mBoard = new OthelloBoard();
			mSquares = new ObservableCollection<OthelloSquare>(
				BoardPosition.GetRectangularPositions(8, 8)
				.Select(p =>new OthelloSquare() {
					Position = p,
					Player = mBoard.GetPlayerAtPosition(p)
				})
			);

			PossibleMoves = new HashSet<BoardPosition>(mBoard.GetPossibleMoves().Select(m => m.Position));

            this.UndoCommand = new UndoCommand(this);
        }

		public void ApplyMove(BoardPosition position) {
			var possMoves = mBoard.GetPossibleMoves() as IEnumerable<OthelloMove>;
			foreach (var move in possMoves) {
				if (move.Position.Equals(position)) {
					mBoard.ApplyMove(move);
					break;
				}
			}

			PossibleMoves = new HashSet<BoardPosition>(mBoard.GetPossibleMoves().Select(m => m.Position));
			var newSquares = BoardPosition.GetRectangularPositions(8, 8);
			int i = 0;
			foreach (var pos in newSquares) {
				mSquares[i].Player = mBoard.GetPlayerAtPosition(pos);
				i++;
			}
			OnPropertyChanged(nameof(CurrentAdvantage));
            OnPropertyChanged(nameof(CurrentPlayer));
        }

        public void UndoLastMove()
        {
            mBoard.UndoLastMove();
            OnPropertyChanged(nameof(CurrentAdvantage));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(MoveHistory));
        }

        public ObservableCollection<OthelloSquare> Squares {
			get { return mSquares; }
		}

		public HashSet<BoardPosition> PossibleMoves {
			get; private set;
		}

		public GameAdvantage CurrentAdvantage { get { return mBoard.CurrentAdvantage; } }

        public int CurrentPlayer { get { return mBoard.CurrentPlayer; } }

        public IEnumerable<OthelloMove> MoveHistory { get { return mBoard.MoveHistory; } }


    }
}
