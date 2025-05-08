import { Character, Skill, GameState } from '../types/game';
import { mockCharacters, mockSkills } from '../data/mockServerData';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://api.example.com';
const USE_MOCK_SERVER = import.meta.env.VITE_USE_MOCK_SERVER === 'true';
const MOCK_API_DELAY = 500;

async function mockApiCall<T>(mockData: T): Promise<T> {
  await new Promise(resolve => setTimeout(resolve, MOCK_API_DELAY));
  return mockData;
}

export const serverConfig = {
  async fetchCharacters(): Promise<Character[]> {
    if (USE_MOCK_SERVER) {
      return mockApiCall(mockCharacters);
    }
    const response = await fetch(`${API_BASE_URL}/characters`);
    return response.json();
  },

  async fetchSkills(): Promise<Skill[]> {
    if (USE_MOCK_SERVER) {
      return mockApiCall(mockSkills);
    }
    const response = await fetch(`${API_BASE_URL}/skills`);
    return response.json();
  },

  async authenticateUser(token: string): Promise<void> {
    if (USE_MOCK_SERVER) {
      return mockApiCall(undefined);
    }
    await fetch(`${API_BASE_URL}/auth`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ token }),
    });
  },

  async saveGameState(gameId: string, state: GameState): Promise<void> {
    if (USE_MOCK_SERVER) {
      return mockApiCall(undefined);
    }
    await fetch(`${API_BASE_URL}/games/${gameId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(state),
    });
  },
};