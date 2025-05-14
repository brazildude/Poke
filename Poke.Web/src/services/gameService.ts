import { GameData, GameState, Team, PlayMove } from '../types/game';
import { serverService } from './serverService';

interface MatchmakingResponse {
  status: 'queued' | 'matched' | 'cancelled';
  matchID?: number;
}

export const gameService = {
  async createGame(userId: string): Promise<GameData> {
    try {
      return await serverService.get<GameData>('/api/plays');
    } catch (error) {
      console.error('Error creating game:', error);
      throw error;
    }
  },

  async getGame(gameId: number): Promise<GameData | null> {
    try {
      return await serverService.get<GameData>(`/api/plays/${gameId}`);
    } catch (error) {
      console.error('Error fetching game:', error);
      throw error;
    }
  },

  async makeMove(matchID: number, moveData: PlayMove): Promise<GameData> {
    try {
      return await serverService.post<GameData>('/api/plays', moveData);
    } catch (error) {
      console.error('Error making move:', error);
      throw error;
    }
  },

  async endTurn(gameId: number): Promise<GameData> {
    try {
      return await serverService.post<GameData>(`/api/plays/${gameId}/end-turn`, {});
    } catch (error) {
      console.error('Error ending turn:', error);
      throw error;
    }
  },

  async getTeams(): Promise<Team[]> {
    try {
      return await serverService.post<Team[]>('/api/users/teams');
    } catch (error) {
      console.error('Error fetching teams:', error);
      throw error;
    }
  },

  async startMatchmaking(teamID: number): Promise<string> {
    try {
      return await serverService.get<string>(`/api/matchmaking/join?teamID=${teamID}`);
    } catch (error) {
      console.error('Error starting matchmaking:', error);
      throw error;
    }
  },

  async cancelMatchmaking(): Promise<void> {
    try {
      await serverService.get('/api/matchmaking/cancel');
    } catch (error) {
      console.error('Error canceling matchmaking:', error);
      throw error;
    }
  },

  async checkMatchmakingStatus(): Promise<string> {
    try {
      return await serverService.get<string>('/api/matchmaking/wait');
    } catch (error) {
      console.error('Error checking matchmaking status:', error);
      throw error;
    }
  }
};