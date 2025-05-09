export type CharacterClass = 'warrior' | 'mage' | 'healer' | 'archer' | 'assassin' | 'tank';
export type TargetType = 'single' | 'multiple' | 'random' | 'all' | 'self';

export interface Character {
  id: string;
  name: string;
  health: number;
  maxHealth: number;
  class: CharacterClass;
  skills: Skill[];
  status: StatusEffect[];
  isActive: boolean;
  usedSkillThisTurn: boolean;
}

export interface Skill {
  id: string;
  name: string;
  description: string;
  damage?: number;
  healing?: number;
  cooldown: number;
  currentCooldown: number;
  type: 'attack' | 'heal' | 'buff' | 'debuff';
  targetType: TargetType;
  targetCount?: number;
  characterClass: CharacterClass;
}

export interface StatusEffect {
  id: string;
  name: string;
  description: string;
  duration: number;
  type: 'buff' | 'debuff';
}

export interface Player {
  id: string;
  name: string;
  characters: Character[];
  isCurrentTurn: boolean;
}

export interface LogEntry {
  id: string;
  timestamp: number;
  text: string;
  type: 'attack' | 'heal' | 'buff' | 'debuff' | 'system';
  sourceCharacterId?: string;
  targetCharacterId?: string;
}

export interface GameState {
  id: string; // Add ID to GameState interface
  players: Player[];
  currentPlayerId: string;
  activeCharacterId: string;
  log: LogEntry[];
  turnNumber: number;
  gameStatus: 'waiting' | 'in-progress' | 'completed';
  selectedTargets: string[];
}

export interface GameData {
  id: string;
  createdAt: number;
  updatedAt: number;
  players: Player[];
  state: GameState;
  winnerId?: string;
}