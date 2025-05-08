import React from 'react';
import { Character } from '../types/game';

interface CharacterPortraitProps {
  character: Character;
  isPlayerTurn: boolean;
  onSelectCharacter: (id: string) => void;
}

const CharacterPortrait: React.FC<CharacterPortraitProps> = ({ 
  character, 
  isPlayerTurn,
  onSelectCharacter 
}) => {
  const healthPercentage = (character.health / character.maxHealth) * 100;
  const healthColor = healthPercentage > 60 
    ? 'bg-green-500' 
    : healthPercentage > 30 
      ? 'bg-yellow-500' 
      : 'bg-red-500';

  return (
    <div 
      className={`relative p-2 rounded-lg ${character.isActive && isPlayerTurn ? 'ring-2 ring-yellow-400 shadow-lg' : ''} 
      transition-all duration-300 h-36 w-28 flex flex-col bg-gray-800 ${isPlayerTurn ? 'cursor-pointer hover:ring-1 hover:ring-blue-400' : ''}`}
      onClick={(e) => {
        e.stopPropagation();
        onSelectCharacter(character.id);
      }}
    >
      <div className="relative overflow-hidden h-20 rounded flex items-center justify-center bg-gray-700">
        <div className={`absolute inset-0 flex items-center justify-center text-3xl font-bold 
        ${getClassColor(character.class)}`}>
          {getClassIcon(character.class)}
        </div>
        
        {character.status.length > 0 && (
          <div className="absolute top-0 right-0 flex space-x-1 p-1">
            {character.status.map((status) => (
              <div 
                key={status.id} 
                className={`h-4 w-4 rounded-full ${status.type === 'buff' ? 'bg-blue-500' : 'bg-red-500'}`}
                title={`${status.name}: ${status.description} (${status.duration} turns)`}
              />
            ))}
          </div>
        )}
      </div>

      <div className="mt-2 text-center">
        <div className="text-xs font-medium text-white truncate">{character.name}</div>
        <div className="text-[10px] text-gray-300">{character.class}</div>
      </div>

      <div className="mt-1 h-2 w-full bg-gray-700 rounded-full overflow-hidden">
        <div 
          className={`h-full ${healthColor} transition-all duration-500 ease-out`}
          style={{ width: `${healthPercentage}%` }}
        />
      </div>
      
      <div className="mt-1 text-[10px] text-gray-300 text-center">
        {character.health}/{character.maxHealth}
      </div>
    </div>
  );
};

function getClassColor(characterClass: string): string {
  switch (characterClass) {
    case 'warrior': return 'text-red-500';
    case 'mage': return 'text-blue-500';
    case 'healer': return 'text-green-500';
    case 'archer': return 'text-yellow-500';
    case 'assassin': return 'text-purple-500';
    case 'tank': return 'text-gray-400';
    default: return 'text-white';
  }
}

function getClassIcon(characterClass: string): string {
  switch (characterClass) {
    case 'warrior': return '⚔️';
    case 'mage': return '🔮';
    case 'healer': return '✚';
    case 'archer': return '🏹';
    case 'assassin': return '🗡️';
    case 'tank': return '🛡️';
    default: return '?';
  }
}

export default CharacterPortrait;