import { GameData, GameState } from '../types/game';
import { serverConfig } from '../config/serverConfig';

export const gameService = {
  async createGame(userId: string): Promise<GameData> {
    try {
      const response = await fetch(`${serverConfig.API_BASE_URL}/games`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ userId })
      });
      
      if (!response.ok) {
        throw new Error('Failed to create game');
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error creating game:', error);
      throw error;
    }
  },

  async getGame(gameId: string): Promise<GameData | null> {
    try {
      const response = await fetch(`${serverConfig.API_BASE_URL}/games/${gameId}`);
      
      if (!response.ok) {
        if (response.status === 404) {
          return null;
        }
        throw new Error('Failed to fetch game');
      }
      
      return await response.json();
    } catch (error) {
      console.error('Error fetching game:', error);
      throw error;
    }
  },

  async updateGameState(gameId: string, state: GameState): Promise<void> {
    try {
      const response = await fetch(`${serverConfig.API_BASE_URL}/games/${gameId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ state })
      });
      
      if (!response.ok) {
        throw new Error('Failed to update game state');
      }
    } catch (error) {
      console.error('Error updating game state:', error);
      throw error;
    }
  },

  async generateInitialState(userId: string): Promise<GameState> {
    try {
      const [characters, skills] = await Promise.all([
        serverConfig.fetchCharacters(),
        serverConfig.fetchSkills(),
      ]);

      const charactersWithSkills = characters.map(character => ({
        ...character,
        skills: skills.filter(skill => skill.characterClass === character.class),
        isActive: false,
        usedSkillThisTurn: false,
      }));

      const midPoint = Math.ceil(charactersWithSkills.length / 2);
      const player1Characters = charactersWithSkills.slice(0, midPoint);
      const player2Characters = charactersWithSkills.slice(midPoint);

      if (player1Characters.length > 0) {
        player1Characters[0].isActive = true;
      }

      const initialState: GameState = {
        id: crypto.randomUUID(),
        players: [
          {
            id: userId,
            name: 'Player 1',
            characters: player1Characters,
            isCurrentTurn: true,
          },
          {
            id: 'ai-opponent',
            name: 'Player 2',
            characters: player2Characters,
            isCurrentTurn: false,
          },
        ],
        currentPlayerId: userId,
        activeCharacterId: player1Characters[0].id,
        log: [{
          id: 'initial-log',
          timestamp: Date.now(),
          text: 'Game has started!',
          type: 'system',
        }],
        turnNumber: 1,
        gameStatus: 'in-progress',
        selectedTargets: [],
      };

      return initialState;
    } catch (error) {
      console.error('Error generating initial state:', error);
      throw error;
    }
  },
};