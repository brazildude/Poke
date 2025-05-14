import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Shield, Loader2 } from 'lucide-react';
import { Team } from '../types/game';
import { gameService } from '../services/gameService';

export function TeamSelect() {
  const navigate = useNavigate();
  const [teams, setTeams] = useState<Team[]>([]);
  const [selectedTeamId, setSelectedTeamId] = useState<string | null>(null);
  const [isQueuing, setIsQueuing] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadTeams();
  }, []);

  useEffect(() => {
    let statusInterval: NodeJS.Timeout;
    
    if (isQueuing) {
      statusInterval = setInterval(checkMatchStatus, 2000);
    }

    return () => {
      if (statusInterval) {
        clearInterval(statusInterval);
      }
    };
  }, [isQueuing]);

  const loadTeams = async () => {
    try {
      const fetchedTeams = await gameService.getTeams();
      setTeams(fetchedTeams);
      setIsLoading(false);
    } catch (err) {
      setError('Failed to load teams. Please try again.');
      setIsLoading(false);
    }
  };

  const startMatchmaking = async () => {
    if (!selectedTeamId) return;

    try {
      setIsQueuing(true);
      await gameService.startMatchmaking(selectedTeamId);
    } catch (err) {
      setError('Failed to start matchmaking. Please try again.');
      setIsQueuing(false);
    }
  };

  const checkMatchStatus = async () => {
    try {
      const status = await gameService.checkMatchmakingStatus();
      if (status.status === 'matched' && status.gameId) {
        setIsQueuing(false);
        navigate(`/game/${status.gameId}`);
      }
    } catch (err) {
      console.error('Error checking match status:', err);
    }
  };

  const cancelMatchmaking = async () => {
    try {
      await gameService.cancelMatchmaking();
      setIsQueuing(false);
    } catch (err) {
      setError('Failed to cancel matchmaking. Please try again.');
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-900 text-white flex items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-blue-500" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-900 text-white p-8">
      <div className="max-w-4xl mx-auto">
        <div className="text-center mb-8">
          <Shield className="w-16 h-16 text-blue-500 mx-auto mb-4" />
          <h1 className="text-3xl font-bold mb-2">Choose Your Team</h1>
          <p className="text-gray-400">Select a team to enter matchmaking</p>
        </div>

        {error && (
          <div className="bg-red-500 bg-opacity-10 border border-red-500 text-red-500 px-4 py-3 rounded-lg text-center mb-6">
            {error}
          </div>
        )}

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
          {teams.map(team => (
            <div
              key={team.id}
              onClick={() => setSelectedTeamId(team.id)}
              className={`
                p-6 rounded-lg border-2 cursor-pointer transition-all
                ${selectedTeamId === team.id
                  ? 'border-blue-500 bg-blue-500 bg-opacity-20'
                  : 'border-gray-700 hover:border-gray-600 bg-gray-800'
                }
              `}
            >
              <div className="flex items-center mb-4">
                <img
                  src={team.icon}
                  alt={`${team.name} icon`}
                  className="w-12 h-12 rounded-full mr-4"
                />
                <h3 className="text-xl font-bold">{team.name}</h3>
              </div>
              
              <div className="space-y-2">
                {team.characters.map((character, index) => (
                  <div key={index} className="flex items-center text-gray-300">
                    <span className="w-6 text-center mr-2">
                      {character.class === 'warrior' ? '⚔️' : 
                       character.class === 'mage' ? '🔮' : 
                       character.class === 'healer' ? '✚' :
                       character.class === 'archer' ? '🏹' :
                       character.class === 'assassin' ? '🗡️' : '🛡️'}
                    </span>
                    <span className="flex-1">{character.name}</span>
                    <span className="text-sm text-gray-500 capitalize">{character.class}</span>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>

        <div className="flex justify-center space-x-4">
          {!isQueuing ? (
            <button
              onClick={startMatchmaking}
              disabled={!selectedTeamId}
              className={`
                px-6 py-3 rounded-lg font-semibold transition-all
                ${!selectedTeamId
                  ? 'bg-gray-700 cursor-not-allowed'
                  : 'bg-blue-600 hover:bg-blue-700'
                }
              `}
            >
              Start Matchmaking
            </button>
          ) : (
            <>
              <div className="flex items-center bg-blue-500 bg-opacity-20 px-6 py-3 rounded-lg">
                <Loader2 className="animate-spin h-5 w-5 mr-3" />
                <span>Finding match...</span>
              </div>
              <button
                onClick={cancelMatchmaking}
                className="px-6 py-3 rounded-lg font-semibold bg-red-600 hover:bg-red-700 transition-all"
              >
                Cancel
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
}