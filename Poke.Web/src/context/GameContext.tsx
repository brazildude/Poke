import { createContext, useContext, useReducer, useEffect, ReactNode } from 'react';
import { GameState, Character, Skill, LogEntry } from '../types/game';
import { gameService } from '../services/gameService';
import { useAuth } from './AuthContext';

type GameAction = 
  | { type: 'SELECT_CHARACTER'; characterId: string }
  | { type: 'SELECT_TARGET'; targetId: string }
  | { type: 'CLEAR_TARGETS' }
  | { type: 'USE_SKILL'; skillId: string; targetId: string }
  | { type: 'END_TURN' }
  | { type: 'START_NEW_ROUND' }
  | { type: 'SET_GAME_STATE'; state: GameState }
  | { type: 'ADD_LOG_ENTRY'; entry: LogEntry };

interface GameContextType {
  state: GameState;
  selectCharacter: (characterId: string) => void;
  selectTarget: (targetId: string) => void;
  clearTargets: () => void;
  useSkill: (skillId: string, targetId: string) => void;
  endTurn: () => void;
  addLogEntry: (entry: LogEntry) => void;
}

const gameReducer = (state: GameState, action: GameAction): GameState => {
  switch (action.type) {
    case 'SELECT_CHARACTER': {
      const updatedPlayers = state.players.map(player => ({
        ...player,
        characters: player.characters.map(character => ({
          ...character,
          isActive: character.id === action.characterId
        }))
      }));
      
      return { 
        ...state, 
        players: updatedPlayers,
        activeCharacterId: action.characterId,
        selectedTargets: []
      };
    }
    
    case 'SELECT_TARGET': {
      const activeCharacter = findCharacterById(state, state.activeCharacterId);
      const activeSkill = activeCharacter?.skills.find(s => 
        s.currentCooldown === 0 && s.targetType !== 'random'
      );
      
      if (!activeSkill) return state;
      
      let selectedTargets = [...state.selectedTargets];
      
      if (activeSkill.targetType === 'single') {
        selectedTargets = [action.targetId];
      } else if (activeSkill.targetType === 'multiple') {
        const maxTargets = activeSkill.targetCount || 1;
        const targetIndex = selectedTargets.indexOf(action.targetId);
        
        if (targetIndex === -1 && selectedTargets.length < maxTargets) {
          selectedTargets.push(action.targetId);
        } else if (targetIndex !== -1) {
          selectedTargets.splice(targetIndex, 1);
        }
      }
      
      return {
        ...state,
        selectedTargets
      };
    }
    
    case 'CLEAR_TARGETS': {
      return {
        ...state,
        selectedTargets: []
      };
    }
    
    case 'USE_SKILL': {
      const activeCharacter = findCharacterById(state, state.activeCharacterId);
      const skill = activeCharacter?.skills.find(s => s.id === action.skillId);
      
      if (!activeCharacter || !skill || skill.currentCooldown > 0 || activeCharacter.usedSkillThisTurn) return state;
      
      let targets: string[] = [];
      const currentPlayer = state.players.find(p => p.id === state.currentPlayerId);
      const otherPlayer = state.players.find(p => p.id !== state.currentPlayerId);
      
      if (!currentPlayer || !otherPlayer) return state;
      
      // Determine valid targets based on skill type
      if (skill.type === 'heal') {
        // Healing skills can only target friendly units
        targets = [action.targetId];
        const targetCharacter = findCharacterById(state, action.targetId);
        if (!currentPlayer.characters.some(c => c.id === targetCharacter?.id)) {
          return state; // Invalid target for healing
        }
      } else if (skill.targetType === 'random') {
        // Random targeting for enemy units
        const availableTargets = otherPlayer.characters.map(c => c.id);
        const randomIndex = Math.floor(Math.random() * availableTargets.length);
        targets = [availableTargets[randomIndex]];
      } else if (skill.targetType === 'all') {
        // All enemy units
        targets = otherPlayer.characters.map(c => c.id);
      } else {
        // Single or multiple targeting for enemy units
        targets = [action.targetId];
        const targetCharacter = findCharacterById(state, action.targetId);
        if (skill.type !== 'heal' && currentPlayer.characters.some(c => c.id === targetCharacter?.id)) {
          return state; // Invalid target for damage skills
        }
      }
      
      // Apply skill effects to all targets
      const updatedPlayers = state.players.map(player => ({
        ...player,
        characters: player.characters.map(character => {
          if (character.id === activeCharacter.id) {
            // Update the skill cooldown and mark skill as used for this turn
            return {
              ...character,
              usedSkillThisTurn: true,
              skills: character.skills.map(s =>
                s.id === skill.id
                  ? { ...s, currentCooldown: s.cooldown }
                  : s
              )
            };
          }
          
          if (targets.includes(character.id)) {
            // Apply damage or healing to the target
            let health = character.health;
            if (skill.damage) {
              health = Math.max(0, health - skill.damage);
            }
            if (skill.healing) {
              health = Math.min(character.maxHealth, health + skill.healing);
            }
            
            // Apply status effects if any
            let status = [...character.status];
            if (skill.type === 'buff' || skill.type === 'debuff') {
              status.push({
                id: `status-${Date.now()}`,
                name: skill.name,
                description: skill.description,
                duration: 3, // Default duration
                type: skill.type
              });
            }
            
            return { ...character, health, status };
          }
          
          return character;
        })
      }));
      
      const logEntry: LogEntry = {
        id: `log-${Date.now()}`,
        timestamp: Date.now(),
        text: `${activeCharacter.name} uses ${skill.name} on ${targets.length} target(s)${skill.damage ? ` for ${skill.damage} damage` : ''}${skill.healing ? ` healing for ${skill.healing}` : ''}!`,
        type: skill.type,
        sourceCharacterId: activeCharacter.id,
        targetCharacterId: targets[0]
      };
      
      return {
        ...state,
        players: updatedPlayers,
        selectedTargets: [],
        log: [...state.log, logEntry]
      };
    }
    
    case 'END_TURN': {
      const currentPlayerIndex = state.players.findIndex(p => p.id === state.currentPlayerId);
      const nextPlayerIndex = (currentPlayerIndex + 1) % state.players.length;
      
      // Check if all characters have played this round
      const allCharactersPlayed = state.players[currentPlayerIndex].characters.every(
        character => character.usedSkillThisTurn
      );

      let updatedPlayers = state.players;
      let updatedTurnNumber = state.turnNumber;

      if (allCharactersPlayed && nextPlayerIndex === 0) {
        // Start a new round
        updatedTurnNumber++;
        updatedPlayers = state.players.map(player => ({
          ...player,
          isCurrentTurn: player.id === state.players[0].id,
          characters: player.characters.map(character => ({
            ...character,
            isActive: player.id === state.players[0].id && character.id === player.characters[0].id,
            usedSkillThisTurn: false,
            skills: character.skills.map(skill => ({
              ...skill,
              currentCooldown: Math.max(0, skill.currentCooldown - 1)
            })),
            status: character.status
              .map(status => ({
                ...status,
                duration: status.duration - 1
              }))
              .filter(status => status.duration > 0)
          }))
        }));
      } else {
        // Continue with the current round
        updatedPlayers = state.players.map((player, index) => ({
          ...player,
          isCurrentTurn: index === nextPlayerIndex,
          characters: player.characters.map(character => ({
            ...character,
            isActive: index === nextPlayerIndex && character.id === player.characters[0].id,
            usedSkillThisTurn: false,
            skills: character.skills.map(skill => ({
              ...skill,
              currentCooldown: Math.max(0, skill.currentCooldown - 1)
            })),
            status: character.status
              .map(status => ({
                ...status,
                duration: status.duration - 1
              }))
              .filter(status => status.duration > 0)
          }))
        }));
      }
      
      const logEntry: LogEntry = {
        id: `log-turn-${Date.now()}`,
        timestamp: Date.now(),
        text: updatedTurnNumber > state.turnNumber
          ? `Round ${updatedTurnNumber} begins!`
          : `${updatedPlayers[nextPlayerIndex].name} begins their turn.`,
        type: 'system'
      };
      
      return {
        ...state,
        players: updatedPlayers,
        currentPlayerId: updatedPlayers[nextPlayerIndex].id,
        activeCharacterId: updatedPlayers[nextPlayerIndex].characters[0].id,
        turnNumber: updatedTurnNumber,
        selectedTargets: [],
        log: [...state.log, logEntry]
      };
    }
    
    case 'SET_GAME_STATE':
      return action.state;
    
    case 'ADD_LOG_ENTRY':
      return {
        ...state,
        log: [...state.log, action.entry]
      };
    
    default:
      return state;
  }
};

