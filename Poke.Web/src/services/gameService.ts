import { GameData, Team } from '../types/game';
import { serverService } from './serverService';

interface MovePayload {
  characterId: string;
  targets: string[];
  skillId: string;
}

interface MatchmakingResponse {
  status: 'queued' | 'matched' | 'cancelled';
  gameId?: string;
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
  },

  async getTeams(): Promise<Team[]> {
    try {
      return await serverService.get<Team[]>('/teams');
    } catch (error) {
      console.error('Error fetching teams:', error);
      throw error;
    }
  },

  async startMatchmaking(teamId: string): Promise<MatchmakingResponse> {
    try {
      return await serverService.post<MatchmakingResponse>('/matchmaking/queue', { teamId });
    } catch (error) {
      console.error('Error starting matchmaking:', error);
      throw error;
    }
  },

  async cancelMatchmaking(): Promise<void> {
    try {
      await serverService.post('/matchmaking/cancel');
    } catch (error) {
      console.error('Error canceling matchmaking:', error);
      throw error;
    }
  },

  async checkMatchmakingStatus(): Promise<MatchmakingResponse> {
    try {
      return await serverService.get<MatchmakingResponse>('/matchmaking/status');
    } catch (error) {
      console.error('Error checking matchmaking status:', error);
      throw error;
    }
  }
};