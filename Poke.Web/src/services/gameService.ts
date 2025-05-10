import { GameData, GameState } from '../types/game';
import { serverService } from './serverService';

interface MovePayload {
  characterId: string;
  targets: string[];
  skillId: string;
}

export const gameService = {
  async createGame(userId: string): Promise<GameData> {
    try {
      return await serverService.post<GameData>('/games', { userId });
    } catch (error) {
      console.error('Error creating game:', error);
      throw error;
    }
  },

  async getGame(gameId: string): Promise<GameData | null> {
    try {
      return await serverService.get<GameData>(`/games/${gameId}`);
    } catch (error) {
      console.error('Error fetching game:', error);
      throw error;
    }
  },

  async makeMove(gameId: string, moveData: MovePayload): Promise<GameData> {
    try {
      return await serverService.post<GameData>(`/games/${gameId}/moves`, moveData);
    } catch (error) {
      console.error('Error making move:', error);
      throw error;
    }
  },

  async endTurn(gameId: string): Promise<GameData> {
    try {
      return await serverService.post<GameData>(`/games/${gameId}/end-turn`, {});
    } catch (error) {
      console.error('Error ending turn:', error);
      throw error;
    }
  }
};