const findCharacterById = (state: GameState, characterId: string): Character | undefined => {
  for (const player of state.players) {
    const character = player.characters.find(c => c.id === characterId);
    if (character) return character;
  }
  return undefined;
};

const GameContext = createContext<GameContextType | undefined>(undefined);

export const GameProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [state, dispatch] = useReducer(gameReducer, {
    id: crypto.randomUUID(),
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
  
  useEffect(() => {
    const saveGame = async () => {
      if (user) {
        try {
          await gameService.updateGameState(state.id, state);
        } catch (error) {
          console.error('Failed to save game state:', error);
        }
      }
    };
    saveGame();
  }, [state, user]);
  
  const selectCharacter = (characterId: string) => {
    dispatch({ type: 'SELECT_CHARACTER', characterId });
  };
  
  const selectTarget = (targetId: string) => {
    dispatch({ type: 'SELECT_TARGET', targetId });
  };
  
  const clearTargets = () => {
    dispatch({ type: 'CLEAR_TARGETS' });
  };
  
  const useSkill = (skillId: string, targetId: string) => {
    dispatch({ type: 'USE_SKILL', skillId, targetId });
  };
  
  const endTurn = () => {
    dispatch({ type: 'END_TURN' });
  };
  
  const addLogEntry = (entry: LogEntry) => {
    dispatch({ type: 'ADD_LOG_ENTRY', entry });
  };
  
  return (
    <GameContext.Provider value={{ 
      state, 
      selectCharacter, 
      selectTarget,
      clearTargets,
      useSkill, 
      endTurn, 
      addLogEntry 
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