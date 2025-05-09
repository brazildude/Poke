import { Character, Skill, GameState } from '../types/game';
import { mockCharacters, mockSkills } from '../data/mockServerData';

export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://api.example.com';
export const USE_MOCK_SERVER = import.meta.env.VITE_USE_MOCK_SERVER === 'true';
const MOCK_API_DELAY = 500;

async function mockApiCall<T>(mockData: T): Promise<T> {
  await new Promise(resolve => setTimeout(resolve, MOCK_API_DELAY));
  return mockData;
}

export const serverConfig = {
  API_BASE_URL,
  
  async fetchCharacters(): Promise<Character[]> {
    if (USE_MOCK_SERVER) {
      return mockApiCall(mockCharacters);
    }
    const response = await fetch(`${API_BASE_URL}/characters`);
    if (!response.ok) {
      throw new Error('Failed to fetch characters');
    }
    return response.json();
  },

  async fetchSkills(): Promise<Skill[]> {
    if (USE_MOCK_SERVER) {
      return mockApiCall(mockSkills);
    }
    const response = await fetch(`${API_BASE_URL}/skills`);
    if (!response.ok) {
      throw new Error('Failed to fetch skills');
    }
    return response.json();
  },

  async authenticateUser(token: string): Promise<void> {
    if (USE_MOCK_SERVER) {
      return mockApiCall(undefined);
    }
    const response = await fetch(`${API_BASE_URL}/auth`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ token })
    });
    
    if (!response.ok) {
      throw new Error('Authentication failed');
    }
  }
};