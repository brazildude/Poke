import { doc, getDoc, setDoc, updateDoc } from 'firebase/firestore';
import { GameData, GameState } from '../types/game';
import { db } from '../lib/firebase';
import { serverConfig } from '../config/serverConfig';

export const gameService = {
  async createGame(userId: string): Promise<GameData> {
    const gameRef = doc(db, 'games', crypto.randomUUID());
    const initialState = await this.generateInitialState(userId);
    
    const gameData: GameData = {
      id: gameRef.id,
      createdAt: Date.now(),
      updatedAt: Date.now(),
      players: initialState.players,
      state: initialState,
    };

    await setDoc(gameRef, gameData);
    return gameData;
  },

  async getGame(gameId: string): Promise<GameData | null> {
    const gameRef = doc(db, 'games', gameId);
    const gameDoc = await getDoc(gameRef);
    return gameDoc.exists() ? gameDoc.data() as GameData : null;
  },

  async updateGameState(gameId: string, state: GameState): Promise<void> {
    const gameRef = doc(db, 'games', gameId);
    await updateDoc(gameRef, {
      state,
      updatedAt: Date.now(),
    });
    await serverConfig.saveGameState(gameId, state);
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
        id: "1",
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