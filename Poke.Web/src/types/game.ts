import { CharacterClass } from './game';

export interface Team {
  teamID: number;
  name: string;
  units: string[];
}

export type CharacterClass = 'warrior' | 'mage' | 'healer' | 'archer' | 'assassin' | 'tank';
export type TargetType = 'single' | 'multiple' | 'random' | 'all' | 'self';

export interface Character {
  unitID: number;
  name: string;
  life: number;
  mana: number;
  isAbleToAttack: boolean;
  skills: Skill[];
  status: StatusEffect[];
  isActive: boolean;
  usedSkillThisTurn: boolean;
}

export interface Skill {
  skillID: number;
  name: string;
  description: string;
  applyValue: {
    minValue: number;
    maxValue: number;
    type: number;
    toProperty: number;
  };
  totalCooldown: number;
  currentCooldown: number;
  target: {
    targetType: number;
    targetDirection: number;
    quantity: number | null;
  };
}

export interface StatusEffect {
  id: string;
  name: string;
  description: string;
  duration: number;
  type: 'buff' | 'debuff';
}

export interface Player {
  userID: string;
  name: string | null;
  email: string | null;
  characters: Character[];
  isCurrentTurn: boolean;
}

export interface PlayMove {
  matchID: number;
  unitID: number;
  skillID: number;
  targetIDs: number[];
}

export interface GameState {
  matchID: number;
  players: Player[];
  currentPlayerId: string;
  activeCharacterId: string;
  turnNumber: number;
  gameStatus: 'waiting' | 'in-progress' | 'completed';
  selectedTargets: string[];
  lastMove?: PlayMove;
}

export interface GameData {
  matchID: number;
  createdAt: number;
  updatedAt: number;
  players: Player[];
  state: GameState;
  winnerId?: string;
  lastMove?: PlayMove;
}