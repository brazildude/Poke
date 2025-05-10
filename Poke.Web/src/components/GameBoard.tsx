import React, { useState, useEffect } from 'react';
import CharacterList from './CharacterList';
import GameLog from './GameLog';
import SkillBar from './SkillBar';
import TurnIndicator from './TurnIndicator';
import { useGame } from '../context/GameContext';
import { Move } from '../types/game';

const GameBoard: React.FC = () => {
  const { state, selectCharacter, makeMove, endTurn, clearSelection } = useGame();
  const [selectedSkillId, setSelectedSkillId] = useState<string | null>(null);
  const [targetingMode, setTargetingMode] = useState(false);
  const [moves, setMoves] = useState<Move[]>([]);
  
  const currentPlayer = state.players.find(p => p.id === state.currentPlayerId);
  const otherPlayer = state.players.find(p => p.id !== state.currentPlayerId);
  const activeCharacter = state.players
    .flatMap(p => p.characters)
    .find(c => c.id === state.activeCharacterId);
  
  useEffect(() => {
    if (state.lastMove) {
      setMoves(prevMoves => [...prevMoves, state.lastMove!]);
    }
  }, [state.lastMove]);

  if (!currentPlayer || !otherPlayer || !activeCharacter) {
    return <div>Loading game...</div>;
  }
  
  const handleCharacterSelect = (characterId: string) => {
    if (targetingMode && selectedSkillId) {
      handleTargetSelect(characterId);
    } else {
      // Only allow selecting characters of the current player when not targeting
      const character = currentPlayer.characters.find(c => c.id === characterId);
      if (character) {
        selectCharacter(characterId);
      }
    }
  };
  
  const handleSkillSelect = (skillId: string) => {
    const skill = activeCharacter.skills.find(s => s.id === skillId);
    if (skill && !activeCharacter.usedSkillThisTurn && skill.currentCooldown === 0) {
      setSelectedSkillId(skillId);
      setTargetingMode(true);
    }
  };
  
  const handleTargetSelect = async (targetId: string) => {
    if (!targetingMode || !selectedSkillId) return;

    const skill = activeCharacter.skills.find(s => s.id === selectedSkillId);
    if (!skill) return;

    // Check if the target is valid based on skill type
    const isTargetFriendly = currentPlayer.characters.some(c => c.id === targetId);
    const isHealingSkill = skill.type === 'heal';

    if ((isHealingSkill && isTargetFriendly) || (!isHealingSkill && !isTargetFriendly)) {
      try {
        await makeMove(activeCharacter.id, [targetId], selectedSkillId);
        setTargetingMode(false);
        setSelectedSkillId(null);
      } catch (error) {
        console.error('Failed to make move:', error);
      }
    }
  };
  
  const handleCancelTargeting = () => {
    setTargetingMode(false);
    setSelectedSkillId(null);
    clearSelection();
  };
  
  return (
    <div className="min-h-screen bg-gray-900 text-white p-4 flex flex-col">
      <TurnIndicator 
        currentPlayer={currentPlayer.name} 
        turnNumber={state.turnNumber} 
      />
      
      {/* Top player (opponent) */}
      <CharacterList 
        characters={otherPlayer.characters} 
        isCurrentTurn={otherPlayer.isCurrentTurn}
        position="top"
        onSelectCharacter={handleCharacterSelect}
      />
      
      {/* Game log in the middle */}
      <div className="flex-1 my-4">
        <GameLog moves={moves} />
      </div>
      
      {/* Bottom player (current user) */}
      <CharacterList 
        characters={currentPlayer.characters} 
        isCurrentTurn={currentPlayer.isCurrentTurn}
        position="bottom"
        onSelectCharacter={handleCharacterSelect}
      />
      
      {/* Skills for the active character */}
      <SkillBar 
        skills={activeCharacter.skills} 
        onUseSkill={handleSkillSelect} 
        isActive={currentPlayer.isCurrentTurn && !activeCharacter.usedSkillThisTurn}
      />
      
      {/* Game controls */}
      <div className="flex justify-center mt-2">
        <button 
          onClick={() => endTurn()}
          className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
        >
          End Turn
        </button>
        
        {targetingMode && (
          <button 
            onClick={handleCancelTargeting}
            className="ml-2 px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg transition-colors"
          >
            Cancel
          </button>
        )}
      </div>
      
      {/* Targeting mode indicator */}
      {targetingMode && selectedSkillId && (
        <div className="fixed top-16 left-1/2 transform -translate-x-1/2 bg-red-600 py-1 px-4 rounded-full shadow-lg">
          <div className="text-sm font-medium text-white flex items-center">
            <span className="mr-2">
              Select a target 
              {activeCharacter.skills.find(s => s.id === selectedSkillId)?.type === 'heal' 
                ? '(friendly unit)' 
                : '(enemy unit)'}
            </span>
            <div className="h-2 w-2 rounded-full bg-white animate-ping"></div>
          </div>
        </div>
      )}
    </div>
  );
};

export default GameBoard