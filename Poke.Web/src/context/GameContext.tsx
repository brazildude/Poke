import { createContext, useContext, useReducer, useEffect, ReactNode } from 'react';
import { GameState, Character, Skill, LogEntry } from '../types/game';
import { gameService } from '../services/gameService';
import { useAuth } from './AuthContext';

type GameAction = 
  | { type: 'SELECT_CHARACTER'; characterId: string }
  | { type: 'CLEAR_SELECTION' }
  | { type: 'SET_GAME_STATE'; state: GameState }
  | { type: 'SET_TARGETS'; targets: string[] };

interface GameContextType {
  state: GameState;
  selectCharacter: (characterId: string) => void;
  clearSelection: () => void;
  makeMove: (characterId: string, targets: string[], skillId: string) => Promise<void>;
  endTurn: () => Promise<void>;
}

const gameReducer = (state: GameState, action: GameAction): GameState => {
  switch (action.type) {
    case 'SELECT_CHARACTER': {
      // Only allow selecting characters if it's the player's turn
      const currentPlayer = state.players.find(p => p.id === state.currentPlayerId);
      const character = currentPlayer?.characters.find(c => c.id === action.characterId);
      
      if (!currentPlayer?.isCurrentTurn || !character || character.usedSkillThisTurn) {
        return state;
      }

      return {
        ...state,
        activeCharacterId: action.characterId,
        selectedTargets: []
      };
    }
    
    case 'CLEAR_SELECTION': {
      return {
        ...state,
        activeCharacterId: '',
        selectedTargets: []
      };
    }
    
    case 'SET_GAME_STATE':
      return action.state;
    
    case 'SET_TARGETS':
      return {
        ...state,
        selectedTargets: action.targets
      };
    
    default:
      return state;
  }
};

const GameContext = createContext<GameContextType | undefined>(undefined);

export const GameProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [state, dispatch] = useReducer(gameReducer, {
    id: '',
    players: [],
    currentPlayerId: '',
    activeCharacterId: '',
    log: [],
    turnNumber: 1,
    gameStatus: 'waiting',
    selectedTargets: []
  });
  
  const { user } = useAuth();
  
  useEffect(() => {
    if (user) {
      const loadGame = async () => {
        try {
          const gameData = await gameService.createGame(user.uid);
          dispatch({ type: 'SET_GAME_STATE', state: gameData.state });
        } catch (error) {
          console.error('Failed to load game:', error);
        }
      };
      loadGame();
    }
  }, [user]);
  
  const selectCharacter = (characterId: string) => {
    dispatch({ type: 'SELECT_CHARACTER', characterId });
  };
  
  const clearSelection = () => {
    dispatch({ type: 'CLEAR_SELECTION' });
  };
  
  const makeMove = async (characterId: string, targets: string[], skillId: string) => {
    try {
      const response = await gameService.makeMove(state.id, {
        characterId,
        targets,
        skillId
      });
      dispatch({ type: 'SET_GAME_STATE', state: response.state });
    } catch (error) {
      console.error('Failed to make move:', error);
      throw error;
    }
  };
  
  const endTurn = async () => {
    try {
      const response = await gameService.endTurn(state.id);
      dispatch({ type: 'SET_GAME_STATE', state: response.state });
    } catch (error) {
      console.error('Failed to end turn:', error);
      throw error;
    }
  };
  
  return (
    <GameContext.Provider value={{ 
      state, 
      selectCharacter,
      clearSelection,
      makeMove,
      endTurn
    }}>
      {children}
    </GameContext.Provider>
  );
};

export const useGame = (): GameContextType => {
  const context = useContext(GameContext);
  if (!context) {
    throw new Error('useGame must be used within a GameProvider');
  }
  return context;
};