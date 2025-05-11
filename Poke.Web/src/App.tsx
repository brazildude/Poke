import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { GameProvider } from './context/GameContext';
import { AuthProvider } from './context/AuthContext';
import { AuthGuard } from './components/AuthGuard';
import GameBoard from './components/GameBoard';
import { Login } from './pages/Login';
import { Signup } from './pages/Signup';
import { TeamSelect } from './pages/TeamSelect';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/signup" element={<Signup />} />
          <Route
            path="/team-select"
            element={
              <AuthGuard>
                <TeamSelect />
              </AuthGuard>
            }
          />
          <Route
            path="/"
            element={
              <AuthGuard>
                <div className="min-h-screen bg-gray-900">
                  <GameProvider>
                    <GameBoard />
                  </GameProvider>
                </div>
              </AuthGuard>
            }
          />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;