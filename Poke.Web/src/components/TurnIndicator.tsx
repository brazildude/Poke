import React from 'react';

interface TurnIndicatorProps {
  currentPlayer: string;
  turnNumber: number;
}

const TurnIndicator: React.FC<TurnIndicatorProps> = ({ currentPlayer, turnNumber }) => {
  return (
    <div className="fixed top-2 left-1/2 transform -translate-x-1/2 bg-gray-800 bg-opacity-90 py-1 px-4 rounded-full shadow-lg">
      <div className="flex items-center space-x-2">
        <div className="text-xs text-gray-400">Turn {turnNumber}</div>
        <div className="h-3 w-3 rounded-full bg-yellow-400 animate-pulse"></div>
        <div className="text-sm font-medium text-white">{currentPlayer}'s Turn</div>
      </div>
    </div>
  );
};

export default TurnIndicator;