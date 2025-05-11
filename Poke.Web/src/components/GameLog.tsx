import React, { useEffect, useRef } from 'react';
import { Move } from '../types/game';

interface GameLogProps {
  moves: Move[];
}

const GameLog: React.FC<GameLogProps> = ({ moves }) => {
  const logEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (logEndRef.current) {
      logEndRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [moves]);

  const formatMove = (move: Move) => {
    return `Player ${move.playerId} used character ${move.characterId} to target ${move.targets.join(', ')}`;
  };

  return (
    <div className="w-full h-64 bg-gray-900 rounded-lg overflow-hidden flex flex-col">
      <div className="p-2 bg-gray-800 border-b border-gray-700">
        <h3 className="text-sm font-medium text-gray-200">Battle Log</h3>
      </div>
      <div className="flex-1 overflow-y-auto p-3 space-y-2 text-sm">
        {moves.map((move) => (
          <div 
            key={move.id} 
            className="animate-fadeIn py-1 px-2 rounded text-gray-300"
          >
            <span className="text-gray-500 text-xs mr-2">
              {new Date(move.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
            </span>
            {formatMove(move)}
          </div>
        ))}
        <div ref={logEndRef} />
      </div>
    </div>
  );
};

export default GameLog;