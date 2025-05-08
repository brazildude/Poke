import React, { useEffect, useRef } from 'react';
import { LogEntry } from '../types/game';

interface GameLogProps {
  entries: LogEntry[];
}

const GameLog: React.FC<GameLogProps> = ({ entries }) => {
  const logEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (logEndRef.current) {
      logEndRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [entries]);

  const getLogTypeStyles = (type: LogEntry['type']) => {
    switch (type) {
      case 'attack':
        return 'text-red-400';
      case 'heal':
        return 'text-green-400';
      case 'buff':
        return 'text-blue-400';
      case 'debuff':
        return 'text-purple-400';
      case 'system':
        return 'text-yellow-300';
      default:
        return 'text-gray-300';
    }
  };

  return (
    <div className="w-full h-64 bg-gray-900 rounded-lg overflow-hidden flex flex-col">
      <div className="p-2 bg-gray-800 border-b border-gray-700">
        <h3 className="text-sm font-medium text-gray-200">Battle Log</h3>
      </div>
      <div className="flex-1 overflow-y-auto p-3 space-y-2 text-sm">
        {entries.map((entry) => (
          <div 
            key={entry.id} 
            className={`animate-fadeIn py-1 px-2 rounded ${getLogTypeStyles(entry.type)}`}
          >
            <span className="text-gray-500 text-xs mr-2">
              {new Date(entry.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
            </span>
            {entry.text}
          </div>
        ))}
        <div ref={logEndRef} />
      </div>
    </div>
  );
};

export default GameLog;