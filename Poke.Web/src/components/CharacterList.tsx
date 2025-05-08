import React from 'react';
import { Character } from '../types/game';
import CharacterPortrait from './CharacterPortrait';

interface CharacterListProps {
  characters: Character[];
  isCurrentTurn: boolean;
  position: 'top' | 'bottom';
  onSelectCharacter: (id: string) => void;
}

const CharacterList: React.FC<CharacterListProps> = ({ 
  characters, 
  isCurrentTurn, 
  position,
  onSelectCharacter
}) => {
  return (
    <div className={`w-full flex justify-center ${position === 'top' ? 'mb-4' : 'mt-4'}`}>
      <div className="flex space-x-4 items-center">
        {characters.map((character) => (
          <div 
            key={character.id} 
            onClick={() => onSelectCharacter(character.id)}
            className={`transform transition-transform duration-300 cursor-pointer ${
              character.isActive ? (position === 'top' ? 'translate-y-2' : '-translate-y-2') : ''
            }`}
          >
            <CharacterPortrait 
              character={character} 
              isPlayerTurn={isCurrentTurn}
              onSelectCharacter={onSelectCharacter} 
            />
          </div>
        ))}
      </div>
    </div>
  );
};

export default CharacterList;