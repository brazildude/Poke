import { Character, Skill } from '../types/game';

export const mockCharacters: Character[] = [
  {
    id: 'p1c1',
    name: 'Aric',
    health: 100,
    maxHealth: 100,
    class: 'warrior',
    status: [],
    isActive: false,
    usedSkillThisTurn: false,
    skills: []
  },
  {
    id: 'p1c2',
    name: 'Eliza',
    health: 80,
    maxHealth: 80,
    class: 'mage',
    status: [],
    isActive: false,
    usedSkillThisTurn: false,
    skills: []
  },
  {
    id: 'p1c3',
    name: 'Lyra',
    health: 70,
    maxHealth: 70,
    class: 'healer',
    status: [],
    isActive: false,
    usedSkillThisTurn: false,
    skills: []
  },
  {
    id: 'p2c1',
    name: 'Darian',
    health: 90,
    maxHealth: 90,
    class: 'archer',
    status: [],
    isActive: false,
    usedSkillThisTurn: false,
    skills: []
  },
  {
    id: 'p2c2',
    name: 'Vex',
    health: 75,
    maxHealth: 75,
    class: 'assassin',
    status: [],
    isActive: false,
    usedSkillThisTurn: false,
    skills: []
  },
  {
    id: 'p2c3',
    name: 'Korg',
    health: 120,
    maxHealth: 120,
    class: 'tank',
    status: [],
    isActive: false,
    usedSkillThisTurn: false,
    skills: []
  }
];

export const mockSkills: Skill[] = [
  {
    id: 'slash',
    name: 'Slash',
    description: 'A basic attack',
    damage: 15,
    cooldown: 0,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'warrior'
  },
  {
    id: 'heavyStrike',
    name: 'Heavy Strike',
    description: 'A powerful attack',
    damage: 30,
    cooldown: 2,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'warrior'
  },
  {
    id: 'defend',
    name: 'Defend',
    description: 'Increase defense for 2 turns',
    cooldown: 3,
    currentCooldown: 0,
    type: 'buff',
    targetType: 'self',
    characterClass: 'warrior'
  },
  {
    id: 'fireball',
    name: 'Fireball',
    description: 'Launch a ball of fire',
    damage: 25,
    cooldown: 1,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'mage'
  },
  {
    id: 'frostbolt',
    name: 'Frostbolt',
    description: 'Slow and damage the enemy',
    damage: 15,
    cooldown: 2,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'mage'
  },
  {
    id: 'arcaneShield',
    name: 'Arcane Shield',
    description: 'Block the next attack',
    cooldown: 4,
    currentCooldown: 0,
    type: 'buff',
    targetType: 'single',
    characterClass: 'mage'
  },
  {
    id: 'heal',
    name: 'Heal',
    description: 'Restore health to an ally',
    healing: 20,
    cooldown: 1,
    currentCooldown: 0,
    type: 'heal',
    targetType: 'single',
    characterClass: 'healer'
  },
  {
    id: 'groupHeal',
    name: 'Group Heal',
    description: 'Heal all allies',
    healing: 10,
    cooldown: 3,
    currentCooldown: 0,
    type: 'heal',
    targetType: 'all',
    characterClass: 'healer'
  },
  {
    id: 'smite',
    name: 'Smite',
    description: 'Attack an enemy',
    damage: 10,
    cooldown: 0,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'healer'
  },
  {
    id: 'quickShot',
    name: 'Quick Shot',
    description: 'A fast attack',
    damage: 12,
    cooldown: 0,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'archer'
  },
  {
    id: 'poisonArrow',
    name: 'Poison Arrow',
    description: 'Poison the target',
    damage: 8,
    cooldown: 2,
    currentCooldown: 0,
    type: 'debuff',
    targetType: 'single',
    characterClass: 'archer'
  },
  {
    id: 'volley',
    name: 'Volley',
    description: 'Attack all enemies',
    damage: 15,
    cooldown: 3,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'all',
    characterClass: 'archer'
  },
  {
    id: 'backstab',
    name: 'Backstab',
    description: 'A sneaky attack',
    damage: 20,
    cooldown: 1,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'assassin'
  },
  {
    id: 'poisonDagger',
    name: 'Poison Dagger',
    description: 'Poison the target',
    damage: 10,
    cooldown: 2,
    currentCooldown: 0,
    type: 'debuff',
    targetType: 'single',
    characterClass: 'assassin'
  },
  {
    id: 'stealth',
    name: 'Stealth',
    description: 'Become invisible for 1 turn',
    cooldown: 4,
    currentCooldown: 0,
    type: 'buff',
    targetType: 'self',
    characterClass: 'assassin'
  },
  {
    id: 'shieldBash',
    name: 'Shield Bash',
    description: 'A basic attack',
    damage: 10,
    cooldown: 0,
    currentCooldown: 0,
    type: 'attack',
    targetType: 'single',
    characterClass: 'tank'
  },
  {
    id: 'taunt',
    name: 'Taunt',
    description: 'Force enemy to attack you',
    cooldown: 2,
    currentCooldown: 0,
    type: 'debuff',
    targetType: 'single',
    characterClass: 'tank'
  },
  {
    id: 'bulwark',
    name: 'Bulwark',
    description: 'Reduce damage taken',
    cooldown: 3,
    currentCooldown: 0,
    type: 'buff',
    targetType: 'self',
    characterClass: 'tank'
  }
];